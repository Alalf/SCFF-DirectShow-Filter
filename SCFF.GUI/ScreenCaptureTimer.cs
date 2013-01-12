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

/// @file SCFF.GUI/ScreenCaptureTimer.cs
/// @copydoc SCFF::GUI::ScreenCaptureTimer

namespace SCFF.GUI {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SCFF.Common;
using SCFF.Common.GUI;

/// スクリーンキャプチャデータを取得するためのスレッド管理クラス
public class ScreenCaptureTimer : IDisposable {
  //===================================================================
  // 定数
  //===================================================================

  private const int DefaultTimerPeriod = 3000;  ///< 更新間隔3秒

  private const int MaxBitmapWidth = 640;   ///< BitmapSourceの最大幅
  private const int MaxBitmapHeight = 480;  ///< BitmapSourceの最大高さ

  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public ScreenCaptureTimer() {
    this.timerPeriod = ScreenCaptureTimer.DefaultTimerPeriod;

    this.captureTimer = new Timer(TimerCallback, null, 0, (int)this.timerPeriod);
    Debug.WriteLine("ScreenCaptureTimer.captureTimer", "*** MEMORY[NEW] ***");
  }

  /// Dispose
  public void Dispose() {
    lock (this.sharedLock) {
      if (this.captureTimer != null) {
        this.isRunning = false;
        this.captureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.captureTimer.Dispose();
        this.captureTimer = null;
        Debug.WriteLine("ScreenCaptureTimer.captureTimer", "*** MEMORY[DISPOSE] ***");
      }
    }
    GC.SuppressFinalize(this);
  }

  /// デストラクタ
  ~ScreenCaptureTimer() {
    this.Dispose();
  }

  //===================================================================
  // スクリーンキャプチャ
  //===================================================================

  /// 最大幅に調整する
  private BitmapSource Resize(BitmapSource original) {
    // scale計算
    double scaleX = 1.0;
    double scaleY = 1.0;
    if (original.PixelWidth <= ScreenCaptureTimer.MaxBitmapWidth &&
        original.PixelHeight <= ScreenCaptureTimer.MaxBitmapHeight) {
      // 最大サイズよりちいさい
      // nop
    } else if (original.PixelWidth < original.PixelHeight) {
      // 最大サイズオーバー＋縦長＝縦を短く
      scaleY = (double)ScreenCaptureTimer.MaxBitmapHeight / original.PixelHeight;
      scaleX = scaleY;
    } else {
      // 最大サイズオーバー＋横長＝横を短く
      scaleX = (double)ScreenCaptureTimer.MaxBitmapWidth / original.PixelWidth;
      scaleY = scaleX;
    }
    
    return new TransformedBitmap(original, new ScaleTransform(scaleX, scaleY));
  }

  /// キャプチャしたHBitmapをBitmapSource(Freezed)にして返す
  /// @param request スクリーンキャプチャ設定をまとめたリクエスト
  /// @return スクリーンキャプチャ結果が格納されたBitmapSource
  private BitmapSource Capture(ScreenCaptureRequest request) {
    // HBitmapからBitmapSourceを作成
    BitmapSource bitmap = null;
    using (var result = request.Execute()) {
      if (result == null) return bitmap;

      // HBitmapからBitmapSourceに変換
      bitmap = Imaging.CreateBitmapSourceFromHBitmap(
          result.Bitmap, IntPtr.Zero, Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());

      // Debug.WriteLine(string.Format("Captured: {0:D}x{1:D}",
      //                 bitmap.PixelWidth, bitmap.PixelHeight),
      //                 "ScreenCapture");
    }

    // Alphaチャンネル情報を削除
    bitmap = new FormatConvertedBitmap(bitmap, PixelFormats.Bgr32, null, 0.0);

    /// @todo(me) あまり大きな画像をメモリにおいておきたくない。
    ///           とはいえ、TransformedBitmapはちょっと重過ぎる。
    ///           メモリよりもCPUリソースを残しておきたいのでこのままでいいかも。
    //bitmap = this.Resize(bitmap);
      
    // スレッド越しにアクセスされるためFreeze
    bitmap.Freeze();
    return bitmap;
  }

