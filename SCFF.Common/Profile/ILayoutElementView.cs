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

/// @file SCFF.Common/Profile/ILayoutElementView.cs
/// @copydoc SCFF::Common::Profile::ILayoutElementView

namespace SCFF.Common.Profile {

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
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::WindowType
  WindowTypes WindowType { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::WindowCaption
  string WindowCaption { get; }

  //=================================================================
  // Area
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::Fit
  bool Fit { get; }

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingXWithoutFit
  int ClippingXWithoutFit { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingYWithoutFit
  int ClippingYWithoutFit { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingWidthWithoutFit
  int ClippingWidthWithoutFit { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingHeightWithoutFit
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
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHShift
  float SWScaleChromaHShift { get; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVShift
  float SWScaleChromaVShift { get; }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeLeft
  double BoundRelativeLeft { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeTop
  double BoundRelativeTop { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeRight
  double BoundRelativeRight { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeBottom
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
  // Backup
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::HasBackedUp
  bool HasBackedUp { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingX
  int BackupScreenClippingX { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingY
  int BackupScreenClippingY { get;} 
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingWidth
  int BackupClippingWidth { get; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingHeight
  int BackupClippingHeight { get; }
}
}   // namespace SCFF.Common.Profile
