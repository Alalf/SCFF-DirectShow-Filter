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
using SCFF.Common.Ext;
using SCFF.Common.GUI;

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
  /// @todo(me) まだ違和感が残る。エラーがあるかもしれないのにHasErrorを消していいのか？
  ///           もういっかいValidateしないとだめでは？
  private void ClippingX_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingX.Tag as string == "HasError") {
      this.CanChangeProfile = false;
      this.ClippingX.Text = App.Profile.CurrentView.ClippingXString;
      this.CanChangeProfile = true;
      this.ClippingX.Tag = null;
    }
  }
  /// ClippingY: LostFocus
  private void ClippingY_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingY.Tag as string == "HasError") {
      this.CanChangeProfile = false;
      this.ClippingY.Text = App.Profile.CurrentView.ClippingYString;
      this.CanChangeProfile = true;
      this.ClippingY.Tag = null;
    }
  }
  /// ClippingWidth: LostFocus
  private void ClippingWidth_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingWidth.Tag as string == "HasError") {
      this.CanChangeProfile = false;
      this.ClippingWidth.Text = App.Profile.CurrentView.ClippingWidthString;
      this.CanChangeProfile = true;
      this.ClippingWidth.Tag = null;
    }
  }
  /// ClippingHeight: LostFocus
  private void ClippingHeight_LostFocus(object sender, RoutedEventArgs e) {
    if (this.ClippingHeight.Tag as string == "HasError") {
      this.CanChangeProfile = false;
      this.ClippingHeight.Text = App.Profile.CurrentView.ClippingHeightString;
      this.CanChangeProfile = true;
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

  /// 下限・上限つきでテキストボックスから値を取得する
  private bool TryParseClippingParameters(TextBox textBox,
      int lowerBound, int upperBound, out int parsedValue) {
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
    if (!App.Profile.CurrentView.ValidateClippingX(clippingX, out fixedX, out fixedWidth)) {
      // Validateで失敗したら自分とWidthのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingXWithoutFit = fixedX;
      App.Profile.Current.SetClippingWidthWithoutFit = fixedWidth;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      this.CanChangeProfile = false;
      if (clippingX != fixedX) {
        this.ClippingX.Text = App.Profile.CurrentView.ClippingXString;
        this.ClippingX.SelectAll();
      }
      this.ClippingWidth.Text = App.Profile.CurrentView.ClippingWidthString;
      this.CanChangeProfile = true;
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

  /// ClippingY: TextChanged
  private void ClippingY_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0;
    var upperBound = 9999;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingY, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingYWithoutFit = parsedValue;
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
    if (!App.Profile.CurrentView.ValidateClippingWidth(clippingWidth, out fixedX, out fixedWidth)) {
      // Validateで失敗したら自分とWidthのTextとデータを置き換える
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingXWithoutFit = fixedX;
      App.Profile.Current.SetClippingWidthWithoutFit = fixedWidth;
      App.Profile.Current.Close();

      //---------------------------------------------------------------
      // Notify self
      this.CanChangeProfile = false;
      this.ClippingX.Text = App.Profile.CurrentView.ClippingXString;
      if (clippingWidth != fixedWidth) {
        this.ClippingWidth.Text = App.Profile.CurrentView.ClippingWidthString;
        this.ClippingWidth.SelectAll();
      }
      this.CanChangeProfile = true;
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

  /// ClippingHeight: TextChanged
  private void ClippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var lowerBound = 0;
    var upperBound = 9999;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingHeight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.Current.Open();
      App.Profile.Current.SetClippingHeightWithoutFit = parsedValue;
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
    this.ClippingX.Text = App.Profile.CurrentView.ClippingXString;
    this.ClippingY.Text = App.Profile.CurrentView.ClippingYString;
    this.ClippingWidth.Text = App.Profile.CurrentView.ClippingWidthString;
    this.ClippingHeight.Text = App.Profile.CurrentView.ClippingHeightString;
    
    this.CanChangeProfile = true;
  }

  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // current以外のデータを表示する必要はない
    this.OnCurrentLayoutElementChanged();
  }
}
}   // namespace SCFF.GUI.Controls
