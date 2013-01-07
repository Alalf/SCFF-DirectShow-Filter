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
/// @copydoc SCFF::GUI::Controls::LayoutParameter

namespace SCFF.GUI.Controls {

using System;
using System.Windows.Controls;
using SCFF.Common;

/// 数値を指定してレイアウト配置を調整するためのUserControl
public partial class LayoutParameter
    : UserControl, IUpdateByProfile, IUpdateByRuntimeOptions {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
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

  /// Fit: Click
  /// @param sender 使用しない
  /// @param e
  private void Fit_Click(object sender, System.Windows.RoutedEventArgs e) {
    /// 現在選択中のレイアウト要素のアスペクト比をあわせ、黒帯を消す
    /// @todo(me) コンテキストメニューにも実装したいのでCommand化したい。
    ///           というか結構めんどくさくないか？これ。むしろAdobeっぽく
    //            Shiftドラッグで比率維持とかやったほうがいいと思うんだけど
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// BoundX/BoundY/BoundWidth/BoundHeightを更新する
  /// @attention *Changedイベントハンドラが無いのでそのまま代入してOK
  private void UpdateDisabledTextboxes() {
    if (App.RuntimeOptions.SelectedEntryIndex >= 0) {
      // プロセス選択中
      this.BoundX.Text = App.Profile.CurrentInputLayoutElement.BoundLeft(
          App.RuntimeOptions.CurrentSampleWidth).ToString();
      this.BoundY.Text = App.Profile.CurrentInputLayoutElement.BoundTop(
          App.RuntimeOptions.CurrentSampleHeight).ToString();
      this.BoundWidth.Text = App.Profile.CurrentInputLayoutElement.BoundWidth(
          App.RuntimeOptions.CurrentSampleWidth).ToString();
      this.BoundHeight.Text = App.Profile.CurrentInputLayoutElement.BoundHeight(
          App.RuntimeOptions.CurrentSampleHeight).ToString();
    } else {
      // プロセス選択なし
      this.BoundX.Text = "n/a";
      this.BoundY.Text = "n/a";
      this.BoundWidth.Text = "n/a";
      this.BoundHeight.Text = "n/a";
    }
  }

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
      this.UpdateDisabledTextboxes();
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
      this.UpdateDisabledTextboxes();
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
      this.UpdateDisabledTextboxes();
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
      this.UpdateDisabledTextboxes();
    }
  }

  //===================================================================
  // IUpdateByRuntimeOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByRuntimeOptions::UpdateByRuntimeOptions
  public void UpdateByRuntimeOptions() {
    this.UpdateDisabledTextboxes();
  }

  /// @copydoc IUpdateByRuntimeOptions::DetachRuntimeOptionsChangedEventHandlers
  public void DetachRuntimeOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByRuntimeOptions::AttachRuntimeOptionsChangedEventHandlers
  public void AttachRuntimeOptionsChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// GroupBox.Headerの最大文字数
  private const int maxHeaderLength = 60;

  /// @copydoc IUpdateByProfile::UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    var header = string.Format("Layout {0:D}: {1}",
        App.Profile.CurrentInputLayoutElement.Index + 1,
        App.Profile.CurrentInputLayoutElement.WindowCaption);
    this.GroupBox.Header = header.Substring(0,
        Math.Min(header.Length, LayoutParameter.maxHeaderLength));

    this.UpdateDisabledTextboxes();

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
