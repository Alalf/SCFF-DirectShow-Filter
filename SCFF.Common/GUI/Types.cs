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

/// @file SCFF.Common/GUI/Types.cs
/// SCFF.Common.GUIモジュールで利用する型

/// GUIに関連したクラス(ただしGUIアセンブリ非依存)をまとめた名前空間
namespace SCFF.Common.GUI {

/// ヒットテストの結果をまとめた列挙型
///
/// N,W,S,Eは場合によっては消える場合もある
public enum HitModes {
  Neutral,
  Move,
  SizeNW,
  SizeNE,
  SizeSW,
  SizeSE,
  SizeN,
  SizeW,
  SizeS,
  SizeE
}

/// System.Windowsが使えないので代替用のPoint
public class RelativePoint {
  public RelativePoint(double X, double Y) {
    this.X = X;
    this.Y = Y;
  }
  public double X { get; set; }
  public double Y { get; set; }
}

/// System.Windowsが使えないので代替用のRect
public class RelativeRect {
  public double X { get; set; }
  public double Y { get; set; }
  public double Width { get; set; }
  public double Height { get; set; }

  /// 含有判定
  public bool Contains(RelativePoint point) {
    return this.X <= point.X && point.X <= this.X + this.Width &&
           this.Y <= point.Y && point.Y <= this.Y + this.Height;
  }
}
}   // namespace SCFF.Common
