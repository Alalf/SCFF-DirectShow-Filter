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

/// @file scff-app/data/layout-parameter-app.cs
/// @brief LayoutParameterのscff_app用拡張

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace scff_app.data {

// scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
public partial class LayoutParameter {
  //-------------------------------------------------------------------
  // Wrappers
  //-------------------------------------------------------------------
  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  static extern int GetClassName(UIntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
  [DllImport("user32.dll", SetLastError = false)]
  static extern UIntPtr GetDesktopWindow();
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsWindow(UIntPtr hWnd);
  //-------------------------------------------------------------------

  /// @brief レイアウトの名前代わりに使用するWindowのクラス名
  public string WindowText {
    get {
      if (Window == UIntPtr.Zero) {
        return "(Splash)";
      } else if (Window == GetDesktopWindow()) {
        return "(Desktop)";
      } else {
        if (!IsWindow(Window)) {
          return "*** INVALID WINDOW ***";
        } else {
          StringBuilder class_name = new StringBuilder(256);
          GetClassName(Window, class_name, 256);
          return class_name.ToString();
        }
      }
    }
  }

  //-------------------------------------------------------------------
  // scff_app独自の値 (Messageには書き込まれない)
  //-------------------------------------------------------------------

  /// @brief 0.0-1.0を境界の幅としたときの境界内の左端の座標
  public Double BoundRelativeLeft { get; set; }
  /// @brief 0.0-1.0を境界の幅としたときの境界内の右端の座標
  public Double BoundRelativeRight { get; set; }
  /// @brief 0.0-1.0を境界の高さとしたときの境界内の上端の座標
  public Double BoundRelativeTop { get; set; }
  /// @brief 0.0-1.0を境界の高さとしたときの境界内の下端の座標
  public Double BoundRelativeBottom { get; set; }

  /// @brief Clipping領域のFitオプション
  public Boolean Fit { get; set; }
}
}
