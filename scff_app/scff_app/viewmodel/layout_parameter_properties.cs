// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file scff_app/viewmodel/layout_parameter_properties.cs
/// scff_app.viewmodel.LayoutParameterのプロパティの定義

namespace scff_app.viewmodel {

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Runtime.Serialization;
using System.Collections.Generic;

/// scff_inteprocess.LayoutParameterのビューモデル
[DataContract]
partial class LayoutParameter : INotifyPropertyChanged {

  [DataMember]
  public UIntPtr Window {
    get {
      return window_;
    }
    private set {
      if (window_ != value) {
        window_ = value;
        OnPropertyChanged("Window");
      }
      if (this.Fit) {
        OnPropertyChanged("ClippingX");
        OnPropertyChanged("ClippingY");
        OnPropertyChanged("ClippingWidth");
        OnPropertyChanged("ClippingHeight");
      }
    }
  }
  UIntPtr window_;

  [DataMember]
  public Int32 ActualClippingX {
    get {
      return actual_clipping_x_;
    }
    set {
      if (actual_clipping_x_ != value) {
        actual_clipping_x_ = value;
        OnPropertyChanged("ClippingX");
        OnPropertyChanged("ActualClippingX");
      }
    }
  }
  Int32 actual_clipping_x_;

  [DataMember]
  public Int32 ActualClippingY {
    get {
      return actual_clipping_y_;
    }
    set {
      if (actual_clipping_y_ != value) {
        actual_clipping_y_ = value;
        OnPropertyChanged("ClippingY");
        OnPropertyChanged("ActualClippingY");
      }
    }
  }
  Int32 actual_clipping_y_;

  [DataMember]
  public Int32 ActualClippingWidth {
    get {
      return actual_clipping_width_;
    }
    set {
      if (actual_clipping_width_ != value) {
        actual_clipping_width_ = value;
        OnPropertyChanged("ClippingWidth");
        OnPropertyChanged("ActualClippingWidth");
      }
    }
  }
  Int32 actual_clipping_width_;

  [DataMember]
  public Int32 ActualClippingHeight {
    get {
      return actual_clipping_height_;
    }
    set {
      if (actual_clipping_height_ != value) {
        actual_clipping_height_ = value;
        OnPropertyChanged("ClippingHeight");
        OnPropertyChanged("ActualClippingHeight");
      }
    }
  }
  Int32 actual_clipping_height_;

  [DataMember]
  public Boolean ShowCursor {
    get {
      return show_cursor_;
    }
    set {
      if (show_cursor_ != value) {
        show_cursor_ = value;
        OnPropertyChanged("ShowCursor");
      }
    }
  }
  Boolean show_cursor_;

  [DataMember]
  public Boolean ShowLayeredWindow {
    get {
      return show_layered_window_;
    }
    set {
      if (show_layered_window_ != value) {
        show_layered_window_ = value;
        OnPropertyChanged("ShowLayeredWindow");
      }
    }
  }
  Boolean show_layered_window_;

  //-------------------------------------------------------------------

  [DataMember]
  SWScaleConfig SWScaleConfig {
    get {
      return swscale_config_;
    }
    set {
      if (swscale_config_ != value) {
        swscale_config_ = value;
        OnPropertyChanged("SWScaleConfig");
      }
    }
  }
  SWScaleConfig swscale_config_;

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

  //-------------------------------------------------------------------

  [DataMember]
  public Boolean Stretch {
    get {
      return stretch_;
    }
    set {
      if (stretch_ != value) {
        stretch_ = value;
        OnPropertyChanged("Stretch");
      }
    }
  }
  Boolean stretch_;

  [DataMember]
  public Boolean KeepAspectRatio {
    get {
      return keep_aspect_ratio_;
    }
    set {
      if (keep_aspect_ratio_ != value) {
        keep_aspect_ratio_ = value;
        OnPropertyChanged("KeepAspectRatio");
      }
    }
  }
  Boolean keep_aspect_ratio_;

  [DataMember]
  public scff_interprocess.RotateDirections RotateDirection {
    get {
      return rotate_direction_;
    }
    set {
      if (rotate_direction_ != value) {
        rotate_direction_ = value;
        OnPropertyChanged("RotateDirection");
      }
    }
  }
  scff_interprocess.RotateDirections rotate_direction_;

