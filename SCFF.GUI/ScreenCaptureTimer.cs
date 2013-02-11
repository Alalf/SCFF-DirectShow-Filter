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
using SCFF.Common.GUI;
using SCFF.Common.Profile;
  
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
    this.SharedLock   = new object();
    this.IsRunning    = false;
    this.Cache        = new Dictionary<ScreenCaptureRequest,BitmapSource>();
    this.TimerPeriod  = ScreenCaptureTimer.DefaultTimerPeriod;

    var context = SynchronizationContext.Current; // UIThread
    this.CaptureTimer = new Timer(this.TimerCallback, context, 0, (int)this.TimerPeriod);

    Debug.WriteLine("ScreenCaptureTimer.CaptureTimer", "*** MEMORY[NEW] ***");
  }

  /// Dispose
  public void Dispose() {
    lock (this.SharedLock) {
      this.IsRunning = false;
      if (this.CaptureTimer != null) {
        this.CaptureTimer.Change(Timeout.Infinite, Timeout.Infinite);
        this.CaptureTimer.Dispose();
        this.CaptureTimer = null;
        Debug.WriteLine("ScreenCaptureTimer.CaptureTimer", "*** MEMORY[DISPOSE] ***");
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
    if (request.ClippingWidth <= 0 || request.ClippingHeight <= 0) {
      // Debug.WriteLine("Invalid clipping size", "ScreenCaptureTimer.CaptureByGetDIBits");
      return null;
    }

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
    if (request.ClippingWidth <= 0 || request.ClippingHeight <= 0) {
      // Debug.WriteLine("Invalid clipping size", "ScreenCaptureTimer.CaptureByGetDIBits");
      return null;
    }
    
    // GetDIBitsでbyte[]にデータを格納
    var result = request.ExecuteByGetDIBits();
    if (result == null) return null;

    var bitmap = BitmapSource.Create(request.ClippingWidth, request.ClippingHeight,
                                 96.0, 96.0, PixelFormats.Bgr32, null, result, request.Stride);

    result = null;
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
    lock (this.SharedLock) {
      if (!this.IsRunning) return;
      
      var requests = new List<ScreenCaptureRequest>(this.Cache.Keys);
      foreach (var request in requests) {
        // var bitmapSource = this.Capture(request);
        var bitmapSource = this.CaptureByGetDIBits(request);
        Debug.WriteIf(bitmapSource != null, "o");
        this.Cache[request] = bitmapSource;
      }
      /// @todo(me) 潔癖症過ぎないか？
      // GC.Collect();
      // Debug.Write("G");
    }

    // Event: Tick(UI Thread)
    var context = state as SynchronizationContext;
    context.Post((s) => {
      var handler = this.Tick;
      if (handler != null) handler(this, EventArgs.Empty);
    }, null);
  }

  //===================================================================
  // 外部インタフェース
  //===================================================================

  //-------------------------------------------------------------------
  // スレッドの動作関連
  //-------------------------------------------------------------------

  /// 開始
  public void Start() {
    lock (this.SharedLock) {
      var changed = !this.IsRunning;
      Debug.WriteLineIf(changed, "Start", "ScreenCaptureTimer");
      this.IsRunning = true;
    }
  }

  /// 中断
  public void Suspend() {
    lock (this.SharedLock) {
      var changed = this.IsRunning;
      Debug.WriteLineIf(changed, "Suspend", "ScreenCaptureTimer");
      this.IsRunning = false;
      // メモリ解放
      if (changed && this.Cache.Count > 0) {
        this.Cache.Clear();
        GC.Collect();
        //GC.WaitForPendingFinalizers();
        //GC.Collect();
        Debug.WriteLine("Collect ScreenCaptureTimer.Cache", "*** MEMORY[GC] ***");
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

    lock (this.SharedLock) {
      Debug.Assert(this.IsRunning, "Must be running", "ScreenCaptureTimer");

      // 三つの場合で処理がわかれる
      // 1. profileにもCacheにも含まれている = nop
      // 2. Cacheにはあるがprofileにはない = メモリ解放
      // 3. profileにはあるがCacheにはない = Cacheに追加して即時更新
      var onlyCacheRequests = new HashSet<ScreenCaptureRequest>(this.Cache.Keys);
      onlyCacheRequests.ExceptWith(profileRequests);
      var onlyProfileRequests = profileRequests; // もう使わないのでCloneしない
      onlyProfileRequests.ExceptWith(this.Cache.Keys);

      // 2.
      var needGC = false;
      foreach (var request in onlyCacheRequests) {
        this.Cache[request] = null;
        var result = this.Cache.Remove(request);
        Debug.WriteLineIf(!result, "!");
        needGC = true;
      }
      if (needGC) {
        Debug.Write("G");
        GC.Collect();
        //GC.WaitForPendingFinalizers();
        //GC.Collect();
      }

      // 3.
      foreach (var request in onlyProfileRequests) {
        // var bitmapSource = this.Capture(request);
        var bitmapSource = this.CaptureByGetDIBits(request);
        Debug.WriteIf(bitmapSource != null, "x");
        this.Cache[request] = bitmapSource;
      }
    }
  }
  
  /// 結果を取得
  /// @param layoutElement 対象のレイアウト要素
  /// @return 生成済みならBitmapSourceを。でなければNullを返す。
  public BitmapSource GetBitmapSource(ILayoutElementView layoutElement) {
    if (!layoutElement.IsWindowValid) return null;
    var request = new ScreenCaptureRequest(layoutElement);

    /// ディクショナリはスレッドセーフではない
    lock (this.SharedLock) {
      BitmapSource result;
      var success = this.Cache.TryGetValue(request, out result);
      Debug.WriteLineIf(!success, "No request in Cache", "ScreenCaptureTimer");
      return result;
    }
  }

  //===================================================================
  // イベント
  //===================================================================

  /// キャプチャ終了後
  public event EventHandler Tick;

  //===================================================================
  // プロパティ
  //===================================================================

  //-------------------------------------------------------------------
  // 複数のスレッドで共有するデータ
  //-------------------------------------------------------------------

  /// 共有ロック
  public object SharedLock { get; set; }

  /// 共有(自R/他W): プレビュー表示中かどうか
  private bool IsRunning { get; set; }
  
  /// 共有(自R/他W): リクエストと結果をまとめたディクショナリ
  private Dictionary<ScreenCaptureRequest,BitmapSource> Cache { get; set; }
  
  /// 共有(自R/他W): タイマーの間隔
  private double TimerPeriod { get; set; }

  //-------------------------------------------------------------------

  /// タイマー
  private Timer CaptureTimer { get; set; }
}
}   // namespace SCFF.GUI.Controls
