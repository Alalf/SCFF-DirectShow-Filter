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

/// @file scff-app/data/swscale-config-view.cs
/// @brief SWScaleConfigの表示用メソッドの定義

namespace scff_app.data {

using System;
using System.Collections.Generic;

// scff_interprocess.SWScaleConfigをマネージドクラス化したクラス
partial class SWScaleConfig {

  /// @brief ResizeMethodコンボボックス用リスト
  static SortedList<scff_interprocess.SWScaleFlags, string> resize_method_list_ =
      new SortedList<scff_interprocess.SWScaleFlags, string> {
    {scff_interprocess.SWScaleFlags.kFastBilinear, "FastBilinear (fast bilinear)"},
    {scff_interprocess.SWScaleFlags.kBilinear, "Bilinear (bilinear)"},
    {scff_interprocess.SWScaleFlags.kBicubic, "Bicubic (bicubic)"},
    {scff_interprocess.SWScaleFlags.kX, "X (experimental)"},
    {scff_interprocess.SWScaleFlags.kPoint, "Point (nearest neighbor)"},
    {scff_interprocess.SWScaleFlags.kArea, "Area (averaging area)"},
    {scff_interprocess.SWScaleFlags.kBicublin, "Bicublin (luma bicubic, chroma bilinear)"},
    {scff_interprocess.SWScaleFlags.kGauss, "Gauss (gaussian)"},
    {scff_interprocess.SWScaleFlags.kSinc, "Sinc (sinc)"},
    {scff_interprocess.SWScaleFlags.kLanczos, "Lanczos (lanczos)"},
    {scff_interprocess.SWScaleFlags.kSpline, "Spline (natural bicubic spline)"}
  };

  /// @brief ResizeMethodコンボボックス用リストへのアクセッサ
  public static SortedList<scff_interprocess.SWScaleFlags, string> ResizeMethodList {
    get { return resize_method_list_; }
  }
}
}