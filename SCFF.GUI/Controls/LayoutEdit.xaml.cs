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

/// @file SCFF.GUI/Controls/LayoutEdit.xaml.cs
/// @copydoc SCFF::GUI::Controls::LayoutEdit

namespace SCFF.GUI.Controls {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using SCFF.Common;
using SCFF.Common.GUI;

/// WYSIWYGレイアウト編集用UserControl
///
/// LayoutEditImage内の座標系は([0-1]*Scale,[0-1]*Scale)で固定（プレビューのサイズに依存しない）
/// 逆に言うと依存させてはいけない
public partial class LayoutEdit : UserControl, IUpdateByProfile, IUpdateByOptions {
  //===================================================================
  // 定数
  //===================================================================  

  /// 相対座標→Image座標の倍率
  /// @warning 1.0のままやると値が小さすぎてフォントがバグるので100倍
  private const double Scale = 100.0;

  /// Imageの幅
  private const double MaxImageWidth = 1.0 * Scale;
  /// Imageの高さ
  private const double MaxImageHeight = 1.0 * Scale;
  /// Image座標系でのペンの太さ
  private const double PenThickness = 0.005 * Scale;
  /// Image座標系でのフォントサイズ
  private const double CaptionSize = 0.03 * Scale;
  /// Image座標系でのキャプションと枠線のマージン
  private const double CaptionMargin = PenThickness;

  /// カーソルをまとめたディクショナリ
  private static readonly Dictionary<HitModes, Cursor> HitModesToCursors =
      new Dictionary<HitModes,Cursor> {
    {HitModes.Neutral, null},
    {HitModes.Move, Cursors.SizeAll},
    {HitModes.SizeNW, Cursors.SizeNWSE},
    {HitModes.SizeNE, Cursors.SizeNESW},
    {HitModes.SizeSW, Cursors.SizeNESW},
    {HitModes.SizeSE, Cursors.SizeNWSE},
    {HitModes.SizeN, Cursors.SizeNS},
    {HitModes.SizeW, Cursors.SizeWE},
    {HitModes.SizeS, Cursors.SizeNS},
    {HitModes.SizeE, Cursors.SizeWE}
  };

  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  /// @todo(me) CompactViewで起動したときになぜかバックグラウンドでスレッドが実行されている
  public LayoutEdit() {
    InitializeComponent();
    this.Dispatcher.ShutdownStarted += OnShutdownStarted;

    /// @todo(me) App.RuntimeOptionsからの値の取得
    this.LayoutEditViewBox.Width = App.RuntimeOptions.CurrentSampleWidth;
    this.LayoutEditViewBox.Height = App.RuntimeOptions.CurrentSampleHeight;
    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);

    // Clipping
    // 重くないか？
    this.DrawingGroup.ClipGeometry = new RectangleGeometry(this.previewRect);
    this.DrawingGroup.ClipGeometry.Freeze();

    // スクリーンキャプチャマネージャの準備
    this.screenCaptureTimer = new ScreenCaptureTimer(redrawTimerPeriod);
    this.screenCaptureTimer.Init();

    // BitmapSource更新用タイマーの準備
    this.StartRedrawTimer();
  }

  /// Loaded
  void OnLoaded(object sender, RoutedEventArgs e) {
    Debug.WriteLine("LayoutEdit: OnLoaded");
  }

