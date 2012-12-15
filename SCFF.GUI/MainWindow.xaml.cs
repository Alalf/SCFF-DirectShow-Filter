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

  /// MainWindow.xaml の相互作用ロジック
  public partial class MainWindow : Window {
    /// コンストラクタ
    public MainWindow() {
      this.InitializeComponent();
    }

		private void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e) {
			SystemCommands.CloseWindow((Window)e.Parameter);
		}

		private void maximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
			SystemCommands.MaximizeWindow((Window)e.Parameter);
		}

		private void minimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
			SystemCommands.MinimizeWindow((Window)e.Parameter);
		}

		private void restoreWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
			SystemCommands.RestoreWindow((Window)e.Parameter);
		}

    private void MenuItem_Click_1(object sender, RoutedEventArgs e) {
      // this.areaExpander.Visibility = Visibility.Collapsed;
      this.optionsExpander.Visibility = Visibility.Collapsed;
      this.resizeMethodExpander.Visibility = Visibility.Collapsed;
      // this.layoutTab.Visibility = Visibility.Collapsed;
      // this.layoutExpander.Visibility = Visibility.Visible;
      this.layoutExpander.IsExpanded = false;
    }

    private void MenuItem_Click_2(object sender, RoutedEventArgs e) {
      // this.areaExpander.Visibility = Visibility.Collapsed;
      this.optionsExpander.Visibility = Visibility.Visible;
      this.optionsExpander.IsExpanded = true;
      this.resizeMethodExpander.Visibility = Visibility.Visible;
      this.resizeMethodExpander.IsExpanded = true;
      // this.layoutTab.Visibility = Visibility.Collapsed;
      // this.layoutExpander.Visibility = Visibility.Visible;
      this.layoutExpander.IsExpanded = false;
    }

    private void MenuItem_Click_3(object sender, RoutedEventArgs e) {
      // this.areaExpander.Visibility = Visibility.Collapsed;
      this.optionsExpander.Visibility = Visibility.Visible;
      this.optionsExpander.IsExpanded = true;
      this.resizeMethodExpander.Visibility = Visibility.Visible;
      this.resizeMethodExpander.IsExpanded = true;
      //this.layoutTab.Visibility = Visibility.Visible;
      // this.layoutExpander.Visibility = Visibility.Visible;
      this.layoutExpander.IsExpanded = true;
    }
  }
}
