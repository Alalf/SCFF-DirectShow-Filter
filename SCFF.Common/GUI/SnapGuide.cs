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
/// レイアウト用スナップガイド

namespace SCFF.Common.GUI {

using System.Collections.Generic;

/// レイアウト用スナップガイド
public class SnapGuide {
  //-------------------------------------------------------------------
  // 定数
  //-------------------------------------------------------------------
  private const double WESnapBorderThickness = 0.02;
  private const double NSSnapBorderThickness = 0.02;
  
  //-------------------------------------------------------------------
  // メソッド
  //-------------------------------------------------------------------

  /// コンストラクタ
  public SnapGuide(Profile profile, bool isEnabled) {
    this.IsEnabled = isEnabled;
    if (!this.IsEnabled) return;

    var currentIndex = profile.CurrentInputLayoutElement.Index;
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

  /// 縦方向のスナップ補正
  public bool TryVerticalSnap(ref double original) {
    if (!this.IsEnabled) return false;
    if (this.verticalSnapGuides.Count == 0) return false;

    // キャッシュ付き線形探索
    var guide = this.verticalSnapGuides.First;
    do {
      var lowerBound = guide.Value - NSSnapBorderThickness;
      var upperBound = guide.Value + NSSnapBorderThickness;
      if (lowerBound <= original && original <= upperBound) {
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
  public bool TryHorizontalSnap(ref double original) {
    if (!this.IsEnabled) return false;
    if (this.horizontalSnapGuides.Count == 0) return false;

    // キャッシュ付き線形探索
    var guide = this.horizontalSnapGuides.First;
    do {
      var lowerBound = guide.Value - WESnapBorderThickness;
      var upperBound = guide.Value + WESnapBorderThickness;
      if (lowerBound <= original && original <= upperBound) {
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

  //-------------------------------------------------------------------
  // メンバ変数
  //-------------------------------------------------------------------
  private bool IsEnabled { get; set; }
  private LinkedList<double> verticalSnapGuides = new LinkedList<double>();
  private LinkedList<double> horizontalSnapGuides = new LinkedList<double>();
}
}   // namespace SCFF.Common.GUI
