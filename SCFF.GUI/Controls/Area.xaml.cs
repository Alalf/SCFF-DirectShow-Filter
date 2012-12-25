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

/// @file SCFF.GUI/Controls/Area.cs
/// クリッピング領域設定用

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
  using System.Windows.Media;

/// クリッピング領域設定用
public partial class Area : UserControl, IProfileToControl {

  private readonly Brush normalBorderBrush = Brushes.DarkOrange;
  private Brush normalGridBrush;
  private readonly Brush desktopListViewBorderBrush = Brushes.DarkCyan;
  private Brush desktopListViewGridBrush;
  private readonly Brush desktopBorderBrush = Brushes.DarkGreen;
  private Brush desktopGridBrush;

  /// コンストラクタ
  public Area() {
    InitializeComponent();

    //<Border x:Name="WindowBorder" BorderBrush="DarkOrange" BorderThickness="1">
    //<Grid x:Name="WindowGrid" Background="#99FF8000">

    // AreaSelectWindow用のブラシの生成
    var normalGridColor = new Color() {
      A = 0x99, R = 0xFF, G = 0x80, B = 0x00
    };
    this.normalGridBrush = new SolidColorBrush(normalGridColor);
    this.normalGridBrush.Freeze();

    var desktopListViewGridColor = Colors.DarkCyan;
    desktopListViewGridColor.A = 0x99;
    this.desktopListViewGridBrush = new SolidColorBrush(desktopListViewGridColor);
    this.desktopListViewGridBrush.Freeze();

    var desktpGridColor = Colors.DarkGreen;
    desktpGridColor.A = 0x99;
    this.desktopGridBrush = new SolidColorBrush(desktpGridColor);
    this.desktopGridBrush.Freeze();
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  /// @copydoc IProfileToControl.UpdateByProfile
  public void UpdateByProfile() {
    // Enabled/Disabled
    switch (App.Profile.CurrentInputLayoutElement.WindowType) {
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

    // checkboxはclickがあるのでeventハンドラをattach/detachする必要はない
    this.Fit.IsChecked = App.Profile.CurrentInputLayoutElement.Fit;

    // *Changed
    this.DetachChangedEventHandlers();
    this.ClippingX.Text = App.Profile.CurrentInputLayoutElement.ClippingXWithFit.ToString();
    this.ClippingY.Text = App.Profile.CurrentInputLayoutElement.ClippingYWithFit.ToString();
    this.ClippingWidth.Text = App.Profile.CurrentInputLayoutElement.ClippingWidthWithFit.ToString();
    this.ClippingHeight.Text = App.Profile.CurrentInputLayoutElement.ClippingHeightWithFit.ToString();
    this.AttachChangedEventHandlers();
  }

  /// @copydoc IProfileToControl.AttachChangedEventHandlers
  public void AttachChangedEventHandlers() {
    this.ClippingX.TextChanged += clippingX_TextChanged;
    this.ClippingY.TextChanged += clippingY_TextChanged;
    this.ClippingWidth.TextChanged += clippingWidth_TextChanged;
    this.ClippingHeight.TextChanged += clippingHeight_TextChanged;
  }

  /// @copydoc IProfileToControl.DetachChangedEventHandlers
  public void DetachChangedEventHandlers() {
    this.ClippingX.TextChanged -= clippingX_TextChanged;
    this.ClippingY.TextChanged -= clippingY_TextChanged;
    this.ClippingWidth.TextChanged -= clippingWidth_TextChanged;
    this.ClippingHeight.TextChanged -= clippingHeight_TextChanged;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void fit_Click(object sender, RoutedEventArgs e) {
    if (!this.Fit.IsChecked.HasValue) return;

    App.Profile.CurrentOutputLayoutElement.Fit = (bool)this.Fit.IsChecked;
    this.UpdateByProfile();
  }

  private void CommonAreaSelect(ExternalAPI.RECT boundScreenRect, WindowTypes nextWindowType) {
    // ダイアログの準備
    var dialog = new AreaSelectWindow();
    dialog.Left   = App.Profile.CurrentInputLayoutElement.ScreenClippingXWithFit;
    dialog.Top    = App.Profile.CurrentInputLayoutElement.ScreenClippingYWithFit;
    dialog.Width  = App.Profile.CurrentInputLayoutElement.ClippingWidthWithFit;
    dialog.Height = App.Profile.CurrentInputLayoutElement.ClippingHeightWithFit;

    // カラーの変更
    switch (nextWindowType) {
      case WindowTypes.Normal: {
        // 更に現在のTypeによって色を分ける
        switch (App.Profile.CurrentInputLayoutElement.WindowType) {
          case WindowTypes.DesktopListView: {
            dialog.WindowBorder.BorderBrush = desktopListViewBorderBrush;
            dialog.WindowGrid.Background = desktopListViewGridBrush;
            break;
          }
          case WindowTypes.Desktop: {
            dialog.WindowBorder.BorderBrush = desktopBorderBrush;
            dialog.WindowGrid.Background = desktopGridBrush;
            break;
          }
          default: {
            dialog.WindowBorder.BorderBrush = normalBorderBrush;
            dialog.WindowGrid.Background = normalGridBrush;
            break;
          }
        }
        break;
      }
      case WindowTypes.DesktopListView: {
        dialog.WindowBorder.BorderBrush = desktopListViewBorderBrush;
        dialog.WindowGrid.Background = desktopListViewGridBrush;
        break;
      }
      case WindowTypes.Desktop: {
        dialog.WindowBorder.BorderBrush = desktopBorderBrush;
        dialog.WindowGrid.Background = desktopGridBrush;
        break;
      }
      default : {
        Debug.Fail("CommonAreaSelect: Invalid WindowTypes");
        break;
      }
    }

    // ダイアログの表示
    var result = dialog.ShowDialog();
    if (!result.HasValue || !(bool)result) return;

    // 結果をRECTにまとめる
    var nextScreenRect = new ExternalAPI.RECT {
      Left    = (int)dialog.Left,
      Top     = (int)dialog.Top,
      Right   = (int)(dialog.Left + dialog.Width),
      Bottom  = (int)(dialog.Top + dialog.Height)
    };

    // ウィンドウの領域とIntersectをとる
    ExternalAPI.RECT intersectScreenRect;
    var intersectResult = ExternalAPI.IntersectRect(out intersectScreenRect,
                                                    ref boundScreenRect,
                                                    ref nextScreenRect);
    if (!intersectResult) return;

    // 結果をProfileに書き込み
    var intersectWidth = intersectScreenRect.Right - intersectScreenRect.Left;
    var intersectHeight = intersectScreenRect.Bottom - intersectScreenRect.Top;

    switch (nextWindowType) {
      case WindowTypes.Normal: {
        // nop
        break;
      }
      case WindowTypes.Desktop: {
        App.Profile.CurrentOutputLayoutElement.SetWindowToDesktop();
        break;
      }
      case WindowTypes.DesktopListView: {
        App.Profile.CurrentOutputLayoutElement.SetWindowToDesktopListView();
        break;
      }
      default : {
        Debug.Fail("CommonAreaSelect: Invalid WindowTypes");
        break;
      }
    }
    App.Profile.CurrentOutputLayoutElement.Fit = false;
    App.Profile.CurrentOutputLayoutElement.ClippingXWithoutFit =
        intersectScreenRect.Left - boundScreenRect.Left;
    App.Profile.CurrentOutputLayoutElement.ClippingYWithoutFit = 
        intersectScreenRect.Top - boundScreenRect.Top;
    App.Profile.CurrentOutputLayoutElement.ClippingWidthWithoutFit =
        intersectWidth;
    App.Profile.CurrentOutputLayoutElement.ClippingHeightWithoutFit =
        intersectHeight;
    
    // 関連するコントロールの更新
    if (nextWindowType != WindowTypes.Normal) {
      // コマンドをMainWindowに送信して関連するコントロールを更新
      Commands.ChangeTargetWindowCommand.Execute(null, null);
    } else {
      // 変更はAreaコントロールのみに収まるのでコマンドは発行しない
      this.UpdateByProfile();
    }
  }

  private void AreaSelect_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(App.Profile.CurrentInputLayoutElement.ScreenWindowRect,
                          WindowTypes.Normal);
  }

  private void ListView_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(Utilities.VirtualScreenRect, WindowTypes.DesktopListView);
  }

  private void Desktop_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(Utilities.VirtualScreenRect, WindowTypes.Desktop);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  private void fit_Checked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = false;
    this.ClippingY.IsEnabled = false;
    this.ClippingWidth.IsEnabled = false;
    this.ClippingHeight.IsEnabled = false;
  }

  private void fit_Unchecked(object sender, RoutedEventArgs e) {
    this.ClippingX.IsEnabled = true;
    this.ClippingY.IsEnabled = true;
    this.ClippingWidth.IsEnabled = true;
    this.ClippingHeight.IsEnabled = true;
  }

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------

  private bool TryParseClippingParameters(TextBox textBox, int lowerBound, int upperBound, out int parsedValue) {
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

  private void clippingX_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingX, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingXWithoutFit = parsedValue;
    }
  }

  private void clippingY_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingY, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingYWithoutFit = parsedValue;
    }
  }

  private void clippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingWidth, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingWidthWithoutFit = parsedValue;
    }
  }

  private void clippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentInputLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.ClippingHeight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentOutputLayoutElement.ClippingHeightWithoutFit = parsedValue;
    }
  }
}
}   // namespace SCFF.GUI.Controls
