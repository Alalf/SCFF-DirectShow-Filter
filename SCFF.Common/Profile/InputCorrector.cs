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
  // 列挙型
  //=================================================================
  public enum TryResult {
    NothingChanged = 0x0,
    PositionChanged = 0x1,
    SizeChanged = 0x2,
    PositionAndSizeChanged = 0x3
  }

  //=================================================================
  // Clipping Position(X,Y)/Size(Width,Height)
  //=================================================================

  private static ClientRect GetClippingRectWithoutFit(ILayoutElementView layoutElement) {
    return new ClientRect(layoutElement.ClippingXWithoutFit,
                          layoutElement.ClippingYWithoutFit,
                          layoutElement.ClippingWidthWithoutFit,
                          layoutElement.ClippingHeightWithoutFit);
  }


  /// Rectの位置要素(X/Y)の変更を試みる
  /// @param original 変更前のRect
  /// @param position 変更対象の位置要素(X/Y)
  /// @param value 変更値
  /// @param bound 上限・下限を決めるRect
  /// @param sizeLowerBound 変更後のサイズ要素(Width/Height)の最小値
  /// @return 変更した場合のRect
  private static TryResult TryChangePosition(ClientRect original,
      Positions position, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    // 準備
    var fixedPosition = value;
    var fixedSize = (position == Positions.X) ? original.Width : original.Height;
    var positionLowerBound = (position == Positions.X) ? bound.X : bound.Y;
    var positionUpperBound = (position == Positions.X) ? bound.Right : bound.Bottom;
    var sizeUpperBound = (position == Positions.X) ? bound.Width : bound.Height;
    var tryResult = TryResult.NothingChanged;

    // 上限・下限の補正
    if (fixedPosition < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      fixedPosition = positionLowerBound;
      tryResult |= TryResult.PositionChanged;
    } else if (positionUpperBound < fixedPosition) {
      // Positionが大きすぎる場合、Positionを減らしてSizeを下限に
      fixedPosition = positionUpperBound - sizeLowerBound;
      fixedSize = sizeLowerBound;
      tryResult |= TryResult.PositionAndSizeChanged;
    }
      
    // Sizeの補正
    if (fixedSize < sizeLowerBound) {
      // Sizeは下限以上
      fixedSize = sizeLowerBound;
      tryResult |= TryResult.SizeChanged;
    }
    if (positionUpperBound < fixedPosition + fixedSize) {
      // 領域が境界内に収まらない場合、Positionは保持＆Sizeを縮める
      fixedSize = positionUpperBound - fixedPosition;
      tryResult |= TryResult.SizeChanged;
    }

    Debug.Assert(positionLowerBound <= fixedPosition &&
                 fixedPosition + fixedSize <= positionUpperBound);
    changed = (position == Positions.X)
        ? new ClientRect(fixedPosition, original.Y, fixedSize, original.Height)
        : new ClientRect(original.X, fixedPosition, original.Width, fixedSize);
    return tryResult;
  }
  /// Rectのサイズ要素(Width/Height)の変更を試みる
  /// @param original 変更前のRect
  /// @param size 変更対象のサイズ要素(Width/Height)
  /// @param value 変更値
  /// @param bound 上限・下限を決めるRect
  /// @param sizeLowerBound 変更後のサイズ要素(Width/Height)の最小値
  /// @return 変更した場合のRect
  private static TryResult TryChangeSize(ClientRect original,
      Sizes size, int value, ClientRect bound, int sizeLowerBound,
      out ClientRect changed) {
    // 準備
    var fixedPosition = (size == Sizes.Width) ? original.X : original.Y;
    var fixedSize = value;
    var positionLowerBound = (size == Sizes.Width) ? bound.X : bound.Y;
    var positionUpperBound = (size == Sizes.Width) ? bound.Right : bound.Bottom;
    var sizeUpperBound = (size == Sizes.Width) ? bound.Width : bound.Height;
    var tryResult = TryResult.NothingChanged;

    // 上限・下限の補正
    if (fixedSize < sizeLowerBound) {
      // Sizeは下限以上
      fixedSize = sizeLowerBound;
      tryResult |= TryResult.SizeChanged;
    } else if (sizeUpperBound < fixedSize) {
      // Sizeが大きすぎる場合はFitさせる
      fixedPosition = positionLowerBound;
      fixedSize = sizeUpperBound;
      tryResult |= TryResult.PositionAndSizeChanged;
    }
 
    // Positionの補正
    if (fixedPosition < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      fixedPosition = positionLowerBound;
      tryResult |= TryResult.PositionChanged;
    } else if (positionUpperBound < fixedPosition) {
      // Positionが大きすぎる場合、Positionを減らしてWidthを下限に
      fixedPosition = positionUpperBound - sizeLowerBound;
      fixedSize = sizeLowerBound;
      tryResult |= TryResult.PositionAndSizeChanged;
    }
    if (positionUpperBound < fixedPosition + fixedSize) {
      // 領域が境界内に収まらない場合、Sizeは保持＆Positionを小さく
      fixedPosition = positionUpperBound - fixedSize;
      tryResult |= TryResult.PositionChanged;
    }

    Debug.Assert(positionLowerBound <= fixedPosition &&
                 fixedPosition + fixedSize <= positionUpperBound);
    changed = (size == Sizes.Width)
        ? new ClientRect(fixedPosition, original.Y, fixedSize, original.Height)
        : new ClientRect(original.X, fixedPosition, original.Width, fixedSize);
    return tryResult;
  }

  public static TryResult TryChangeClippingPosition(
      ILayoutElementView layoutElement, Positions position, int value,
      out ClientRect changed) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    // 準備
    var original = InputCorrector.GetClippingRectWithoutFit(layoutElement);
    var window = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 訂正
    return InputCorrector.TryChangePosition(original, position, value, window, 0, out changed);
  }

  public static TryResult TryChangeClippingSize(
      ILayoutElementView layoutElement, Sizes size, int value,
      out ClientRect changed) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    // 準備
    var original = InputCorrector.GetClippingRectWithoutFit(layoutElement);
    var window = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 訂正
    return InputCorrector.TryChangeSize(original, size, value, window, 0, out changed);
  }

  //=================================================================
  // BoundRelative Left/Right/Top/Bottom
  //=================================================================

  /// BoundRelativeLeftのユーザ入力を訂正する
  public static bool CorrectInputBoundRelativeLeft(double inputLeft, double originalRight, out double fixedLeft, out double fixedRight) {
    var result = true;
    fixedLeft = inputLeft;
    fixedRight = originalRight;

    // Rightの補正
    if (fixedRight < 0.0 + Constants.MinimumBoundRelativeSize) {
      fixedRight = 0.0 + Constants.MinimumBoundRelativeSize;
      result = false;
    } else if (1.0 < fixedRight) {
      fixedRight = 1.0;
      result = false;
    }

    // 上限・下限の補正
    if (fixedLeft < 0.0) {
      fixedLeft = 0.0;
      result = false;
    } else if (1.0 < fixedLeft + Constants.MinimumBoundRelativeSize) {
      fixedLeft = 1.0 - Constants.MinimumBoundRelativeSize;
      result = false;
    }
      
    // 右にはみ出ていたらRightを右にずらして最小幅を確保
    if (fixedRight < fixedLeft + Constants.MinimumBoundRelativeSize) {
      fixedRight = fixedLeft + Constants.MinimumBoundRelativeSize;
      result = false;
    }
        
    // 出力
    Debug.Assert(0.0 <= fixedLeft &&
                 fixedLeft + Constants.MinimumBoundRelativeSize <= fixedRight &&
                 fixedRight <= 1.0);
    return result;
  }
  /// BoundRelativeRightのユーザ入力を訂正する
  public static bool CorrectInputBoundRelativeRight(double originalLeft, double inputRight, out double fixedLeft, out double fixedRight) {
    var result = true;
    fixedLeft = originalLeft;
    fixedRight = inputRight;

    // Leftの補正
    if (fixedLeft < 0.0) {
      fixedLeft = 0.0;
      result = false;
    } else if (1.0 < fixedLeft + Constants.MinimumBoundRelativeSize) {
      fixedLeft = 1.0 - Constants.MinimumBoundRelativeSize;
      result = false;
    }

    // 上限・下限の補正
    if (fixedRight < 0.0 + Constants.MinimumBoundRelativeSize) {
      fixedRight = 0.0 + Constants.MinimumBoundRelativeSize;
      result = false;
    } else if (1.0 < fixedRight) {
      fixedRight = 1.0;
      result = false;
    }
      
    // 左にはみ出ていたらLeftを左にずらして最小幅を確保
    if (fixedRight < fixedLeft + Constants.MinimumBoundRelativeSize) {
      fixedLeft = fixedRight - Constants.MinimumBoundRelativeSize;
      result = false;
    }
        
    // 出力
    Debug.Assert(0.0 <= fixedLeft &&
                 fixedLeft + Constants.MinimumBoundRelativeSize <= fixedRight &&
                 fixedRight <= 1.0);
    return result;
  }
  /// BoundRelativeTopのユーザ入力を訂正する
  public static bool CorrectInputBoundRelativeTop(double inputTop, double originalBottom, out double fixedTop, out double fixedBottom) {
    throw new System.NotImplementedException();
  }
  /// BoundRelativeBottomのユーザ入力を訂正する
  public static bool CorrectInputBoundRelativeBottom(double originalTop, double inputBottom, out double fixedTop, out double fixedBottom) {
    throw new System.NotImplementedException();
  }

  //=================================================================
  // SWScale *
  //=================================================================

  /// SWScaleLumaGBlurのユーザ入力を訂正する
  public static bool CorrectInputSWScaleLumaGBlur(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
  /// SWScaleLumaSharpenのユーザ入力を訂正する
  public static bool CorrectInputSWScaleLumaSharpen(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
  /// SWScaleChromaHshiftのユーザ入力を訂正する
  public static bool CorrectInputSWScaleChromaHshift(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
  /// SWScaleChromaGBlurのユーザ入力を訂正する
  public static bool CorrectInputSWScaleChromaGBlur(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
  /// SWScaleChromaSharpenのユーザ入力を訂正する
  public static bool CorrectInputSWScaleChromaSharpen(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
  /// SWScaleChromaVshiftのユーザ入力を訂正する
  public static bool CorrectInputSWScaleChromaVshift(double value, out double fixedValue) {
    throw new System.NotImplementedException();
  }
}
}   // namespace SCFF.Common.Profile
