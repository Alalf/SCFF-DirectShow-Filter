
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
/// @brief scffappnet::Form1�̈ꕔ���\�b�h�̒�`

#include "Form1.h"

#include <Windows.h>
#include <Dwmapi.h>

#include <ctime>

#include "base/scff-interprocess.h"
#include "scffappnet/data_source.h"


namespace scffappnet {

// �R���X�g���N�^
Form1::Form1(void) {
  //---------------------------------------------------------------
  // DO NOT DELETE THIS!!!
  InitializeComponent();
  //---------------------------------------------------------------

  // �܂���Aero�̏�Ԃ�ۑ����āA�����I�ɖ����ɂ���
  BOOL was_dwm_enabled_on_start;
  DwmIsCompositionEnabled(&was_dwm_enabled_on_start);
  if (was_dwm_enabled_on_start) {
    DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
    Diagnostics::Debug::WriteLine("Aero ON to OFF");
  } else {
    Diagnostics::Debug::WriteLine("Aero Already OFF");
  }
  this->aero_on_item->Checked = false;
  was_dwm_enabled_on_start_ = was_dwm_enabled_on_start == TRUE;

  // �v���Z�X�ԒʐM�ɕK�v�ȃI�u�W�F�N�g�̐���
  interprocess_ = new SCFFInterprocess;
  // ���C�A�E�g�p�����[�^���i�[���邽�߂̃I�u�W�F�N�g�𐶐�
  layout_parameter_ = new SCFFLayoutParameter;
  ZeroMemory(layout_parameter_,sizeof(SCFFLayoutParameter));

  // �R���g���[���̏���
  BuildResizeMethodCombobox();

  // �ҏW���̃��C�A�E�g�C���f�b�N�X
  editing_layout_index_ = 0;
  // �f�B���N�g���擾
  UpdateDirectory();

  // �f�t�H���g�̐ݒ����������
  DoCaptureDesktopWindow();
  this->option_keep_aspect_ratio->Checked = true;
  this->option_enable_enlargement->Checked = true;
  this->option_resize_method_combo->SelectedIndex = 9;

}

// �f�X�g���N�^
Form1::~Form1() {
  //---------------------------------------------------------------
  // DO NOT DELETE THIS!!!
  if (components) {
    delete components;
  }
  //---------------------------------------------------------------

  // Aero�̏�Ԃ����ɖ߂�
  if (was_dwm_enabled_on_start_) {
    DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
    Diagnostics::Debug::WriteLine("Aero OFF to ON");
  }

  // �v���Z�X�ԒʐM�ɕK�v�ȃI�u�W�F�N�g�̍폜
  delete interprocess_;
}

// enum�i�[�p
// Tuple�̓��삪�s����Ƃ̏��𕷂����̂ł��傤���Ȃ������
ref class ResizeMethod {
 public:
  ResizeMethod(String^ name, SCFFSWScaleFlags flags) {
    MethodName = name;
    SWScaleFlags = flags;
  }
  property String^ MethodName;
  property SCFFSWScaleFlags SWScaleFlags;
};

// ResizeMethod ComboBox�Ƀf�[�^�\�[�X��ݒ肷��
void Form1::BuildResizeMethodCombobox() {
  // ���X�g��V�����쐬����
  ArrayList^ resize_methods = gcnew ArrayList();

  resize_methods->Add(gcnew ResizeMethod("fast bilinear", kSCFFFastBilinear));
  resize_methods->Add(gcnew ResizeMethod("bilinear", kSCFFBilinear));
  resize_methods->Add(gcnew ResizeMethod("bicubic", kSCFFBicubic));
  resize_methods->Add(gcnew ResizeMethod("experimental", kSCFFX));
  resize_methods->Add(gcnew ResizeMethod("nearest neighbor", kSCFFPoint));
  resize_methods->Add(gcnew ResizeMethod("averaging area", kSCFFArea));
  resize_methods->Add(gcnew ResizeMethod("luma bicubic, chroma bilinear", kSCFFBicublin));
  resize_methods->Add(gcnew ResizeMethod("gaussian", kSCFFGauss));
  resize_methods->Add(gcnew ResizeMethod("sinc", kSCFFSinc));
  resize_methods->Add(gcnew ResizeMethod("natural", kSCFFLanczos));
  resize_methods->Add(gcnew ResizeMethod("natural bicubic spline", kSCFFSpline));

  this->option_resize_method_combo->DataSource = resize_methods;
  this->option_resize_method_combo->DisplayMember = "MethodName";
  this->option_resize_method_combo->ValueMember = "SWScaleFlags";
  this->option_resize_method_combo->Enabled = true;
  this->option_resize_method_combo->SelectedIndex = 0;
}

// ���L����������f�B���N�g�����擾���A���낢�돈��
void Form1::UpdateDirectory() {
  // ���L����������f�[�^���擾
  interprocess_->InitDirectory();
  SCFFDirectory directory;
  interprocess_->GetDirectory(&directory);
  Diagnostics::Debug::WriteLine("Get Directory");

  // ���X�g��V�����쐬����
  ArrayList^ managed_directory = gcnew ArrayList();

  // �R���{�{�b�N�X�̓��e���\�z
  for (int i = 0; i < kSCFFMaxEntry; i++) {
    if (directory.entries[i].process_id == 0) continue;
    ManagedSCFFEntry^ entry = gcnew ManagedSCFFEntry(directory.entries[i]);
    managed_directory->Add(entry);
  }
  this->process_combo->DataSource = managed_directory;

  if (managed_directory->Count > 0) {
    // SCFH DSF����
    this->process_combo->DisplayMember = "Info";
    this->process_combo->ValueMember = "ProcessID";
    this->process_combo->Enabled = true;
    this->process_combo->SelectedIndex =0;

    // ���b�Z�[�W�𑗂邽�߂̃{�^����L����
    this->splash->Enabled = true;
    this->apply->Enabled = true;
  } else {
    // SCFH DSF�����[�h����Ă��Ȃ��ꍇ
    this->process_combo->Enabled = false;

    // ���b�Z�[�W�𑗂邽�߂̃{�^���𖳌���
    this->splash->Enabled = false;
    this->apply->Enabled = false;
  }
}

// ���L��������NullLayout���N�G�X�g��ݒ�
void Form1::SendNullLayoutRequest() {
  // ���b�Z�[�W�������đ���
  SCFFMessage message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = static_cast<int32_t>(timestamp);
  message.layout_type = kSCFFNullLayout;
    
  // ���L���������J���đ���
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
}

/// @brief ���L��������NativeLayout���N�G�X�g��ݒ�
void Form1::SendNativeLayoutRequest() {
  // ���b�Z�[�W�������đ���
  SCFFMessage message;
  time_t timestamp;
  time(&timestamp);
  message.timestamp = static_cast<int64_t>(timestamp);
  message.layout_type = kSCFFNativeLayout;
  // ���������
  message.layout_element_count = 1;
  message.layout_parameters[0].bound_x = 0;
  message.layout_parameters[0].bound_y = 0;
  message.layout_parameters[0].bound_width = 0;
  message.layout_parameters[0].bound_height = 0;
  // �����܂Ŗ���
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

  // ���L���������J���đ���
  if (this->process_combo->SelectedValue != nullptr) {
    uint32_t process_id = (uint32_t)(this->process_combo->SelectedValue);
    interprocess_->InitMessage(process_id);
    interprocess_->SendMessage(message);
  }
}

void Form1::DoAeroOn() {
  if (this->aero_on_item->Checked) {
    DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
  } else {
    DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
  }
  this->aero_on_item->Checked = !(this->aero_on_item->Checked);
}

void Form1::DoCaptureDesktopWindow() {
  SetWindow(GetDesktopWindow());
}

// �N���b�s���O�̈�����Z�b�g����
void Form1::ResetClippingRegion() {
  HWND window_handle = reinterpret_cast<HWND>(
      this->layout_parameter_->window);

  if (window_handle == NULL || !IsWindow(window_handle)) {
    return;
  }

  RECT window_rect;
  GetClientRect(window_handle,&window_rect);
  this->area_fit->Checked = true;

  // Minimum��Maximum��Value�̍X�V�O�ɍX�V���Ă����Ȃ��Ɨ�O����������
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

// �E�B���h�E���w�肷��
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

/// @brief �p�����[�^��Validate
bool Form1::ValidateParameters() {
  // �����Ƃ��댯�ȏ�ԂɂȂ�₷���E�B���h�E����`�F�b�N
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

  // �N���b�s���O���[�W�����̔���
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
    // nop ���Ȃ�
  } else {
    MessageBox::Show("Clipping region is invalid", "Invalid Clipping Region",
        MessageBoxButtons::OK, MessageBoxIcon::Error);
    return false;
  }

  return true;
}

}   // namespace scffappnet