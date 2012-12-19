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
/// アプリケーションの起動中にプロパティが最新の状態になっている必要の無いものには
/// Tmpという接頭辞をつけている。プログラム内部でそれらのプロパティを使うことは
/// Loaded/Closingイベントハンドラ以外では非推奨である。
public class Options {

  //===================================================================
  // 定数
  //===================================================================

  private const string OptionsFilePath = "SCFF.Common.Options.ini";
  private const string OptionsHeader = "; SCFF-DirectShow-Filter Options Ver.0.1.7";

  private const int RecentProfilesLength = 5;

  /// ファイル入出力に用いるキー
  public enum Key {
    // Recent Profiles
    RecentProfile1FilePath,
    RecentProfile1FileName,
    RecentProfile1Timestamp,
    RecentProfile2FilePath,
    RecentProfile2FileName,
    RecentProfile2Timestamp,
    RecentProfile3FilePath,
    RecentProfile3FileName,
    RecentProfile3Timestamp,
    RecentProfile4FilePath,
    RecentProfile4FileName,
    RecentProfile4Timestamp,
    RecentProfile5FilePath,
    RecentProfile5FileName,
    RecentProfile5Timestamp,

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

  /// Options用WindowState。System.Windows.WindowStateと相互に変換する。
  public enum WindowState {
    Normal,
    Minimized,
    Maximized
  }

