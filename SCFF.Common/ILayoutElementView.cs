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

/// @file SCFF.Common/ILayoutElementView.cs
/// @copydoc SCFF::Common::ILayoutElementView

namespace SCFF.Common {

using System;

/// プロファイル内を参照するためのカーソルクラスインタフェース
public interface ILayoutElementView {
  /// インデックス
  int Index { get; }

  //=================================================================
  // TargetWindow
  //=================================================================

  /// Windowハンドル
  UIntPtr Window { get; }
  /// 正常なWindowハンドルかどうか
  bool IsWindowValid { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::WindowType
  WindowTypes WindowType { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::WindowCaption
  string WindowCaption { get; }

  //=================================================================
  // Area
  //=================================================================

  /// @copydoc SCFF::Common::AdditionalLayoutParameter::Fit
  bool Fit { get; }

  /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingXWithoutFit
  int ClippingXWithoutFit { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingYWithoutFit
  int ClippingYWithoutFit { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingWidthWithoutFit
  int ClippingWidthWithoutFit { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingHeightWithoutFit
  int ClippingHeightWithoutFit { get; }

  /// Fitオプションを考慮したクリッピング領域左上端のX座標
  /// @pre IsWindowValid == True
  int ClippingXWithFit { get; }
  /// Fitオプションを考慮したクリッピング領域左上端のY座標
  /// @pre IsWindowValid == True
  int ClippingYWithFit { get; }
  /// Fitオプションを考慮したクリッピング領域の幅
  /// @pre IsWindowValid == True
  int ClippingWidthWithFit { get; }
  /// Fitオプションを考慮したクリッピング領域の高さ
  /// @pre IsWindowValid == True
  int ClippingHeightWithFit { get; }

  //=================================================================
  // Options
  //=================================================================

  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
  bool ShowCursor { get; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
  bool ShowLayeredWindow { get; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
  bool Stretch { get; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
  bool KeepAspectRatio { get; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
  RotateDirections RotateDirection { get; }

  //=================================================================
  // ResizeMethod
  //=================================================================

  /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
  SWScaleFlags SWScaleFlags { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
  bool SWScaleAccurateRnd { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
  bool SWScaleIsFilterEnabled { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
  float SWScaleLumaGBlur { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
  float SWScaleChromaGBlur { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
  float SWScaleLumaSharpen { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
  float SWScaleChromaSharpen { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHshift
  float SWScaleChromaHshift { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVshift
  float SWScaleChromaVshift { get; }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeLeft
  double BoundRelativeLeft { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeTop
  double BoundRelativeTop { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeRight
  double BoundRelativeRight { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeBottom
  double BoundRelativeBottom { get; }

  /// 相対座標系でのレイアウト要素の幅
  double BoundRelativeWidth { get; }
  /// 相対座標系でのレイアウト要素の高さ
  double BoundRelativeHeight { get; }

  //=================================================================
  // Screen
  //=================================================================

  /// スクリーン座標系で表されたクリッピング領域(Fit考慮)を取得する
  ScreenRect ScreenClippingRectWithFit { get; }

  //=================================================================
  // Sample
  //=================================================================

  /// サンプル座標系でのレイアウト要素の領域
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return サンプル座標系でのレイアウト要素の領域
  SampleRect GetBoundRect(int sampleWidth, int sampleHeight);

  /// サンプル座標系でのレイアウト要素の画像部分が占める領域
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return サンプル座標系でのレイアウト要素の画像部分が占める領域
  SampleRect GetActualBoundRect(int sampleWidth, int sampleHeight);

  //=================================================================
  // ToString
  //=================================================================

  /// ヘッダー表示用文字列
  /// @param maxLength 文字列の長さの上限
  /// @return ヘッダー表示用文字列
  string GetHeaderString(int maxLength);
  /// ヘッダー表示用文字列(GUI用)
  /// @param isCurrent 現在選択中のLayoutElementか
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return ヘッダー表示用文字列(GUI用)
  string GetHeaderStringForGUI(bool isCurrent, bool isDummy, int sampleWidth, int sampleHeight);

  /// ClippingX表示用
  string ClippingXString { get; }
  /// ClippingY表示用
  string ClippingYString { get; }
  /// ClippingWidth表示用
  string ClippingWidthString { get; }
  /// ClippingHeight表示用
  string ClippingHeightString { get; }

  /// BoundRelativeLeft表示用
  string BoundRelativeLeftString { get; }
  /// BoundRelativeTop表示用
  string BoundRelativeTopString { get; }
  /// BoundRelativeRight表示用
  string BoundRelativeRightString { get; }
  /// BoundRelativeBottom表示用
  string BoundRelativeBottomString { get; }

  /// BoundLeft表示用
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @return BoundLeftを表す文字列
  string GetBoundLeftString(bool isDummy, int sampleWidth);
  /// BoundTop表示用
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleHeight サンプルの高さ
  /// @return BoundTopを表す文字列
  string GetBoundTopString(bool isDummy, int sampleHeight);
  /// BoundRight表示用
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleWidth サンプルの幅
  /// @return BoundWidthを表す文字列
  string GetBoundWidthString(bool isDummy, int sampleWidth);
  /// BoundBottom表示用
  /// @param isDummy ダミープレビューサイズかどうか
  /// @param sampleHeight サンプルの高さ
  /// @return BoundHeightを表す文字列
  string GetBoundHeightString(bool isDummy, int sampleHeight);

  /// SWScaleLumaGBlur表示用
  string SWScaleLumaGBlurString { get; }
  /// SWScaleLumaSharpen表示用
  string SWScaleLumaSharpenString { get; } 
  /// SWScaleChromaHshift表示用
  string SWScaleChromaHshiftString { get; }
  /// SWScaleChromaGBlur表示用
  string SWScaleChromaGBlurString { get; }
  /// SWScaleChromaSharpen表示用
  string SWScaleChromaSharpenString { get; }
  /// SWScaleChromaVshift表示用
  string SWScaleChromaVshiftString { get; }

  //=================================================================
  // Backup
  //=================================================================

  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupScreenClippingX
  int BackupScreenClippingX { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupScreenClippingY
  int BackupScreenClippingY { get;} 
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupClippingWidth
  int BackupClippingWidth { get; }
  /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupClippingHeight
  int BackupClippingHeight { get; }


}
}   // namespace SCFF.Common
