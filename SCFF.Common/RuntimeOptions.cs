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
using SCFF.Common.Ext;
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

    this.CurrentProcessID = 0;
    this.Entries = new Dictionary<UInt32,InternalEntry>();
    this.EntryLabels = new List<Tuple<UInt32,string>>();

    this.WasAeroOnWhenStartup = false;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 現在編集中のProfileのフルパス
  public string ProfilePath { get; set; }

  /// 現在編集中のProfile名
  public string ProfileName { get; set; }

  /// 最後に保存した時のタイムスタンプ
  /// @warning DateTime.Now.Ticksを格納するので負の数にはなりえない
  public Int64 LastSavedTimestamp { get; set; }

  /// 最後にApplyしたときのタイムスタンプ
  /// @warning DateTime.Now.Ticksを格納するので負の数にはなりえない
  /// @todo(me) これはProcessIDごとになるのでは？
  public Int64 LastAppliedTimestamp { get; set; }

  //-------------------------------------------------------------------
 
  /// 現在のプロセスID
  public UInt32 CurrentProcessID { get; set; }

  /// Entryは構造体のため、Dictionaryから取り出すごとにコピーが発生する
  /// これを避けるため、参照型のまま取り扱えるEntryクラスを用意した
  private class InternalEntry {
    public InternalEntry(int sampleWidth, int sampleHeight,
                         ImagePixelFormats samplePixelFormat) {
      this.SampleWidth = sampleWidth;
      this.SampleHeight = sampleHeight;
      this.SamplePixelFormat = samplePixelFormat;
    }

    public int SampleWidth { get; private set; }
    public int SampleHeight { get; private set; }
    public ImagePixelFormats SamplePixelFormat { get; private set; }
    // FPS
  }

  /// エントリディクショナリ
  private Dictionary<UInt32,InternalEntry> Entries { get; set; }

  /// エントリの表示用ディクショナリ
  public List<Tuple<UInt32,string>> EntryLabels { get; private set; }

  //-------------------------------------------------------------------

  /// 起動時にAeroがOnだったか
  private bool WasAeroOnWhenStartup { get; set; }

  /// DWMAPI.DLLが利用可能かどうか
  private bool CanUseAero {
    get {
      // Vista以降なら利用可能
      return (Environment.OSVersion.Platform == PlatformID.Win32NT &&
              Environment.OSVersion.Version.Major >= 6);
    }
  }

  /// Aeroの状態を変更可能か
  public bool CanSetAero {
    get { return this.CanUseAero && this.WasAeroOnWhenStartup; }
  }

  //-------------------------------------------------------------------

  /// 現在編集中のプロセスIDが有効か
  public bool IsCurrentProcessIDValid {
    get { return this.Entries.ContainsKey(this.CurrentProcessID); }
  }

  /// 現在選択中のピクセルフォーマット
  public ImagePixelFormats CurrentSamplePixelFormat {
    get {
      if (!this.IsCurrentProcessIDValid)
        return ImagePixelFormats.IYUV;

      var entry = this.Entries[this.CurrentProcessID];
      return entry.SamplePixelFormat;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル幅
  public int CurrentSampleWidth {
    get {
      if (!this.IsCurrentProcessIDValid)
        return Constants.DummySampleWidth;

      var entry = this.Entries[this.CurrentProcessID];
      return entry.SampleWidth;
    }
  }
  /// 現在選択中のプロセスが要求するサンプル高さ
  public int CurrentSampleHeight {
    get {
      if (!this.IsCurrentProcessIDValid)
        return Constants.DummySampleHeight;
      
      var entry = this.Entries[this.CurrentProcessID];
      return entry.SampleHeight;
    }
  }

  //===================================================================
  // アクセサ
  //===================================================================

  //-------------------------------------------------------------------
  // Directory/Entry
  //-------------------------------------------------------------------

  /// Entryをラベルに変換する
  private string GetEntryLabel(Entry entry) {
    var pixelFormatString =
        Constants.ImagePixelFormatLabels[(ImagePixelFormats)entry.SamplePixelFormat];

    return string.Format("[{0}] {1} ({2} {3}x{4} {5:F0}fps)",
        entry.ProcessID,
        entry.ProcessName,
        pixelFormatString,
        entry.SampleWidth, entry.SampleHeight,
        entry.FPS);
  }

  /// 共有メモリアクセスオブジェクトからDirectoryを読み込む
  /// @param interprocess プロセス間通信用オブジェクト
  public void RefreshDirectory(Interprocess interprocess) {
    // 共有メモリにアクセス
    var initResult = interprocess.InitDirectory();
    if (!initResult) return;
    Directory directory;
    var getResult = interprocess.GetDirectory(out directory);
    if (!getResult) return;

    // Entries/EntryLabelsを更新
    this.Entries.Clear();
    this.EntryLabels.Clear();
    foreach (var entry in directory.Entries) {
      if (entry.ProcessID == 0) continue;

      // InternalEntryの生成と追加
      var internalEntry = new InternalEntry(
          entry.SampleWidth, entry.SampleHeight,
          (ImagePixelFormats)entry.SamplePixelFormat);
      this.Entries.Add(entry.ProcessID, internalEntry);

      // ラベルの生成と追加
      var tuple = new Tuple<UInt32,string>(entry.ProcessID, this.GetEntryLabel(entry));
      this.EntryLabels.Add(tuple);
    }

    // 現在選択中のプロセスIDがなくなっていた場合
    if (!this.IsCurrentProcessIDValid && this.Entries.Count > 0) {
      // 適当に最初の一個を選ぶ
      foreach (var key in this.Entries.Keys) {
        this.CurrentProcessID = key;
        break;
      }
    }
  }

  //-------------------------------------------------------------------
  // Aero
  //-------------------------------------------------------------------

  /// 起動時のAeroの状態を保存
  public void SaveStartupAeroState() {
    if (!this.CanUseAero) {
      this.WasAeroOnWhenStartup = false;
      return;
    }

    bool isAeroOn;
    DWMAPI.DwmIsCompositionEnabled(out isAeroOn);
    this.WasAeroOnWhenStartup = isAeroOn;
  }

  /// Dwmapi.dllを利用してAeroをOnに
  public void SetAeroOn() {
    if (!this.CanUseAero) return;
   
    bool isAeroOnNow;
    DWMAPI.DwmIsCompositionEnabled(out isAeroOnNow);
    if (!isAeroOnNow) {
      DWMAPI.DwmEnableComposition(DWMAPI.DWM_EC_ENABLECOMPOSITION);
    }
  }

  /// Dwmapi.dllを利用してAeroをOffに
  public void SetAeroOff() {
    if (!this.CanUseAero) return;
   
    bool isAeroOnNow;
    DWMAPI.DwmIsCompositionEnabled(out isAeroOnNow);
    if (isAeroOnNow) {
      DWMAPI.DwmEnableComposition(DWMAPI.DWM_EC_DISABLECOMPOSITION);
    }
  }

  /// AeroをOffにしていたらOnに戻す
  public void RestoreStartupAeroState() {
    if (!this.CanUseAero) return;
    if (this.WasAeroOnWhenStartup) {
      this.SetAeroOn();
    } else {
      this.SetAeroOff();
    }
  }

}
}   // namespace SCFF.Common
