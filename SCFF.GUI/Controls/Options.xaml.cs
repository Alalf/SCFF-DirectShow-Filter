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

/// @file SCFF.GUI/Controls/Options.xaml.cs
/// @copydoc SCFF::GUI::Controls::Options

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;
using SCFF.Common.GUI;

/// SWScale以外の設定用UserControl
public partial class Options : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public Options() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// ShowCursor: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void ShowCursor_Click(object sender, RoutedEventArgs e) {
    if (!this.ShowCursor.IsChecked.HasValue) return;

    App.Profile.Current.Open();
    App.Profile.Current.SetShowCursor = (bool)this.ShowCursor.IsChecked;
    App.Profile.Current.Close();
    UpdateCommands.UpdateLayoutEditByCurrentProfile.Execute(null, null);
  }

  /// ShowLayeredWindow: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void ShowLayeredWindow_Click(object sender, RoutedEventArgs e) {
    if (!this.ShowLayeredWindow.IsChecked.HasValue) return;

    App.Profile.Current.Open();
    App.Profile.Current.SetShowLayeredWindow = (bool)this.ShowLayeredWindow.IsChecked;
    App.Profile.Current.Close();
    UpdateCommands.UpdateLayoutEditByCurrentProfile.Execute(null, null);
  }

  /// KeepAspectRatio: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void KeepAspectRatio_Click(object sender, RoutedEventArgs e) {
    if (!this.KeepAspectRatio.IsChecked.HasValue) return;

    App.Profile.Current.Open();
    App.Profile.Current.SetKeepAspectRatio = (bool)this.KeepAspectRatio.IsChecked;
    App.Profile.Current.Close();
    UpdateCommands.UpdateLayoutEditByCurrentProfile.Execute(null, null);
  }

  /// Stretch: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Stretch_Click(object sender, RoutedEventArgs e) {
    if (!this.Stretch.IsChecked.HasValue) return;
 
    App.Profile.Current.Open();
    App.Profile.Current.SetStretch = (bool)this.Stretch.IsChecked;
    App.Profile.Current.Close();
    UpdateCommands.UpdateLayoutEditByCurrentProfile.Execute(null, null);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentProfileChanged
  public void OnCurrentProfileChanged() {
    this.CanChangeProfile = false;
    // checkboxはclickがあるのでeventハンドラをattach/detachする必要はない
    this.ShowCursor.IsChecked = App.Profile.CurrentView.ShowCursor;
    this.ShowLayeredWindow.IsChecked = App.Profile.CurrentView.ShowLayeredWindow;
    this.KeepAspectRatio.IsChecked = App.Profile.CurrentView.KeepAspectRatio;
    this.Stretch.IsChecked = App.Profile.CurrentView.Stretch;
    // @todo(me) overSampingとthreadCountはまだDSFでも実装されていない
    this.CanChangeProfile = true;
  }
  /// @copydoc Common::GUI::IBindingProfile::OnEntireProfileChanged
  public void OnEntireProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentProfileChanged();
  }
}
}   // namespace SCFF.GUI.Controls
