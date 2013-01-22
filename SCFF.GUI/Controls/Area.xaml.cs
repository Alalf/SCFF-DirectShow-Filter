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

/// @file SCFF.GUI/Controls/Area.xaml.cs
/// @copydoc SCFF::GUI::Controls::Area

namespace SCFF.GUI.Controls {

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

/// クリッピング領域設定用UserControl
public partial class Area : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public Area() {
    InitializeComponent();
    
    // HACK!: PlacementTargetが上手く動かないのでここで設定する
    this.GetToolTip(InputCorrector.Clipping.X).PlacementTarget =
        this.GetTextBox(InputCorrector.Clipping.X);
    this.GetToolTip(InputCorrector.Clipping.Y).PlacementTarget =
        this.GetTextBox(InputCorrector.Clipping.Y);
    this.GetToolTip(InputCorrector.Clipping.Width).PlacementTarget =
        this.GetTextBox(InputCorrector.Clipping.Width);
    this.GetToolTip(InputCorrector.Clipping.Height).PlacementTarget =
        this.GetTextBox(InputCorrector.Clipping.Height);
  }

  //===================================================================
  // RectPropertiesをキーとしたメソッド群
  //===================================================================

  /// Rectのプロパティ名->Clipping*文字列
  private string GetClippingValueString(InputCorrector.Clipping name) {
    switch (name) {
      case InputCorrector.Clipping.X: return StringConverter.GetClippingXString(App.Profile.CurrentView);
      case InputCorrector.Clipping.Y: return StringConverter.GetClippingYString(App.Profile.CurrentView);
      case InputCorrector.Clipping.Width: return StringConverter.GetClippingWidthString(App.Profile.CurrentView);
      case InputCorrector.Clipping.Height: return StringConverter.GetClippingHeightString(App.Profile.CurrentView);
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// Rectのプロパティ名と値を指定してClipping*WithoutFitを変更
  private void SetClippingValue(InputCorrector.Clipping name, int value) {
    switch (name) {
      case InputCorrector.Clipping.X: App.Profile.Current.SetClippingXWithoutFit = value; break;
      case InputCorrector.Clipping.Y: App.Profile.Current.SetClippingYWithoutFit = value; break;
      case InputCorrector.Clipping.Width: App.Profile.Current.SetClippingWidthWithoutFit = value; break;
      case InputCorrector.Clipping.Height: App.Profile.Current.SetClippingHeightWithoutFit = value; break;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// Rectのプロパティ名->TextBox
  private TextBox GetTextBox(InputCorrector.Clipping name) {
    switch (name) {
      case InputCorrector.Clipping.X: return this.ClippingX;
      case InputCorrector.Clipping.Y: return this.ClippingY;
      case InputCorrector.Clipping.Width: return this.ClippingWidth;
      case InputCorrector.Clipping.Height: return this.ClippingHeight;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }
  /// Rectのプロパティ名->ToolTip
  private ToolTip GetToolTip(InputCorrector.Clipping name) {
    switch (name) {
      case InputCorrector.Clipping.X: return this.ClippingXToolTip;
      case InputCorrector.Clipping.Y: return this.ClippingYToolTip;
      case InputCorrector.Clipping.Width: return this.ClippingWidthToolTip;
      case InputCorrector.Clipping.Height: return this.ClippingHeightToolTip;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // 入力エラー処理
  //===================================================================

  /// Rectのプロパティ名を指定してTextBoxのエラー状態解除
  private void ResetError(InputCorrector.Clipping name) {
    TextBoxError.ResetError(this.GetTextBox(name), this.GetToolTip(name));
  }
  /// Rectのプロパティ名を指定してTextBoxのエラー状態設定
  private void SetError(InputCorrector.Clipping name, string message = null) {
    TextBoxError.SetError(this.GetTextBox(name), this.GetToolTip(name), message);
  }
  /// Rectのプロパティ名を指定してTextBoxの警告状態設定
  private void SetWarning(InputCorrector.Clipping name, string message = null) {
    TextBoxError.SetWarning(this.GetTextBox(name), this.GetToolTip(name), message);
  }

  //-------------------------------------------------------------------

  /// Rectのプロパティ名を指定してレイアウト要素の内容を変更する
  private void Change(InputCorrector.Clipping target) {
    var dependent = InputCorrector.GetDependent(target);

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(target).Text, out value)) {
      this.SetError(target, "must be integer");
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
    ClientRect changed;
    var result = InputCorrector.TryChangeClippingRectWithoutFit(
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
    this.SetClippingValue(target, value);
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  /// Rectのプロパティ名を指定してレイアウト要素の内容を訂正後に変更する
  private void Correct(InputCorrector.Clipping target) {
    var dependent = InputCorrector.GetDependent(target);

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(target).Text, out value)) {
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
    ClientRect changed;
    var result = InputCorrector.TryChangeClippingRectWithoutFit(
        App.Profile.CurrentView, target, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (result == InputCorrector.TryResult.NothingChanged) return;

    // Update Profile
    App.Profile.Current.Open();
    App.Profile.Current.SetClippingXWithoutFit = changed.X;
    App.Profile.Current.SetClippingWidthWithoutFit = changed.Width;
    App.Profile.Current.SetClippingYWithoutFit = changed.Y;
    App.Profile.Current.SetClippingHeightWithoutFit = changed.Height;
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
  private void Fit_Click(object sender, RoutedEventArgs e) {
    if (!this.Fit.IsChecked.HasValue) return;

    App.Profile.Current.Open();
    App.Profile.Current.SetFit = (bool)this.Fit.IsChecked;
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    this.OnCurrentLayoutElementChanged();
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //-----------------------------------------------------------------
  }

  //-------------------------------------------------------------------

  /// レイアウトタイプに合わせて適切なBrush/Penを設定する
  private void GetWindowTypesBrushes(
      WindowTypes currentWindowType, WindowTypes nextWindowType,
      out Brush border, out Brush background) {
    switch (nextWindowType) {
      case WindowTypes.Normal: {
        // 更に現在のTypeによって色を分ける
        switch (currentWindowType) {
          case WindowTypes.DesktopListView: {
            border = BrushesAndPens.CurrentDesktopListViewBrush;
            background = BrushesAndPens.DesktopListViewBrush;
            break;
          }
          case WindowTypes.Desktop: {
            border = BrushesAndPens.CurrentDesktopBrush;
            background = BrushesAndPens.DesktopBrush;
            break;
          }
          case WindowTypes.Normal: {
            border = BrushesAndPens.CurrentNormalBrush;
            background = BrushesAndPens.NormalBrush;
            break;
          }
          default: Debug.Fail("switch"); throw new System.ArgumentException();
        }
        break;
      }
      case WindowTypes.DesktopListView: {
        border = BrushesAndPens.CurrentDesktopListViewBrush;
        background = BrushesAndPens.DesktopListViewBrush;
        break;
      }
      case WindowTypes.Desktop: {
        border = BrushesAndPens.CurrentDesktopBrush;
        background = BrushesAndPens.DesktopBrush;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// 現在編集中のレイアウト要素のクリッピング領域/Fitオプションを変更する
  private void CommonAreaSelect(bool changeWindowType = false,
                                WindowTypes nextWindowType = WindowTypes.Normal) {
    if (!App.Profile.CurrentView.IsWindowValid) {
      Debug.WriteLine("Invalid Window", "Area.CommonAreaSelect");
      return;
    }

    //-----------------------------------------------------------------
    // ダイアログの準備
    //-----------------------------------------------------------------
    var dialog = new AreaSelectWindow();

    // サイズ
    var screenRect = App.Profile.CurrentView.ScreenClippingRectWithFit;
    dialog.Left   = screenRect.X;
    dialog.Top    = screenRect.Y;
    dialog.Width  = screenRect.Width;
    dialog.Height = screenRect.Height;

    // カラーの変更
    Brush border, background;
    this.GetWindowTypesBrushes(
        App.Profile.CurrentView.WindowType, nextWindowType,
        out border, out background);
    dialog.WindowBorder.BorderBrush = border;
    dialog.WindowGrid.Background = background;

    // ダイアログの表示
    var result = dialog.ShowDialog();
    if (!result.HasValue || !(bool)result) return;

    // 結果をScreenRectにまとめる
    var nextScreenRect = new ScreenRect(
        (int)dialog.Left, (int)dialog.Top, (int)dialog.Width, (int)dialog.Height);

    // Profile更新
    App.Profile.Current.Open();
    if (changeWindowType) {
      switch (nextWindowType) {
        case WindowTypes.DesktopListView: {
          App.Profile.Current.SetWindowToDesktopListView();
          break;
        }
        case WindowTypes.Desktop: {
          App.Profile.Current.SetWindowToDesktop();
          break;
        }
        default: Debug.Fail("switch"); throw new System.ArgumentException();
      }
    }
    App.Profile.Current.SetClippingRectByScreenRect(nextScreenRect);
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    this.OnCurrentLayoutElementChanged();
    // Notify other controls
    if (changeWindowType) {
      Commands.TargetWindowChanged.Execute(null, null);
    } else {
      // TargetWindowの更新は必要ないのでプレビューのみ更新
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    }
    //-----------------------------------------------------------------
  }

  /// AreaSelect: Click
  private void AreaSelect_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect();
  }

  /// ListView: Click
  private void ListView_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(true, WindowTypes.DesktopListView);
  }

  /// Desktop: Click
  private void Desktop_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(true, WindowTypes.Desktop);
  }

  //-------------------------------------------------------------------

  /// ClippingX: KeyDown
  private void ClippingX_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.Clipping.X);
  }
  /// ClippingY: KeyDown
  private void ClippingY_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.Clipping.Y);
  }
  /// ClippingWidth: KeyDown
  private void ClippingWidth_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.Clipping.Width);
  }
  /// ClippingHeight: KeyDown
  private void ClippingHeight_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.Correct(InputCorrector.Clipping.Height);
  }

  /// ClippingX: LostFocus
  private void ClippingX_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.Clipping.X;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// ClippingY: LostFocus
  private void ClippingY_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.Clipping.Y;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// ClippingWidth: LostFocus
  private void ClippingWidth_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.Clipping.Width;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }
  /// ClippingHeight: LostFocus
  private void ClippingHeight_LostFocus(object sender, RoutedEventArgs e) {
    var target = InputCorrector.Clipping.Height;
    var dependent = InputCorrector.GetDependent(target);
    this.OverwriteText(target);
    this.ResetError(dependent);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  /// Fit: Checked
  private void Fit_Checked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = false;
    this.ClippingY.IsEnabled = false;
    this.ClippingWidth.IsEnabled = false;
    this.ClippingHeight.IsEnabled = false;
  }

  /// Fit: Unchecked
  private void Fit_Unchecked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = true;
    this.ClippingY.IsEnabled = true;
    this.ClippingWidth.IsEnabled = true;
    this.ClippingHeight.IsEnabled = true;
  }

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// ClippingX: TextChanged
  private void ClippingX_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.Clipping.X);
  }
  /// ClippingY: TextChanged
  private void ClippingY_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.Clipping.Y);
  }
  /// ClippingWidth: TextChanged
  private void ClippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.Clipping.Width);
  }
  /// ClippingHeight: TextChanged
  private void ClippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.Change(InputCorrector.Clipping.Height);
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// Rectのプロパティ名を指定して、イベントハンドラの実行なしにTextBox.Textを置き換える
  private void OverwriteText(InputCorrector.Clipping name) {
    this.CanChangeProfile = false;

    var textBox = this.GetTextBox(name);
    textBox.Text = this.GetClippingValueString(name);
    if (textBox.IsKeyboardFocused) {
      textBox.Select(textBox.Text.Length, 0);
    }
    this.ResetError(name);

    this.CanChangeProfile = true;
  }

  //-------------------------------------------------------------------

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    this.CanChangeProfile = false;

    switch (App.Profile.CurrentView.WindowType) {
      case WindowTypes.Normal: {
        this.ListView.IsEnabled = true;
        this.Desktop.IsEnabled = true;
        break;
      }
      case WindowTypes.Desktop: {
        this.ListView.IsEnabled = true;
        this.Desktop.IsEnabled = false;
        break;
      }
      case WindowTypes.DesktopListView: {
        this.ListView.IsEnabled = false;
        this.Desktop.IsEnabled = true;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
    this.Fit.IsChecked = App.Profile.CurrentView.Fit;

    // *Changed/Collapsed/Expanded
    this.ClippingX.Text = StringConverter.GetClippingXString(App.Profile.CurrentView);
    this.ClippingY.Text = StringConverter.GetClippingYString(App.Profile.CurrentView);
    this.ClippingWidth.Text = StringConverter.GetClippingWidthString(App.Profile.CurrentView);
    this.ClippingHeight.Text = StringConverter.GetClippingHeightString(App.Profile.CurrentView);
    this.ResetError(InputCorrector.Clipping.X);
    this.ResetError(InputCorrector.Clipping.Y);
    this.ResetError(InputCorrector.Clipping.Width);
    this.ResetError(InputCorrector.Clipping.Height);

    this.CanChangeProfile = true;
  }

  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // current以外のデータを表示する必要はない
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
