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

/// @file SCFF.Common/Profile/BoundRelativeInputCorrector.cs
/// @copydoc SCFF::Common::Profile::BoundRelativeInputCorrector

namespace SCFF.Common.Profile {

using System.Diagnostics;

/// BoundRelative*の入力エラーを判定するためのメソッドを集めたstaticクラス
/// @attention Decoratorパターンにすべきではない(newが多くなりすぎるので)
public static class BoundRelativeInputCorrector {
  //=================================================================
  // 列挙型
  //=================================================================

  /// 試行結果（訂正箇所）
  /// - Target: 変更箇所
  /// - Dependent: Targetを変更した場合に訂正される可能性がある箇所
  public enum TryResult {
    /// 訂正なしでTargetへの値の設定が可能
    NothingChanged    = 0x0,
    /// Targetの訂正が必要
    TargetChanged     = 0x1,
    /// Dependentの訂正が必要
    DependentChanged  = 0x2,
    /// Target/Dependent両方の訂正が必要
    BothChanged       = TargetChanged | DependentChanged
  }

  /// LayoutElement.BoundRelative*の要素を表す列挙型
  public enum Names {
    Left,
    Top,
    Right,
    Bottom
  }

  /// LayoutElement.BoundRelative*の関連するプロパティを返す
  public static Names GetDependent(Names target) {
    switch (target) {
      case Names.Left: return Names.Right;
      case Names.Top: return Names.Bottom;
      case Names.Right: return Names.Left;
      case Names.Bottom: return Names.Top;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //=================================================================
  // 訂正
  //=================================================================

  /// BoundRelative*の値が小さい方の位置要素(Left/Top)の変更を試みる
  /// @param original 変更前のBoundRelativeLTRB
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたBoundRelativeLTRB
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangeLow(RelativeLTRB original,
      Names target, double value, double sizeLowerBound,
      out RelativeLTRB changed) {
    Debug.Assert(target == Names.Left || target == Names.Top);

    // 準備
    var onX = (target == Names.Left);
    var low = value;
    var high = onX ? original.Right : original.Bottom;
    var result = TryResult.NothingChanged;

    // highのチェック
    Debug.Assert(0.0 + sizeLowerBound <= high && high <= 1.0);

    // 上限・下限の補正
    if (low < 0.0) {
      low = 0.0;
      result |= TryResult.TargetChanged;
    } else if (1.0 < low + sizeLowerBound) {
      low = 1.0 - sizeLowerBound;
      result |= TryResult.TargetChanged;
    }

    // lowが大きすぎる場合、highを増やして最小幅を確保
    if (high < low + sizeLowerBound) {
      high = low + sizeLowerBound;
      result |= TryResult.DependentChanged;
    }

    // 出力
    Debug.Assert(0.0 <= low &&
                 sizeLowerBound <= high - low &&
                 high <= 1.0);
    changed = onX
        ? new RelativeLTRB(low, original.Top, high, original.Bottom)
        : new RelativeLTRB(original.Left, low, original.Right, high);
    return result;
  }

  /// BoundRelative*の値が大きい方の位置要素(Right/Bottom)の変更を試みる
  /// @param original 変更前のBoundRelativeLTRB
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたBoundRelativeLTRB
  /// @return 試行結果（訂正箇所）
  public static TryResult TryChangeHigh(RelativeLTRB original,
      Names target, double value, double sizeLowerBound,
      out RelativeLTRB changed) {
    Debug.Assert(target == Names.Right || target == Names.Bottom);
 
    // 準備
    var onX = (target == Names.Right);
    var low = onX ? original.Left : original.Top;
    var high = value;
    var result = TryResult.NothingChanged;

    // lowのチェック
    Debug.Assert(0.0 <= low && low <= 1.0 - sizeLowerBound);

    // 上限・下限の補正
    if (high < 0.0 + sizeLowerBound) {
      high = 0.0 + sizeLowerBound;
      result |= TryResult.TargetChanged;
    } else if (1.0 < high) {
      high = 1.0;
      result |= TryResult.TargetChanged;
    }

    // lowが大きすぎる場合、lowを減らして最小幅を確保
    if (high < low + sizeLowerBound) {
      low = high - sizeLowerBound;
      result |= TryResult.DependentChanged;
    }

    // 出力
    Debug.Assert(0.0 <= low &&
                 sizeLowerBound <= high - low &&
                 high <= 1.0);
    changed = onX
        ? new RelativeLTRB(low, original.Top, high, original.Bottom)
        : new RelativeLTRB(original.Left, low, original.Right, high);
    return result;
  }

  //-------------------------------------------------------------------

  /// レイアウト要素からClipping領域(Fitオプションなし)を取得
  private static RelativeLTRB GetOriginal(ILayoutElementView layoutElement) {
    return new RelativeLTRB(layoutElement.BoundRelativeLeft,
                            layoutElement.BoundRelativeTop,
                            layoutElement.BoundRelativeRight,
                            layoutElement.BoundRelativeBottom);
  }

  /// レイアウト要素のBoundRelative領域の変更を試みる
  /// @param layoutElement レイアウト要素
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたBoundRelative領域
  /// @return 試行結果（訂正箇所）
  public static TryResult TryChange(
      ILayoutElementView layoutElement, Names target, double value,
      out RelativeLTRB changed) {
    // 準備
    var original = BoundRelativeInputCorrector.GetOriginal(layoutElement);

    // 訂正
    switch (target) {
      case Names.Left:
      case Names.Top: {
        return BoundRelativeInputCorrector.TryChangeLow(original, target, value,
            Constants.MinimumBoundRelativeSize, out changed);
      }
      case Names.Right:
      case Names.Bottom: {
        return BoundRelativeInputCorrector.TryChangeHigh(original, target, value,
            Constants.MinimumBoundRelativeSize, out changed);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
}
}   // namespace SCFF.Common.Profile
