// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
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
/// scff_imaging/utilities.ccとhの移植版

/// scff_imagingモジュールのC#版(オリジナルはC++)
namespace SCFF.Common.Imaging {

using System.Diagnostics;

/// 画像の操作に便利な関数をまとめたクラス(C#では関数をまとめる為に名前空間は使えない)
internal class Utilities {
  /// 境界の座標系と同じ座標系の新しい配置を計算する
  public static bool CalculateLayout(int boundX, int boundY,
      int boundWidth, int boundHeight,
      int inputWidth, int inputHeight,
      bool stretch, bool keepAspectRatio,
      out int newX, out int newY,
      out int newWidth, out int newHeight) {
    // 高さと幅はかならず0より上
    Debug.Assert(inputWidth > 0 && inputHeight > 0 &&
                 boundWidth > 0 && boundHeight > 0,
                 "Invalid parameters");

    // 高さ、幅が境界と一致しているか？
    if (inputWidth == boundWidth && inputHeight == boundHeight) {
      // サイズが完全に同じならば何もしなくてもよい
      newX = boundX;
      newY = boundY;
      newWidth = boundWidth;
      newHeight = boundHeight;
      return true;
    }

    // 高さと幅の比率を求めておく
    double boundAspect = (double)boundWidth / boundHeight;
    double inputAspect = (double)inputWidth / inputHeight;

    // inputのサイズがboundより完全に小さいかどうか
    bool needExpand = inputWidth <= boundWidth &&
                      inputHeight <= boundHeight;

    // オプションごとに条件分岐
    if ((!keepAspectRatio && needExpand && stretch) ||
        (!keepAspectRatio && !needExpand)) {
      // 境界と一致させる
      newX = boundX;
      newY = boundY;
      newWidth = boundWidth;
      newHeight = boundHeight;
    } else if ((keepAspectRatio && needExpand && stretch) ||
               (keepAspectRatio && !needExpand)) {
      // アスペクト比維持しつつ拡大縮小:
      if (inputAspect >= boundAspect) {
        // 入力のほうが横長
        //    = widthを境界にあわせる
        //    = heightの倍率はwidthの引き伸ばし比率で求められる
        newWidth = boundWidth;
        newHeight = inputHeight * boundWidth / inputWidth;
        Debug.Assert(newHeight <= boundHeight, "Cannot expand over bound-height");
        newX = boundX;
        int paddingHeight = (boundHeight - newHeight) / 2;
        newY = boundY + paddingHeight;
      } else {
        // 出力のほうが横長
        //    = heightを境界にあわせる
        //    = widthの倍率はheightの引き伸ばし比率で求められる
        newHeight = boundHeight;
        newWidth = inputWidth * boundHeight / inputHeight;
        Debug.Assert(newHeight <= boundHeight, "Cannot expand over bound-height");
        newY = boundY;
        int paddingWidth = (boundWidth - newWidth) / 2;
        newX = boundX + paddingWidth;
      }
    } else if (needExpand && !stretch) {
      // パディングを入れる
      int paddingWidth = (boundWidth - inputWidth) / 2;
      int paddingHeight = (boundHeight - inputHeight) / 2;
      newX = boundX + paddingWidth;
      newY = boundY + paddingHeight;
      newWidth = inputWidth;
      newHeight = inputHeight;
    } else {
      Debug.Assert(false, "Fail");
      newX = -1;
      newY = -1;
      newWidth = -1;
      newHeight = -1;
      return false;
    }

    return true;
  }

  /// 幅と高さから拡大縮小した場合のパディングサイズを求める
  public static bool CalculatePaddingSize(int boundWidth, int boundHeight,
      int inputWidth, int inputHeight,
      bool stretch, bool keepAspectRatio,
      out int paddingTop, out int paddingBottom,
      out int paddingLeft, out int paddingRight) {
    int newX, newY, newWidth, newHeight;
    // 座標系はbound領域内
    bool error = CalculateLayout(0, 0, boundWidth, boundHeight,
        inputWidth, inputHeight,
        stretch, keepAspectRatio,
        out newX,  out newY, out newWidth, out newHeight);
    if (error != true) {
      Debug.Assert(false, "Fail");
      paddingLeft = -1;
      paddingRight = -1;
      paddingTop = -1;
      paddingBottom = -1;
      return error;
    }
    paddingLeft = newX;
    paddingTop = newY;
    paddingRight = boundWidth - (newX + newWidth);
    paddingBottom = boundHeight - (newY + newHeight);
    return true;
  }
}
}