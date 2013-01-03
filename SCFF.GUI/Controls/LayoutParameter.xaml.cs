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

/// @file SCFF.GUI/Controls/LayoutParameter.xaml.cs
/// レイアウト配置設定

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Windows.Controls;

/// レイアウト配置設定
public partial class LayoutParameter : UserControl, IUpdateByProfile {

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutParameter() {
    InitializeComponent();
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile.UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    var header = "Layout " + (App.Profile.CurrentInputLayoutElement.Index+1) +
        ": " + App.Profile.CurrentInputLayoutElement.WindowCaption;
    if (header.Length > 60) {
      this.GroupBox.Header = header.Substring(0, 60);
    } else {
      this.GroupBox.Header = header;
    }

    /// @todo(me) プロセス情報はMainWindowから取ってこれるので、それを参照にしてBoundX/BoundYも更新
    this.BoundX.Text = App.Profile.CurrentInputLayoutElement.BoundLeft(Constants.DefaultPreviewWidth).ToString();
    this.BoundY.Text = App.Profile.CurrentInputLayoutElement.BoundTop(Constants.DefaultPreviewHeight).ToString();
    this.BoundWidth.Text = App.Profile.CurrentInputLayoutElement.BoundWidth(Constants.DefaultPreviewWidth).ToString();
    this.BoundHeight.Text = App.Profile.CurrentInputLayoutElement.BoundHeight(Constants.DefaultPreviewHeight).ToString();

    this.DetachProfileChangedEventHandlers();

    this.BoundRelativeLeft.Text = App.Profile.CurrentInputLayoutElement.BoundRelativeLeft.ToString("F3");
    this.BoundRelativeTop.Text = App.Profile.CurrentInputLayoutElement.BoundRelativeTop.ToString("F3");
    this.BoundRelativeRight.Text = App.Profile.CurrentInputLayoutElement.BoundRelativeRight.ToString("F3");
    this.BoundRelativeBottom.Text = App.Profile.CurrentInputLayoutElement.BoundRelativeBottom.ToString("F3");

    this.AttachProfileChangedEventHandlers();

    var isComplexLayout = App.Profile.LayoutType == LayoutTypes.ComplexLayout;
    this.BoundRelativeLeft.IsEnabled = isComplexLayout;
    this.BoundRelativeTop.IsEnabled = isComplexLayout;
    this.BoundRelativeRight.IsEnabled = isComplexLayout;
    this.BoundRelativeBottom.IsEnabled = isComplexLayout;
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    // 編集するのはCurrentのみ
    this.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile.AttachProfileChangedEventHandlers
  public void AttachProfileChangedEventHandlers() {
    this.BoundRelativeLeft.TextChanged += boundRelativeLeft_TextChanged;
    this.BoundRelativeTop.TextChanged += boundRelativeTop_TextChanged;
    this.BoundRelativeRight.TextChanged += boundRelativeRight_TextChanged;
    this.BoundRelativeBottom.TextChanged += boundRelativeBottom_TextChanged;
  }

  /// @copydoc IUpdateByProfile.DetachProfileChangedEventHandlers
  public void DetachProfileChangedEventHandlers() {
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
      App.Profile.CurrentOutputLayoutElement.BoundRelativeLeft = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundX.Text = App.Profile.CurrentInputLayoutElement.BoundLeft(Constants.DummyPreviewWidth).ToString();
    }
  }

  private void boundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeTop, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.BoundRelativeTop = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundY.Text = App.Profile.CurrentInputLayoutElement.BoundTop(Constants.DummyPreviewHeight).ToString();
    }
  }

  private void boundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeRight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.BoundRelativeRight = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundWidth.Text = App.Profile.CurrentInputLayoutElement.BoundWidth(Constants.DummyPreviewWidth).ToString();
    }
  }

  private void boundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0;
    var upperBound = 1.0;
    double parsedValue;
    if (this.TryParseBoundRelativeParameter(this.BoundRelativeBottom, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.BoundRelativeBottom = parsedValue;
      // Changedイベントで発動するものはないのでそのまま代入
      /// @todo(me) ダミープレビューサイズ
      this.BoundHeight.Text = App.Profile.CurrentInputLayoutElement.BoundHeight(Constants.DummyPreviewHeight).ToString();
    }
  }
}
}
