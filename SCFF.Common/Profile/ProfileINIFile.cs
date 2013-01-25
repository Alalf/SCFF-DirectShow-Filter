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
        
        return true;
      }
    } catch (Exception) {
      // 特に何も警告はしない
      Debug.WriteLine("Cannot save profile", "ProfileINIFile.Save");
      return false;
    }
  }
}
}   // namespace SCFF.Common.Profile
