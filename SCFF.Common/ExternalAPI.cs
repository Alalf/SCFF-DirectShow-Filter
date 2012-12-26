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
/// SCFF.*モジュールで利用する外部APIをまとめたクラスの定義

namespace SCFF.Common {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// SCFF.*モジュールで利用する外部APIをまとめたクラス
/// HWNDは特例としてUIntPtr、それ以外はIntPtrで取り扱うこと
public class ExternalAPI {

  //-------------------------------------------------------------------
  // user32.dll
  //-------------------------------------------------------------------

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

  //[DllImport("user32.dll")]
  //public static extern bool InvalidateRect(UIntPtr hWnd, ref RECT lpRect, bool bErase);

  [DllImport("user32.dll")]
  public static extern int GetSystemMetrics(int nIndex);

  //[DllImport("user32.dll")]
  //public static extern bool IntersectRect(out RECT lprcDst, ref RECT lprcSrc1, ref RECT lprcSrc2);

  //-------------------------------------------------------------------
  // gdi32.dll
  //-------------------------------------------------------------------

  // Constants

  public const int SRCCOPY    = 0x00CC0020;
  public const int CAPTUREBLT = 0x40000000;

  // public const int PS_SOLID    = 0x00000000;
  public const int PS_NULL       = 0x00000005;

  public const int R2_XORPEN   = 7;

  // API
  [DllImport("gdi32.dll")]
  public static extern int BitBlt(IntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      IntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);

  [DllImport("gdi32.dll")]
  public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

  [DllImport("gdi32.dll")]
  public static extern int SetROP2(IntPtr hdc, int fnDrawMode);

  [DllImport("gdi32.dll")]
  public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

  [DllImport("gdi32.dll")]
  public static extern bool DeleteObject(IntPtr hObject);

  [DllImport("gdi32.dll")]
  public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

  //-------------------------------------------------------------------
  // dwmapi.dll
  //-------------------------------------------------------------------

  // Constants
  public const int DWM_EC_DISABLECOMPOSITION = 0;
  public const int DWM_EC_ENABLECOMPOSITION = 1;

  // API
  [DllImport("dwmapi.dll")]
  public static extern int DwmIsCompositionEnabled(out bool enabled);
  [DllImport("dwmapi.dll")]
  public static extern int DwmEnableComposition(uint uCompositionAction);
}
}
