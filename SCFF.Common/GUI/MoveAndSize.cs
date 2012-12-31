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

/// @file SCFF.Common/GUI/MoveAndResize.cs
/// 与えられたマウスポイントを利用してレイアウト要素を移動・拡大縮小する

namespace SCFF.Common.GUI {

using System;
using System.Collections.Generic;
using System.Diagnostics;

/// 与えられたマウスポイントを利用してレイアウト要素を移動・拡大縮小する
public static class MoveAndSize {
  //-------------------------------------------------------------------
  // 定数
  //-------------------------------------------------------------------
  private const double MinimumWidth = Constants.WEBorderThickness * 2;
  private const double MinimumHeight = Constants.NSBorderThickness * 2;

  //-------------------------------------------------------------------
  // staticメソッド
  //-------------------------------------------------------------------

  /// Move
  public static void Move(Profile.InputLayoutElement layoutElement,
      Point point, RelativeMouseOffset offset, SnapGuide snapGuide,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Snapするのは軸あたり1回のみ
    bool hasAlreadyHorizontalSnapped = false;
    bool hasAlreadyVerticalSnapped = false;

    // Left
    var tryNextLeft = point.X - offset.Left;
    hasAlreadyHorizontalSnapped |= snapGuide.TryHorizontalSnap(ref tryNextLeft);
    if (tryNextLeft < 0.0) tryNextLeft = 0.0;

    // Top
    var tryNextTop = point.Y - offset.Top;
    hasAlreadyVerticalSnapped |= snapGuide.TryVerticalSnap(ref tryNextTop);
    if (tryNextTop < 0.0) tryNextTop = 0.0;

    // Right
    var tryNextRight = tryNextLeft + layoutElement.BoundRelativeWidth;
    if (!hasAlreadyHorizontalSnapped && snapGuide.TryHorizontalSnap(ref tryNextRight)) {
      // スナップ補正がかかった場合
      tryNextLeft = tryNextRight - layoutElement.BoundRelativeWidth;
    }
    if (tryNextRight > 1.0) {
      tryNextRight = 1.0;
      tryNextLeft = tryNextRight - layoutElement.BoundRelativeWidth;
    }

    // Bottom
    var tryNextBottom = tryNextTop + layoutElement.BoundRelativeHeight;
    if (!hasAlreadyVerticalSnapped && snapGuide.TryVerticalSnap(ref tryNextBottom)) {
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

  /// Size
  public static void Size(Profile.InputLayoutElement layoutElement,
      HitModes hitMode,
      Point point, RelativeMouseOffset offset, SnapGuide snapGuide,
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
        var tryNextTop = point.Y - offset.Top;
        snapGuide.TryVerticalSnap(ref tryNextTop);
        if (tryNextTop < 0.0) tryNextTop = 0.0;
        var topUpperBound = nextBottom - MinimumHeight;
        if (tryNextTop > topUpperBound) tryNextTop = topUpperBound;
        nextTop = tryNextTop;
        break;
      }
      case HitModes.SizeS:
      case HitModes.SizeSW:
      case HitModes.SizeSE: {
        // Bottom
        var tryNextBottom = point.Y - offset.Bottom;
        snapGuide.TryVerticalSnap(ref tryNextBottom);
        if (tryNextBottom > 1.0) tryNextBottom = 1.0;
        var bottomLowerBound = nextTop + MinimumHeight;
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
        var tryNextLeft = point.X - offset.Left;
        snapGuide.TryHorizontalSnap(ref tryNextLeft);
        if (tryNextLeft < 0.0) tryNextLeft = 0.0;
        var leftUpperBound = nextRight - MinimumWidth;
        if (tryNextLeft > leftUpperBound) tryNextLeft = leftUpperBound;
        nextLeft = tryNextLeft;
        break;
      }
      case HitModes.SizeNE:
      case HitModes.SizeE:
      case HitModes.SizeSE: {
        // Right
        var tryNextRight = point.X - offset.Right;
        snapGuide.TryHorizontalSnap(ref tryNextRight);
        if (tryNextRight > 1.0) tryNextRight = 1.0;
        var rightLowerBound = nextLeft + MinimumWidth;
        if (tryNextRight < rightLowerBound) tryNextRight = rightLowerBound;
        nextRight = tryNextRight;
        break;
      }
    }
  }
}
}   // namespace SCFF.Common.GUI
