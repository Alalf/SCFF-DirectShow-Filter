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

      this.MouseLeftButtonDown += (sender, e) => this.DragMove();
    }

		private void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.CloseWindow((Window)e.Parameter);
		}

		private void maximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.MaximizeWindow((Window)e.Parameter);
		}

		private void minimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.MinimizeWindow((Window)e.Parameter);
		}

		private void restoreWindow_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			SystemCommands.RestoreWindow((Window)e.Parameter);
		}
  }
}
