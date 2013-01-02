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

/// @file SCFF.Common/Types.cs
/// SCFF.Commonモジュールで利用する型

namespace SCFF.Common {

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
}   // namespace SCFF.Common
