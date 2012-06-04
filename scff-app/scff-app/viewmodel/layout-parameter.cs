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

/// @file scff-app/viewmodel/layout-parameter.cs
/// @brief scff_app.viewmodel.LayoutParameterのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

// scff_inteprocess.LayoutParameterのビューモデル
partial class LayoutParameter : IDataErrorInfo {

  /// @brief デフォルトコンストラクタ
  public LayoutParameter() {
    this.Init();
  }

  public void SetWindow(UIntPtr window) {
    this.Window = window;
    this.Fit = true;
    this.ClippingX = 0;
    this.ClippingY = 0;
    this.ClippingWidth = this.WindowSize.Width;
    this.ClippingHeight = this.WindowSize.Height;
  }

  public void SetWindowWithClippingRegion(UIntPtr window, int clipping_x, int clipping_y, int clipping_width, int clipping_height) {
    this.Window = window;
    this.Fit = false;
    this.ClippingX = clipping_x;
    this.ClippingY = clipping_y;
    this.ClippingWidth = clipping_width;
    this.ClippingHeight = clipping_height;
  }

  /// @brief 修正
  public void ModifyClippingRegion() {
    int modified_x = this.ClippingX;
    int modified_y = this.ClippingY;
    int modified_width = this.ClippingWidth;
    int modified_height = this.ClippingHeight;

    // デスクトップウィンドウと普通のウィンドウで処理をわける
    int bound_x = 0;
    int bound_y = 0;
    int bound_width = this.WindowSize.Width;
    int bound_height = this.WindowSize.Height;
    if (this.Window == ExternalAPI.GetDesktopWindow()) {
      bound_x = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN);
      bound_y = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN);
      bound_width = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXVIRTUALSCREEN);
      bound_height = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CYVIRTUALSCREEN);
    }

    if (this.ClippingX < bound_x) {
      modified_width += this.ClippingX;
      modified_x = bound_x;
    }
    if (this.ClippingY < bound_y) {
      modified_height += this.ClippingY;
      modified_y = bound_y;
    }
    if (this.ClippingX > bound_width) {
      modified_x = bound_width - this.ClippingWidth;
    }
    if (this.ClippingY > bound_height) {
      modified_y = bound_height - this.ClippingHeight;
    }

    if (modified_x + modified_width > bound_width) {
      modified_width = bound_width - modified_x;
    }
    if (modified_y + modified_height > bound_height) {
      modified_height = bound_height - modified_y;
    }

    this.ClippingX = modified_x;
    this.ClippingY = modified_y;
    this.ClippingWidth = modified_width;
    this.ClippingHeight = modified_height;
  }

  /// @brief scff_interprocess用に変換
  public scff_interprocess.LayoutParameter ToInterprocess(int bound_width, int bound_height) {
    scff_interprocess.LayoutParameter output = new scff_interprocess.LayoutParameter();

    // 相対比率→ピクセル値変換
    output.bound_x = (Int32)(bound_width * this.BoundRelativeLeft) / 100;
    output.bound_y = (Int32)(bound_height * this.BoundRelativeTop) / 100;
    output.bound_width =
        (Int32)(bound_width * this.BoundRelativeRight) / 100 - output.bound_x;
    output.bound_height =
        (Int32)(bound_height * this.BoundRelativeBottom) / 100 - output.bound_y;

    output.window = (UInt64)this.Window;
    output.clipping_x = this.ClippingX;
    output.clipping_y = this.ClippingY;
    output.clipping_width = this.ClippingWidth;
    output.clipping_height = this.ClippingHeight;
    output.show_cursor = Convert.ToByte(this.ShowCursor);
    output.show_layered_window = Convert.ToByte(this.ShowLayeredWindow);

    // 拡大縮小設定
    output.swscale_config = this.SWScaleConfig.ToInterprocess();
   
    output.stretch = Convert.ToByte(this.Stretch);
    output.keep_aspect_ratio = Convert.ToByte(this.KeepAspectRatio);
    output.rotate_direction = (Int32)this.RotateDirection;

    return output;
  }

  #region IDataErrorInfo メンバー

  Dictionary<string, string> errors_ = new Dictionary<string,string>();

  public string Error {
    get {
      StringBuilder error_string = new StringBuilder();
      foreach (var i in errors_.Keys) {
        error_string.AppendLine(this[i]);
      }
      return error_string.ToString();
    }
  }

  public string this[string columnName] {
    get {
      if (errors_.ContainsKey(columnName)) {
        return errors_[columnName];
      }
      return string.Empty;
    }
  }

  /// @brief 検証
  void Validate() {
    errors_.Clear();

    // もっとも危険な状態になりやすいウィンドウからチェック
    if (this.Window == UIntPtr.Zero) { // NULL
      errors_["Window"] = "Specified window is invalid";
    } else if (!ExternalAPI.IsWindow(this.Window)) {
      errors_["Window"] = "Specified window is invalid";
    }

    // 境界設定のチェック
    if (this.BoundRelativeTop >= this.BoundRelativeBottom) {
      errors_["BoundRelativeTop"] = "Specified bound-top is invalid";
      errors_["BoundRelativeBottom"] = "Specified bound-bottom is invalid";
    }
    if (this.BoundRelativeLeft >= this.BoundRelativeRight) {
      errors_["BoundRelativeLeft"] = "Specified bound-left is invalid";
      errors_["BoundRelativeRight"] = "Specified bound-right is invalid";
    }

    // クリッピングリージョンの判定
    Rectangle bound_rectangle = new Rectangle(0,0,0,0);
    Rectangle clipping_rectangle = new Rectangle(this.ClippingX, this.ClippingY, this.ClippingWidth, this.ClippingHeight);
    if (this.Window == ExternalAPI.GetDesktopWindow()) {
      // デスクトップの場合
      bound_rectangle = Utilities.GetVirtualDesktopRectangle();
    } else if (ExternalAPI.IsWindow(this.Window)) {
      // 通常のウィンドウの場合
      ExternalAPI.RECT window_rect;
      ExternalAPI.GetClientRect(this.Window, out window_rect);
      bound_rectangle = new Rectangle(window_rect.left, window_rect.top, window_rect.right, window_rect.bottom);
    }
    if (!bound_rectangle.Contains(clipping_rectangle)) {
      // bound_rectangleが(0,0,0,0)なら必ず実行される
      errors_["ClippingX"] = "Clipping-x is invalid";
      errors_["ClippingY"] = "Clipping-y is invalid";
      errors_["ClippingWidth"] = "Clipping-width is invalid";
      errors_["ClippingHeight"] = "Clipping-height is invalid";
    }
  }

  public bool IsValid() {
    Validate();

    if (this.Error == string.Empty) {
      return true;
    } else {
      return false;
    }
  }

  #endregion

  //-------------------------------------------------------------------

  /// @brief デフォルトパラメータを設定
  void Init() {
    this.SetWindow(ExternalAPI.GetDesktopWindow());

    this.BoundRelativeLeft = 0.0;
    this.BoundRelativeRight = 100.0;
    this.BoundRelativeTop = 0.0;
    this.BoundRelativeBottom = 100.0;

    // 拡大縮小設定
    this.SWScaleConfig = new SWScaleConfig();

    this.Stretch = true;
    this.KeepAspectRatio = true;

    this.RotateDirection = scff_interprocess.RotateDirection.kNoRotate;
  }
}
}
