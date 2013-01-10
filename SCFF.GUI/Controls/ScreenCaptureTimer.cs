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

/// @file SCFF.GUI/Controls/ScreenCaptureTimer.cs
/// @copydoc SCFF::GUI::Controls::ScreenCaptureTimer

namespace SCFF.GUI.Controls {

using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Media.Imaging;
using SCFF.Common.GUI;

/// スクリーンキャプチャデータを取得するためのスレッド管理クラス
public class ScreenCaptureTimer {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public ScreenCaptureTimer(double timerPeriod) {
    this.timerPeriod = timerPeriod;
  }

  //===================================================================
  // タイマーコールバック
  //===================================================================

  /// タイマーコールバック
  private void TimerCallback(object state) {
    lock (this.sharedLock) {
      if (this.isSuspended) return;

      // キャプチャ
      for (int i = 0; i < Common.Constants.MaxLayoutElementCount; ++i) {
        if (this.requests[i] == null) {
          // 明示的に参照を外す
          this.cachedBitmaps[i] = null;
        } else {
          this.cachedBitmaps[i] = ScreenCapture.Capture(this.requests[i]);
        }
      }
    }
  }

  //===================================================================
  // 外部インタフェース
  //===================================================================

  //-------------------------------------------------------------------
  // スレッドの動作関連
  //-------------------------------------------------------------------

  /// 開始
  public void Init() {
    Debug.WriteLine("Init", "ScreenCaptureTimer");
    this.captureTimer = new Timer(TimerCallback, null, 0, (int)this.timerPeriod);
  }

  /// 開始
  public void Start() {
    Debug.WriteLine("Start", "ScreenCaptureTimer");
    lock (this.sharedLock) {
      this.isSuspended = false;
    }
  }

  /// 中断
  public void Suspend() {
    Debug.WriteLine("Suspend", "ScreenCaptureTimer");
    lock (this.sharedLock) {
      this.isSuspended = true;
      // すべての格納されたBitmapを削除する
      Array.Clear(this.cachedBitmaps, 0, Common.Constants.MaxLayoutElementCount);
    }
  }

  /// 終了
  /// @warning DispatcherShutdownなどで必ず呼ぶこと
  public void End() {
    Debug.WriteLine("End", "ScreenCaptureTimer");
    lock (this.sharedLock) {
      this.isSuspended = true;
      if (this.captureTimer != null) {
        this.captureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.captureTimer.Dispose();
        Debug.WriteLine("ScreenCaptureTimer.captureTimer", "*** MEMORY[DELETE] ***");
        this.captureTimer = null;
      }
    }
  }

  //-------------------------------------------------------------------
  // リクエストの設定とキャプチャされたビットマップの取得
  //-------------------------------------------------------------------

  /// スクリーンキャプチャをこのマネージャに依頼
  public void SendRequest(ScreenCaptureRequest request, bool forceUpdate) {
    Debug.WriteLine(string.Format("SendRequest: {0}(forceUpdate: {1})",
                    request.Index + 1, forceUpdate),
                    "ScreenCaptureTimer");
    lock (this.sharedLock) {
      this.requests[request.Index] = request;
      // forceUpdate時はUIスレッドで一回処理を行う
      if (!forceUpdate) return;
      if (this.isSuspended) return;
      this.cachedBitmaps[request.Index] = ScreenCapture.Capture(request);
    }
  }

  /// すべてのリクエストを削除する
  public void ClearRequests() {
    Debug.WriteLine("ClearRequests", "ScreenCaptureTimer");
    lock (this.sharedLock) {
      Array.Clear(this.requests, 0, Common.Constants.MaxLayoutElementCount);
    }
  }

  /// 結果を取得
  public BitmapSource GetBitmapSource(int index) {
    //　参照の代入はアトミックなので問題がない
    return this.cachedBitmaps[index];
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// タイマー
  private Timer captureTimer = null;

  /// タイマーの間隔
  private double timerPeriod;

  //-------------------------------------------------------------------
  // 複数のスレッドで共有するデータ
  //-------------------------------------------------------------------

  /// 共有ロック
  private readonly object sharedLock = new Object();

  /// 共有(自R/他W): プレビュー表示中かどうか
  private bool isSuspended = true;
  
  /// 共有(自R/他W): スクリーンキャプチャのリクエストをまとめた配列
  private ScreenCaptureRequest[] requests =
      new ScreenCaptureRequest[Common.Constants.MaxLayoutElementCount];

  /// 共有(自W/他R): スクリーンキャプチャビットマップのキャッシュ
  private BitmapSource[] cachedBitmaps =
      new BitmapSource[Common.Constants.MaxLayoutElementCount];
}
}   // namespace SCFF.GUI.Controls
