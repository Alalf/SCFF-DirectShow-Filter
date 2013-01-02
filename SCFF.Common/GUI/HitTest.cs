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

/// @file SCFF.Common/GUI/HitTest.cs
/// 与えられたマウスポイント(0-1,0-1)からレイアウトIndexとModeを返す

namespace SCFF.Common.GUI {

using System;
using System.Collections.Generic;
using System.Diagnostics;

/// 与えられたマウスポイント(0-1,0-1)からレイアウトIndexとModeを返す
///
/// このクラスの中では座標系はすべて正規化された相対値(0.0-1.0)なので
/// わざわざ変数名などにRelativeをつける必要はない
///
/// Borderは内側だけでなく外側にもあることに注意
///
/// ややこしいことになるのでこのクラスの中からApp.Profileの変更は禁止
public static class HitTest {

  /// 最大外接矩形を返す
  private static Rect GetMaximumBoundRect(Profile.InputLayoutElement layoutElement) {
    return new Rect {
      X = layoutElement.BoundRelativeLeft - Constants.WEBorderThickness,
      Y = layoutElement.BoundRelativeTop - Constants.NSBorderThickness,
      Width = layoutElement.BoundRelativeWidth + Constants.WEBorderThickness * 2,
      Height = layoutElement.BoundRelativeHeight + Constants.NSBorderThickness * 2
    };
  }

  /// 移動用領域を返す
  private static Rect GetMoveRect(Profile.InputLayoutElement layoutElement) {
    return new Rect {
      X = layoutElement.BoundRelativeLeft + Constants.WEBorderThickness,
      Y = layoutElement.BoundRelativeTop + Constants.NSBorderThickness,
      Width = Math.Max(layoutElement.BoundRelativeWidth - Constants.WEBorderThickness * 2, 0.0),
      Height = Math.Max(layoutElement.BoundRelativeHeight - Constants.NSBorderThickness * 2, 0.0)
    };
  }

  /// 指定した座標からHitModesを返す
  /// @pre 必ずHitModes.Resize*を返すことが可能なlayoutElement
  private static HitModes GetHitMode(Profile.InputLayoutElement layoutElement, Point point) {
    // ---------------
    // |  |1     |2  |
    // ---------------

    // H1
    var borderWRight = layoutElement.BoundRelativeLeft + Constants.WEBorderThickness;
    // H2
    var borderELeft = layoutElement.BoundRelativeRight - Constants.WEBorderThickness;

    // V1
    var borderNBottom = layoutElement.BoundRelativeTop + Constants.NSBorderThickness;
    // v2
    var borderSTop = layoutElement.BoundRelativeBottom - Constants.NSBorderThickness;
    
    // x座標→Y座標
    if (point.X <= borderWRight) {
      // W
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeNW;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        return HitModes.SizeW;
      } else {
        // S
        return HitModes.SizeSW;
      }
    } else if (point.X <= borderELeft) {
      // (W)-(E)
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeN;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        Debug.Fail("GetHitMode: Move??");
        return HitModes.Move;
      } else {
        // S
        return HitModes.SizeS;
      }
    } else {
      // E
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeNE;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        return HitModes.SizeE;
      } else {
        // S
        return HitModes.SizeSE;
      }
    }
  }

  /// ヒットテスト
  public static bool TryHitTest(Profile profile, Point point, out int hitIndex, out HitModes hitMode) {
    // 計算途中の結果をまとめるスタック
    var moveStack = new Stack<Profile.InputLayoutElement>();
    var sizeStack = new Stack<Profile.InputLayoutElement>();

    // レイアウト要素を線形探索
    foreach (var layoutElement in profile) {
      // 最大外接矩形に入っていなければヒットテスト対象外
      var maximumBoundRect = HitTest.GetMaximumBoundRect(layoutElement);
      if (!maximumBoundRect.Contains(point)) continue;

      var moveRect = HitTest.GetMoveRect(layoutElement);
      if (moveRect.Contains(point)) {
        // 移動用領域に入ってればStackに積む
        moveStack.Push(layoutElement);
      } else {
        // 最大外接矩形に入っていて、移動用領域でないということはサイズ変更用領域に入ってる
        sizeStack.Push(layoutElement);
      }
    }
   
    // sizeStack優先
    foreach (var layoutElement in sizeStack) {
      // 見つかり次第終了
      hitMode = HitTest.GetHitMode(layoutElement, point);
      hitIndex = layoutElement.Index;
      return true;
    }

    // moveStack
    foreach (var layoutElement in moveStack) {
      // 見つかり次第終了
      hitMode = HitModes.Move;
      hitIndex = layoutElement.Index;
      return true;
    }

    hitIndex = -1;
    hitMode = HitModes.Neutral;
    return false;
  }
}
}   // namespace SCFF.Common.GUI
