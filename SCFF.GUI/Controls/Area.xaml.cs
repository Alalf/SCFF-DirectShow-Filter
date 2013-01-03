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
/// クリッピング領域設定用

namespace SCFF.GUI.Controls {

using SCFF.Common;
using SCFF.Common.Ext;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/// クリッピング領域設定用
public partial class Area : UserControl, IUpdateByProfile {

  /// コンストラクタ
  public Area() {
    InitializeComponent();
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile.UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
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
    this.DetachProfileChangedEventHandlers();
    this.ClippingX.Text = App.Profile.CurrentInputLayoutElement.ClippingXWithFit.ToString();
    this.ClippingY.Text = App.Profile.CurrentInputLayoutElement.ClippingYWithFit.ToString();
    this.ClippingWidth.Text = App.Profile.CurrentInputLayoutElement.ClippingWidthWithFit.ToString();
    this.ClippingHeight.Text = App.Profile.CurrentInputLayoutElement.ClippingHeightWithFit.ToString();
    this.AttachProfileChangedEventHandlers();
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    // current以外のデータを表示する必要はない
    this.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile.AttachProfileChangedEventHandlers
  public void AttachProfileChangedEventHandlers() {
    this.ClippingX.TextChanged += clippingX_TextChanged;
    this.ClippingY.TextChanged += clippingY_TextChanged;
    this.ClippingWidth.TextChanged += clippingWidth_TextChanged;
    this.ClippingHeight.TextChanged += clippingHeight_TextChanged;
  }

  /// @copydoc IUpdateByProfile.DetachProfileChangedEventHandlers
  public void DetachProfileChangedEventHandlers() {
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
    this.UpdateByCurrentProfile();
  }

  private void CommonAreaSelect(Rect boundScreenRect, WindowTypes nextWindowType) {
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
            dialog.WindowBorder.BorderBrush = BrushesAndPens.CurrentDesktopListViewBrush;
            dialog.WindowGrid.Background = BrushesAndPens.DesktopListViewBrush;
            break;
          }
          case WindowTypes.Desktop: {
            dialog.WindowBorder.BorderBrush = BrushesAndPens.CurrentDesktopBrush;
            dialog.WindowGrid.Background = BrushesAndPens.DesktopBrush;
            break;
          }
          default: {
            dialog.WindowBorder.BorderBrush = BrushesAndPens.CurrentNormalBrush;
            dialog.WindowGrid.Background = BrushesAndPens.NormalBrush;
            break;
          }
        }
        break;
      }
      case WindowTypes.DesktopListView: {
        dialog.WindowBorder.BorderBrush = BrushesAndPens.CurrentDesktopListViewBrush;
        dialog.WindowGrid.Background = BrushesAndPens.DesktopListViewBrush;
        break;
      }
      case WindowTypes.Desktop: {
        dialog.WindowBorder.BorderBrush = BrushesAndPens.CurrentDesktopBrush;
        dialog.WindowGrid.Background = BrushesAndPens.DesktopBrush;
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
    var nextScreenRect = new Rect(dialog.Left, dialog.Top, dialog.Width, dialog.Height);

    // ウィンドウの領域とIntersectをとる
    if (!nextScreenRect.IntersectsWith(boundScreenRect)) return;
    nextScreenRect.Intersect(boundScreenRect);

    // 結果をProfileに書き込み

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
        (int)(nextScreenRect.X - boundScreenRect.Left);
    App.Profile.CurrentOutputLayoutElement.ClippingYWithoutFit = 
        (int)(nextScreenRect.Y - boundScreenRect.Top);
    App.Profile.CurrentOutputLayoutElement.ClippingWidthWithoutFit =
        (int)nextScreenRect.Width;
    App.Profile.CurrentOutputLayoutElement.ClippingHeightWithoutFit =
        (int)nextScreenRect.Height;
    
    // コマンドをMainWindowに送信して関連するコントロールを更新
    UpdateCommands.UpdateTargetWindowByCurrentProfile.Execute(null, null);
  }

  private void AreaSelect_Click(object sender, RoutedEventArgs e) {
    var boundScreenRect = new Rect {
      X = App.Profile.CurrentInputLayoutElement.ScreenWindowX,
      Y = App.Profile.CurrentInputLayoutElement.ScreenWindowY,
      Width = App.Profile.CurrentInputLayoutElement.WindowWidth,
      Height = App.Profile.CurrentInputLayoutElement.WindowHeight
    };
    this.CommonAreaSelect(boundScreenRect, WindowTypes.Normal);
  }

  /// 仮想ディスプレイのデータをRECT化したプロパティ
  /// @todo(me) 現在Desktop/DesktopListViewで使い回ししているが、問題が発生する可能性あり
  private readonly Rect virtualScreenRect = new Rect {
    X       = User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
    Y       = User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN),
    Width   = User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
    Height  = User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN)
  };

  private void ListView_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(this.virtualScreenRect, WindowTypes.DesktopListView);
  }

  private void Desktop_Click(object sender, RoutedEventArgs e) {
    this.CommonAreaSelect(this.virtualScreenRect, WindowTypes.Desktop);
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