  //-------------------------------------------------------------------
  // scff_app独自の値 (Messageには書き込まれない)
  //-------------------------------------------------------------------

  /// 0.0-1.0を境界の幅としたときの境界内の左端の座標
  [DataMember]
  public Double BoundRelativeLeft {
    get {
      return bound_relative_left_;
    }
    set {
      if (bound_relative_left_ != value) {
        bound_relative_left_ = value;
        OnPropertyChanged("BoundRelativeLeft");
      }
    }
  }
  Double bound_relative_left_;

  /// 0.0-1.0を境界の幅としたときの境界内の右端の座標
  [DataMember]
  public Double BoundRelativeRight {
    get {
      return bound_relative_right_;
    }
    set {
      if (bound_relative_right_ != value) {
        bound_relative_right_ = value;
        OnPropertyChanged("BoundRelativeRight");
      }
    }
  }
  Double bound_relative_right_;

  /// 0.0-1.0を境界の高さとしたときの境界内の上端の座標
  [DataMember]
  public Double BoundRelativeTop {
    get {
      return bound_relative_top_;
    }
    set {
      if (bound_relative_top_ != value) {
        bound_relative_top_ = value;
        OnPropertyChanged("BoundRelativeTop");
      }
    }
  }
  Double bound_relative_top_;

  /// 0.0-1.0を境界の高さとしたときの境界内の下端の座標
  [DataMember]
  public Double BoundRelativeBottom {
    get {
      return bound_relative_bottom_;
    }
    set {
      if (bound_relative_bottom_ != value) {
        bound_relative_bottom_ = value;
        OnPropertyChanged("BoundRelativeBottom");
      }
    }
  }
  Double bound_relative_bottom_;

  /// Clipping領域のFitオプション
  [DataMember]
  public Boolean Fit {
    get {
      return fit_;
    }
    set {
      if (fit_ != value) {
        fit_ = value;
        OnPropertyChanged("Fit");
        OnPropertyChanged("ClippingX");
        OnPropertyChanged("ClippingY");
        OnPropertyChanged("ClippingWidth");
        OnPropertyChanged("ClippingHeight");
      }
    }
  }
  Boolean fit_;

  //-------------------------------------------------------------------
  // プロキシ
  //-------------------------------------------------------------------

  public Int32 ClippingX {
    get {
      if (this.Fit) {
        return this.WindowRectangle.X;
      } else {
        return this.ActualClippingX;
      }
    }
    set {
      this.ActualClippingX = value;
    }
  }

  public Int32 ClippingY {
    get {
      if (this.Fit) {
        return this.WindowRectangle.Y;
      } else {
        return this.ActualClippingY;
      }
    }
    set {
      this.ActualClippingY = value;
    }
  }

  public Int32 ClippingWidth {
    get {
      if (this.Fit) {
        return this.WindowRectangle.Width;
      } else {
        return this.ActualClippingWidth;
      }
    }
    set {
      this.ActualClippingWidth = value;
    }
  }

  public Int32 ClippingHeight {
    get {
      if (this.Fit) {
        return this.WindowRectangle.Height;
      } else {
        return this.ActualClippingHeight;
      }
    }
    set {
      this.ActualClippingHeight = value;
    }
  }

  //-------------------------------------------------------------------
  // 表示用
  //-------------------------------------------------------------------

  /// レイアウトの名前代わりに使用するWindowのクラス名
  public string WindowText {
    get {
      if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(window_)) {
        return "*** INVALID WINDOW ***";
      } else if (this.Window == ExternalAPI.GetDesktopWindow()) {
        return "(Desktop)";
      } else {
        StringBuilder class_name = new StringBuilder(256);
        ExternalAPI.GetClassName(window_, class_name, 256);
        return class_name.ToString();
      }
    }
  }

  //-------------------------------------------------------------------

  [OnDeserializing]
  internal void OnDeserializingCallBack(StreamingContext streamingContext) {
    errors_ = new Dictionary<string, string>();
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
