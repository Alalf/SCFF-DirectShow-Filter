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

/// MainWindowのコードビハインド
public partial class MainWindow : Window {

  /// コンストラクタ
  public MainWindow() {
    this.InitializeComponent();
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
    // アプリケーションの設定からUIに関連するものを読み込む
    // 存在しない場合は勝手にデフォルト値が読み込まれる・・・はず
    OptionsINIFile.Load(App.Options);

    this.UpdateByOptions();
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
    this.SaveOptions();

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
