// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
//
// SCFF DSF is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCFF DSF is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SCFF DSF.  If not, see <http://www.gnu.org/licenses/>.

/// @file SCFF.GUI/Controls/ScreenCaptureManager.cs
/// スクリーンキャプチャデータを取得するためのスレッド管理クラス

namespace SCFF.GUI.Controls {

using SCFF.Common;
using SCFF.Common.Ext;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
  using System.Windows.Threading;

/// スクリーンキャプチャに必要なリクエスト(App.Profileを使いたくないので)
public class ScreenCaptureRequest {
  public int Index { get; set; }
  public UIntPtr Window { get; set; }
  public int ClippingX { get; set; }
  public int ClippingY { get; set; }
  public int ClippingWidth { get; set; }
  public int ClippingHeight { get; set; }
  public bool ShowCursor { get; set; }
  public bool ShowLayeredWindow { get; set; }
}

/// スクリーンキャプチャデータを取得するためのスレッド管理クラス
public class ScreenCaptureManager {
  /// タイマーの間隔
  private const int TimerPeriod = 1000;

  /// タイマー
  private DispatcherTimer captureTimer = null;

  //-------------------------------------------------------------------
  // 複数のスレッドで共有するデータ
  //-------------------------------------------------------------------

  /// 共有データ用ロックオブジェクト
  private readonly object sharedLock = new object();

  /// 共有(自R/他W): サスペンド中かどうか
  private bool isSuspended = false;
  
  /// 共有(自R/他W): スクリーンキャプチャのリクエストをまとめた配列
  private ScreenCaptureRequest[] captureRequests =
      new ScreenCaptureRequest[Constants.MaxLayoutElementCount];

  /// 共有(自W/他R): スクリーンキャプチャの結果をまとめた配列
  private BitmapSource[] capturedBitmaps =
      new BitmapSource[Constants.MaxLayoutElementCount];

  //-------------------------------------------------------------------

  /// コンストラクタ
  public ScreenCaptureManager() {
    // nop  
  }

  //-------------------------------------------------------------------
  // タイマー関連
  //-------------------------------------------------------------------

  /// スレッド開始
  public void Start() {
    this.captureTimer = new DispatcherTimer();
    this.captureTimer.Interval = TimeSpan.FromMilliseconds(TimerPeriod);
    this.captureTimer.Tick += captureTimer_Tick;
    this.captureTimer.Start();
  }

  /// タイマーはとめないがスクリーンキャプチャは止める
  public void Suspend() {
    lock (sharedLock) {
      this.isSuspended = true;
      // メモリがもったいないので開放しておく
      this.capturedBitmaps = new BitmapSource[Constants.MaxLayoutElementCount];
    }
  }
  
  /// スクリーンキャプチャを再開する
  public void Resume() {
    lock (sharedLock) {
      this.isSuspended = false;
    }
  }

  /// スレッド終了
  /// @warning DispatcherShutdownなどで必ず呼ぶこと
  public void End() {
    if (this.captureTimer != null) {
      this.captureTimer.Stop();
      this.isSuspended = true;
      this.captureTimer = null;
    }
  }

  /// スクリーンキャプチャをこのマネージャに依頼
  public void SendRequest(ScreenCaptureRequest request) {
    lock (sharedLock) {
      this.captureRequests[request.Index] = request;
      /// @todo(me) リクエストした瞬間にImageを一回取り込む
      // this.capturedBitmaps[request.Index] = this.ScreenCapture(request);
    }
  }

  public delegate void DrawCapturedBitmapDelegate(int index, DrawingContext dc, Rect rect);

  /// DrawingContextを利用して描画
  public void DrawCapturedBitmap(int index, DrawingContext dc, Rect rect) {
    lock (sharedLock) {
      if (this.capturedBitmaps[index] == null) return;
      dc.DrawImage(this.capturedBitmaps[index], rect);
    }
  }

  //-------------------------------------------------------------------
  // コールバックメソッド
  //-------------------------------------------------------------------
  void captureTimer_Tick(object sender, EventArgs e) {
    // BitmapSourceを更新中なのでlock
    lock (sharedLock) {
      Debug.WriteLine("Timer Tick....");
      if (this.isSuspended) return;
      foreach (var request in this.captureRequests) {
        if (request == null) continue;
        this.capturedBitmaps[request.Index] = this.ScreenCapture(request);
      }
    }
  }

  //-------------------------------------------------------------------
  // スクリーンキャプチャ用メソッド
  //-------------------------------------------------------------------
  
  private const int MaxBitmapWidth = 640;
  private const int MaxBitmapHeight = 480;

  private void CalcScale(int width, int height, out double scaleX, out double scaleY) {
    if (width <= MaxBitmapWidth && height <= MaxBitmapHeight) {
      // scaleの必要なし
      scaleX = 1.0;
      scaleY = 1.0;
      return;
    }
    // アスペクトを計算
    if (width < height) {
      // 縦長なのでまず縦を小さくしなければならない
      scaleY = (double)MaxBitmapHeight / height;
      scaleX = scaleY;
    } else {
      // 横長
      scaleX = (double)MaxBitmapWidth / width;
      scaleY = scaleX;
    }
  }

  private BitmapSource ScreenCapture(ScreenCaptureRequest request) {
    // 返り値
    BitmapSource result = null;

    // Windowチェック
    var window = request.Window;
    if (!User32.IsWindow(window)) return result;

    // キャプチャ用の情報をまとめる
    var x = request.ClippingX;
    var y = request.ClippingY;

    var width = request.ClippingWidth;
    var height = request.ClippingHeight;
    var rasterOperation = GDI32.SRCCOPY;
    if (request.ShowLayeredWindow) rasterOperation |= GDI32.CAPTUREBLT;
    if (request.ShowCursor) {
      /// @todo(me) マウスカーソルの合成・・・？いるか？
    }

    // BitBlt
    var windowDC = User32.GetDC(window);
    var capturedDC = GDI32.CreateCompatibleDC(windowDC);
    var capturedBitmap = GDI32.CreateCompatibleBitmap(windowDC, width, height);
    {
      var originalBitmap = GDI32.SelectObject(capturedDC, capturedBitmap);
      GDI32.BitBlt(capturedDC, 0, 0, width, height, windowDC, x, y, rasterOperation);
      GDI32.SelectObject(capturedDC, originalBitmap);
    }
    GDI32.DeleteDC(capturedDC);
    User32.ReleaseDC(window, windowDC);

    try {
      // HBitmapからコピー
      var rawBitmap = Imaging.CreateBitmapSourceFromHBitmap(capturedBitmap,
          IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
      // α情報を除去
      var noAlphaBitmap = new FormatConvertedBitmap(rawBitmap, PixelFormats.Bgr24, null, 0.0);
      // メモリ容量低減のために縮小
      double scaleX;
      double scaleY;
      CalcScale(width, height, out scaleX, out scaleY);
      Debug.WriteLine("scale: {0:F2}, {1:F2}", scaleX, scaleY);
      result = new TransformedBitmap(noAlphaBitmap, new ScaleTransform(scaleX, scaleY)); 
    } catch (Exception ex) {
      Debug.WriteLine("ScreenCapture: " + ex.Message);
    } finally {
      // 5秒に一回程度の更新なので、HDC/HBitmapは使いまわさないですぐに消す
      GDI32.DeleteObject(capturedBitmap);
      GC.Collect();
    }
    return result;
  }
}
}   // namespace SCFF.GUI.Controls