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

/// @file SCFF.Common/GUI/SnapGuide.cs
/// @copydoc SCFF::Common::GUI::SnapGuide

namespace SCFF.Common.GUI {

using System.Collections.Generic;
using SCFF.Common.Profile;

/// レイアウト用スナップガイド
public class SnapGuide {  
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public SnapGuide(Profile profile) {
    var currentIndex = profile.CurrentIndex;
    foreach (var layoutElement in profile) {
      // currentにSnapさせる必要はない
      if (layoutElement.Index == currentIndex) continue;

      // Top/Leftは先に、Right/Bottomは後に
      verticalSnapGuides.AddFirst(layoutElement.BoundRelativeTop);
      verticalSnapGuides.AddLast(layoutElement.BoundRelativeBottom);
      horizontalSnapGuides.AddFirst(layoutElement.BoundRelativeLeft);
      horizontalSnapGuides.AddLast(layoutElement.BoundRelativeRight);
    }
  }

  //===================================================================
  // スナップ補正
  //===================================================================

  /// 縦方向のスナップ補正
  /// @param[in,out] original 補正対象
  /// @return 補正されたかどうか
  public bool TryVerticalSnap(ref double original) {
    if (this.verticalSnapGuides.Count == 0) return false;

    // キャッシュ付き線形探索
    var guide = this.verticalSnapGuides.First;
    do {
      var lowerBound = guide.Value - Constants.BorderRelativeThickness;
      var upperBound = guide.Value + Constants.BorderRelativeThickness;
      /// @attention 浮動小数点数の比較
      if (lowerBound < original && original < upperBound) {
        original = guide.Value;
        if (guide != this.verticalSnapGuides.First) {
          // キャッシング: ガイドを削除して先頭につめなおす
          // こうすればキャッシュ効果でかなり早くなるはず
          this.verticalSnapGuides.Remove(guide);
          this.verticalSnapGuides.AddFirst(original);
        }
        return true;
      }
      guide = guide.Next;
    } while (guide != null);

    return false;
  }

  /// 水平方向のスナップ補正
  /// @param[in,out] original 補正対象
  /// @return 補正されたかどうか
  public bool TryHorizontalSnap(ref double original) {
    if (this.horizontalSnapGuides.Count == 0) return false;

    // キャッシュ付き線形探索
    var guide = this.horizontalSnapGuides.First;
    do {
      var lowerBound = guide.Value - Constants.BorderRelativeThickness;
      var upperBound = guide.Value + Constants.BorderRelativeThickness;
      /// @attention 浮動小数点数の比較
      if (lowerBound < original && original < upperBound) {
        original = guide.Value;
        if (guide != this.horizontalSnapGuides.First) {
          // キャッシング: ガイドを削除して先頭につめなおす
          // こうすればキャッシュ効果でかなり早くなるはず
          this.horizontalSnapGuides.Remove(guide);
          this.horizontalSnapGuides.AddFirst(original);
        }
        return true;
      }
      guide = guide.Next;
    } while (guide != null);

    return false;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// 横方向のスナップガイドリスト
  private readonly LinkedList<double> verticalSnapGuides = new LinkedList<double>();
  /// 縦方向のスナップガイドリスト
  private readonly LinkedList<double> horizontalSnapGuides = new LinkedList<double>();
}
}   // namespace SCFF.Common.GUI
