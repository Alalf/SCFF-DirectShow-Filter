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

/// @file SCFF.Common/OptionsINIFile.cs
/// @copydoc SCFF::Common::OptionsINIFile

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/// プロファイル以外のアプリケーション設定のINIファイル入出力機能
/// @attention リフレクションは使わない
public static class OptionsINIFile {
  //===================================================================
  // 定数
  //===================================================================

  /// INIファイル名
  private const string OptionsFileName = "SCFF.Common.Options.ini";
  /// INIファイルの先頭に付加するヘッダー
  private const string OptionsHeader =
      "; SCFF-DirectShow-Filter Options Ver.0.1.7";

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @param options 保存するOptions
  /// @return 保存が成功したか
  public static bool Save(Options options) {
    try {
      var path = Utilities.GetDefaultFilePath + OptionsINIFile.OptionsFileName;
      using (var writer = new StreamWriter(path)) {
        writer.WriteLine(OptionsINIFile.OptionsHeader);
        for (int i = 0; i < 5; ++i) {
          writer.WriteLine("RecentProfile{0}={1}", i,
                           options.GetRecentProfile(i));
        }
        writer.WriteLine("FFmpegPath={0}", options.FFmpegPath);
        writer.WriteLine("FFmpegArguments={0}", options.FFmpegArguments);
        writer.WriteLine("TmpLeft={0}", options.TmpLeft);
        writer.WriteLine("TmpTop={0}", options.TmpTop);

        writer.WriteLine("TmpNormalWidth={0}", options.TmpNormalWidth);
        writer.WriteLine("TmpNormalHeight={0}", options.TmpNormalHeight);
        writer.WriteLine("TmpNormalLayoutWidth={0}", options.TmpNormalLayoutWidth);
        writer.WriteLine("TmpNormalLayoutHeight={0}", options.TmpNormalLayoutHeight);
        writer.WriteLine("TmpCompactWidth={0}", options.TmpCompactWidth);
        writer.WriteLine("TmpCompactHeight={0}", options.TmpCompactHeight);
        writer.WriteLine("TmpCompactLayoutWidth={0}", options.TmpCompactLayoutWidth);
        writer.WriteLine("TmpCompactLayoutHeight={0}", options.TmpCompactLayoutHeight);

        writer.WriteLine("TmpWindowState={0}", options.TmpWindowState);
        writer.WriteLine("AreaIsExpanded={0}", options.AreaIsExpanded);
        writer.WriteLine("OptionsIsExpanded={0}", options.OptionsIsExpanded);
        writer.WriteLine("ResizeMethodIsExpanded={0}",
                         options.ResizeMethodIsExpanded);
        writer.WriteLine("LayoutIsExpanded={0}", options.LayoutIsExpanded);
        writer.WriteLine("AutoApply={0}", options.AutoApply);
        writer.WriteLine("LayoutPreview={0}", options.LayoutPreview);
        writer.WriteLine("LayoutBorder={0}", options.LayoutBorder);
        writer.WriteLine("LayoutSnap={0}", options.LayoutSnap);
        writer.WriteLine("CompactView={0}", options.CompactView);
        writer.WriteLine("ForceAeroOn={0}", options.ForceAeroOn);
        writer.WriteLine("RestoreLastProfile={0}", options.RestoreLastProfile);
        return true;
      }
    } catch (Exception) {
      // 特に何も警告はしない
      Debug.WriteLine("Cannot save options", "OptionsINIFile.Save");
      return false;
    }
  }

  //===================================================================
  // ファイル入力
  //===================================================================

  /// ファイル入力
  ///
  /// アプリケーションの設定からUIに関連するものを読み込む
  /// 存在しない場合は勝手にデフォルト値が読み込まれる・・・はず
  /// @param[out] options ファイルから入力したデータの設定先
  /// @return 正常終了
  public static void Load(Options options) {
    // ファイル->ディクショナリ
    var path = Utilities.GetDefaultFilePath + OptionsINIFile.OptionsFileName;
    var labelToRawData = Utilities.LoadDictionaryFromINIFile(path);

    // ディクショナリ->Options
    OptionsINIFile.LoadFromDictionary(labelToRawData, options);
  }

