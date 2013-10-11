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

/// @file SCFF.Common/Profile/ILayoutElement.cs
/// @copydoc SCFF::Common::Profile::ILayoutElement

namespace SCFF.Common.Profile {

using System;
using SCFF.Interprocess;

/// プロファイル内を編集するためのカーソルクラスインタフェース
///
/// @attention HACK!: Usingで論理単位での編集後にタイムスタンプを更新する
public interface ILayoutElement {
  //=================================================================
  // TargetWindow
  //=================================================================

  /// WindowタイプをNormalに＋Windowハンドルを設定する
  /// @param window 設定するWindowハンドル
  void SetWindow(UIntPtr window);
  /// WindowタイプをDesktopにする
  void SetWindowToDesktop();
  /// WindowタイプをDesktopListViewにする
  void SetWindowToDesktopListView();

  //=================================================================
  // Area
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::Fit
  bool Fit { set; }

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingXWithoutFit
  int ClippingXWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingYWithoutFit
  int ClippingYWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingWidthWithoutFit
  int ClippingWidthWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingHeightWithoutFit
  int ClippingHeightWithoutFit { set; }

  //=================================================================
  // Options
  //=================================================================

  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
  bool ShowCursor { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
  bool ShowLayeredWindow { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
  bool Stretch { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
  bool KeepAspectRatio { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
  RotateDirections RotateDirection { set; }

  //=================================================================
  // ResizeMethod
  //=================================================================

  /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
  SWScaleFlags SWScaleFlags { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
  bool SWScaleAccurateRnd { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
  bool SWScaleIsFilterEnabled { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
  float SWScaleLumaGBlur { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
  float SWScaleChromaGBlur { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
  float SWScaleLumaSharpen { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
  float SWScaleChromaSharpen { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHShift
  float SWScaleChromaHShift { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVShift
  float SWScaleChromaVShift { set; }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeLeft
  double BoundRelativeLeft { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeTop
  double BoundRelativeTop { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeRight
  double BoundRelativeRight { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeBottom
  double BoundRelativeBottom { set; }

  /// LayoutParameterをサンプル座標系で調整する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  void FitBoundRelativeRect(int sampleWidth, int sampleHeight);

  //=================================================================
  // Screen
  //=================================================================

  /// Screen座標系のRectとIntersectを取り、新しいClipping領域に設定する。
  /// @param nextScreenRect 新しいClipping領域(Intersect前)
  void SetClippingRectByScreenRect(ScreenRect nextScreenRect);

  //=================================================================
  // Backup
  //=================================================================

  /// Backup*をすべて更新
  void UpdateBackupParameters();
  /// BackupからWindow/Clipping*を設定
  void RestoreBackupParameters();
  /// Backup*をクリア
  void ClearBackupParameters();

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::HasBackedUp
  bool HasBackedUp { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingX
  int BackupScreenClippingX { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingY
  int BackupScreenClippingY { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingWidth
  int BackupClippingWidth { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingHeight
  int BackupClippingHeight { set; }
}
}   // namespace SCFF.Common.Profile
