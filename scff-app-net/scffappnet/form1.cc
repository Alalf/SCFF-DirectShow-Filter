
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

/// @file scffappnet/form1.cc
/// @brief scffappnet::Form1の一部メソッドの定義

#include "Form1.h"

#include <Windows.h>
#include <ctime>

#include "scff-interprocess/interprocess.h"
#include "scffappnet/data_source.h"

namespace scffappnet {

// コンストラクタ
Form1::Form1(void)
    : can_use_dwmapi_dll_(false),
      was_dwm_enabled_on_start_(false),
      interprocess_(0) {    // NULL
  //---------------------------------------------------------------
  // DO NOT DELETE THIS!!!
  InitializeComponent();
  //---------------------------------------------------------------

  // DWMAPI.DLLが利用可能かどうか調べる
  OSVERSIONINFO os_info;
  os_info.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
  GetVersionEx(&os_info);
  if (os_info.dwPlatformId == 2 && os_info.dwMajorVersion >= 6) {
    can_use_dwmapi_dll_ = true;
  }

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

  // コントロールの準備
  BuildResizeMethodCombobox();

  // 編集中のレイアウトインデックス
  editing_layout_index_ = 0;
  // ListViewを選択する
  this->layout_list->Items[editing_layout_index_]->Selected = true;
  this->layout_list->Select();

  // ディレクトリ取得
  UpdateDirectory();

  // デフォルトの設定を書き込む
  DoCaptureDesktopWindow();
  this->option_keep_aspect_ratio->Checked = true;
  this->option_enable_enlargement->Checked = true;
  this->option_resize_method_combo->SelectedIndex = 9;

}

// デストラクタ
Form1::~Form1() {
  //---------------------------------------------------------------
  // DO NOT DELETE THIS!!!
  if (components) {
    delete components;
  }
  //---------------------------------------------------------------

  DWMAPIRestore();
  
  // レイアウトパラメータを全て消去
  delete layout1_parameter_;
  delete layout2_parameter_;
  delete layout3_parameter_;
  delete layout4_parameter_;
  delete layout5_parameter_;
  delete layout6_parameter_;
  delete layout7_parameter_;
  delete layout8_parameter_;

  // プロセス間通信に必要なオブジェクトの削除
  delete interprocess_;
}

// enum格納用
// Tupleの動作が不安定との情報を聞いたのでしょうがなく作った
ref class ResizeMethod {
 public:
  ResizeMethod(String^ name, scff_interprocess::SWScaleFlags flags) {
    MethodName = name;
    SWScaleFlags = flags;
  }
  property String^ MethodName;
  property scff_interprocess::SWScaleFlags SWScaleFlags;
};

// ResizeMethod ComboBoxにデータソースを設定する
void Form1::BuildResizeMethodCombobox() {
  // リストを新しく作成する
  ArrayList^ resize_methods = gcnew ArrayList();

  resize_methods->Add(gcnew ResizeMethod("FastBilinear (fast bilinear)", scff_interprocess::kFastBilinear));
  resize_methods->Add(gcnew ResizeMethod("Bilinear (bilinear)", scff_interprocess::kBilinear));
  resize_methods->Add(gcnew ResizeMethod("Bicubic (bicubic)", scff_interprocess::kBicubic));
  resize_methods->Add(gcnew ResizeMethod("X (experimental)", scff_interprocess::kX));
  resize_methods->Add(gcnew ResizeMethod("Point (nearest neighbor)", scff_interprocess::kPoint));
  resize_methods->Add(gcnew ResizeMethod("Area (averaging area)", scff_interprocess::kArea));
  resize_methods->Add(gcnew ResizeMethod("Bicublin (luma bicubic, chroma bilinear)", scff_interprocess::kBicublin));
  resize_methods->Add(gcnew ResizeMethod("Gauss (gaussian)", scff_interprocess::kGauss));
  resize_methods->Add(gcnew ResizeMethod("Sinc (sinc)", scff_interprocess::kSinc));
  resize_methods->Add(gcnew ResizeMethod("Lanczos (natural)", scff_interprocess::kLanczos));
  resize_methods->Add(gcnew ResizeMethod("Spline (natural bicubic spline)", scff_interprocess::kSpline));

  this->option_resize_method_combo->DataSource = resize_methods;
  this->option_resize_method_combo->DisplayMember = "MethodName";
  this->option_resize_method_combo->ValueMember = "SWScaleFlags";
  this->option_resize_method_combo->Enabled = true;
  this->option_resize_method_combo->SelectedIndex = 0;
}

// 共有メモリからディレクトリを取得し、いろいろ処理
void Form1::UpdateDirectory() {
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
}

// 共有メモリにNullLayoutリクエストを設定
void Form1::SendNullLayoutRequest() {
  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = timestamp;
  message.layout_type = scff_interprocess::kNullLayout;
    
  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
}

/// @brief 共有メモリにNativeLayoutリクエストを設定
void Form1::SendNativeLayoutRequest() {
  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = timestamp;
  message.layout_type = scff_interprocess::kNativeLayout;
  // 無視される
  message.layout_element_count = 1;
  message.layout_parameters[0].bound_x = 0;
  message.layout_parameters[0].bound_y = 0;
  message.layout_parameters[0].bound_width = 0;
  message.layout_parameters[0].bound_height = 0;
  // ここまで無視
  message.layout_parameters[0].window = GetCurrentLayoutParameter()->window;
  message.layout_parameters[0].clipping_x = GetCurrentLayoutParameter()->clipping_x;
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
}

/// @brief 共有メモリにComplexLayoutリクエストを設定
void Form1::SendComplexLayoutRequest() {
  /// @todo(me) テスト中！あとで直す！

  // メッセージを書いて送る
  scff_interprocess::Message message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = timestamp;
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
}

void Form1::DoCaptureDesktopWindow() {
  SetWindow(GetDesktopWindow());
}

// クリッピング領域をリセットする
void Form1::ResetClippingRegion() {
  HWND window_handle = reinterpret_cast<HWND>(
      GetCurrentLayoutParameter()->window);

  if (window_handle == NULL || !IsWindow(window_handle)) {
    return;
  }

  RECT window_rect;
  GetClientRect(window_handle,&window_rect);
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
}

// ウィンドウを指定する
void Form1::SetWindow(HWND window_handle) {
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
}

/// @brief パラメータのValidate
bool Form1::ValidateParameters() {
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
}
}   // namespace scffappnet
