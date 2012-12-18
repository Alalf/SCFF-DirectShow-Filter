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
using System.Text;

/// プロファイル以外のアプリケーションの設定
public class Options {

  //===================================================================
  // 定数
  //===================================================================

  private const string OptionsFilePath = "SCFF.GUI.ini";
  private const string OptionsHeader = "; SCFF-DirectShow-Filter Options Ver.0.1.7";

  // リフレクションの使用回避
  private readonly Dictionary<Key, string> keyToLabel = new Dictionary<Key, string>() {
    {Key.RecentProfile1, "RecentProfile1"},
    {Key.RecentProfile2, "RecentProfile2"},
    {Key.RecentProfile3, "RecentProfile3"},
    {Key.RecentProfile4, "RecentProfile4"},
    {Key.RecentProfile5, "RecentProfile5"},
    {Key.FFmpegPath, "FFmpegPath"},
    {Key.FFmpegArguments, "FFmpegArguments"},
    {Key.MainWindowLeft, "MainWindowLeft"},
    {Key.MainWindowTop, "MainWindowTop"},
    {Key.MainWindowWidth, "MainWindowWidth"},
    {Key.MainWindowHeight, "MainWindowHeight"},
    {Key.MainWindowState, "MainWindowState"},
    {Key.AreaExpanderIsExpanded, "AreaExpanderIsExpanded"},
    {Key.OptionsExpanderIsExpanded, "OptionsExpanderIsExpanded"},
    {Key.ResizeMethodExpanderIsExpanded, "ResizeMethodExpanderIsExpanded"},
    {Key.LayoutExpanderIsExpanded, "LayoutExpanderIsExpanded"},
    {Key.AutoApply, "AutoApply"},
    {Key.LayoutPreview, "LayoutPreview"},
    {Key.LayoutBorder, "LayoutBorder"},
    {Key.LayoutSnap, "LayoutSnap"},
    {Key.CompactView, "CompactView"},
    {Key.ForceAeroOn, "ForceAeroOn"},
    {Key.RestoreLastProfile, "RestoreLastProfile"},
  };

  // そもそも大した数じゃないんだし手書きでもエンバグはしないだろう
  public enum Key {
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
    MainWindowLeft,
    MainWindowTop,
    MainWindowWidth,
    MainWindowHeight,
    MainWindowState,

    // MainWindow Expanders
    AreaExpanderIsExpanded,
    OptionsExpanderIsExpanded,
    ResizeMethodExpanderIsExpanded,
    LayoutExpanderIsExpanded,

    // SCFF Options
    AutoApply,
    LayoutPreview,
    LayoutBorder,
    LayoutSnap,

    // SCFF Menu Options
    CompactView,
    ForceAeroOn,
    RestoreLastProfile
  }

  public enum WindowState {
    Normal,
    Minimized,
    Maximized
  }

  //===================================================================
  // ファイル入出力
  //===================================================================

