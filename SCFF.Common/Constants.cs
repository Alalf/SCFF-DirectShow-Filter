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

/// @file SCFF.Common/Constants.cs
/// SCFF.Commonモジュールで利用する定数・テーブル

namespace SCFF.Common {

using System.Collections.Generic;

/// SCFF.Commonモジュールで利用する定数・テーブル
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
  public static string[] ResizeMethodLabels = new string[] {
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

  public static Profile.SWScaleFlags[] ResizeMethodArray = new Profile.SWScaleFlags[] {
    Profile.SWScaleFlags.FastBilinear,
    Profile.SWScaleFlags.Bilinear,
    Profile.SWScaleFlags.Bicubic,
    Profile.SWScaleFlags.X,
    Profile.SWScaleFlags.Point,
    Profile.SWScaleFlags.Area,
    Profile.SWScaleFlags.Bicublin,
    Profile.SWScaleFlags.Gauss,
    Profile.SWScaleFlags.Sinc,
    Profile.SWScaleFlags.Lanczos,
    Profile.SWScaleFlags.Spline,
  };

  public static Dictionary<Profile.SWScaleFlags, int> ResizeMethodIndexes =
      new Dictionary<Profile.SWScaleFlags, int>() {
    {Profile.SWScaleFlags.FastBilinear, 0},
    {Profile.SWScaleFlags.Bilinear, 1},
    {Profile.SWScaleFlags.Bicubic, 2},
    {Profile.SWScaleFlags.X, 3},
    {Profile.SWScaleFlags.Point, 4},
    {Profile.SWScaleFlags.Area, 5},
    {Profile.SWScaleFlags.Bicublin, 6},
    {Profile.SWScaleFlags.Gauss, 7},
    {Profile.SWScaleFlags.Sinc, 8},
    {Profile.SWScaleFlags.Lanczos, 9},
    {Profile.SWScaleFlags.Spline, 10}
  };
}
}
