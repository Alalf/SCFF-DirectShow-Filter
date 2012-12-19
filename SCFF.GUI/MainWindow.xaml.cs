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
    if (App.Options.GetRecentProfile(0) == string.Empty) {
      this.recentProfile1.Header = "1 (_1)";
      this.recentProfile1.IsEnabled = false;
    } else {
      this.recentProfile1.Header = "1 " + App.Options.GetRecentProfile(0) + "(_1)";
      this.recentProfile1.IsEnabled = true;
    }
    if (App.Options.GetRecentProfile(1) == string.Empty) {
      this.recentProfile2.Header = "2 (_2)";
      this.recentProfile2.IsEnabled = false;
    } else {
      this.recentProfile2.Header = "2 " + App.Options.GetRecentProfile(1) + "(_2)";
      this.recentProfile2.IsEnabled = true;
    }
    if (App.Options.GetRecentProfile(2) == string.Empty) {
      this.recentProfile3.Header = "3 (_3)";
      this.recentProfile3.IsEnabled = false;
    } else {
      this.recentProfile3.Header = "3 " + App.Options.GetRecentProfile(2) + "(_3)";
      this.recentProfile3.IsEnabled = true;
    }
    if (App.Options.GetRecentProfile(3) == string.Empty) {
      this.recentProfile4.Header = "4 (_4)";
      this.recentProfile4.IsEnabled = false;
    } else {
      this.recentProfile4.Header = "4 " + App.Options.GetRecentProfile(3) + "(_4)";
      this.recentProfile4.IsEnabled = true;
    }
    if (App.Options.GetRecentProfile(4) == string.Empty) {
      this.recentProfile5.Header = "5 (_5)";
      this.recentProfile5.IsEnabled = false;
    } else {
      this.recentProfile5.Header = "5 " + App.Options.GetRecentProfile(4) + "(_5)";
      this.recentProfile5.IsEnabled = true;
    }
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
    // アプリケーションの設定からUIに関連するものを読み込む
    // 存在しない場合は勝手にデフォルト値が読み込まれる・・・はず
    OptionsINIFile.Load(App.Options);

    // Recent Profiles
    this.UpdateRecentProfiles();

    // MainWindow
    this.Left = App.Options.TmpMainWindowLeft;
    this.Top = App.Options.TmpMainWindowTop;
    this.Width = App.Options.TmpMainWindowWidth;
    this.Height = App.Options.TmpMainWindowHeight;
    this.WindowState = (WindowState)App.Options.TmpMainWindowState;
    
    // MainWindow Expanders
    this.areaExpander.IsExpanded = App.Options.TmpAreaIsExpanded;
    this.optionsExpander.IsExpanded = App.Options.TmpOptionsIsExpanded;
    this.resizeMethodExpander.IsExpanded = App.Options.TmpResizeMethodIsExpanded;
    this.layoutExpander.IsExpanded = App.Options.TmpLayoutIsExpanded;

    // SCFF Options
    this.autoApply.IsChecked = App.Options.AutoApply;
    this.layoutPreview.IsChecked = App.Options.LayoutPreview;
    this.layoutBorder.IsChecked = App.Options.LayoutBorder;
    this.layoutSnap.IsChecked = App.Options.LayoutSnap;

    // SCFF Menu Options
    this.compactView.IsChecked = App.Options.TmpCompactView;
    this.forceAeroOn.IsChecked = App.Options.ForceAeroOn;
    this.restoreLastProfile.IsChecked = App.Options.TmpRestoreLastProfile;
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
    // Tmp接頭辞のプロパティだけはここで更新する必要がある
    var isNormal = this.WindowState == WindowState.Normal;
    App.Options.TmpMainWindowLeft = isNormal ? this.Left : this.RestoreBounds.Left;
    App.Options.TmpMainWindowTop = isNormal ? this.Top : this.RestoreBounds.Top;
    App.Options.TmpMainWindowWidth = isNormal ? this.Width : this.RestoreBounds.Width;
    App.Options.TmpMainWindowHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    App.Options.TmpMainWindowState = (Options.WindowState)this.WindowState;

    // MainWindow Expanders
    App.Options.TmpAreaIsExpanded = this.areaExpander.IsExpanded;
    App.Options.TmpOptionsIsExpanded = this.optionsExpander.IsExpanded;
    App.Options.TmpResizeMethodIsExpanded = this.resizeMethodExpander.IsExpanded;
    App.Options.TmpLayoutIsExpanded = this.layoutExpander.IsExpanded;

    // SCFF Menu Options
    App.Options.TmpCompactView = this.compactView.IsChecked;
    App.Options.TmpRestoreLastProfile = this.restoreLastProfile.IsChecked;

    OptionsINIFile.Save(App.Options);
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
  }

  private void MenuItem_Checked_1(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Collapsed;
    this.resizeMethodExpander.Visibility = Visibility.Collapsed;
    this.layoutExpander.IsExpanded = false;
    this.Width = Defaults.CompactMainWindowWidth;
    this.Height = Defaults.CompactMainWindowHeight;
  }

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    //App.Options.AddRecentProfile(e.ToString());
    //this.UpdateRecentProfiles();
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void autoApply_Checked(object sender, RoutedEventArgs e) {
    App.Options.AutoApply = true;
  }

  private void autoApply_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.AutoApply = false;
  }

  private void forceAeroOn_Checked(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = true;
  }

  private void forceAeroOn_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = false;
  }

  private void layoutPreview_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutPreview = true;
  }

  private void layoutPreview_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutPreview = false;
  }

  private void layoutBorder_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutBorder = true;
  }

  private void layoutBorder_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutBorder = false;
  }

  private void layoutSnap_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutSnap = false;
  }

  private void layoutSnap_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutSnap = true;
  }

  private void enableFilter_Checked(object sender, RoutedEventArgs e) {
    this.filterLGBlur.IsEnabled = true;
    this.filterLSharpen.IsEnabled = true;
    // this.filterCVShift.IsEnabled = true;
    this.filterCGBlur.IsEnabled = true;
    this.filterCSharpen.IsEnabled = true;
    // this.filterCHShift.IsEnabled = true;
  }

  private void enableFilter_Unchecked(object sender, RoutedEventArgs e) {
    this.filterLGBlur.IsEnabled = false;
    this.filterLSharpen.IsEnabled = false;
    // this.filterCVShift.IsEnabled = false;
    this.filterCGBlur.IsEnabled = false;
    this.filterCSharpen.IsEnabled = false;
    // this.filterCHShift.IsEnabled = false;
  }


}
}
