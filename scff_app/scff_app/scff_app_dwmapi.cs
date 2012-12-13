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

/// @file scff_app/scff_app_dwmapi.cs
/// SCFFAppのDWMAPI関連のメソッドの定義

namespace scff_app {

using System;

// MVCパターンにおけるController
partial class SCFFApp {

  /// Dwmapi.dllを利用してAeroをOffに
  public void DWMAPIOff() {
    if (!CanUseDWMAPIDLL()) {
      // dwmapi.dllを利用できなければ何もしない
      was_dwm_enabled_on_start_ = false;
      return;
    }

    bool was_dwm_enabled_on_start;
    ExternalDWMAPI.DwmIsCompositionEnabled(out was_dwm_enabled_on_start);
    if (was_dwm_enabled_on_start) {
      ExternalDWMAPI.DwmEnableComposition(ExternalDWMAPI.DWM_EC_DISABLECOMPOSITION);
    }
    was_dwm_enabled_on_start_ = was_dwm_enabled_on_start;
  }

  /// 強制的にAeroのOn/Offを切り替える
  public void DWMAPIFlip(bool current) {
    if (!CanUseDWMAPIDLL()) {
      // dwmapi.dllを利用できなければ何もしない
      return;
    }

    if (current) {
      ExternalDWMAPI.DwmEnableComposition(ExternalDWMAPI.DWM_EC_DISABLECOMPOSITION);
    } else {
      ExternalDWMAPI.DwmEnableComposition(ExternalDWMAPI.DWM_EC_ENABLECOMPOSITION);
    }
  }

  /// AeroをOffにしていたらOnに戻す
  public void DWMAPIRestore() {
    if (!CanUseDWMAPIDLL()) {
      // dwmapi.dllを利用できなければ何もしない
      return;
    }

    if (was_dwm_enabled_on_start_) {
      ExternalDWMAPI.DwmEnableComposition(ExternalDWMAPI.DWM_EC_ENABLECOMPOSITION);
    }
  }

  //-------------------------------------------------------------------

  /// DWMAPI.DLLが利用可能かどうか
  bool CanUseDWMAPIDLL() {
    if (Environment.OSVersion.Platform == PlatformID.Win32NT &&
        Environment.OSVersion.Version.Major >= 6) {
      return true;
    } else {
      return false;
    }
  }

  /// Aeroが起動時にONになっていたかどうか
  bool was_dwm_enabled_on_start_ = false;
}
}   // namespace scff_app
