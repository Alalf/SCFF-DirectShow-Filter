
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
#include "AreaSelectForm.h"

namespace scffappnet {

//-------------------------------------------------------------------
// Form
//-------------------------------------------------------------------

System::Void Form1::Form1_Shown(System::Object^  sender, System::EventArgs^  e) {
  DWMAPIOff();
}

//-------------------------------------------------------------------
// Menu
//-------------------------------------------------------------------

System::Void Form1::aero_on_item_Click(System::Object^  sender, System::EventArgs^  e) {
  DWMAPIFlip();
}

//-------------------------------------------------------------------
// OK/Cancel (Apply/Splash)
//-------------------------------------------------------------------

System::Void Form1::splash_Click(System::Object^  sender, System::EventArgs^  e) {
  SendNullLayoutRequest();
}

System::Void Form1::apply_Click(System::Object^  sender, System::EventArgs^  e) {
  if (ValidateParameters()) {
    SendNativeLayoutRequest();
  }
}

//-------------------------------------------------------------------
// Process
//-------------------------------------------------------------------

System::Void Form1::process_refresh_Click(System::Object^  sender, System::EventArgs^  e) {
  // ディレクトリから更新
  UpdateDirectory();
}

System::Void Form1::process_combo_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) {
  Diagnostics::Debug::WriteLine("Index Changed");
}

//-------------------------------------------------------------------
// Window
//-------------------------------------------------------------------

System::Void Form1::window_draghere_MouseDown(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
  Diagnostics::Debug::WriteLine("Mouse Down!");
  this->window_draghere->BackColor = Color::Orange;
}

System::Void Form1::window_draghere_MouseUp(System::Object^  sender, System::Windows::Forms::MouseEventArgs^  e) {
  Diagnostics::Debug::WriteLine("Mouse up!");
  this->window_draghere->BackColor = SystemColors::Control;

  Point location = this->window_draghere->PointToScreen(e->Location);
  POINT point;
  point.x = location.X;
  point.y = location.Y;

  HWND window_handle = WindowFromPoint(point);
  if (window_handle != NULL) {
    // 見つかった場合
    SetWindow(window_handle);
    Diagnostics::Debug::WriteLine(location);
  } else {
    // nop
  }
}

System::Void Form1::window_desktop_Click(System::Object^  sender, System::EventArgs^  e) {
  DoCaptureDesktopWindow();
}

//-------------------------------------------------------------------
// Area
//-------------------------------------------------------------------

System::Void Form1::area_fit_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  if (this->area_fit->Checked) {
    this->area_clipping_x->Enabled = false;
    this->area_clipping_y->Enabled = false;
    this->area_clipping_width->Enabled = false;
    this->area_clipping_height->Enabled = false;

    ResetClippingRegion();
  } else {
    this->area_clipping_x->Enabled = true;
    this->area_clipping_y->Enabled = true;
    this->area_clipping_width->Enabled = true;
    this->area_clipping_height->Enabled = true;
  }
}

System::Void Form1::area_clipping_x_ValueChanged(System::Object^  sender, System::EventArgs^  e) {
  this->layout_parameter_->clipping_x =
      static_cast<int32_t>(this->area_clipping_x->Value);
}

System::Void Form1::area_clipping_y_ValueChanged(System::Object^  sender, System::EventArgs^  e) {
  this->layout_parameter_->clipping_y =
      static_cast<int32_t>(this->area_clipping_y->Value);
}

System::Void Form1::area_clipping_width_ValueChanged(System::Object^  sender, System::EventArgs^  e) {
  this->layout_parameter_->clipping_width =
      static_cast<int32_t>(this->area_clipping_width->Value);
}

System::Void Form1::area_clipping_height_ValueChanged(System::Object^  sender, System::EventArgs^  e) {
  this->layout_parameter_->clipping_height =
      static_cast<int32_t>(this->area_clipping_height->Value);
}

System::Void Form1::target_area_select_Click(System::Object^  sender, System::EventArgs^  e) {
  AreaSelectForm^ form = gcnew AreaSelectForm();
  Point new_loc((int)this->area_clipping_x->Value,
                (int)this->area_clipping_y->Value);
  form->Location = new_loc;
  System::Drawing::Size new_size((int)this->area_clipping_width->Value,
                                 (int)this->area_clipping_height->Value);
  form->Size = new_size;
  form->ShowDialog();

  // デスクトップキャプチャに変更
  DoCaptureDesktopWindow();
  // FitをはずしてClippingをかく
  this->area_fit->Checked = false;
  this->area_clipping_x->Value = form->clipping_x;
  this->area_clipping_y->Value = form->clipping_y;
  this->area_clipping_width->Value = form->clipping_width;
  this->area_clipping_height->Value = form->clipping_height;
}

//-------------------------------------------------------------------
// Option
//-------------------------------------------------------------------

System::Void Form1::option_show_mouse_cursor_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  if (this->option_show_mouse_cursor->Checked) {
    this->layout_parameter_->show_cursor = 1;
  } else {
    this->layout_parameter_->show_cursor = 0;
  }
}

System::Void Form1::option_show_layered_window_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  if (this->option_show_layered_window->Checked) {
    this->layout_parameter_->show_layered_window = 1;
  } else {
    this->layout_parameter_->show_layered_window = 0;
  }
}

System::Void Form1::option_keep_aspect_ratio_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  if (this->option_keep_aspect_ratio->Checked) {
    this->layout_parameter_->keep_aspect_ratio = 1;
  } else {
    this->layout_parameter_->keep_aspect_ratio = 0;
  }
}

System::Void Form1::option_enable_enlargement_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  if (this->option_enable_enlargement->Checked) {
    this->layout_parameter_->stretch = 1;
  } else {
    this->layout_parameter_->stretch = 0;
  }
}

System::Void Form1::option_over_sampling_CheckedChanged(System::Object^  sender, System::EventArgs^  e) {
  // this->option_over_sampling->Checked) {
  /// @todo(me) 実装
}

System::Void Form1::option_thread_num_ValueChanged(System::Object^  sender, System::EventArgs^  e) {
  // this->option_thread_num->Value
  /// @todo(me) 実装
}

System::Void Form1::option_resize_method_combo_SelectedIndexChanged(System::Object^  sender, System::EventArgs^  e) {
  this->layout_parameter_->sws_flags =
      static_cast<int32_t>(this->option_resize_method_combo->SelectedValue);
}
}   // namespace scffappnet
