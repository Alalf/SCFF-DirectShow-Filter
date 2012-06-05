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
using scff_app.viewmodel;

/// @brief エリア選択ウィンドウ
public partial class AreaSelectForm : Form {

  /// @brief コンストラクタ
  public AreaSelectForm(BindingSource layoutParameters) {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    layout_parameters_ = layoutParameters;

    movable_and_resizable_ = new MovableAndResizable(this, Utilities.GetWindowRectangle(ExternalAPI.GetDesktopWindow()));

    // オリジナルの値を保持しておく
    LayoutParameter current = (LayoutParameter)layoutParameters.Current;
    original_x_ = current.ClippingX;
    original_y_ = current.ClippingY;
    original_width_ = current.ClippingWidth;
    original_height_ = current.ClippingHeight;

    // ウィンドウ取り込み時はスクリーン座標に変換する
    if (current.Window != ExternalAPI.GetDesktopWindow() && ExternalAPI.IsWindow(current.Window)) {
      Utilities.GetScreenClientRect(current.Window,
          out original_x_, out original_y_, out original_width_, out original_height_);
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

  private void AreaSelectForm_DoubleClick(object sender, EventArgs e) {
    Apply();

    this.DialogResult = System.Windows.Forms.DialogResult.OK;
    this.Close();
  }

  //-------------------------------------------------------------------

  void Apply() {
    // フォームのクライアント領域をスクリーン座標に変換
    Rectangle window_rect = RectangleToScreen(this.ClientRectangle);

    // デスクトップ取り込みに変更
    ((LayoutParameter)layout_parameters_.Current).SetWindowWithClippingRegion(
        ExternalAPI.GetDesktopWindow(),
        window_rect.X, window_rect.Y, window_rect.Width, window_rect.Height);
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
