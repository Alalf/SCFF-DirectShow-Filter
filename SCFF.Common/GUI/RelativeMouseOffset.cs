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

/// @file SCFF.Common/GUI/RelativeMouseOffset.cs
/// マウスポインタ座標がレイアウト要素の上下左右とどれだけ離れているか

namespace SCFF.Common.GUI {

/// マウスポインタ座標がレイアウト要素の上下左右とどれだけ離れているか
public class RelativeMouseOffset {
  public RelativeMouseOffset(Profile.InputLayoutElement layoutElement, Point relativeMousePoint) {
    this.Left = relativeMousePoint.X - layoutElement.BoundRelativeLeft;
    this.Top = relativeMousePoint.Y - layoutElement.BoundRelativeTop;
    this.Right = relativeMousePoint.X - layoutElement.BoundRelativeRight;
    this.Bottom = relativeMousePoint.Y - layoutElement.BoundRelativeBottom;
  }

  public double Left { get; private set; }
  public double Top { get; private set; }
  public double Right { get; private set; }
  public double Bottom { get; private set; }
}
}   // namespace SCFF.Common.GUI
