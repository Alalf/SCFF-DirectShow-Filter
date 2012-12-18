// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

namespace SCFF.GUI {

using Microsoft.Windows.Shell;
using System.Windows;
using System.Windows.Input;
using SCFF.Common;

/// MainWindow.xaml の相互作用ロジック
public partial class MainWindow : Window {
  /// コンストラクタ
  public MainWindow() {
    this.InitializeComponent();

    // 明示的にチェックを外してみる
    //this.compactViewMenu.IsChecked = false;
  }

  private void mainWindow_Loaded(object sender, RoutedEventArgs e) {

  }

	private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.CloseWindow((Window)e.Parameter);
	}

	private void MaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MaximizeWindow((Window)e.Parameter);
	}

	private void MinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MinimizeWindow((Window)e.Parameter);
	}

	private void RestoreWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.RestoreWindow((Window)e.Parameter);
	}

  private void MenuItem_Unchecked_1(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Visible;
    this.resizeMethodExpander.Visibility = Visibility.Visible;
    if (this.Height < SCFF.Common.Defaults.MainWindowWidth) {
      this.Height = SCFF.Common.Defaults.MainWindowWidth;
    }
  }

  private void MenuItem_Checked_1(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Collapsed;
    this.resizeMethodExpander.Visibility = Visibility.Collapsed;
    this.layoutExpander.IsExpanded = false;
    this.Width = SCFF.Common.Defaults.CompactMainWindowWidth;
    this.Height = SCFF.Common.Defaults.CompactMainWindowHeight;
  }

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    var options = new Options();
    options.Save();
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
    var options = new Options();
    options.Save();
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
    var options = new Options();
    options.Save();
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
    var options = new Options();
    options.Save();
  }

}
}
