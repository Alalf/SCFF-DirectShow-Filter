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

/// @file SCFF.Common/ExternalAPI.cs
/// SCFF.Commonモジュールで利用する外部APIをまとめたクラスの定義

namespace SCFF.Common {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// scff_appモジュールで利用する外部APIをまとめたクラス
internal class ExternalAPI {

  //-------------------------------------------------------------------
  // user32.dll
  //-------------------------------------------------------------------

  // Constants: System Metric
  internal const int SM_CXSCREEN = 0;
  internal const int SM_CYSCREEN = 1;
  internal const int SM_XVIRTUALSCREEN  = 76;
  internal const int SM_YVIRTUALSCREEN  = 77;
  internal const int SM_CXVIRTUALSCREEN = 78;
  internal const int SM_CYVIRTUALSCREEN = 79;

  // Type
  [StructLayout(LayoutKind.Sequential)]
  internal struct RECT { public int Left, Top, Right, Bottom; }

  [StructLayout(LayoutKind.Sequential)]
  internal struct POINT { public int X, Y; }

  // Delegate
  internal delegate bool WNDENUMProc(UIntPtr hWnd, UIntPtr lParam);

  // API
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool IsWindow(UIntPtr hWnd);

  [DllImport("user32.dll", SetLastError = false)]
  internal static extern UIntPtr GetDesktopWindow();

  [DllImport("user32.dll")]
  internal static extern UIntPtr FindWindowEx(UIntPtr hWnd, UIntPtr hwndChildAfter, string lpszClass, string lpszWindow);

  [DllImport("user32.dll")]
  internal static extern bool EnumWindows(WNDENUMProc lpEnumFunc, UIntPtr lParam);

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  internal static extern int GetClassName(UIntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

  [DllImport("user32.dll")]
  internal static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);

  [DllImport("user32.dll")]
  internal static extern bool ClientToScreen(UIntPtr hWnd, ref POINT lpPoint);

  [DllImport("user32.dll")]
  internal static extern UIntPtr WindowFromPoint(int xPoint, int yPoint);

  [DllImport("user32.dll")]
  internal static extern IntPtr GetDC(UIntPtr hWnd);

  [DllImport("user32.dll")]
  internal static extern int ReleaseDC(UIntPtr hWnd, IntPtr hDC);

  [DllImport("user32.dll")]
  internal static extern bool InvalidateRect(UIntPtr hWnd, ref RECT lpRect, bool bErase);

  [DllImport("user32.dll")]
  internal static extern int GetSystemMetrics(int nIndex);


  //-------------------------------------------------------------------
  // gdi32.dll
  //-------------------------------------------------------------------

  // Constants

  internal const int SRCCOPY    = 0x00CC0020;
  internal const int CAPTUREBLT = 0x40000000;

  // API
  [DllImport("gdi32.dll")]
  internal static extern int BitBlt(IntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      IntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);

  //-------------------------------------------------------------------
  // dwmapi.dll
  //-------------------------------------------------------------------

  // Constants
  internal const int DWM_EC_DISABLECOMPOSITION = 0;
  internal const int DWM_EC_ENABLECOMPOSITION = 1;

  // API
  [DllImport("dwmapi.dll")]
  internal static extern int DwmIsCompositionEnabled(out bool enabled);
  [DllImport("dwmapi.dll")]
  internal static extern int DwmEnableComposition(uint uCompositionAction);
}
}
