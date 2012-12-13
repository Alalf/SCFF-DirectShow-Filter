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

/// @file scff_app/external_api.cs
/// scff_appモジュールで利用する外部APIをまとめたクラスの定義

namespace scff_app {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// scff_appモジュールで利用する外部APIをまとめたクラス
class ExternalAPI {
  /// 生成禁止用コンストラクタ
  private ExternalAPI() {}

  //-------------------------------------------------------------------
  // user32.dll
  //-------------------------------------------------------------------

  // Constants

  // Window Message
  internal const int WM_NCHITTEST      = 0x0084;
  internal const int WM_NCLBUTTONDOWN  = 0x00A1;
  internal const int WM_LBUTTONDOWN    = 0x0201;
  internal const int WM_SIZING         = 0x0214;
  internal const int WM_MOVING         = 0x0216;

  // Hit Test
  internal const int HTTRANSPARENT      = -1;
  internal const int HTNOWHERE          = 0;
  internal const int HTCLIENT           = 1;
  internal const int HTCAPTION          = 2;

  internal const int HTLEFT             = 10;
  internal const int HTRIGHT            = 11;
  internal const int HTTOP              = 12;
  internal const int HTTOPLEFT          = 13;
  internal const int HTTOPRIGHT         = 14;
  internal const int HTBOTTOM           = 15;
  internal const int HTBOTTOMLEFT       = 16;
  internal const int HTBOTTOMRIGHT      = 17;

  // Type

  [StructLayout(LayoutKind.Sequential)]
  internal struct RECT { public int left, top, right, bottom; }

  [StructLayout(LayoutKind.Sequential)]
  internal struct POINT { public int x, y; }

  // API

  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  internal static extern bool IsWindow(UIntPtr hWnd);

  [DllImport("user32.dll", SetLastError = false)]
  internal static extern UIntPtr GetDesktopWindow();

  [DllImport("user32.dll")]
  internal static extern UIntPtr WindowFromPoint(int xPoint, int yPoint);

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  internal static extern int GetClassName(UIntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

  [DllImport("user32.dll")]
  internal static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);

  [DllImport("user32.dll")]
  internal static extern bool ClientToScreen(UIntPtr hWnd, ref POINT lpPoint);

  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

  [DllImport("user32.dll")]
  internal static extern bool ReleaseCapture();

  [DllImport("user32.dll")]
  internal static extern IntPtr GetDC(UIntPtr hWnd);

  [DllImport("user32.dll")]
  internal static extern int ReleaseDC(UIntPtr hWnd, IntPtr hDC);

  [DllImport("user32.dll")]
  internal static extern bool InvalidateRect(UIntPtr hWnd, ref RECT lpRect, bool bErase);

  //-------------------------------------------------------------------
  // gdi32.dll
  //-------------------------------------------------------------------

  // Constants

  internal const int SRCCOPY = 13369376;

  internal const int CAPTUREBLT = 1073741824;

  // API

  [DllImport("gdi32.dll")]
  internal static extern int BitBlt(IntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      IntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);
}
}