  /// キャプチャしたbyte[]をBitmapSource(Freezed)にして返す
  /// @param request スクリーンキャプチャ設定をまとめたリクエスト
  /// @return スクリーンキャプチャ結果が格納されたBitmapSource
  private BitmapSource CaptureByGetDIBits(ScreenCaptureRequest request) {
    // GetDIBitsでbyte[]にデータを格納
    var result = request.ExecuteByGetDIBits();
    if (result == null) return null;

    var bitmap = BitmapSource.Create(request.ClippingWidth, request.ClippingHeight,
                                 96.0, 96.0, PixelFormats.Bgr32, null, result, request.Stride);

    /// @todo(me) あまり大きな画像をメモリにおいておきたくない。
    ///           とはいえ、TransformedBitmapはちょっと重過ぎる。
    ///           メモリよりもCPUリソースを残しておきたいのでこのままでいいかも。
    //bitmap = this.Resize(bitmap);
      
    // スレッド越しにアクセスされるためFreeze
    bitmap.Freeze();
    return bitmap;
  }

  //===================================================================
  // タイマーコールバック
  //===================================================================

  /// タイマーコールバック
  /// @param state 使わない
  private void TimerCallback(object state) {
    lock (this.sharedLock) {
      if (!this.isRunning) return;
      
      var requests = new List<ScreenCaptureRequest>(this.cache.Keys);
      foreach(var request in requests) {
        Debug.Write("o");
        // this.cache[request] = this.Capture(request);
        this.cache[request] = this.CaptureByGetDIBits(request);
      }
      /// @todo(me) 潔癖症過ぎないか？
      // GC.Collect();
      // Debug.Write("G");
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
    lock (this.sharedLock) {
      var changed = !this.isRunning;
      Debug.WriteLineIf(changed, "Start", "ScreenCaptureTimer");
      this.isRunning = true;
    }
  }

  /// 中断
  public void Suspend() {
    lock (this.sharedLock) {
      var changed = this.isRunning;
      Debug.WriteLineIf(changed, "Suspend", "ScreenCaptureTimer");
      this.isRunning = false;
      // メモリ解放
      if (changed && this.cache.Count > 0) {
        this.cache.Clear();
        GC.Collect();
        Debug.WriteLine("Collect ScreenCaptureTimer.cache", "*** MEMORY[GC] ***");
      }
    }
  }

  //-------------------------------------------------------------------
  // リクエストの設定とキャプチャされたビットマップの取得
  //-------------------------------------------------------------------

  /// スクリーンキャプチャをこのマネージャに依頼
  public void UpdateRequest(Profile profile) {
    // ProfileからRequestを生成
    var profileRequests = new HashSet<ScreenCaptureRequest>();
    foreach (var layoutElement in profile) {
      if (!layoutElement.IsWindowValid) continue;
      profileRequests.Add(new ScreenCaptureRequest(layoutElement));
    }

    lock (this.sharedLock) {
      Debug.Assert(this.isRunning, "Must be running", "ScreenCaptureTimer");

      // 三つの場合で処理がわかれる
      // 1. profileにもcacheにも含まれている = nop
      // 2. cacheにはあるがprofileにはない = メモリ解放
      // 3. profileにはあるがcacheにはない = cacheに追加して即時更新
      var onlyCacheRequests = new HashSet<ScreenCaptureRequest>(this.cache.Keys);
      onlyCacheRequests.ExceptWith(profileRequests);
      var onlyProfileRequests = profileRequests; // もう使わないのでCloneしない
      onlyProfileRequests.ExceptWith(this.cache.Keys);

      // 2.
      var needGC = false;
      foreach (var request in onlyCacheRequests) {
        this.cache[request] = null;
        this.cache.Remove(request);
        needGC = true;
      }
      if (needGC) {
        Debug.Write("G");
        GC.Collect();
      }

      // 3.
      foreach (var request in onlyProfileRequests) {
        Debug.Write("x");
        // this.cache[request] = this.Capture(request);
        this.cache[request] = this.CaptureByGetDIBits(request);
      }
    }
  }

  /// 結果を取得
  /// @param layoutElement 対象のレイアウト要素
  /// @return 生成済みならBitmapSourceを。でなければNullを返す。
  public BitmapSource GetBitmapSource(ILayoutElementView layoutElement) {
    if (!layoutElement.IsWindowValid) return null;
    var request = new ScreenCaptureRequest(layoutElement);
    lock (this.sharedLock) {
      /// ディクショナリはスレッドセーフではない
      Debug.Assert(this.cache.ContainsKey(request), "No request in cache", "ScreenCaptureTimer");
      return this.cache[request];
    }
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
  private bool isRunning = false;
  
  /// 共有(自R/他W): リクエストと結果をまとめたディクショナリ
  private Dictionary<ScreenCaptureRequest, BitmapSource> cache =
      new Dictionary<ScreenCaptureRequest, BitmapSource>();
}
}   // namespace SCFF.GUI.Controls
