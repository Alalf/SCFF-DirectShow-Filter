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

/// @file SCFF.GUI/Controls/LayoutEdit.cs
/// レイアウトエディタコントロール

namespace SCFF.GUI.Controls {

using SCFF.Common;
using SCFF.Common.Ext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

/// レイアウトエディタコントロール
///
/// LayoutEditImage内の座標系は([0-100],[0-100])で固定（プレビューのサイズに依存しない）
/// 逆に言うと依存させてはいけない
public partial class LayoutEdit : UserControl, IUpdateByProfile, IUpdateByOptions {

  //===================================================================
  // 定数
  //===================================================================  

  // 1.0のままやると値が小さすぎてフォントがバグるので100倍
  private const double Scale = 100.0;

  private const double MaxImageWidth = 1.0 * Scale;
  private const double MaxImageHeight = 1.0 * Scale;
  private const double PenThickness = 0.005 * Scale;
  private const double CaptionSize = 0.04 * Scale;
  private const double CaptionMargin = PenThickness;

  //===================================================================
  // privateメンバ
  //===================================================================

  private Pen dummyPen = new Pen(Brushes.DarkOrange, PenThickness);
  private Rect previewRect = new Rect(0.0, 0.0, MaxImageWidth, MaxImageHeight);

  /// スクリーンキャプチャ用スレッド管理クラスのインスタンス
  private Common.GUI.ScreenCapturer screenCapturer = null;

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutEdit() {
    InitializeComponent();
    this.Dispatcher.ShutdownStarted += OnShutdownStarted;

    this.LayoutEditViewBox.Width = Constants.DummyPreviewWidth;
    this.LayoutEditViewBox.Height = Constants.DummyPreviewHeight;
    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);

    // Clipping
    // 重くないか？
    this.DrawingGroup.ClipGeometry = new RectangleGeometry(this.previewRect);
    this.DrawingGroup.ClipGeometry.Freeze();

    // 使いまわすリソースはFreezeしておくとパフォーマンスがあがる
    this.dummyPen.Freeze();

    // スクリーンキャプチャマネージャの準備
    this.screenCapturer = new Common.GUI.ScreenCapturer(bitmapsUpdateTimerPeriod);
    this.screenCapturer.Start();

    // BitmapSource更新用タイマーの準備
    this.StartBitmapsUpdateTimer();

    // 最初に更新
    this.UpdateByEntireProfile();
  }

  /// Loadedイベントハンドラ
  void OnLoaded(object sender, RoutedEventArgs e) {
    Debug.WriteLine("LayoutEdit: OnLoaded");
  }

  /// Dispatcher.ShutdownStartedイベントハンドラ
  private void OnShutdownStarted(object sender, EventArgs e) {
    Debug.WriteLine("LayoutEdit: ShutdownStarted");
    this.screenCapturer.End();
  }

  //===================================================================
  // this.DrawingGroupへの描画
  //===================================================================

  /// レイアウト要素の描画範囲を求める
  private Rect CreateLayoutElementRect(Profile.InputLayoutElement layoutElement) {
    return new Rect() {
      X = layoutElement.BoundRelativeLeft * MaxImageWidth,
      Y = layoutElement.BoundRelativeTop * MaxImageHeight,
      Width = (layoutElement.BoundRelativeRight - layoutElement.BoundRelativeLeft) * MaxImageWidth,
      Height = (layoutElement.BoundRelativeBottom - layoutElement.BoundRelativeTop) * MaxImageHeight
    };
  }

  private FormattedText CreateLayoutElementCaption(Profile.InputLayoutElement layoutElement) {
    // 1: WindowCaption [(640x400) あれば]
    /// @todo(me) ピクセル単位の幅と高さの出力
    var layoutElementCaption = (layoutElement.Index+1).ToString() +
        ": " + layoutElement.WindowCaption;
  
    return new FormattedText(layoutElementCaption,
      System.Globalization.CultureInfo.CurrentUICulture,
      FlowDirection.LeftToRight,
      new Typeface("Meiryo"),
      CaptionSize,
      Brushes.DarkOrange);
  }

  private void DrawLayout(DrawingContext dc, Profile.InputLayoutElement layoutElement) {
    var layoutElementRect = this.CreateLayoutElementRect(layoutElement);

    if (App.Options.LayoutPreview && this.capturedBitmaps[layoutElement.Index] != null) {
      dc.DrawImage(this.capturedBitmaps[layoutElement.Index], layoutElementRect);
    }

    if (App.Options.LayoutBorder) {
      // フレームの描画
      dc.DrawRectangle(Brushes.Transparent, dummyPen, layoutElementRect);

      // キャプションの描画
      var layoutElementCaption = this.CreateLayoutElementCaption(layoutElement);
      layoutElementCaption.MaxTextWidth = layoutElement.BoundRelativeWidth * Scale;
      layoutElementCaption.MaxLineCount = 1;
      var captionPoint = new Point(layoutElementRect.X + CaptionMargin, layoutElementRect.Y + CaptionMargin);
      dc.DrawText(layoutElementCaption, captionPoint);
    }
  }

