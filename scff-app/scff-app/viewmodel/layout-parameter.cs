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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// scff_inteprocess.LayoutParameterのビューモデル
partial class LayoutParameter {

  /// @brief デフォルトコンストラクタ
  public LayoutParameter() {
    this.Init();
  }

  public void SetWindow(UIntPtr window) {
    this.SetWindowFromPtr(window);
    this.Fit = true;
    this.ClippingX = 0;
    this.ClippingY = 0;
    this.ClippingWidth = this.WindowSize.Width;
    this.ClippingHeight = this.WindowSize.Height;
  }

  public void SetWindowWithClippingRegion(UIntPtr window, int clipping_x, int clipping_y, int clipping_width, int clipping_height) {
    this.SetWindowFromPtr(window);
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

    if (this.ClippingX < 0) {
      modified_width += this.ClippingX;
      modified_x = 0;
    }
    if (this.ClippingY < 0) {
      modified_height += this.ClippingY;
      modified_y = 0;
    }
    if (this.ClippingX > this.WindowSize.Width) {
      modified_x = this.WindowSize.Width - this.ClippingWidth;
    }
    if (this.ClippingY > this.WindowSize.Height) {
      modified_y = this.WindowSize.Height - this.ClippingHeight;
    }

    if (modified_x + modified_width > this.WindowSize.Width) {
      modified_width = this.WindowSize.Width - modified_x;
    }
    if (modified_y + modified_height > this.WindowSize.Height) {
      modified_height = this.WindowSize.Height - modified_y;
    }

    this.ClippingX = modified_x;
    this.ClippingY = modified_y;
    this.ClippingWidth = modified_width;
    this.ClippingHeight = modified_height;
  }

  /// @brief 検証
  public bool Validate(bool show_message) {
    // もっとも危険な状態になりやすいウィンドウからチェック
    if (this.Window == UIntPtr.Zero) { // NULL
      if (show_message) {
        MessageBox.Show("Specified window is invalid", "Invalid Window",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }
    if (!ExternalAPI.IsWindow(this.Window)) {
      if (show_message) {
        MessageBox.Show("Specified window is invalid", "Invalid Window",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }

    // 境界設定のチェック
    if (this.BoundRelativeTop < this.BoundRelativeBottom &&
        this.BoundRelativeLeft < this.BoundRelativeRight) {
      // ok
    } else {
      if (show_message) {
        MessageBox.Show("Specified bound-rect is invalid", "Invalid Bound-rect",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }

    // クリッピングリージョンの判定
    ExternalAPI.RECT window_rect;
    ExternalAPI.GetClientRect(this.Window, out window_rect);
    if (this.ClippingX + this.ClippingWidth <= window_rect.right &&
        this.ClippingY + this.ClippingHeight <= window_rect.bottom &&
        this.ClippingWidth > 0 &&
        this.ClippingHeight > 0) {
      // nop 問題なし
    } else {
      if (show_message) {
        MessageBox.Show("Clipping region is invalid", "Invalid Clipping Region",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }

    return true;
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

  void SetWindowFromPtr(UIntPtr window) {
    this.Window = window;
    if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
      
    } else {
      ExternalAPI.RECT window_rect;
      ExternalAPI.GetClientRect(this.Window, out window_rect);
      this.WindowSize = new Size(window_rect.right, window_rect.bottom);
    }

    if (this.Window == UIntPtr.Zero) {
      this.WindowText = "(splash)";
      this.WindowSize = new Size(0, 0);

    } else if (!ExternalAPI.IsWindow(Window)) {
      this.WindowText = "*** INVALID WINDOW ***";
      this.WindowSize = new Size(0, 0);

    } else if (Window == ExternalAPI.GetDesktopWindow()) {
      this.WindowText = "(Desktop)";

      ExternalAPI.RECT window_rect;
      ExternalAPI.GetClientRect(this.Window, out window_rect);
      this.WindowSize = new Size(window_rect.right, window_rect.bottom);

    } else {
      StringBuilder class_name = new StringBuilder(256);
      ExternalAPI.GetClassName(Window, class_name, 256);
      this.WindowText = class_name.ToString();

      ExternalAPI.RECT window_rect;
      ExternalAPI.GetClientRect(this.Window, out window_rect);
      this.WindowSize = new Size(window_rect.right, window_rect.bottom);
    }
  }
}
}
