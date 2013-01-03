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
/// 拡大縮小時のオプション

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// 拡大縮小時のオプション
public partial class Options : UserControl, IUpdateByProfile {

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public Options() {
    InitializeComponent();
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile.UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    // checkboxはclickがあるのでeventハンドラをattach/detachする必要はない
    this.ShowCursor.IsChecked = App.Profile.CurrentInputLayoutElement.ShowCursor;
    this.ShowLayeredWindow.IsChecked = App.Profile.CurrentInputLayoutElement.ShowLayeredWindow;
    this.KeepAspectRatio.IsChecked = App.Profile.CurrentInputLayoutElement.KeepAspectRatio;
    this.Stretch.IsChecked = App.Profile.CurrentInputLayoutElement.Stretch;
    // @todo(me) overSampingとthreadCountはまだDSFでも実装されていない
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    // 編集するのはCurrentのみ
    this.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile.AttachProfileChangedEventHandlers
  public void AttachProfileChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByProfile.DetachProfileChangedEventHandlers
  public void DetachProfileChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void showCursor_Click(object sender, RoutedEventArgs e) {
    if (!this.ShowCursor.IsChecked.HasValue) return;

    App.Profile.CurrentOutputLayoutElement.ShowCursor = (bool)this.ShowCursor.IsChecked;
  }

  private void showLayeredWindow_Click(object sender, RoutedEventArgs e) {
    if (!this.ShowLayeredWindow.IsChecked.HasValue) return;

    App.Profile.CurrentOutputLayoutElement.ShowLayeredWindow = (bool)this.ShowLayeredWindow.IsChecked;
  }

  private void keepAspectRatio_Click(object sender, RoutedEventArgs e) {
    if (!this.KeepAspectRatio.IsChecked.HasValue) return;

    App.Profile.CurrentOutputLayoutElement.KeepAspectRatio = (bool)this.KeepAspectRatio.IsChecked;
  }

  private void stretch_Click(object sender, RoutedEventArgs e) {
    if (!this.Stretch.IsChecked.HasValue) return;
 
    App.Profile.CurrentOutputLayoutElement.Stretch = (bool)this.Stretch.IsChecked;
  }
}
}