  /// ファイル出力
  public bool Save() {
    using (var writer = new StreamWriter(OptionsFilePath)) {
      try {
        writer.WriteLine(OptionsHeader);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile1] + "=" + this.recentProfiles[4]);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile2] + "=" + this.recentProfiles[3]);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile3] + "=" + this.recentProfiles[2]);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile4] + "=" + this.recentProfiles[1]);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile5] + "=" + this.recentProfiles[0]);
        writer.WriteLine(this.keyToLabel[Key.FFmpegPath] + "=" + this.ffmpegPath);
        writer.WriteLine(this.keyToLabel[Key.FFmpegArguments] + "=" + this.ffmpegArguments);
        writer.WriteLine(this.keyToLabel[Key.MainWindowLeft] + "=" + this.mainWindowLeft);
        writer.WriteLine(this.keyToLabel[Key.MainWindowTop] + "=" + this.mainWindowTop);
        writer.WriteLine(this.keyToLabel[Key.MainWindowWidth] + "=" + this.mainWindowWidth);
        writer.WriteLine(this.keyToLabel[Key.MainWindowHeight] + "=" + this.mainWindowHeight);
        writer.WriteLine(this.keyToLabel[Key.MainWindowState] + "=" + (int)this.mainWindowState);
        writer.WriteLine(this.keyToLabel[Key.AreaExpanderIsExpanded] + "=" + this.areaExpanderIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.OptionsExpanderIsExpanded] + "=" + this.optionsExpanderIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.ResizeMethodExpanderIsExpanded] + "=" + this.resizeMethodExpanderIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.LayoutExpanderIsExpanded] + "=" + this.layoutExpanderIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.AutoApply] + "=" + this.autoApply);
        writer.WriteLine(this.keyToLabel[Key.LayoutPreview] + "=" + this.layoutPreview);
        writer.WriteLine(this.keyToLabel[Key.LayoutBorder] + "=" + this.layoutBorder);
        writer.WriteLine(this.keyToLabel[Key.LayoutSnap] + "=" + this.layoutSnap);
        writer.WriteLine(this.keyToLabel[Key.CompactView] + "=" + this.compactView);
        writer.WriteLine(this.keyToLabel[Key.ForceAeroOn] + "=" + this.forceAeroOn);
        writer.WriteLine(this.keyToLabel[Key.RestoreLastProfile] + "=" + this.restoreLastProfile);
        return true;
      } catch {
        // 特に何も警告はしない
        Debug.Fail("Failed while writing options");
        return false;
      }
    }
  }

  /// ファイル入力
  public bool Load() {
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

      var labelAndRawData = line.Split(separator, 1);
      if (labelAndRawData.Length == 2) {
        // *=*になっていなければ処理はしない
        labelToRawData.Add(labelAndRawData[0], labelAndRawData[1]);
      }
    }

    // keyToLabelから値を設定していく
    foreach (var keyAndLabel in keyToLabel) {
      if (labelToRawData.ContainsKey(keyAndLabel.Value)) {
        // すべてO(1)
        var key = keyAndLabel.Key;
        var rawData = labelToRawData[keyAndLabel.Value];
        this.ParseRawData(key, rawData);
      }
    }

    return true;
  }

  /// ファイル入力内容からメンバ変数に値を設定する
  private void ParseRawData(Key key, string rawData) {
    // keyからメンバ変数に値を代入していく
    switch (key) {
      case Key.RecentProfile1: {
        this.recentProfiles[4] = rawData;
        break;
      }
      case Key.RecentProfile2: {
        this.recentProfiles[3] = rawData;
        break;
      }
      case Key.RecentProfile3: {
        this.recentProfiles[2] = rawData;
        break;
      }
      case Key.RecentProfile4: {
        this.recentProfiles[1] = rawData;
        break;
      }
      case Key.RecentProfile5: {
        this.recentProfiles[0] = rawData;
        break;
      }
      case Key.FFmpegPath: {
        this.ffmpegPath = rawData;
        break;
      }
      case Key.FFmpegArguments: {
        this.ffmpegArguments = rawData;
        break;
      }
      case Key.MainWindowLeft: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.mainWindowLeft = parsedData; 
        }
        break;
      }
      case Key.MainWindowTop: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.mainWindowTop = parsedData;
        }
        break;
      }
      case Key.MainWindowWidth: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.mainWindowWidth = parsedData;
        }
        break;
      }
      case Key.MainWindowState: {
        WindowState parsedData;
        if (Enum.TryParse<WindowState>(rawData, out parsedData)) {
          this.mainWindowState = parsedData;
        }
        break;
      }
      case Key.AreaExpanderIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.areaExpanderIsExpanded = parsedData;
        }
        break;
      }
      case Key.OptionsExpanderIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.optionsExpanderIsExpanded = parsedData;
        }
        break;
      }
      case Key.ResizeMethodExpanderIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.resizeMethodExpanderIsExpanded = parsedData;
        }
        break;
      }
      case Key.LayoutExpanderIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.layoutExpanderIsExpanded = parsedData; 
        }
        break;
      }
      case Key.AutoApply: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.autoApply = parsedData;
        }
        break;
      }
      case Key.LayoutPreview: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.layoutPreview = parsedData;
        }
        break;
      }
      case Key.LayoutBorder: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.layoutBorder = parsedData;
        }
        break;
      }
      case Key.LayoutSnap: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.layoutSnap = parsedData;
        }
        break;
      }
      case Key.CompactView: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.compactView = parsedData;
        }
        break;
      }
      case Key.ForceAeroOn: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.forceAeroOn = parsedData;
        }
        break;
      }
      case Key.RestoreLastProfile: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.restoreLastProfile = parsedData;
        }
        break;
      }
    }
  }

  //===================================================================
  // アクセッサ
  // MVVM/DataBindingを使いたくなるのを回避するため、プロパティは使わない
  //===================================================================

  public void AddRecentProfile(string profile) {
    // Queueに再構成してから配列に書き戻す
    var queue = new Queue<string>(this.recentProfiles);
    if (!queue.Contains(profile)) {
      queue.Dequeue();
      queue.Enqueue(profile);
      Debug.Assert(queue.Count == 5); 
    }
    queue.CopyTo(this.recentProfiles, 0);
  }
  public string RecentProfile1() {
    return this.recentProfiles[4];
  }
  public string RecentProfile2() {
    return this.recentProfiles[3];
  }
  public string RecentProfile3() {
    return this.recentProfiles[2];
  }
  public string RecentProfile4() {
    return this.recentProfiles[1];
  }
  public string RecentProfile5() {
    return this.recentProfiles[0];
  }
  
  public void SetFFmpegOptions(string ffmpegPath, string ffmpegArguments) {
    this.ffmpegPath = ffmpegPath;
    this.ffmpegArguments = ffmpegArguments;
  }
  public string FFmpegPath() {
    return this.ffmpegPath;
  }
  public string FFmpegArguments() {
    return this.ffmpegArguments;
  }

  public void SetMainWindowOptions(double mainWindowLeft,
      double mainWindowTop, double mainWindowWidth, double mainWindowHeight,
      WindowState mainWindowState) {
    this.mainWindowLeft = mainWindowLeft;
    this.mainWindowTop = mainWindowTop;
    this.mainWindowWidth = mainWindowWidth;
    this.mainWindowHeight = mainWindowHeight;
    this.mainWindowState = mainWindowState;
  }
  public double MainWindowLeft() {
    return this.mainWindowLeft;
  }
  public double MainWindowTop() {
    return this.mainWindowTop;
  }
  public double MainWindowWidth() {
    return this.mainWindowWidth;
  }
  public double MainWindowHeight() {
    return this.mainWindowHeight;
  }
  public WindowState MainWindowState() {
    return this.mainWindowState;
  }

  public void SetMainWindowExpanders(bool areaExpanderIsExpanded,
      bool optionsExpanderIsExpanded, bool resizeMethodExpanderIsExpanded,
      bool layoutExpanderIsExpanded) {
    this.areaExpanderIsExpanded = areaExpanderIsExpanded;
    this.optionsExpanderIsExpanded = optionsExpanderIsExpanded;
    this.resizeMethodExpanderIsExpanded = resizeMethodExpanderIsExpanded;
    this.layoutExpanderIsExpanded = layoutExpanderIsExpanded;
  }
  public bool AreaExpanderIsExpanded() {
    return this.areaExpanderIsExpanded;
  }
  public bool OptionsExpanderIsExpanded() {
    return this.optionsExpanderIsExpanded;
  }
  public bool ResizeMethodExpanderIsExpanded() {
    return this.resizeMethodExpanderIsExpanded;
  }
  public bool LayoutExpanderIsExpanded() {
    return this.layoutExpanderIsExpanded;
  }

  public void SetSCFFOptions(bool autoApply, bool layoutPreview,
      bool layoutBorder, bool layoutSnap) {
    this.autoApply = autoApply;
    this.layoutPreview = layoutPreview;
    this.layoutBorder = layoutBorder;
    this.layoutSnap = layoutSnap;
  }
  public bool AutoApply() {
    return this.autoApply;
  }
  public bool LayoutPreview() {
    return this.layoutPreview;
  }
  public bool LayoutBorder() {
    return this.layoutBorder;
  }

  public void SetSCFFMenuOptions(bool compactView, bool forceAeroOn,
      bool restoreLastProfile) {
    this.compactView = compactView;
    this.forceAeroOn = forceAeroOn;
    this.restoreLastProfile = restoreLastProfile;
  }
  public bool CompactView() {
    return this.compactView;
  }
  public bool ForceAeroOn() {
    return this.forceAeroOn;
  }
  public bool RestoreLastProfile() {
    return this.restoreLastProfile;
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  /// @caution 先頭から古く、末尾が一番新しい
  private string[] recentProfiles = new string[5] {
      string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};
  private string ffmpegPath = string.Empty;
  private string ffmpegArguments = string.Empty;
  private double mainWindowLeft = 0.0;
  private double mainWindowTop = 0.0;
  private double mainWindowWidth = Defaults.MainWindowWidth;
  private double mainWindowHeight = Defaults.MainWindowHeight;
  private WindowState mainWindowState = WindowState.Normal;
  private bool areaExpanderIsExpanded = true;
  private bool optionsExpanderIsExpanded = true;
  private bool resizeMethodExpanderIsExpanded = true;
  private bool layoutExpanderIsExpanded = true;
  private bool autoApply = true;
  private bool layoutPreview = true;
  private bool layoutBorder = true;
  private bool layoutSnap = true;
  private bool compactView = false;
  private bool forceAeroOn = false;
  private bool restoreLastProfile = true;
}
}
