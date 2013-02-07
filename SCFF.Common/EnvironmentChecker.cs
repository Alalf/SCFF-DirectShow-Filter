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

/// @file SCFF.Common/EnvironmentChecker.cs
/// @copydoc SCFF::Common::EnvironmentChecker

namespace SCFF.Common {

using System.IO;
using System.Text;
using Microsoft.Win32;

public static class EnvironmentChecker {
  //===================================================================
  // DirectShow Plugin関連
  //===================================================================

  /// インストール確認
  public static bool CheckSCFFDSFInstalled(out string message) {
    // チェック用変数
    var hasWin32DLLInstalled = false;
    var isWin32DLLFound = false;
    var win32DLLPath = string.Empty;
    var hasX64DLLInstalled = false;
    bool isX64DLLFound = false;
    string x64DLLPath = string.Empty;

    //------------------
    // 32bit版のチェック
    //------------------
    try {
      var baseKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry32);
      var scffSourceKey = baseKey.OpenSubKey(Constants.SCFFSourceRegistryKey);
      if (scffSourceKey != null) {
        hasWin32DLLInstalled = true;
      }

      var pathKey = scffSourceKey.OpenSubKey("InprocServer32");
      win32DLLPath = pathKey.GetValue("").ToString();
      if (File.Exists(win32DLLPath)) {
        isWin32DLLFound = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //------------------
    // 64bit版のチェック
    //------------------
    try {
      var baseKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
      var scffSourceKey = baseKey.OpenSubKey(Constants.SCFFSourceRegistryKey);
      if (scffSourceKey != null) {
        hasX64DLLInstalled = true;
      }

      var pathKey = scffSourceKey.OpenSubKey("InprocServer32");
      x64DLLPath = pathKey.GetValue("").ToString();
      if (File.Exists(x64DLLPath)) {
        isX64DLLFound = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //----------------------
    // メッセージ生成
    //----------------------
    var messageBuilder = new StringBuilder();

    // install_*.batが実行されていない
    if (!hasWin32DLLInstalled && !hasX64DLLInstalled) {
      messageBuilder.AppendLine("scff_dsf_*.ax is not correctly installed.");
      messageBuilder.AppendLine("Please re-install SCFF DirectShow Filter.");
      message = messageBuilder.ToString();
      return false;
    }

    // install_*.bat実行時の場所にDLLが無い
    if (hasWin32DLLInstalled && !isWin32DLLFound ||
        hasX64DLLInstalled && !isX64DLLFound) {
      messageBuilder.AppendLine("scff_dsf_*.ax is not found:");
      // Win32
      if (hasWin32DLLInstalled && !isWin32DLLFound) {
        messageBuilder.AppendLine("[win32] " + win32DLLPath);
      }
      // X64
      if (hasX64DLLInstalled && !isX64DLLFound) {
        messageBuilder.AppendLine("[x64] " + x64DLLPath);
      }
      messageBuilder.AppendLine();
      messageBuilder.AppendLine("Check your SCFF directory.");
      message = messageBuilder.ToString();
      return false;
    }

    // 起動OK
    message = string.Empty;
    return true;
  }
}
}   // namespace SCFF.Common
