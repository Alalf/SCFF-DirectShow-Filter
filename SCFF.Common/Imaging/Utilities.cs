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

/// @file SCFF.Common/Imaging/Utilities.cs
/// @copydoc SCFF::Common::Imaging::Utilities

/// scff_imagingモジュールの C# 版(オリジナルは C++ )
namespace SCFF.Common.Imaging {

using System;
using System.Diagnostics;

/// 画像の操作に便利な関数をまとめたクラス
/// (scff_imaging/utilities.ccとhの移植版)
/// @attention C#では関数をまとめる為に名前空間は使えない
public static class Utilities {
  /// 境界に合わせる
  private static void Fit(int boundX, int boundY, int boundWidth, int boundHeight,
                          out int newX, out int newY, out int newWidth, out int newHeight) {
    newX = boundX;
    newY = boundY;
    newWidth = boundWidth;
    newHeight = boundHeight;
  }

  /// 比率を維持したまま拡大・縮小する
  private static void Letterbox(int boundX, int boundY, int boundWidth, int boundHeight,
                                int inputWidth, int inputHeight,
                                out int newX, out int newY, out int newWidth, out int newHeight) {
    // アスペクト比の計算
    var boundAspect = (double)boundWidth / boundHeight;
    var inputAspect = (double)inputWidth / inputHeight;
    var isLetterboxing = (inputAspect >= boundAspect);

    if (isLetterboxing) {
      // A. 境界よりも元が横長(isLetterboxing)
      //  - widthを境界にあわせる
      //  - heightの倍率はwidthの引き伸ばし比率で求められる
      newX = boundX;
      newWidth = boundWidth;
      var actualHeight = boundWidth / inputAspect;
      var actualPaddingHeight = (boundHeight - actualHeight) / 2.0;
      // floor
      newY = boundY + (int)actualPaddingHeight;
      // ceil
      newHeight = (int)Math.Ceiling(actualHeight);
    } else {
      // B. 境界よりも元が縦長(!isLetterboxing = isPillarboxing)
      //  - heightを境界にあわせる
      //  - widthの倍率はheightの引き伸ばし比率で求められる
      newY = boundY;
      newHeight = boundHeight;
      var actualWidth = boundHeight * inputAspect;
      var actualPaddingWidth = (boundWidth - actualWidth) / 2.0;
      // floor
      newX = boundX + (int)actualPaddingWidth;
      // ceil
      newWidth = (int)Math.Ceiling(actualWidth);
    }
    Debug.Assert(boundX <= newX && newX + newWidth <= boundX + boundWidth &&
                 boundY <= newY && newY + newHeight <= boundY + boundHeight);
  }

  /// 拡大縮小はせずパディングだけ行う
  private static void Pad(int boundX, int boundY, int boundWidth, int boundHeight,
                          int inputWidth, int inputHeight,
                          out int newX, out int newY, out int newWidth, out int newHeight) {
    var actualPaddingWidth = (boundWidth - inputWidth) / 2.0;
    var actualPaddingHeight = (boundHeight - inputHeight) / 2.0;
    // floor
    newX = boundX + (int)actualPaddingWidth;
    newY = boundY + (int)actualPaddingHeight;
    newWidth = inputWidth;
    newHeight = inputHeight;
    Debug.Assert(boundX <= newX && newX + newWidth <= boundX + boundWidth &&
                 boundY <= newY && newY + newHeight <= boundY + boundHeight);
  }

