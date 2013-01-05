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

/// @file SCFF.Common/Ext/User32.cs
/// SCFFで利用するUser32.dllのAPIをまとめたクラス

/// 外部APIをまとめた名前空間
namespace SCFF.Common.Ext {

using System;
using System.Runtime.InteropServices;
using System.Text;

/// SCFFで利用するUser32.dllのAPIをまとめたクラス
public class User32 {
  //===================================================================
  // 定数
  //===================================================================

  /// System Metric: プライマリディスプレイの幅
  public const int SM_CXSCREEN = 0;
  /// System Metric: プライマリディスプレイの高さ
  public const int SM_CYSCREEN = 1;
  /// System Metric: 仮想画面の左端のスクリーン座標(x)
  public const int SM_XVIRTUALSCREEN  = 76;
  /// System Metric: 仮想画面の上端のスクリーン座標(y)
  public const int SM_YVIRTUALSCREEN  = 77;
  /// System Metric: 仮想画面の幅
  public const int SM_CXVIRTUALSCREEN = 78;
  /// System Metric: 仮想画面の高さ
  public const int SM_CYVIRTUALSCREEN = 79;

  //===================================================================
  // 構造体
  //===================================================================

  /// RECT
  [StructLayout(LayoutKind.Sequential)]
  public struct RECT { public int Left, Top, Right, Bottom; }

  /// POINT
  [StructLayout(LayoutKind.Sequential)]
  public struct POINT { public int X, Y; }

  //===================================================================
  // コールバック関数
  //===================================================================

  /// EnumWindowsに渡すコールバック関数
  public delegate bool WNDENUMProc(UIntPtr hWnd, IntPtr lParam);

  //===================================================================
  // API
  //===================================================================

  /// Windowハンドルかどうかの判定
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  public static extern bool IsWindow(UIntPtr hWnd);

  /// 親を持たないルートWindow(≒デスクトップ)ハンドルの取得
  [DllImport("user32.dll", SetLastError = false)]
  public static extern UIntPtr GetDesktopWindow();

  /// 指定されたクラス名、ウィンドウ名のWindowハンドルを探す
  [DllImport("user32.dll")]
  public static extern UIntPtr FindWindowEx(UIntPtr hWnd,
      UIntPtr hwndChildAfter, string lpszClass, string lpszWindow);

  ///画面上のトップレベルWindowハンドルを列挙し、Windowハンドルをコールバック関数に渡す
  [DllImport("user32.dll")]
  public static extern bool EnumWindows(WNDENUMProc lpEnumFunc, IntPtr lParam);

  /// Windowハンドルからクラス名を取得する
  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  public static extern int GetClassName(UIntPtr hWnd,
      StringBuilder lpClassName, int nMaxCount);

  /// Windowハンドルのクライアント領域を表すRECTを取得
  [DllImport("user32.dll")]
  public static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);

  /// 指定したWindowハンドルのクライアント座標をスクリーン座標に変換する
  [DllImport("user32.dll")]
  public static extern bool ClientToScreen(UIntPtr hWnd, ref POINT lpPoint);

  /// 指定したスクリーン座標からWindowハンドルを得る
  [DllImport("user32.dll")]
  public static extern UIntPtr WindowFromPoint(int xPoint, int yPoint);

  /// WindowハンドルからDCを取得
  [DllImport("user32.dll")]
  public static extern IntPtr GetDC(UIntPtr hWnd);

  /// Windowハンドルから取得したDCを解放
  [DllImport("user32.dll")]
  public static extern int ReleaseDC(UIntPtr hWnd, IntPtr hDC);

  /// システム情報を取得
  [DllImport("user32.dll")]
  public static extern int GetSystemMetrics(int nIndex);
}
}   // namespace SCFF.Common.Ext
