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

/// @file SCFF.GUI/Controls/Utilities.cs
/// SCFF.GUI.Controls共通の機能をまとめたクラス

namespace SCFF.GUI.Controls {

using SCFF.Common;
using SCFF.Common.Ext;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

/// SCFF.GUI.Controls共通の機能をまとめたクラス
public static class Utilities {
  private const int MaxBitmapWidth = 640;
  private const int MaxBitmapHeight = 480;

  private static void CalcScale(int width, int height, out double scaleX, out double scaleY) {
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

  public static BitmapSource ScreenCapture(Profile.InputLayoutElement layoutElement) {
    // 返り値
    BitmapSource result = null;

    // Windowチェック
    var window = layoutElement.Window;
    if (!User32.IsWindow(window)) return result;

    // キャプチャ用の情報をまとめる
    var x = layoutElement.WindowType == WindowTypes.Desktop ? layoutElement.ScreenClippingXWithFit
                                                            : layoutElement.ClippingXWithFit;
    var y = layoutElement.WindowType == WindowTypes.Desktop ? layoutElement.ScreenClippingYWithFit
                                                            : layoutElement.ClippingYWithFit;

    var width = layoutElement.ClippingWidthWithFit;
    var height = layoutElement.ClippingHeightWithFit;
    var rasterOperation = GDI32.SRCCOPY;
    if (layoutElement.ShowLayeredWindow) rasterOperation |= GDI32.CAPTUREBLT;

    /// @todo(me) マウスカーソルの合成・・・？いるか？

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