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

/// @file SCFF.GUI/Controls/Area.cs
/// クリッピング領域設定用

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// クリッピング領域設定用
public partial class Area : UserControl, IProfileToControl {

  /// コンストラクタ
  public Area() {
    InitializeComponent();
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  /// @copydoc IProfileToControl.UpdateByProfile
  public void UpdateByProfile() {
    // checkboxはclickがあるのでeventハンドラをattach/detachする必要はない
    this.Fit.IsChecked = App.Profile.CurrentInputLayoutElement.Fit;
    this.DetachChangedEventHandlers();
    this.ClippingX.Text = App.Profile.CurrentInputLayoutElement.ClippingXWithFit.ToString();
    this.ClippingY.Text = App.Profile.CurrentInputLayoutElement.ClippingYWithFit.ToString();
    this.ClippingWidth.Text = App.Profile.CurrentInputLayoutElement.ClippingWidthWithFit.ToString();
    this.ClippingHeight.Text = App.Profile.CurrentInputLayoutElement.ClippingHeightWithFit.ToString();
    this.AttachChangedEventHandlers();
  }

  /// @copydoc IProfileToControl.AttachChangedEventHandlers
  public void AttachChangedEventHandlers() {
    this.ClippingX.TextChanged += clippingX_TextChanged;
    this.ClippingY.TextChanged += clippingY_TextChanged;
    this.ClippingWidth.TextChanged += clippingWidth_TextChanged;
    this.ClippingHeight.TextChanged += clippingHeight_TextChanged;
  }

  /// @copydoc IProfileToControl.DetachChangedEventHandlers
  public void DetachChangedEventHandlers() {
    this.ClippingX.TextChanged -= clippingX_TextChanged;
    this.ClippingY.TextChanged -= clippingY_TextChanged;
    this.ClippingWidth.TextChanged -= clippingWidth_TextChanged;
    this.ClippingHeight.TextChanged -= clippingHeight_TextChanged;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void fit_Click(object sender, RoutedEventArgs e) {
    if (this.Fit.IsChecked.HasValue) {
      App.Profile.CurrentOutputLayoutElement.Fit = (bool)this.Fit.IsChecked;

      this.UpdateByProfile();
    }
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  private void fit_Checked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = false;
    this.ClippingY.IsEnabled = false;
    this.ClippingWidth.IsEnabled = false;
    this.ClippingHeight.IsEnabled = false;
  }

  private void fit_Unchecked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = true;
    this.ClippingY.IsEnabled = true;
    this.ClippingWidth.IsEnabled = true;
    this.ClippingHeight.IsEnabled = true;
  }

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------

  private bool TryParseClippingParameters(TextBox textBox, int lowerBound, int upperBound, out int parsedValue) {
    // Parse
    if (!int.TryParse(textBox.Text, out parsedValue)) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString();
      return false;
    }

    // Validation
    if (parsedValue < lowerBound) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString();
      return false;
    } else if (parsedValue > upperBound) {
      parsedValue = upperBound;
      textBox.Text = upperBound.ToString();
      return false;
    }

    return true;
  }

  private void clippingX_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingX, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingXWithoutFit = parsedValue;
    }
  }

  private void clippingY_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingY, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingYWithoutFit = parsedValue;
    }
  }

  private void clippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingWidth, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingWidthWithoutFit = parsedValue;
    }
  }

  private void clippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingHeight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingHeightWithoutFit = parsedValue;
    }
  }
}
}   // namespace SCFF.GUI.Controls
