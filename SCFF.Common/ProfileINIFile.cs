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

/// @file SCFF.Common/ProfileINIFile.cs
/// @copydoc SCFF::Common::ProfileINIFile

namespace SCFF.Common {

/// プロファイルのINIファイル入出力機能
public static class ProfileINIFile {
  //===================================================================
  // 定数
  //===================================================================

  /// プロファイル保存時のファイル名のPrefix
  private const string ProfilePathPrefix = "SCFF.Common.Profile.";
  /// プロファイル保存時のファイル名のSuffix(=拡張子)
  private const string ProfilePathSuffix = ".ini";
}
}   // namespace SCFF.Common
