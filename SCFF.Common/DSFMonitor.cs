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

/// @file SCFF.Common/DSFMonitor.cs
/// @copydoc SCFF::Common::DSFMonitor

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using SCFF.Interprocess;

/// SCFF DirectShow Filterのエラー状態・生存チェックを行う
public class DSFMonitor {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public DSFMonitor(Interprocess interprocess) {
    this.Interprocess = interprocess;
    this.MonitoredDSFs = new Dictionary<UInt32,Task>();

    this.Interprocess.InitShutdownEvent();
  }

  //===================================================================
  // アクセサ
  //===================================================================
 
  /// 監視スタート
  /// @param[in] processID 監視対象のプロセスID
  public void Start(UInt32 processID) {
    // すでに監視中でTaskがまだ生きている場合は何もしない
    if (this.MonitoredDSFs.ContainsKey(processID) &&
        !this.MonitoredDSFs[processID].IsCompleted) return;

    var context = SynchronizationContext.Current;

    // Task生成
    var task = new Task(() => {
      // 監視開始
      Debug.WriteLine("Start Task: " + processID, "DSFMonitor");

      // 開始前に一回Eventをクリアしておく
      this.Interprocess.CheckErrorEvent(processID);

      // エラーが起きるまで待機
      var dsfErrorOccured = this.Interprocess.WaitUntilErrorEventOccured(processID);
      if (dsfErrorOccured) {
        context.Post((s) => {
          // Event: DSFErrorOccured
          var args = new DSFErrorOccuredEventArgs(processID);
          var handler = this.OnErrorOccured;
          if (handler != null) handler(this, args);
        }, null);
      }

      // 監視終了
      Debug.WriteLine("End Task: " + processID, "DSFMonitor");
    });

    // 監視中リストに追加
    // ValueはIsCompletedであることが保障されているので上書き可能
    this.MonitoredDSFs[processID] = task;

    // Taskの実行
    task.Start();
  }

  /// 死んだプロセスの監視Taskを削除
  public void RemoveZombies() {
    var removeList = new List<UInt32>();
    foreach (var kv in this.MonitoredDSFs) {
      // プロセスがまだ実行中なら消去の必要はない
      if (Utilities.IsProcessAlive(kv.Key)) continue;

      // WaitOneを強制解除してTaskの終了を待つ
      Debug.WriteLine("Removing Zombie: " + kv.Key, "DSFMonitor");
      this.Interprocess.SetErrorEvent(kv.Key);
      kv.Value.Wait();
      removeList.Add(kv.Key);
    }

    // MonitoredDSFsの更新
    foreach (var processID in removeList) {
      Debug.WriteLine("Removing Task: " + processID, "DSFMonitor");
      this.MonitoredDSFs.Remove(processID);
    }
  }

  /// 死んだプロセスを明示的に削除
  public void RemoveZombie(UInt32 processID) {
    Debug.Assert(!Utilities.IsProcessAlive(processID));

    // WaitOneを強制解除してTaskの終了を待つ
    Debug.WriteLine("Removing Zombie: " + processID, "DSFMonitor");
    this.Interprocess.SetErrorEvent(processID);
    this.MonitoredDSFs[processID].Wait();

    // MonitoredDSFsの更新
    Debug.WriteLine("Removing Task: " + processID, "DSFMonitor");
    this.MonitoredDSFs.Remove(processID);
  }

  /// 全てのTaskの終了を待機
  public void Exit() {
    Debug.WriteLine("Exit", "DSFMonitor");
    this.Interprocess.SetShutdownEvent();
    foreach (var kv in this.MonitoredDSFs) {
      kv.Value.Wait();
    }
    // MonitoredDSFsの更新
    this.MonitoredDSFs.Clear();
  }

  //===================================================================
  // イベント
  //===================================================================

  /// DSFでエラーが発生した後
  public event EventHandler<DSFErrorOccuredEventArgs> OnErrorOccured;

  //===================================================================
  // プロパティ
  //===================================================================

  /// プロセス間通信用オブジェクト
  private Interprocess Interprocess { get; set; }

  /// 監視中のDirectShow Filter
  private Dictionary<UInt32,Task> MonitoredDSFs { get; set; }
}
}
