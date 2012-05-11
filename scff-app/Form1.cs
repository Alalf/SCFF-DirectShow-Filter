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

// TODO(progre): 移植未達部分が完了次第名称含め全体をリファクタリング

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Runtime.InteropServices;
using scff_interprocess;

namespace scff_app
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32")]
        private static extern bool GetVersionEx(ref OSVERSIONINFO osvi);
        [StructLayout(LayoutKind.Sequential)]
        class OSVERSIONINFO
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public Int16 wServicePackMajor;
            public Int16 wServicePackMinor;
            public Int16 wSuiteMask;
            public Byte wProductType;
            public Byte wReserved;
        }

        public Form1()
        {
            InitializeComponent();

            can_use_dwmapi_dll_ = false;
            was_dwm_enabled_on_start_ = false;
            interprocess_ = null;

            // DWMAPI.DLLが利用可能かどうか調べる
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT && System.Environment.OSVersion.Version.Major >= 6)
            {
                can_use_dwmapi_dll_ = true;
            }

            // プロセス間通信に必要なオブジェクトの生成
            interprocess_ = new Interprocess();
            // レイアウトパラメータを格納するためのオブジェクトを生成
            layout1_parameter_ = new Interprocess.LayoutParameter();
            layout2_parameter_ = new Interprocess.LayoutParameter();
            layout3_parameter_ = new Interprocess.LayoutParameter();
            layout4_parameter_ = new Interprocess.LayoutParameter();
            layout5_parameter_ = new Interprocess.LayoutParameter();
            layout6_parameter_ = new Interprocess.LayoutParameter();
            layout7_parameter_ = new Interprocess.LayoutParameter();
            layout8_parameter_ = new Interprocess.LayoutParameter();

            // コントロールの準備
            BuildResizeMethodCombobox();

            // 編集中のレイアウトインデックス
            editing_layout_index_ = 0;
            // ListViewを選択する
            layout_list.Items[editing_layout_index_].Selected = true;
            layout_list.Select();

            // ディレクトリ取得
            UpdateDirectory();

            // デフォルトの設定を書き込む
            DoCaptureDesktopWindow();
            option_keep_aspect_ratio.Checked = true;
            option_enable_enlargement.Checked = true;
            option_resize_method_combo.SelectedIndex = 9;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DWMAPIRestore();
        }

        // enum格納用
        // Tupleの動作が不安定との情報を聞いたのでしょうがなく作った
        class ResizeMethod
        {
            public
            ResizeMethod(String name, Interprocess.SWScaleFlags flags)
            {
                MethodName = name;
                SWScaleFlags = flags;
            }
            public String MethodName;
            public Interprocess.SWScaleFlags SWScaleFlags;
        }

        /// 共有メモリからディレクトリを取得し、いろいろ処理
        private void UpdateDirectory()
        {
            // 共有メモリからデータを取得
            interprocess_.InitDirectory();
            Interprocess.Directory directory;
            interprocess_.GetDirectory(out directory);

            // リストを新しく作成する
            ArrayList managed_directory = new ArrayList();

            // コンボボックスの内容を構築
            for (int i = 0; i < Interprocess.kMaxEntry; i++)
            {
                if (directory.entries[i].process_id == 0) continue;
                ManagedEntry entry = new ManagedEntry(directory.entries[i]);
                managed_directory.Add(entry);
            }
            process_combo.DataSource = managed_directory;

            if (managed_directory.Count > 0)
            {
                // SCFH DSF発見
                process_combo.DisplayMember = "Info";
                process_combo.ValueMember = "ProcessID";
                process_combo.Enabled = true;
                process_combo.SelectedIndex = 0;

                // メッセージを送るためのボタンを有効化
                splash.Enabled = true;
                apply.Enabled = true;
            }
            else
            {
                // SCFH DSFがロードされていない場合
                process_combo.Enabled = false;

                // メッセージを送るためのボタンを無効化
                splash.Enabled = false;
                apply.Enabled = false;
            }
        }
        /// 共有メモリにNullLayoutリクエストを設定
        private void SendNullLayoutRequest()
        {
            // メッセージを書いて送る
            Interprocess.Message message = new Interprocess.Message();
            message.timestamp = DateTime.Now.Ticks;
            message.layout_type = (int)Interprocess.LayoutType.kNullLayout;

            // 共有メモリを開いて送る
            if (process_combo.SelectedValue != null)
            {
                var process_id = (uint)((ManagedEntry)process_combo.SelectedItem).ProcessID;
                interprocess_.InitMessage(process_id);
                interprocess_.SendMessage(message);
            }
        }
        /// 共有メモリにNativeLayoutリクエストを設定
        private void SendNativeLayoutRequest()
        {
            // メッセージを書いて送る
            Interprocess.Message message = new Interprocess.Message();
            message.timestamp = DateTime.Now.Ticks;
            message.layout_type = (int)Interprocess.LayoutType.kNativeLayout;
            // 無視される
            message.layout_element_count = 1;
            message.layout_parameters = new Interprocess.LayoutParameter[Interprocess.kMaxComplexLayoutElements];
            message.layout_parameters[0].bound_x = 0;
            message.layout_parameters[0].bound_y = 0;
            message.layout_parameters[0].bound_width = 0;
            message.layout_parameters[0].bound_height = 0;
            // ここまで無視
            message.layout_parameters[0].window = GetCurrentLayoutParameter().window;
            message.layout_parameters[0].clipping_x = GetCurrentLayoutParameter().clipping_x;
            message.layout_parameters[0].clipping_y = GetCurrentLayoutParameter().clipping_y;
            message.layout_parameters[0].clipping_width = GetCurrentLayoutParameter().clipping_width;
            message.layout_parameters[0].clipping_height = GetCurrentLayoutParameter().clipping_height;
            message.layout_parameters[0].show_cursor = GetCurrentLayoutParameter().show_cursor;
            message.layout_parameters[0].show_layered_window = GetCurrentLayoutParameter().show_layered_window;
            message.layout_parameters[0].sws_flags = GetCurrentLayoutParameter().sws_flags;
            message.layout_parameters[0].stretch = GetCurrentLayoutParameter().stretch;
            message.layout_parameters[0].keep_aspect_ratio = GetCurrentLayoutParameter().keep_aspect_ratio;

            // 共有メモリを開いて送る
            if (process_combo.SelectedValue != null)
            {
                var process_id = (uint)((ManagedEntry)(process_combo.SelectedItem)).ProcessID;
                interprocess_.InitMessage(process_id);
                interprocess_.SendMessage(message);
            }
        }
        /// 共有メモリにComplexLayoutリクエストを設定
        private void SendComplexLayoutRequest()
        {
            // todo(Alalf) テスト中！あとで直す！

            // メッセージを書いて送る
            Interprocess.Message message = new Interprocess.Message();
            message.timestamp = DateTime.Now.Ticks;
            message.layout_type = (int)Interprocess.LayoutType.kComplexLayout;
            message.layout_element_count = 2;
            // 1個目の取り込み範囲
            message.layout_parameters[0].bound_x = 32;
            message.layout_parameters[0].bound_y = 32;
            message.layout_parameters[0].bound_width = 320;
            message.layout_parameters[0].bound_height = 240;
            message.layout_parameters[0].window = (ulong)(GetDesktopWindow());
            message.layout_parameters[0].clipping_x = 0;
            message.layout_parameters[0].clipping_y = 0;
            message.layout_parameters[0].clipping_width = 1000;
            message.layout_parameters[0].clipping_height = 500;
            message.layout_parameters[0].show_cursor = 0;
            message.layout_parameters[0].show_layered_window = 0;
            message.layout_parameters[0].sws_flags = (int)Interprocess.SWScaleFlags.kLanczos;
            message.layout_parameters[0].stretch = 1;
            message.layout_parameters[0].keep_aspect_ratio = 1;
            // 2個目の取り込み範囲
            message.layout_parameters[1].bound_x = 300;
            message.layout_parameters[1].bound_y = 0;
            message.layout_parameters[1].bound_width = 300;
            message.layout_parameters[1].bound_height = 100;
            message.layout_parameters[1].window = (ulong)(GetDesktopWindow());
            message.layout_parameters[1].clipping_x = 320;
            message.layout_parameters[1].clipping_y = 320;
            message.layout_parameters[1].clipping_width = 200;
            message.layout_parameters[1].clipping_height = 200;
            message.layout_parameters[1].show_cursor = 0;
            message.layout_parameters[1].show_layered_window = 0;
            message.layout_parameters[1].sws_flags = (int)Interprocess.SWScaleFlags.kLanczos;
            message.layout_parameters[1].stretch = 1;
            message.layout_parameters[1].keep_aspect_ratio = 1;

            // 共有メモリを開いて送る
            if (process_combo.SelectedValue != null)
            {
                var process_id = (uint)(process_combo.SelectedValue);
                interprocess_.InitMessage(process_id);
                interprocess_.SendMessage(message);
            }
        }

        //-------------------------------------------------------------------

        /// ResizeMethod ComboBoxにデータソースを設定する
        private void BuildResizeMethodCombobox()
        {
            // リストを新しく作成する
            ArrayList resize_methods = new ArrayList();
            resize_methods.Add(new ResizeMethod("FastBilinear (fast bilinear)", Interprocess.SWScaleFlags.kFastBilinear));
            resize_methods.Add(new ResizeMethod("Bilinear (bilinear)", Interprocess.SWScaleFlags.kBilinear));
            resize_methods.Add(new ResizeMethod("Bicubic (bicubic)", Interprocess.SWScaleFlags.kBicubic));
            resize_methods.Add(new ResizeMethod("X (experimental)", Interprocess.SWScaleFlags.kX));
            resize_methods.Add(new ResizeMethod("Point (nearest neighbor)", Interprocess.SWScaleFlags.kPoint));
            resize_methods.Add(new ResizeMethod("Area (averaging area)", Interprocess.SWScaleFlags.kArea));
            resize_methods.Add(new ResizeMethod("Bicublin (luma bicubic, chroma bilinear)", Interprocess.SWScaleFlags.kBicublin));
            resize_methods.Add(new ResizeMethod("Gauss (gaussian)", Interprocess.SWScaleFlags.kGauss));
            resize_methods.Add(new ResizeMethod("Sinc (sinc)", Interprocess.SWScaleFlags.kSinc));
            resize_methods.Add(new ResizeMethod("Lanczos (natural)", Interprocess.SWScaleFlags.kLanczos));
            resize_methods.Add(new ResizeMethod("Spline (natural bicubic spline)", Interprocess.SWScaleFlags.kSpline));
            option_resize_method_combo.DataSource = resize_methods;
            option_resize_method_combo.DisplayMember = "MethodName";
            //option_resize_method_combo.ValueMember = "SWScaleFlags";
            option_resize_method_combo.Enabled = true;
            option_resize_method_combo.SelectedIndex = 0;
        }

        //-------------------------------------------------------------------

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        /// ウィンドウを指定する
        private void SetWindow(IntPtr window_handle)
        {
            layout1_parameter_.window = (ulong)window_handle;

            if (window_handle == null)
            {
                this.window_handle.Text = "(Splash)";
            }
            else if (window_handle == GetDesktopWindow())
            {
                this.window_handle.Text = "(Desktop)";
            }
            else
            {
                StringBuilder class_name = new StringBuilder(256);
                GetClassName(window_handle, class_name, 256);
                this.window_handle.Text = class_name.ToString();
            }

            ResetClippingRegion();
        }
        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();        /// デスクトップを全画面で取り込む
        private void DoCaptureDesktopWindow()
        {
            SetWindow(GetDesktopWindow());
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindow(IntPtr hWnd);
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left, top, right, bottom;
        }
        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        /// クリッピング領域をリセットする
        private void ResetClippingRegion()
        {
            IntPtr window_handle =
                (IntPtr)GetCurrentLayoutParameter().window;

            if (window_handle == null || !IsWindow(window_handle))
            {
                return;
            }

            RECT window_rect;
            GetClientRect(window_handle, out window_rect);
            area_fit.Checked = true;

            // MinimumとMaximumはValueの更新前に更新しておかないと例外が発生する
            area_clipping_x.Minimum = window_rect.left;
            area_clipping_x.Maximum = window_rect.right;
            area_clipping_y.Minimum = window_rect.top;
            area_clipping_y.Maximum = window_rect.bottom;
            area_clipping_width.Maximum = window_rect.right;
            area_clipping_height.Maximum = window_rect.bottom;

            area_clipping_x.Value = window_rect.left;
            area_clipping_y.Value = window_rect.top;
            area_clipping_width.Value = window_rect.right;
            area_clipping_height.Value = window_rect.bottom;

            layout1_parameter_.clipping_x = window_rect.left;
            layout1_parameter_.clipping_y = window_rect.top;
            layout1_parameter_.clipping_width = window_rect.right;
            layout1_parameter_.clipping_height = window_rect.bottom;
        }

        /// パラメータのValidate
        private bool ValidateParameters()
        {
            // もっとも危険な状態になりやすいウィンドウからチェック
            if (GetCurrentLayoutParameter().window == 0)
            { // NULL
                MessageBox.Show("Specified window is invalid", "Invalid Window",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var window_handle = (IntPtr)(GetCurrentLayoutParameter().window);
            if (!IsWindow(window_handle))
            {
                MessageBox.Show("Specified window is invalid", "Invalid Window",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // クリッピングリージョンの判定
            RECT window_rect;
            GetClientRect(window_handle, out window_rect);
            if (GetCurrentLayoutParameter().clipping_x +
                GetCurrentLayoutParameter().clipping_width
                <= window_rect.right &&
                GetCurrentLayoutParameter().clipping_y +
                GetCurrentLayoutParameter().clipping_height
                <= window_rect.bottom &&
                GetCurrentLayoutParameter().clipping_width > 0 &&
                GetCurrentLayoutParameter().clipping_height > 0)
            {
                // nop 問題なし
            }
            else
            {
                MessageBox.Show("Clipping region is invalid", "Invalid Clipping Region",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        //-------------------------------------------------------------------

        [DllImport("dwmapi.dll")]
        private static extern int DwmIsCompositionEnabled(out bool enabled);
        [DllImport("dwmapi.dll")]
        private static extern int DwmEnableComposition(uint uCompositionAction);
        const int DWM_EC_DISABLECOMPOSITION = 0;
        const int DWM_EC_ENABLECOMPOSITION = 1;
        /// Dwmapi.dllを利用してAeroをOffに
        private void DWMAPIOff()
        {
            if (!can_use_dwmapi_dll_)
            {
                // dwmapi.dllを利用できなければ何もしない
                was_dwm_enabled_on_start_ = false;
                return;
            }

            bool was_dwm_enabled_on_start;
            DwmIsCompositionEnabled(out was_dwm_enabled_on_start);
            if (was_dwm_enabled_on_start)
            {
                //DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
            }
            else
            {
            }
            aero_on_item.Checked = false;
            was_dwm_enabled_on_start_ = was_dwm_enabled_on_start == true;
        }
        /// 強制的にAeroのOn/Offを切り替える
        private void DWMAPIFlip()
        {
            if (!can_use_dwmapi_dll_)
            {
                // dwmapi.dllを利用できなければ何もしない
                return;
            }

            if (aero_on_item.Checked)
            {
                DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
            }
            else
            {
                DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
            }
            aero_on_item.Checked = !(aero_on_item.Checked);
        }
        /// AeroをOffにしていたらOnに戻す
        private void DWMAPIRestore()
        {
            if (!can_use_dwmapi_dll_)
            {
                // dwmapi.dllを利用できなければ何もしない
                return;
            }

            if (was_dwm_enabled_on_start_)
            {
                DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
            }
        }
        /// Dwmapi.dllが利用可能かどうか
        private bool can_use_dwmapi_dll_;
        /// Aeroが起動時にONになっていたかどうか
        private bool was_dwm_enabled_on_start_;

        //-------------------------------------------------------------------

        /// プロセス間通信用オブジェクト
        private Interprocess interprocess_;
        /// 現在編集中のレイアウト番号
        private int editing_layout_index_;

        private Interprocess.LayoutParameter GetLayoutParameterByIndex(int index)
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
                default:
                    throw new ArgumentException();
            }
        }

        Interprocess.LayoutParameter GetCurrentLayoutParameter()
        {
            return GetLayoutParameterByIndex(editing_layout_index_);
        }

        /// レイアウトパラメータ1
        Interprocess.LayoutParameter layout1_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout2_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout3_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout4_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout5_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout6_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout7_parameter_ = new Interprocess.LayoutParameter();
        Interprocess.LayoutParameter layout8_parameter_ = new Interprocess.LayoutParameter();

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
        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(int xPoint, int yPoint);
        private void window_draghere_MouseUp(object sender, MouseEventArgs e)
        {
            window_draghere.BackColor = SystemColors.Control;

            Point location = window_draghere.PointToScreen(e.Location);

            IntPtr window_handle = WindowFromPoint(location.X, location.Y);
            if (window_handle != IntPtr.Zero)
            {
                // 見つかった場合
                SetWindow(window_handle);
            }
            else
            {
                // nop
            }
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
            layout1_parameter_.clipping_x = (int)area_clipping_x.Value;
        }
        private void area_clipping_y_ValueChanged(object sender, EventArgs e)
        {
            layout1_parameter_.clipping_y = (int)area_clipping_y.Value;
        }
        private void area_clipping_width_ValueChanged(object sender, EventArgs e)
        {
            layout1_parameter_.clipping_width =
            (int)area_clipping_width.Value;
        }
        private void area_clipping_height_ValueChanged(object sender, EventArgs e)
        {
            layout1_parameter_.clipping_height = (int)
            area_clipping_height.Value;
        }
        private void option_show_mouse_cursor_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_mouse_cursor.Checked)
            {
                layout1_parameter_.show_cursor = 1;
            }
            else
            {
                layout1_parameter_.show_cursor = 0;
            }
        }
        private void option_show_layered_window_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_layered_window.Checked)
            {
                layout1_parameter_.show_layered_window = 1;
            }
            else
            {
                layout1_parameter_.show_layered_window = 0;
            }
        }
        private void option_keep_aspect_ratio_CheckedChanged(object sender, EventArgs e)
        {
            if (option_keep_aspect_ratio.Checked)
            {
                layout1_parameter_.keep_aspect_ratio = 1;
            }
            else
            {
                layout1_parameter_.keep_aspect_ratio = 0;
            }
        }
        private void option_enable_enlargement_CheckedChanged(object sender, EventArgs e)
        {
            if (option_enable_enlargement.Checked)
            {
                layout1_parameter_.stretch = 1;
            }
            else
            {
                layout1_parameter_.stretch = 0;
            }
        }
        private void option_over_sampling_CheckedChanged(object sender, EventArgs e) { }
        private void option_thread_num_ValueChanged(object sender, EventArgs e) { }
        private void option_resize_method_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            layout1_parameter_.sws_flags =(int)(
                (ResizeMethod)                option_resize_method_combo.SelectedValue).SWScaleFlags;
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
