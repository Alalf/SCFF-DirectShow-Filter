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
/// スクリーンキャプチャを取得するためのクラス

namespace SCFF.Common.GUI {

using SCFF.Common.Ext;
using System;

/// スクリーンキャプチャに必要なリクエスト(App.Profileを使いたくないので)
public class ScreenCaptureRequest {
  public ScreenCaptureRequest(int index, UIntPtr window,
      int clippingX, int clippingY, int clippingWidth, int clippingHeight,
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

  public int Index { get; private set; }
  public UIntPtr Window { get; private set; }
  public int ClippingX { get; private set; }
  public int ClippingY { get; private set; }
  public int ClippingWidth { get; private set; }
  public int ClippingHeight { get; private set; }
  public bool ShowCursor { get; private set; }
  public bool ShowLayeredWindow { get; private set; }
}

/// スクリーンキャプチャ結果(単なるHBitmapラッパー)
/// @warning 32bit限定
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

/// スクリーンキャプチャを取得するためのクラス
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