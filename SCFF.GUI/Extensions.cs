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

/// @file SCFF.GUI/Extensions.cs
/// @copydoc SCFF::GUI::Extensions

namespace SCFF.GUI {

using System.Windows;
using SCFF.Common;

/// SCFF.Commonで定義されたクラス・構造体をWPFで使える形に変換するためのExtensions
public static class Extensions {
  /// IntRectの拡張: Rectへ変換
  public static Rect ToRect(this IntRect rect) {
    return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
  }
  /// DoubleRectの拡張: Rectへ変換
  public static Rect ToRect(this DoubleRect rect) {
    return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
  }
}
}   // namespace SCFF.GUI
