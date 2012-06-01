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

/// @file scff-app/view/AreaSelectForm.cs
/// @brief エリア選択ウィンドウの定義

namespace scff_app.view {

using System;
using System.Drawing;
using System.Windows.Forms;

/// @brief エリア選択ウィンドウ
public partial class AreaSelectForm : Form {

  /// @brief コンストラクタ
  public AreaSelectForm(BindingSource layoutParameters) {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    layout_parameters_ = layoutParameters;

    /// @todo(me) マルチモニタ対応
    ExternalAPI.RECT window_rect;
    ExternalAPI.GetClientRect(ExternalAPI.GetDesktopWindow(), out window_rect);
    int bound_width = window_rect.right;
    int bound_height = window_rect.bottom;

    movable_and_resizable_ = new MovableAndResizable(this, bound_width, bound_height);

    // オリジナルの値を保持しておく
    viewmodel.LayoutParameter current = (viewmodel.LayoutParameter)layoutParameters.Current;
    if (current.Window != ExternalAPI.GetDesktopWindow()) {
      // ウィンドウ取り込み時はスクリーン座標に変換する
      ExternalAPI.RECT window_screen_rect;
      ExternalAPI.GetWindowRect(current.Window, out window_screen_rect);
      original_x_ = window_screen_rect.left;
      original_y_ = window_screen_rect.top;
      original_width_ = window_screen_rect.right - window_screen_rect.left;
      original_height_ = window_screen_rect.bottom - window_screen_rect.top;
    } else {
      // デスクトップ取り込み時はそのまま
      original_x_ = current.ClippingX;
      original_y_ = current.ClippingY;
      original_width_ = current.ClippingWidth;
      original_height_ = current.ClippingHeight;
    }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void AreaSelectForm_Load(object sender, EventArgs e) {
    // Formのプロパティを編集する際はLoadの中でやるのが好ましい
    this.Location = new Point(original_x_, original_y_);
    this.Size = new Size(original_width_, original_height_);
  }

  private void accept_Click(object sender, EventArgs e) {
    // HACK!: フォームの左上をスクリーン座標に変換
    int window_screen_x, window_screen_y, window_width, window_height;
    Point origin = new Point(0, 0);
    window_screen_x = PointToScreen(origin).X;
    window_screen_y = PointToScreen(origin).Y;
    window_width = this.Width;
    window_height = this.Height;

    // デスクトップ取り込みに変更
    ((viewmodel.LayoutParameter)layout_parameters_.Current).SetWindowWithClippingRegion(
        ExternalAPI.GetDesktopWindow(),
        window_screen_x, window_screen_y, window_width, window_height);

    // 念のためウィンドウ幅を超えないように修正
    ((viewmodel.LayoutParameter)layout_parameters_.Current).ModifyClippingRegion();

    this.Close();
  }

  private void cancel_Click(object sender, EventArgs e) {
    // nop
    this.Close();
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  BindingSource layout_parameters_;

  // ウィンドウにドラッグによる移動・リサイズ機能を付加
  MovableAndResizable movable_and_resizable_;

  readonly int original_x_;
  readonly int original_y_;
  readonly int original_width_;
  readonly int original_height_;
}
}
