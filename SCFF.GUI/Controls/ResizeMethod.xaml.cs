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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;
/// @copydoc SCFF::Common::Profile::SWScaleInputCorrector::Names
using SWScale = SCFF.Common.Profile.SWScaleInputCorrector.Names;

/// SWScaleパラメータ設定用UserControl
public partial class ResizeMethod : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public ResizeMethod() {
    InitializeComponent();

    // SWScaleFlags
    this.SWScaleFlags.ItemsSource = Constants.SWScaleFlagLabels;
    this.SWScaleFlags.SelectedValuePath = "Key";
    this.SWScaleFlags.DisplayMemberPath = "Value";

    // HACK!: PlacementTargetが上手く動かないのでここで設定する
    this.SetPlacementTarget(SWScale.LumaGBlur);
    this.SetPlacementTarget(SWScale.LumaSharpen);
    this.SetPlacementTarget(SWScale.ChromaHShift);
    this.SetPlacementTarget(SWScale.ChromaGBlur);
    this.SetPlacementTarget(SWScale.ChromaSharpen);
    this.SetPlacementTarget(SWScale.ChromaVShift);
  }

  //===================================================================
  // SWScaleをキーとしたメソッド群
  //===================================================================

  /// TextBoxとToolTipの位置的な関連付け
  private void SetPlacementTarget(SWScale name) {
    this.GetToolTip(name).PlacementTarget = this.GetTextBox(name);
  }
  /// enum->TextBox
  private TextBox GetTextBox(SWScale name) {
    switch (name) {
      case SWScale.LumaGBlur: return this.SWScaleLumaGBlur;
      case SWScale.LumaSharpen: return this.SWScaleLumaSharpen;
      case SWScale.ChromaHShift: return this.SWScaleChromaHShift;
      case SWScale.ChromaGBlur: return this.SWScaleChromaGBlur;
      case SWScale.ChromaSharpen: return this.SWScaleChromaSharpen;
      case SWScale.ChromaVShift: return this.SWScaleChromaVShift;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enum->ToolTip
  private ToolTip GetToolTip(SWScale name) {
    switch (name) {
      case SWScale.LumaGBlur: return this.SWScaleLumaGBlurToolTip;
      case SWScale.LumaSharpen: return this.SWScaleLumaSharpenToolTip;
      case SWScale.ChromaHShift: return this.SWScaleChromaHShiftToolTip;
      case SWScale.ChromaGBlur: return this.SWScaleChromaGBlurToolTip;
      case SWScale.ChromaSharpen: return this.SWScaleChromaSharpenToolTip;
      case SWScale.ChromaVShift: return this.SWScaleChromaVShiftToolTip;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //-------------------------------------------------------------------

  /// enum->文字列
  private string GetSWScaleValueString(SWScale name) {
    switch (name) {
      case SWScale.LumaGBlur: return StringConverter.GetSWScaleLumaGBlurString(App.Profile.CurrentView);
      case SWScale.LumaSharpen: return StringConverter.GetSWScaleLumaSharpenString(App.Profile.CurrentView);
      case SWScale.ChromaHShift: return StringConverter.GetSWScaleChromaHShiftString(App.Profile.CurrentView);
      case SWScale.ChromaGBlur: return StringConverter.GetSWScaleChromaGBlurString(App.Profile.CurrentView);
      case SWScale.ChromaSharpen: return StringConverter.GetSWScaleChromaSharpenString(App.Profile.CurrentView);
      case SWScale.ChromaVShift: return StringConverter.GetSWScaleChromaVShiftString(App.Profile.CurrentView);
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enumと値を指定してProfileを変更
  private void SetSWScaleValue(SWScale name, float value) {
    switch (name) {
      case SWScale.LumaGBlur: App.Profile.Current.SetSWScaleLumaGBlur = value; break;
      case SWScale.LumaSharpen: App.Profile.Current.SetSWScaleLumaSharpen = value; break;
      case SWScale.ChromaHShift: App.Profile.Current.SetSWScaleChromaHShift = value; break;
      case SWScale.ChromaGBlur: App.Profile.Current.SetSWScaleChromaGBlur = value; break;
      case SWScale.ChromaSharpen: App.Profile.Current.SetSWScaleChromaSharpen = value; break;
      case SWScale.ChromaVShift: App.Profile.Current.SetSWScaleChromaVShift = value; break;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // 入力エラー処理
  //===================================================================

  /// enumを指定してTextBoxのエラー状態解除
  private void ResetError(SWScale name) {
    TextBoxError.ResetError(this.GetTextBox(name), this.GetToolTip(name));
  }
  /// enumを指定してTextBoxのエラー状態設定
  private void SetError(SWScale name, string message = null) {
    TextBoxError.SetError(this.GetTextBox(name), this.GetToolTip(name), message);
  }
  /// enumを指定してTextBoxの警告状態設定
  private void SetWarning(SWScale name, string message = null) {
    TextBoxError.SetWarning(this.GetTextBox(name), this.GetToolTip(name), message);
  }

  //-------------------------------------------------------------------

  /// enumを指定してレイアウト要素の内容を変更する
  private void Change(SWScale target) {
    // Parse
    float value;
    if (!float.TryParse(this.GetTextBox(target).Text, out value)) {
      this.SetError(target, "must be float");
      return;
    }

    // Correct
    float changed;
    var result = SWScaleInputCorrector.TryChange(target, value, out changed);

    // Error表示
    if (result) {
      // 成功
      this.ResetError(target);
    } else {
      // 失敗
      this.SetWarning(target, "Return/Enter: Correct Value");
      return;
    }

    // 成功: そのまま書き換え(Textは変更しない)
    App.Profile.Current.Open();
    this.SetSWScaleValue(target, value);
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, this);
    //---------------------------------------------------------------
  }

  /// enumを指定してレイアウト要素の内容を訂正後に変更する
  private void Correct(SWScale target) {
    // Parse
    float value;
    if (!float.TryParse(this.GetTextBox(target).Text, out value)) {
      this.OverwriteText(target);
      return;
    }

    // Correct
    float changed;
    var result = SWScaleInputCorrector.TryChange(target, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (result) return;

    // Update Profile
    App.Profile.Current.Open();
    this.SetSWScaleValue(target, changed);
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    this.OverwriteText(target);
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, this);
    //---------------------------------------------------------------
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

  /// SWScaleLumaGBlur: KeyDown
  private void SWScaleLumaGBlur_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.LumaGBlur);
  }
  /// SWScaleChromaGBlur: KeyDown
  private void SWScaleChromaGBlur_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.ChromaGBlur);
  }
  /// SWScaleLumaSharpen: KeyDown
  private void SWScaleLumaSharpen_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.LumaSharpen);
  }
  /// SWScaleChromaSharpen: KeyDown
  private void SWScaleChromaSharpen_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.ChromaSharpen);
  }
  /// SWScaleChromaHShift: KeyDown
  private void SWScaleChromaHShift_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.ChromaHShift);
  }
  /// SWScaleChromaVShift: KeyDown
  private void SWScaleChromaVShift_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(SWScale.ChromaVShift);
  }

  /// SWScaleLumaGBlur: LostFocus
  private void SWScaleLumaGBlur_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.LumaGBlur;
    this.OverwriteText(target);
  }
  /// SWScaleChromaGBlur: LostFocus
  private void SWScaleChromaGBlur_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.ChromaGBlur;
    this.OverwriteText(target);
  }
  /// SWScaleLumaSharpen: LostFocus
  private void SWScaleLumaSharpen_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.LumaSharpen;
    this.OverwriteText(target);
  }
  /// SWScaleChromaSharpen: LostFocus
  private void SWScaleChromaSharpen_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.ChromaSharpen;
    this.OverwriteText(target);
  }
  /// SWScaleChromaHShift: LostFocus
  private void SWScaleChromaHShift_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.ChromaHShift;
    this.OverwriteText(target);
  }
  /// SWScaleChromaVShift: LostFocus
  private void SWScaleChromaVShift_LostFocus(object sender, RoutedEventArgs e) {
    var target = SWScale.ChromaVShift;
    this.OverwriteText(target);
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
    /// @todo(me) HShiftおよびVShiftの使い方がわかるまで設定できないように
  }

  /// SWScaleIsFilterEnabled: Unchecked
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleIsFilterEnabled_Unchecked(object sender, RoutedEventArgs e) {
    this.SWScaleLumaGBlur.IsEnabled = false;
    this.SWScaleLumaSharpen.IsEnabled = false;
    this.SWScaleChromaGBlur.IsEnabled = false;
    this.SWScaleChromaSharpen.IsEnabled = false;
    /// @todo(me) HShiftおよびVShiftの使い方がわかるまで設定できないように
  }

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// SWScaleFlags: SelectionChanged
  private void SWScaleFlags_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var flags = (Interprocess.SWScaleFlags)this.SWScaleFlags.SelectedValue;

    App.Profile.Current.Open();
    App.Profile.Current.SetSWScaleFlags = flags;
    App.Profile.Current.Close();
  }

  /// SWScaleLumaGBlur: TextChanged
  private void SWScaleLumaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.LumaGBlur);
  }

  /// SWScaleChromaGBlur: TextChanged
  private void SWScaleChromaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.ChromaGBlur);
  }

  /// SWScaleLumaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleLumaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.LumaSharpen);
  }

  /// SWScaleChromaSharpen: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaSharpen_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.ChromaSharpen);
  }

  /// SWScaleChromaHShift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaHShift_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.ChromaHShift);
  }

  /// SWScaleChromaVShift: TextChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void SWScaleChromaVShift_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(SWScale.ChromaVShift);
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// enumを指定して、イベントハンドラの実行なしにTextBox.Textを置き換える
  private void OverwriteText(SWScale name) {
    this.CanChangeProfile = false;

    var textBox = this.GetTextBox(name);
    textBox.Text = this.GetSWScaleValueString(name);
    if (textBox.IsKeyboardFocused) {
      textBox.Select(textBox.Text.Length, 0);
    }
    this.ResetError(name);

    this.CanChangeProfile = true;
  }

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    this.CanChangeProfile = false;
    this.SWScaleAccurateRnd.IsChecked = App.Profile.CurrentView.SWScaleAccurateRnd;
    this.SWScaleIsFilterEnabled.IsChecked = App.Profile.CurrentView.SWScaleIsFilterEnabled;
    this.SWScaleFlags.SelectedValue = App.Profile.CurrentView.SWScaleFlags;
    this.SWScaleLumaGBlur.Text = StringConverter.GetSWScaleLumaGBlurString(App.Profile.CurrentView);
    this.SWScaleLumaSharpen.Text = StringConverter.GetSWScaleLumaSharpenString(App.Profile.CurrentView);
    this.SWScaleChromaHShift.Text = StringConverter.GetSWScaleChromaHShiftString(App.Profile.CurrentView);
    this.SWScaleChromaGBlur.Text = StringConverter.GetSWScaleChromaGBlurString(App.Profile.CurrentView);
    this.SWScaleChromaSharpen.Text = StringConverter.GetSWScaleChromaSharpenString(App.Profile.CurrentView);
    this.SWScaleChromaVShift.Text = StringConverter.GetSWScaleChromaVShiftString(App.Profile.CurrentView);
    this.CanChangeProfile = true;
  }
  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentLayoutElementChanged();
  }

}
}   // namespace SCFF.GUI.Controls
