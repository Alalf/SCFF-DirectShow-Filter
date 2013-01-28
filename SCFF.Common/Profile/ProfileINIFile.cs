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

/// @file SCFF.Common/Profile/ProfileINIFile.cs
/// @copydoc SCFF::Common::Profile::ProfileINIFile

namespace SCFF.Common.Profile {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/// プロファイルのINIファイル入出力機能
public static class ProfileINIFile {
  //===================================================================
  // 定数
  //===================================================================

  /// INIファイルの拡張子
  public const string ProfileExtension = ".scffprofile";

  //-------------------------------------------------------------------

  /// INIファイルの先頭に付加するヘッダー
  private const string ProfileHeader = "; " + Constants.SCFFVersion;

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @param profile 保存するProfile
  /// @param path 保存先
  /// @return 保存が成功したか
  public static bool Save(Profile profile, string path) {
    try {
      using (var writer = new StreamWriter(path)) {
        writer.WriteLine(ProfileINIFile.ProfileHeader);
        writer.WriteLine("LayoutElementCount={0}", profile.LayoutElementCount);
        writer.WriteLine("CurrentIndex={0}", profile.CurrentIndex);
        foreach (var layoutElement in profile) {
          var index = layoutElement.Index;
          writer.WriteLine("[LayoutElement{0}]", index);

          // TargetWindow
          writer.WriteLine("Window{0}={1}", index, layoutElement.Window);
          writer.WriteLine("WindowType{0}={1}", index, layoutElement.WindowType);
          // Area
          writer.WriteLine("Fit{0}={1}", index, layoutElement.Fit);
          writer.WriteLine("ClippingXWithoutFit{0}={1}", index, layoutElement.ClippingXWithoutFit);
          writer.WriteLine("ClippingYWithoutFit{0}={1}", index, layoutElement.ClippingYWithoutFit);
          writer.WriteLine("ClippingWidthWithoutFit{0}={1}", index, layoutElement.ClippingWidthWithoutFit);
          writer.WriteLine("ClippingHeightWithoutFit{0}={1}", index, layoutElement.ClippingHeightWithoutFit);
          // Options
          writer.WriteLine("ShowCursor{0}={1}", index, layoutElement.ShowCursor);
          writer.WriteLine("ShowLayeredWindow{0}={1}", index, layoutElement.ShowLayeredWindow);
          writer.WriteLine("Stretch{0}={1}", index, layoutElement.Stretch);
          writer.WriteLine("KeepAspectRatio{0}={1}", index, layoutElement.KeepAspectRatio);
          writer.WriteLine("RotateDirection{0}={1}", index, layoutElement.RotateDirection);
          // ResizeMethod
          writer.WriteLine("SWScaleFlags{0}={1}", index, layoutElement.SWScaleFlags);
          writer.WriteLine("SWScaleAccurateRnd{0}={1}", index, layoutElement.SWScaleAccurateRnd);
          writer.WriteLine("SWScaleIsFilterEnabled{0}={1}", index, layoutElement.SWScaleIsFilterEnabled);
          writer.WriteLine("SWScaleLumaGBlur{0}={1}", index, layoutElement.SWScaleLumaGBlur);
          writer.WriteLine("SWScaleChromaGBlur{0}={1}", index, layoutElement.SWScaleChromaGBlur);
          writer.WriteLine("SWScaleLumaSharpen{0}={1}", index, layoutElement.SWScaleLumaSharpen);
          writer.WriteLine("SWScaleChromaSharpen{0}={1}", index, layoutElement.SWScaleChromaSharpen);
          writer.WriteLine("SWScaleChromaHShift{0}={1}", index, layoutElement.SWScaleChromaHShift);
          writer.WriteLine("SWScaleChromaVShift{0}={1}", index, layoutElement.SWScaleChromaVShift);
          // LayoutParameter
          writer.WriteLine("BoundRelativeLeft{0}={1}", index, layoutElement.BoundRelativeLeft);
          writer.WriteLine("BoundRelativeTop{0}={1}", index, layoutElement.BoundRelativeTop);
          writer.WriteLine("BoundRelativeRight{0}={1}", index, layoutElement.BoundRelativeRight);
          writer.WriteLine("BoundRelativeBottom{0}={1}", index, layoutElement.BoundRelativeBottom);
          // Backup
          writer.WriteLine("HasBackedUp{0}={1}", index, layoutElement.HasBackedUp);
          writer.WriteLine("BackupScreenClippingX{0}={1}", index, layoutElement.BackupScreenClippingX);
          writer.WriteLine("BackupScreenClippingY{0}={1}", index, layoutElement.BackupScreenClippingY);
          writer.WriteLine("BackupClippingWidth{0}={1}", index, layoutElement.BackupClippingWidth);
          writer.WriteLine("BackupClippingHeight{0}={1}", index, layoutElement.BackupClippingHeight);
        }
        return true;
      }
    } catch (Exception) {
      // 特に何も警告はしない
      Debug.WriteLine("Cannot save profile", "ProfileINIFile.Save");
      return false;
    }
  }

