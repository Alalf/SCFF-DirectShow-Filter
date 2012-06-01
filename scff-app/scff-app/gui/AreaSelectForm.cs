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

namespace scff_app.gui {

using System;
using System.Drawing;
using System.Windows.Forms;

/// @brief エリア選択ウィンドウ
public partial class AreaSelectForm : Form {

  /// @brief ウィンドウにドラッグによる移動・リサイズ機能を付加
  MovableAndResizable movable_and_resizable_;

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
    data.LayoutParameter current = (data.LayoutParameter)layoutParameters.Current;
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

    // HACK!: 一応ここでも更新するが信頼できない
    this.Location = new Point(original_x_, original_y_);
    this.Size = new Size(original_width_, original_height_);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------

  private void AreaSelectForm_Shown(object sender, EventArgs e) {
    // HACK!: コンストラクタで設定したLocation/Sizeは信頼できないのでここで変更
    this.Location = new Point(original_x_, original_y_);
    this.Size = new Size(original_width_, original_height_);
  }

  private void accept_Click(object sender, EventArgs e) {
    // HACK!: フォームの左上をスクリーン座標に変換
    int clipping_x, clipping_y, clipping_width, clipping_height;
    Point origin = new Point(0, 0);
    clipping_x = PointToScreen(origin).X;
    clipping_y = PointToScreen(origin).Y;
    clipping_width = this.Width;
    clipping_height = this.Height;

    // デスクトップ取り込みに変更
    ((data.LayoutParameter)layout_parameters_.Current).SetWindowWithClippingRegion(
        ExternalAPI.GetDesktopWindow(),
        clipping_x, clipping_y, clipping_width, clipping_height);

    // 念のためウィンドウ幅を超えないように修正
    ((data.LayoutParameter)layout_parameters_.Current).ModifyClippingRegion();

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

  readonly int original_x_;
  readonly int original_y_;
  readonly int original_width_;
  readonly int original_height_;
}
}
