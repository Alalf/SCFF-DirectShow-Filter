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
      foreach (var request in this.requests) {
        if (request == null) continue;
        this.cachedBitmaps[request.Index] = ScreenCapture.Capture(request);
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
  public void Start() {
    Debug.WriteLine("ScreenCapturer: Start");
    this.captureTimer = new Timer(TimerCallback, null, 0, (int)this.timerPeriod);
  }

  /// 中断
  public void Suspend() {
    Debug.WriteLine("ScreenCapturer: Suspend");
    lock (this.sharedLock) {
      this.isSuspended = true;
      // すべての格納されたBitmapを削除する
      this.cachedBitmaps = new BitmapSource[Common.Constants.MaxLayoutElementCount];
    }
  }

  /// 再開
  public void Resume() {
    Debug.WriteLine("ScreenCapturer: Resume");
    lock (this.sharedLock) {
      this.isSuspended = false;
      /// @todo(me) すべてのBitmapを一回UIスレッドで書き直す
    }
  }

  /// 終了
  /// @warning DispatcherShutdownなどで必ず呼ぶこと
  public void End() {
    Debug.WriteLine("ScreenCapturer: End");
    lock (this.sharedLock) {
      this.isSuspended = true;
      if (this.captureTimer != null) {
        this.captureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.captureTimer.Dispose();
        Debug.WriteLine("ScreenCapturer: Timer is Disposed");
        this.captureTimer = null;
      }
    }
  }

  //-------------------------------------------------------------------
  // リクエストの設定とキャプチャされたビットマップの取得
  //-------------------------------------------------------------------

  /// スクリーンキャプチャをこのマネージャに依頼
  public void SendRequest(ScreenCaptureRequest request, bool forceUpdate) {
    Debug.WriteLine("ScreenCapturer: Request Arrived");
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
    Debug.WriteLine("ScreenCapturer: Clear All Requests");
    lock (this.sharedLock) {
      this.requests = new ScreenCaptureRequest[Common.Constants.MaxLayoutElementCount];
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
  private bool isSuspended = false;
  
  /// 共有(自R/他W): スクリーンキャプチャのリクエストをまとめた配列
  private ScreenCaptureRequest[] requests =
      new ScreenCaptureRequest[Common.Constants.MaxLayoutElementCount];

  /// 共有(自W/他R): スクリーンキャプチャのキャッシュ
  private BitmapSource[] cachedBitmaps =
      new BitmapSource[Common.Constants.MaxLayoutElementCount];
}
}   // namespace SCFF.GUI.Controls