  //===================================================================
  // ファイル入力
  //===================================================================

  /// ファイル入力
  public static bool Load(Profile profile, string path) {
    // ファイル->ディクショナリ
    Dictionary<string,string> labelToRawData;
    var fileResult = Utilities.LoadDictionaryFromINIFile(path, out labelToRawData);
    if (!fileResult) return false;

    // ディクショナリ->Profile
    return ProfileINIFile.LoadFromDictionary(labelToRawData, profile);
  }

  private static bool LoadFromDictionary(Dictionary<string, string> labelToRawData, Profile profile) {
    // Profileのクリア
    profile.RestoreDefault();

    // 使いまわすので注意
    string stringValue;
    int intValue;
    double doubleValue;
    float floatValue;
    bool boolValue;

    // Dictionaryを調べながら値を設定する
    var originalLayoutElementCount = profile.LayoutElementCount;
    if (labelToRawData.TryGetInt("LayoutElementCount", out intValue)) {
      // 範囲チェック
      if (intValue < 1 ||
          Constants.MaxLayoutElementCount < intValue) {
        return false;
      }
      profile.LayoutElementCount = intValue;
    }

    if (labelToRawData.TryGetInt("CurrentIndex", out intValue)) {
      //　範囲チェック
      if (intValue < 0 || profile.LayoutElementCount <= intValue) {
        return false;
      }
      profile.CurrentIndex = intValue;
    }
    
    for (int i = 0; i < profile.LayoutElementCount; ++i) {
      var layoutElement = profile.GetLayoutElement(i);
      layoutElement.RestoreDefault();

      // TargetWindow
      WindowTypes windowTypes;
      if (labelToRawData.TryGetValue("WindowType" + i, out stringValue)) {
        if (Enum.TryParse<WindowTypes>(stringValue, out windowTypes)) {
          switch (windowTypes) {
            case WindowTypes.Normal: {
              UIntPtr uintptrValue;
              if (labelToRawData.TryGetUIntPtr("Window" + i, out uintptrValue)) {
                layoutElement.SetWindow(uintptrValue);
              }
              break;
            }
            case WindowTypes.DesktopListView: {
              layoutElement.SetWindowToDesktopListView();
              break;
            }
            case WindowTypes.Desktop: {
              layoutElement.SetWindowToDesktop();
              break;
            }
          }
        }
      }

      // Area
      if (labelToRawData.TryGetBool("Fit" + i, out boolValue)) {
        layoutElement.SetFit = boolValue;
      }
      if (labelToRawData.TryGetInt("ClippingXWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingXWithoutFit = intValue;
      }
      if (labelToRawData.TryGetInt("ClippingYWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingYWithoutFit = intValue;
      }
      if (labelToRawData.TryGetInt("ClippingWidthWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingWidthWithoutFit = intValue;
      }
      if (labelToRawData.TryGetInt("ClippingHeightWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingHeightWithoutFit = intValue;
      }
      // Options
      if (labelToRawData.TryGetBool("ShowCursor" + i, out boolValue)) {
        layoutElement.SetShowCursor = boolValue;
      }
      if (labelToRawData.TryGetBool("ShowLayeredWindow" + i, out boolValue)) {
        layoutElement.SetShowLayeredWindow = boolValue;
      }
      if (labelToRawData.TryGetBool("Stretch" + i, out boolValue)) {
        layoutElement.SetStretch = boolValue;
      }
      if (labelToRawData.TryGetBool("KeepAspectRatio" + i, out boolValue)) {
        layoutElement.SetKeepAspectRatio = boolValue;
      }
      RotateDirections rotateDirections;
      if (labelToRawData.TryGetEnum<RotateDirections>("RotateDirection" + i, out rotateDirections)) {
        layoutElement.SetRotateDirection = rotateDirections;
      }
      // ResizeMethod
      SWScaleFlags swscaleFlags;
      if (labelToRawData.TryGetEnum<SWScaleFlags>("SWScaleFlags" + i, out swscaleFlags)) {
        layoutElement.SetSWScaleFlags = swscaleFlags;
      }
      if (labelToRawData.TryGetBool("SWScaleAccurateRnd" + i, out boolValue)) {
        layoutElement.SetSWScaleAccurateRnd = boolValue;
      }
      if (labelToRawData.TryGetBool("SWScaleIsFilterEnabled" + i, out boolValue)) {
        layoutElement.SetSWScaleIsFilterEnabled = boolValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleLumaGBlur" + i, out floatValue)) {
        layoutElement.SetSWScaleLumaGBlur = floatValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleChromaGBlur" + i, out floatValue)) {
        layoutElement.SetSWScaleChromaGBlur = floatValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleLumaSharpen" + i, out floatValue)) {
        layoutElement.SetSWScaleLumaSharpen = floatValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleChromaSharpen" + i, out floatValue)) {
        layoutElement.SetSWScaleChromaSharpen = floatValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleChromaHShift" + i, out floatValue)) {
        layoutElement.SetSWScaleChromaHShift = floatValue;
      }
      if (labelToRawData.TryGetFloat("SWScaleChromaVShift" + i, out floatValue)) {
        layoutElement.SetSWScaleChromaVShift = floatValue;
      }
      // LayoutParameter
      if (labelToRawData.TryGetDouble("BoundRelativeLeft" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeLeft = doubleValue;
      }
      if (labelToRawData.TryGetDouble("BoundRelativeTop" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeTop = doubleValue;
      }
      if (labelToRawData.TryGetDouble("BoundRelativeRight" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeRight = doubleValue;
      }
      if (labelToRawData.TryGetDouble("BoundRelativeBottom" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeBottom = doubleValue;
      }
      // Backup
      if (labelToRawData.TryGetBool("HasBackedUp" + i, out boolValue)) {
        layoutElement.SetHasBackedUp = boolValue;
      }
      if (labelToRawData.TryGetInt("BackupScreenClippingX" + i, out intValue)) {
        layoutElement.SetBackupScreenClippingX = intValue;
      }
      if (labelToRawData.TryGetInt("BackupScreenClippingY" + i, out intValue)) {
        layoutElement.SetBackupScreenClippingY = intValue;
      }
      if (labelToRawData.TryGetInt("BackupClippingWidth" + i, out intValue)) {
        layoutElement.SetBackupClippingWidth = intValue;
      }
      if (labelToRawData.TryGetInt("BackupClippingHeight" + i, out intValue)) {
        layoutElement.SetBackupClippingHeight = intValue;
      }
    }

    return true;
  }
}
}   // namespace SCFF.Common.Profile
