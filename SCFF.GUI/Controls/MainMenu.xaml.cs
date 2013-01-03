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

/// @file SCFF.GUI/Controls/MainMenu.xaml.cs
/// メインメニュー

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Windows;
using System.Windows.Controls;

/// メインメニュー
public partial class MainMenu : UserControl, IUpdateByOptions {

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public MainMenu() {
    InitializeComponent();
  }

  //===================================================================
  // IUpdateByOptionsの実装
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

  /// @copydoc IUpdateByOptions.UpdateByOptions
  public void UpdateByOptions() {
    this.UpdateRecentProfiles();
    this.CompactView.IsChecked = App.Options.CompactView;
    this.ForceAeroOn.IsChecked = App.Options.ForceAeroOn;
    this.RestoreLastProfile.IsChecked = App.Options.RestoreLastProfile;
  }

  /// @copydoc IUpdateByOptions.DetachOptionsChangedEventHandlers
  public void DetachOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByOptions.AttachOptionsChangedEventHandlers
  public void AttachOptionsChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void ForceAeroOn_Click(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = this.ForceAeroOn.IsChecked;
    Commands.SetAero.Execute(null, null);
  }

  private void CompactView_Click(object sender, RoutedEventArgs e) {
    App.Options.CompactView = this.CompactView.IsChecked;
    Commands.SetCompactView.Execute(null, null);
  }

  private void RestoreLastProfile_Click(object sender, RoutedEventArgs e) {
    App.Options.RestoreLastProfile = this.CompactView.IsChecked;
  }

  private void RecentProfile1_Click(object sender, RoutedEventArgs e) {
    /// @todo(me) 実装
  }

  private void RecentProfile2_Click(object sender, RoutedEventArgs e) {
    /// @todo(me) 実装
  }

  private void RecentProfile3_Click(object sender, RoutedEventArgs e) {
    /// @todo(me) 実装
  }

  private void RecentProfile4_Click(object sender, RoutedEventArgs e) {
    /// @todo(me) 実装 
  }

  private void RecentProfile5_Click(object sender, RoutedEventArgs e) {
    /// @todo(me) 実装 
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------
}
}   // namespace SCFF.GUI.Controls
