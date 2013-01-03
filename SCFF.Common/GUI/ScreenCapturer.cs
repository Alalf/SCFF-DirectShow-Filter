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

/// @file SCFF.Common/GUI/ScreenCapturer.cs
/// スクリーンキャプチャデータを取得するためのスレッド管理クラス

namespace SCFF.Common.GUI {

using SCFF.Common;
using SCFF.Common.Ext;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

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

  public GDI32.BITMAPINFO CreateBitmapInfo() {
    var info = new GDI32.BITMAPINFO();
    info.bmiColors = new uint[1];
    var header = new GDI32.BITMAPINFOHEADER();
    header.biBitCount = 32;
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
public class ScreenCapturer {
  /// タイマー
  private Timer captureTimer = null;

  /// タイマーの間隔
  private double timerPeriod;

  //-------------------------------------------------------------------
  // 複数のスレッドで共有するデータ
  //-------------------------------------------------------------------

  /// 共有ロック
  private readonly object sharedLock = new Object();

  /// 共有(自R/他W): 終了したかどうか
  private bool isTerminated = false;

  /// 共有(自R/他W): プレビュー表示中かどうか
  private bool isSuspended = false;
  
  /// 共有(自R/他W): スクリーンキャプチャのリクエストをまとめた配列
  private ScreenCaptureRequest[] requests =
      new ScreenCaptureRequest[Common.Constants.MaxLayoutElementCount];

  /// 共有(自W/他R): スクリーンキャプチャの結果をまとめた配列
  private ScreenCaptureResult[] results =
      new ScreenCaptureResult[Common.Constants.MaxLayoutElementCount];

  //-------------------------------------------------------------------

  /// コンストラクタ
  public ScreenCapturer(double timerPeriod) {
    this.timerPeriod = timerPeriod;
  }

  //-------------------------------------------------------------------
  // タイマー関連
  //-------------------------------------------------------------------

  /// 開始
  public void Start() {
    Debug.WriteLine("ScreenCapturer: Start");
    this.captureTimer = new Timer(TimerCallback, null, 0, (int)this.timerPeriod);
  }

  /// 中断
  public void Suspend() {
    lock (this.sharedLock) {
      this.isSuspended = true;
    }
  }

  /// 再開
  public void Resume() {
    lock (this.sharedLock) {
      this.isSuspended = false;
    }
  }

  /// タイマーコールバック
  private void TimerCallback(object state) {
    lock (this.sharedLock) {
      if (this.isTerminated) return;
      if (this.isSuspended) return;
      /// @todo(me) メモリがもったいないので何かしたい

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
    Debug.WriteLine("ScreenCapturer: End");
    lock (this.sharedLock) {
      this.isSuspended = true;
      this.isTerminated = true;
      if (this.captureTimer != null) {
        this.captureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.captureTimer.Dispose();
        Debug.WriteLine("ScreenCapturer: Timer is Disposed");
        this.captureTimer = null;
      }
    }
  }

  /// スクリーンキャプチャをこのマネージャに依頼
  /// @todo(me) デフォルトパラメータは使用しないように
  /// @todo(me) やばい。Requestを消す手段が無いから困った事になってる
  public void SendRequest(ScreenCaptureRequest request) {
    Debug.WriteLine("ScreenCapturer: Request Arrived");
    lock (this.sharedLock) {
      this.requests[request.Index] = request;
    }
  }

  public void ClearRequests() {
    Debug.WriteLine("ScreenCapturer: Clear All Requests");
    lock (this.sharedLock) {
      this.requests = new ScreenCaptureRequest[Common.Constants.MaxLayoutElementCount];
    }
  }

  /// 結果を取得
  public ScreenCaptureResult GetResult(int index) {
    //　参照カウンタがあるのなら消えはしない（GCの悪用！）
    return this.results[index];
  }

  //-------------------------------------------------------------------
  // スクリーンキャプチャ用メソッド
  //-------------------------------------------------------------------

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
      GDI32.SelectObject(capturedDC, originalBitmap);
    }
    User32.ReleaseDC(window, windowDC);

    // GetDIBits
    var dibitsResult = GDI32.GetDIBits(capturedDC, capturedBitmap, 0, (uint)result.PixelHeight, result.Pixels,
        ref bitmapInfo, GDI32.DIB_RGB_COLORS);
    Debug.Assert(dibitsResult == (uint)result.PixelHeight);

    GDI32.DeleteDC(capturedDC);
    GDI32.DeleteObject(capturedBitmap);
    GC.Collect();

    Debug.WriteLine("ScreenCapturer: {0:D}-Captured(DataSize: {1:D})", request.Index, result.Size);
    return result;
  }
}
}   // namespace SCFF.Common.GUI