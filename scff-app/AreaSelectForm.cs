
// Copyright 2012 Progre <djyayutto_at_gmail.com>
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

/// @file AreaSelectForm.cs
/// @brief エリア選択ウィンドウの定義

using System;
using System.Drawing;
using System.Windows.Forms;

namespace scff_app {

/// @brief エリア選択ウィンドウ
public partial class AreaSelectForm : Form {
  /// @brief コンストラクタ
  public AreaSelectForm() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    // 初期化
    resizing_ = false;
    last_location_ = new Point(0, 0);
    moving_start_location_ = new Point(0, 0);
    clipping_x_ = 0;
    clipping_y_ = 0;
    clipping_width_ = 32;
    clipping_height_ = 32;
  }

  /// @brief 結果を取得
  public void GetResult(out int clipping_x, out int clipping_y, out int clipping_width, out int clipping_height) {
    clipping_x = clipping_x_;
    clipping_y = clipping_y_;
    clipping_width = clipping_width_;
    clipping_height = clipping_height_;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------
  private void AreaSelectForm_DoubleClick(object sender, EventArgs e) {
    Close();
  }

  private void AreaSelectForm_MouseDown(object sender, MouseEventArgs e) {
    resizing_ = true;
    last_location_ = e.Location;
    moving_start_location_ = e.Location;
  }

  private void AreaSelectForm_MouseUp(object sender, MouseEventArgs e) {
    resizing_ = false;
  }

  private void AreaSelectForm_MouseMove(object sender, MouseEventArgs e) {
    if (resizing_) {
      int w = Size.Width;
      int h = Size.Height;

      if (Cursor.Equals(Cursors.SizeNWSE)) {
        Size my_size = new Size(w + (e.Location.X - last_location_.X), h + (e.Location.Y - last_location_.Y));
        if (my_size.Width > 32 && my_size.Height > 32) {
          Size = my_size;
        }
      } else if (Cursor.Equals(Cursors.SizeWE)) {
        var my_size = new Size(w + (e.Location.X - last_location_.X), h);
        if (my_size.Width > 32 && my_size.Height > 32) {
          Size = my_size;
        }
      } else if (Cursor.Equals(Cursors.SizeNS)) {
        var my_size = new Size(w, h + (e.Location.Y - last_location_.Y));
        if (my_size.Width > 32 && my_size.Height > 32) {
          this.Size = my_size;
        }
      } else if (Cursor.Equals(Cursors.SizeAll)) {
        Left += e.Location.X - moving_start_location_.X;
        Top += e.Location.Y - moving_start_location_.Y;
      }

      last_location_ = e.Location;
    } else {
      bool resize_x = e.X > (Width - 16);
      bool resize_y = e.Y > (Height - 16);

      if (resize_x && resize_y) {
        Cursor = Cursors.SizeNWSE;
      } else if (resize_x) {
        Cursor = Cursors.SizeWE;
      } else if (resize_y) {
        Cursor = Cursors.SizeNS;
      } else {
        Cursor = Cursors.SizeAll;
      }
    }
  }
  private void AreaSelectForm_FormClosed(object sender, FormClosedEventArgs e) {
    Point origin = new Point(0, 0);
    clipping_x_ = PointToScreen(origin).X;
    clipping_y_ = PointToScreen(origin).Y;
    clipping_width_ = Size.Width;
    clipping_height_ = Size.Height;
  }

  //===================================================================
  // メンバ変数
  //===================================================================
  private int clipping_x_;
  private int clipping_y_;
  private int clipping_width_;
  private int clipping_height_;

  private bool resizing_;
  private Point last_location_;
  private Point moving_start_location_;
}
}
