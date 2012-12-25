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

/// @file SCFF.GUI/MainWindow.xaml.cs
/// MainWindowのコードビハインド

namespace SCFF.GUI {

using Microsoft.Win32;
using Microsoft.Windows.Shell;
using SCFF.Common;
using System.Windows;
using System.Windows.Input;

/// MainWindowのコードビハインド
public partial class MainWindow : Window {

  /// コンストラクタ
  public MainWindow() {    
    //-----------------------------------------------------------------
    // Controls
    //-----------------------------------------------------------------
    this.InitializeComponent();
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void OnLoaded(object sender, RoutedEventArgs e) {
    this.UpdateByOptions();
    this.UpdateByProfile();
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
    this.SaveOptions();
  }

  //===================================================================
  // Options
  //===================================================================

  /// 最近使用したプロファイルメニューの更新
  private void UpdateRecentProfiles() {
    for (int i = 0; i < Constants.RecentProfilesLength; ++i ) {
      var isEmpty = App.Options.GetRecentProfile(i) == string.Empty;
      var header = (i+1) + " " + (isEmpty ? "" : App.Options.GetRecentProfile(i)) +
        "(_" + (i+1) + ")";

      switch (i) {
        case 0:
          this.RecentProfile1.IsEnabled = !isEmpty;
          this.RecentProfile1.Header = header;
          break;
        case 1:
          this.RecentProfile2.IsEnabled = !isEmpty;
          this.RecentProfile2.Header = header;
          break;
        case 2:
          this.RecentProfile3.IsEnabled = !isEmpty;
          this.RecentProfile3.Header = header;
          break;
        case 3:
          this.RecentProfile4.IsEnabled = !isEmpty;
          this.RecentProfile4.Header = header;
          break;
        case 4:
          this.RecentProfile5.IsEnabled = !isEmpty;
          this.RecentProfile5.Header = header;
          break;
      }
    }
  }

  /// 設定からUIを更新
  private void UpdateByOptions() {
    // Recent Profiles
    this.UpdateRecentProfiles();

    // MainWindow
    this.Left         = App.Options.TmpMainWindowLeft;
    this.Top          = App.Options.TmpMainWindowTop;
    this.Width        = App.Options.TmpMainWindowWidth;
    this.Height       = App.Options.TmpMainWindowHeight;
    this.WindowState  = (System.Windows.WindowState)App.Options.TmpMainWindowState;
    
    // MainWindow Expanders
    this.AreaExpander.IsExpanded          = App.Options.TmpAreaIsExpanded;
    this.OptionsExpander.IsExpanded       = App.Options.TmpOptionsIsExpanded;
    this.ResizeMethodExpander.IsExpanded  = App.Options.TmpResizeMethodIsExpanded;
    this.LayoutExpander.IsExpanded        = App.Options.TmpLayoutIsExpanded;

    // SCFF Options
    this.AutoApply.IsChecked = App.Options.AutoApply;
    this.LayoutPreview.IsChecked = App.Options.LayoutPreview;
    this.LayoutBorder.IsChecked = App.Options.LayoutBorder;
    this.LayoutSnap.IsChecked = App.Options.LayoutSnap;

    // SCFF Menu Options
    this.CompactView.IsChecked = App.Options.TmpCompactView;
    this.ForceAeroOn.IsChecked = App.Options.ForceAeroOn;
    this.RestoreLastProfile.IsChecked = App.Options.TmpRestoreLastProfile;
  }

  /// UIから設定にデータを保存
  private void SaveOptions() {
    // Tmp接頭辞のプロパティだけはここで更新する必要がある
    var isNormal = this.WindowState == System.Windows.WindowState.Normal;
    App.Options.TmpMainWindowLeft = isNormal ? this.Left : this.RestoreBounds.Left;
    App.Options.TmpMainWindowTop = isNormal ? this.Top : this.RestoreBounds.Top;
    App.Options.TmpMainWindowWidth = isNormal ? this.Width : this.RestoreBounds.Width;
    App.Options.TmpMainWindowHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    App.Options.TmpMainWindowState = (SCFF.Common.WindowState)this.WindowState;

    // MainWindow Expanders
    App.Options.TmpAreaIsExpanded = this.AreaExpander.IsExpanded;
    App.Options.TmpOptionsIsExpanded = this.OptionsExpander.IsExpanded;
    App.Options.TmpResizeMethodIsExpanded = this.ResizeMethodExpander.IsExpanded;
    App.Options.TmpLayoutIsExpanded = this.LayoutExpander.IsExpanded;

    // SCFF Menu Options
    App.Options.TmpCompactView = this.CompactView.IsChecked;
    App.Options.TmpRestoreLastProfile = this.RestoreLastProfile.IsChecked;
  }

  //===================================================================
  // Profile
  //===================================================================

  /// プロファイルからUIを更新
  private void UpdateByProfile() {

    // TargetWindow
    this.TargetWindow.UpdateByProfile();

    // Area
    this.Area.UpdateByProfile();

    // Options
    this.Options.UpdateByProfile();

    // Resize Method
    this.ResizeMethod.UpdateByProfile();

    // Layout
    this.LayoutParameter.UpdateByProfile();
  }

  //===================================================================
  // ApplicationCommandsハンドラ
  //===================================================================

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) すでに保存されていない場合はダイアログをだす
    if ((string)this.Tag == "") {
      var save = new SaveFileDialog();
      save.Title = "SCFF.GUI";
      save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
      var saveResult = save.ShowDialog();
      if (saveResult.HasValue && (bool)saveResult) {
        /// @todo(me) 実装
        MessageBox.Show(save.FileName);
        App.Options.AddRecentProfile(save.FileName);
        this.UpdateRecentProfiles();
      }
    }
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
    var result = MessageBox.Show("Do you want to save changes?",
                                 "SCFF.GUI",
                                 MessageBoxButton.YesNoCancel,
                                 MessageBoxImage.Warning,
                                 MessageBoxResult.Yes);
    switch (result) {
      case MessageBoxResult.No: {
        App.Profile.ResetProfile();
        this.LayoutTab.ResetTabs();
        this.UpdateByProfile();
        break;
      }
      case MessageBoxResult.Yes: {
        var save = new SaveFileDialog();
        save.Title = "SCFF.GUI";
        save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
        var saveResult = save.ShowDialog();
        if (saveResult.HasValue && (bool)saveResult) {
          /// @todo(me) 実装
          MessageBox.Show(save.FileName);
          App.Options.AddRecentProfile(save.FileName);
          this.UpdateRecentProfiles();

          App.Profile.ResetProfile();
          this.LayoutTab.ResetTabs();
          this.UpdateByProfile();
        }
        break;
      }
    }
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) Newと似たコードが必要だがかなりめんどくさい。あとでかく
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
    var save = new SaveFileDialog();
    save.Title = "SCFF.GUI";
    save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
    var saveResult = save.ShowDialog();
    if (saveResult.HasValue && (bool)saveResult) {
      /// @todo(me) 実装
      MessageBox.Show(save.FileName);
      App.Options.AddRecentProfile(save.FileName);
      this.UpdateRecentProfiles();
    }
  }

  //===================================================================
  // Windows.Shell.SystemCommandsハンドラ
  //===================================================================
  
	private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.CloseWindow(this);
	}

	private void MaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MaximizeWindow(this);
	}

	private void MinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MinimizeWindow(this);
	}

	private void RestoreWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.RestoreWindow(this);
	}

  //===================================================================
  // SCFF.GUI.Commandsハンドラ
  //===================================================================

  private void AddLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.AddLayoutElement();
    this.LayoutTab.AddTab();

    this.UpdateByProfile();
  }

  private void AddLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanAddLayoutElement();
  }

  private void RemoveLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();
    this.LayoutTab.RemoveCurrentTab();
    
    this.UpdateByProfile();
  }

  private void RemoveLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanRemoveLayoutElement();
  }

  private void ChangeCurrentLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.UpdateByProfile();
  }

  private void ChangeTargetWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) ここでClippingWithoutFitの調整を行う。ついでにバックアップも？

    // TargetWindow
    this.TargetWindow.UpdateByProfile();

    // Area
    this.Area.UpdateByProfile();

    // Layout
    this.LayoutParameter.UpdateByProfile();
  }

  //===================================================================
  // *Changedではないが、値が変更したときに発生するイベントハンドラ
  // プロパティへの代入で発生するので、App.Profileの変更は禁止
  // このためthis.UpdateByProfile()の必要はない(呼び出しても良い)
  //===================================================================

  private void compactView_Checked(object sender, RoutedEventArgs e) {
    this.OptionsExpander.Visibility = Visibility.Collapsed;
    this.ResizeMethodExpander.Visibility = Visibility.Collapsed;
    this.LayoutExpander.IsExpanded = false;
    this.Width = Constants.CompactMainWindowWidth;
    this.Height = Constants.CompactMainWindowHeight;
  }

  private void compactView_Unchecked(object sender, RoutedEventArgs e) {
    this.OptionsExpander.Visibility = Visibility.Visible;
    this.ResizeMethodExpander.Visibility = Visibility.Visible;
  }

  //===================================================================
  // *Changed以外のイベントハンドラ
  // プロパティへの代入では発生しないので、
  // この中でのApp.Optionsの変更は許可
  //===================================================================

  private void forceAeroOn_Click(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = this.ForceAeroOn.IsChecked;
  }

  private void autoApply_Click(object sender, RoutedEventArgs e) {
    if (!this.AutoApply.IsChecked.HasValue) return;

    App.Options.AutoApply = (bool)this.AutoApply.IsChecked;
  }

  private void layoutPreview_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutPreview.IsChecked.HasValue) return;

    App.Options.LayoutPreview = (bool)this.LayoutPreview.IsChecked;
  }

  private void layoutSnap_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutSnap.IsChecked.HasValue) return;

    App.Options.LayoutSnap = (bool)this.LayoutSnap.IsChecked;
  }

  private void layoutBorder_Click(object sender, RoutedEventArgs e) {
    if (this.LayoutBorder.IsChecked.HasValue) return;

    App.Options.LayoutBorder = (bool)this.LayoutBorder.IsChecked;
  }
}
}
