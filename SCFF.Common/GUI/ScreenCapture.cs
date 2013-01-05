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

/// @file SCFF.Common/GUI/ScreenCapture.cs
/// スクリーンキャプチャクラス

namespace SCFF.Common.GUI {

using System;
using SCFF.Common.Ext;

//=====================================================================
// ScreenCaptureRequest
//=====================================================================

/// スクリーンキャプチャに必要なデータをまとめたリクエストクラス
public class ScreenCaptureRequest {
  /// コンストラクタ
  public ScreenCaptureRequest(int index, UIntPtr window,
      int clippingX, int clippingY,
      int clippingWidth, int clippingHeight,
      bool showCursor, bool showLayeredWindow) {
    this.Index = index;
    this.Window = window;
    this.ClippingX = clippingX;
    this.ClippingY = clippingY;
    this.ClippingWidth = clippingWidth;
    this.ClippingHeight = clippingHeight;
    this.ShowCursor = showCursor;
    this.ShowLayeredWindow = showLayeredWindow;
  }

  /// レイアウト要素のIndex
  public int Index { get; private set; }
  /// Windowハンドル
  public UIntPtr Window { get; private set; }
  /// クリッピング領域の原点(x)
  public int ClippingX { get; private set; }
  /// クリッピング領域の原点(y)
  public int ClippingY { get; private set; }
  /// クリッピング領域の幅
  public int ClippingWidth { get; private set; }
  /// クリッピング領域の高さ
  public int ClippingHeight { get; private set; }
  /// マウスカーソルを取り込むか
  public bool ShowCursor { get; private set; }
  /// レイヤードウィンドウまで取り込むか
  public bool ShowLayeredWindow { get; private set; }
}

//=====================================================================
// ScreenCaptureResult
//=====================================================================

/// スクリーンキャプチャ結果を格納するHBitmapラッパークラス
public class ScreenCaptureResult : IDisposable {
  /// コンストラクタ
  public ScreenCaptureResult (IntPtr bitmap) {
    this.Bitmap = bitmap;
  }

  /// デストラクタ
  public void Dispose() {
    if (this.Bitmap != IntPtr.Zero) {
      GDI32.DeleteObject(this.Bitmap);
      this.Bitmap = IntPtr.Zero;
    }
  }

  /// プロパティ: HBitmap
  public IntPtr Bitmap { get; private set; }
}

//=====================================================================
// ScreenCapture
//=====================================================================

/// スクリーンキャプチャクラス
public static class ScreenCapture {
  /// スクリーンキャプチャした結果をHBitmapに格納する
  /// @warning 返り値はかならずDisposeするか、usingと一緒に使うこと
  public static ScreenCaptureResult Open(ScreenCaptureRequest request) {
    // Windowチェック
    var window = request.Window;
    if (!User32.IsWindow(window)) return null;

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
    User32.ReleaseDC(window, windowDC);
    GDI32.DeleteDC(capturedDC);

    return new ScreenCaptureResult(capturedBitmap);
  }
}
}   // namespace SCFF.Common.GUI