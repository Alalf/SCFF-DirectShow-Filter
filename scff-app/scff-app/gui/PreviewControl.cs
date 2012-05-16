
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

/// @file scff-app/gui/PreviewControl.cs
/// @brief LayoutForm内で使用するプレビューコントロールの定義

using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System;

namespace scff_app.gui {

/// @brief LayoutForm内で使用するプレビューコントロール
public partial class PreviewControl : UserControl {

  //-------------------------------------------------------------------
  // Wrapper
  //-------------------------------------------------------------------
  private const int SRCCOPY = 13369376;
  private const int CAPTUREBLT = 1073741824;
  [StructLayout(LayoutKind.Sequential)]
  private struct RECT {
    public int left;
    public int top;
    public int right;
    public int bottom;
  }
  [DllImport("user32.dll")]
  private static extern IntPtr GetDC(UIntPtr hwnd);
  [DllImport("gdi32.dll")]
  private static extern int BitBlt(IntPtr hDestDC,
      int x,
      int y,
      int nWidth,
      int nHeight,
      IntPtr hSrcDC,
      int xSrc,
      int ySrc,
      int dwRop);
  [DllImport("user32.dll")]
  private static extern IntPtr ReleaseDC(UIntPtr hwnd, IntPtr hdc);
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsWindow(UIntPtr hWnd);
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // メソッド
  //-------------------------------------------------------------------

  /// @brief コンストラクタ
  public PreviewControl(int bound_width, int bound_height,
                        int index_in_layout_parameter_binding_source,
                        LayoutParameter layout_parameter) {
    InitializeComponent();

    // メンバの設定
    layout_parameter_ = layout_parameter;
    IndexInLayoutParameterBindingSource = index_in_layout_parameter_binding_source;
    movable_and_resizable_ = new MovableAndResizable(this, bound_width, bound_height);
  }

  private void PreviewControl_Load(object sender, EventArgs e) {
    info_font_ = new Font("Verdana", 10, FontStyle.Bold);
    info_point_f_ = new PointF(0, 0);

    // ビットマップ作成/キャプチャ/タイマーOn
    captured_bitmap_ = new Bitmap(layout_parameter_.ClippingWidth,
                                  layout_parameter_.ClippingHeight);
    ScreenCapture();
    capture_timer.Enabled = true;

    // Unloadでビットマップを解放
    this.Disposed += PreviewControl_UnLoad;

    // ダブルバッファ設定
    this.SetStyle(ControlStyles.DoubleBuffer, true);
    this.SetStyle(ControlStyles.UserPaint, true);
    this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
  }

  public void PreviewControl_UnLoad(object sender, EventArgs e) {
    capture_timer.Enabled = false;
    info_font_.Dispose();
    captured_bitmap_.Dispose();
  }

  private void fit_item_Click(object sender, EventArgs e) {
    int padding_top, padding_bottom, padding_left, padding_right;
    scff_imaging.Utilities.CalculatePaddingSize(Width, Height,
        captured_bitmap_.Width, captured_bitmap_.Height,
        layout_parameter_.Stretch,
        layout_parameter_.KeepAspectRatio,
        out padding_top, out padding_bottom, out padding_left, out padding_right);

    Size = new Size(Width - padding_left - padding_right, Height - padding_top - padding_bottom);
  }

  public override string ToString() {
    string output = "[";
    output += (IndexInLayoutParameterBindingSource+1).ToString();
    output += "] ";
    output += Width.ToString() + "x" + Height.ToString();
    output += " " + layout_parameter_.WindowText;
    return output;
  }

  private void PreviewControl_Paint(object sender, PaintEventArgs e) {
    // 描画位置を計算
    int new_x, new_y, new_width, new_height;
    scff_imaging.Utilities.CalculateLayout(0,0,Width,Height,
        captured_bitmap_.Width, captured_bitmap_.Height,
        layout_parameter_.Stretch,
        layout_parameter_.KeepAspectRatio,
        out new_x, out new_y, out new_width, out new_height);
    
    // しょうがないので枠は黒で塗りつぶす
    int padding_left = new_x;
    int padding_top = new_y;
    int padding_right = Width - (new_x + new_width);
    int padding_bottom = Height - (new_y + new_height);

    // 上
    e.Graphics.FillRectangle(Brushes.Black, 0, 0, Width, padding_top);
    // 下
    e.Graphics.FillRectangle(Brushes.Black, 0, padding_top + new_height, Width, padding_bottom);
    // 左
    e.Graphics.FillRectangle(Brushes.Black, 0, padding_top, padding_left, new_height);
    // 右
    e.Graphics.FillRectangle(Brushes.Black, padding_left + new_width, padding_top, padding_right, new_height);

    e.Graphics.DrawImage(captured_bitmap_, new Rectangle(new_x, new_y, new_width, new_height));

    e.Graphics.DrawString(ToString(), info_font_, Brushes.DarkOrange, info_point_f_);
    e.Graphics.DrawRectangle(Pens.DarkOrange, 0, 0, Width - 1, Height - 1);
  }

  private void PreviewControl_SizeChanged(object sender, EventArgs e) {
    Invalidate();
  }

  protected override void OnPaintBackground(PaintEventArgs pevent) {
    // 何もしない
    // base.OnPaintBackground(pevent);
  }

  //-------------------------------------------------------------------
  // スクリーンキャプチャ
  //-------------------------------------------------------------------

  private void capture_timer_Tick(object sender, EventArgs e) {
    // キャプチャする
    ScreenCapture();
    Invalidate();
  }

  private void ScreenCapture() {
    UIntPtr window = layout_parameter_.Window;
    if (!IsWindow(window)) {
      return;
    }
    IntPtr window_dc = GetDC(window);
    if (window_dc == IntPtr.Zero) {
      // 不正なウィンドウなので何もしない
      return;
    }

    Graphics graphics = Graphics.FromImage(captured_bitmap_);
    IntPtr captured_bitmap_dc = graphics.GetHdc();

    // BitBlt
    BitBlt(captured_bitmap_dc, 0, 0, captured_bitmap_.Width, captured_bitmap_.Height,
           window_dc, 0, 0, SRCCOPY);
    graphics.ReleaseHdc(captured_bitmap_dc);
    graphics.Dispose();
    
    ReleaseDC(window, window_dc);
  }

  //-------------------------------------------------------------------
  // メンバ変数
  //-------------------------------------------------------------------

  public int IndexInLayoutParameterBindingSource { get; set; }

  private MovableAndResizable movable_and_resizable_;

  // レイアウトパラメータ
  private LayoutParameter layout_parameter_;

  // 3秒に一回更新するスクリーンキャプチャビットマップ
  Bitmap captured_bitmap_;

  // 情報表示用
  private Font info_font_;
  private PointF info_point_f_;
}
}
