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

using System.Windows;
using SCFF.Common;

public partial class MainWindow : Window {

  //===================================================================
  // Options
  //===================================================================

  /// 最近使用したプロファイルメニューの更新
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

  /// 設定からUIを更新
  private void UpdateByOptions() {
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

  /// UIから設定にデータを保存
  private void SaveOptions() {
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
  }

  //===================================================================
  // Profile
  //===================================================================
  
  /// タブコントロールの更新
  private void UpdateLayoutTab() {
    this.layoutTab.Items.Clear();
    for (int i = 0; i < App.Profile.LayoutElementCount; ++i) {
      this.layoutTab.Items.Add(i);
    }
  }

  /// プロファイルからUIを更新
  private void UpdateByProfile() {
    // Window Caption
    this.windowCaption.Text = App.Profile.Current.WindowCaption;
    
  }
}
}