  /// Dispatcher.ShutdownStarted
  private void OnShutdownStarted(object sender, EventArgs e) {
    Debug.WriteLine("LayoutEdit: ShutdownStarted");
    this.screenCaptureTimer.End();
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

  /// レイアウト要素のキャプションの生成
  /// @param layoutElement 対象のレイアウト要素
  /// @return DrawingContext.DrawTextで描画可能なDrawingVisualオブジェクト
  private FormattedText CreateLayoutElementCaption(Profile.InputLayoutElement layoutElement) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentInputLayoutElement.Index;

    // Caption
    // サンプル: [1] (640x480) WindowCaption
    var layoutElementCaption = "[" + (layoutElement.Index+1) + "] "; 
    if (isCurrent) {
      /// @todo(me) ピクセル単位の幅と高さの出力
      layoutElementCaption += layoutElement.WindowCaption;
    } else {
      // Currentでなければ[1]以外は表示する必要はない
    }
    
    // Brush
    Brush textBrush = null;
    switch (layoutElement.WindowType) {
      case WindowTypes.Normal: {
        textBrush = isCurrent ? BrushesAndPens.CurrentNormalBrush
                              : BrushesAndPens.NormalBrush;
        break;
      }
      case WindowTypes.DesktopListView: {
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopListViewBrush
                              : BrushesAndPens.DesktopListViewBrush;
        break;
      }
      case WindowTypes.Desktop: {
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopBrush
                              : BrushesAndPens.DesktopBrush;
        break;
      }
    }

    // FormattedText
    var formattedText = new FormattedText(layoutElementCaption,
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        new Typeface("Meiryo"),
        CaptionSize,
        textBrush);
    formattedText.MaxTextWidth = layoutElement.BoundRelativeWidth * Scale;
    formattedText.MaxLineCount = 1;
    return formattedText;
  }

  /// プレビュー画像の描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawPreview(DrawingContext dc, Profile.InputLayoutElement layoutElement) {
    var bitmap = this.screenCaptureTimer.GetBitmapSource(layoutElement.Index);
    if (bitmap == null) return;

