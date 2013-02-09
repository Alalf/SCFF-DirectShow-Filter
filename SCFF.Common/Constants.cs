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

  /// アプリケーション名＋バージョン表記
  public const string SCFFVersion = "SCFF DirectShow Filter Ver.0.2.0";

  /// SCFF DirectShow Filter GUID
  public const string SCFFSourceGUID = "D64DB8AA-9055-418F-AFE9-A080A4FAE47A";
  /// レジストリ上の登録場所
  public const string SCFFSourceRegistryKey = "CLSID\\{" + Constants.SCFFSourceGUID + "}";

  //-------------------------------------------------------------------
  // OptionsFile
  //-------------------------------------------------------------------

  /// INIファイル名
  public const string OptionsFileName = "SCFF.Common.Options.ini";
  /// INIファイルの先頭に付加するヘッダー
  public const string OptionsHeader = "; " + Constants.SCFFVersion;

  //-------------------------------------------------------------------
  // ProfileFile
  //-------------------------------------------------------------------

  /// ProfileFileの拡張子
  public const string ProfileExtension = ".scffprofile";
  /// ProfileFileの先頭に付加するヘッダー
  public const string ProfileHeader = "; " + Constants.SCFFVersion;

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
  public const double NormalMaxHeight = 550.0;
  /// Compact: ウィンドウの最大高さ
  public const double CompactMaxHeight = 280.0;
  /// NormalLayout/CompactLayout: ウィンドウの最大高さ
  public const double LayoutMaxHeight = double.PositiveInfinity;

  /// Normal: ウィンドウの初期幅
  public const double NormalDefaultWidth = Constants.NoLayoutMinWidth;
  /// Normal: ウィンドウの初期高さ
  public const double NormalDefaultHeight = 550.0;

  /// NormalLayout: ウィンドウの初期幅
  public const double NormalLayoutDefaultWidth = 730.0;
  /// NormalLayout: ウィンドウの初期高さ
  public const double NormalLayoutDefaultHeight = 550.0;

  /// Compact: ウィンドウの初期幅
  public const double CompactDefaultWidth = Constants.NoLayoutMinWidth;
  /// Compact: ウィンドウの初期高さ
  public const double CompactDefaultHeight = Constants.MinHeight;

  /// CompactLayout: ウィンドウの初期幅
  public const double CompactLayoutDefaultWidth = 730.0;
  /// CompactLayout: ウィンドウの初期高さ
  public const double CompactLayoutDefaultHeight = 550.0;
  
  //-------------------------------------------------------------------
  // Preview
  //-------------------------------------------------------------------

  /// プロセスエントリが見つからないときのサンプルの幅
  public const int DummySampleWidth = 640;
  /// プロセスエントリが見つからないときのサンプルの高さ
  public const int DummySampleHeight = 360;

  //-------------------------------------------------------------------
  // Options
  //-------------------------------------------------------------------

  /// 「最近使ったプロファイル」に表示される数
  public const int RecentProfilesLength = 5;

  //-------------------------------------------------------------------
  // LayoutElement
  //-------------------------------------------------------------------

  /// クリッピング領域の最小幅＆高さ
  public const int MinimumClippingSize = 1;

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
  // ImagePixelFormats
  //-------------------------------------------------------------------

  /// ImagePixelFormats->ImagePixelFormatsの名前
  public static readonly Dictionary<ImagePixelFormats,string> ImagePixelFormatLabels
      = new Dictionary<ImagePixelFormats,string> {
    {ImagePixelFormats.InvalidPixelFormat, "InvalidPixelFormat"},
    {ImagePixelFormats.I420, "I420"},
    {ImagePixelFormats.IYUV, "IYUV"},
    {ImagePixelFormats.YV12, "YV12"},
    {ImagePixelFormats.UYVY, "UYVY"},
    {ImagePixelFormats.YUY2, "YUY2"},
    {ImagePixelFormats.RGB0, "RGB0"},
    {ImagePixelFormats.SupportedPixelFormatsCount, "SupportedPixelFormatsCount"}
  };

  //-------------------------------------------------------------------
  // ResizeMethod
  //-------------------------------------------------------------------

  /// SWScaleFlags->SWScaleFlagsの名前
  public static readonly Dictionary<SWScaleFlags,string> SWScaleFlagLabels
      = new Dictionary<SWScaleFlags,string> {
    {SWScaleFlags.FastBilinear, "FastBilinear (fast bilinear)"},
    {SWScaleFlags.Bilinear,     "Bilinear (bilinear)"},
    {SWScaleFlags.Bicubic,      "Bicubic (bicubic)"},
    {SWScaleFlags.X,            "X (experimental)"},
    {SWScaleFlags.Point,        "Point (nearest neighbor)"},
    {SWScaleFlags.Area,         "Area (averaging area)"},
    {SWScaleFlags.Bicublin,     "Bicublin (luma bicubic, chroma bilinear)"},
    {SWScaleFlags.Gauss,        "Gauss (gaussian)"},
    {SWScaleFlags.Sinc,         "Sinc (sinc)"},
    {SWScaleFlags.Lanczos,      "Lanczos (lanczos)"},
    {SWScaleFlags.Spline,       "Spline (natural bicubic spline)"}
  };
}
}   // namespace SCFF.Common
