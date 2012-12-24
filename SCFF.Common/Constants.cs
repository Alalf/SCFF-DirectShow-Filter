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

/// @file SCFF.Common/Constants.cs
/// SCFF.Commonモジュールで利用する定数・テーブル

namespace SCFF.Common {

using System.Collections.Generic;

//=====================================================================
// Enum
//=====================================================================

/// Options用WindowState。System.Windows.WindowStateと相互に変換する。
public enum WindowState {
  Normal,
  Minimized,
  Maximized
}

/// Profile用Windowの種類
public enum WindowTypes {
  Normal,
  DesktopListView,
  Desktop,
}

/// @copydoc SCFF.Interprocess.LayoutTypes
public enum LayoutTypes {
  NullLayout    = Interprocess.LayoutTypes.NullLayout,
  NativeLayout  = Interprocess.LayoutTypes.NativeLayout,
  ComplexLayout = Interprocess.LayoutTypes.ComplexLayout
}

/// @copydoc SCFF.Interprocess.SWScaleFlags
public enum SWScaleFlags {
  FastBilinear  = Interprocess.SWScaleFlags.FastBilinear,
  Bilinear      = Interprocess.SWScaleFlags.Bilinear,
  Bicubic       = Interprocess.SWScaleFlags.Bicubic,
  X             = Interprocess.SWScaleFlags.X,
  Point         = Interprocess.SWScaleFlags.Point,
  Area          = Interprocess.SWScaleFlags.Area,
  Bicublin      = Interprocess.SWScaleFlags.Bicublin,
  Gauss         = Interprocess.SWScaleFlags.Gauss,
  Sinc          = Interprocess.SWScaleFlags.Sinc,
  Lanczos       = Interprocess.SWScaleFlags.Lanczos,
  Spline        = Interprocess.SWScaleFlags.Spline
}

/// @copydoc SCFF.Interprocess.RotateDirections
public enum RotateDirections {
  NoRotate      = Interprocess.RotateDirections.NoRotate,
  Degrees90     = Interprocess.RotateDirections.Degrees90,
  Degrees180    = Interprocess.RotateDirections.Degrees180,
  Degrees270    = Interprocess.RotateDirections.Degrees270,
}

//=====================================================================
// staticクラス
//=====================================================================

/// SCFF.Commonモジュールが外部に公開するテーブルと定数
public static class Constants {
  public const int MaxLayoutElementCount = Interprocess.Interprocess.MaxComplexLayoutElements;

  public const double MainWindowLeft = 32.0;
  public const double MainWindowTop = 32.0;
  public const double MainWindowWidth = 730.0;
  public const double MainWindowHeight = 545.0;
  public const double CompactMainWindowWidth = 280.0;
  public const double CompactMainWindowHeight = 280.0;

  public const int DefaultPreviewWidth = 640;
  public const int DefaultPreviewHeight = 400;
  
  /// @todo(me) ダミーなのであとで消す
  public const int DummyPreviewWidth = DefaultPreviewWidth;
  public const int DummyPreviewHeight = DefaultPreviewHeight;

  public const int RecentProfilesLength = 5;

  // ItemsSourceを使いたくないのでディクショナリを二つ用意しておく
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

  public static readonly SWScaleFlags[] ResizeMethodArray = new SWScaleFlags[] {
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

  public static readonly Dictionary<SWScaleFlags, int> ResizeMethodIndexes =
      new Dictionary<SWScaleFlags, int>() {
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
}
