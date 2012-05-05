
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
/// @brief scffappnet::Form1��DwmAPI�֘A�̃��\�b�h�̒�`

#include "Form1.h"

#include <Dwmapi.h>

namespace scffappnet {

// Dwmapi.dll�𗘗p����Aero��Off��
void Form1::DWMAPIOff() {
  if (!can_use_dwmapi_dll_) {
    // dwmapi.dll�𗘗p�ł��Ȃ���Ή������Ȃ�
    was_dwm_enabled_on_start_ = false;
    return;
  }

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
}

// �����I��Aero��On/Off��؂�ւ���
void Form1::DWMAPIFlip() {
  if (!can_use_dwmapi_dll_) {
    // dwmapi.dll�𗘗p�ł��Ȃ���Ή������Ȃ�
    return;
  }

  if (this->aero_on_item->Checked) {
    DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
  } else {
    DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
  }
  this->aero_on_item->Checked = !(this->aero_on_item->Checked);
}

// Aero��Off�ɂ��Ă�����On�ɖ߂�
void Form1::DWMAPIRestore() {
  if (!can_use_dwmapi_dll_) {
    // dwmapi.dll�𗘗p�ł��Ȃ���Ή������Ȃ�
    return;
  }

  if (was_dwm_enabled_on_start_) {
    DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
    Diagnostics::Debug::WriteLine("Aero OFF to ON");
  }
}

}   // namespace scffappnet
