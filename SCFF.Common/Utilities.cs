// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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
/// ユーティリティメソッドを集めたstaticクラス

namespace SCFF.Common {

using System;
using System.Text;

/// ユーティリティメソッドを集めたstaticクラス
public static class Utilities {

  /// Desktop(VirtualScreen)座標からScreen座標へ変換する
  private static void DesktopToScreen(int desktopX, int desktopY, out int screenX, out int screenY) {
    // desktopPointは仮想画面上の座標(左上が(0,0)であることが保障されている)
    // screenPointはプライマリモニタの左上の座標が(0,0)なので-になることもある
    screenX = desktopX + ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN);
    screenY = desktopY + ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN);
  }

  // DesktopListViewWindow
  //
  // GetDesktopWindow()
  //   - Progman (XP/Win7(No Aero)/Vista(No Aero))
  //     - SHELLDLL_DefView (XP/Win7 No Aero/Vista No Aero?)
  //       - Internet Exproler_Server (XP Active Desktop)
  //       - SysListView32 (XP?/Win7 No Aero/Vista No Aero?)
  //   - WorkerW[/WorkerW]* (Win7 Aero/Vista Aero?)
  //     - SHELLDLL_DefView
  //       - SysListView32
  //   - EdgeUiInputWndClass (Win 8)
  // パッと見る限り明らかに重いのはAero On時。EnumWindows必須。

  private static UIntPtr enumerateWindowResult = UIntPtr.Zero;
  private static bool EnumerateWindow(UIntPtr hWnd, UIntPtr lParam) {
    StringBuilder className = new StringBuilder(256);
    ExternalAPI.GetClassName(hWnd, className, 256);
    if (className.ToString() == "WorkerW") {
      UIntPtr shellDLLDefView = ExternalAPI.FindWindowEx(hWnd, UIntPtr.Zero, "SHELLDLL_DefView", null);
      if (shellDLLDefView != UIntPtr.Zero) {
        enumerateWindowResult = shellDLLDefView;
        return false;
      }
    }
    return true;
  }

  public static UIntPtr DesktopListViewWindow {
    get {
      UIntPtr progman = ExternalAPI.FindWindowEx(UIntPtr.Zero, UIntPtr.Zero, "Progman", null);
      if (progman != UIntPtr.Zero) {
        // XP/Win7(No Aero)/Vista(No Aero)
        UIntPtr shellDLLDefView = ExternalAPI.FindWindowEx(progman, UIntPtr.Zero, "SHELLDLL_DefView", null);
        if (shellDLLDefView != UIntPtr.Zero) {
          UIntPtr sysListView32 = ExternalAPI.FindWindowEx(shellDLLDefView, UIntPtr.Zero, "SysListView32", null);
          if (sysListView32 != UIntPtr.Zero) {
            // XP(No ActiveDesktop)/Win7(No Aero)/Vista(No Aero)
            return sysListView32;
          } 
          UIntPtr internetExprolerServer = ExternalAPI.FindWindowEx(shellDLLDefView, UIntPtr.Zero, "Internet Exproler_Server", null);
          if (internetExprolerServer != UIntPtr.Zero) {
            // XP(ActiveDesktop)
            return internetExprolerServer;
          }
        }
      }
      UIntPtr edgeUiInputWndClass = ExternalAPI.FindWindowEx(UIntPtr.Zero, UIntPtr.Zero, "EdgeUiInputWndClass", null);
      if (edgeUiInputWndClass != UIntPtr.Zero) {
        // Win8
        return edgeUiInputWndClass;
      }
      enumerateWindowResult = UIntPtr.Zero;
      ExternalAPI.EnumWindows(new ExternalAPI.WNDENUMProc(EnumerateWindow), UIntPtr.Zero);
      if (enumerateWindowResult != UIntPtr.Zero) {
        // Win7(Aero)/Vista(Aero)
        UIntPtr sysListView32 = ExternalAPI.FindWindowEx(enumerateWindowResult, UIntPtr.Zero, "SysListView32", null);
        if (sysListView32 != UIntPtr.Zero) {
          return sysListView32;
        }
      }
      return ExternalAPI.GetDesktopWindow();
    }
  }

  /// 仮想ディスプレイのデータをRECT化したプロパティ
  /// @todo(me) 現在Desktop/DesktopListViewで使い回ししているが、問題が発生する可能性あり
  public static ExternalAPI.RECT VirtualScreenRect {
    get {
      return new ExternalAPI.RECT {
        Left = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN),
        Top = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN),
        Right = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN) +
                ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXVIRTUALSCREEN),
        Bottom = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN) +
                 ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXVIRTUALSCREEN)
      };
    }
  }
}
}
