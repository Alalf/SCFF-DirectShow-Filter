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

/// @file SCFF.GUI/Controls/LayoutParameter.cs
/// レイアウト配置設定

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Windows.Controls;

/// レイアウト配置設定
public partial class LayoutParameter : UserControl, IProfileToControl {

  /// コンストラクタ
  public LayoutParameter() {
    InitializeComponent();
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  /// @copydoc IProfileToControl.UpdateByProfile
  public void UpdateByProfile() {
    this.GroupBox.Header = "Layout " + (App.Profile.CurrentLayoutElement.Index+1) +
        ": " + App.Profile.CurrentLayoutElement.WindowCaption;

    /// @todo(me) プロセス情報はMainWindowから取ってこれるので、それを参照にしてBoundX/BoundYも更新
    this.BoundX.Text = App.Profile.CurrentLayoutElement.BoundLeft(Constants.DefaultPreviewWidth).ToString();
    this.BoundY.Text = App.Profile.CurrentLayoutElement.BoundTop(Constants.DefaultPreviewHeight).ToString();
    this.BoundWidth.Text = App.Profile.CurrentLayoutElement.BoundWidth(Constants.DefaultPreviewWidth).ToString();
    this.BoundHeight.Text = App.Profile.CurrentLayoutElement.BoundHeight(Constants.DefaultPreviewHeight).ToString();

    this.DetachChangedEventHandlers();

    this.BoundRelativeLeft.Text = App.Profile.CurrentLayoutElement.BoundRelativeLeft.ToString("F3");
    this.BoundRelativeTop.Text = App.Profile.CurrentLayoutElement.BoundRelativeTop.ToString("F3");
    this.BoundRelativeRight.Text = App.Profile.CurrentLayoutElement.BoundRelativeRight.ToString("F3");
    this.BoundRelativeBottom.Text = App.Profile.CurrentLayoutElement.BoundRelativeBottom.ToString("F3");

    this.AttachChangedEventHandlers();
  }

  /// @copydoc IProfileToControl.AttachChangedEventHandlers
  public void AttachChangedEventHandlers() {
    this.BoundRelativeLeft.TextChanged += boundRelativeLeft_TextChanged;
    this.BoundRelativeTop.TextChanged += boundRelativeTop_TextChanged;
    this.BoundRelativeRight.TextChanged += boundRelativeRight_TextChanged;
    this.BoundRelativeBottom.TextChanged += boundRelativeBottom_TextChanged;
  }

  /// @copydoc IProfileToControl.DetachChangedEventHandlers
  public void DetachChangedEventHandlers() {
    this.BoundRelativeLeft.TextChanged -= boundRelativeLeft_TextChanged;
    this.BoundRelativeTop.TextChanged -= boundRelativeTop_TextChanged;
    this.BoundRelativeRight.TextChanged -= boundRelativeRight_TextChanged;
    this.BoundRelativeBottom.TextChanged -= boundRelativeBottom_TextChanged;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------

  private bool TryParseBoundRelativeParameter(TextBox textBox, double lowerBound, double upperBound, out double parsedValue) {
    // Parse
    if (!double.TryParse(textBox.Text, out parsedValue)) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString("F3");
      return false;
    }

    // Validation
    if (parsedValue < lowerBound) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString("F3");
      return false;
    } else if (parsedValue > upperBound) {
      parsedValue = upperBound;
      textBox.Text = upperBound.ToString("F3");
      return false;
    }

    return true;
  }

  private void boundRelativeLeft_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeLeft, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.BoundRelativeLeft = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundX.Text = App.Profile.CurrentLayoutElement.BoundLeft(Constants.DummyPreviewWidth).ToString();
    }
  }

  private void boundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeTop, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.BoundRelativeTop = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundY.Text = App.Profile.CurrentLayoutElement.BoundTop(Constants.DummyPreviewHeight).ToString();
    }
  }

  private void boundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeRight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.BoundRelativeRight = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundWidth.Text = App.Profile.CurrentLayoutElement.BoundWidth(Constants.DummyPreviewWidth).ToString();
    }
  }

  private void boundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeBottom, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.BoundRelativeBottom = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundHeight.Text = App.Profile.CurrentLayoutElement.BoundHeight(Constants.DummyPreviewHeight).ToString();
    }
  }
}
}
