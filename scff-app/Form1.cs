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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace scff_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// 共有メモリからディレクトリを取得し、いろいろ処理
        private void UpdateDirectory() { throw new NotImplementedException(); }
        /// 共有メモリにNullLayoutリクエストを設定
        private void SendNullLayoutRequest() { throw new NotImplementedException(); }
        /// 共有メモリにNativeLayoutリクエストを設定
        private void SendNativeLayoutRequest() { throw new NotImplementedException(); }
        /// 共有メモリにComplexLayoutリクエストを設定
        private void SendComplexLayoutRequest() { throw new NotImplementedException(); }

        //-------------------------------------------------------------------

        /// ResizeMethod ComboBoxにデータソースを設定する
        private void BuildResizeMethodCombobox() { throw new NotImplementedException(); }

        //-------------------------------------------------------------------

        /// ウィンドウを指定する
        private void SetWindow(IntPtr /*HWND*/ window_handle) { throw new NotImplementedException(); }
        /// デスクトップを全画面で取り込む
        private void DoCaptureDesktopWindow() { throw new NotImplementedException(); }
        /// クリッピング領域をリセットする
        private void ResetClippingRegion() { throw new NotImplementedException(); }

        /// パラメータのValidate
        private bool ValidateParameters() { throw new NotImplementedException(); }

        //-------------------------------------------------------------------

        /// Dwmapi.dllを利用してAeroをOffに
        private void DWMAPIOff() { throw new NotImplementedException(); }
        /// 強制的にAeroのOn/Offを切り替える
        private void DWMAPIFlip() { throw new NotImplementedException(); }
        /// AeroをOffにしていたらOnに戻す
        private void DWMAPIRestore() { throw new NotImplementedException(); }
        /// Dwmapi.dllが利用可能かどうか
        private bool can_use_dwmapi_dll_;
        /// Aeroが起動時にONになっていたかどうか
        private bool was_dwm_enabled_on_start_;

        //-------------------------------------------------------------------

        /// プロセス間通信用オブジェクト
        private object /*scff_interprocess::Interprocess **/interprocess_;
        /// 現在編集中のレイアウト番号
        private int editing_layout_index_;

        private object /*scff_interprocess::LayoutParameter**/ GetLayoutParameterByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return layout1_parameter_;
                case 1:
                    return layout2_parameter_;
                case 2:
                    return layout3_parameter_;
                case 3:
                    return layout4_parameter_;
                case 4:
                    return layout5_parameter_;
                case 5:
                    return layout6_parameter_;
                case 6:
                    return layout7_parameter_;
                case 7:
                    return layout8_parameter_;
            }
            return 0;
        }

        dynamic /*scff_interprocess::LayoutParameter**/ GetCurrentLayoutParameter()
        {
            return GetLayoutParameterByIndex(editing_layout_index_);
        }

        /// レイアウトパラメータ1
        dynamic /*scff_interprocess::LayoutParameter**/ layout1_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout2_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout3_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout4_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout5_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout6_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout7_parameter_;
        dynamic /*scff_interprocess::LayoutParameter**/ layout8_parameter_;

        private void aero_on_item_Click(object sender, EventArgs e)
        {
            DWMAPIFlip();
        }

        private void window_draghere_MouseDown(object sender, MouseEventArgs e)
        {
            window_draghere.BackColor = Color.Orange;
        }

        private void process_refresh_Click(object sender, EventArgs e)
        {  // ディレクトリから更新
            UpdateDirectory();
        }
        private void window_draghere_MouseUp(object sender, MouseEventArgs e)
        {
            //this->window_draghere->BackColor = SystemColors::Control;

            //Point location = this->window_draghere->PointToScreen(e->Location);
            //POINT point;
            //point.x = location.X;
            //point.y = location.Y;

            //HWND window_handle = WindowFromPoint(point);
            //if (window_handle != NULL) {
            //  // 見つかった場合
            //  SetWindow(window_handle);
            //  Diagnostics::Debug::WriteLine(location);
            //} else {
            //  // nop
            //}
        }
        private void splash_Click(object sender, EventArgs e)
        {
            SendNullLayoutRequest();
        }
        private void apply_Click(object sender, EventArgs e)
        {
            if (ValidateParameters())
            {
                SendNativeLayoutRequest();
            }
        }
        private void process_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        private void area_fit_CheckedChanged(object sender, EventArgs e)
        {
            if (area_fit.Checked)
            {
                area_clipping_x.Enabled = false;
                area_clipping_y.Enabled = false;
                area_clipping_width.Enabled = false;
                area_clipping_height.Enabled = false;

                ResetClippingRegion();
            }
            else
            {
                area_clipping_x.Enabled = true;
                area_clipping_y.Enabled = true;
                area_clipping_width.Enabled = true;
                area_clipping_height.Enabled = true;
            }
        }
        private void window_desktop_Click(object sender, EventArgs e)
        {
            DoCaptureDesktopWindow();
        }
        private void area_clipping_x_ValueChanged(object sender, EventArgs e)
        {
            GetCurrentLayoutParameter().clipping_x = area_clipping_x.Value;
        }
        private void area_clipping_y_ValueChanged(object sender, EventArgs e)
        {
            GetCurrentLayoutParameter().clipping_y = area_clipping_y.Value;
        }
        private void area_clipping_width_ValueChanged(object sender, EventArgs e)
        {
            GetCurrentLayoutParameter().clipping_width =
                area_clipping_width.Value;
        }
        private void area_clipping_height_ValueChanged(object sender, EventArgs e)
        {
            GetCurrentLayoutParameter().clipping_height =
                area_clipping_height.Value;
        }
        private void option_show_mouse_cursor_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_mouse_cursor.Checked)
            {
                GetCurrentLayoutParameter().show_cursor = 1;
            }
            else
            {
                GetCurrentLayoutParameter().show_cursor = 0;
            }
        }
        private void option_show_layered_window_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_layered_window.Checked)
            {
                GetCurrentLayoutParameter().show_layered_window = 1;
            }
            else
            {
                GetCurrentLayoutParameter().show_layered_window = 0;
            }
        }
        private void option_keep_aspect_ratio_CheckedChanged(object sender, EventArgs e)
        {
            if (option_keep_aspect_ratio.Checked)
            {
                GetCurrentLayoutParameter().keep_aspect_ratio = 1;
            }
            else
            {
                GetCurrentLayoutParameter().keep_aspect_ratio = 0;
            }
        }
        private void option_enable_enlargement_CheckedChanged(object sender, EventArgs e)
        {
            if (option_enable_enlargement.Checked)
            {
                GetCurrentLayoutParameter().stretch = 1;
            }
            else
            {
                GetCurrentLayoutParameter().stretch = 0;
            }
        }
        private void option_over_sampling_CheckedChanged(object sender, EventArgs e) { }
        private void option_thread_num_ValueChanged(object sender, EventArgs e) { }
        private void option_resize_method_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetCurrentLayoutParameter().sws_flags =
                option_resize_method_combo.SelectedValue;
        }
        private void target_area_select_Click(object sender, EventArgs e)
        {
            AreaSelectForm form = new AreaSelectForm();
            Point new_loc = new Point((int)area_clipping_x.Value,
                          (int)area_clipping_y.Value);
            form.Location = new_loc;
            Size new_size = new Size((int)area_clipping_width.Value,
                                           (int)area_clipping_height.Value);
            form.Size = new_size;
            form.ShowDialog();

            // デスクトップキャプチャに変更
            DoCaptureDesktopWindow();
            // FitをはずしてClippingをかく
            area_fit.Checked = false;
            area_clipping_x.Value = Math.Max(form.clipping_x, area_clipping_x.Minimum);
            area_clipping_y.Value = Math.Max(form.clipping_y, area_clipping_y.Minimum);
            area_clipping_width.Value = Math.Min(form.clipping_width, area_clipping_width.Maximum - area_clipping_x.Value);
            area_clipping_height.Value = Math.Min(form.clipping_height, area_clipping_height.Maximum - area_clipping_y.Value);
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            DWMAPIOff();
        }
        private void layout_layout_Click(object sender, EventArgs e)
        {
            if (ValidateParameters())
            {
                SendComplexLayoutRequest();
            }
        }
        private void layout_add_Click(object sender, EventArgs e)
        {
        }
        private void layout_remove_Click(object sender, EventArgs e)
        {
        }
        private void layout_strip_ButtonClick(object sender, EventArgs e)
        {
            if (Width > 300)
            {
                Width = 300;
            }
            else
            {
                Width = 488;
            }
        }
        private void layout_list_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

    }
}