  /// 辞書から読み込む
  private static void LoadFromDictionary(Dictionary<string,string> labelToRawData, Options options) {
    // 使いまわすので注意
    string stringValue;
    double doubleValue;
    bool boolValue;

    // Dictionaryを調べながら値を設定する
    string prefix = "RecentProfile";
    for (int i = 0; i < 5; ++i) {
      if (labelToRawData.TryGetValue(prefix + i, out stringValue)) {
        options.SetRecentProfile(i, stringValue);
      }
    }
    if (labelToRawData.TryGetValue("FFmpegPath", out stringValue)) {
      options.FFmpegPath = stringValue;
    }
    if (labelToRawData.TryGetValue("FFmpegArguments", out stringValue)) {
      options.FFmpegArguments = stringValue;
    }
    if (labelToRawData.TryGetDouble("TmpLeft", out doubleValue)) {
      options.TmpLeft = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpTop", out doubleValue)) {
      options.TmpTop = doubleValue;
    }

    if (labelToRawData.TryGetDouble("TmpNormalWidth", out doubleValue)) {
      options.TmpNormalWidth = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpNormalHeight", out doubleValue)) {
      options.TmpNormalHeight = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpNormalLayoutWidth", out doubleValue)) {
      options.TmpNormalLayoutWidth = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpNormalLayoutHeight", out doubleValue)) {
      options.TmpNormalLayoutHeight = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpCompactWidth", out doubleValue)) {
      options.TmpCompactWidth = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpCompactHeight", out doubleValue)) {
      options.TmpCompactHeight = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpCompactLayoutWidth", out doubleValue)) {
      options.TmpCompactLayoutWidth = doubleValue;
    }
    if (labelToRawData.TryGetDouble("TmpCompactLayoutHeight", out doubleValue)) {
      options.TmpCompactLayoutHeight = doubleValue;
    }

    WindowState windowState;
    if (labelToRawData.TryGetEnum<WindowState>("TmpWindowState", out windowState)) {
      options.TmpWindowState = windowState;
    }
    if (labelToRawData.TryGetBool("AreaIsExpanded", out boolValue)) {
      options.AreaIsExpanded = boolValue;
    }
    if (labelToRawData.TryGetBool("OptionsIsExpanded", out boolValue)) {
      options.OptionsIsExpanded = boolValue;
    }
    if (labelToRawData.TryGetBool("ResizeMethodIsExpanded", out boolValue)) {
      options.ResizeMethodIsExpanded = boolValue;
    }
    if (labelToRawData.TryGetBool("LayoutIsExpanded", out boolValue)) {
      options.LayoutIsExpanded = boolValue;
    }
    if (labelToRawData.TryGetBool("AutoApply", out boolValue)) {
      options.AutoApply = boolValue;
    }
    if (labelToRawData.TryGetBool("LayoutPreview", out boolValue)) {
      options.LayoutPreview = boolValue;
    }
    if (labelToRawData.TryGetBool("LayoutBorder", out boolValue)) {
      options.LayoutBorder = boolValue;
    }
    if (labelToRawData.TryGetBool("LayoutSnap", out boolValue)) {
      options.LayoutSnap = boolValue;
    }
    if (labelToRawData.TryGetBool("CompactView", out boolValue)) {
      options.CompactView = boolValue;
    }
    if (labelToRawData.TryGetBool("ForceAeroOn", out boolValue)) {
      options.ForceAeroOn = boolValue;
    }
    if (labelToRawData.TryGetBool("RestoreLastProfile", out boolValue)) {
      options.RestoreLastProfile = boolValue;
    }
  }
}
}   // namespace SCFF.Common
