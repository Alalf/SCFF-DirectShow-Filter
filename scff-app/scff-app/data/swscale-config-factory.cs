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

/// @file scff-app/data/swscale-config-factory.cs
/// @brief scff_*.SWScaleConfigを生成・変換するためのクラスの定義

using System;

namespace scff_app.data {

/// @brief scff_*.SWScaleConfigを生成・変換するためのクラス
public class SWScaleConfigFactory {

  /// @brief scff_interprocessモジュールのパラメータから生成
  public static SWScaleConfig FromInterprocess(scff_interprocess.SWScaleConfig input) {
    SWScaleConfig output = new SWScaleConfig();

    output.Flags = (scff_interprocess.SWScaleFlags)
        Enum.ToObject(typeof(scff_interprocess.SWScaleFlags), input.flags);
    output.AccurateRnd = Convert.ToBoolean(input.accurate_rnd);
    output.IsFilterEnabled = Convert.ToBoolean(input.is_filter_enabled);
    output.LumaGBlur = input.luma_gblur;
    output.ChromaGBlur = input.chroma_gblur;
    output.LumaSharpen = input.luma_sharpen;
    output.ChromaSharpen = input.chroma_sharpen;
    output.ChromaHShift = input.chroma_hshift;
    output.ChromaVShift = input.chroma_vshift;

    return output;
  }

  /// @brief scff_interprocessモジュールのパラメータを生成
  public static scff_interprocess.SWScaleConfig ToInterprocess(SWScaleConfig input) {
    scff_interprocess.SWScaleConfig output = new scff_interprocess.SWScaleConfig();

    output.flags = (Int32)input.Flags;
    output.accurate_rnd = Convert.ToByte(input.AccurateRnd);
    output.is_filter_enabled = Convert.ToByte(input.IsFilterEnabled);
    output.luma_gblur = input.LumaGBlur;
    output.chroma_gblur = input.ChromaGBlur;
    output.luma_sharpen = input.LumaSharpen;
    output.chroma_sharpen = input.ChromaSharpen;
    output.chroma_hshift = input.ChromaHShift;
    output.chroma_vshift = input.ChromaVShift;

    return output;
  }
}
}