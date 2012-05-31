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

/// @file scff-app/data/swscale-config.cs
/// @brief scff_interprocess.SWScaleConfigをマネージドクラス化したクラスの定義

namespace scff_app.data {

using System;
using System.ComponentModel;

/// @brief scff_interprocess.SWScaleConfigをマネージドクラス化したクラス
partial class SWScaleConfig : INotifyPropertyChanged {
  scff_interprocess.SWScaleFlags flags_;
  Boolean accurate_rnd_;
  Boolean is_filter_enabled_;
  Single luma_gblur_;
  Single chroma_gblur_;
  Single luma_sharpen_;
  Single chroma_sharpen_;
  Single chroma_hshift_;
  Single chroma_vshift_;

  public scff_interprocess.SWScaleFlags Flags {
    get {
      return flags_;
    }
    set {
      if (flags_ != value) {
        flags_ = value;
        OnPropertyChanged("Flags");
      }
    }
  }
  public Boolean AccurateRnd {
    get {
      return accurate_rnd_;
    }
    set {
      if (accurate_rnd_ != value) {
        accurate_rnd_ = value;
        OnPropertyChanged("AccurateRnd");
      }
    }
  }
  public Boolean IsFilterEnabled {
    get {
      return is_filter_enabled_;
    }
    set {
      if (is_filter_enabled_ != value) {
        is_filter_enabled_ = value;
        OnPropertyChanged("IsFilterEnabled");
      }
    }
  }
  public Single LumaGBlur {
    get {
      return luma_gblur_;
    }
    set {
      if (luma_gblur_ != value) {
        luma_gblur_ = value;
        OnPropertyChanged("LumaGBlur");
      }
    }
  }
  public Single ChromaGBlur {
    get {
      return chroma_gblur_;
    }
    set {
      if (chroma_gblur_ != value) {
        chroma_gblur_ = value;
        OnPropertyChanged("ChromaGBlur");
      }
    }
  }
  public Single LumaSharpen {
    get {
      return luma_sharpen_;
    }
    set {
      if (luma_sharpen_ != value) {
        luma_sharpen_ = value;
        OnPropertyChanged("LumaSharpen");
      }
    }
  }
  public Single ChromaSharpen {
    get {
      return chroma_sharpen_;
    }
    set {
      if (chroma_sharpen_ != value) {
        chroma_sharpen_ = value;
        OnPropertyChanged("ChromaSharpen");
      }
    }
  }
  public Single ChromaHShift {
    get {
      return chroma_hshift_;
    }
    set {
      if (chroma_hshift_ != value) {
        chroma_hshift_ = value;
        OnPropertyChanged("ChromaHShift");
      }
    }
  }
  public Single ChromaVShift {
    get {
      return chroma_vshift_;
    }
    set {
      if (chroma_vshift_ != value) {
        chroma_vshift_ = value;
        OnPropertyChanged("ChromaVShift");
      }
    }
  }

  #region INotifyPropertyChanged メンバー

  public event PropertyChangedEventHandler PropertyChanged;
  void OnPropertyChanged(string name) {
    if (PropertyChanged != null) {
      PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
  }

  #endregion
}
}