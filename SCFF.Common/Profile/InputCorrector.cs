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
  // Clipping Position(X,Y)/Size(Width,Height)
  //=================================================================

  /// ClippingX/Yのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけX/Yは訂正せずWidth/Heightの調整で訂正を行う
  /// @param isHorizontal X-Widthが対象か
  /// @param layoutElement 対象のレイアウト要素
  /// @param position TextBoxから入力された数値
  /// @param fixedPosition 訂正後のClippingX/Y
  /// @param fixedSize 訂正後のClippingWidth/Height
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedPosition/fixedSizeである必要がある
  public static bool CorrectInputClippingPosition(bool isHorizontal,
      ILayoutElementView layoutElement, int position,
      out int fixedPosition, out int fixedSize) {
    // 準備
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);
    var result = true;
    fixedPosition = isHorizontal
        ? layoutElement.ClippingXWithoutFit : layoutElement.ClippingYWithoutFit;
    fixedSize = isHorizontal
        ? layoutElement.ClippingWidthWithoutFit : layoutElement.ClippingHeightWithoutFit;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);
    var positionLowerBound = isHorizontal ? windowRect.X : windowRect.Y;
    var positionUpperBound = isHorizontal ? windowRect.Right : windowRect.Bottom;
    var sizeLowerBound = 0;
    var sizeUpperBound = isHorizontal ? windowRect.Width : windowRect.Height;

    // 訂正開始
    fixedPosition = position;

    // 上限・下限の補正
    if (fixedPosition < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      fixedPosition = positionLowerBound;
      result = false;
    } else if (positionUpperBound < fixedPosition) {
      // Positionが大きすぎる場合、Positionを減らしてSizeを下限に
      fixedPosition = positionUpperBound - sizeLowerBound;
      fixedSize = sizeLowerBound;
      result = false;
    }
      
    // Sizeの補正
    if (fixedSize < sizeLowerBound) {
      // Sizeは下限以上
      fixedSize = sizeLowerBound;
      result = false;
    }
    if (positionUpperBound < fixedPosition + fixedSize) {
      // 領域が境界内に収まらない場合、Positionは保持＆Sizeを縮める
      fixedSize = positionUpperBound - fixedPosition;
      result = false;
    }
  
    // 出力
    Debug.Assert(positionLowerBound <= fixedPosition &&
                 fixedPosition + fixedSize <= positionUpperBound);
    return result;
  }

  /// ClippingWidth/Heightのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけWidth/Heightは訂正せずX/Yの調整で訂正を行う
  /// @param isHorizontal X-Widthが対象か
  /// @param layoutElement 対象のレイアウト要素
  /// @param size TextBoxから入力された数値
  /// @param fixedPosition 訂正後のClippingX
  /// @param fixedSize 訂正後のClippingWidth
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedPosition/fixedSizeである必要がある
  public static bool CorrectInputClippingSize(bool isHorizontal,
      ILayoutElementView layoutElement, int size,
      out int fixedPosition, out int fixedSize) {
    // 準備
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);
    var result = true;
    fixedPosition = isHorizontal
        ? layoutElement.ClippingXWithoutFit : layoutElement.ClippingYWithoutFit;
    fixedSize = isHorizontal
        ? layoutElement.ClippingWidthWithoutFit : layoutElement.ClippingHeightWithoutFit;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);
    var positionLowerBound = isHorizontal ? windowRect.X : windowRect.Y;
    var positionUpperBound = isHorizontal ? windowRect.Right : windowRect.Bottom;
    var sizeLowerBound = 0;
    var sizeUpperBound = isHorizontal ? windowRect.Width : windowRect.Height;

    // 訂正開始
    fixedSize = size;

    // 上限・下限の補正
    if (fixedSize < sizeLowerBound) {
      // Sizeは下限以上
      fixedSize = sizeLowerBound;
      result = false;
    } else if (sizeUpperBound < fixedSize) {
      // Sizeが大きすぎる場合はFitさせる
      fixedPosition = positionLowerBound;
      fixedSize = sizeUpperBound;
      result = false;
    }
 
    // Positionの補正
    if (fixedPosition < positionLowerBound) {
      // Positionが小さすぎる場合、Sizeは保持＆Positionを増やす
      fixedPosition = positionLowerBound;
      result = false;
    } else if (positionUpperBound < fixedPosition) {
      // Positionが大きすぎる場合、Positionを減らしてWidthを下限に
      fixedPosition = positionUpperBound - sizeLowerBound;
      fixedSize = sizeLowerBound;
      result = false;
    }
    if (positionUpperBound < fixedPosition + fixedSize) {
      // 領域が境界内に収まらない場合、Sizeは保持＆Positionを小さく
      fixedPosition = positionUpperBound - fixedSize;
      result = false;
    }
  
    // 出力
    Debug.Assert(positionLowerBound <= fixedPosition &&
                 fixedPosition + fixedSize <= positionUpperBound);
    return result;
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
