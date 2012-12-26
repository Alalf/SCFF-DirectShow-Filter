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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/// レイアウトエディタ
public partial class LayoutEdit : UserControl, IProfileToControl {

  /// コンストラクタ
  public LayoutEdit() {
    InitializeComponent();

    RenderOptions.SetBitmapScalingMode(this.DrawingGroup, BitmapScalingMode.LowQuality);
    this.DrawTest(string.Empty);
  }

  private void DrawLayout(DrawingContext dc, Profile.InputLayoutElement layoutElement) {
    //dc.DrawRectangle(Brushes.Black, null, new Rect(0,0,Constants.DefaultPreviewWidth,Constants.DefaultPreviewHeight));
    //dc.DrawRectangle(Brushes.DarkGray, null, new Rect(10,10,100,100));
    //if (text != string.Empty) {
    //  var formattedText = new FormattedText(text,
    //      System.Globalization.CultureInfo.CurrentUICulture,
    //      FlowDirection.LeftToRight,
    //      new Typeface("Meiryo"),
    //      10,
    //      Brushes.White);
    //  dc.DrawText(formattedText, new Point(10,200));
    //}
  }

  /// 描画テスト用
  private void DrawTest(string text) {
    

    using (var dc = this.DrawingGroup.Open()) {
      // 背景描画でサイズを決める
      dc.DrawRectangle(Brushes.Black, null, new Rect(0,0,Constants.DefaultPreviewWidth,Constants.DefaultPreviewHeight));

      foreach (var layoutElement in App.Profile) {
        DrawLayout(dc, layoutElement);
      }
    }
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  public void UpdateByProfile() {
    this.DrawTest("Update");
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
    this.DrawTest(x + ", " + y);
  }

  private void LayoutEditImage_MouseMove(object sender, MouseEventArgs e) {
      // nop
  }

  private void LayoutEditImage_MouseUp(object sender, MouseButtonEventArgs e) {

  }
}
}
