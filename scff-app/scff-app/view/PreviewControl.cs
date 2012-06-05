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

/// @file scff-app/view/PreviewControl.cs
/// @brief LayoutForm内で使用するプレビューコントロールの定義

namespace scff_app.view {

using System;
using System.Drawing;
using System.Windows.Forms;
using scff_app.viewmodel;

/// @brief LayoutForm内で使用するプレビューコントロール
partial class PreviewControl : UserControl {

  /// @brief コンストラクタ
  public PreviewControl(BindingSource layout_parameters, int index) {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    index_ = index;
    layout_parameters_ = layout_parameters;

    movable_and_resizable_ = new MovableAndResizable(this, true);

    // Bitmap作成＋１回スクリーンキャプチャ
    LayoutParameter target_parameter = GetTargetParameter();
    captured_bitmap_ = new Bitmap(target_parameter.ClippingWidth, target_parameter.ClippingHeight);
    ScreenCapture();

    info_font_ = new Font("Verdana", 10, FontStyle.Bold);
    info_point_f_ = new PointF(0, 0);
  }

  public void Apply() {
    // 結果をBindingSourceに書き戻し
    ((LayoutParameter)layout_parameters_[index_]).BoundRelativeLeft = this.RelativeLeft;
    ((LayoutParameter)layout_parameters_[index_]).BoundRelativeTop = this.RelativeTop;
    ((LayoutParameter)layout_parameters_[index_]).BoundRelativeRight =  this.RelativeRight;
    ((LayoutParameter)layout_parameters_[index_]).BoundRelativeBottom = this.RelativeBottom;
  }

  //===================================================================
  // オーバーライド
  //===================================================================

  protected override void OnPaintBackground(PaintEventArgs pevent) {
    // 何もしない
    // base.OnPaintBackground(pevent);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void PreviewControl_Load(object sender, EventArgs e) {
    // 初期値を取得
    LayoutParameter target_parameter = GetTargetParameter();
    this.RelativeLeft = target_parameter.BoundRelativeLeft;
    this.RelativeTop = target_parameter.BoundRelativeTop;
    this.RelativeRight = target_parameter.BoundRelativeRight;
    this.RelativeBottom = target_parameter.BoundRelativeBottom;

    // タイマーOn
    this.captureTimer.Enabled = true;

    // Unloadでビットマップを解放
    this.Disposed += PreviewControl_UnLoad;

    // ダブルバッファ設定
    this.SetStyle(ControlStyles.DoubleBuffer, true);
    this.SetStyle(ControlStyles.UserPaint, true);
    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
  }

  public void PreviewControl_UnLoad(object sender, EventArgs e) {
    this.captureTimer.Enabled = false;

    // BitmapとFontはメモリ食いなので明示的にDisposeする
    info_font_.Dispose();
    captured_bitmap_.Dispose();
  }

  private void PreviewControl_Paint(object sender, PaintEventArgs e) {
    // 描画位置を計算
    LayoutParameter target_parameter = GetTargetParameter();
    int new_x, new_y, new_width, new_height;
    scff_imaging.Utilities.CalculateLayout(
        0, 0, this.Width, this.Height,
        captured_bitmap_.Width, captured_bitmap_.Height,
        target_parameter.Stretch,
        target_parameter.KeepAspectRatio,
        out new_x, out new_y, out new_width, out new_height);
    
    // しょうがないので枠は黒で塗りつぶす
    int padding_left = new_x;
    int padding_top = new_y;
    int padding_right = this.Width - (new_x + new_width);
    int padding_bottom = this.Height - (new_y + new_height);

    // 上
    e.Graphics.FillRectangle(Brushes.Black, 0, 0, this.Width, padding_top);
    // 下
    e.Graphics.FillRectangle(Brushes.Black, 0, padding_top + new_height, this.Width, padding_bottom);
    // 左
    e.Graphics.FillRectangle(Brushes.Black, 0, padding_top, padding_left, new_height);
    // 右
    e.Graphics.FillRectangle(Brushes.Black, padding_left + new_width, padding_top, padding_right, new_height);

    e.Graphics.DrawImage(captured_bitmap_, new Rectangle(new_x, new_y, new_width, new_height));

    e.Graphics.DrawString(PreviewInfo(), info_font_, Brushes.DarkOrange, info_point_f_);
    e.Graphics.DrawRectangle(Pens.DarkOrange, 0, 0, this.Width - 1, this.Height - 1);
  }

