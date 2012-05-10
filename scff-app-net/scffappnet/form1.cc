﻿
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
      interprocess_(0),         // NULL
      layout_parameter_(0) {    // NULL
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
  layout_parameter_ = new scff_interprocess::LayoutParameter;
  ZeroMemory(layout_parameter_,sizeof(scff_interprocess::LayoutParameter));

  // コントロールの準備
  BuildResizeMethodCombobox();

  // 編集中のレイアウトインデックス
  editing_layout_index_ = 0;
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
  message.timestamp = static_cast<int32_t>(timestamp);
  message.layout_type = scff_interprocess::kNullLayout;
    
  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->set_process_id(process_id);
    interprocess_->InitMessage();
    interprocess_->SendMessage(message);
  }
}

/// @brief 共有メモリにNativeLayoutリクエストを設定
void Form1::SendNativeLayoutRequest() {
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
  message.layout_parameters[0].window = this->layout_parameter_->window;
  message.layout_parameters[0].clipping_x = this->layout_parameter_->clipping_x;
  message.layout_parameters[0].clipping_y = this->layout_parameter_->clipping_y;
  message.layout_parameters[0].clipping_width = this->layout_parameter_->clipping_width;
  message.layout_parameters[0].clipping_height = this->layout_parameter_->clipping_height;
  message.layout_parameters[0].show_cursor = this->layout_parameter_->show_cursor;
  message.layout_parameters[0].show_layered_window = this->layout_parameter_->show_layered_window;
  message.layout_parameters[0].sws_flags = this->layout_parameter_->sws_flags;
  message.layout_parameters[0].stretch = this->layout_parameter_->stretch;
  message.layout_parameters[0].keep_aspect_ratio = this->layout_parameter_->keep_aspect_ratio;

  // 共有メモリを開いて送る
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->set_process_id(process_id);
    interprocess_->InitMessage();
    interprocess_->SendMessage(message);
  }
}

void Form1::DoCaptureDesktopWindow() {
  SetWindow(GetDesktopWindow());
}

// クリッピング領域をリセットする
void Form1::ResetClippingRegion() {
  HWND window_handle = reinterpret_cast<HWND>(
      this->layout_parameter_->window);

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
  
  this->layout_parameter_->clipping_x = window_rect.left;
  this->layout_parameter_->clipping_y = window_rect.top;
  this->layout_parameter_->clipping_width = window_rect.right;
  this->layout_parameter_->clipping_height = window_rect.bottom;  
}

// ウィンドウを指定する
void Form1::SetWindow(HWND window_handle) {
  uint64_t window = reinterpret_cast<uint64_t>(window_handle);
  this->layout_parameter_->window = window;
  
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
  if (this->layout_parameter_->window == 0) { // NULL
    MessageBox::Show("Specified window is invalid", "Invalid Window",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }
  HWND window_handle = reinterpret_cast<HWND>(this->layout_parameter_->window);
  if (!IsWindow(window_handle)) {
    MessageBox::Show("Specified window is invalid", "Invalid Window",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }

  // クリッピングリージョンの判定
  RECT window_rect;
  GetClientRect(window_handle,&window_rect);
  if (this->layout_parameter_->clipping_x +
      this->layout_parameter_->clipping_width
      <= window_rect.right &&
      this->layout_parameter_->clipping_y +
      this->layout_parameter_->clipping_height
      <= window_rect.bottom &&
      this->layout_parameter_->clipping_width > 0 &&
      this->layout_parameter_->clipping_height > 0) {
    // nop 問題なし
  } else {
    MessageBox::Show("Clipping region is invalid", "Invalid Clipping Region",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }

  return true;
}
}   // namespace scffappnet
