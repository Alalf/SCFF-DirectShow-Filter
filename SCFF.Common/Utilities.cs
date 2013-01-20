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
using System.Diagnostics;
using System.Text;
using SCFF.Common.Ext;

/// SCFF.Commonモジュール共通で利用する機能
public static class Utilities {
  //===================================================================
  // Enum関連
  //===================================================================

  /// Rectの関連するプロパティを返す
  public static RectProperties GetDependent(RectProperties target) {
    switch (target) {
      case RectProperties.X: return RectProperties.Width;
      case RectProperties.Y: return RectProperties.Height;
      case RectProperties.Width: return RectProperties.X;
      case RectProperties.Height: return RectProperties.Y;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // WindowType/Window別の機能
  //===================================================================

  /// Windowハンドルが正常かどうか
  public static bool IsWindowValid(WindowTypes windowType, UIntPtr window) {
    switch(windowType) {
      case WindowTypes.Normal: {
        return (window != UIntPtr.Zero && User32.IsWindow(window));
      }
      case WindowTypes.DesktopListView:
      case WindowTypes.Desktop: {
        return true;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// WindowのClient座標系での領域を返す
  /// @warning windowType == Desktop時のみ、左上端がマイナス座標になる可能性がある
  /// @param windowType Windowタイプ
  /// @param window Windowハンドル
  /// @return WindowのClient座標系での領域(Desktopのみ左上端が(0,0)とは限らない)
  public static ClientRect GetWindowRect(WindowTypes windowType, UIntPtr window) {
    switch (windowType) {
      case WindowTypes.Normal: {
        Debug.Assert(Utilities.IsWindowValid(windowType, window),
                     "Invalid Window", "GetWindowRect");
        User32.RECT windowRect;
        User32.GetClientRect(window, out windowRect);
        return new ClientRect(0, 0, windowRect.Right, windowRect.Bottom);
      }
      case WindowTypes.DesktopListView: {
        return new ClientRect(0, 0,
            User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
      }
      case WindowTypes.Desktop: {
        return new ClientRect(
            User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
            User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Client座標系の座標をWindowタイプ別にScreen座標系に変換する
  public static ScreenPoint ClientToScreen(WindowTypes windowType, UIntPtr window, int clientX, int clientY) {
    switch (windowType) {
      case WindowTypes.Normal: {
        Debug.Assert(Utilities.IsWindowValid(windowType, window),
                     "Invalid Window", "ClientToScreen");
        User32.POINT windowPoint = new User32.POINT { X = clientX, Y = clientY };
        User32.ClientToScreen(window, ref windowPoint);
        return new ScreenPoint(windowPoint.X, windowPoint.Y);
      }
      case WindowTypes.DesktopListView: {
        //　仮想スクリーン座標なので補正を戻す
        return new ScreenPoint(clientX + User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
                               clientY + User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN));
      }
      case WindowTypes.Desktop: {
        // スクリーン座標系なのでそのまま返す
        return new ScreenPoint(clientX, clientY);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Windowタイプ別にScreen座標系での領域を返す
  public static ScreenRect GetWindowScreenRect(WindowTypes windowType, UIntPtr window) {
    var screenRect = Utilities.GetWindowRect(windowType, window);
    var screenPoint =
        Utilities.ClientToScreen(windowType, window, screenRect.X, screenRect.Y);
    return new ScreenRect(screenPoint.X, screenPoint.Y,
                          screenRect.Width, screenRect.Height);
  }

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
