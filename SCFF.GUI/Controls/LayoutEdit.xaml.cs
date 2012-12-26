// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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
/// レイアウトエディタ

namespace SCFF.GUI.Controls {

  using SCFF.Common;
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;

/// レイアウトエディタ
///
/// LayoutEditImage内の座標系は([0-100],[0-100])で固定（プレビューのサイズに依存しない）
/// 逆に言うと依存させてはいけない
public partial class LayoutEdit : UserControl, IProfileToControl {

  private const double MaxImageWidth = 100.0;
  private const double MaxImageHeight = 100.0;
  private const double PenThickness = 0.5;
  private const double CaptionSize = 4.0;
  private const double CaptionMargin = 0.5;

  private Pen dummyPen = new Pen(Brushes.DarkOrange, PenThickness);

  /// コンストラクタ
  public LayoutEdit() {
    InitializeComponent();

    this.LayoutEditViewBox.Width = Constants.DummyPreviewWidth;
    this.LayoutEditViewBox.Height = Constants.DummyPreviewHeight;
    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);
    this.DrawTest();
  }

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
    if (App.Options.LayoutBorder) {
      // フレームの描画
      var layoutElementRect = this.CreateLayoutElementRect(layoutElement);
      dc.DrawRectangle(Brushes.Transparent, dummyPen, layoutElementRect);

      // キャプションの描画
      var layoutElementCaption = this.CreateLayoutElementCaption(layoutElement);
      var captionPoint = new Point(layoutElementRect.X + CaptionMargin, layoutElementRect.Y + CaptionMargin);
      dc.DrawText(layoutElementCaption, captionPoint);
    }
  }

  /// 描画テスト用
  private void DrawTest() {
    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, new Rect(0,0,MaxImageWidth,MaxImageHeight));

      foreach (var layoutElement in App.Profile) {
        DrawLayout(dc, layoutElement);
      }
    }
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  public void UpdateByProfile() {
    this.DrawTest();
  }

  public void AttachChangedEventHandlers() {
    // nop
  }

  public void DetachChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void LayoutEditImage_MouseDown(object sender, MouseButtonEventArgs e) {
    var pt = e.GetPosition((IInputElement)sender);
    var x = (int)pt.X;
    var y = (int)pt.Y;
    Debug.WriteLine("MouseDown: " + x + ", " + y);
    this.DrawTest();
  }

  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
      // nop
  }

  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {

  }
}
}
