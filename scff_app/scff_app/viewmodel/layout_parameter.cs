// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file scff_app/viewmodel/layout_parameter.cs
/// scff_app.viewmodel.LayoutParameterのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;

// scff_inteprocess.LayoutParameterのビューモデル
partial class LayoutParameter : IDataErrorInfo {

  /// デフォルトコンストラクタ
  public LayoutParameter() {
    this.Init();
  }

  /// scff_interprocess用に変換
  public scff_interprocess.LayoutParameter ToInterprocess(int bound_width, int bound_height) {
    scff_interprocess.LayoutParameter output = new scff_interprocess.LayoutParameter();

    // 相対比率→ピクセル値変換
    // Left/Topは切り捨て、Right/Bottomは切り上げ
    output.bound_x = (Int32)Math.Ceiling(this.BoundRelativeLeft * bound_width);
    output.bound_y = (Int32)Math.Ceiling(this.BoundRelativeTop * bound_height);
    output.bound_width =
        (Int32)Math.Floor(this.BoundRelativeRight * bound_width) - output.bound_x;
    output.bound_height =
        (Int32)Math.Floor(this.BoundRelativeBottom * bound_height) - output.bound_y;

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
  //===================================================================
  // IDataErrorInfo メンバー
  //===================================================================

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
      if (errors_ != null && errors_.ContainsKey(columnName)) {
        return errors_[columnName];
      }
      return string.Empty;
    }
  }

  /// 検証
  void Validate() {
    errors_.Clear();

    // もっとも危険な状態になりやすいウィンドウからチェック
    if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
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
    Rectangle bound_rectangle = this.WindowRectangle;
    Rectangle clipping_rectangle = this.ClippingRectangle;
    if (this.ClippingWidth == 0) {
      errors_["ClippingWidth"] = "Clipping-width is invalid";
    }
    if (this.ClippingHeight == 0) {
      errors_["ClippingHeight"] = "Clipping-height is invalid";
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

  //===================================================================
  // プロパティアクセッサ
  //===================================================================

  public void SetWindow(UIntPtr window) {
    this.Window = window;
    this.Fit = true;
    ModifyActualClippingRectangle();
  }

  public void SetWindowWithClippingRegion(UIntPtr window, int clipping_x, int clipping_y, int clipping_width, int clipping_height) {
    this.Window = window;
    this.Fit = false;
    this.ActualClippingX = clipping_x;
    this.ActualClippingY = clipping_y;
    this.ActualClippingWidth = clipping_width;
    this.ActualClippingHeight = clipping_height;
  }

  public void SetPrimaryDesktopWindow() {
    ExternalAPI.RECT primary_desktop_rect;
    ExternalAPI.GetClientRect(ExternalAPI.GetDesktopWindow(), out primary_desktop_rect);
    this.SetWindowWithClippingRegion(ExternalAPI.GetDesktopWindow(),
        primary_desktop_rect.left, primary_desktop_rect.top,
        primary_desktop_rect.right, primary_desktop_rect.bottom);
  }

  /// クリッピング領域をスクリーン座標に変換したもの
  public Rectangle ClippingScreenRectangle {
    get {
      ExternalAPI.POINT clipping_screen_origin;
      clipping_screen_origin.x = this.ClippingX;
      clipping_screen_origin.y = this.ClippingY;
      ExternalAPI.ClientToScreen(this.Window, ref clipping_screen_origin);

      return new Rectangle(
        clipping_screen_origin.x,
        clipping_screen_origin.y,
        this.ClippingWidth,
        this.ClippingHeight);
    }
  }

  /// クリッピング領域を得る
  Rectangle ClippingRectangle {
    get {
      return new Rectangle(this.ClippingX, this.ClippingY, this.ClippingWidth, this.ClippingHeight);
    }
  }

  // Windowの大きさを返す
  Rectangle WindowRectangle {
    get {
      return Utilities.GetWindowRectangle(this.Window);
    }
  }

  // ActualClipping領域を返す
  Rectangle ActualClippingRectangle {
    get {
      return new Rectangle(this.ActualClippingX, this.ActualClippingY, this.ActualClippingWidth, this.ActualClippingHeight);
    }
  }

  //-------------------------------------------------------------------

  // ActualiClippingRegionの領域がWindowRectangleを超えていたらリセットする
  void ModifyActualClippingRectangle() {
    Rectangle window_rect = this.WindowRectangle;
    if (!window_rect.Contains(this.ActualClippingRectangle)) {
      this.ActualClippingX = window_rect.X;
      this.ActualClippingY = window_rect.Y;
      this.ActualClippingWidth = window_rect.Width;
      this.ActualClippingHeight = window_rect.Height;
    }
  }

  /// デフォルトパラメータを設定
  void Init() {
    // プライマリディスプレイを初期値にする
    SetPrimaryDesktopWindow();

    this.BoundRelativeLeft = 0.0;
    this.BoundRelativeRight = 1.0;
    this.BoundRelativeTop = 0.0;
    this.BoundRelativeBottom = 1.0;

    // 拡大縮小設定
    this.SWScaleConfig = new SWScaleConfig();

    this.Stretch = true;
    this.KeepAspectRatio = true;

    this.RotateDirection = scff_interprocess.RotateDirection.kNoRotate;
  }
}
}
