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

/// @file SCFF.Common/Profile/ClippingInputCorrector.cs
/// @copydoc SCFF::Common::Profile::ClippingInputCorrector

namespace SCFF.Common.Profile {

using System.Diagnostics;

/// Clipping*の入力エラーを判定するためのメソッドを集めたstaticクラス
/// @attention Decoratorパターンにすべきではない(newが多くなりすぎるので)
public static class ClippingInputCorrector {
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

  /// LayoutElement.Clipping*の要素を表す列挙型
  public enum Names {
    X,
    Y,
    Width,
    Height
  }

  /// LayoutElement.Clipping*の関連するプロパティを返す
  public static Names GetDependent(Names target) {
    switch (target) {
      case Names.X: return Names.Width;
      case Names.Y: return Names.Height;
      case Names.Width: return Names.X;
      case Names.Height: return Names.Y;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //=================================================================
  // 訂正
  //=================================================================

  /// Clipping*の位置要素(X/Y)の変更を試みる
  /// @warning boundは常に変わり続けている。
  ///          よって、Position入力中でもSizeを常に最新の値に更新しておく必要がある。
  /// @param original 変更前のClientRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param bound 境界を表すClientRect
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたClientRect
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangePosition(ClientRect original,
      Names target, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    Debug.Assert(target == Names.X || target == Names.Y);

    // 準備
    var onX = (target == Names.X);
    var position = value;
    var size = onX ? original.Width : original.Height;
    var positionLowerBound = onX ? bound.X : bound.Y;
    var positionUpperBound = onX ? bound.Right : bound.Bottom;
    var sizeUpperBound = onX ? bound.Width : bound.Height;
    var result = TryResult.NothingChanged;

    // 上限・下限の補正
    if (position < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      position = positionLowerBound;
      result |= TryResult.TargetChanged;
    } else if (positionUpperBound < position + sizeLowerBound) {
      // Positionが大きすぎる場合、Positionを減らしてSizeを下限に
      position = positionUpperBound - sizeLowerBound;
      size = sizeLowerBound;
      result |= TryResult.BothChanged;
    }

    // Sizeの補正
    if (size < sizeLowerBound) {
      // Sizeは下限以上
      size = sizeLowerBound;
      result |= TryResult.DependentChanged;
    }

    // 領域が境界内に収まらない場合、Positionは保持＆Sizeを縮める
    if (positionUpperBound < position + size) {
      size = positionUpperBound - position;
      result |= TryResult.DependentChanged;
    }

    Debug.Assert(positionLowerBound <= position &&
                 sizeLowerBound <= size &&
                 position + size <= positionUpperBound);
    changed = onX
        ? new ClientRect(position, original.Y, size, original.Height)
        : new ClientRect(original.X, position, original.Width, size);
    return result;
  }

  /// Clipping*のサイズ要素(Width/Height)の変更を試みる
  /// @warning boundは常に変わり続けている。
  ///          よって、Size入力中でもPositionを常に最新の値に更新しておく必要がある。
  /// @param original 変更前のClientRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param bound 境界を表すClientRect
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたClientRect
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangeSize(ClientRect original,
      Names target, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    Debug.Assert(target == Names.Width || target == Names.Height);

    // 準備
    var onX = (target == Names.Width);
    var position = onX ? original.X : original.Y;
    var size = value;
    var positionLowerBound = onX ? bound.X : bound.Y;
    var positionUpperBound = onX ? bound.Right : bound.Bottom;
    var sizeUpperBound = onX ? bound.Width : bound.Height;
    var result = TryResult.NothingChanged;

    // 上限・下限の補正
    if (size < sizeLowerBound) {
      // Sizeは下限以上
      size = sizeLowerBound;
      result |= TryResult.TargetChanged;
    } else if (sizeUpperBound < size) {
      // Sizeが大きすぎる場合はFitさせる
      position = positionLowerBound;
      size = sizeUpperBound;
      result |= TryResult.BothChanged;
    }

    // Positionの補正
    if (position < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      position = positionLowerBound;
      result |= TryResult.DependentChanged;
    } else if (positionUpperBound < position + sizeLowerBound) {
      // Positionが大きすぎる場合、Positionを減らしてWidthを下限に
      position = positionUpperBound - sizeLowerBound;
      size = sizeLowerBound;
      result |= TryResult.BothChanged;
    }

    // 領域が境界内に収まらない場合、Sizeは保持＆Positionを小さく
    if (positionUpperBound < position + size) {
      position = positionUpperBound - size;
      result |= TryResult.DependentChanged;
    }

    Debug.Assert(positionLowerBound <= position &&
                 sizeLowerBound <= size &&
                 position + size <= positionUpperBound);
    changed = onX
        ? new ClientRect(position, original.Y, size, original.Height)
        : new ClientRect(original.X, position, original.Width, size);
    return result;
  }

  //-------------------------------------------------------------------

  /// レイアウト要素からClipping領域(Fitオプションなし)を取得
  private static ClientRect GetOriginal(LayoutElement layoutElement) {
    return new ClientRect(layoutElement.ClippingXWithoutFit,
                          layoutElement.ClippingYWithoutFit,
                          layoutElement.ClippingWidthWithoutFit,
                          layoutElement.ClippingHeightWithoutFit);
  }

  /// レイアウト要素のClipping領域(Fitオプションなし)の変更を試みる
  /// @param layoutElement レイアウト要素
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたClipping領域
  /// @return 試行結果（訂正箇所）
  public static TryResult TryChange(
      LayoutElement layoutElement, Names target, int value,
      out ClientRect changed) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    // 準備
    var original = ClippingInputCorrector.GetOriginal(layoutElement);
    var window = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 訂正
    switch (target) {
      case Names.X:
      case Names.Y: {
        return ClippingInputCorrector.TryChangePosition(
            original, target, value, window,
            Constants.MinimumClippingSize, out changed);
      }
      case Names.Width:
      case Names.Height: {
        return ClippingInputCorrector.TryChangeSize(
            original, target, value, window,
            Constants.MinimumClippingSize, out changed);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
}
}   // namespace SCFF.Common.Profile
