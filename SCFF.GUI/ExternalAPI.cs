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

/// @file SCFF.GUI/ExternalAPI.cs
/// SCFF.GUIモジュールで利用する外部APIをまとめたクラスの定義

namespace SCFF.GUI {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// scff_appモジュールで利用する外部APIをまとめたクラス
internal class ExternalAPI {

  //-------------------------------------------------------------------
  // user32.dll
  //-------------------------------------------------------------------

  // Type
  [StructLayout(LayoutKind.Sequential)]
  internal struct RECT { public int Left, Top, Right, Bottom; }

  [DllImport("user32.dll")]
  internal static extern UIntPtr WindowFromPoint(int xPoint, int yPoint);

  [DllImport("user32.dll")]
  internal static extern UIntPtr GetDC(UIntPtr hWnd);

  [DllImport("user32.dll")]
  internal static extern int ReleaseDC(UIntPtr hWnd, UIntPtr hDC);

  [DllImport("user32.dll")]
  internal static extern bool InvalidateRect(UIntPtr hWnd, ref RECT lpRect, bool bErase);

  [DllImport("user32.dll")]
  internal static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);

  //-------------------------------------------------------------------
  // gdi32.dll
  //-------------------------------------------------------------------

  // Constants

  internal const int SRCCOPY    = 0x00CC0020;
  internal const int CAPTUREBLT = 0x40000000;

  //internal const int PS_SOLID    = 0x00000000;
  internal const int PS_NULL       = 0x00000005;

  internal const int R2_XORPEN   = 7;

  // API
  [DllImport("gdi32.dll")]
  internal static extern int BitBlt(UIntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      UIntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);

  [DllImport("gdi32.dll")]
  internal static extern UIntPtr CreatePen(int fnPenStyle, int nWidth, uint crColor);

  [DllImport("gdi32.dll")]
  internal static extern int SetROP2(UIntPtr hdc, int fnDrawMode);

  [DllImport("gdi32.dll")]
  internal static extern UIntPtr SelectObject(UIntPtr hdc, UIntPtr hgdiobj);

  [DllImport("gdi32.dll")]
  internal static extern bool DeleteObject(UIntPtr hObject);

  [DllImport("gdi32.dll")]
  internal static extern bool Rectangle(UIntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
}
}
