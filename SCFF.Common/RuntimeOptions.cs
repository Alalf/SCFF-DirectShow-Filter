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

/// @file SCFF.Common/RuntimeOptions.cs
/// @copydoc SCFF::Common::RuntimeOptions

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SCFF.Interprocess;

/// アプリケーションの実行時設定
///
/// プロセスリストの管理や現在編集中のプレビューのサイズなどはこれで保持すること
public class RuntimeOptions {
  //===================================================================
  // 定数
  //===================================================================
  
  /// 不正なタイムスタンプ
  public const Int64 InvalidTimestamp = -1L;

  /// 不正な選択
  public const int InvalidSelection = -1;

  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public RuntimeOptions() {
    this.ProfilePath = string.Empty;
    this.ProfileName = string.Empty;
    this.LastSavedTimestamp = RuntimeOptions.InvalidTimestamp;
    this.LastAppliedTimestamp = RuntimeOptions.InvalidTimestamp;

    this.SelectedEntryIndex = RuntimeOptions.InvalidSelection;
    this.EntryList = new List<Entry>();
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 現在編集中のProfileのフルパス
  public string ProfilePath { get; set; }

  /// 現在編集中のProfile名
  public string ProfileName { get; set; }

  /// 最後に保存した時のタイムスタンプ
  /// @warning DateTime.UTCNow.Ticksを格納するので負の数にはなりえない
  public Int64 LastSavedTimestamp { get; set; }

  /// 最後にApplyしたときのタイムスタンプ
  /// @warning DateTime.UTCNow.Ticksを格納するので負の数にはなりえない
  public Int64 LastAppliedTimestamp { get; set; }

  //-------------------------------------------------------------------

  /// 現在選択中のエントリ(選択なしは-1)
  public int SelectedEntryIndex { get; set; }

  /// エントリのリスト
  private List<Entry> EntryList { get; set; }

  //-------------------------------------------------------------------

  /// EntryListを文字列化したリスト
  public List<string> EntryStringList {
    get {
      var list = new List<string>();
      foreach (var entry in this.EntryList) {
        var pixelFormatString =
            Constants.ImagePixelFormatLabels[(ImagePixelFormats)entry.SamplePixelFormat];

        var entryString = string.Format("[{0}] {1} ({2} {3}x{4} {5:F0}fps)",
            entry.ProcessID,
            entry.ProcessName,
            pixelFormatString,
            entry.SampleWidth, entry.SampleHeight,
            entry.FPS);

        list.Add(entryString);
      }
      return list;
    }
  }

  /// Directoryが空か
  public bool IsEntryListEmpty {
    get { return this.EntryList.Count == 0; }
  }

  /// 現在選択中のプロセスID
  public UInt32 CurrentProcessID {
    get {
      if (this.IsEntryListEmpty) return 0;
      return this.EntryList[this.SelectedEntryIndex].ProcessID;
    }
  }
  /// 現在選択中のピクセルフォーマット
  public ImagePixelFormats CurrentSamplePixelFormat {
    get {
      if (this.IsEntryListEmpty) return ImagePixelFormats.IYUV;
      return (ImagePixelFormats)this.EntryList[this.SelectedEntryIndex].SamplePixelFormat;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル幅
  public int CurrentSampleWidth {
    get {
      if (this.IsEntryListEmpty) return Constants.DummySampleWidth;
      return this.EntryList[this.SelectedEntryIndex].SampleWidth;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル高さ
  public int CurrentSampleHeight {
    get {
      if (this.IsEntryListEmpty) return Constants.DummySampleHeight;
      return this.EntryList[this.SelectedEntryIndex].SampleHeight;
    }
  }

  //===================================================================
  // アクセサ
  //===================================================================

  /// 共有メモリアクセスオブジェクトからDirectoryを読み込む
  /// @param interprocess 共有メモリアクセス用オブジェクト
  public void RefreshDirectory(Interprocess interprocess) {
    // 共有メモリにアクセス
    var initResult = interprocess.InitDirectory();
    if (!initResult) return;
    Directory directory;
    var getResult = interprocess.GetDirectory(out directory);
    if (!getResult) return;

    // EntryListを更新
    this.EntryList.Clear();
    foreach (var entry in directory.Entries) {
      if (entry.ProcessID == 0) continue;
      this.EntryList.Add(entry);
    }

    // EntrySelectedIndexを更新
    if (this.IsEntryListEmpty) {
      this.SelectedEntryIndex = RuntimeOptions.InvalidSelection;
      return;
    }

    // SelectedEntryIndexが更新後も有効ならそのまま保持する
    /// @todo(me) プロセスIDが同じものを選択し続けるほうが正しい動作
    if (0 <= this.SelectedEntryIndex &&
        this.SelectedEntryIndex < this.EntryList.Count) {
      // リストの長さの中に納まっている場合は変更しない
      // nop
    } else {
      this.SelectedEntryIndex = 0;
    }
  }
}
}   // namespace SCFF.Common
