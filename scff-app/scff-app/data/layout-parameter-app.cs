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

/// @file scff-app/data/layout-parameter-app.cs
/// @brief LayoutParameterのscff_app用拡張

namespace scff_app.data {

using System;
using System.Runtime.InteropServices;
using System.Text;
  using System.Windows.Forms;
  using System.Drawing;

// scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
partial class LayoutParameter {

  /// @brief レイアウトの名前代わりに使用するWindowのクラス名
  public string WindowText { get; private set; }

  /// @brief Windowの大きさ
  public Size WindowSize { get; private set; }

  //-------------------------------------------------------------------

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
}
}
