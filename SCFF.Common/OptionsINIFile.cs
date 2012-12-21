// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class OptionsINIFile {

  //===================================================================
  // 定数
  //===================================================================

  private const string OptionsFilePath = "SCFF.Common.Options.ini";
  private const string OptionsHeader = "; SCFF-DirectShow-Filter Options Ver.0.1.7";

  /// ファイル入出力に用いるキー
  private enum Key {
    // Recent Profiles
    RecentProfile1,
    RecentProfile2,
    RecentProfile3,
    RecentProfile4,
    RecentProfile5,

    // FFmpeg Options
    FFmpegPath,
    FFmpegArguments,

    // MainWindow
    TmpMainWindowLeft,
    TmpMainWindowTop,
    TmpMainWindowWidth,
    TmpMainWindowHeight,
    TmpMainWindowState,

    // MainWindow Expanders
    TmpAreaIsExpanded,
    TmpOptionsIsExpanded,
    TmpResizeMethodIsExpanded,
    TmpLayoutIsExpanded,

    // SCFF Options
    AutoApply,
    LayoutPreview,
    LayoutBorder,
    LayoutSnap,

    // SCFF Menu Options
    TmpCompactView,
    ForceAeroOn,
    TmpRestoreLastProfile
  }

  /// Key(Enum)->String変換用の辞書。リフレクション利用回避のためわざわざ作成した。
  private static readonly Dictionary<Key, string> keyToLabel = new Dictionary<Key, string>() {
    {Key.RecentProfile1, "RecentProfile1"},
    {Key.RecentProfile2, "RecentProfile2"},
    {Key.RecentProfile3, "RecentProfile3"},
    {Key.RecentProfile4, "RecentProfile4"},
    {Key.RecentProfile5, "RecentProfile5"},
    {Key.FFmpegPath, "FFmpegPath"},
    {Key.FFmpegArguments, "FFmpegArguments"},
    {Key.TmpMainWindowLeft, "TmpMainWindowLeft"},
    {Key.TmpMainWindowTop, "TmpMainWindowTop"},
    {Key.TmpMainWindowWidth, "TmpMainWindowWidth"},
    {Key.TmpMainWindowHeight, "TmpMainWindowHeight"},
    {Key.TmpMainWindowState, "TmpMainWindowState"},
    {Key.TmpAreaIsExpanded, "TmpAreaIsExpanded"},
    {Key.TmpOptionsIsExpanded, "TmpOptionsIsExpanded"},
    {Key.TmpResizeMethodIsExpanded, "TmpResizeMethodIsExpanded"},
    {Key.TmpLayoutIsExpanded, "TmpLayoutIsExpanded"},
    {Key.AutoApply, "AutoApply"},
    {Key.LayoutPreview, "LayoutPreview"},
    {Key.LayoutBorder, "LayoutBorder"},
    {Key.LayoutSnap, "LayoutSnap"},
    {Key.TmpCompactView, "TmpCompactView"},
    {Key.ForceAeroOn, "ForceAeroOn"},
    {Key.TmpRestoreLastProfile, "TmpRestoreLastProfile"},
  };

  //===================================================================
  // internalメソッド
  //===================================================================

  /// ファイル出力
  public static bool Save(Options options) {
    using (var writer = new StreamWriter(OptionsFilePath)) {
      try {
        writer.WriteLine(OptionsHeader);
        writer.WriteLine(keyToLabel[Key.RecentProfile1] + "=" + options.GetRecentProfile(0));
        writer.WriteLine(keyToLabel[Key.RecentProfile2] + "=" + options.GetRecentProfile(1));
        writer.WriteLine(keyToLabel[Key.RecentProfile3] + "=" + options.GetRecentProfile(2));
        writer.WriteLine(keyToLabel[Key.RecentProfile4] + "=" + options.GetRecentProfile(3));
        writer.WriteLine(keyToLabel[Key.RecentProfile5] + "=" + options.GetRecentProfile(4));
        writer.WriteLine(keyToLabel[Key.FFmpegPath] + "=" + options.FFmpegPath);
        writer.WriteLine(keyToLabel[Key.FFmpegArguments] + "=" + options.FFmpegArguments);
        writer.WriteLine(keyToLabel[Key.TmpMainWindowLeft] + "=" + options.TmpMainWindowLeft);
        writer.WriteLine(keyToLabel[Key.TmpMainWindowTop] + "=" + options.TmpMainWindowTop);
        writer.WriteLine(keyToLabel[Key.TmpMainWindowWidth] + "=" + options.TmpMainWindowWidth);
        writer.WriteLine(keyToLabel[Key.TmpMainWindowHeight] + "=" + options.TmpMainWindowHeight);
        writer.WriteLine(keyToLabel[Key.TmpMainWindowState] + "=" + (int)options.TmpMainWindowState);
        writer.WriteLine(keyToLabel[Key.TmpAreaIsExpanded] + "=" + options.TmpAreaIsExpanded);
        writer.WriteLine(keyToLabel[Key.TmpOptionsIsExpanded] + "=" + options.TmpOptionsIsExpanded);
        writer.WriteLine(keyToLabel[Key.TmpResizeMethodIsExpanded] + "=" + options.TmpResizeMethodIsExpanded);
        writer.WriteLine(keyToLabel[Key.TmpLayoutIsExpanded] + "=" + options.TmpLayoutIsExpanded);
        writer.WriteLine(keyToLabel[Key.AutoApply] + "=" + options.AutoApply);
        writer.WriteLine(keyToLabel[Key.LayoutPreview] + "=" + options.LayoutPreview);
        writer.WriteLine(keyToLabel[Key.LayoutBorder] + "=" + options.LayoutBorder);
        writer.WriteLine(keyToLabel[Key.LayoutSnap] + "=" + options.LayoutSnap);
        writer.WriteLine(keyToLabel[Key.TmpCompactView] + "=" + options.TmpCompactView);
        writer.WriteLine(keyToLabel[Key.ForceAeroOn] + "=" + options.ForceAeroOn);
        writer.WriteLine(keyToLabel[Key.TmpRestoreLastProfile] + "=" + options.TmpRestoreLastProfile);
        return true;
      } catch (Exception ex) {
        // 特に何も警告はしない
        Debug.Fail(ex.Message);
        return false;
      }
    }
  }

  /// ファイル入力
  /// アプリケーションの設定からUIに関連するものを読み込む
  /// 存在しない場合は勝手にデフォルト値が読み込まれる・・・はず
  public static bool Load(Options options) {
    // ファイル読み込み
    var lines = new List<string>();
    try {
      using (var reader = new StreamReader(OptionsFilePath)) {
        while (!reader.EndOfStream) {
          lines.Add(reader.ReadLine());
        }
      }
    } catch (FileNotFoundException ex) {
      Debug.Fail(ex.ToString());
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
      var label = line.Substring(0, splitIndex);
      var rawData = line.Substring(splitIndex+1);
      labelToRawData.Add(label, rawData);
    }

    // keyToLabelから値を設定していく
    foreach (var keyAndLabel in keyToLabel) {
      if (labelToRawData.ContainsKey(keyAndLabel.Value)) {
        // すべてO(1)
        var key = keyAndLabel.Key;
        var rawData = labelToRawData[keyAndLabel.Value];
        ParseRawData(options, key, rawData);
      }
    }

    return true;
  }

  /// ファイル入力内容からメンバ変数に値を設定する
  private static void ParseRawData(Options options, Key key, string rawData) {
    // keyからメンバ変数に値を代入していく
    switch (key) {
      case Key.RecentProfile1: {
        options.SetRecentProfile(0, rawData);
        break;
      }
      case Key.RecentProfile2: {
        options.SetRecentProfile(1, rawData);
        break;
      }
      case Key.RecentProfile3: {
        options.SetRecentProfile(2, rawData);
        break;
      }
      case Key.RecentProfile4: {
        options.SetRecentProfile(3, rawData);
        break;
      }
      case Key.RecentProfile5: {
        options.SetRecentProfile(4, rawData);
        break;
      }
      case Key.FFmpegPath: {
        options.FFmpegPath = rawData;
        break;
      }
      case Key.FFmpegArguments: {
        options.FFmpegArguments = rawData;
        break;
      }
      case Key.TmpMainWindowLeft: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          options.TmpMainWindowLeft = parsedData; 
        }
        break;
      }
      case Key.TmpMainWindowTop: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          options.TmpMainWindowTop = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowWidth: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          options.TmpMainWindowWidth = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowHeight: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          options.TmpMainWindowHeight = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowState: {
        Options.WindowState parsedData;
        if (Enum.TryParse<Options.WindowState>(rawData, out parsedData)) {
          options.TmpMainWindowState = parsedData;
        }
        break;
      }
      case Key.TmpAreaIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpAreaIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpOptionsIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpOptionsIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpResizeMethodIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpResizeMethodIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpLayoutIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpLayoutIsExpanded = parsedData; 
        }
        break;
      }
      case Key.AutoApply: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.AutoApply = parsedData;
        }
        break;
      }
      case Key.LayoutPreview: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.LayoutPreview = parsedData;
        }
        break;
      }
      case Key.LayoutBorder: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.LayoutBorder = parsedData;
        }
        break;
      }
      case Key.LayoutSnap: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.LayoutSnap = parsedData;
        }
        break;
      }
      case Key.TmpCompactView: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpCompactView = parsedData;
        }
        break;
      }
      case Key.ForceAeroOn: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.ForceAeroOn = parsedData;
        }
        break;
      }
      case Key.TmpRestoreLastProfile: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          options.TmpRestoreLastProfile = parsedData;
        }
        break;
      }
    }
  }
}
}
