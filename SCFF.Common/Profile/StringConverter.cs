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

/// @file SCFF.Common/Profile/StringConverter.cs
/// @copydoc SCFF::Common::Profile::StringConverter

namespace SCFF.Common.Profile {

using System;

/// 値を文字列に変換するためのstaticクラス
/// @attention Decoratorパターンにすべきではない(newが多くなりすぎるので)
public static class StringConverter {
  //=================================================================
  // Header
  //=================================================================

  /// LayoutParameter用ヘッダー文字列
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param maxLength 文字列の長さの上限
  /// @return LayoutParameter用ヘッダー文字列
  public static string GetHeaderStringForLayoutParameter(ILayoutElementView layoutElement, int maxLength) {
    var header = string.Format("Layout {0:D}: {1}",
                                layoutElement.Index + 1,
                                layoutElement.WindowCaption);
    return header.Substring(0, Math.Min(header.Length, maxLength));
  }

  /// LayoutEdit用ヘッダー文字列
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param isCurrent 現在選択中のLayoutElementか
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return LayoutEdit用ヘッダー文字列
  public static string GetHeaderStringForLayoutEdit(ILayoutElementView layoutElement, bool isCurrent, bool isDummy, int sampleWidth, int sampleHeight) {
    if (isCurrent && isDummy) {
      return string.Format(" [{0}] {1}", layoutElement.Index + 1, layoutElement.WindowCaption);
    } else if (isCurrent) {
      var boundRect = layoutElement.GetBoundRect(sampleWidth, sampleHeight);
      return string.Format(" [{0}] ({1}x{2}) {3}",
          layoutElement.Index + 1,
          boundRect.Width,
          boundRect.Height,
          layoutElement.WindowCaption);
    } else {
      return string.Format(" [{0}]", layoutElement.Index + 1);
    }
  }

  //=================================================================
  // Window
  //=================================================================

  /// ウィンドウ名
  public static string GetWindowCaption(ILayoutElementView layoutElement) {
    if (layoutElement.IsWindowValid) {
      return layoutElement.WindowCaption;
    } else {
      return string.Format("n/a ({0})", layoutElement.WindowCaption);
    }
  }

  //=================================================================
  // Clipping X/Y/Width/Height
  //=================================================================

  /// ClippingX表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return ClippingX表示用文字列
  public static string GetClippingXString(ILayoutElementView layoutElement) {
    if (layoutElement.Fit && !layoutElement.IsWindowValid) {
      return "n/a";
    } else if (layoutElement.Fit) {
      return string.Format("{0}*", layoutElement.ClippingXWithFit);
    } else {
      return layoutElement.ClippingXWithFit.ToString();
    }
  }
  /// ClippingY表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return ClippingY表示用文字列
  public static string GetClippingYString(ILayoutElementView layoutElement) {
    if (layoutElement.Fit && !layoutElement.IsWindowValid) {
      return "n/a";
    } else if (layoutElement.Fit) {
      return string.Format("{0}*", layoutElement.ClippingYWithFit);
    } else {
      return layoutElement.ClippingYWithFit.ToString();
    }
  }
  /// ClippingWidth表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return ClippingWidth表示用文字列
  public static string GetClippingWidthString(ILayoutElementView layoutElement) {
    if (layoutElement.Fit && !layoutElement.IsWindowValid) {
      return "n/a";
    } else if (layoutElement.Fit) {
      return string.Format("{0}*", layoutElement.ClippingWidthWithFit);
    } else {
      return layoutElement.ClippingWidthWithFit.ToString();
    }
  }
  /// ClippingHeight表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return ClippingHeight表示用文字列
  public static string GetClippingHeightString(ILayoutElementView layoutElement) {
    if (layoutElement.Fit && !layoutElement.IsWindowValid) {
      return "n/a";
    } else if (layoutElement.Fit) {
      return string.Format("{0}*", layoutElement.ClippingHeightWithFit);
    } else {
      return layoutElement.ClippingHeightWithFit.ToString();
    }
  }

  //=================================================================
  // BoundRelative Left/Top/Right/Bottom
  //=================================================================

