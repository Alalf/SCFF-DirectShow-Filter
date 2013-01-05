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

/// SCFF.Commonモジュールで利用する定数
public static class Constants {
  //===================================================================
  // 定数
  //===================================================================

  /// 最大レイアウト要素数
  public const int MaxLayoutElementCount =
      Interprocess.Interprocess.MaxComplexLayoutElements;

  //-------------------------------------------------------------------
  // MainWindow
  //-------------------------------------------------------------------

  /// メインウィンドウの初期位置(x)
  public const double MainWindowLeft = 32.0;
  /// メインウィンドウの初期位置(y)            
  public const double MainWindowTop = 32.0;
  /// メインウィンドウの初期幅
  public const double MainWindowWidth = 730.0;
  /// メインウィンドウの初期高さ
  public const double MainWindowHeight = 545.0;
  /// コンパクト表示時の幅
  public const double CompactMainWindowWidth = 280.0;
  /// コンパクト表示時の高さ
  public const double CompactMainWindowHeight = 280.0;

  //-------------------------------------------------------------------
  // Preview
  //-------------------------------------------------------------------

  /// プロセスエントリが見つからないときのプレビューの幅
  public const int DefaultPreviewWidth = 640;
  /// プロセスエントリが見つからないときのプレビューの高さ
  public const int DefaultPreviewHeight = 400;
  
  /// ダミープレビュー幅
  /// @todo(me) ダミーなのであとで消す
  public const int DummyPreviewWidth = DefaultPreviewWidth;
  /// ダミープレビュー高さ
  /// @todo(me) ダミーなのであとで消す
  public const int DummyPreviewHeight = DefaultPreviewHeight;

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
  public const double BorderRelativeThickness = MinimumBoundRelativeSize / 2;

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
  public static readonly Dictionary<SWScaleFlags, int> ResizeMethodIndexes =
      new Dictionary<SWScaleFlags, int> {
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
