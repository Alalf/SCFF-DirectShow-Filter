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

/// @file SCFF.GUI/Controls/ResizeMethod.xaml.cs
/// @copydoc SCFF::GUI::Controls::ResizeMethod

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;
using SCFF.Common;

/// SWScaleパラメータ設定用UserControl
public partial class ResizeMethod : UserControl, IUpdateByProfile {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================

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
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------
 
  /// SWScaleAccurateRnd: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleAccurateRnd_Click(object sender, RoutedEventArgs e) {
    if (!this.SWScaleAccurateRnd.IsChecked.HasValue) return;
    
    App.Profile.CurrentMutable.SetSWScaleAccurateRnd =
        (bool)this.SWScaleAccurateRnd.IsChecked;
  }

  /// SWScaleIsFilterEnabled: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleIsFilterEnabled_Click(object sender, RoutedEventArgs e) {
    if (!this.SWScaleIsFilterEnabled.IsChecked.HasValue) return;

    App.Profile.CurrentMutable.SetSWScaleIsFilterEnabled =
        (bool)this.SWScaleIsFilterEnabled.IsChecked;
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  /// SWScaleIsFilterEnabled: Checked
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleIsFilterEnabled_Checked(object sender, RoutedEventArgs e) {
    this.SWScaleLumaGBlur.IsEnabled = true;
    this.SWScaleLumaSharpen.IsEnabled = true;
    this.SWScaleChromaGBlur.IsEnabled = true;
    this.SWScaleChromaSharpen.IsEnabled = true;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  /// SWScaleIsFilterEnabled: Unchecked
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleIsFilterEnabled_Unchecked(object sender, RoutedEventArgs e) {
    this.SWScaleLumaGBlur.IsEnabled = false;
    this.SWScaleLumaSharpen.IsEnabled = false;
    this.SWScaleChromaGBlur.IsEnabled = false;
    this.SWScaleChromaSharpen.IsEnabled = false;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// SWScaleFlags: SelectionChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleFlags_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    SWScaleFlags flags = Constants.ResizeMethodArray[this.SWScaleFlags.SelectedIndex];
    App.Profile.CurrentMutable.SetSWScaleFlags = flags;
  }

  /// 下限・上限つきでテキストボックスから値を取得する
  private bool TryParseSWScaleFilterParameter(TextBox textBox,
      float lowerBound, float upperBound, out float parsedValue) {
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

  /// SWScaleLumaGBlur: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleLumaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleLumaGBlur = parsedValue;
    }
  }

  /// SWScaleChromaGBlur: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleChromaGBlur = parsedValue;
    }
  }

  /// SWScaleLumaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleLumaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleLumaSharpen = parsedValue;
    }
  }

  /// SWScaleChromaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleChromaSharpen = parsedValue;
    }
  }

  /// SWScaleChromaHshift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaHshift_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaHshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleChromaHshift = parsedValue;
    }
  }

  /// SWScaleChromaVshift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaVshift_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaVshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentMutable.SetSWScaleChromaVshift = parsedValue;
    }
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile::UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    this.SWScaleAccurateRnd.IsChecked = App.Profile.Current.SWScaleAccurateRnd;
    this.SWScaleIsFilterEnabled.IsChecked = App.Profile.Current.SWScaleIsFilterEnabled;

    this.DetachProfileChangedEventHandlers();

    var index = Constants.ResizeMethodIndexes[App.Profile.Current.SWScaleFlags];
    this.SWScaleFlags.SelectedIndex = index;
    this.SWScaleLumaGBlur.Text = App.Profile.Current.SWScaleLumaGBlur.ToString("F2");
    this.SWScaleLumaSharpen.Text = App.Profile.Current.SWScaleLumaSharpen.ToString("F2");
    this.SWScaleChromaHshift.Text = App.Profile.Current.SWScaleChromaHshift.ToString("F2");
    this.SWScaleChromaGBlur.Text = App.Profile.Current.SWScaleChromaGBlur.ToString("F2");
    this.SWScaleChromaSharpen.Text = App.Profile.Current.SWScaleChromaSharpen.ToString("F2");
    this.SWScaleChromaVshift.Text = App.Profile.Current.SWScaleChromaVshift.ToString("F2");

    this.AttachProfileChangedEventHandlers();
  }

  /// @copydoc IUpdateByProfile::UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    // 編集するのはCurrentのみ
    this.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile::AttachProfileChangedEventHandlers
  public void AttachProfileChangedEventHandlers() {
    this.SWScaleFlags.SelectionChanged += SWScaleFlags_SelectionChanged;
    this.SWScaleLumaGBlur.TextChanged += SWScaleLumaGBlur_TextChanged;
    this.SWScaleLumaSharpen.TextChanged += SWScaleLumaSharpen_TextChanged;
    this.SWScaleChromaHshift.TextChanged += SWScaleChromaHshift_TextChanged;
    this.SWScaleChromaGBlur.TextChanged += SWScaleChromaGBlur_TextChanged;
    this.SWScaleChromaSharpen.TextChanged += SWScaleChromaSharpen_TextChanged;
    this.SWScaleChromaVshift.TextChanged += SWScaleChromaVshift_TextChanged;
  }

  /// @copydoc IUpdateByProfile::DetachProfileChangedEventHandlers
  public void DetachProfileChangedEventHandlers() {
    this.SWScaleFlags.SelectionChanged -= SWScaleFlags_SelectionChanged;
    this.SWScaleLumaGBlur.TextChanged -= SWScaleLumaGBlur_TextChanged;
    this.SWScaleLumaSharpen.TextChanged -= SWScaleLumaSharpen_TextChanged;
    this.SWScaleChromaHshift.TextChanged -= SWScaleChromaHshift_TextChanged;
    this.SWScaleChromaGBlur.TextChanged -= SWScaleChromaGBlur_TextChanged;
    this.SWScaleChromaSharpen.TextChanged -= SWScaleChromaSharpen_TextChanged;
    this.SWScaleChromaVshift.TextChanged -= SWScaleChromaVshift_TextChanged;
  }
}
}   // namespace SCFF.GUI.Controls