  /// 境界の座標系と同じ座標系の新しい配置を計算する
  public static void CalculateLayout(int boundX, int boundY, int boundWidth, int boundHeight,
                                     int inputWidth, int inputHeight,
                                     bool stretch, bool keepAspectRatio,
                                     out int newX, out int newY, out int newWidth, out int newHeight) {
    // 高さと幅はかならず0より上
    Debug.Assert(inputWidth > 0 && inputHeight > 0 &&
                 boundWidth > 0 && boundHeight > 0,
                 "Invalid parameters");

    // - 1. 高さと幅が同じ(sameSize)
    // - 2. 高さと幅が共に境界より小さい(!sameSize && needExpand)
    //   - 2.1 拡大しない(needExpand && !stretch)
    //   - 2.2 拡大する(needExpand && stretch)
    //     - 2.2.1 アスペクト比維持しない(... && !keepAspectRatio)
    //     - 2.2.2 アスペクト比維持(... && keepAspectRatio)
    //       - 2.2.2.1 境界よりも元が横長(... && isLetterboxing)
    //       - 2.2.2.2 境界よりも元が縦長(... && !isLetterboxing)
    // - 3. 上記以外(!sameSize && !needExpand)
    //   - 3.1 アスペクト比維持しない(... && !keepAspectRatio)
    //   - 3.2 アスペクト比維持(... && keepAspectRatio)
    //     - 3.2.1 境界よりも元が横長(... && isLetterboxing)
    //     - 3.2.2 境界よりも元が縦長(... && !isLetterboxing)

    // 条件分岐用変数
    var sameSize = (inputWidth == boundWidth && inputHeight == boundHeight);
    var needExpand = (inputWidth <= boundWidth && inputHeight <= boundHeight);

    // 1. 高さと幅が同じ(sameSize)
    if (sameSize) {
      Utilities.Fit(boundX, boundY, boundWidth, boundHeight,
                    out newX, out newY, out newWidth, out newHeight);
      return;
    }

    // 2. 高さと幅が共に境界より小さい
    if (needExpand) {
      // 2.1. 拡大しない(!stretch)
      if (!stretch) {  
        Utilities.Pad(boundX, boundY, boundWidth, boundHeight,
                      inputWidth, inputHeight,
                      out newX, out newY, out newWidth, out newHeight);
        return;
      }
      // 2.2. 拡大する(else)
      if (!keepAspectRatio) {
        // 2.2.1. アスペクト比維持しない(!keepAspectRatio)
        Utilities.Fit(boundX, boundY, boundWidth, boundHeight,
                      out newX, out newY, out newWidth, out newHeight);
        return;
      }
      // 2.2.2. アスペクト比維持(else)
      // 2.2.2.1 境界よりも元が横長(... && isLetterboxing)
      // 2.2.2.2 境界よりも元が縦長(... && !isLetterboxing)
      Utilities.Letterbox(boundX, boundY, boundWidth, boundHeight,
                          inputWidth, inputHeight,
                          out newX, out newY, out newWidth, out newHeight);
      return;
    }

    // 3. 上記以外(else=高さか幅のどちらかが境界より大きい)
    if (!keepAspectRatio) {
      // 3.1. アスペクト比維持しない(!keepAspectRatio)
      Utilities.Fit(boundX, boundY, boundWidth, boundHeight,
                    out newX, out newY, out newWidth, out newHeight);
      return;
    }
    // 3.2. アスペクト比維持(else)
    // 3.2.1 境界よりも元が横長(... && isLetterboxing)
    // 3.2.2 境界よりも元が縦長(... && !isLetterboxing)
    Utilities.Letterbox(boundX, boundY, boundWidth, boundHeight,
                        inputWidth, inputHeight,
                        out newX, out newY, out newWidth, out newHeight);
  }

  /// 幅と高さから拡大縮小した場合のパディングサイズを求める
  public static void CalculatePaddingSize(int boundWidth, int boundHeight, int inputWidth, int inputHeight,
                                          bool stretch, bool keepAspectRatio,
                                          out int paddingTop, out int paddingBottom, out int paddingLeft, out int paddingRight) {
    // 領域の計算
    int newX, newY, newWidth, newHeight;
    Utilities.CalculateLayout(0, 0, boundWidth, boundHeight,
                              inputWidth, inputHeight,
                              stretch, keepAspectRatio,
                              out newX, out newY, out newWidth, out newHeight);
    
    // パディングサイズの計算
    paddingLeft = newX;
    paddingTop = newY;
    paddingRight = boundWidth - (newX + newWidth);
    paddingBottom = boundHeight - (newY + newHeight);
  }
}
}   // namespace SCFF.Common.Imaging