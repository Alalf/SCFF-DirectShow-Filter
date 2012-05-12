
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

/// @file scff-app/form1-dwmapi.cs
/// @brief Form1(メインウィンドウ)のDWMAPI関連のメソッドの定義

using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace scff_app {

// メインウィンドウ
public partial class AppImplementation {

  /// @brief API: DwmIsCompositionEnabled
  [DllImport("dwmapi.dll")]
  private static extern int DwmIsCompositionEnabled(out bool enabled);
  /// @brief API: DwmEnableComposition
  [DllImport("dwmapi.dll")]
  private static extern int DwmEnableComposition(uint uCompositionAction);

  /// @brief Disable DWM
  const int DWM_EC_DISABLECOMPOSITION = 0;
  /// @brief Enable DWM
  const int DWM_EC_ENABLECOMPOSITION = 1;

  /// @brief Dwmapi.dllを利用してAeroをOffに
  public void DWMAPIOff() {
    if (!can_use_dwmapi_dll_) {
      // dwmapi.dllを利用できなければ何もしない
      was_dwm_enabled_on_start_ = false;
      return;
    }

    bool was_dwm_enabled_on_start;
    DwmIsCompositionEnabled(out was_dwm_enabled_on_start);
    if (was_dwm_enabled_on_start) {
      //DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
    } else {
    }
    was_dwm_enabled_on_start_ = was_dwm_enabled_on_start == true;
  }

  /// @brief 強制的にAeroのOn/Offを切り替える
  public void DWMAPIFlip(bool current) {
    if (!can_use_dwmapi_dll_) {
      // dwmapi.dllを利用できなければ何もしない
      return;
    }

    if (current) {
      DwmEnableComposition(DWM_EC_DISABLECOMPOSITION);
    } else {
      DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
    }
  }

  /// @brief AeroをOffにしていたらOnに戻す
  public void DWMAPIRestore() {
    if (!can_use_dwmapi_dll_) {
      // dwmapi.dllを利用できなければ何もしない
      return;
    }

    if (was_dwm_enabled_on_start_) {
      DwmEnableComposition(DWM_EC_ENABLECOMPOSITION);
    }
  }

  /// @brief Dwmapi.dllが利用可能かどうか
  private bool can_use_dwmapi_dll_;
  /// @brief Aeroが起動時にONになっていたかどうか
  private bool was_dwm_enabled_on_start_;
}
}   // namespace scff_app
