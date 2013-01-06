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

/// @file SCFF.GUI/Controls/ScreenCapture.cs
/// @copydoc SCFF::GUI::Controls::ScreenCapture

namespace SCFF.GUI.Controls {

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SCFF.Common.GUI;

/// スクリーンキャプチャデータをBitmapSource化
public static class ScreenCapture {
  //===================================================================
  // 定数
  //===================================================================

  private const int MaxBitmapWidth = 640;   ///< BitmapSourceの最大幅
  private const int MaxBitmapHeight = 480;  ///< BitmapSourceの最大高さ

  //===================================================================
  // 拡大縮小
  //===================================================================

  /// 最大幅に調整する
  private static BitmapSource Resize(BitmapSource original) {
    // scale計算
    double scaleX = 1.0;
    double scaleY = 1.0;
    if (original.PixelWidth <= ScreenCapture.MaxBitmapWidth &&
        original.PixelHeight <= ScreenCapture.MaxBitmapHeight) {
      // 最大サイズよりちいさい
      // nop
    } else if (original.PixelWidth < original.PixelHeight) {
      // 最大サイズオーバー＋縦長＝縦を短く
      scaleY = (double)ScreenCapture.MaxBitmapHeight / original.PixelHeight;
      scaleX = scaleY;
    } else {
      // 最大サイズオーバー＋横長＝横を短く
      scaleX = (double)ScreenCapture.MaxBitmapWidth / original.PixelWidth;
      scaleY = scaleX;
    }
    
    return new TransformedBitmap(original, new ScaleTransform(scaleX, scaleY));
  }

  /// キャプチャしたHBitmapをBitmapSource(Freezed)にして返す
  /// @param request スクリーンキャプチャ設定をまとめたリクエスト
  /// @return スクリーンキャプチャ結果が格納されたBitmapSource
  public static BitmapSource Capture(ScreenCaptureRequest request) {
    Debug.Assert(request != null);

    // HBitmapからBitmapSourceを作成
    BitmapSource bitmap = null;
    using (var result = request.Execute()) {
      if (result == null) return bitmap;

      // HBitmapからBitmapSourceに変換
      bitmap = Imaging.CreateBitmapSourceFromHBitmap(
          result.Bitmap, IntPtr.Zero, Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());

      Debug.WriteLine("Capture: [{0:D}] size:{1:D}x{2:D}",
                      request.Index+1,
                      bitmap.PixelWidth, bitmap.PixelHeight);
    }

    // Alphaチャンネル情報を削除
    bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0.0);

    // あまり大きな画像をメモリにおいておきたくないので縮小
    /// @todo(me) TransformedBitmapはちょっと重過ぎる。何かないか考え中。
    // bitmap = this.Resize(bitmap);
      
    // スレッド越しにアクセスされるためFreeze
    bitmap.Freeze();
    return bitmap;
  }
}
}   // namespace SCFF.GUI.Controls