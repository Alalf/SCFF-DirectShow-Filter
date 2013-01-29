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
/// @copydoc SCFF::GUI::Controls::MainMenu

namespace SCFF.GUI.Controls {

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SCFF.Common;
using SCFF.Common.GUI;

/// メインメニュー管理用UserControl
public partial class MainMenu : UserControl, IBindingOptions {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public MainMenu() {
    InitializeComponent();
  }

  //-------------------------------------------------------------------

  /// index->MenuItem変換
  private MenuItem GetMenuItem(int index) {
    switch (index) {
      case 0: return this.RecentProfile1;
      case 1: return this.RecentProfile2;
      case 2: return this.RecentProfile3;
      case 3: return this.RecentProfile4;
      case 4: return this.RecentProfile5;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// ForceAeroOn: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void ForceAeroOn_Click(object sender, RoutedEventArgs e) {
    //-----------------------------------------------------------------
    // Run
    Commands.SetAero.Execute(this.ForceAeroOn.IsChecked, this);
    //-----------------------------------------------------------------
  }

  /// CompactView: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void CompactView_Click(object sender, RoutedEventArgs e) {
    //-----------------------------------------------------------------
    // Run
    Commands.SetCompactView.Execute(this.CompactView.IsChecked, this);
    //-----------------------------------------------------------------
  }

  /// RestoreLastProfile: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RestoreLastProfile_Click(object sender, RoutedEventArgs e) {
    App.Options.RestoreLastProfile = this.RestoreLastProfile.IsChecked;
  }

  /// RecentProfile1: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RecentProfile1_Click(object sender, RoutedEventArgs e) {
    ApplicationCommands.Open.Execute(App.Options.GetRecentProfile(0), this);
  }

  /// RecentProfile2: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RecentProfile2_Click(object sender, RoutedEventArgs e) {
    ApplicationCommands.Open.Execute(App.Options.GetRecentProfile(1), this);
  }

  /// RecentProfile3: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RecentProfile3_Click(object sender, RoutedEventArgs e) {
    ApplicationCommands.Open.Execute(App.Options.GetRecentProfile(2), this);
  }

  /// RecentProfile4: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RecentProfile4_Click(object sender, RoutedEventArgs e) {
    ApplicationCommands.Open.Execute(App.Options.GetRecentProfile(3), this);
  }

  /// RecentProfile5: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void RecentProfile5_Click(object sender, RoutedEventArgs e) {
    ApplicationCommands.Open.Execute(App.Options.GetRecentProfile(4), this);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IBindingOptionsの実装
  //===================================================================

  /// 最近使用したプロファイルメニューの更新
  private void UpdateRecentProfiles() {
    for (int i = 0; i < Constants.RecentProfilesLength; ++i ) {
      var isEmpty = (App.Options.GetRecentProfile(i) == string.Empty);
      var shortPath = isEmpty
          ? ""
          : Utilities.GetShortPath(App.Options.GetRecentProfile(i), 60);        
      var header = string.Format("{0} {1}(_{0})", i+1, shortPath);

      this.GetMenuItem(i).IsEnabled = !isEmpty;
      this.GetMenuItem(i).Header = header;
    }
  }
  /// @copydoc Common::GUI::IBindingOptions::CanChangeOptions
  public bool CanChangeOptions { get; private set; }
  /// @copydoc Common::GUI::IBindingOptions::OnOptionsChanged
  public void OnOptionsChanged() {
    this.CanChangeOptions = false;
    this.UpdateRecentProfiles();
    this.CompactView.IsChecked = App.Options.CompactView;
    this.ForceAeroOn.IsChecked = App.Options.ForceAeroOn;
    this.RestoreLastProfile.IsChecked = App.Options.RestoreLastProfile;
    this.CanChangeOptions = true;
  }
}
}   // namespace SCFF.GUI.Controls
