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
/// @warning 接頭辞Tmpがついたプロパティの使用はApp起動時・終了時以外禁止
/// @warning DataBindingの使用は禁止
/// @attention 配列はプロパティにしなくてよい
public class Options {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
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
    this.AreaIsExpanded = true;
    this.OptionsIsExpanded = true;
    this.ResizeMethodIsExpanded = true;
    this.LayoutIsExpanded = true;
    this.AutoApply = false;
    this.LayoutPreview = false;
    this.LayoutBorder = true;
    this.LayoutSnap = true;
    this.CompactView = false;
    this.ForceAeroOn = false;
    this.RestoreLastProfile = true;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// FFmpeg.exeのフルパス
  public string FFmpegPath { get; set; }
  /// FFmpeg.exeに渡す引数
  public string FFmpegArguments { get; set; }

  /// MainWindowの左上端のX座標
  public double TmpMainWindowLeft { get; set; }
  /// MainWindowの左上端のY座標
  public double TmpMainWindowTop { get; set; }
  /// MainWindowの幅
  public double TmpMainWindowWidth { get; set; }
  /// MainWindowの高さ
  public double TmpMainWindowHeight { get; set; }
  /// MainWindowのウィンドウ状態
  public WindowState TmpMainWindowState { get; set; }

  /// AreaExpanderが開いている
  public bool AreaIsExpanded { get; set; }
  /// OptionsExpanderが開いている
  public bool OptionsIsExpanded { get; set; }
  /// ResizeMethodExpanderが開いている
  public bool ResizeMethodIsExpanded { get; set; }
  /// LayoutExpanderが開いている
  public bool LayoutIsExpanded { get; set; }

  /// プロファイル更新時に自動的に仮想メモリに書き込む
  public bool AutoApply { get; set; }
  /// LayoutEditでプレビュー表示を行う
  public bool LayoutPreview { get; set; }
  /// LayoutEditで枠線とキャプション描画を行う
  public bool LayoutBorder { get; set; }
  /// LayoutEditで移動・拡大縮小時にスナップ機能を使う
  public bool LayoutSnap { get; set; }

  /// コンパクト表示にする
  public bool CompactView { get; set; }
  /// 強制的にAeroをOnにする
  public bool ForceAeroOn { get; set; }
  /// 最後に使用したプロファイルを起動時に読み込む
  public bool RestoreLastProfile { get; set; }

  //===================================================================
  // アクセサ
  //===================================================================

  /// プロファイルパスリストのindex番目を取得する
  public string GetRecentProfile(int index) {
    // 上下逆に変換
    int reverseIndex = Constants.RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < Constants.RecentProfilesLength);
    return this.reverseRecentProfiles[reverseIndex];
  }

  /// プロファイルパスリストにパスを追加する
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
  
  /// プロファイルパスリストに直接indexを指定してパスを設定する
  internal void SetRecentProfile(int index, string profile) {
    // 上下逆に変換
    int reverseIndex = Constants.RecentProfilesLength - index - 1;
    Debug.Assert(0 <= reverseIndex && reverseIndex < Constants.RecentProfilesLength);
    this.reverseRecentProfiles[reverseIndex] = profile;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// 最近使用したプロファイルのパスのリスト
  /// @warning 先頭から古く、末尾が一番新しい
  private string[] reverseRecentProfiles;
}
}
