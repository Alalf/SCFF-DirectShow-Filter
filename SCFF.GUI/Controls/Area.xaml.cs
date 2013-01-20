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
  // 列挙型
  //===================================================================
  private enum Names {
    ClippingX,
    ClippingY,
    ClippingWidth,
    ClippingHeight,
  }

  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public Area() {
    InitializeComponent();
    
    // HACK!: PlacementTargetが上手く動かないのでここで設定する
    this.GetToolTip(Names.ClippingX).PlacementTarget = this.GetTextBox(Names.ClippingX);
    this.GetToolTip(Names.ClippingY).PlacementTarget = this.GetTextBox(Names.ClippingY);
    this.GetToolTip(Names.ClippingWidth).PlacementTarget = this.GetTextBox(Names.ClippingWidth);
    this.GetToolTip(Names.ClippingHeight).PlacementTarget = this.GetTextBox(Names.ClippingHeight);
  }

  //-------------------------------------------------------------------

  private TextBox GetTextBox(Names name) {
    switch (name) {
      case Names.ClippingX: return this.ClippingX;
      case Names.ClippingY: return this.ClippingY;
      case Names.ClippingWidth: return this.ClippingWidth;
      case Names.ClippingHeight: return this.ClippingHeight;
      default: {
        Debug.Fail("Invalid Name", "Area.GetTextBox");
        return null;
      }
    }
  }

  private ToolTip GetToolTip(Names name) {
    switch (name) {
      case Names.ClippingX: return this.ClippingXToolTip;
      case Names.ClippingY: return this.ClippingYToolTip;
      case Names.ClippingWidth: return this.ClippingWidthToolTip;
      case Names.ClippingHeight: return this.ClippingHeightToolTip;
      default: {
        Debug.Fail("Invalid Name", "Area.GetToolTip");
        return null;
      }
    }
  }


  //===================================================================
  // 入力エラー処理
  //===================================================================

  private bool HasError(Names name) {
    return TextBoxError.HasError(this.GetTextBox(name));
  }
  private void ResetError(Names name) {
    TextBoxError.ResetError(this.GetTextBox(name), this.GetToolTip(name));
  }
  private void SetError(Names name, string message = null) {
    TextBoxError.SetError(this.GetTextBox(name), this.GetToolTip(name), message);
  }
  private void SetWarning(Names name, string message = null) {
    TextBoxError.SetWarning(this.GetTextBox(name), this.GetToolTip(name), message);
  }

  //-------------------------------------------------------------------

  private void ChangePosition(Positions position) {
    var onX = (position == Positions.X);
    var positionName = onX ? Names.ClippingX : Names.ClippingY;
    var size = onX ? Sizes.Width : Sizes.Height;
    var sizeName = onX ? Names.ClippingWidth : Names.ClippingHeight;

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(positionName).Text, out value)) {
      this.SetError(positionName, "must be integer");
      this.ResetError(sizeName);
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.SetError(positionName, "Target window is invalid");
      this.ResetError(sizeName);
      return;
    }

    // Correct
    ClientRect changed;
    var tryResult = InputCorrector.TryChangeClippingPosition(
        App.Profile.CurrentView, position, value, out changed);

    // Error表示
    switch (tryResult) {
      case InputCorrector.TryResult.NothingChanged: {
        this.ResetError(positionName);
        this.ResetError(sizeName);
        break;
      }
      case InputCorrector.TryResult.PositionChanged: {
        this.SetWarning(positionName, "Return/Enter: Correct Value");
        this.ResetError(sizeName);
        return;
      }
      case InputCorrector.TryResult.SizeChanged:
      case InputCorrector.TryResult.PositionAndSizeChanged: {
        this.SetWarning(positionName, "Return/Enter: Correct Value");
        this.SetWarning(sizeName);
        return;
      }
      default: {
        Debug.Fail("Invalid TryResult", "Area.ChangePosition");
        return;
      }
    }

    // 成功: そのまま書き換え(Textは変更しない)
    App.Profile.Current.Open();
    if (onX) {
      App.Profile.Current.SetClippingXWithoutFit = value;
    } else {
      App.Profile.Current.SetClippingYWithoutFit = value;
    }
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  private void CorrectPosition(Positions position) {
    var onX = (position == Positions.X);
    var positionName = onX ? Names.ClippingX : Names.ClippingY;
    var size = onX ? Sizes.Width : Sizes.Height;
    var sizeName = onX ? Names.ClippingWidth : Names.ClippingHeight;

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(positionName).Text, out value)) {
      this.OverwriteText(positionName);
      this.ResetError(sizeName);
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.OverwriteText(positionName);
      this.ResetError(sizeName);
      return;
    }

    // Correct
    ClientRect changed;
    var tryResult = InputCorrector.TryChangeClippingPosition(
        App.Profile.CurrentView, position, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (tryResult == InputCorrector.TryResult.NothingChanged) return;

    // Update Profile
    App.Profile.Current.Open();
    if (onX) {
      App.Profile.Current.SetClippingXWithoutFit = changed.X;
      App.Profile.Current.SetClippingWidthWithoutFit = changed.Width;
    } else {
      App.Profile.Current.SetClippingYWithoutFit = changed.Y;
      App.Profile.Current.SetClippingHeightWithoutFit = changed.Height;
    }
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    switch (tryResult) {
      case InputCorrector.TryResult.PositionChanged: {
        this.OverwriteText(positionName);
        this.ResetError(sizeName);
        break;
      }
      case InputCorrector.TryResult.SizeChanged: {
        this.ResetError(positionName);
        this.OverwriteText(sizeName);
        break;
      }
      case InputCorrector.TryResult.PositionAndSizeChanged: {
        this.OverwriteText(positionName);
        this.OverwriteText(sizeName);
        break;
      }
      default: {
        Debug.Fail("Invalid TryResult", "Area.CorrectPosition");
        return;
      }
    }
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  private void ChangeSize(Sizes size) {
    var onX = (size == Sizes.Width);
    var sizeName = onX ? Names.ClippingWidth : Names.ClippingHeight;
    var position = onX ? Positions.X : Positions.Y;
    var positionName = onX ? Names.ClippingX : Names.ClippingY;

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(sizeName).Text, out value)) {
      this.ResetError(positionName);
      this.SetError(sizeName, "must be integer");
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ResetError(positionName);
      this.SetError(sizeName, "Target window is invalid");
      return;
    }

    // Correct
    ClientRect changed;
    var tryResult = InputCorrector.TryChangeClippingSize(
        App.Profile.CurrentView, size, value, out changed);

    // 失敗
    // Error表示
    switch (tryResult) {
      case InputCorrector.TryResult.NothingChanged: {
        this.ResetError(positionName);
        this.ResetError(sizeName);
        break;
      }
      case InputCorrector.TryResult.SizeChanged: {
        this.ResetError(positionName);
        this.SetWarning(sizeName, "Return/Enter: Correct Value");
        return;
      }
      case InputCorrector.TryResult.PositionChanged:
      case InputCorrector.TryResult.PositionAndSizeChanged: {
        this.SetWarning(positionName);
        this.SetWarning(sizeName, "Return/Enter: Correct Value");
        return;
      }
      default: {
        Debug.Fail("Invalid TryResult", "Area.ChangeSize");
        return;
      }
    }

    // 成功: そのまま書き換え(Textは変更しない)
    App.Profile.Current.Open();
    if (onX) {
      App.Profile.Current.SetClippingWidthWithoutFit = value;
    } else {
      App.Profile.Current.SetClippingHeightWithoutFit = value;
    }
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
    //---------------------------------------------------------------
  }

  private void CorrectSize(Sizes size) {
    var onX = (size == Sizes.Width);
    var sizeName = onX ? Names.ClippingWidth : Names.ClippingHeight;
    var position = onX ? Positions.X : Positions.Y;
    var positionName = onX ? Names.ClippingX : Names.ClippingY;

    // Parse
    int value;
    if (!int.TryParse(this.GetTextBox(sizeName).Text, out value)) {
      this.ResetError(positionName);
      this.OverwriteText(sizeName);
      return;
    }

    // Window Check
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ResetError(positionName);
      this.OverwriteText(sizeName);
      return;
    }

    // Correct
    ClientRect changed;
    var tryResult = InputCorrector.TryChangeClippingSize(
        App.Profile.CurrentView, size, value, out changed);

    // 訂正の必要がない=TextChangedで設定済み
    if (tryResult == InputCorrector.TryResult.NothingChanged) return;

    // Update Profile
    App.Profile.Current.Open();
    if (onX) {
      App.Profile.Current.SetClippingXWithoutFit = changed.X;
      App.Profile.Current.SetClippingWidthWithoutFit = changed.Width;
    } else {
      App.Profile.Current.SetClippingYWithoutFit = changed.Y;
      App.Profile.Current.SetClippingHeightWithoutFit = changed.Height;
    }
    App.Profile.Current.Close();

    //---------------------------------------------------------------
    // Notify self
    switch (tryResult) {
      case InputCorrector.TryResult.SizeChanged: {
        this.ResetError(positionName);
        this.OverwriteText(sizeName);
        break;
      }
      case InputCorrector.TryResult.PositionChanged: {
        this.OverwriteText(positionName);
        this.ResetError(sizeName);
        break;
      }
      case InputCorrector.TryResult.PositionAndSizeChanged: {
        this.OverwriteText(positionName);
        this.OverwriteText(sizeName);
        break;
      }
      default: {
        Debug.Fail("Invalid TryResult", "Area.CorrectSize");
        return;
      }
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
          default: {
            border = BrushesAndPens.CurrentNormalBrush;
            background = BrushesAndPens.NormalBrush;
            break;
          }
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
      default : {
        Debug.Fail("Invalid WindowTypes", "Area.GetWindowTypesBrushes");
        border = null;
        background = null;
        break;
      }
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
      Debug.Assert(nextWindowType == WindowTypes.Desktop ||
                   nextWindowType == WindowTypes.DesktopListView);
      switch (nextWindowType) {
        case WindowTypes.DesktopListView: {
          App.Profile.Current.SetWindowToDesktopListView();
          break;
        }
        case WindowTypes.Desktop: {
          App.Profile.Current.SetWindowToDesktop();
          break;
        }
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
    this.CorrectPosition(Positions.X);
  }
  /// ClippingY: KeyDown
  private void ClippingY_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.CorrectPosition(Positions.Y);
  }
  /// ClippingWidth: KeyDown
  private void ClippingWidth_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.CorrectSize(Sizes.Width);
  }
  /// ClippingHeight: KeyDown
  private void ClippingHeight_KeyDown(object sender, KeyEventArgs e) {
    if (e.Key != Key.Return && e.Key != Key.Enter) return;
    this.CorrectSize(Sizes.Height);
  }

  /// ClippingX: LostFocus
  private void ClippingX_LostFocus(object sender, RoutedEventArgs e) {
    this.OverwriteText(Names.ClippingX);
    this.ResetError(Names.ClippingWidth);
  }
  /// ClippingY: LostFocus
  private void ClippingY_LostFocus(object sender, RoutedEventArgs e) {
    this.OverwriteText(Names.ClippingY);
    this.ResetError(Names.ClippingHeight);
  }
  /// ClippingWidth: LostFocus
  private void ClippingWidth_LostFocus(object sender, RoutedEventArgs e) {
    this.ResetError(Names.ClippingX);
    this.OverwriteText(Names.ClippingWidth);
  }
  /// ClippingHeight: LostFocus
  private void ClippingHeight_LostFocus(object sender, RoutedEventArgs e) {
    this.ResetError(Names.ClippingY);
    this.OverwriteText(Names.ClippingHeight);
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
    this.ChangePosition(Positions.X);
  }
  /// ClippingY: TextChanged
  private void ClippingY_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.ChangePosition(Positions.Y);
  }
  /// ClippingWidth: TextChanged
  private void ClippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.ChangeSize(Sizes.Width);
  }
  /// ClippingHeight: TextChanged
  private void ClippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    this.ChangeSize(Sizes.Height);
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  private void OverwriteText(Names name) {
    this.CanChangeProfile = false;

    var textBox = this.GetTextBox(name);
    switch (name) {
      case Names.ClippingX: {
        textBox.Text = StringConverter.GetClippingXString(App.Profile.CurrentView);
        break;
      }
      case Names.ClippingY: {
        textBox.Text = StringConverter.GetClippingYString(App.Profile.CurrentView);
        break;
      }
      case Names.ClippingWidth: {
        textBox.Text = StringConverter.GetClippingWidthString(App.Profile.CurrentView);
        break;
      }
      case Names.ClippingHeight: {
        textBox.Text = StringConverter.GetClippingHeightString(App.Profile.CurrentView);
        break;
      }
      default: {
        Debug.Fail("Invalid Name", "Area.OverwriteText");
        break;
      }
    }
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
    }
    this.Fit.IsChecked = App.Profile.CurrentView.Fit;

    // *Changed/Collapsed/Expanded
    this.ClippingX.Text = StringConverter.GetClippingXString(App.Profile.CurrentView);
    this.ClippingY.Text = StringConverter.GetClippingYString(App.Profile.CurrentView);
    this.ClippingWidth.Text = StringConverter.GetClippingWidthString(App.Profile.CurrentView);
    this.ClippingHeight.Text = StringConverter.GetClippingHeightString(App.Profile.CurrentView);
    this.ResetError(Names.ClippingX);
    this.ResetError(Names.ClippingY);
    this.ResetError(Names.ClippingWidth);
    this.ResetError(Names.ClippingHeight);

    this.CanChangeProfile = true;
  }

  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // current以外のデータを表示する必要はない
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
