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
  private static void Move(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Snapするのは軸あたり1回のみ
    bool hasAlreadyHorizontalSnapped = false;
    bool hasAlreadyVerticalSnapped = false;

    // Left
    var tryNextLeft = mousePoint.X - mouseOffset.Left;
    if (snapGuide != null) {
      hasAlreadyHorizontalSnapped |= snapGuide.TryHorizontalSnap(ref tryNextLeft);
    }
    if (tryNextLeft < 0.0) tryNextLeft = 0.0;

    // Top
    var tryNextTop = mousePoint.Y - mouseOffset.Top;
    if (snapGuide != null) {
      hasAlreadyVerticalSnapped |= snapGuide.TryVerticalSnap(ref tryNextTop);
    }
    if (tryNextTop < 0.0) tryNextTop = 0.0;

    // Right
    var tryNextRight = tryNextLeft + layoutElement.BoundRelativeWidth;
    if (snapGuide != null && !hasAlreadyHorizontalSnapped &&
        snapGuide.TryHorizontalSnap(ref tryNextRight)) {
      // スナップ補正がかかった場合
      tryNextLeft = tryNextRight - layoutElement.BoundRelativeWidth;
    }
    if (tryNextRight > 1.0) {
      tryNextRight = 1.0;
      tryNextLeft = tryNextRight - layoutElement.BoundRelativeWidth;
    }

    // Bottom
    var tryNextBottom = tryNextTop + layoutElement.BoundRelativeHeight;
    if (snapGuide != null && !hasAlreadyVerticalSnapped &&
        snapGuide.TryVerticalSnap(ref tryNextBottom)) {
      // スナップ補正がかかった場合
      tryNextTop = tryNextBottom - layoutElement.BoundRelativeHeight;
    }
    if (tryNextBottom > 1.0) {
      tryNextBottom = 1.0;
      tryNextTop = tryNextBottom - layoutElement.BoundRelativeHeight;
    }

    nextLeft = tryNextLeft;
    nextTop = tryNextTop;
    nextRight = tryNextRight;
    nextBottom = tryNextBottom;
  }

  //===================================================================
  // 拡大縮小
  //===================================================================

  /// レイアウト要素を拡大縮小する
  private static void Size(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // 初期値
    nextLeft = layoutElement.BoundRelativeLeft;
    nextTop = layoutElement.BoundRelativeTop;
    nextRight = layoutElement.BoundRelativeRight;
    nextBottom = layoutElement.BoundRelativeBottom;

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
  }

  //===================================================================
  // 移動 or 拡大縮小
  //===================================================================

  /// hitModeにあわせてレイアウト要素を移動 or 拡大縮小する
  public static void MoveOrSize(ILayoutElementView layoutElement,
      HitModes hitMode,
      RelativePoint mousePoint, RelativeMouseOffset mouseOffset,
      SnapGuide snapGuide,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    if (hitMode == HitModes.Move) {
      MoveAndSize.Move(layoutElement, hitMode, mousePoint, mouseOffset,
          snapGuide, out nextLeft, out nextTop, out nextRight, out nextBottom);
    } else {
      MoveAndSize.Size(layoutElement, hitMode, mousePoint, mouseOffset,
          snapGuide, out nextLeft, out nextTop, out nextRight, out nextBottom);
    }
  }
}
}   // namespace SCFF.Common.GUI
