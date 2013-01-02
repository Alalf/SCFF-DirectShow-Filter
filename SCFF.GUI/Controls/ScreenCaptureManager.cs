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
  using System.Runtime.InteropServices;
  using System.Threading;
  using System.Windows;
  using System.Windows.Interop;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;

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

/// スクリーンキャプチャ結果
/// @warning 32bit限定
public class ScreenCaptureResult {
  public ScreenCaptureResult (int pixelWidth, int pixelHeight) {
    this.PixelWidth = pixelWidth;
    this.PixelHeight = pixelHeight;
    this.Pixels = new byte[this.Size];
  }

  public byte[] Pixels { get; private set; }
  public int PixelWidth { get; private set; }
  public int PixelHeight { get; private set; }
  public double DpiX { get { return 96.0; } }
  public double DpiY { get { return 96.0; } }
  public PixelFormat Format { get { return PixelFormats.Bgr32; } }
  public BitmapPalette Palette { get { return null; } }

  public GDI32.BITMAPINFO CreateBitmapInfo() {
    var info = new GDI32.BITMAPINFO();
    info.bmiColors = new uint[1];
    var header = new GDI32.BITMAPINFOHEADER();
    header.biBitCount = 32;
    // header.biBitCount = (ushort)this.Format.BitsPerPixel;
    header.biWidth = this.PixelWidth;
    header.biHeight = -1 * this.PixelHeight;
    header.biSizeImage = (uint)this.Size;
    header.biPlanes = 1;
    header.biCompression = GDI32.BI_RGB;
    header.biSize = (uint)Marshal.SizeOf(header);
    info.bmih = header;
    return info;
  }

  public int Stride {
    get { return this.PixelWidth * 4; }
  }
  public int Size {
    get { return this.Stride * this.PixelHeight; }
  }
}

/// スクリーンキャプチャデータを取得するためのスレッド管理クラス
public class ScreenCaptureManager {
  /// タイマーの間隔
  private const int TimerPeriod = 3000;

  /// タイマー
  private Timer captureTimer = null;

  //-------------------------------------------------------------------
  // 複数のスレッドで共有するデータ
  //-------------------------------------------------------------------

  /// 共有ロック
  private readonly object sharedLock = new Object();

  /// 共有(自R/他W): 終了したかどうか
  private bool isTerminated = false;
  
  /// 共有(自R/他W): スクリーンキャプチャのリクエストをまとめた配列
  private ScreenCaptureRequest[] requests =
      new ScreenCaptureRequest[Constants.MaxLayoutElementCount];

  /// 共有(自W/他R): スクリーンキャプチャの結果をまとめた配列
  private ScreenCaptureResult[] results =
      new ScreenCaptureResult[Constants.MaxLayoutElementCount];

  //-------------------------------------------------------------------

  /// コンストラクタ
  public ScreenCaptureManager() {
    // nop
  }

  //-------------------------------------------------------------------
  // タイマー関連
  //-------------------------------------------------------------------

  /// 開始
  public void Start() {
    Debug.WriteLine("ScreenCaptureManager: Start");
    this.captureTimer = new Timer(TimerCallback, null, 0, TimerPeriod);
  }

  /// タイマーコールバック
  private void TimerCallback(object state) {
    lock (this.sharedLock) {
      if (this.isTerminated) return;
      if (!App.Options.LayoutPreview) return;
      /// @todo(me) メモリがもったいないので何かしたい
      /// @todo(me) レイアウトタブが見えてなければキャプチャはしない
      //if (!App.Options.TmpLayoutIsExpanded) return;

      // キャプチャ
      foreach (var request in this.requests) {
        if (request == null) continue;
        this.results[request.Index] = this.ScreenCapture(request);
      }
    }
  }

  /// スレッド終了
  /// @warning DispatcherShutdownなどで必ず呼ぶこと
  public void End() {
    Debug.WriteLine("ScreenCaptureManager: End");
    lock (this.sharedLock) {
      this.isTerminated = true;
      if (this.captureTimer != null) {
        this.captureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.captureTimer.Dispose();
        this.captureTimer = null;
      }
    }
  }

  /// スクリーンキャプチャをこのマネージャに依頼
  /// @todo(me) デフォルトパラメータは使用しないように
  public void SendRequest(ScreenCaptureRequest request, bool forceUpdate = false) {
    Debug.WriteLine("ScreenCaptureManager: Request Arrived");
    lock (this.sharedLock) {
      this.requests[request.Index] = request;
      if (forceUpdate) {
       this.results[request.Index] = this.ScreenCapture(request);
      }
    }
  }

  /// BitmapSourceを更新
  public BitmapSource CreateBitmapSource(int index) {
    //　参照カウンタがあるのなら消えはしない（GCの悪用！）
    var result = this.results[index];
    if (result == null) return null;

    return BitmapSource.Create(
        result.PixelWidth, result.PixelHeight,
        result.DpiX, result.DpiY,
        result.Format, result.Palette,
        result.Pixels, result.Stride);
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

  private ScreenCaptureResult ScreenCapture(ScreenCaptureRequest request) {
    // 返り値
    ScreenCaptureResult result = null;

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

    // resultのメモリ領域を確保
    result = new ScreenCaptureResult(width, height);
    var bitmapInfo = result.CreateBitmapInfo();

    // BitBlt
    var windowDC = User32.GetDC(window);
    var capturedDC = GDI32.CreateCompatibleDC(windowDC);
    var capturedBitmap = GDI32.CreateCompatibleBitmap(windowDC, width, height);
    {
      var originalBitmap = GDI32.SelectObject(capturedDC, capturedBitmap);
      GDI32.BitBlt(capturedDC, 0, 0, width, height, windowDC, x, y, rasterOperation);
      GDI32.GetDIBits(capturedDC, capturedBitmap, 0, (uint)result.PixelHeight, result.Pixels,
                      ref bitmapInfo, GDI32.DIB_RGB_COLORS);
      GDI32.SelectObject(capturedDC, originalBitmap);
    }
    User32.ReleaseDC(window, windowDC);
    GDI32.DeleteDC(capturedDC);
    GDI32.DeleteObject(capturedBitmap);
    GC.Collect();

    Debug.WriteLine("ScreenCaptureManager: Captured(DataSize: "+result.Size +")");
    return result;
  }
}
}   // namespace SCFF.GUI.Controls