  /// BoundRelativeLeft表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return BoundRelativeLeft表示用文字列
  public static string GetBoundRelativeLeftString(ILayoutElementView layoutElement) {
    return layoutElement.BoundRelativeLeft.ToString("F3");
  }
  /// BoundRelativeRight表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return BoundRelativeRight表示用文字列
  public static string GetBoundRelativeRightString(ILayoutElementView layoutElement) {
    return layoutElement.BoundRelativeRight.ToString("F3");
  }
  /// BoundRelativeTop表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return BoundRelativeTop表示用文字列
  public static string GetBoundRelativeTopString(ILayoutElementView layoutElement) {
    return layoutElement.BoundRelativeTop.ToString("F3");
  }
  /// BoundRelativeBottom表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return BoundRelativeBottom表示用文字列
  public static string GetBoundRelativeBottomString(ILayoutElementView layoutElement) {
    return layoutElement.BoundRelativeBottom.ToString("F3");
  }

  //=================================================================
  // Bound X/Y/Width/Height
  //=================================================================

  /// GetBoundRectの引数用
  private const int dummySampleSize = 100;

  /// BoundX表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @return BoundX表示用文字列
  public static string GetBoundXString(ILayoutElementView layoutElement, bool isDummy, int sampleWidth) {
    var boundRect = layoutElement.GetBoundRect(sampleWidth, StringConverter.dummySampleSize);
    return isDummy ? string.Format("({0})", boundRect.X)
                   : boundRect.X.ToString();
  }
  /// BoundY表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleHeight サンプルの高さ
  /// @return BoundY表示用文字列
  public static string GetBoundYString(ILayoutElementView layoutElement, bool isDummy, int sampleHeight) {
    var boundRect = layoutElement.GetBoundRect(StringConverter.dummySampleSize, sampleHeight);
    return isDummy ? string.Format("({0})", boundRect.Y)
                   : boundRect.Y.ToString();
  }
  /// BoundWidth表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @return BoundWidth表示用文字列
  public static string GetBoundWidthString(ILayoutElementView layoutElement, bool isDummy, int sampleWidth) {
    var boundRect = layoutElement.GetBoundRect(sampleWidth, StringConverter.dummySampleSize);
    return isDummy ? string.Format("({0})", boundRect.Width)
                   : boundRect.Width.ToString();
  }
  /// BoundHeight表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleHeight サンプルの高さ
  /// @return BoundHeight表示用文字列
  public static string GetBoundHeightString(ILayoutElementView layoutElement, bool isDummy, int sampleHeight) {
    var boundRect = layoutElement.GetBoundRect(StringConverter.dummySampleSize, sampleHeight);
    return isDummy ? string.Format("({0})", boundRect.Height)
                   : boundRect.Height.ToString();
  }

  //=================================================================
  // SWScale
  //=================================================================

  /// SWScaleLumaGBlur表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleLumaGBlur表示用文字列
  public static string GetSWScaleLumaGBlurString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleLumaGBlur.ToString("F2");
  }
  /// SWScaleLumaSharpen表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleLumaSharpen表示用文字列
  public static string GetSWScaleLumaSharpenString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleLumaSharpen.ToString("F2"); 
  } 
  /// SWScaleChromaHshift表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleChromaHshift表示用文字列
  public static string GetSWScaleChromaHshiftString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleChromaHshift.ToString("F2");
  }
  /// SWScaleChromaGBlur表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleChromaGBlur表示用文字列
  public static string GetSWScaleChromaGBlurString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleChromaGBlur.ToString("F2");
  }
  /// SWScaleChromaSharpen表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleChromaSharpen表示用文字列
  public static string GetSWScaleChromaSharpenString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleChromaSharpen.ToString("F2");
  }
  /// SWScaleChromaVshift表示用
  /// @param layoutElement データ取得元のレイアウト要素
  /// @return SWScaleChromaVshift表示用文字列
  public static string GetSWScaleChromaVshiftString(ILayoutElementView layoutElement) {
    return layoutElement.SWScaleChromaVshift.ToString("F2");
  }
}
}   // namespace SCFF.Common.Profile