  /// Key(Enum)->String変換用の辞書。リフレクション利用回避のためわざわざ作成した。
  private readonly Dictionary<Key, string> keyToLabel = new Dictionary<Key, string>() {
    {Key.RecentProfile1FilePath, "RecentProfile1FilePath"},
    {Key.RecentProfile1FileName, "RecentProfile1FileName"},
    {Key.RecentProfile1Timestamp, "RecentProfile1Timestamp"},
    {Key.RecentProfile2FilePath, "RecentProfile2FilePath"},
    {Key.RecentProfile2FileName, "RecentProfile2FileName"},
    {Key.RecentProfile2Timestamp, "RecentProfile2Timestamp"},
    {Key.RecentProfile3FilePath, "RecentProfile3FilePath"},
    {Key.RecentProfile3FileName, "RecentProfile3FileName"},
    {Key.RecentProfile3Timestamp, "RecentProfile3Timestamp"},
    {Key.RecentProfile4FilePath, "RecentProfile4FilePath"},
    {Key.RecentProfile4FileName, "RecentProfile4FileName"},
    {Key.RecentProfile4Timestamp, "RecentProfile4Timestamp"},
    {Key.RecentProfile5FilePath, "RecentProfile5FilePath"},
    {Key.RecentProfile5FileName, "RecentProfile5FileName"},
    {Key.RecentProfile5Timestamp, "RecentProfile5Timestamp"},
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
  // ファイル入出力
  //===================================================================

  /// ファイル出力
  public bool Save() {
    using (var writer = new StreamWriter(OptionsFilePath)) {
      try {
        writer.WriteLine(OptionsHeader);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile1FilePath] + "=" + this.reverseRecentProfiles[4].FilePath);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile1FileName] + "=" + this.reverseRecentProfiles[4].FileName);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile1Timestamp] + "=" + this.reverseRecentProfiles[4].Timestamp);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile2FilePath] + "=" + this.reverseRecentProfiles[3].FilePath);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile2FileName] + "=" + this.reverseRecentProfiles[3].FileName);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile2Timestamp] + "=" + this.reverseRecentProfiles[3].Timestamp);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile3FilePath] + "=" + this.reverseRecentProfiles[2].FilePath);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile3FileName] + "=" + this.reverseRecentProfiles[2].FileName);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile3Timestamp] + "=" + this.reverseRecentProfiles[2].Timestamp);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile4FilePath] + "=" + this.reverseRecentProfiles[1].FilePath);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile4FileName] + "=" + this.reverseRecentProfiles[1].FileName);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile4Timestamp] + "=" + this.reverseRecentProfiles[1].Timestamp);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile5FilePath] + "=" + this.reverseRecentProfiles[0].FilePath);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile5FileName] + "=" + this.reverseRecentProfiles[0].FileName);
        writer.WriteLine(this.keyToLabel[Key.RecentProfile5Timestamp] + "=" + this.reverseRecentProfiles[0].Timestamp);
        writer.WriteLine(this.keyToLabel[Key.FFmpegPath] + "=" + this.ffmpegPath);
        writer.WriteLine(this.keyToLabel[Key.FFmpegArguments] + "=" + this.ffmpegArguments);
        writer.WriteLine(this.keyToLabel[Key.TmpMainWindowLeft] + "=" + this.tmpMainWindowLeft);
        writer.WriteLine(this.keyToLabel[Key.TmpMainWindowTop] + "=" + this.tmpMainWindowTop);
        writer.WriteLine(this.keyToLabel[Key.TmpMainWindowWidth] + "=" + this.tmpMainWindowWidth);
        writer.WriteLine(this.keyToLabel[Key.TmpMainWindowHeight] + "=" + this.tmpMainWindowHeight);
        writer.WriteLine(this.keyToLabel[Key.TmpMainWindowState] + "=" + (int)this.tmpMainWindowState);
        writer.WriteLine(this.keyToLabel[Key.TmpAreaIsExpanded] + "=" + this.tmpAreaIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.TmpOptionsIsExpanded] + "=" + this.tmpOptionsIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.TmpResizeMethodIsExpanded] + "=" + this.tmpResizeMethodIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.TmpLayoutIsExpanded] + "=" + this.tmpLayoutIsExpanded);
        writer.WriteLine(this.keyToLabel[Key.AutoApply] + "=" + this.autoApply);
        writer.WriteLine(this.keyToLabel[Key.LayoutPreview] + "=" + this.layoutPreview);
        writer.WriteLine(this.keyToLabel[Key.LayoutBorder] + "=" + this.layoutBorder);
        writer.WriteLine(this.keyToLabel[Key.LayoutSnap] + "=" + this.layoutSnap);
        writer.WriteLine(this.keyToLabel[Key.TmpCompactView] + "=" + this.tmpCompactView);
        writer.WriteLine(this.keyToLabel[Key.ForceAeroOn] + "=" + this.forceAeroOn);
        writer.WriteLine(this.keyToLabel[Key.TmpRestoreLastProfile] + "=" + this.tmpRestoreLastProfile);
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
        this.ParseRawData(key, rawData);
      }
    }

    return true;
  }

  /// ファイル入力内容からメンバ変数に値を設定する
  private void ParseRawData(Key key, string rawData) {
    // keyからメンバ変数に値を代入していく
    switch (key) {
      case Key.RecentProfile1FilePath: {
        this.reverseRecentProfiles[4].FilePath = rawData;
        break;
      }
      case Key.RecentProfile1FileName: {
        this.reverseRecentProfiles[4].FileName = rawData;
        break;
      }
      case Key.RecentProfile1Timestamp: {
        DateTime parsedDate;
        if (DateTime.TryParse(rawData, out parsedDate)) {
          this.reverseRecentProfiles[4].Timestamp = parsedDate;
        }
        break;
      }
      case Key.RecentProfile2FilePath: {
        this.reverseRecentProfiles[3].FilePath = rawData;
        break;
      }
      case Key.RecentProfile2FileName: {
        this.reverseRecentProfiles[3].FileName = rawData;
        break;
      }
      case Key.RecentProfile2Timestamp: {
        DateTime parsedDate;
        if (DateTime.TryParse(rawData, out parsedDate)) {
          this.reverseRecentProfiles[3].Timestamp = parsedDate;
        }
        break;
      }
      case Key.RecentProfile3FilePath: {
        this.reverseRecentProfiles[2].FilePath = rawData;
        break;
      }
      case Key.RecentProfile3FileName: {
        this.reverseRecentProfiles[2].FileName = rawData;
        break;
      }
      case Key.RecentProfile3Timestamp: {
        DateTime parsedDate;
        if (DateTime.TryParse(rawData, out parsedDate)) {
          this.reverseRecentProfiles[2].Timestamp = parsedDate;
        }
        break;
      }
      case Key.RecentProfile4FilePath: {
        this.reverseRecentProfiles[1].FilePath = rawData;
        break;
      }
      case Key.RecentProfile4FileName: {
        this.reverseRecentProfiles[1].FileName = rawData;
        break;
      }
      case Key.RecentProfile4Timestamp: {
        DateTime parsedDate;
        if (DateTime.TryParse(rawData, out parsedDate)) {
          this.reverseRecentProfiles[1].Timestamp = parsedDate;
        }
        break;
      }
      case Key.RecentProfile5FilePath: {
        this.reverseRecentProfiles[0].FilePath = rawData;
        break;
      }
      case Key.RecentProfile5FileName: {
        this.reverseRecentProfiles[0].FileName = rawData;
        break;
      }
      case Key.RecentProfile5Timestamp: {
        DateTime parsedDate;
        if (DateTime.TryParse(rawData, out parsedDate)) {
          this.reverseRecentProfiles[0].Timestamp = parsedDate;
        }
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
      case Key.TmpMainWindowLeft: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.tmpMainWindowLeft = parsedData; 
        }
        break;
      }
      case Key.TmpMainWindowTop: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.tmpMainWindowTop = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowWidth: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.tmpMainWindowWidth = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowHeight: {
        double parsedData;
        if (double.TryParse(rawData, out parsedData)) {
          this.tmpMainWindowHeight = parsedData;
        }
        break;
      }
      case Key.TmpMainWindowState: {
        WindowState parsedData;
        if (Enum.TryParse<WindowState>(rawData, out parsedData)) {
          this.tmpMainWindowState = parsedData;
        }
        break;
      }
      case Key.TmpAreaIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpAreaIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpOptionsIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpOptionsIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpResizeMethodIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpResizeMethodIsExpanded = parsedData;
        }
        break;
      }
      case Key.TmpLayoutIsExpanded: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpLayoutIsExpanded = parsedData; 
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
      case Key.TmpCompactView: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpCompactView = parsedData;
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
      case Key.TmpRestoreLastProfile: {
        bool parsedData;
        if (bool.TryParse(rawData, out parsedData)) {
          this.tmpRestoreLastProfile = parsedData;
        }
        break;
      }
    }
  }

  //===================================================================
  // アクセッサ
  // プロパティ形式ではあるがDataBindingには使わないように！
  // なお、配列のメンバ変数へのアクセスはプロパティにはこだわらなくても良い
  //===================================================================

  // 上下逆に変換している
  public ProfileInfo RecentProfile(int oneBasedIndex) {
    int reverseIndex = RecentProfilesLength - oneBasedIndex;
    Debug.Assert(0 <= reverseIndex && reverseIndex < RecentProfilesLength);
    return this.reverseRecentProfiles[reverseIndex];
  }
  public void AddRecentProfile(ProfileInfo profile) {
    // Queueに再構成してから配列に書き戻す
    var queue = new Queue<ProfileInfo>();
    var alreadyExists = false;
    foreach (var recentProfile in this.reverseRecentProfiles) {
      if (recentProfile.Equals(profile)) {
        // 追加したいものが配列の中に見つかった場合はEnqueueしない
        alreadyExists = true;
        continue;
      }
      queue.Enqueue(recentProfile);
    }
    if (!alreadyExists) {
      // 新規なら古いのを一個消す
      queue.Dequeue();
    }
    queue.Enqueue(profile);
    Debug.Assert(queue.Count == RecentProfilesLength); 

    // 配列に書き戻して終了
    queue.CopyTo(this.reverseRecentProfiles, 0);
  }

  public string FFmpegPath {
    get { return this.ffmpegPath; }
    set { this.ffmpegPath = value; }
  }
  public string FFmpegArguments {
    get { return this.ffmpegArguments; }
    set { this.ffmpegArguments = value; }
  }

  public double TmpMainWindowLeft {
    get { return this.tmpMainWindowLeft; }
    set { this.tmpMainWindowLeft = value; }
  }
  public double TmpMainWindowTop {
    get { return this.tmpMainWindowTop; }
    set { this.tmpMainWindowTop = value; }
  }
  public double TmpMainWindowWidth {
    get { return this.tmpMainWindowWidth; }
    set { this.tmpMainWindowWidth = value; }
  }
  public double TmpMainWindowHeight {
    get { return this.tmpMainWindowHeight; }
    set { this.tmpMainWindowHeight = value; }
  }
  public WindowState TmpMainWindowState {
    get { return this.tmpMainWindowState; }
    set { this.tmpMainWindowState = value; }
  }

  public bool TmpAreaIsExpanded {
    get { return this.tmpAreaIsExpanded; }
    set { this.tmpAreaIsExpanded = value; }
  }
  public bool TmpOptionsIsExpanded {
    get { return this.tmpOptionsIsExpanded; }
    set { this.tmpOptionsIsExpanded = value; }
  }
  public bool TmpResizeMethodIsExpanded {
    get { return this.tmpResizeMethodIsExpanded; }
    set { this.tmpResizeMethodIsExpanded = value; }
  }
  public bool TmpLayoutIsExpanded {
    get { return this.tmpLayoutIsExpanded; }
    set { this.tmpLayoutIsExpanded = value; }
  }

  public bool AutoApply {
    get { return this.autoApply; }
    set { this.autoApply = value; }
  }
  public bool LayoutPreview {
    get { return this.layoutPreview; }
    set { this.layoutPreview = value; }
  }
  public bool LayoutBorder {
    get { return this.layoutBorder; }
    set { this.layoutBorder = value; }
  }
  public bool LayoutSnap {
    get { return this.layoutSnap; }
    set { this.layoutSnap = value; }
  }

  public bool TmpCompactView {
    get { return this.tmpCompactView; }
    set { this.tmpCompactView = value; }
  }
  public bool ForceAeroOn {
    get { return this.forceAeroOn; }
    set { this.forceAeroOn = value; }
  }
  public bool TmpRestoreLastProfile {
    get { return this.tmpRestoreLastProfile; }
    set { this.tmpRestoreLastProfile = value; }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  public class ProfileInfo {
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public DateTime Timestamp { get; set; }
    public ProfileInfo() {
      this.FilePath = string.Empty;
      this.FileName = string.Empty;
      this.Timestamp = DateTime.Now;
    }
    public bool IsEmpty() {
      return this.FilePath == string.Empty;
    }
    // FilePathをキー代わりに使う
    public override string ToString() {
      return this.FilePath;
    }
    public override bool Equals(object obj) {
      return this.FilePath.Equals(obj);
    }
  }

  /// @caution 先頭から古く、末尾が一番新しい
  private ProfileInfo[] reverseRecentProfiles = new ProfileInfo[RecentProfilesLength] {
    new ProfileInfo(), new ProfileInfo(), new ProfileInfo(), new ProfileInfo(), new ProfileInfo()
  };
  private string ffmpegPath = string.Empty;
  private string ffmpegArguments = string.Empty;
  private double tmpMainWindowLeft = Defaults.MainWindowLeft;
  private double tmpMainWindowTop = Defaults.MainWindowTop;
  private double tmpMainWindowWidth = Defaults.MainWindowWidth;
  private double tmpMainWindowHeight = Defaults.MainWindowHeight;
  private WindowState tmpMainWindowState = WindowState.Normal;
  private bool tmpAreaIsExpanded = true;
  private bool tmpOptionsIsExpanded = true;
  private bool tmpResizeMethodIsExpanded = true;
  private bool tmpLayoutIsExpanded = true;
  private bool autoApply = true;
  private bool layoutPreview = true;
  private bool layoutBorder = true;
  private bool layoutSnap = true;
  private bool tmpCompactView = false;
  private bool forceAeroOn = false;
  private bool tmpRestoreLastProfile = true;
}
}
