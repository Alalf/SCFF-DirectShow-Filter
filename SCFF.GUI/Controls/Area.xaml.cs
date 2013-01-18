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

  /// ClippingX: LostFocus
  private void ClippingX_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingX.Tag as string == "HasError") {
      this.OverwriteClippingXText(false);
      this.ClippingX.Tag = null;
    }
  }
  /// ClippingY: LostFocus
  private void ClippingY_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingY.Tag as string == "HasError") {
      this.OverwriteClippingYText(false);
      this.ClippingY.Tag = null;
    }
  }
  /// ClippingWidth: LostFocus
  private void ClippingWidth_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingWidth.Tag as string == "HasError") {
      this.OverwriteClippingWidthText(false);
      this.ClippingWidth.Tag = null;
    }
  }
  /// ClippingHeight: LostFocus
  private void ClippingHeight_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingHeight.Tag as string == "HasError") {
      this.OverwriteClippingHeightText(false);
      this.ClippingHeight.Tag = null;
    }
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

    // Parse可能か
    int clippingX;
    if (!int.TryParse(this.ClippingX.Text, out clippingX)) {
      this.ClippingX.Tag = "HasError";
      return;
    } else {
      this.ClippingX.Tag = null;
    }

    // Windowチェック
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ClippingX.Tag = "HasError";
      return;
    } else {
      this.ClippingX.Tag = null;
    }

    // 次にValidateする
    int fixedX, fixedWidth;
    if (!InputCorrector.CorrectInputClippingX(App.Profile.CurrentView, clippingX, out fixedX, out fixedWidth)) {
      // Validateで失敗したらX/WidthのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingXWithoutFit = fixedX;
      App.Profile.Current.SetClippingWidthWithoutFit = fixedWidth;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      if (clippingX != fixedX) {
        this.OverwriteClippingXText(true);
      }
      this.OverwriteClippingWidthText(false);
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    } else {
      // 成功したらそのまま書き換え(Textは変更しない)
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingXWithoutFit = clippingX;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    }
  }

  /// ClippingWidth: TextChanged
  private void ClippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;

    // Parse可能か
    int clippingWidth;
    if (!int.TryParse(this.ClippingWidth.Text, out clippingWidth)) {
      this.ClippingWidth.Tag = "HasError";
      return;
    } else {
      this.ClippingWidth.Tag = null;
    }

    // Windowチェック
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ClippingWidth.Tag = "HasError";
      return;
    } else {
      this.ClippingWidth.Tag = null;
    }

    // 次にValidateする
    int fixedX, fixedWidth;
    if (!InputCorrector.CorrectInputClippingWidth(App.Profile.CurrentView, clippingWidth, out fixedX, out fixedWidth)) {
      // Validateで失敗したらX/WidthのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingXWithoutFit = fixedX;
      App.Profile.Current.SetClippingWidthWithoutFit = fixedWidth;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      this.OverwriteClippingXText(false);
      if (clippingWidth != fixedWidth) {
        this.OverwriteClippingWidthText(true);
      }
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    } else {
      // 成功したらそのまま書き換え(Textは変更しない)
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingWidthWithoutFit = clippingWidth;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    }
  }

  /// ClippingY: TextChanged
  private void ClippingY_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;

    // Parse可能か
    int clippingY;
    if (!int.TryParse(this.ClippingY.Text, out clippingY)) {
      this.ClippingY.Tag = "HasError";
      return;
    } else {
      this.ClippingY.Tag = null;
    }

    // Windowチェック
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ClippingY.Tag = "HasError";
      return;
    } else {
      this.ClippingY.Tag = null;
    }

    // 次にValidateする
    int fixedY, fixedHeight;
    if (!InputCorrector.CorrectInputClippingY(App.Profile.CurrentView, clippingY, out fixedY, out fixedHeight)) {
      // Validateで失敗したらY/HeightのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingYWithoutFit = fixedY;
      App.Profile.Current.SetClippingHeightWithoutFit = fixedHeight;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      if (clippingY != fixedY) {
        this.OverwriteClippingYText(true);
      }
      this.OverwriteClippingHeightText(false);
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    } else {
      // 成功したらそのまま書き換え(Textは変更しない)
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingYWithoutFit = clippingY;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    }
  }

  /// ClippingHeight: TextChanged
  private void ClippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;

    // Parse可能か
    int clippingHeight;
    if (!int.TryParse(this.ClippingHeight.Text, out clippingHeight)) {
      this.ClippingHeight.Tag = "HasError";
      return;
    } else {
      this.ClippingHeight.Tag = null;
    }

    // Windowチェック
    if (!App.Profile.CurrentView.IsWindowValid) {
      this.ClippingHeight.Tag = "HasError";
      return;
    } else {
      this.ClippingHeight.Tag = null;
    }

    // 次にValidateする
    int fixedY, fixedHeight;
    if (!InputCorrector.CorrectInputClippingHeight(App.Profile.CurrentView, clippingHeight, out fixedY, out fixedHeight)) {
      // Validateで失敗したらY/HeightのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingYWithoutFit = fixedY;
      App.Profile.Current.SetClippingHeightWithoutFit = fixedHeight;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      this.OverwriteClippingYText(false);
      if (clippingHeight != fixedHeight) {
        this.OverwriteClippingHeightText(true);
      }
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    } else {
      // 成功したらそのまま書き換え(Textは変更しない)
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingHeightWithoutFit = clippingHeight;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      // Notify other controls
      Commands.CurrentLayoutElementVisualChanged.Execute(null, null);
      //---------------------------------------------------------------
    }
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  private void OverwriteClippingXText(bool fixCursor) {
    this.CanChangeProfile = false;
    this.ClippingX.Text = StringConverter.GetClippingXString(App.Profile.CurrentView);
    if (fixCursor) this.ClippingX.Select(this.ClippingX.Text.Length, 0);
    this.CanChangeProfile = true;
  }

  private void OverwriteClippingWidthText(bool fixCursor) {
    this.CanChangeProfile = false;
    this.ClippingWidth.Text = StringConverter.GetClippingWidthString(App.Profile.CurrentView);
    if (fixCursor) this.ClippingWidth.Select(this.ClippingWidth.Text.Length, 0);
    this.CanChangeProfile = true;
  }

  private void OverwriteClippingYText(bool fixCursor) {
    this.CanChangeProfile = false;
    this.ClippingY.Text = StringConverter.GetClippingYString(App.Profile.CurrentView);
    if (fixCursor) this.ClippingY.Select(this.ClippingY.Text.Length, 0);
    this.CanChangeProfile = true;
  }

  private void OverwriteClippingHeightText(bool fixCursor) {
    this.CanChangeProfile = false;
    this.ClippingHeight.Text = StringConverter.GetClippingHeightString(App.Profile.CurrentView);
    if (fixCursor) this.ClippingHeight.Select(this.ClippingHeight.Text.Length, 0);
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

    this.CanChangeProfile = true;
  }

  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // current以外のデータを表示する必要はない
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