  /// 描画テスト用
  /// @todo(me) FPS制限が必要かも？でもあんまりかわらないかも
  private void DrawProfile() {
    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, this.previewRect);

      foreach (var layoutElement in App.Profile) {
        DrawLayout(dc, layoutElement);
      }
    }
  }

  //===================================================================
  // DispatcherTimerによるBitmapSourceの更新
  //===================================================================

  /// BitmapSource更新間隔: 3000ミリ秒
  private const double bitmapsUpdateTimerPeriod = 3000;
  /// BitmapSourceを更新するためのDispatcherTimer
  private DispatcherTimer bitmapsUpdateTimer = new DispatcherTimer();
  /// ScreenCapturer から受け取ったデータを格納しておく
  private BitmapSource[] capturedBitmaps = new BitmapSource[Constants.MaxLayoutElementCount];

  private void StartBitmapsUpdateTimer() {
    bitmapsUpdateTimer.Interval = TimeSpan.FromMilliseconds(bitmapsUpdateTimerPeriod);
    bitmapsUpdateTimer.Tick += bitmapsUpdateTimer_Tick;
    bitmapsUpdateTimer.Start();
  }

  private BitmapSource CreateBitmapSource(int index) {
    var result = this.screenCapturer.GetResult(index);
    if (result == null) return null;
    return BitmapSource.Create(
        result.PixelWidth, result.PixelHeight,
        result.DpiX, result.DpiY,
        PixelFormats.Bgr32, null,
        result.Pixels, result.Stride);
  }

  void bitmapsUpdateTimer_Tick(object sender, EventArgs e) {
    this.capturedBitmaps = new BitmapSource[Constants.MaxLayoutElementCount];
    foreach (var layoutElement in App.Profile) {
      this.capturedBitmaps[layoutElement.Index] =
          this.CreateBitmapSource(layoutElement.Index);
    }
    // プレビュー更新
    this.DrawProfile();
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile.UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    this.SendRequestToScreenCapturer(App.Profile.CurrentInputLayoutElement);
    this.DrawProfile();
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    foreach (var layoutElement in App.Profile) {
      this.SendRequestToScreenCapturer(layoutElement);
    }
    this.DrawProfile();
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void AttachProfileChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void DetachProfileChangedEventHandlers() {
    // nop
  }

  /// LayoutElementの内容からRequestを生成してScreenCapturerに画像生成を依頼
  private void SendRequestToScreenCapturer(Profile.InputLayoutElement layoutElement) {
    var request = new Common.GUI.ScreenCaptureRequest {
      Index = layoutElement.Index,
      Window = layoutElement.Window,
      ClippingX = layoutElement.WindowType == WindowTypes.Desktop ?
                      layoutElement.ScreenClippingXWithFit :
                      layoutElement.ClippingXWithFit,
      ClippingY = layoutElement.WindowType == WindowTypes.Desktop ?
                      layoutElement.ScreenClippingYWithFit :
                      layoutElement.ClippingYWithFit,
      ClippingWidth = layoutElement.ClippingWidthWithFit,
      ClippingHeight = layoutElement.ClippingHeightWithFit,
      ShowCursor = layoutElement.ShowCursor,
      ShowLayeredWindow = layoutElement.ShowLayeredWindow
    };
    this.screenCapturer.SendRequest(request);
  }

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByOptions.UpdateByOptions
  public void UpdateByOptions() {
    if (App.Options.TmpLayoutIsExpanded) {
      this.screenCapturer.Resume();
    } else {
      this.screenCapturer.Suspend();
    }
  }
  
  /// @copydoc IUpdateByOptions.DetachOptionsChangedEventHandlers
  public void DetachOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByOptions.AttachOptionsChangedEventHandlers
  public void AttachOptionsChangedEventHandlers() {
    // nop
  }


  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// カーソルをまとめたディクショナリ
  public readonly Dictionary<Common.GUI.HitModes, Cursor> hitModesToCursors =
      new Dictionary<Common.GUI.HitModes,Cursor> {
    {Common.GUI.HitModes.Neutral, null},
    {Common.GUI.HitModes.Move, Cursors.SizeAll},
    {Common.GUI.HitModes.SizeNW, Cursors.SizeNWSE},
    {Common.GUI.HitModes.SizeNE, Cursors.SizeNESW},
    {Common.GUI.HitModes.SizeSW, Cursors.SizeNESW},
    {Common.GUI.HitModes.SizeSE, Cursors.SizeNWSE},
    {Common.GUI.HitModes.SizeN, Cursors.SizeNS},
    {Common.GUI.HitModes.SizeW, Cursors.SizeWE},
    {Common.GUI.HitModes.SizeS, Cursors.SizeNS},
    {Common.GUI.HitModes.SizeE, Cursors.SizeWE}
  };

  /// マウスポインタとLeft/Right/Top/BottomのOffset
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private Common.GUI.RelativeMouseOffset relativeMouseOffset = null;
  /// スナップガイド
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private Common.GUI.SnapGuide snapGuide = null;
  /// ヒットテストの結果
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private Common.GUI.HitModes hitMode = Common.GUI.HitModes.Neutral;

  /// マウスポインタを(0.0-1.0, 0.0-1.0)のCommon.GUI.Pointに変換
  private Common.GUI.Point GetRelativeMousePoint(IInputElement image, MouseEventArgs e) {
    var mousePoint = e.GetPosition(image);
    return new Common.GUI.Point(mousePoint.X / MaxImageWidth, mousePoint.Y / MaxImageHeight);
  }

  /// MouseDownイベントハンドラ 
  private void LayoutEditImage_MouseDown(object sender, MouseButtonEventArgs e) {
    // 前処理
    e.Handled = true;
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // HitTest
    int hitIndex;
    Common.GUI.HitModes hitMode;
    if (!Common.GUI.HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode)) return;

    // 現在選択中のIndexではない場合はそれに変更する
    if (hitIndex != App.Profile.CurrentInputLayoutElement.Index) {
      Debug.WriteLine("*****LayoutEdit: Change Current*****");
      Debug.WriteLine("{0:D}->{1:D} ({2:F2}, {3:F2})",
                      App.Profile.CurrentInputLayoutElement.Index,
                      hitIndex,
                      relativeMousePoint.X, relativeMousePoint.Y);

      App.Profile.ChangeCurrentIndex(hitIndex);
      Commands.ChangeCurrentLayoutElementCommand.Execute(null, null);
    }

    // マウスを押した場所を記録してマウスキャプチャー開始
    this.hitMode = hitMode;
    this.relativeMouseOffset = new Common.GUI.RelativeMouseOffset(App.Profile.CurrentInputLayoutElement, relativeMousePoint);
    this.snapGuide = new Common.GUI.SnapGuide(App.Profile, App.Options.LayoutSnap);
    image.CaptureMouse();

    this.DrawProfile();
  }

  /// MouseMoveイベントハンドラ
  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
    // 前処理
    e.Handled = true;
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // Neutralのときだけはカーソルを帰るだけ
    if (this.hitMode == Common.GUI.HitModes.Neutral) {
      // カーソルかえるだけ
      int hitIndex;
      Common.GUI.HitModes hitMode;
      Common.GUI.HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode);
      this.Cursor = this.hitModesToCursors[hitMode];
      return;
    }

    // Move/Size*
    double nextLeft = -1.0;
    double nextTop = -1.0;
    double nextRight = -1.0;
    double nextBottom = -1.0;

    if (this.hitMode == Common.GUI.HitModes.Move) {
      // Move
      Common.GUI.MoveAndSize.Move(App.Profile.CurrentInputLayoutElement,
          relativeMousePoint, this.relativeMouseOffset, this.snapGuide,
          out nextLeft, out nextTop, out nextRight, out nextBottom);
    } else {
      // Size*
      Common.GUI.MoveAndSize.Size(App.Profile.CurrentInputLayoutElement,
          this.hitMode, relativeMousePoint, this.relativeMouseOffset, this.snapGuide,
          out nextLeft, out nextTop, out nextRight, out nextBottom);
    }

    // Profileを更新
    App.Profile.CurrentOutputLayoutElement.BoundRelativeLeft = nextLeft;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeTop = nextTop;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeRight = nextRight;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeBottom = nextBottom;
      
    /// @todo(me) 変更をMainWindowに通知
    Commands.ChangeLayoutParameterCommand.Execute(null, null);

    this.DrawProfile();
  }

  /// MouseUpイベントハンドラ
  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {
    e.Handled = true;
    if (this.hitMode != Common.GUI.HitModes.Neutral) {
      this.LayoutEditImage.ReleaseMouseCapture();
      this.hitMode = Common.GUI.HitModes.Neutral;
      this.DrawProfile();
    }
  }
}
}
