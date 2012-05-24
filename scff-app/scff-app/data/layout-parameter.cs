
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

/// @file scff-app/data/layout-parameter.cs
/// @brief scff_interprocess.LayoutParameterをマネージドクラス化したクラスの定義

using System;

namespace scff_app.data {

/// @brief scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
public partial class LayoutParameter {

  /// @brief デフォルトコンストラクタ
  public LayoutParameter() {
    // リスト・クラスはデフォルトコンストラクタで初期化を行う
    SWScaleConfig = new SWScaleConfig();
  }

  public Int32 BoundX { get; set; }
  public Int32 BoundY { get; set; }
  public Int32 BoundWidth { get; set; }
  public Int32 BoundHeight { get; set; }
  public UIntPtr Window { get; set; }
  public Int32 ClippingX { get; set; }
  public Int32 ClippingY { get; set; }
  public Int32 ClippingWidth { get; set; }
  public Int32 ClippingHeight { get; set; }
  public Boolean ShowCursor { get; set; }
  public Boolean ShowLayeredWindow { get; set; }
  public SWScaleConfig SWScaleConfig { get; set; }
  public Boolean Stretch { get; set; }
  public Boolean KeepAspectRatio { get; set; }
  public scff_interprocess.RotateDirection RotateDirection { get; set; }
}
}
