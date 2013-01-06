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
/// 数値を指定してレイアウト配置を調整するためのUserControl

namespace SCFF.GUI.Controls {

using System;
using System.Windows.Controls;
using SCFF.Common;

/// 数値を指定してレイアウト配置を調整するためのUserControl
public partial class LayoutParameter : UserControl, IUpdateByProfile {
  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutParameter() {
    InitializeComponent();
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

  /// 下限・上限つきでテキストボックスから値を取得する
  private bool TryParseBoundRelativeParameter(TextBox textBox,
      double lowerBound, double upperBound, out double parsedValue) {
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

  /// BoundRelativeLeft: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void BoundRelativeLeft_TextChanged(object sender, TextChangedEventArgs e) {
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

  /// BoundRelativeTop: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void BoundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {
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

  /// BoundRelativeRight: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void BoundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {
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

  /// BoundRelativeBottom: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void BoundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {
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

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// GroupBox.Headerの最大文字数
  private const int MaxHeaderLength = 60;

  /// @copydoc IUpdateByProfile::UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    var header = string.Format("Layout {0:D}: {1}",
        App.Profile.CurrentInputLayoutElement.Index + 1,
        App.Profile.CurrentInputLayoutElement.WindowCaption);
    this.GroupBox.Header = header.Substring(0, Math.Min(header.Length, MaxHeaderLength));

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

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    // 編集するのはCurrentのみ
    this.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile::AttachProfileChangedEventHandlers
  public void AttachProfileChangedEventHandlers() {
    this.BoundRelativeLeft.TextChanged += BoundRelativeLeft_TextChanged;
    this.BoundRelativeTop.TextChanged += BoundRelativeTop_TextChanged;
    this.BoundRelativeRight.TextChanged += BoundRelativeRight_TextChanged;
    this.BoundRelativeBottom.TextChanged += BoundRelativeBottom_TextChanged;
  }

  /// @copydoc IUpdateByProfile::DetachProfileChangedEventHandlers
  public void DetachProfileChangedEventHandlers() {
    this.BoundRelativeLeft.TextChanged -= BoundRelativeLeft_TextChanged;
    this.BoundRelativeTop.TextChanged -= BoundRelativeTop_TextChanged;
    this.BoundRelativeRight.TextChanged -= BoundRelativeRight_TextChanged;
    this.BoundRelativeBottom.TextChanged -= BoundRelativeBottom_TextChanged;
  }
}
}   // namespace SCFF.GUI.Controls
