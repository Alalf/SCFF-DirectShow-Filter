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

/// @file SCFF.Common/GUI/ScreenCaptureRequest.cs
/// @copydoc SCFF::Common::GUI::ScreenCaptureRequest

namespace SCFF.Common.GUI {

using System;
using SCFF.Common.Ext;

/// スクリーンキャプチャリクエスト(+スクリーンキャプチャ機能)
public class ScreenCaptureRequest {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public ScreenCaptureRequest(ILayoutElementView layoutElement) {
    this.Index = layoutElement.Index;
    this.Window = layoutElement.Window;
    this.ClippingX = layoutElement.ClippingXWithFit;
    this.ClippingY = layoutElement.ClippingYWithFit;
    this.ClippingWidth = layoutElement.ClippingWidthWithFit;
    this.ClippingHeight = layoutElement.ClippingHeightWithFit;
    this.ShowCursor = layoutElement.ShowCursor;
    this.ShowLayeredWindow = layoutElement.ShowLayeredWindow;
  }

  //===================================================================
  // スクリーンキャプチャ
  //===================================================================

  /// スクリーンキャプチャした結果をHBitmapに格納する
  /// @warning 返り値はかならずusingと一緒に使うかDispose()すること
  /// @return スクリーンキャプチャした結果のHBitmap
  public BitmapHandle Execute() {
    // Windowチェック
    var window = this.Window;
    if (window == UIntPtr.Zero || !User32.IsWindow(window)) return null;

    // BitBlt
    var windowDC = User32.GetDC(window);
    var capturedDC = GDI32.CreateCompatibleDC(windowDC);
    var capturedBitmap = GDI32.CreateCompatibleBitmap(windowDC,
        this.ClippingWidth, this.ClippingHeight);
    {
      var originalBitmap = GDI32.SelectObject(capturedDC, capturedBitmap);
      GDI32.BitBlt(capturedDC,
                   0, 0, this.ClippingWidth, this.ClippingHeight,
                   windowDC,
                   this.ClippingX, this.ClippingY,
                   this.ShowLayeredWindow ? GDI32.SRCCOPY | GDI32.CAPTUREBLT
                                          : GDI32.SRCCOPY);
      GDI32.SelectObject(capturedDC, originalBitmap);
    }    
    GDI32.DeleteDC(capturedDC);
    User32.ReleaseDC(window, windowDC);

    /// @todo(me) マウスカーソルの合成・・・？いるか？
    if (this.ShowCursor) {
      // nop
    }

    return new BitmapHandle(capturedBitmap);
  }

  //===================================================================
  // プロパティ
  //===================================================================

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
}   // namespace SCFF.Common.GUI