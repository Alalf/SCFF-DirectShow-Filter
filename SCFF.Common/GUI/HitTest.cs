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
/// @copydoc SCFF::Common::GUI::HitTest

namespace SCFF.Common.GUI {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SCFF.Common.Profile;

/// 与えられたマウス座標([0-1], [0-1])からレイアウト要素のIndexとHitModesを取得
public static class HitTest {
  //===================================================================
  // ヒットテスト用RelativeRectの生成
  //===================================================================

  /// レイアウト要素のうちボーダーを含めたRectを取得
  private static RelativeRect GetMaximumBoundRect(
      ILayoutElementView layoutElement) {
    return new RelativeRect(
        layoutElement.BoundRelativeLeft -
            Constants.BorderRelativeThickness,
        layoutElement.BoundRelativeTop -
            Constants.BorderRelativeThickness,
        layoutElement.BoundRelativeWidth +
            Constants.BorderRelativeThickness * 2,
        layoutElement.BoundRelativeHeight +
            Constants.BorderRelativeThickness * 2);
  }

  /// レイアウト要素のうちボーダーを含まないRectを取得
  private static RelativeRect GetMoveRect(
      ILayoutElementView layoutElement) {
    return new RelativeRect(
        layoutElement.BoundRelativeLeft +
            Constants.BorderRelativeThickness,
        layoutElement.BoundRelativeTop +
            Constants.BorderRelativeThickness,
        Math.Max(layoutElement.BoundRelativeWidth -
            Constants.BorderRelativeThickness * 2, 0.0),
        Math.Max(layoutElement.BoundRelativeHeight -
            Constants.BorderRelativeThickness * 2, 0.0));
  }

  //===================================================================
  // ヒットモード計算
  //===================================================================

  /// レイアウト要素とマウス相対座標からHitModesを調べる
  /// @param layoutElement レイアウト要素
  /// @param mousePoint layoutElement内のマウス相対座標
  /// @return HitModes.SizeXXXのいずれか
  private static HitModes GetHitMode(ILayoutElementView layoutElement,
      RelativePoint mousePoint) {
    // ---------------
    // |  |1     |2  |
    // ---------------

    // H1
    var borderWRight = layoutElement.BoundRelativeLeft +
                       Constants.BorderRelativeThickness;
    // H2
    var borderELeft = layoutElement.BoundRelativeRight -
                      Constants.BorderRelativeThickness;

    // V1
    var borderNBottom = layoutElement.BoundRelativeTop +
                        Constants.BorderRelativeThickness;
    // v2
    var borderSTop = layoutElement.BoundRelativeBottom -
                     Constants.BorderRelativeThickness;
    
    // x座標→Y座標
    if (mousePoint.X <= borderWRight) {         // W
      if (mousePoint.Y <= borderNBottom) {      // N
        return HitModes.SizeNW;
      } else if (mousePoint.Y <= borderSTop) {  // (N)-(S)
        return HitModes.SizeW;
      } else {                                  // S
        return HitModes.SizeSW;
      }
    } else if (mousePoint.X <= borderELeft) {   // (W)-(E)
      if (mousePoint.Y <= borderNBottom) {      // N
        return HitModes.SizeN;
      } else if (mousePoint.Y <= borderSTop) {  // (N)-(S)
        Debug.Fail("Move?", "HitTest.GetHitMode");
        return HitModes.Move;
      } else {                                  // S
        return HitModes.SizeS;
      }
    } else {                                    // E
      if (mousePoint.Y <= borderNBottom) {      // N
        return HitModes.SizeNE;
      } else if (mousePoint.Y <= borderSTop) {  // (N)-(S)
        return HitModes.SizeE;
      } else {                                  // S
        return HitModes.SizeSE;
      }
    }
  }

  //===================================================================
  // ヒットテスト
  //===================================================================

  /// ヒットテスト
  public static bool TryHitTest(Profile profile, RelativePoint mousePoint,
      out int hitIndex, out HitModes hitMode) {
    // 計算途中の結果をまとめるスタック
    var moveStack = new Stack<ILayoutElementView>();
    var sizeStack = new Stack<ILayoutElementView>();

    // レイアウト要素を線形探索
    foreach (var layoutElement in profile) {
      // ヒットテスト対象外判定
      var maximumBoundRect = HitTest.GetMaximumBoundRect(layoutElement);
      if (!maximumBoundRect.Contains(mousePoint)) continue;

      var moveRect = HitTest.GetMoveRect(layoutElement);
      if (moveRect.Contains(mousePoint)) {
        // 移動用領域
        moveStack.Push(layoutElement);
      } else {
        // 移動用領域でない＝サイズ変更用領域
        sizeStack.Push(layoutElement);
      }
    }
   
    // sizeStack優先
    foreach (var layoutElement in sizeStack) {
      // 見つかり次第終了
      hitMode = HitTest.GetHitMode(layoutElement, mousePoint);
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
