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

/// @file SCFF.Common/ExternalAPI.cs
/// SCFF.*モジュールで利用するUser32.dllのAPIをまとめたクラス

namespace SCFF.Common.Ext {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// SCFF.*モジュールで利用する外部APIをまとめたクラス
/// SCFF.*モジュールで利用するUser32.dllのAPIをまとめたクラス
public class User32 {
  // Constants: System Metric
  public const int SM_CXSCREEN = 0;
  public const int SM_CYSCREEN = 1;
  public const int SM_XVIRTUALSCREEN  = 76;
  public const int SM_YVIRTUALSCREEN  = 77;
  public const int SM_CXVIRTUALSCREEN = 78;
  public const int SM_CYVIRTUALSCREEN = 79;

  // Type
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT { public int Left, Top, Right, Bottom; }

  [StructLayout(LayoutKind.Sequential)]
  public struct POINT { public int X, Y; }

  // Delegate
  public delegate bool WNDENUMProc(UIntPtr hWnd, IntPtr lParam);

  // API
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindow(UIntPtr hWnd);

  [DllImport("user32.dll", SetLastError = false)]
  public static extern UIntPtr GetDesktopWindow();

  [DllImport("user32.dll")]
  public static extern UIntPtr FindWindowEx(UIntPtr hWnd, UIntPtr hwndChildAfter, string lpszClass, string lpszWindow);

  [DllImport("user32.dll")]
  public static extern bool EnumWindows(WNDENUMProc lpEnumFunc, IntPtr lParam);

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  public static extern int GetClassName(UIntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

  [DllImport("user32.dll")]
  public static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);

  [DllImport("user32.dll")]
  public static extern bool ClientToScreen(UIntPtr hWnd, ref POINT lpPoint);

  [DllImport("user32.dll")]
  public static extern UIntPtr WindowFromPoint(int xPoint, int yPoint);

  [DllImport("user32.dll")]
  public static extern IntPtr GetDC(UIntPtr hWnd);

  [DllImport("user32.dll")]
  public static extern int ReleaseDC(UIntPtr hWnd, IntPtr hDC);

  [DllImport("user32.dll")]
  public static extern int GetSystemMetrics(int nIndex);
}
}
