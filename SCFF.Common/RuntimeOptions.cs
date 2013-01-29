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
using System.Diagnostics;
using SCFF.Interprocess;

/// アプリケーションの実行時設定
///
/// プロセスリストの管理や現在編集中のプレビューのサイズなどはこれで保持すること
public class RuntimeOptions {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public RuntimeOptions() {
    this.ProfilePath = string.Empty;
    this.ProfileName = string.Empty;
    this.LastSavedTimestamp = -1L;
    this.LastAppliedTimestamp = -1L;
    this.SelectedEntryIndex = -1;

    // directoryはentriesの初期化がされていないのでここでやる
    this.directory.Entries = new Entry[Interprocess.MaxEntry];
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

  /// 現在選択中のエントリ(選択なしは-1)
  public int SelectedEntryIndex { get; set; }

  //-------------------------------------------------------------------

  /// 現在選択中のプロセスID
  public UInt32 CurrentProcessID {
    get {
      Debug.Assert(this.SelectedEntryIndex >= 0);
      return this.directory.Entries[this.SelectedEntryIndex].ProcessID;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル幅
  public int CurrentSampleWidth {
    get {
      if (this.SelectedEntryIndex < 0) return Constants.DummySampleWidth;
      return this.directory.Entries[this.SelectedEntryIndex].SampleWidth;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル高さ
  public int CurrentSampleHeight {
    get {
      if (this.SelectedEntryIndex < 0) return Constants.DummySampleHeight;
      return this.directory.Entries[this.SelectedEntryIndex].SampleHeight;
    }
  }

  //===================================================================
  // アクセサ
  //===================================================================

  public void Refresh() {
    /// @todo(me) テスト中なのであとで仮想メモリに書き換える
    if (this.SelectedEntryIndex == -1) {
      this.directory.Entries[0] = new Entry() {
        ProcessName = "DUMMY",
        ProcessID = 0,
        FPS = 30,
        SampleWidth = 640,
        SampleHeight = 480,
        SamplePixelFormat = (int)ImagePixelFormats.RGB0
      };
      this.SelectedEntryIndex = 0;
    } else {
      this.directory.Entries[0] = new Entry();
      this.SelectedEntryIndex = -1;
    }
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// 共有メモリから読み込んだディレクトリ
  Directory directory = new Directory();
}
}   // namespace SCFF.Common
