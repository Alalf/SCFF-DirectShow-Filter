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
  }

  private void UpdateRecentProfiles() {
    if (App.Options().RecentProfile1() != string.Empty) {
      this.recentProfile1.Header = "1 " + App.Options().RecentProfile1() + "(_1)";
      this.recentProfile1.IsEnabled = true;
    } else {
      this.recentProfile1.Header = "1 (_1)";
      this.recentProfile1.IsEnabled = false;
    }
    if (App.Options().RecentProfile2() != string.Empty) {
      this.recentProfile2.Header = "2 " + App.Options().RecentProfile2() + "(_2)";
      this.recentProfile2.IsEnabled = true;
    } else {
      this.recentProfile2.Header = "2 (_2)";
      this.recentProfile2.IsEnabled = false;
    }
    if (App.Options().RecentProfile3() != string.Empty) {
      this.recentProfile3.Header = "3 " + App.Options().RecentProfile3() + "(_3)";
      this.recentProfile3.IsEnabled = true;
    } else {
      this.recentProfile3.Header = "3 (_3)";
      this.recentProfile3.IsEnabled = false;
    }
    if (App.Options().RecentProfile4() != string.Empty) {
      this.recentProfile4.Header = "4 " + App.Options().RecentProfile4() + "(_4)";
      this.recentProfile4.IsEnabled = true;
    } else {
      this.recentProfile4.Header = "4 (_4)";
      this.recentProfile4.IsEnabled = false;
    }
    if (App.Options().RecentProfile5() != string.Empty) {
      this.recentProfile5.Header = "5 " + App.Options().RecentProfile5() + "(_5)";
      this.recentProfile5.IsEnabled = true;
    } else {
      this.recentProfile5.Header = "5 (_5)";
      this.recentProfile5.IsEnabled = false;
    }
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
    // アプリケーションの設定からUIに関連するものを読み込む
    // 存在しない場合は勝手にデフォルト値が読み込まれる・・・はず
    App.Options().Load();

    // Recent Profiles
    this.UpdateRecentProfiles();

    // MainWindow
    this.Left = App.Options().MainWindowLeft();
    this.Top = App.Options().MainWindowTop();
    this.Width = App.Options().MainWindowWidth();
    this.Height = App.Options().MainWindowHeight();
    this.WindowState = (WindowState)App.Options().MainWindowState();
    
    // MainWindow Expanders
    this.areaExpander.IsExpanded = App.Options().AreaExpanderIsExpanded();
    this.optionsExpander.IsExpanded = App.Options().OptionsExpanderIsExpanded();
    this.resizeMethodExpander.IsExpanded = App.Options().ResizeMethodExpanderIsExpanded();
    this.layoutExpander.IsExpanded = App.Options().LayoutExpanderIsExpanded();

    // SCFF Options
    this.autoApply.IsChecked = App.Options().AutoApply();
    this.layoutPreview.IsChecked = App.Options().LayoutPreview();
    this.layoutBorder.IsChecked = App.Options().LayoutBorder();
    this.layoutSnap.IsChecked = App.Options().LayoutSnap();

    // SCFF Menu Options
    this.compactView.IsChecked = App.Options().CompactView();
    this.forceAeroOn.IsChecked = App.Options().ForceAeroOn();
    this.restoreLastProfile.IsChecked = App.Options().RestoreLastProfile();
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
    // 念のため取れる範囲でUIから最新のデータを取ってくる

    // MainWindow
    if (this.WindowState == WindowState.Normal) {
      App.Options().SetMainWindowOptions(this.Left, this.Top,
          this.Width, this.Height, (Options.WindowState)this.WindowState);
    } else {
      App.Options().SetMainWindowOptions(this.RestoreBounds.Left,
          this.RestoreBounds.Top, this.RestoreBounds.Width,
          this.RestoreBounds.Height, (Options.WindowState)this.WindowState);
    }

    // MainWindow Expanders
    App.Options().SetMainWindowExpanders(this.areaExpander.IsExpanded,
        this.optionsExpander.IsExpanded, this.resizeMethodExpander.IsExpanded,
        this.layoutExpander.IsExpanded);

    // SCFF Options
    App.Options().SetSCFFOptions((bool)this.autoApply.IsChecked,
        (bool)this.layoutPreview.IsChecked,
        (bool)this.layoutBorder.IsChecked,
        (bool)this.layoutSnap.IsChecked);

    // SCFF Menu Options
    App.Options().SetSCFFMenuOptions((bool)this.compactView.IsChecked,
        (bool)this.forceAeroOn.IsChecked,
        (bool)this.restoreLastProfile.IsChecked);

    App.Options().Save();
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
    if (this.Height < SCFF.Common.Defaults.MainWindowHeight) {
      this.Height = SCFF.Common.Defaults.MainWindowHeight;
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
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
  }





}
}
