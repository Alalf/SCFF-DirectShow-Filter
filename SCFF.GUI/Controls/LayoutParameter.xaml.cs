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
    this.GetToolTip(InputCorrector.BoundRelative.Left).PlacementTarget =
        this.GetTextBox(InputCorrector.BoundRelative.Left);
    this.GetToolTip(InputCorrector.BoundRelative.Top).PlacementTarget =
        this.GetTextBox(InputCorrector.BoundRelative.Top);
    this.GetToolTip(InputCorrector.BoundRelative.Right).PlacementTarget =
        this.GetTextBox(InputCorrector.BoundRelative.Right);
    this.GetToolTip(InputCorrector.BoundRelative.Bottom).PlacementTarget =
        this.GetTextBox(InputCorrector.BoundRelative.Bottom);
  }

  //===================================================================
  // InputCorrector.BoundRelativeをキーとしたメソッド群
  //===================================================================

  /// enum->文字列
  private string GetBoundRelativeValueString(InputCorrector.BoundRelative name) {
    switch (name) {
      case InputCorrector.BoundRelative.Left: return StringConverter.GetBoundRelativeLeftString(App.Profile.CurrentView);
      case InputCorrector.BoundRelative.Top: return StringConverter.GetBoundRelativeTopString(App.Profile.CurrentView);
      case InputCorrector.BoundRelative.Right: return StringConverter.GetBoundRelativeRightString(App.Profile.CurrentView);
      case InputCorrector.BoundRelative.Bottom: return StringConverter.GetBoundRelativeBottomString(App.Profile.CurrentView);
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enumと値を指定してProfileを変更
  private void SetBoundRelativeValue(InputCorrector.BoundRelative name, double value) {
    switch (name) {
      case InputCorrector.BoundRelative.Left: App.Profile.Current.SetBoundRelativeLeft = value; break;
      case InputCorrector.BoundRelative.Top: App.Profile.Current.SetBoundRelativeTop = value; break;
      case InputCorrector.BoundRelative.Right: App.Profile.Current.SetBoundRelativeRight = value; break;
      case InputCorrector.BoundRelative.Bottom: App.Profile.Current.SetBoundRelativeBottom = value; break;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enum->TextBox
  private TextBox GetTextBox(InputCorrector.BoundRelative name) {
    switch (name) {
      case InputCorrector.BoundRelative.Left: return this.BoundRelativeLeft;
      case InputCorrector.BoundRelative.Top: return this.BoundRelativeTop;
      case InputCorrector.BoundRelative.Right: return this.BoundRelativeRight;
      case InputCorrector.BoundRelative.Bottom: return this.BoundRelativeBottom;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// enum->ToolTip
  private ToolTip GetToolTip(InputCorrector.BoundRelative name) {
    switch (name) {
      case InputCorrector.BoundRelative.Left: return this.BoundRelativeLeftToolTip;
      case InputCorrector.BoundRelative.Top: return this.BoundRelativeTopToolTip;
      case InputCorrector.BoundRelative.Right: return this.BoundRelativeRightToolTip;
      case InputCorrector.BoundRelative.Bottom: return this.BoundRelativeBottomToolTip;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // 入力エラー処理
  //===================================================================

  /// enumを指定してTextBoxのエラー状態解除
  private void ResetError(InputCorrector.BoundRelative name) {
    TextBoxError.ResetError(this.GetTextBox(name), this.GetToolTip(name));
  }
  /// enumを指定してTextBoxのエラー状態設定
  private void SetError(InputCorrector.BoundRelative name, string message = null) {
    TextBoxError.SetError(this.GetTextBox(name), this.GetToolTip(name), message);
  }
  /// enumを指定してTextBoxの警告状態設定
  private void SetWarning(InputCorrector.BoundRelative name, string message = null) {
    TextBoxError.SetWarning(this.GetTextBox(name), this.GetToolTip(name), message);
  }

  //-------------------------------------------------------------------

  /// enumを指定してレイアウト要素の内容を変更する
  private void Change(InputCorrector.BoundRelative target) {
    var dependent = InputCorrector.GetDependent(target);

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
    var result = InputCorrector.TryChangeBoundRelativeLTRB(
        App.Profile.CurrentView, target, value, out changed);

    // Error表示
    switch (result) {
      case InputCorrector.TryResult.NothingChanged: {
        this.ResetError(target);
        this.ResetError(dependent);
        break;
      }
      case InputCorrector.TryResult.TargetChanged: {
        this.SetWarning(target, "Return/Enter: Correct Value");
        this.ResetError(dependent);
        return;
      }
      case InputCorrector.TryResult.DependentChanged:
      case InputCorrector.TryResult.BothChanged: {
        this.SetWarning(target, "Return/Enter: Correct Value");
        this.SetWarning(dependent);
        return;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }

    // 成功: そのまま書き換え(Textは変更しない)
    App.Profile.Current.Open();
    this.SetBoundRelativeValue(target, value);
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  /// enumを指定してレイアウト要素の内容を訂正後に変更する
  private void Correct(InputCorrector.BoundRelative target) {
    var dependent = InputCorrector.GetDependent(target);

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
    var result = InputCorrector.TryChangeBoundRelativeLTRB(
        App.Profile.CurrentView, target, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (result == InputCorrector.TryResult.NothingChanged) return;

    // Update Profile
    App.Profile.Current.Open();
    App.Profile.Current.SetBoundRelativeLeft = changed.Left;
    App.Profile.Current.SetBoundRelativeTop = changed.Top;
    App.Profile.Current.SetBoundRelativeRight = changed.Right;
    App.Profile.Current.SetBoundRelativeBottom = changed.Bottom;
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    if (result.HasFlag(InputCorrector.TryResult.TargetChanged)) {
      this.OverwriteText(target);
    } else {
      this.ResetError(target);
    }
    if (result.HasFlag(InputCorrector.TryResult.DependentChanged)) {
      this.OverwriteText(dependent);
    } else {
      this.ResetError(dependent);
    }
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// Fit: Click
  ///
  /// 現在選択中のレイアウト要素のアスペクト比をあわせ、黒帯を消す
  /// @todo(me) コンテキストメニューにも実装したいのでCommand化したい
  ///           (Shiftドラッグで比率維持とかやってもいいかも)
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Fit_Click(object sender, System.Windows.RoutedEventArgs e) {
    if (!App.Profile.CurrentView.IsWindowValid) {
      Debug.WriteLine("Invalid Window", "LayoutParameter.Fit_Click");
      return;
    }

    // Profileの設定を変える
    App.Profile.Current.Open();
    App.Profile.Current.FitBoundRelativeRect(
        App.RuntimeOptions.CurrentSampleWidth,
        App.RuntimeOptions.CurrentSampleHeight);
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    this.OnCurrentLayoutElementChanged();
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //-----------------------------------------------------------------
  }

  //-------------------------------------------------------------------

  /// BoundRelativeLeft: KeyDown
  private void BoundRelativeLeft_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.BoundRelative.Left);
  }
  /// BoundRelativeTop: KeyDown
  private void BoundRelativeTop_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.BoundRelative.Top);
  }
  /// BoundRelativeRight: KeyDown
  private void BoundRelativeRight_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.BoundRelative.Right);
  }
  /// BoundRelativeBottom: KeyDown
  private void BoundRelativeBottom_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.BoundRelative.Bottom);
  }

  /// BoundRelativeLeft: LostFocus
  private void BoundRelativeLeft_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.BoundRelative.Left;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeTop: LostFocus
  private void BoundRelativeTop_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.BoundRelative.Top;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeRight: LostFocus
  private void BoundRelativeRight_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.BoundRelative.Right;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// BoundRelativeBottom: LostFocus
  private void BoundRelativeBottom_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.BoundRelative.Bottom;
    var dependent = InputCorrector.GetDependent(target);
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
    this.Change(InputCorrector.BoundRelative.Left);
  }
  /// BoundRelativeTop: TextChanged
  private void BoundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.BoundRelative.Top);
  }
  /// BoundRelativeRight: TextChanged
  private void BoundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.BoundRelative.Bottom);
  }
  /// BoundRelativeBottom: TextChanged
  private void BoundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.BoundRelative.Bottom);
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
  private void OverwriteText(InputCorrector.BoundRelative name) {
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
    var isDummy = App.RuntimeOptions.SelectedEntryIndex == -1;

    this.BoundX.Text =
        StringConverter.GetBoundXString(App.Profile.CurrentView, isDummy, sampleWidth);
    this.BoundY.Text =
        StringConverter.GetBoundYString(App.Profile.CurrentView, isDummy, sampleHeight);
    this.BoundWidth.Text =
        StringConverter.GetBoundWidthString(App.Profile.CurrentView, isDummy, sampleWidth);
    this.BoundHeight.Text =
        StringConverter.GetBoundHeightString(App.Profile.CurrentView, isDummy, sampleHeight);
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
            App.Profile.CurrentView, LayoutParameter.maxHeaderLength);

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
    this.ResetError(InputCorrector.BoundRelative.Left);
    this.ResetError(InputCorrector.BoundRelative.Top);
    this.ResetError(InputCorrector.BoundRelative.Right);
    this.ResetError(InputCorrector.BoundRelative.Bottom);

    this.CanChangeProfile = true;
  }
  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
