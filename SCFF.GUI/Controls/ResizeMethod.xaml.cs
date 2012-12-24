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

/// @file SCFF.GUI/Controls/ResizeMethod.cs
/// 拡大縮小方式設定用

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Windows;
using System.Windows.Controls;

/// 拡大縮小方式設定用
public partial class ResizeMethod : UserControl, IProfileToControl {

  /// コンストラクタ
  public ResizeMethod() {
    InitializeComponent();

    // SWScaleFlags
    this.SWScaleFlags.Items.Clear();
    foreach (var method in Constants.ResizeMethodLabels) {
      var item = new ComboBoxItem();
      item.Content = method;
      this.SWScaleFlags.Items.Add(item);
    }
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  /// @copydoc IProfileToControl.UpdateByProfile
  public void UpdateByProfile() {
    this.SWScaleAccurateRnd.IsChecked = App.Profile.CurrentLayoutElement.SWScaleAccurateRnd;
    this.SWScaleIsFilterEnabled.IsChecked = App.Profile.CurrentLayoutElement.SWScaleIsFilterEnabled;

    this.DetachChangedEventHandlers();

    var index = Constants.ResizeMethodIndexes[App.Profile.CurrentLayoutElement.SWScaleFlags];
    this.SWScaleFlags.SelectedIndex = index;
    this.SWScaleLumaGBlur.Text = App.Profile.CurrentLayoutElement.SWScaleLumaGBlur.ToString("F2");
    this.SWScaleLumaSharpen.Text = App.Profile.CurrentLayoutElement.SWScaleLumaSharpen.ToString("F2");
    this.SWScaleChromaHshift.Text = App.Profile.CurrentLayoutElement.SWScaleChromaHshift.ToString("F2");
    this.SWScaleChromaGBlur.Text = App.Profile.CurrentLayoutElement.SWScaleChromaGBlur.ToString("F2");
    this.SWScaleChromaSharpen.Text = App.Profile.CurrentLayoutElement.SWScaleChromaSharpen.ToString("F2");
    this.SWScaleChromaVshift.Text = App.Profile.CurrentLayoutElement.SWScaleChromaVshift.ToString("F2");

    this.AttachChangedEventHandlers();
  }

  /// @copydoc IProfileToControl.AttachChangedEventHandlers
  public void AttachChangedEventHandlers() {
    this.SWScaleFlags.SelectionChanged += swscaleFlags_SelectionChanged;
    this.SWScaleLumaGBlur.TextChanged += swscaleLumaGBlur_TextChanged;
    this.SWScaleLumaSharpen.TextChanged += swscaleLumaSharpen_TextChanged;
    this.SWScaleChromaHshift.TextChanged += swscaleChromaHshift_TextChanged;
    this.SWScaleChromaGBlur.TextChanged += swscaleChromaGBlur_TextChanged;
    this.SWScaleChromaSharpen.TextChanged += swscaleChromaSharpen_TextChanged;
    this.SWScaleChromaVshift.TextChanged += swscaleChromaVshift_TextChanged;
  }

  /// @copydoc IProfileToControl.DetachChangedEventHandlers
  public void DetachChangedEventHandlers() {
    this.SWScaleFlags.SelectionChanged -= swscaleFlags_SelectionChanged;
    this.SWScaleLumaGBlur.TextChanged -= swscaleLumaGBlur_TextChanged;
    this.SWScaleLumaSharpen.TextChanged -= swscaleLumaSharpen_TextChanged;
    this.SWScaleChromaHshift.TextChanged -= swscaleChromaHshift_TextChanged;
    this.SWScaleChromaGBlur.TextChanged -= swscaleChromaGBlur_TextChanged;
    this.SWScaleChromaSharpen.TextChanged -= swscaleChromaSharpen_TextChanged;
    this.SWScaleChromaVshift.TextChanged -= swscaleChromaVshift_TextChanged;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------
 
  private void swscaleAccurateRnd_Click(object sender, RoutedEventArgs e) {
    if (this.SWScaleAccurateRnd.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.SWScaleAccurateRnd = (bool)this.SWScaleAccurateRnd.IsChecked;
    }
  }

  private void swscaleIsFilterEnabled_Click(object sender, RoutedEventArgs e) {
    if (this.SWScaleIsFilterEnabled.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.SWScaleIsFilterEnabled = (bool)this.SWScaleIsFilterEnabled.IsChecked;
    }
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  private void swscaleIsFilterEnabled_Checked(object sender, RoutedEventArgs e) {
    this.SWScaleLumaGBlur.IsEnabled = true;
    this.SWScaleLumaSharpen.IsEnabled = true;
    this.SWScaleChromaGBlur.IsEnabled = true;
    this.SWScaleChromaSharpen.IsEnabled = true;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  private void swscaleIsFilterEnabled_Unchecked(object sender, RoutedEventArgs e) {
    this.SWScaleLumaGBlur.IsEnabled = false;
    this.SWScaleLumaSharpen.IsEnabled = false;
    this.SWScaleChromaGBlur.IsEnabled = false;
    this.SWScaleChromaSharpen.IsEnabled = false;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------

  private void swscaleFlags_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    SWScaleFlags flags = Constants.ResizeMethodArray[this.SWScaleFlags.SelectedIndex];
    App.Profile.CurrentLayoutElement.SWScaleFlags = flags;
  }

  private bool TryParseSWScaleFilterParameter(TextBox textBox, float lowerBound, float upperBound, out float parsedValue) {
    // Parse
    if (!float.TryParse(textBox.Text, out parsedValue)) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString("F2");
      return false;
    }

    // Validation
    if (parsedValue < lowerBound) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString("F2");
      return false;
    } else if (parsedValue > upperBound) {
      parsedValue = upperBound;
      textBox.Text = upperBound.ToString("F2");
      return false;
    }

    return true;
  }

  private void swscaleLumaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleLumaGBlur = parsedValue;
    }
  }

  private void swscaleChromaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleChromaGBlur = parsedValue;
    }
  }

  private void swscaleLumaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleLumaSharpen = parsedValue;
    }
  }

  private void swscaleChromaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleChromaSharpen = parsedValue;
    }
  }

  private void swscaleChromaHshift_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaHshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleChromaHshift = parsedValue;
    }
  }

  private void swscaleChromaVshift_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaVshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.SWScaleChromaVshift = parsedValue;
    }
  }
}
}
