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

/// プロファイル内を編集するためのカーソルクラスインタフェース
///
/// @attention HACK!: Usingで論理単位での編集後にタイムスタンプを更新する
public interface ILayoutElement {

  /// 初期値に戻す
  void RestoreDefault();

  /// 編集開始
  void Open();

  /// 編集完了
  void Close();

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
  bool SetFit { set; }

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingXWithoutFit
  int SetClippingXWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingYWithoutFit
  int SetClippingYWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingWidthWithoutFit
  int SetClippingWidthWithoutFit { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::ClippingHeightWithoutFit
  int SetClippingHeightWithoutFit { set; }

  //=================================================================
  // Options
  //=================================================================

  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
  bool SetShowCursor { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
  bool SetShowLayeredWindow { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
  bool SetStretch { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
  bool SetKeepAspectRatio { set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
  RotateDirections SetRotateDirection { set; }

  //=================================================================
  // ResizeMethod
  //=================================================================

  /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
  SWScaleFlags SetSWScaleFlags { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
  bool SetSWScaleAccurateRnd { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
  bool SetSWScaleIsFilterEnabled { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
  float SetSWScaleLumaGBlur { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
  float SetSWScaleChromaGBlur { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
  float SetSWScaleLumaSharpen { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
  float SetSWScaleChromaSharpen { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHShift
  float SetSWScaleChromaHShift { set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVShift
  float SetSWScaleChromaVShift { set; }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeLeft
  double SetBoundRelativeLeft { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeTop
  double SetBoundRelativeTop { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeRight
  double SetBoundRelativeRight { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BoundRelativeBottom
  double SetBoundRelativeBottom { set; }

  /// LayoutParameterをサンプル座標系で調整する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  void FitBoundRelativeRect(int sampleWidth, int sampleHeight);

  //=================================================================
  // Screen
  //=================================================================

  /// スクリーン座標系のRectとIntersectを取り、新しいClipping領域に設定する。
  /// @param nextScreenRect 新しいClipping領域(Intersect前)
  void SetClippingRectByScreenRect(ScreenRect nextScreenRect);

  //=================================================================
  // Backup
  //=================================================================

  /// Backup*をすべて更新
  void UpdateBackupParameters();
  /// BackupからWindow/Clipping*を設定
  void RestoreBackupParameters();
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::HasBackedUp
  bool SetHasBackedUp { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingX
  int SetBackupScreenClippingX { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupScreenClippingY
  int SetBackupScreenClippingY { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingWidth
  int SetBackupClippingWidth { set; }
  /// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter::BackupClippingHeight
  int SetBackupClippingHeight { set; }
}
}   // namespace SCFF.Common.Profile
