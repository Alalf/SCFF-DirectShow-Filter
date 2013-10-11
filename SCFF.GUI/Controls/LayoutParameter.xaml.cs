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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;
using SCFF.Interprocess;
/// @copydoc SCFF::Common::Profile::BoundRelativeInputCorrector::Names
using BoundRelative = SCFF.Common.Profile.BoundRelativeInputCorrector.Names;

/// 数値を指定してレイアウト配置を調整するためのUserControl
/// @todo(me) InputValidationが甘すぎるので何とかする
public partial class LayoutParameter
    : UserControl, IBindingProfile, IBindingRuntimeOptions {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public LayoutParameter() {
    InitializeComponent();

    // HACK!: PlacementTargetが上手く動かないのでここで設定する
    this.SetPlacementTarget(BoundRelative.Left);
    this.SetPlacementTarget(BoundRelative.Top);
    this.SetPlacementTarget(BoundRelative.Right);
    this.SetPlacementTarget(BoundRelative.Bottom);
  }

  //===================================================================
  // BoundRelativeをキーとしたメソッド群
  //===================================================================

  /// TextBoxとToolTipの位置的な関連付け
  private void SetPlacementTarget(BoundRelative name) {
    this.GetToolTip(name).PlacementTarget = this.GetTextBox(name);
  }
  /// enum->TextBox
  private TextBox GetTextBox(BoundRelative name) {
    switch (name) {
      case BoundRelative.Left: return this.BoundRelativeLeft;
      case BoundRelative.Top: return this.BoundRelativeTop;
      case BoundRelative.Right: return this.BoundRelativeRight;
      case BoundRelative.Bottom: return this.BoundRelativeBottom;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enum->ToolTip
  private ToolTip GetToolTip(BoundRelative name) {
    switch (name) {
      case BoundRelative.Left: return this.BoundRelativeLeftToolTip;
      case BoundRelative.Top: return this.BoundRelativeTopToolTip;
      case BoundRelative.Right: return this.BoundRelativeRightToolTip;
      case BoundRelative.Bottom: return this.BoundRelativeBottomToolTip;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //-------------------------------------------------------------------

  /// enum->文字列
  private string GetBoundRelativeValueString(BoundRelative name) {
    switch (name) {
      case BoundRelative.Left: return StringConverter.GetBoundRelativeLeftString(App.Profile.CurrentView);
      case BoundRelative.Top: return StringConverter.GetBoundRelativeTopString(App.Profile.CurrentView);
      case BoundRelative.Right: return StringConverter.GetBoundRelativeRightString(App.Profile.CurrentView);
      case BoundRelative.Bottom: return StringConverter.GetBoundRelativeBottomString(App.Profile.CurrentView);
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enumと値を指定してProfileを変更
  private void SetBoundRelativeValue(BoundRelative name, double value) {
    switch (name) {
      case BoundRelative.Left: App.Profile.Current.BoundRelativeLeft = value; break;
      case BoundRelative.Top: App.Profile.Current.BoundRelativeTop = value; break;
      case BoundRelative.Right: App.Profile.Current.BoundRelativeRight = value; break;
      case BoundRelative.Bottom: App.Profile.Current.BoundRelativeBottom = value; break;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // 入力エラー処理
  //===================================================================

  /// enumを指定してTextBoxのエラー状態解除
  private void ResetError(BoundRelative name) {
    TextBoxError.ResetError(this.GetTextBox(name), this.GetToolTip(name));
  }
  /// enumを指定してTextBoxのエラー状態設定
  private void SetError(BoundRelative name, string message = null) {
    TextBoxError.SetError(this.GetTextBox(name), this.GetToolTip(name), message);
  }
  /// enumを指定してTextBoxの警告状態設定
  private void SetWarning(BoundRelative name, string message = null) {
    TextBoxError.SetWarning(this.GetTextBox(name), this.GetToolTip(name), message);
  }

  //-------------------------------------------------------------------

  /// enumを指定してレイアウト要素の内容を変更する
  private void Change(BoundRelative target) {
    var dependent = BoundRelativeInputCorrector.GetDependent(target);

    // Parse
    double value;
    if (!double.TryParse(this.GetTextBox(target).Text, out value)) {
      this.SetError(target, "must be double");
      this.ResetError(dependent);
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.SetError(target, "Target window is invalid");
      this.ResetError(dependent);
      return;
    }

    // Correct
    RelativeLTRB changed;
    var result = BoundRelativeInputCorrector.TryChange(
        App.Profile.CurrentView, target, value, out changed);

    // Error表示
    switch (result) {
      case BoundRelativeInputCorrector.TryResult.NothingChanged: {
        this.ResetError(target);
        this.ResetError(dependent);
        break;
      }
      case BoundRelativeInputCorrector.TryResult.TargetChanged: {
        this.SetWarning(target, "Return/Enter: Correct Value");
        this.ResetError(dependent);
        return;
      }
      case BoundRelativeInputCorrector.TryResult.DependentChanged:
      case BoundRelativeInputCorrector.TryResult.BothChanged: {
        this.SetWarning(target, "Return/Enter: Correct Value");
        this.SetWarning(dependent);
        return;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }

    // 成功: そのまま書き換え(Textは変更しない)
    App.Profile.Open();
    this.SetBoundRelativeValue(target, value);
    App.Profile.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, this);
    //---------------------------------------------------------------
  }

  /// enumを指定してレイアウト要素の内容を訂正後に変更する
  private void Correct(BoundRelative target) {
    var dependent = BoundRelativeInputCorrector.GetDependent(target);

    // Parse
    double value;
    if (!double.TryParse(this.GetTextBox(target).Text, out value)) {
      this.OverwriteText(target);
      this.ResetError(dependent);
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.OverwriteText(target);
      this.ResetError(dependent);
      return;
    }

    // Correct
    RelativeLTRB changed;
    var result = BoundRelativeInputCorrector.TryChange(
        App.Profile.CurrentView, target, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (result == BoundRelativeInputCorrector.TryResult.NothingChanged) return;

    // Update Profile
    App.Profile.Open();
    App.Profile.Current.BoundRelativeLeft = changed.Left;
    App.Profile.Current.BoundRelativeTop = changed.Top;
    App.Profile.Current.BoundRelativeRight = changed.Right;
    App.Profile.Current.BoundRelativeBottom = changed.Bottom;
    App.Profile.Close();

    //---------------------------------------------------------------
    // Notify self
    if (result.HasFlag(BoundRelativeInputCorrector.TryResult.TargetChanged)) {
      this.OverwriteText(target);
    } else {
      this.ResetError(target);
    }
    if (result.HasFlag(BoundRelativeInputCorrector.TryResult.DependentChanged)) {
      this.OverwriteText(dependent);
    } else {
      this.ResetError(dependent);
    }
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

  /// BoundRelativeLeft: KeyDown
  private void BoundRelativeLeft_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(BoundRelative.Left);
  }
  /// BoundRelativeTop: KeyDown
  private void BoundRelativeTop_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(BoundRelative.Top);
  }
  /// BoundRelativeRight: KeyDown
  private void BoundRelativeRight_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(BoundRelative.Right);
  }
  /// BoundRelativeBottom: KeyDown
  private void BoundRelativeBottom_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(BoundRelative.Bottom);
  }

  /// BoundRelativeLeft: LostFocus
  private void BoundRelativeLeft_LostFocus(object sender, RoutedEventArgs e) {
    var target = BoundRelative.Left;
    var dependent = BoundRelativeInputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeTop: LostFocus
  private void BoundRelativeTop_LostFocus(object sender, RoutedEventArgs e) {
    var target = BoundRelative.Top;
    var dependent = BoundRelativeInputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeRight: LostFocus
  private void BoundRelativeRight_LostFocus(object sender, RoutedEventArgs e) {
    var target = BoundRelative.Right;
    var dependent = BoundRelativeInputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeBottom: LostFocus
  private void BoundRelativeBottom_LostFocus(object sender, RoutedEventArgs e) {
    var target = BoundRelative.Bottom;
    var dependent = BoundRelativeInputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// BoundRelativeLeft: TextChanged
  private void BoundRelativeLeft_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(BoundRelative.Left);
  }
  /// BoundRelativeTop: TextChanged
  private void BoundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(BoundRelative.Top);
  }
  /// BoundRelativeRight: TextChanged
  private void BoundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(BoundRelative.Right);
  }
  /// BoundRelativeBottom: TextChanged
  private void BoundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(BoundRelative.Bottom);
  }

  //===================================================================
  // IBindingRuntimeOptionsの実装
  //===================================================================

  /// @copydoc Common::GUI::IBindingRuntimeOptions::CanChangeRuntimeOptions
  public bool CanChangeRuntimeOptions { get; private set; }
  /// @copydoc Common::GUI::IBindingRuntimeOptions::OnRuntimeOptionsChanged
  public void OnRuntimeOptionsChanged() {
    this.CanChangeRuntimeOptions = false;
    this.OverwriteBoundRectText();
    this.CanChangeRuntimeOptions = true;
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// enumを指定して、イベントハンドラの実行なしにTextBox.Textを置き換える
  private void OverwriteText(BoundRelative name) {
    this.CanChangeProfile = false;

    var textBox = this.GetTextBox(name);
    textBox.Text = this.GetBoundRelativeValueString(name);
    if (textBox.IsKeyboardFocused) {
      textBox.Select(textBox.Text.Length, 0);
    }
    this.ResetError(name);

    this.CanChangeProfile = true;
  }

  /// BoundX/BoundY/BoundWidth/BoundHeightを更新する
  /// @attention *Changedイベントハンドラが無いのでそのまま代入してOK
  private void OverwriteBoundRectText() {
    // dummyの場合もあり
    var sampleWidth = App.RuntimeOptions.CurrentSampleWidth;
    var sampleHeight = App.RuntimeOptions.CurrentSampleHeight;
    var isDummy = !App.RuntimeOptions.IsCurrentProcessIDValid;

    // 文字列化
    string x, y, width, height;
    StringConverter.GetBoundRectString(App.Profile.CurrentView,
        isDummy, sampleWidth, sampleHeight,
        out x, out y, out width, out height);

    this.BoundX.Text = x;
    this.BoundY.Text = y;
    this.BoundWidth.Text = width;
    this.BoundHeight.Text = height;
  }

  //-------------------------------------------------------------------

  /// GroupBox.Headerの最大文字数
  private const int maxHeaderLength = 60;

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    this.CanChangeProfile = false;

    this.GroupBox.Header =
        StringConverter.GetHeaderStringForLayoutParameter(
            App.Profile.CurrentView, LayoutParameter.maxHeaderLength,
            App.Profile.GetCurrentIndex());

    this.OverwriteBoundRectText();

    var isComplexLayout = App.Profile.LayoutType == LayoutTypes.ComplexLayout;
    this.BoundRelativeLeft.IsEnabled = isComplexLayout;
    this.BoundRelativeTop.IsEnabled = isComplexLayout;
    this.BoundRelativeRight.IsEnabled = isComplexLayout;
    this.BoundRelativeBottom.IsEnabled = isComplexLayout;

    // *Changed/Collapsed/Expanded
    this.BoundRelativeLeft.Text = StringConverter.GetBoundRelativeLeftString(App.Profile.CurrentView);
    this.BoundRelativeTop.Text = StringConverter.GetBoundRelativeTopString(App.Profile.CurrentView);
    this.BoundRelativeRight.Text = StringConverter.GetBoundRelativeRightString(App.Profile.CurrentView);
    this.BoundRelativeBottom.Text = StringConverter.GetBoundRelativeBottomString(App.Profile.CurrentView);
    this.ResetError(BoundRelative.Left);
    this.ResetError(BoundRelative.Top);
    this.ResetError(BoundRelative.Right);
    this.ResetError(BoundRelative.Bottom);

    this.CanChangeProfile = true;
  }
  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
