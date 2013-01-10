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

/// @file SCFF.Common/GUI/MoveAndSize.cs
/// @copydoc SCFF::Common::GUI::MoveAndSize

namespace SCFF.Common.GUI {

/// 現在のマウス座標とオフセットを指定してレイアウト要素を移動・拡大縮小
public static class MoveAndSize {
  //===================================================================
  // 移動
  //===================================================================

  /// レイアウト要素を移動する
  private static RelativeLTRB Move(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide) {
    // Snapするのは軸あたり1回のみ
    bool hasAlreadyHorizontalSnapped = false;
    bool hasAlreadyVerticalSnapped = false;

    // Left
    var nextLeft = mousePoint.X - mouseOffset.Left;
    if (snapGuide != null) {
      hasAlreadyHorizontalSnapped |= snapGuide.TryHorizontalSnap(ref nextLeft);
    }
    if (nextLeft < 0.0) nextLeft = 0.0;

    // Top
    var nextTop = mousePoint.Y - mouseOffset.Top;
    if (snapGuide != null) {
      hasAlreadyVerticalSnapped |= snapGuide.TryVerticalSnap(ref nextTop);
    }
    if (nextTop < 0.0) nextTop = 0.0;

    // Right
    var nextRight = nextLeft + layoutElement.BoundRelativeWidth;
    if (snapGuide != null && !hasAlreadyHorizontalSnapped &&
        snapGuide.TryHorizontalSnap(ref nextRight)) {
      // スナップ補正がかかった場合
      nextLeft = nextRight - layoutElement.BoundRelativeWidth;
    }
    if (nextRight > 1.0) {
      nextRight = 1.0;
      nextLeft = nextRight - layoutElement.BoundRelativeWidth;
    }

    // Bottom
    var nextBottom = nextTop + layoutElement.BoundRelativeHeight;
    if (snapGuide != null && !hasAlreadyVerticalSnapped &&
        snapGuide.TryVerticalSnap(ref nextBottom)) {
      // スナップ補正がかかった場合
      nextTop = nextBottom - layoutElement.BoundRelativeHeight;
    }
    if (nextBottom > 1.0) {
      nextBottom = 1.0;
      nextTop = nextBottom - layoutElement.BoundRelativeHeight;
    }

    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  //===================================================================
  // 拡大縮小
  //===================================================================

  /// レイアウト要素を拡大縮小する
  private static RelativeLTRB Size(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide) {
    // 初期値
    var nextLeft = layoutElement.BoundRelativeLeft;
    var nextTop = layoutElement.BoundRelativeTop;
    var nextRight = layoutElement.BoundRelativeRight;
    var nextBottom = layoutElement.BoundRelativeBottom;

    // Top/Bottom
    switch (hitMode) {
      case HitModes.SizeN:
      case HitModes.SizeNW:
      case HitModes.SizeNE: {
        // Top
        var tryNextTop = mousePoint.Y - mouseOffset.Top;
        if (snapGuide != null) snapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - Constants.MinimumBoundRelativeSize;
        if (tryNextTop > topUpperBound) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeS:
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = mousePoint.Y - mouseOffset.Bottom;
        if (snapGuide != null) snapGuide.TryVerticalSnap(ref tryNextBottom);
        if (tryNextBottom > 1.0) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + Constants.MinimumBoundRelativeSize;
        if (tryNextBottom < bottomLowerBound) tryNextBottom = bottomLowerBound;
        nextBottom = tryNextBottom;
        break;
      }
    }

    // Left/Right
    switch (hitMode) {
      case HitModes.SizeNW:
      case HitModes.SizeW:
      case HitModes.SizeSW: {
        // Left
        var tryNextLeft = mousePoint.X - mouseOffset.Left;
        if (snapGuide != null) snapGuide.TryHorizontalSnap(ref tryNextLeft);
        if (tryNextLeft < 0.0) tryNextLeft = 0.0;
        var leftUpperBound = nextRight - Constants.MinimumBoundRelativeSize;
        if (tryNextLeft > leftUpperBound) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = mousePoint.X - mouseOffset.Right;
        if (snapGuide != null) snapGuide.TryHorizontalSnap(ref tryNextRight);
        if (tryNextRight > 1.0) tryNextRight = 1.0;
        var rightLowerBound = nextLeft + Constants.MinimumBoundRelativeSize;
        if (tryNextRight < rightLowerBound) tryNextRight = rightLowerBound;
        nextRight = tryNextRight;
        break;
      }
    }
    return new RelativeLTRB(nextLeft, nextTop, nextRight, nextBottom);
  }

  //===================================================================
  // 移動 or 拡大縮小
  //===================================================================

  /// hitModeにあわせてレイアウト要素を移動 or 拡大縮小する
  public static RelativeLTRB MoveOrSize(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide) {
    if (hitMode == HitModes.Move) {
      return MoveAndSize.Move(layoutElement, hitMode,
          mousePoint, mouseOffset, snapGuide);
    } else {
      return MoveAndSize.Size(layoutElement, hitMode,
          mousePoint, mouseOffset, snapGuide);
    }
  }
}
}   // namespace SCFF.Common.GUI
