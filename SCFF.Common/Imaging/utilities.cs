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

/// @file SCFF.Common/Imaging/utilities.cs
/// scff_imaging/utilities.ccとhの移植版

namespace SCFF.Common.Imaging {

using System.Diagnostics;

/// 画像の操作に便利な関数をまとめたクラス
class Utilities {
  /// 境界の座標系と同じ座標系の新しい配置を計算する
  public static bool CalculateLayout(int bound_x, int bound_y,
                              int bound_width, int bound_height,
                              int input_width, int input_height,
                              bool stretch, bool keep_aspect_ratio,
                              out int new_x, out int new_y,
                              out int new_width, out int new_height) {
    // 高さと幅はかならず0より上
    Debug.Assert(input_width > 0 && input_height > 0 &&
                 bound_width > 0 && bound_height > 0);

    // 高さ、幅が境界と一致しているか？
    if (input_width == bound_width && input_height == bound_height) {
      // サイズが完全に同じならば何もしなくてもよい
      new_x = bound_x;
      new_y = bound_y;
      new_width = bound_width;
      new_height = bound_height;
      return true;
    }

    // 高さと幅の比率を求めておく
    double bound_aspect = (double)(bound_width) / bound_height;
    double input_aspect = (double)(input_width) / input_height;

    // inputのサイズがboundより完全に小さいかどうか
    bool need_expand = input_width <= bound_width &&
                       input_height <= bound_height;

    // オプションごとに条件分岐
    if (!keep_aspect_ratio && need_expand && stretch ||
        !keep_aspect_ratio && !need_expand) {
      // 境界と一致させる
      new_x = bound_x;
      new_y = bound_y;
      new_width = bound_width;
      new_height = bound_height;
    } else if (keep_aspect_ratio && need_expand && stretch ||
               keep_aspect_ratio && !need_expand) {
      // アスペクト比維持しつつ拡大縮小:
      if (input_aspect >= bound_aspect) {
        // 入力のほうが横長
        //    = widthを境界にあわせる
        //    = heightの倍率はwidthの引き伸ばし比率で求められる
        new_width = bound_width;
        new_height = input_height * bound_width / input_width;
        Debug.Assert(new_height <= bound_height);
        new_x = bound_x;
        int padding_height = (bound_height - new_height) / 2;
        new_y = bound_y + padding_height;
      } else {
        // 出力のほうが横長
        //    = heightを境界にあわせる
        //    = widthの倍率はheightの引き伸ばし比率で求められる
        new_height = bound_height;
        new_width = input_width * bound_height / input_height;
        Debug.Assert(new_height <= bound_height);
        new_y = bound_y;
        int padding_width = (bound_width - new_width) / 2;
        new_x = bound_x + padding_width;
      }
    } else if (need_expand && !stretch) {
      // パディングを入れる
      int padding_width = (bound_width - input_width) / 2;
      int padding_height = (bound_height - input_height) / 2;
      new_x = bound_x + padding_width;
      new_y = bound_y + padding_height;
      new_width = input_width;
      new_height = input_height;
    } else {
      Debug.Assert(false);
      new_x = -1;
      new_y = -1;
      new_width = -1;
      new_height = -1;
      return false;
    }

    return true;
  }

  /// 幅と高さから拡大縮小した場合のパディングサイズを求める
  public static bool CalculatePaddingSize(int bound_width, int bound_height,
                                   int input_width, int input_height,
                                   bool stretch, bool keep_aspect_ratio,
                                   out int padding_top, out int padding_bottom,
                                   out int padding_left, out int padding_right) {
    int new_x, new_y, new_width, new_height;
    // 座標系はbound領域内
    bool error =
        CalculateLayout(0, 0, bound_width, bound_height,
                        input_width, input_height,
                        stretch, keep_aspect_ratio,
                        out new_x,  out new_y, out new_width, out new_height);
    if (error != true) {
      Debug.Assert(false);
      padding_left = -1;
      padding_right = -1;
      padding_top = -1;
      padding_bottom = -1;
      return error;
    }
    padding_left = new_x;
    padding_top = new_y;
    padding_right = bound_width - (new_x + new_width);
    padding_bottom = bound_height - (new_y + new_height);
    return true;
  }
}
}