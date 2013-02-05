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

/// @file SCFF.Common/DSFMonitr.cs
/// SCFF DirectShow Filterのエラー状態・生存チェックを行う

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using SCFF.Interprocess;

/// SCFF DirectShow Filterでエラーが発生したとき
public class DSFErrorOccuredEventArgs : EventArgs {
  /// コンストラクタ
  public DSFErrorOccuredEventArgs(UInt32 processID) {
    this.ProcessID = processID;
  }
  /// プロパティ: エラーが発生したDSFがロードされているプロセスID
  public uint ProcessID { get; private set; }
}

public class DSFMonitor {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public DSFMonitor(Interprocess interprocess) {
    this.SharedLock = new Object();
    this.Interprocess = interprocess;
    this.MonitoredDSFs = new HashSet<UInt32>();
  }

  /// デストラクタ
  ~DSFMonitor() {
    this.CleanupAll();
  }

  //===================================================================
  // アクセサ
  //===================================================================
 
  /// 監視スタート
  public void Start(UInt32 processID) {
    lock (this.SharedLock) {
      if (this.MonitoredDSFs.Contains(processID)) {
        // 既に監視中なので何もしない
        return;
      }
      Debug.WriteLine("Creating Task: " + processID, "DSFMonitor");
      var task = Task.Factory.StartNew(() => {
        this.Interprocess.InitErrorEvent(processID);
        this.Interprocess.WaitUntilErrorEventOccured();
      });
      task.ContinueWith((t) => {
        var handler = this.OnErrorOccured;
        if (handler != null) {
          handler(this, new DSFErrorOccuredEventArgs(processID));
        }
        lock (this.SharedLock) {
          Debug.WriteLine("Removing Task: " + processID, "DSFMonitor");
          this.MonitoredDSFs.Remove(processID);
        }
      });
      // 監視中リストに追加
      this.MonitoredDSFs.Add(processID);
    }
  }

  /// 監視リストをチェックして必要の無くなったtaskを終了させておく
  public void CleanupAll() {
    lock (this.SharedLock) {
      foreach (var processID in this.MonitoredDSFs) {
        if (Utilities.IsProcessAlive(processID)) return;
        Debug.WriteLine("Cleanup: " + processID, "DSFMonitor");
        this.Interprocess.InitErrorEvent(processID);
        // WaitOneを解除
        this.Interprocess.RaiseErrorEvent();
      }
    }
  }

  /// 一つだけTaskを終了させる
  public void Cleanup(UInt32 processID) {
    lock (this.SharedLock) {
      Debug.Assert(!Utilities.IsProcessAlive(processID));
      Debug.WriteLine("Cleanup: " + processID, "DSFMonitor");
      this.Interprocess.InitErrorEvent(processID);
      // WaitOneを解除
      this.Interprocess.RaiseErrorEvent();
    }
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 共有ロック
  private Object SharedLock { get; set; }

  /// イベント
  public event EventHandler<DSFErrorOccuredEventArgs> OnErrorOccured;

  /// プロセス間通信用オブジェクト
  private Interprocess Interprocess { get; set; }

  /// 監視中のDirectShow Filter
  private HashSet<UInt32> MonitoredDSFs { get; set; }
}
}
