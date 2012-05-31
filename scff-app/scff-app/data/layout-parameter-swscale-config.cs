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

/// @file scff-app/data/layout-parameter-swscale-config.cs
/// @brief LayoutParameter.SWScaleConfigのプロキシメソッドの定義

namespace scff_app.data {

using System;

// scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
partial class LayoutParameter {

  public scff_interprocess.SWScaleFlags SWScaleConfigFlags {
    get {
      return swscale_config_.Flags;
    }
    set {
      if (swscale_config_.Flags != value) {
        swscale_config_.Flags = value;
        OnPropertyChanged("SWScaleConfigFlags");
      }
    }
  }
  public Boolean SWScaleConfigAccurateRnd {
    get {
      return swscale_config_.AccurateRnd;
    }
    set {
      if (swscale_config_.AccurateRnd != value) {
        swscale_config_.AccurateRnd = value;
        OnPropertyChanged("SWScaleConfigAccurateRnd");
      }
    }
  }
  public Boolean SWScaleConfigIsFilterEnabled {
    get {
      return swscale_config_.IsFilterEnabled;
    }
    set {
      if (swscale_config_.IsFilterEnabled != value) {
        swscale_config_.IsFilterEnabled = value;
        OnPropertyChanged("SWScaleConfigIsFilterEnabled");
      }
    }
  }
  public Single SWScaleConfigLumaGBlur {
    get {
      return swscale_config_.LumaGBlur;
    }
    set {
      if (swscale_config_.LumaGBlur != value) {
        swscale_config_.LumaGBlur = value;
        OnPropertyChanged("SWScaleConfigLumaGBlur");
      }
    }
  }
  public Single SWScaleConfigChromaGBlur {
    get {
      return swscale_config_.ChromaGBlur;
    }
    set {
      if (swscale_config_.ChromaGBlur != value) {
        swscale_config_.ChromaGBlur = value;
        OnPropertyChanged("SWScaleConfigChromaGBlur");
      }
    }
  }
  public Single SWScaleConfigLumaSharpen {
    get {
      return swscale_config_.LumaSharpen;
    }
    set {
      if (swscale_config_.LumaSharpen != value) {
        swscale_config_.LumaSharpen = value;
        OnPropertyChanged("SWScaleConfigLumaSharpen");
      }
    }
  }
  public Single SWScaleConfigChromaSharpen {
    get {
      return swscale_config_.ChromaSharpen;
    }
    set {
      if (swscale_config_.ChromaSharpen != value) {
        swscale_config_.ChromaSharpen = value;
        OnPropertyChanged("SWScaleConfigChromaSharpen");
      }
    }
  }
  public Single SWScaleConfigChromaHShift {
    get {
      return swscale_config_.ChromaHShift;
    }
    set {
      if (swscale_config_.ChromaHShift != value) {
        swscale_config_.ChromaHShift = value;
        OnPropertyChanged("SWScaleConfigChromaHShift");
      }
    }
  }
  public Single SWScaleConfigChromaVShift {
    get {
      return swscale_config_.ChromaVShift;
    }
    set {
      if (swscale_config_.ChromaVShift != value) {
        swscale_config_.ChromaVShift = value;
        OnPropertyChanged("SWScaleConfigChromaVShift");
      }
    }
  }

}
}
