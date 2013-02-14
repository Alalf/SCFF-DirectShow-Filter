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
using SCFF.Common.Profile;

/// WYSIWYGレイアウト編集用UserControl
public partial class LayoutEdit
    : UserControl, IBindingProfile, IBindingOptions, IBindingRuntimeOptions {
  //===================================================================
  // 定数
  //===================================================================  

  /// カーソルをまとめたディクショナリ
  private static readonly Dictionary<HitModes,Cursor> HitModesToCursors =
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
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public LayoutEdit() {
    InitializeComponent();

    // できるだけ軽く
    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);

    // 再描画タイマーの準備
    App.ScreenCaptureTimer.Tick += ScreenCaptureTimer_Tick;
  }

  /// デストラクタ
  ~LayoutEdit() {
    App.ScreenCaptureTimer.Tick -= ScreenCaptureTimer_Tick;
  }

  //===================================================================
  // this.DrawingGroupへの描画
  //===================================================================

  /// キャプションのフォントサイズ
  private const int CaptionFontSize = 20;

  /// DrawBorder用のスタイル(Brush,Pen)を生成
  private void CreateBorderStyle(ILayoutElementView layoutElement,
      out Pen framePen, out Brush textBrush) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentIndex;

    // framePen/textBrush
    switch (layoutElement.WindowType) {
      case WindowTypes.Normal: {
        framePen  = isCurrent ? BrushesAndPens.CurrentNormalPen
                              : BrushesAndPens.NormalPen;
        textBrush = isCurrent ? BrushesAndPens.CurrentNormalBrush
                              : BrushesAndPens.NormalBrush;
        break;
      }
      case WindowTypes.DesktopListView: {
        framePen  = isCurrent ? BrushesAndPens.CurrentDesktopListViewPen
                              : BrushesAndPens.DesktopListViewPen;
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopListViewBrush
                              : BrushesAndPens.DesktopListViewBrush;
        break;
      }
      case WindowTypes.Desktop: {
        framePen  = isCurrent ? BrushesAndPens.CurrentDesktopPen
                              : BrushesAndPens.DesktopPen;
        textBrush = isCurrent ? BrushesAndPens.CurrentDesktopBrush
                              : BrushesAndPens.DesktopBrush;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// レイアウト要素のヘッダーの生成
  /// @param layoutElement 対象のレイアウト要素
  /// @param boundRect ヘッダー表示領域
  /// @param textBrush テキスト描画用ブラシ
  /// @return DrawingContext.DrawTextで描画可能なDrawingVisualオブジェクト
  private FormattedText CreateHeader(ILayoutElementView layoutElement,
      Rect boundRect, Brush textBrush) {
    var isCurrent = layoutElement.Index == App.Profile.CurrentIndex;
    var isDummy = !App.RuntimeOptions.IsCurrentProcessIDValid;

    // Caption
    var header = StringConverter.GetHeaderStringForLayoutEdit(layoutElement,
            isCurrent, isDummy,
            App.RuntimeOptions.CurrentSampleWidth,
            App.RuntimeOptions.CurrentSampleHeight);

    // FormattedText
    var formattedText = new FormattedText(header,
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        new Typeface("Segoe UI, Tahoma"),
        CaptionFontSize,
        textBrush);

    // Clipping
    formattedText.MaxTextWidth = boundRect.Width;
    formattedText.MaxTextHeight = boundRect.Height;
    formattedText.MaxLineCount = 1;
    return formattedText;
  }

  /// プレビュー画像の描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawPreview(DrawingContext dc, ILayoutElementView layoutElement) {
    var bitmap = App.ScreenCaptureTimer.GetBitmapSource(layoutElement);
    if (bitmap == null) {
      // エラーが起きた場合は赤色の四角形を描画
      var boundRect = layoutElement.GetBoundRect(
          App.RuntimeOptions.CurrentSampleWidth,
          App.RuntimeOptions.CurrentSampleHeight).ToRect();
      dc.DrawRectangle(Brushes.Red, null, boundRect);
    } else {
      // プレビューの描画
      var actualBoundRect = layoutElement.GetActualBoundRect(
          App.RuntimeOptions.CurrentSampleWidth,
          App.RuntimeOptions.CurrentSampleHeight).ToRect();
      dc.DrawImage(bitmap, actualBoundRect);
    }
  }

  /// 枠線とキャプションの描画
  /// @param dc 描画先
  /// @param layoutElement 描画対象
  private void DrawBorder(DrawingContext dc, ILayoutElementView layoutElement) {
    // フレームサイズ
    var boundRect = layoutElement.GetBoundRect(
        App.RuntimeOptions.CurrentSampleWidth,
        App.RuntimeOptions.CurrentSampleHeight).ToRect();

    // スタイルの取得
    Brush textBrush;
    Pen framePen;
    this.CreateBorderStyle(layoutElement, out framePen, out textBrush);

    // フレームの描画
    var inflateValue = framePen.Thickness / 2;
    var infraleRect = Rect.Inflate(boundRect, -inflateValue, -inflateValue);
    dc.DrawRectangle(null, framePen, infraleRect);

    // ヘッダーのドロップシャドウの描画
    var shadowOffset = 1.0;
    var shadowPoint = new Point(boundRect.X + shadowOffset, boundRect.Y + shadowOffset);
    var shadowBoundRect = new Rect(boundRect.X + shadowOffset,
                                   boundRect.Y + shadowOffset,
                                   boundRect.Width - shadowOffset,
                                   boundRect.Height - shadowOffset);
    var shadow = this.CreateHeader(layoutElement, shadowBoundRect, BrushesAndPens.DropShadowBrush);
    dc.DrawText(shadow, shadowPoint);

    // ヘッダーの描画
    var headerPoint = new Point(boundRect.X, boundRect.Y);
    var header = this.CreateHeader(layoutElement, boundRect, textBrush);
    dc.DrawText(header, headerPoint);
  }

  /// サンプルの幅・高さをRectに変換
  private Rect SampleRect {
    get {
      return new Rect(0.0, 0.0,
          (double)App.RuntimeOptions.CurrentSampleWidth,
          (double)App.RuntimeOptions.CurrentSampleHeight);
    }
  }

  /// DrawingGroupの生成
  ///
  /// @todo(me) 負荷軽減のためFPS制限してもいいかも(30FPSでだいたい半分だがカクカク)
  private void BuildDrawingGroup() {
    Debug.Assert(App.Options.IsLayoutVisible, "Must be shown", "LayoutEdit");

    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, this.SampleRect);

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

  /// DrawingGroupの消去
  private void ClearDrawingGroup() {
    if (this.DrawingGroup.Children.Count == 0) return;
    this.DrawingGroup.Children.Clear();
    GC.Collect();
    //GC.WaitForPendingFinalizers();
    //GC.Collect();
    Debug.WriteLine("Collect LayoutEdit.DrawingGroup", "*** MEMORY[GC] ***");
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// 再描画タイマーコールバック
  /// @param sender 使用しない
  /// @param e 使用しない
  void ScreenCaptureTimer_Tick(object sender, EventArgs e) {
    // プレビューが必要なければ更新しない
    if (!App.Options.IsLayoutVisible) return;
    if (!App.Options.LayoutPreview) return;
    
    // マウス操作中は更新しない
    if (this.moveAndSize.IsRunning) return;

    // プレビュー内容更新のためにリクエスト送信
    App.ScreenCaptureTimer.UpdateRequest(App.Profile);

    // 再描画
    /// @attention 浮動小数点数の比較
    Debug.WriteLineIf(this.LayoutEditImage.ActualWidth > App.RuntimeOptions.CurrentSampleWidth ||
                      this.LayoutEditImage.ActualHeight > App.RuntimeOptions.CurrentSampleHeight,
                      string.Format("Redraw ({0:F2}, {1:F2})",
                                    this.LayoutEditImage.ActualWidth,
                                    this.LayoutEditImage.ActualHeight),
                      "LayoutEdit");
    this.BuildDrawingGroup();
  }

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// マウス座標(LayoutEditImageのClient座標系)をサンプルサイズに対する相対座標系に変換
  private RelativePoint GetRelativeMousePoint(IInputElement image, MouseEventArgs e) {
    var mousePoint = e.GetPosition(image);
    var relativeX = mousePoint.X / App.RuntimeOptions.CurrentSampleWidth;
    var relativeY = mousePoint.Y / App.RuntimeOptions.CurrentSampleHeight;
    return new RelativePoint(relativeX, relativeY);
  }

  //-------------------------------------------------------------------

  /// LayoutEditImage: MouseDown
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseDown(object sender, MouseButtonEventArgs e) {
    // 左/右クリック以外はすぐに戻る
    if (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Right) return;
    
    // 前処理
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // HitTest
    int hitIndex;
    HitModes hitMode;
    if (!HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode)) return;

    // 現在選択中のIndexではない場合はそれに変更する
    if (hitIndex != App.Profile.CurrentIndex) {
      Debug.WriteLine(string.Format("CurrentIndex ({0:D}->{1:D})",
                                    App.Profile.CurrentIndex + 1, hitIndex + 1),
                      "LayoutEdit");

      App.Profile.CurrentIndex = hitIndex;

      //---------------------------------------------------------------
      // Notify self
      // Notify other controls
      Commands.ProfileStructureChanged.Execute(null, this);
      //---------------------------------------------------------------

      this.BuildDrawingGroup();
    }

    // 右クリックの場合はCurrentを変えただけで終了（残りはコンテキストメニューに任せる）
    if (e.ChangedButton == MouseButton.Right) return;

    // マウスを押した場所を記録してマウスキャプチャー開始
    var snapGuide = App.Options.LayoutSnap ? new SnapGuide(App.Profile) : null;
    this.moveAndSize.Start(App.Profile.CurrentView, hitMode, relativeMousePoint, snapGuide);

    // マウスキャプチャー開始
    image.CaptureMouse();

    // Profileを更新
    App.Profile.Current.Open();

    // CompositionTarget.Renderingの実行はコストが高いのでここだけで使う
    CompositionTarget.Rendering += CompositionTarget_Rendering;
  }

  /// CompositionTarget.Rendering
  /// @param sender 使用しない
  /// @param e 使用しない
  private void CompositionTarget_Rendering(object sender, EventArgs e) {
    // ドラッグ中で無ければ何もしない
    if (!this.moveAndSize.IsRunning) return;

    //-----------------------------------------------------------------
    // Notify self
    // Notify other controls
    if (this.moveAndSize.ShouldUpdateControl) {
      Commands.LayoutParameterChanged.Execute(null, this);
      this.moveAndSize.ShouldUpdateControl = false;
    }
    //-----------------------------------------------------------------

    this.BuildDrawingGroup();
  }


  /// LayoutEditImage: MouseMove
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
    // 前処理
    var image = (IInputElement)sender;
    var relativeMousePoint = this.GetRelativeMousePoint(image, e);

    // 動作中でなければカーソルを変更するだけ
    if (!this.moveAndSize.IsRunning) {
      int hitIndex;
      HitModes hitMode;
      HitTest.TryHitTest(App.Profile, relativeMousePoint, out hitIndex, out hitMode);
      this.Cursor = LayoutEdit.HitModesToCursors[hitMode];
      return;
    }

    // Move or Size
    this.moveAndSize.MousePoint = relativeMousePoint;
    var nextLTRB = this.moveAndSize.Do(Keyboard.Modifiers == ModifierKeys.Shift);
      
    App.Profile.Current.BoundRelativeLeft = nextLTRB.Left;
    App.Profile.Current.BoundRelativeTop = nextLTRB.Top;
    App.Profile.Current.BoundRelativeRight = nextLTRB.Right;
    App.Profile.Current.BoundRelativeBottom = nextLTRB.Bottom;

    // 描画自体はCompositionTarget.Renderingで行う
  }

  /// LayoutEditImage: MouseUp
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {
    if (!this.moveAndSize.IsRunning) return;
    this.moveAndSize.End();
    this.LayoutEditImage.ReleaseMouseCapture();
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.LayoutParameterChanged.Execute(null, this);
    //-----------------------------------------------------------------

    // CompositionTarget.Renderingの実行はコストが高いのでここだけで使う
    CompositionTarget.Rendering -= CompositionTarget_Rendering;
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

  /// @copydoc IBindingOptions::CanChangeOptions
  public bool CanChangeOptions { get; private set; }
  /// @copydoc IBindingOptions::OnOptionsChanged
  public void OnOptionsChanged() {
    this.CanChangeOptions = false;

    if (App.Options.IsLayoutVisible && App.Options.LayoutPreview) {
      App.ScreenCaptureTimer.Start();       // タイマー再開
      App.ScreenCaptureTimer.UpdateRequest(App.Profile);
      this.BuildDrawingGroup();
    } else if (App.Options.IsLayoutVisible && !App.Options.LayoutPreview) {
      App.ScreenCaptureTimer.Suspend();     // タイマー停止
      this.BuildDrawingGroup();
    } else if (!App.Options.IsLayoutVisible) {
      App.ScreenCaptureTimer.Suspend();     // タイマー停止
      this.ClearDrawingGroup();             // DrawingGroupもクリア
    }

    this.CanChangeOptions = true;
  }
  
  //===================================================================
  // IUpdateByRuntimeOptionsの実装
  //===================================================================

  /// @copydoc IBindingRuntimeOptions::CanChangeRuntimeOptions
  public bool CanChangeRuntimeOptions { get; private set; }
  /// @copydoc IBindingRuntimeOptions::OnRuntimeOptionsChanged
  public void OnRuntimeOptionsChanged() {
    this.CanChangeRuntimeOptions = false;

    // 再描画
    if (App.Options.IsLayoutVisible) this.BuildDrawingGroup();

    this.CanChangeRuntimeOptions = true;
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    // Currentのみの更新はできない
    this.OnProfileChanged();
  }
  /// @copydoc IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    this.CanChangeProfile = false;

    if (App.Options.IsLayoutVisible) {
      if (App.Options.LayoutPreview) {
        App.ScreenCaptureTimer.UpdateRequest(App.Profile);
      }
      this.BuildDrawingGroup();
    }

    this.CanChangeProfile = true;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// MoveAndSize
  private readonly MoveAndSize moveAndSize = new MoveAndSize();
}
}   // namespace SCFF.GUI.Controls
