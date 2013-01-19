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
  // Clipping X/Width/Y/Height
  //=================================================================

  /// ClippingXのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけvalueは訂正せずfixedWidthの調整で訂正を行う
  /// @param layoutElement 対象のレイアウト要素
  /// @param value TextBoxから入力された数値
  /// @param fixedX 訂正後のClippingX
  /// @param fixedWidth 訂正後のClippingWidth
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedX/fixedWidthである必要がある
  public static bool CorrectInputClippingX(ILayoutElementView layoutElement,
      int value, out int fixedX, out int fixedWidth) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    var result = true;
    fixedX = value;
    fixedWidth = layoutElement.ClippingWidthWithoutFit;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);
 
    // 上限・下限の補正
    if (fixedX < windowRect.X) {
      // Xが左にはみでている場合、Widthは保持＆Xを右にずらす
      fixedX = windowRect.X;
      result = false;
    } else if (windowRect.Right < fixedX) {
      // Xが右にはみでている場合、Xを左にずらしてWidthを0に
      fixedX = windowRect.Right;
      fixedWidth = 0;
      result = false;
    }
      
    // Widthの補正
    if (fixedWidth < 0) {
      // Widthは0以上
      fixedWidth = 0;
      result = false;
    }
    if (windowRect.Right < fixedX + fixedWidth) {
      // 領域が右にはみでている場合、Xは保持＆Widthを縮める
      fixedWidth = windowRect.Right - fixedX;
      result = false;
    }
  
    // 出力
    Debug.Assert(windowRect.X <= fixedX && fixedX + fixedWidth <= windowRect.Right);
    return result;
  }

  /// ClippingWidthのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけvalueは訂正せずfixedXの調整で訂正を行う
  /// @param layoutElement 対象のレイアウト要素
  /// @param value TextBoxから入力された数値
  /// @param fixedX 訂正後のClippingX
  /// @param fixedWidth 訂正後のClippingWidth
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedX/fixedWidthである必要がある
  public static bool CorrectInputClippingWidth(ILayoutElementView layoutElement,
      int value, out int fixedX, out int fixedWidth) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    var result = true;
    fixedX = layoutElement.ClippingXWithoutFit;
    fixedWidth = value;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 上限・下限の補正
    if (fixedWidth < 0) {
      // Widthは0以上
      fixedWidth = 0;
      result = false;
    } else if (windowRect.Width < fixedWidth) {
      // Widthが大きすぎる場合はFitさせる
      fixedX = windowRect.X;
      fixedWidth = windowRect.Width;
      result = false;
    }
 
    // Xの補正
    if (fixedX < windowRect.X) {
      // Xが左にはみでている場合、Widthは保持＆Xを右にずらす
      fixedX = windowRect.X;
      result = false;
    } else if (windowRect.Right < fixedX) {
      // Xが右にはみでている場合、Xを左にずらしてWidthを0に
      fixedX = windowRect.Right;
      fixedWidth = 0;
      result = false;
    }
    if (windowRect.Right < fixedX + fixedWidth) {
      // 領域が右にはみでている場合、Widthは保持＆Xを左にずらす
      fixedX = windowRect.Right - fixedWidth;
      result = false;
    }
  
    // 出力
    Debug.Assert(windowRect.X <= fixedX && fixedX + fixedWidth <= windowRect.Right);
    return result;
  }

  /// ClippingYのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけvalueは訂正せずfixedHeightの調整で訂正を行う
  /// @param layoutElement 対象のレイアウト要素
  /// @param value TextBoxから入力された数値
  /// @param fixedY 訂正後のClippingY
  /// @param fixedHeight 訂正後のClippingHeight
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedY/fixedHeightである必要がある
  public static bool CorrectInputClippingY(ILayoutElementView layoutElement,
      int value, out int fixedY, out int fixedHeight) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    var result = true;
    fixedY = value;
    fixedHeight = layoutElement.ClippingHeightWithoutFit;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);
 
    // 上限・下限の補正
    if (fixedY < windowRect.Y) {
      // Yが上にはみでている場合、Heightは保持＆Yを下にずらす
      fixedY = windowRect.Y;
      result = false;
    } else if (windowRect.Bottom < fixedY) {
      // Yが下にはみでている場合、Yを上にずらしてHeightを0に
      fixedY = windowRect.Bottom;
      fixedHeight = 0;
      result = false;
    }
      
    // Heightの補正
    if (fixedHeight < 0) {
      // Heightは0以上
      fixedHeight = 0;
      result = false;
    }
    if (windowRect.Bottom < fixedY + fixedHeight) {
      // 領域が下にはみでている場合、Yは保持＆Heightを縮める
      fixedHeight = windowRect.Bottom - fixedY;
      result = false;
    }
  
    // 出力
    Debug.Assert(windowRect.Y <= fixedY && fixedY + fixedHeight <= windowRect.Bottom);
    return result;
  }

  /// ClippingHeightのユーザ入力を制約(WindowSizeなど)に基づいて訂正する
  /// @attention できるだけvalueは訂正せずfixedYの調整で訂正を行う
  /// @param layoutElement 対象のレイアウト要素
  /// @param value TextBoxから入力された数値
  /// @param fixedY 訂正後のClippingY
  /// @param fixedHeight 訂正後のClippingHeight
  /// @retval true valueは制約を満たした値である
  /// @retval false 制約を満たすにはfixedY/fixedHeightである必要がある
  public static bool CorrectInputClippingHeight(ILayoutElementView layoutElement,
      int value, out int fixedY, out int fixedHeight) {
    Debug.Assert(layoutElement.IsWindowValid);
    Debug.Assert(!layoutElement.Fit);

    var result = true;
    fixedY = layoutElement.ClippingYWithoutFit;
    fixedHeight = value;
    var windowRect = Utilities.GetWindowRect(layoutElement.WindowType, layoutElement.Window);

    // 上限・下限の補正
    if (fixedHeight < 0) {
      // Heightは0以上
      fixedHeight = 0;
      result = false;
    } else if (windowRect.Height < fixedHeight) {
      // Heightが大きすぎる場合はFitさせる
      fixedY = windowRect.Y;
      fixedHeight = windowRect.Height;
      result = false;
    }
 
    // Yの補正
    if (fixedY < windowRect.Y) {
      // Yが上にはみでている場合、Heightは保持＆Yを下にずらす
      fixedY = windowRect.Y;
      result = false;
    } else if (windowRect.Bottom < fixedY) {
      // Yが下にはみでている場合、Yを上にずらしてHeightを0に
      fixedY = windowRect.Bottom;
      fixedHeight = 0;
      result = false;
    }
    if (windowRect.Bottom < fixedY + fixedHeight) {
      // 領域が下にはみでている場合、Heightは保持＆Yを上にずらす
      fixedY = windowRect.Bottom - fixedHeight;
      result = false;
    }
  
    // 出力
    Debug.Assert(windowRect.Y <= fixedY && fixedY + fixedHeight <= windowRect.Height);
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
