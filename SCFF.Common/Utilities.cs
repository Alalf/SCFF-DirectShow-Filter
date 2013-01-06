// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
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

/// @file SCFF.Common/Utilities.cs
/// @copydoc SCFF::Common::Utilities

namespace SCFF.Common {

using System;
using System.Text;
using SCFF.Common.Ext;

/// SCFF.Commonモジュール共通で利用する機能
public static class Utilities {
  //===================================================================
  // 特定のOSに依存しないDesktopListViewWindowの取得
  //===================================================================

  /// 特定のOSに依存しないDesktopListViewWindowの取得
  ///
  /// デスクトップのHWNDの階層関係は以下のとおり:
  /// - GetDesktopWindow()
  ///   - Progman (XP/Win7(No Aero)/Vista(No Aero))
  ///     - SHELLDLL_DefView (XP/Win7 No Aero/Vista No Aero?)
  ///       - Internet Exproler_Server (XP Active Desktop)
  ///       - SysListView32 (XP?/Win7 No Aero/Vista No Aero?)
  ///   - WorkerW[/WorkerW]* (Win7 Aero/Vista Aero?)
  ///     - SHELLDLL_DefView
  ///       - SysListView32
  ///   - EdgeUiInputWndClass (Win 8)
  ///
  /// パッと見る限り明らかに重いのはAero On時。EnumWindows必須。
  public static UIntPtr DesktopListViewWindow {
    get {
      UIntPtr progman = User32.FindWindowEx(UIntPtr.Zero,
          UIntPtr.Zero, "Progman", null);
      if (progman != UIntPtr.Zero) {
        // XP/Win7(No Aero)/Vista(No Aero)
        UIntPtr shellDLLDefView = User32.FindWindowEx(progman,
            UIntPtr.Zero, "SHELLDLL_DefView", null);
        if (shellDLLDefView != UIntPtr.Zero) {
          UIntPtr sysListView32 = User32.FindWindowEx(shellDLLDefView,
              UIntPtr.Zero, "SysListView32", null);
          if (sysListView32 != UIntPtr.Zero) {
            // XP(No ActiveDesktop)/Win7(No Aero)/Vista(No Aero)
            return sysListView32;
          } 
          UIntPtr internetExprolerServer = User32.FindWindowEx(shellDLLDefView,
              UIntPtr.Zero, "Internet Exproler_Server", null);
          if (internetExprolerServer != UIntPtr.Zero) {
            // XP(ActiveDesktop)
            return internetExprolerServer;
          }
        }
      }
      UIntPtr edgeUiInputWndClass = User32.FindWindowEx(UIntPtr.Zero,
          UIntPtr.Zero, "EdgeUiInputWndClass", null);
      if (edgeUiInputWndClass != UIntPtr.Zero) {
        // Win8
        return edgeUiInputWndClass;
      }
      enumerateWindowResult = UIntPtr.Zero;
      User32.EnumWindows(new User32.WNDENUMProc(EnumerateWindow), IntPtr.Zero);
      if (enumerateWindowResult != UIntPtr.Zero) {
        // Win7(Aero)/Vista(Aero)
        UIntPtr sysListView32 = User32.FindWindowEx(enumerateWindowResult,
            UIntPtr.Zero, "SysListView32", null);
        if (sysListView32 != UIntPtr.Zero) {
          return sysListView32;
        }
      }
      return User32.GetDesktopWindow();
    }
  }

  /// EnumerateWindowの結果を格納するポインタ
  private static UIntPtr enumerateWindowResult = UIntPtr.Zero;
  /// FindWindowExに渡されるWindow列挙関数
  private static bool EnumerateWindow(UIntPtr hWnd, IntPtr lParam) {
    StringBuilder className = new StringBuilder(256);
    User32.GetClassName(hWnd, className, 256);
    // "WorkerW"以外はスキップ
    if (className.ToString() != "WorkerW") return true;

    // "WorkerW" > "SHELLDLL_DefView"になってなければスキップ
    UIntPtr shellDLLDefView = User32.FindWindowEx(hWnd, UIntPtr.Zero, "SHELLDLL_DefView", null);
    if (shellDLLDefView == UIntPtr.Zero) return true;
      
    enumerateWindowResult = shellDLLDefView;
    return false;
  }
}
}   // namespace SCFF.Common
