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
  public AreaSelectForm(int bound_width, int bound_height, ref BindingSource layoutParameters) {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    movable_and_resizable_ = new MovableAndResizable(this, bound_width, bound_height);

    // オリジナルの値を保持しておく
    data.LayoutParameter current = (data.LayoutParameter)layoutParameters.Current;
    if (current.Window != ExternalAPI.GetDesktopWindow()) {
      ExternalAPI.RECT window_screen_rect;
      ExternalAPI.GetWindowRect(current.Window, out window_screen_rect);
      original_x_ = window_screen_rect.left;
      original_y_ = window_screen_rect.top;
      original_width_ = window_screen_rect.right - window_screen_rect.left;
      original_height_ = window_screen_rect.bottom - window_screen_rect.top;
    } else {
      original_x_ = current.ClippingX;
      original_y_ = current.ClippingY;
      original_width_ = current.ClippingWidth;
      original_height_ = current.ClippingHeight;
    }

    // HACK!: 一応ここでも更新するが信頼できない
    Location = new Point(original_x_, original_y_);
    Size = new Size(original_width_, original_height_);

    // 初期化
    clipping_x_ = original_x_;
    clipping_y_ = original_y_;
    clipping_width_ = original_width_;
    clipping_height_ = original_height_;
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

  private void AreaSelectForm_Shown(object sender, EventArgs e) {
    // HACK!: コンストラクタで設定したLocation/Sizeは信頼できないのでここで変更
    Location = new Point(original_x_, original_y_);
    Size = new Size(original_width_, original_height_);
  }

  private void AreaSelectForm_DoubleClick(object sender, EventArgs e) {
    Close();
  }

  private void AreaSelectForm_FormClosed(object sender, FormClosedEventArgs e) {
    // HACK!: フォームの左上をスクリーン座標に変換
    Point origin = new Point(0, 0);
    clipping_x_ = PointToScreen(origin).X;
    clipping_y_ = PointToScreen(origin).Y;
    clipping_width_ = Width;
    clipping_height_ = Height;
  }

  //===================================================================
  // メンバ変数
  //===================================================================
  int clipping_x_;
  int clipping_y_;
  int clipping_width_;
  int clipping_height_;

  int original_x_;
  int original_y_;
  int original_width_;
  int original_height_;
}
}
