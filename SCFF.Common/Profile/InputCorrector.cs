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

/// @file SCFF.Common/Profile/InputCorrector.cs
/// @copydoc SCFF::Common::Profile::InputCorrector

namespace SCFF.Common.Profile {

using System.Diagnostics;

/// 入力エラーを判定するためのメソッドを集めたstaticクラス
/// @attention Decoratorパターンにすべきではない(newが多くなりすぎるので)
public static class InputCorrector {
  //=================================================================
  // 共通
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

  //=================================================================
  // LayoutElement.Clipping*
  //=================================================================

  /// LayoutElement.Clipping*の要素を表す列挙型
  public enum Clipping {
    X,
    Y,
    Width,
    Height
  }

  /// LayoutElement.Clipping*の関連するプロパティを返す
  public static Clipping GetDependent(Clipping target) {
    switch (target) {
      case Clipping.X: return Clipping.Width;
      case Clipping.Y: return Clipping.Height;
      case Clipping.Width: return Clipping.X;
      case Clipping.Height: return Clipping.Y;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }


  /// Clipping*の位置要素(X/Y)の変更を試みる
  /// @warning sizeLowerBound = 0以外の動作テストをしていない
  /// @param original 変更前のClientRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param bound 境界を表すClientRect
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたClientRect
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangeClippingPosition(ClientRect original,
      Clipping target, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    Debug.Assert(target == Clipping.X || target == Clipping.Y);

    // 準備
    var onX = (target == Clipping.X);
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
    } else if (positionUpperBound < position) {
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
                 position + size <= positionUpperBound);
    changed = onX
        ? new ClientRect(position, original.Y, size, original.Height)
        : new ClientRect(original.X, position, original.Width, size);
    return result;
  }

  /// Clipping*のサイズ要素(Width/Height)の変更を試みる
  /// @warning sizeLowerBound = 0以外の動作テストをしていない
  /// @param original 変更前のClientRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param bound 境界を表すClientRect
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたClientRect
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangeClippingSize(ClientRect original,
      Clipping target, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    Debug.Assert(target == Clipping.Width || target == Clipping.Height);

    // 準備
    var onX = (target == Clipping.Width);
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
    } else if (positionUpperBound < position) {
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
                 position + size <= positionUpperBound);
    changed = onX
        ? new ClientRect(position, original.Y, size, original.Height)
        : new ClientRect(original.X, position, original.Width, size);
    return result;
  }

  //-------------------------------------------------------------------

