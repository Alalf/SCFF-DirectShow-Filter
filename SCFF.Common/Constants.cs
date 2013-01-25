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

/// @file SCFF.Common/Constants.cs
/// SCFF.Commonモジュールで利用する定数

namespace SCFF.Common {

using System.Collections.Generic;
using SCFF.Interprocess;

/// SCFF.Commonモジュールで利用する定数
public static class Constants {
  //===================================================================
  // 定数
  //===================================================================

  /// 最大レイアウト要素数
  public const int MaxLayoutElementCount =
      Interprocess.MaxComplexLayoutElements;

  //-------------------------------------------------------------------
  // MainWindow
  //-------------------------------------------------------------------

  /// ウィンドウの初期位置(x)
  public const double DefaultLeft = 32.0;
  /// ウィンドウの初期位置(y)            
  public const double DefaultTop = 32.0;

  //-------------------------------------------------------------------
  // MainWindow.Width/Height
  //-------------------------------------------------------------------

  // 1. Normal        : !LayoutIsExpanded && !CompactView
  // 2. NormalLayout  : LayoutIsExpanded && !CompactView
  // 3. Compact       : !LayoutIsExpanded && CompactView
  // 4. CompactLayout : LayoutIsExpanded && CompactView

  /// Normal/Compact: ウィンドウの最小幅
  public const double NoLayoutMinWidth = 280.0;
  /// NormalLayout/CompactLayout: ウィンドウの最小幅
  public const double LayoutMinWidth = 650.0;

  /// Normal/Compact: ウィンドウの最大幅
  public const double NoLayoutMaxWidth = Constants.NoLayoutMinWidth;
  /// NormalLayout/CompactLayout: ウィンドウの最大幅
  public const double LayoutMaxWidth = double.PositiveInfinity;

  /// ウィンドウの最小高さ
  public const double MinHeight = 280.0;

  /// Normal: ウィンドウの最大高さ
  public const double NormalMaxHeight = 545.0;
  /// Compact: ウィンドウの最大高さ
  public const double CompactMaxHeight = 280.0;
  /// NormalLayout/CompactLayout: ウィンドウの最大高さ
  public const double LayoutMaxHeight = double.PositiveInfinity;

  /// Normal: ウィンドウの初期幅
  public const double NormalDefaultWidth = Constants.NoLayoutMinWidth;
  /// Normal: ウィンドウの初期高さ
  public const double NormalDefaultHeight = 545.0;

  /// NormalLayout: ウィンドウの初期幅
  public const double NormalLayoutDefaultWidth = 730.0;
  /// NormalLayout: ウィンドウの初期高さ
  public const double NormalLayoutDefaultHeight = 545.0;

  /// Compact: ウィンドウの初期幅
  public const double CompactDefaultWidth = Constants.NoLayoutMinWidth;
  /// Compact: ウィンドウの初期高さ
  public const double CompactDefaultHeight = Constants.MinHeight;

  /// CompactLayout: ウィンドウの初期幅
  public const double CompactLayoutDefaultWidth = 730.0;
  /// CompactLayout: ウィンドウの初期高さ
  public const double CompactLayoutDefaultHeight = 545.0;
  
  //-------------------------------------------------------------------
  // Preview
  //-------------------------------------------------------------------

  /// プロセスエントリが見つからないときのサンプルの幅
  public const int DummySampleWidth = 640;
  /// プロセスエントリが見つからないときのサンプルの高さ
  public const int DummySampleHeight = 400;

  //-------------------------------------------------------------------
  // Options
  //-------------------------------------------------------------------

  /// 「最近使ったプロファイル」に表示される数
  public const int RecentProfilesLength = 5;

  //-------------------------------------------------------------------
  // LayoutElement
  //-------------------------------------------------------------------

  /// レイアウト要素の最小幅＆高さ
  public const double MinimumBoundRelativeSize = 0.04;

  /// 単位ボーダーサイズ(内側に1, 外側に1)
  /// @attention ボーダーは外にもあることに注意
  public const double BorderRelativeThickness =
      Constants.MinimumBoundRelativeSize / 2;

  //===================================================================
  // 読み込み専用辞書
  //===================================================================

  //-------------------------------------------------------------------
  // ResizeMethod
  //-------------------------------------------------------------------

  /// Index->ResizeMethodの名前
  public static readonly string[] ResizeMethodLabels = new string[] {
    "FastBilinear (fast bilinear)",
    "Bilinear (bilinear)",
    "Bicubic (bicubic)",
    "X (experimental)",
    "Point (nearest neighbor)",
    "Area (averaging area)",
    "Bicublin (luma bicubic, chroma bilinear)",
    "Gauss (gaussian)",
    "Sinc (sinc)",
    "Lanczos (lanczos)",
    "Spline (natural bicubic spline)"
  };

  /// Index->SWScaleFlags
  public static readonly SWScaleFlags[] ResizeMethodArray =
      new SWScaleFlags[] {
    SWScaleFlags.FastBilinear,
    SWScaleFlags.Bilinear,
    SWScaleFlags.Bicubic,
    SWScaleFlags.X,
    SWScaleFlags.Point,
    SWScaleFlags.Area,
    SWScaleFlags.Bicublin,
    SWScaleFlags.Gauss,
    SWScaleFlags.Sinc,
    SWScaleFlags.Lanczos,
    SWScaleFlags.Spline,
  };

  /// SWScaleFlags->Index
  public static readonly Dictionary<SWScaleFlags,int> ResizeMethodIndexes =
      new Dictionary<SWScaleFlags,int> {
    {SWScaleFlags.FastBilinear, 0},
    {SWScaleFlags.Bilinear, 1},
    {SWScaleFlags.Bicubic, 2},
    {SWScaleFlags.X, 3},
    {SWScaleFlags.Point, 4},
    {SWScaleFlags.Area, 5},
    {SWScaleFlags.Bicublin, 6},
    {SWScaleFlags.Gauss, 7},
    {SWScaleFlags.Sinc, 8},
    {SWScaleFlags.Lanczos, 9},
    {SWScaleFlags.Spline, 10}
  };
}
}   // namespace SCFF.Common
