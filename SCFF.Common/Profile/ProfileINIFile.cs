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
  private const string ProfileHeader =
      "; SCFF-DirectShow-Filter Options Ver.0.1.7";

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


}
}   // namespace SCFF.Common.Profile