    // プレビューの描画
    var layoutElementRect = this.CreateLayoutElementRect(layoutElement);
    /// @todo(me) ピクセルベース計算じゃないと根本的におかしい！
    ///           なぜならばStretchの意味がなくなるから
    var aspect = this.LayoutEditViewBox.Width / this.LayoutEditViewBox.Height;
    double x, y, width, height;
    Common.Imaging.Utilities.CalculateLayout(
        layoutElementRect.Left, layoutElementRect.Top,
        layoutElementRect.Width, layoutElementRect.Height,
        layoutElement.ClippingWidthWithFit, layoutElement.ClippingHeightWithFit * aspect,
        layoutElement.Stretch, layoutElement.KeepAspectRatio,
        out x, out y, out width, out height);
    var actualLayoutElementRect = new Rect(x, y, width, height);
    dc.DrawImage(bitmap, actualLayoutElementRect);
  }

  /// 枠線とキャプションの描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawBorder(DrawingContext dc, Profile.InputLayoutElement layoutElement) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentInputLayoutElement.Index;

    // Pen
    Pen framePen = null;
    switch (layoutElement.WindowType) {
      case WindowTypes.Normal: {
        framePen = isCurrent ? BrushesAndPens.CurrentNormalPen
                             : BrushesAndPens.NormalPen;
        break;
      }
      case WindowTypes.DesktopListView: {
        framePen = isCurrent ? BrushesAndPens.CurrentDesktopListViewPen
                             : BrushesAndPens.DesktopListViewPen;
        break;
      }
      case WindowTypes.Desktop: {
        framePen = isCurrent ? BrushesAndPens.CurrentDesktopPen
                             : BrushesAndPens.DesktopPen;
        break;
      }
    }

    // フレームの描画
    var layoutElementRect = this.CreateLayoutElementRect(layoutElement);
    dc.DrawRectangle(Brushes.Transparent, framePen, layoutElementRect);

    // キャプションの描画
    var layoutElementCaption = this.CreateLayoutElementCaption(layoutElement);
    var captionPoint = new Point(layoutElementRect.X + CaptionMargin, layoutElementRect.Y + CaptionMargin);

    // キャプションから縁取りを取得
    /// @todo(me) 若干重い？
    var textGeometry = layoutElementCaption.BuildGeometry(captionPoint);
    dc.DrawGeometry(null, BrushesAndPens.DropShadowPen, textGeometry);

    dc.DrawText(layoutElementCaption, captionPoint);
  }

  /// プロファイル全体の描画
  /// @todo(me) FPS制限が必要かも？でもあんまりかわらないかも
  private void DrawProfile() {
    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, this.previewRect);

      // プレビューを下に描画
      if (App.Options.LayoutPreview) {
        foreach (var layoutElement in App.Profile) {
          this.DrawPreview(dc, layoutElement);
        }
      }

      // 枠線とキャプションを描画
      if (App.Options.LayoutBorder) {
        foreach (var layoutElement in App.Profile) {
          this.DrawBorder(dc, layoutElement);
        }
      }
    }
  }

  //===================================================================
  // DispatcherTimerによる再描画
  //===================================================================

  /// 再描画間隔: 5000ミリ秒
  private const double redrawTimerPeriod = 5000;
  /// 再描画用DispatcherTimer
  private DispatcherTimer redrawTimer = new DispatcherTimer();

  /// 再描画タイマー起動
  private void StartRedrawTimer() {
    redrawTimer.Interval = TimeSpan.FromMilliseconds(redrawTimerPeriod);
    redrawTimer.Tick += redrawTimer_Tick;
    redrawTimer.Start();
  }

  /// 再描画タイマーコールバック
  void redrawTimer_Tick(object sender, EventArgs e) {
    // プレビューが必要なければ更新しない
    if (!App.Options.LayoutIsExpanded) return;
    if (!App.Options.LayoutPreview) return;
    
    // マウス操作中は更新しない
    if (this.hitMode != HitModes.Neutral) return;

    // プレビュー更新のために再描画
    this.DrawProfile();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// マウス座標(image座標系)を(0.0-1.0, 0.0-1.0)のRelativePointに変換
  private RelativePoint GetRelativeMousePoint(IInputElement image, MouseEventArgs e) {
    var mousePoint = e.GetPosition(image);
    return new RelativePoint(mousePoint.X / MaxImageWidth, mousePoint.Y / MaxImageHeight);
  }

  /// LayoutEditImage: MouseDown
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseDown(object sender, MouseButtonEventArgs e) {
    // 前処理
    e.Handled = true;
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // HitTest
    int hitIndex;
    HitModes hitMode;
    if (!HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode)) return;

    // 現在選択中のIndexではない場合はそれに変更する
    if (hitIndex != App.Profile.CurrentInputLayoutElement.Index) {
      Debug.WriteLine("*****LayoutEdit: Change Current*****");
      Debug.WriteLine("{0:D}->{1:D} ({2:F2}, {3:F2})",
                      App.Profile.CurrentInputLayoutElement.Index,
                      hitIndex,
                      relativeMousePoint.X, relativeMousePoint.Y);

      App.Profile.ChangeCurrentIndex(hitIndex);
      UpdateCommands.UpdateMainWindowByEntireProfile.Execute(null, null);
    }

    // マウスを押した場所を記録してマウスキャプチャー開始
    this.hitMode = hitMode;
    this.relativeMouseOffset = new RelativeMouseOffset(App.Profile.CurrentInputLayoutElement, relativeMousePoint);
    if (App.Options.LayoutSnap) {
      this.snapGuide = new SnapGuide(App.Profile);
    } else {
      this.snapGuide = null;
    }
    image.CaptureMouse();

    this.DrawProfile();
  }

  /// LayoutEditImage: MouseMove
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
    // 前処理
    e.Handled = true;
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // Neutralのときだけはカーソルを帰るだけ
    if (this.hitMode == HitModes.Neutral) {
      // カーソルかえるだけ
      int hitIndex;
      HitModes hitMode;
      HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode);
      this.Cursor = LayoutEdit.HitModesToCursors[hitMode];
      return;
    }

    // Move or Size
    double nextLeft, nextTop, nextRight, nextBottom;
    MoveAndSize.MoveOrSize(App.Profile.CurrentInputLayoutElement, this.hitMode,
        relativeMousePoint, this.relativeMouseOffset, this.snapGuide, 
        out nextLeft, out nextTop, out nextRight, out nextBottom);

    // Profileを更新
    App.Profile.CurrentOutputLayoutElement.BoundRelativeLeft = nextLeft;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeTop = nextTop;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeRight = nextRight;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeBottom = nextBottom;
      
    /// @todo(me) 変更をMainWindowに通知
    UpdateCommands.UpdateLayoutParameterByCurrentProfile.Execute(null, null);

    this.DrawProfile();
  }

  /// LayoutEditImage: MouseUp
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {
    e.Handled = true;
    if (this.hitMode != HitModes.Neutral) {
      this.LayoutEditImage.ReleaseMouseCapture();
      this.hitMode = HitModes.Neutral;
      this.DrawProfile();
    }
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByOptions::UpdateByOptions
  public void UpdateByOptions() {
    if (App.Options.LayoutIsExpanded && App.Options.LayoutPreview) {
      this.screenCaptureTimer.Start();      // タイマー再開
      this.UpdateByEntireProfile();         // プレビュー強制更新
    } else {
      this.screenCaptureTimer.Suspend();    // タイマー停止
      if (App.Options.LayoutIsExpanded) {
        this.DrawProfile();                 // 再描画
      } else {
        this.DrawingGroup.Children.Clear(); // DrawingGroupもクリア
      }
      Debug.WriteLine("[GARBAGE COLLECT!]");
      GC.Collect();
    }
  }
  
  /// @copydoc IUpdateByOptions::DetachOptionsChangedEventHandlers
  public void DetachOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByOptions::AttachOptionsChangedEventHandlers
  public void AttachOptionsChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile::UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    this.SendRequest(App.Profile.CurrentInputLayoutElement, true);
    this.DrawProfile();
    Debug.WriteLine("[GARBAGE COLLECT!]");
    GC.Collect();
  }

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    this.screenCaptureTimer.ClearRequests();
    foreach (var layoutElement in App.Profile) {
      this.SendRequest(layoutElement, true);
    }
    this.DrawProfile();
    Debug.WriteLine("[GARBAGE COLLECT!]");
    GC.Collect();
  }

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void AttachProfileChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void DetachProfileChangedEventHandlers() {
    // nop
  }

  /// LayoutElementの内容からRequestを生成してScreenCapturerに画像生成を依頼
  private void SendRequest(Profile.InputLayoutElement layoutElement, bool forceUpdate) {
    var request = new ScreenCaptureRequest(
        layoutElement.Index,
        layoutElement.Window,
        layoutElement.WindowType == WindowTypes.Desktop ?
                        layoutElement.ScreenClippingXWithFit :
                        layoutElement.ClippingXWithFit,
        layoutElement.WindowType == WindowTypes.Desktop ?
                        layoutElement.ScreenClippingYWithFit :
                        layoutElement.ClippingYWithFit,
        layoutElement.ClippingWidthWithFit,
        layoutElement.ClippingHeightWithFit,
        layoutElement.ShowCursor,
        layoutElement.ShowLayeredWindow);
    this.screenCaptureTimer.SendRequest(request, forceUpdate);
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// プレビューサイズを決めるRect
  private Rect previewRect = new Rect(0.0, 0.0, MaxImageWidth, MaxImageHeight);

  /// スクリーンキャプチャ用スレッド管理クラスのインスタンス
  private ScreenCaptureTimer screenCaptureTimer = null;

  //-------------------------------------------------------------------
  // LayoutEditImage_MouseDown/Move/Up時の状態変数
  //-------------------------------------------------------------------

  /// マウス座標とLeft/Right/Top/BottomのOffset
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private RelativeMouseOffset relativeMouseOffset = null;
  /// スナップガイド
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private SnapGuide snapGuide = null;
  /// ヒットテストの結果
  /// @todo(me) MoveAndSizeStateとしてまとめられないだろうか？
  private HitModes hitMode = HitModes.Neutral;
}
}   // namespace SCFF.GUI.Controls