  /// レイアウト要素からClipping領域(Fitオプションなし)を取得
  private static ClientRect GetClippingRectWithoutFit(ILayoutElementView layoutElement) {
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
  public static TryResult TryChangeClippingRectWithoutFit(
      ILayoutElementView layoutElement, Clipping target, int value,
      out ClientRect changed) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    // 準備
    var original = InputCorrector.GetClippingRectWithoutFit(layoutElement);
    var window = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 訂正
    switch (target) {
      case Clipping.X:
      case Clipping.Y: {
        return InputCorrector.TryChangeClippingPosition(original, target, value, window, 0, out changed);
      }
      case Clipping.Width:
      case Clipping.Height: {
        return InputCorrector.TryChangeClippingSize(original, target, value, window, 0, out changed);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //=================================================================
  // LayoutElement.BoundRelative*
  //=================================================================

  /// LayoutElement.BoundRelative*の要素を表す列挙型
  public enum BoundRelative {
    Left,
    Top,
    Right,
    Bottom
  }

  /// LayoutElement.BoundRelative*の関連するプロパティを返す
  public static BoundRelative GetDependent(BoundRelative target) {
    switch (target) {
      case BoundRelative.Left: return BoundRelative.Right;
      case BoundRelative.Top: return BoundRelative.Bottom;
      case BoundRelative.Right: return BoundRelative.Left;
      case BoundRelative.Bottom: return BoundRelative.Top;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// BoundRelative*の値が小さい方の位置要素(Left/Top)の変更を試みる
  /// @param original 変更前のBoundRelativeRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたBoundRelativeRect
  /// @return 試行結果（訂正箇所）
  private static TryResult TryChangeBoundRelativeLow(RelativeLTRB original,
      BoundRelative target, double value, double sizeLowerBound,
      out RelativeLTRB changed) {
    Debug.Assert(target == BoundRelative.Left || target == BoundRelative.Top);

    // 準備
    var onX = (target == BoundRelative.Left);
    var low = value;
    var high = onX ? original.Right : original.Bottom;
    var result = TryResult.NothingChanged;

    // highの補正
    if (high < 0.0 + sizeLowerBound) {
      high = 0.0 + sizeLowerBound;
      result |= TryResult.DependentChanged;
    } else if (1.0 < high) {
      high = 1.0;
      result |= TryResult.DependentChanged;
    }

    // 上限・下限の補正
    if (low < 0.0) {
      low = 0.0;
      result |= TryResult.TargetChanged;
    } else if (1.0 < low + sizeLowerBound) {
      low = 1.0 - sizeLowerBound;
      result |= TryResult.TargetChanged;
    }

    // lowが大きすぎる場合、highを減らして最小幅を確保
    if (high < low + sizeLowerBound) {
      high = low + sizeLowerBound;
      result |= TryResult.DependentChanged;
    }

    // 出力
    Debug.Assert(0.0 <= low &&
                 low + sizeLowerBound <= high &&
                 high <= 1.0);
    changed = onX
        ? new RelativeLTRB(low, original.Top, high, original.Bottom)
        : new RelativeLTRB(original.Left, low, original.Right, high);
    return result;
  }

  /// BoundRelative*の値が大きい方の位置要素(Right/Bottom)の変更を試みる
  /// @param original 変更前のBoundRelativeRect
  /// @param target 変更箇所
  /// @param value 変更値
  /// @param sizeLowerBound 訂正後のサイズ要素(Width/Height)の最小値
  /// @param[out] changed 変更後、制約を満たす形に訂正されたBoundRelativeRect
  /// @return 試行結果（訂正箇所）
  public static TryResult TryChangeBoundRelativeHigh(RelativeLTRB original,
      BoundRelative target, double value, double sizeLowerBound,
      out RelativeLTRB changed) {
    Debug.Assert(target == BoundRelative.Right || target == BoundRelative.Bottom);
 
    // 準備
    var onX = (target == BoundRelative.Right);
    var low = onX ? original.Left : original.Top;
    var high = value;
    var result = TryResult.NothingChanged;

    // lowの補正
    if (low < 0.0) {
      low = 0.0;
      result |= TryResult.DependentChanged;
    } else if (1.0 < low + sizeLowerBound) {
      low = 1.0 - sizeLowerBound;
      result |= TryResult.DependentChanged;
    }

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
                 low + sizeLowerBound <= high &&
                 high <= 1.0);
    changed = onX
        ? new RelativeLTRB(low, original.Top, high, original.Bottom)
        : new RelativeLTRB(original.Left, low, original.Right, high);
    return result;
  }

  //-------------------------------------------------------------------

  /// レイアウト要素からClipping領域(Fitオプションなし)を取得
  private static RelativeLTRB GetBoundRelativeLTRB(ILayoutElementView layoutElement) {
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
  public static TryResult TryChangeBoundRelativeLTRB(
      ILayoutElementView layoutElement, BoundRelative target, double value,
      out RelativeLTRB changed) {
    // 準備
    var original = InputCorrector.GetBoundRelativeLTRB(layoutElement);

    // 訂正
    switch (target) {
      case BoundRelative.Left:
      case BoundRelative.Top: {
        return InputCorrector.TryChangeBoundRelativeLow(original, target, value,
            Constants.MinimumBoundRelativeSize, out changed);
      }
      case BoundRelative.Right:
      case BoundRelative.Bottom: {
        return InputCorrector.TryChangeBoundRelativeHigh(original, target, value,
            Constants.MinimumBoundRelativeSize, out changed);
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //=================================================================
  // SWScale *
  //=================================================================

  /// LayoutElement.SWScale*の要素を表す列挙型
  public enum SWScale {
    LumaGBlur,
    LumaSharpen,
    ChromaHShift,
    ChromaGBlur,
    ChromaSharpen,
    ChromaVShift
  }

  /// レイアウト要素のSWScale*の変更を試みる
  public static bool TryChangeSWScaleValue(SWScale target, float value, out float changed) {
    float lowerBound = 0.0F;
    float upperBound = 0.0F;
    switch (target) {
      case SWScale.LumaGBlur:
      case SWScale.ChromaGBlur: {
        lowerBound = 0.0F;
        upperBound = 2.0F;
        break;
      }
      case SWScale.LumaSharpen:
      case SWScale.ChromaSharpen: {
        lowerBound = 0.0F;
        upperBound = 4.0F;
        break;
      }
      case SWScale.ChromaHShift:
      case SWScale.ChromaVShift: {
        lowerBound = 0.0F;
        upperBound = 1.0F;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
    if (value < lowerBound) {
      changed = lowerBound;
      return false;
    } else if (upperBound < value) {
      changed = upperBound;
      return false;
    } else {
      changed = value;
      return true;
    }
  }
}
}   // namespace SCFF.Common.Profile
