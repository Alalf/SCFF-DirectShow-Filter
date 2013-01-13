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
using SCFF.Common.GUI;

/// SWScaleパラメータ設定用UserControl
public partial class ResizeMethod : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
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
    
    App.Profile.Current.Open();
    App.Profile.Current.SetSWScaleAccurateRnd =
        (bool)this.SWScaleAccurateRnd.IsChecked;
    App.Profile.Current.Close();
  }

  /// SWScaleIsFilterEnabled: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleIsFilterEnabled_Click(object sender, RoutedEventArgs e) {
    if (!this.SWScaleIsFilterEnabled.IsChecked.HasValue) return;

    App.Profile.Current.Open();
    App.Profile.Current.SetSWScaleIsFilterEnabled =
        (bool)this.SWScaleIsFilterEnabled.IsChecked;
    App.Profile.Current.Close();
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
    if (!this.CanChangeProfile) return;
    SWScaleFlags flags = Constants.ResizeMethodArray[this.SWScaleFlags.SelectedIndex];

    App.Profile.Current.Open();
    App.Profile.Current.SetSWScaleFlags = flags;
    App.Profile.Current.Close();
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
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleLumaGBlur = parsedValue;
      App.Profile.Current.Close();
    }
  }

  /// SWScaleChromaGBlur: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 2.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaGBlur, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleChromaGBlur = parsedValue;
      App.Profile.Current.Close();
    }
  }

  /// SWScaleLumaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleLumaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleLumaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleLumaSharpen = parsedValue;
      App.Profile.Current.Close();
    }
  }

  /// SWScaleChromaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 4.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaSharpen, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleChromaSharpen = parsedValue;
      App.Profile.Current.Close();
    }
  }

  /// SWScaleChromaHshift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaHshift_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaHshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleChromaHshift = parsedValue;
      App.Profile.Current.Close();
    }
  }

  /// SWScaleChromaVshift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaVshift_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0.0F;
    var upperBound = 1.0F;
    float parsedValue;
    if (this.TryParseSWScaleFilterParameter(this.SWScaleChromaVshift, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetSWScaleChromaVshift = parsedValue;
      App.Profile.Current.Close();
    }
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    this.CanChangeProfile = false;
    this.SWScaleAccurateRnd.IsChecked = App.Profile.CurrentView.SWScaleAccurateRnd;
    this.SWScaleIsFilterEnabled.IsChecked = App.Profile.CurrentView.SWScaleIsFilterEnabled;

    var index = Constants.ResizeMethodIndexes[App.Profile.CurrentView.SWScaleFlags];
    this.SWScaleFlags.SelectedIndex = index;
    this.SWScaleLumaGBlur.Text = App.Profile.CurrentView.SWScaleLumaGBlurString;
    this.SWScaleLumaSharpen.Text = App.Profile.CurrentView.SWScaleLumaSharpenString;
    this.SWScaleChromaHshift.Text = App.Profile.CurrentView.SWScaleChromaHshiftString;
    this.SWScaleChromaGBlur.Text = App.Profile.CurrentView.SWScaleChromaGBlurString;
    this.SWScaleChromaSharpen.Text = App.Profile.CurrentView.SWScaleChromaSharpenString;
    this.SWScaleChromaVshift.Text = App.Profile.CurrentView.SWScaleChromaVshiftString;
    this.CanChangeProfile = true;
  }
  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
