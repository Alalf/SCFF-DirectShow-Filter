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

/// @file scff-app/viewmodel/layout-parameter-properties.cs
/// @brief scff_app.viewmodel.LayoutParameterのプロパティの定義

namespace scff_app.viewmodel {

using System;
using System.ComponentModel;
using System.Drawing;
  using System.Text;

/// @brief scff_inteprocess.LayoutParameterのビューモデル
partial class LayoutParameter : INotifyPropertyChanged {

  public UIntPtr Window {
    get {
      return window_;
    }
    private set {
      if (window_ != value) {
        window_ = value;
        OnPropertyChanged("Window");

        if (window_ == UIntPtr.Zero) {
          this.WindowText = "(splash)";
          this.WindowSize = new Size(0, 0);
          return;
        } else if (!ExternalAPI.IsWindow(window_)) {
          this.WindowText = "*** INVALID WINDOW ***";
          this.WindowSize = new Size(0, 0);
          return;
        }
                
        if (window_ == ExternalAPI.GetDesktopWindow()) {
          this.WindowText = "(Desktop)";
        } else {
          StringBuilder class_name = new StringBuilder(256);
          ExternalAPI.GetClassName(window_, class_name, 256);
          this.WindowText = class_name.ToString();
        }
        ExternalAPI.RECT window_rect;
        ExternalAPI.GetClientRect(window_, out window_rect);
        this.WindowSize = new Size(window_rect.right, window_rect.bottom);
      }
    }
  }
  UIntPtr window_;

  public Int32 ClippingX {
    get {
      return clipping_x_;
    }
    set {
      if (clipping_x_ != value) {
        clipping_x_ = value;
        OnPropertyChanged("ClippingX");
      }
    }
  }
  Int32 clipping_x_;

  public Int32 ClippingY {
    get {
      return clipping_y_;
    }
    set {
      if (clipping_y_ != value) {
        clipping_y_ = value;
        OnPropertyChanged("ClippingY");
      }
    }
  }
  Int32 clipping_y_;

  public Int32 ClippingWidth {
    get {
      return clipping_width_;
    }
    set {
      if (clipping_width_ != value) {
        clipping_width_ = value;
        OnPropertyChanged("ClippingWidth");
      }
    }
  }
  Int32 clipping_width_;

  public Int32 ClippingHeight {
    get {
      return clipping_height_;
    }
    set {
      if (clipping_height_ != value) {
        clipping_height_ = value;
        OnPropertyChanged("ClippingHeight");
      }
    }
  }
  Int32 clipping_height_;

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

  public scff_interprocess.RotateDirection RotateDirection {
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
  scff_interprocess.RotateDirection rotate_direction_;

  //-------------------------------------------------------------------
  // scff_app独自の値 (Messageには書き込まれない)
  //-------------------------------------------------------------------

  /// @brief 0.0-1.0を境界の幅としたときの境界内の左端の座標
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

  /// @brief 0.0-1.0を境界の幅としたときの境界内の右端の座標
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

  /// @brief 0.0-1.0を境界の高さとしたときの境界内の上端の座標
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

  /// @brief 0.0-1.0を境界の高さとしたときの境界内の下端の座標
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

  /// @brief Clipping領域のFitオプション
  public Boolean Fit {
    get {
      return fit_;
    }
    set {
      if (fit_ != value) {
        fit_ = value;
        OnPropertyChanged("Fit");
      }

      if (fit_) {
        this.ClippingX = 0;
        this.ClippingY = 0;
        this.ClippingWidth = this.WindowSize.Width;
        this.ClippingHeight = this.WindowSize.Height;
      }
    }
  }
  Boolean fit_;

  //-------------------------------------------------------------------
  // 表示用
  //-------------------------------------------------------------------

  /// @brief レイアウトの名前代わりに使用するWindowのクラス名
  public string WindowText {
    get {
      return window_text_;
    }
    private set {
      if (window_text_ != value) {
        window_text_ = value;
        OnPropertyChanged("WindowText");
      }
    }  
  }
  string window_text_;

  /// @brief Windowの大きさ
  public Size WindowSize {
    get {
      return window_size_;  
    }
    private set {
      if (window_size_ != value) {
        window_size_ = value;
        OnPropertyChanged("WindowSize");
      }
    }  
  }
  Size window_size_;

  //-------------------------------------------------------------------

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
