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

namespace scff_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            can_use_dwmapi_dll_ = false;
            was_dwm_enabled_on_start_ = false;
            interprocess_ = 0;

            // DWMAPI.DLLが利用可能かどうか調べる
            /*
  OSVERSIONINFO os_info;
  os_info.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
  GetVersionEx(&os_info);
  if (os_info.dwPlatformId == 2 && os_info.dwMajorVersion >= 6) {
    can_use_dwmapi_dll_ = true;
  }
             */

            /*
  // プロセス間通信に必要なオブジェクトの生成
  interprocess_ = new scff_interprocess::Interprocess;
  // レイアウトパラメータを格納するためのオブジェクトを生成
  layout1_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout1_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout2_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout2_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout3_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout3_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout4_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout4_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout5_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout5_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout6_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout6_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout7_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout7_parameter_, sizeof(scff_interprocess::LayoutParameter));
  layout8_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout8_parameter_, sizeof(scff_interprocess::LayoutParameter));
             * */

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
            //option_resize_method_combo.SelectedIndex = 9;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DWMAPIRestore();
        }
        /*
        // enum格納用
        // Tupleの動作が不安定との情報を聞いたのでしょうがなく作った
        class ResizeMethod
        {
            public
            ResizeMethod(String name, scff_interprocess::SWScaleFlags* flags)
            {
                MethodName = name;
                SWScaleFlags = flags;
            }
            public String MethodName;
            public scff_interprocess::SWScaleFlags* SWScaleFlags;
        }
         */

        /// 共有メモリからディレクトリを取得し、いろいろ処理
        private void UpdateDirectory()
        {
            /*
  // 共有メモリからデータを取得
  interprocess_->InitDirectory();
  scff_interprocess::Directory directory;
  interprocess_->GetDirectory(&directory);
  Diagnostics::Debug::WriteLine("Get Directory");

  // リストを新しく作成する
  ArrayList^ managed_directory = gcnew ArrayList();

  // コンボボックスの内容を構築
  for (int i = 0; i < scff_interprocess::kMaxEntry; i++) {
    if (directory.entries[i].process_id == 0) continue;
    ManagedEntry^ entry = gcnew ManagedEntry(directory.entries[i]);
    managed_directory->Add(entry);
  }
  this->process_combo->DataSource = managed_directory;

  if (managed_directory->Count > 0) {
    // SCFH DSF発見
    this->process_combo->DisplayMember = "Info";
    this->process_combo->ValueMember = "ProcessID";
    this->process_combo->Enabled = true;
    this->process_combo->SelectedIndex =0;

    // メッセージを送るためのボタンを有効化
    this->splash->Enabled = true;
    this->apply->Enabled = true;
  } else {
    // SCFH DSFがロードされていない場合
    this->process_combo->Enabled = false;

    // メッセージを送るためのボタンを無効化
    this->splash->Enabled = false;
    this->apply->Enabled = false;
  }
            */
        }
        /// 共有メモリにNullLayoutリクエストを設定
        private void SendNullLayoutRequest()
        {
            /*
  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = static_cast<int32_t>(timestamp);
  message.layout_type = scff_interprocess::kNullLayout;
    
  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
            */
        }
        /// 共有メモリにNativeLayoutリクエストを設定
        private void SendNativeLayoutRequest()
        {
            /*
  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = static_cast<int64_t>(timestamp);
  message.layout_type = scff_interprocess::kNativeLayout;
  // 無視される
  message.layout_element_count = 1;
  message.layout_parameters[0].bound_x = 0;
  message.layout_parameters[0].bound_y = 0;
  message.layout_parameters[0].bound_width = 0;
  message.layout_parameters[0].bound_height = 0;
  // ここまで無視
  message.layout_parameters[0].window = GetCurrentLayoutParameter()->window;
  message.layout_parameters [0].clipping_x = GetCurrentLayoutParameter()->clipping_x;
  message.layout_parameters[0].clipping_y = GetCurrentLayoutParameter()->clipping_y;
  message.layout_parameters[0].clipping_width = GetCurrentLayoutParameter()->clipping_width;
  message.layout_parameters[0].clipping_height = GetCurrentLayoutParameter()->clipping_height;
  message.layout_parameters[0].show_cursor = GetCurrentLayoutParameter()->show_cursor;
  message.layout_parameters[0].show_layered_window = GetCurrentLayoutParameter()->show_layered_window;
  message.layout_parameters[0].sws_flags = GetCurrentLayoutParameter()->sws_flags;
  message.layout_parameters[0].stretch = GetCurrentLayoutParameter()->stretch;
  message.layout_parameters[0].keep_aspect_ratio = GetCurrentLayoutParameter()->keep_aspect_ratio;

  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
            */
        }
        /// 共有メモリにComplexLayoutリクエストを設定
        private void SendComplexLayoutRequest()
        {
            /*
  /// @todo(me) テスト中！あとで直す！

  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = static_cast<int64_t>(timestamp);
  message.layout_type = scff_interprocess::kComplexLayout;
  message.layout_element_count = 2;
  // 1個目の取り込み範囲
  message.layout_parameters[0].bound_x = 32;
  message.layout_parameters[0].bound_y = 32;
  message.layout_parameters[0].bound_width = 320;
  message.layout_parameters[0].bound_height = 240;
  message.layout_parameters[0].window = reinterpret_cast<uint64_t>(GetDesktopWindow());
  message.layout_parameters[0].clipping_x = 0;
  message.layout_parameters[0].clipping_y = 0;
  message.layout_parameters[0].clipping_width = 1000;
  message.layout_parameters[0].clipping_height = 500;
  message.layout_parameters[0].show_cursor = 0;
  message.layout_parameters[0].show_layered_window = 0;
  message.layout_parameters[0].sws_flags = scff_interprocess::kLanczos;
  message.layout_parameters[0].stretch = 1;
  message.layout_parameters[0].keep_aspect_ratio = 1;
  // 2個目の取り込み範囲
  message.layout_parameters[1].bound_x = 300;
  message.layout_parameters[1].bound_y = 0;
  message.layout_parameters[1].bound_width = 300;
  message.layout_parameters[1].bound_height = 100;
  message.layout_parameters[1].window = reinterpret_cast<uint64_t>(GetDesktopWindow());
  message.layout_parameters[1].clipping_x = 320;
  message.layout_parameters[1].clipping_y = 320;
  message.layout_parameters[1].clipping_width = 200;
  message.layout_parameters[1].clipping_height = 200;
  message.layout_parameters[1].show_cursor = 0;
  message.layout_parameters[1].show_layered_window = 0;
  message.layout_parameters[1].sws_flags = scff_interprocess::kLanczos;
  message.layout_parameters[1].stretch = 1;
  message.layout_parameters[1].keep_aspect_ratio = 1;

  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
            */
        }

        //-------------------------------------------------------------------

        /// ResizeMethod ComboBoxにデータソースを設定する
        private void BuildResizeMethodCombobox()
        {
            // リストを新しく作成する
            ArrayList resize_methods = new ArrayList();
            /*
              resize_methods.Add(new ResizeMethod("FastBilinear (fast bilinear)", scff_interprocess::kFastBilinear));
              resize_methods.Add(new ResizeMethod("Bilinear (bilinear)", scff_interprocess::kBilinear));
              resize_methods.Add(new ResizeMethod("Bicubic (bicubic)", scff_interprocess::kBicubic));
              resize_methods.Add(new ResizeMethod("X (experimental)", scff_interprocess::kX));
              resize_methods.Add(new ResizeMethod("Point (nearest neighbor)", scff_interprocess::kPoint));
              resize_methods.Add(new ResizeMethod("Area (averaging area)", scff_interprocess::kArea));
              resize_methods.Add(new ResizeMethod("Bicublin (luma bicubic, chroma bilinear)", scff_interprocess::kBicublin));
              resize_methods.Add(new ResizeMethod("Gauss (gaussian)", scff_interprocess::kGauss));
              resize_methods.Add(new ResizeMethod("Sinc (sinc)", scff_interprocess::kSinc));
              resize_methods.Add(new ResizeMethod("Lanczos (natural)", scff_interprocess::kLanczos));
              resize_methods.Add(new ResizeMethod("Spline (natural bicubic spline)", scff_interprocess::kSpline));
                        */
            option_resize_method_combo.DataSource = resize_methods;
            option_resize_method_combo.DisplayMember = "MethodName";
            //option_resize_method_combo.ValueMember = "SWScaleFlags";
            option_resize_method_combo.Enabled = true;
            //option_resize_method_combo.SelectedIndex = 0;
        }

        //-------------------------------------------------------------------

        /// ウィンドウを指定する
        private void SetWindow(IntPtr /*HWND*/ window_handle)
        {
            /*
  uint64_t window = reinterpret_cast<uint64_t>(window_handle);
  GetCurrentLayoutParameter()->window = window;
  
  if (window_handle == NULL) {
    this->window_handle->Text = "(Splash)";
  } else if (window_handle == GetDesktopWindow()) {
    this->window_handle->Text = "(Desktop)";
  } else {
    TCHAR class_name[256];
    GetClassName(window_handle, class_name, 256);
    this->window_handle->Text =  gcnew String(class_name);
  }

  ResetClippingRegion();
             */
        }
        /// デスクトップを全画面で取り込む
        private void DoCaptureDesktopWindow()
        {
            /*
            SetWindow(GetDesktopWindow());
             */
        }
        /// クリッピング領域をリセットする
        private void ResetClippingRegion()
        {
            /*
            HWND window_handle = reinterpret_cast<HWND>(
                GetCurrentLayoutParameter()->window);

            if (window_handle == NULL || !IsWindow(window_handle))
            {
                return;
            }

            RECT window_rect;
            GetClientRect(window_handle, &window_rect);
            this->area_fit->Checked = true;

            // MinimumとMaximumはValueの更新前に更新しておかないと例外が発生する
            this->area_clipping_x->Minimum = window_rect.left;
            this->area_clipping_x->Maximum = window_rect.right;
            this->area_clipping_y->Minimum = window_rect.top;
            this->area_clipping_y->Maximum = window_rect.bottom;
            this->area_clipping_width->Maximum = window_rect.right;
            this->area_clipping_height->Maximum = window_rect.bottom;

            this->area_clipping_x->Value = window_rect.left;
            this->area_clipping_y->Value = window_rect.top;
            this->area_clipping_width->Value = window_rect.right;
            this->area_clipping_height->Value = window_rect.bottom;

            GetCurrentLayoutParameter()->clipping_x = window_rect.left;
            GetCurrentLayoutParameter()->clipping_y = window_rect.top;
            GetCurrentLayoutParameter()->clipping_width = window_rect.right;
            GetCurrentLayoutParameter()->clipping_height = window_rect.bottom;
             */
        }

        /// パラメータのValidate
        private bool ValidateParameters()
        {
            /*
  // もっとも危険な状態になりやすいウィンドウからチェック
  if (GetCurrentLayoutParameter()->window == 0) { // NULL
    MessageBox::Show("Specified window is invalid", "Invalid Window",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }
  HWND window_handle = reinterpret_cast<HWND>(GetCurrentLayoutParameter()->window);
  if (!IsWindow(window_handle)) {
    MessageBox::Show("Specified window is invalid", "Invalid Window",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }

  // クリッピングリージョンの判定
  RECT window_rect;
  GetClientRect(window_handle,&window_rect);
  if (GetCurrentLayoutParameter()->clipping_x +
      GetCurrentLayoutParameter()->clipping_width
      <= window_rect.right &&
      GetCurrentLayoutParameter()->clipping_y +
      GetCurrentLayoutParameter()->clipping_height
      <= window_rect.bottom &&
      GetCurrentLayoutParameter()->clipping_width > 0 &&
      GetCurrentLayoutParameter()->clipping_height > 0) {
    // nop 問題なし
  } else {
    MessageBox::Show("Clipping region is invalid", "Invalid Clipping Region",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }

  return true;
             */
            throw new NotImplementedException();
        }

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
                /*
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
              */
            }
            return 0;
        }

        /*
        scff_interprocess::LayoutParameter* GetCurrentLayoutParameter()
        {
            return GetLayoutParameterByIndex(editing_layout_index_);
        }
         */

        /// レイアウトパラメータ1
        /*
        scff_interprocess::LayoutParameter* layout1_parameter_;
        scff_interprocess::LayoutParameter* layout2_parameter_;
        scff_interprocess::LayoutParameter* layout3_parameter_;
        scff_interprocess::LayoutParameter* layout4_parameter_;
        scff_interprocess::LayoutParameter* layout5_parameter_;
        scff_interprocess::LayoutParameter* layout6_parameter_;
        scff_interprocess::LayoutParameter* layout7_parameter_;
        scff_interprocess::LayoutParameter* layout8_parameter_;
         */

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
            //GetCurrentLayoutParameter().clipping_x = area_clipping_x.Value;
        }
        private void area_clipping_y_ValueChanged(object sender, EventArgs e)
        {
            //GetCurrentLayoutParameter().clipping_y = area_clipping_y.Value;
        }
        private void area_clipping_width_ValueChanged(object sender, EventArgs e)
        {
            //GetCurrentLayoutParameter().clipping_width =
                //area_clipping_width.Value;
        }
        private void area_clipping_height_ValueChanged(object sender, EventArgs e)
        {
            //GetCurrentLayoutParameter().clipping_height =
                //area_clipping_height.Value;
        }
        private void option_show_mouse_cursor_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_mouse_cursor.Checked)
            {
                //GetCurrentLayoutParameter().show_cursor = 1;
            }
            else
            {
                //GetCurrentLayoutParameter().show_cursor = 0;
            }
        }
        private void option_show_layered_window_CheckedChanged(object sender, EventArgs e)
        {
            if (option_show_layered_window.Checked)
            {
                //GetCurrentLayoutParameter().show_layered_window = 1;
            }
            else
            {
                //GetCurrentLayoutParameter().show_layered_window = 0;
            }
        }
        private void option_keep_aspect_ratio_CheckedChanged(object sender, EventArgs e)
        {
            if (option_keep_aspect_ratio.Checked)
            {
                //GetCurrentLayoutParameter().keep_aspect_ratio = 1;
            }
            else
            {
                //GetCurrentLayoutParameter().keep_aspect_ratio = 0;
            }
        }
        private void option_enable_enlargement_CheckedChanged(object sender, EventArgs e)
        {
            if (option_enable_enlargement.Checked)
            {
                //GetCurrentLayoutParameter().stretch = 1;
            }
            else
            {
                //GetCurrentLayoutParameter().stretch = 0;
            }
        }
        private void option_over_sampling_CheckedChanged(object sender, EventArgs e) { }
        private void option_thread_num_ValueChanged(object sender, EventArgs e) { }
        private void option_resize_method_combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //GetCurrentLayoutParameter().sws_flags =
            //    option_resize_method_combo.SelectedValue;
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
