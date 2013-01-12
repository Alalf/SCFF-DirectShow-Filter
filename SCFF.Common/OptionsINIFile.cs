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
public static class OptionsINIFile {
  //===================================================================
  // 定数
  //===================================================================

  /// INIファイル名
  private const string OptionsFilePath = "SCFF.Common.Options.ini";
  /// INIファイルの先頭に付加するヘッダー
  private const string OptionsHeader =
      "; SCFF-DirectShow-Filter Options Ver.0.1.7";

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @todo(me) パスを指定して保存を行いたい(現在だとワーキングディレクトリに保存されてしまう)
  public static bool Save(Options options) {
    try {
      using (var writer = new StreamWriter(OptionsINIFile.OptionsFilePath)) {
        writer.WriteLine(OptionsINIFile.OptionsHeader);
        for (int i = 0; i < 5; ++i) {
          writer.WriteLine("RecentProfile{0}={1}", i,
                           options.GetRecentProfile(i));
        }
        writer.WriteLine("FFmpegPath={0}", options.FFmpegPath);
        writer.WriteLine("FFmpegArguments={0}", options.FFmpegArguments);
        writer.WriteLine("TmpMainWindowLeft={0}", options.TmpMainWindowLeft);
        writer.WriteLine("TmpMainWindowTop={0}", options.TmpMainWindowTop);
        writer.WriteLine("TmpMainWindowWidth={0}", options.TmpMainWindowWidth);
        writer.WriteLine("TmpMainWindowHeight={0}",
                         options.TmpMainWindowHeight);
        writer.WriteLine("TmpMainWindowState={0}", options.TmpMainWindowState);
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
  public static bool Load(Options options) {
    // ファイル読み込み
    var lines = new List<string>();
    try {
      using (var reader = new StreamReader(OptionsINIFile.OptionsFilePath)) {
        while (!reader.EndOfStream) {
          lines.Add(reader.ReadLine());
        }
      }
    } catch (Exception) {
      Debug.WriteLine("Cannot read options", "OptionsINIFile.Load");
      return false;
    }

    // 読み込んだデータを*=*でSplit
    var separator = new char[1] {'='};
    var labelToRawData = new Dictionary<string, string>();
    foreach (var line in lines) {
      if (line.Length == 0 || line[0] == ';' || line[0] == '[') {
        // 空行、コメント行、セクション記述行は読み飛ばす
        continue;
      }

      var splitIndex = line.IndexOf('=');
      if (splitIndex == -1) {
        // '='が見つからなければ読みとばす
        continue;
      } else if (splitIndex == line.Length - 1) {
        // 空文字列なので読み飛ばす
        continue;
      }
      var label = line.Substring(0, splitIndex).Trim();
      var rawData = line.Substring(splitIndex+1);
      labelToRawData.Add(label, rawData);
    }

    // ディクショナリを参考にしながらデータを設定
    OptionsINIFile.LoadFromDictionary(labelToRawData, options);

    return true;
  }

  /// 辞書から読み込む
  private static void LoadFromDictionary(
      Dictionary<string, string> labelToRawData, Options options) {
    // 使いまわすので注意
    string rawData;

    // Dictionaryを調べながら値を設定する
    string prefix = "RecentProfile";
    for (int i = 0; i < 5; ++i) {
      if (labelToRawData.TryGetValue(prefix + i, out rawData)) {
        options.SetRecentProfile(i, rawData);
      }
    }
    if (labelToRawData.TryGetValue("FFmpegPath", out rawData)) {
      options.FFmpegPath = rawData;
    }
    if (labelToRawData.TryGetValue("FFmpegArguments", out rawData)) {
      options.FFmpegArguments = rawData;
    }
    if (labelToRawData.TryGetValue("TmpMainWindowLeft", out rawData)) {
      double parsedData;
      if (double.TryParse(rawData, out parsedData)) {
        options.TmpMainWindowLeft = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("TmpMainWindowTop", out rawData)) {
      double parsedData;
      if (double.TryParse(rawData, out parsedData)) {
        options.TmpMainWindowTop = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("TmpMainWindowWidth", out rawData)) {
      double parsedData;
      if (double.TryParse(rawData, out parsedData)) {
        options.TmpMainWindowWidth = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("TmpMainWindowHeight", out rawData)) {
      double parsedData;
      if (double.TryParse(rawData, out parsedData)) {
        options.TmpMainWindowHeight = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("TmpMainWindowState", out rawData)) {
      WindowState parsedData;
      if (Enum.TryParse<WindowState>(rawData, out parsedData)) {
        options.TmpMainWindowState = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("AreaIsExpanded", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.AreaIsExpanded = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("OptionsIsExpanded", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.OptionsIsExpanded = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("ResizeMethodIsExpanded", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.ResizeMethodIsExpanded = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("LayoutIsExpanded", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.LayoutIsExpanded = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("AutoApply", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.AutoApply = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("LayoutPreview", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.LayoutPreview = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("LayoutBorder", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.LayoutBorder = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("LayoutSnap", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.LayoutSnap = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("CompactView", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.CompactView = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("ForceAeroOn", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.ForceAeroOn = parsedData;
      }
    }
    if (labelToRawData.TryGetValue("RestoreLastProfile", out rawData)) {
      bool parsedData;
      if (bool.TryParse(rawData, out parsedData)) {
        options.RestoreLastProfile = parsedData;
      }
    }
  }
}
}   // namespace SCFF.Common
