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

/// @file scff_app/viewmodel/swscale_config.cs
/// scff_app.viewmodel.SWScaleConfigのメソッドの定義

namespace scff_app.viewmodel {

using System;

// scff_interprocess.SWScaleConfigのビューモデル
partial class SWScaleConfig {

  /// デフォルトコンストラクタ
  public SWScaleConfig() {
    this.Init();
  }

  /// scff_interprocess用に変換
  public scff_interprocess.SWScaleConfig ToInterprocess() {
    scff_interprocess.SWScaleConfig output = new scff_interprocess.SWScaleConfig();

    output.flags = (Int32)this.Flags;
    output.accurate_rnd = Convert.ToByte(this.AccurateRnd);
    output.is_filter_enabled = Convert.ToByte(this.IsFilterEnabled);
    output.luma_gblur = this.LumaGBlur;
    output.chroma_gblur = this.ChromaGBlur;
    output.luma_sharpen = this.LumaSharpen;
    output.chroma_sharpen = this.ChromaSharpen;

    /// @warning HShift/VShiftの設定はffdshowでも0.0固定になっていた
    output.chroma_hshift = 0.0F;
    output.chroma_vshift = 0.0F;
    // output.chroma_hshift = this.ChromaHShift;
    // output.chroma_vshift = this.ChromaVShift;

    return output;
  }

  //-------------------------------------------------------------------

  /// デフォルトパラメータを設定
  void Init() {
    this.Flags = scff_interprocess.SWScaleFlags.kArea;
    this.IsFilterEnabled = false;
    this.ChromaHShift = 1.0F;
    this.ChromaVShift = 1.0F;
  }
}
}