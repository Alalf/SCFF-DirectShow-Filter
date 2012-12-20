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
/// Loaded/Closingイベントハンドラ以外ではしてはいけない。
public class Options {

  //===================================================================
  // 定数
  //===================================================================

  private const int RecentProfilesLength = 5;

  /// Options用WindowState。System.Windows.WindowStateと相互に変換する。
  public enum WindowState {
    Normal,
    Minimized,
    Maximized
  }

  //===================================================================
  // アクセッサ
  // プロパティ形式ではあるがDataBindingには使わないように！
  // なお、配列のメンバ変数へのアクセスはプロパティにはこだわらなくても良い
  //===================================================================

  public string GetRecentProfile(int index) {
    // 上下逆に変換
    int reverseIndex = RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < RecentProfilesLength);
    return this.reverseRecentProfiles[reverseIndex];
  }
  public void AddRecentProfile(string profile) {
    // Queueに再構成してから配列に書き戻す
    var queue = new Queue<string>();
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
  internal void SetRecentProfile(int index, string profile) {
    // 上下逆に変換
    int reverseIndex = RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < RecentProfilesLength);
    this.reverseRecentProfiles[reverseIndex] = profile;
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

  /// @caution 先頭から古く、末尾が一番新しい
  private string[] reverseRecentProfiles = new string[RecentProfilesLength] {
    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
  };
  private string ffmpegPath = string.Empty;
  private string ffmpegArguments = string.Empty;
  private double tmpMainWindowLeft = Constants.MainWindowLeft;
  private double tmpMainWindowTop = Constants.MainWindowTop;
  private double tmpMainWindowWidth = Constants.MainWindowWidth;
  private double tmpMainWindowHeight = Constants.MainWindowHeight;
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
