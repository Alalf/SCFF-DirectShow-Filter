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

namespace SCFF.GUI {

using System.Windows;
using System.Windows.Controls;
using SCFF.Common;

public partial class MainWindow : Window {

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
}
}