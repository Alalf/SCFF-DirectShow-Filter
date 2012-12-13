// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file scff_app/external_dwmapi.cs
/// scff_appモジュールで利用するDWMAPIをまとめたクラスの定義

namespace scff_app {

using System.Runtime.InteropServices;

/// scff_appモジュールで利用するDWMAPIをまとめたクラス
class ExternalDWMAPI {
  /// 生成禁止用コンストラクタ
  private ExternalDWMAPI() {}

  //-------------------------------------------------------------------
  // dwmapi.dll
  //-------------------------------------------------------------------
  [DllImport("dwmapi.dll")]
  internal static extern int DwmIsCompositionEnabled(out bool enabled);
  [DllImport("dwmapi.dll")]
  internal static extern int DwmEnableComposition(uint uCompositionAction);

  internal const int DWM_EC_DISABLECOMPOSITION = 0;
  internal const int DWM_EC_ENABLECOMPOSITION = 1;
}

}