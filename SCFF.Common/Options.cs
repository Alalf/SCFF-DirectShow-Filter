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

/// @file SCFF.Common/Options.cs
/// プロファイル以外のアプリケーション設定

namespace SCFF.Common {

using System.Collections.Generic;
using System.Diagnostics;

/// プロファイル以外のアプリケーション設定
///
/// アプリケーションの起動中にプロパティが最新の状態になっている必要の無いものには
/// Tmpという接頭辞をつけている。プログラム内部でそれらのプロパティを使うことは
/// Loaded/Closingイベントハンドラ以外ではしてはいけない。
public class Options {

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  public Options() {
    this.reverseRecentProfiles = new string[Constants.RecentProfilesLength] {
      string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 
    };
    this.FFmpegPath = string.Empty;
    this.FFmpegArguments = string.Empty;
    this.TmpMainWindowLeft = Constants.MainWindowLeft;
    this.TmpMainWindowTop = Constants.MainWindowTop;
    this.TmpMainWindowWidth = Constants.MainWindowWidth;
    this.TmpMainWindowHeight = Constants.MainWindowHeight;
    this.TmpMainWindowState = WindowState.Normal;
    this.TmpAreaIsExpanded = true;
    this.TmpOptionsIsExpanded = true;
    this.TmpResizeMethodIsExpanded = true;
    this.TmpLayoutIsExpanded = true;
    this.AutoApply = false;
    this.LayoutPreview = false;
    this.LayoutBorder = true;
    this.LayoutSnap = true;
    this.CompactView = false;
    this.ForceAeroOn = false;
    this.RestoreLastProfile = true;
  }

  //===================================================================
  // アクセッサ
  // プロパティ形式ではあるがDataBindingには使わないように！
  // なお、配列のメンバ変数へのアクセスはプロパティにはこだわらなくても良い
  //===================================================================

  public string GetRecentProfile(int index) {
    // 上下逆に変換
    int reverseIndex = Constants.RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < Constants.RecentProfilesLength);
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
    Debug.Assert(queue.Count == Constants.RecentProfilesLength); 

    // 配列に書き戻して終了
    queue.CopyTo(this.reverseRecentProfiles, 0);
  }
  internal void SetRecentProfile(int index, string profile) {
    // 上下逆に変換
    int reverseIndex = Constants.RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < Constants.RecentProfilesLength);
    this.reverseRecentProfiles[reverseIndex] = profile;
  }

  public string FFmpegPath { get; set; }
  public string FFmpegArguments { get; set; }

  public double TmpMainWindowLeft { get; set; }
  public double TmpMainWindowTop { get; set; }
  public double TmpMainWindowWidth { get; set; }
  public double TmpMainWindowHeight { get; set; }
  public WindowState TmpMainWindowState { get; set; }

  public bool TmpAreaIsExpanded { get; set; }
  public bool TmpOptionsIsExpanded { get; set; }
  public bool TmpResizeMethodIsExpanded { get; set; }
  public bool TmpLayoutIsExpanded { get; set; }

  public bool AutoApply { get; set; }
  public bool LayoutPreview { get; set; }
  public bool LayoutBorder { get; set; }
  public bool LayoutSnap { get; set; }

  public bool CompactView { get; set; }
  public bool ForceAeroOn { get; set; }
  public bool RestoreLastProfile { get; set; }

  //===================================================================
  // メンバ変数
  //===================================================================

  /// @caution 先頭から古く、末尾が一番新しい
  private string[] reverseRecentProfiles;
}
}