  private void PreviewControl_SizeChanged(object sender, EventArgs e) {
    Invalidate();
  }

  private void fit_Click(object sender, EventArgs e) {
    double aspect = (double)captured_bitmap_.Width / captured_bitmap_.Height;
    double current_aspect = (double)this.Width / this.Height;

    if (aspect < current_aspect) {
      // 縦長なので横方向に短くする
      this.Width = (int)Math.Floor(this.Height * aspect);
    } else {
      // 横長なので縦方向に小さくする
      this.Height = (int)Math.Floor(this.Width / aspect);
    }
  }

  private void captureTimer_Tick(object sender, EventArgs e) {
    // キャプチャする
    ScreenCapture();
    Invalidate();
  }

  //-------------------------------------------------------------------

  LayoutParameter GetTargetParameter() {
    return (LayoutParameter)layout_parameters_[index_];
  }

  // プレビュー情報
  string PreviewInfo() {
    LayoutParameter target_parameter = GetTargetParameter();
    string output = "[";
    output += (index_ + 1).ToString();
    output += "] ";
    output += this.Width.ToString() + "x" + this.Height.ToString();
    output += " " + target_parameter.WindowText;
    return output;
  }

  // スクリーンキャプチャ
  void ScreenCapture() {
    LayoutParameter target_parameter = GetTargetParameter();
    UIntPtr window = target_parameter.Window;
    if (!ExternalAPI.IsWindow(window)) {
      return;
    }
    IntPtr window_dc = ExternalAPI.GetDC(window);
    if (window_dc == IntPtr.Zero) {
      // 不正なウィンドウなので何もしない
      return;
    }

    Graphics graphics = Graphics.FromImage(captured_bitmap_);
    IntPtr captured_bitmap_dc = graphics.GetHdc();

    // BitBlt
    ExternalAPI.BitBlt(captured_bitmap_dc, 0, 0, captured_bitmap_.Width, captured_bitmap_.Height,
           window_dc, target_parameter.ClippingX, target_parameter.ClippingY, ExternalAPI.SRCCOPY);
    graphics.ReleaseHdc(captured_bitmap_dc);
    graphics.Dispose();
    
    ExternalAPI.ReleaseDC(window, window_dc);
  }

  //===================================================================
  // プロパティ
  //===================================================================

  public int Index {
    get { return index_; }
  }

  public Double RelativeLeft {
    get {
      if (this.Parent != null) {
        return (Double)this.Left / this.Parent.Width;
      }
      return 0.0;
    }
    set {
      if (this.Parent != null) {
        this.Left = (int)Math.Ceiling(value * this.Parent.Width);
      }
    }
  }

  public Double RelativeTop {
    get {
      if (this.Parent != null) {
        return (Double)this.Top / this.Parent.Height;
      }
      return 0.0;
    }
    set {
      if (this.Parent != null) {
        this.Top = (int)Math.Ceiling(value * this.Parent.Height);
      }
    }
  }

  public Double RelativeRight {
    get {
      if (this.Parent != null) {
        return (Double)this.Right / this.Parent.Width;
      }
      return 1.0;
    }
    set {
      if (this.Parent != null) {
        int right = (int)Math.Floor(value * this.Parent.Width);
        this.Width = right - this.Left;
      }
    }
  }

  public Double RelativeBottom {
    get {
      if (this.Parent != null) {
        return (Double)this.Bottom / this.Parent.Height;
      }
      return 1.0;
    }
    set {
      if (this.Parent != null) {
        int bottom = (int)Math.Floor(value * this.Parent.Height);
        this.Height = bottom - this.Top;
      }
    }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  // レイアウトパラメータ
  int index_;
  BindingSource layout_parameters_;

  // ウィンドウにドラッグによる移動・リサイズ機能を付加
  MovableAndResizable movable_and_resizable_;

  // 3秒に一回更新するスクリーンキャプチャビットマップ
  Bitmap captured_bitmap_;

  // 情報表示用
  Font info_font_;
  PointF info_point_f_;
}
}
