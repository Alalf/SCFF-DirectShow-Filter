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

/// @file SCFF.Common/Utilities.cs
/// @copydoc SCFF::Common::Utilities

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using SCFF.Common.Ext;

/// SCFF.Commonモジュール共通で利用する機能
public static class Utilities {
  //===================================================================
  // Windowの有効確認
  //===================================================================

  /// Windowが有効か
  public static bool IsWindowValid(UIntPtr window) {
    return window != null && User32.IsWindow(window) && !User32.IsIconic(window);
  }

  //===================================================================
  // プロセスの生存確認
  //===================================================================

  /// プロセスの生存確認
  /// @param processID プロセスID
  /// @return 生存しているか
  public static bool IsProcessAlive(UInt32 processID) {
    try {
      /// @warning DWORD->int変換！オーバーフローの可能性あり
      Process.GetProcessById((int)processID);
      return true;
    } catch {
      return false;
    }
  }

  //===================================================================
  // ファイル入出力用の機能
  //===================================================================

  /// アプリケーションアセンブリのあるディレクトリ
  public static string ApplicationDirectory {
    get {
      // 末尾が\\で終わること
      var applicationPath = Assembly.GetEntryAssembly().Location;
      return Path.GetDirectoryName(applicationPath) + @"\";
    }
  }

  //===================================================================
  // ファイルパスの短縮
  //===================================================================

  /// 短縮した後の長さ
  private static int CalcShortPathLength(string pathRoot,
      List<string> directoryList, bool shortened, string fileName) {
    var length = pathRoot.Length;
    if (shortened) length += "...\\".Length;
    foreach (var i in directoryList) {
      if (i == string.Empty) continue;
      length += i.Length + 1;   // '\\'.Length = 1
    }
    length += fileName.Length;
    return length;
  }

  /// 短縮パスを文字列に直す
  private static string BuildShortPath(string pathRoot,
      List<string> directoryList, bool shortened, string fileName) {
    var builder = new StringBuilder();
    builder.Append(pathRoot);
    if (shortened) builder.Append("...\\");
    foreach (var i in directoryList) {
      if (i == string.Empty) continue;
      builder.Append(i);
      builder.Append('\\');
    }
    builder.Append(fileName);
    return builder.ToString();
  }

  //-------------------------------------------------------------------

  /// 最大文字数(目安)を指定して短縮パスを得る
  public static string GetShortPath(string path, int length) {
    var fileName = Path.GetFileName(path);
    var pathRoot = Path.GetPathRoot(path);
    var directoryPath = Path.GetDirectoryName(path).Substring(pathRoot.Length);
    var directoryList = new List<string>(directoryPath.Split('\\'));

    // 短縮が必要ない場合はそのまま文字列にして返す
    var currentLength = Utilities.CalcShortPathLength(pathRoot, directoryList, false, fileName);
    if (currentLength <= length) {
      // 正常な結果なので文字列に戻して返す
      return Utilities.BuildShortPath(pathRoot, directoryList, false, fileName);
    }

    // 短縮開始
    do {
      directoryList.RemoveAt(0);
      var shortenedLength = Utilities.CalcShortPathLength(pathRoot, directoryList, true, fileName);
      if (shortenedLength <= length) {
        return Utilities.BuildShortPath(pathRoot, directoryList, true, fileName);
      }
    } while (directoryList.Count > 0);

    return Utilities.BuildShortPath(pathRoot, directoryList, true, fileName);
  }
}
}   // namespace SCFF.Common
