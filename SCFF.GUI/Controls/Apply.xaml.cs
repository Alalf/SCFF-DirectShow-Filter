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

/// @file SCFF.GUI/Controls/Apply.xaml.cs
/// @copydoc SCFF::GUI::Controls::Apply

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// Splash/ApplyボタンとAutoApplyチェックボックスを管理するためのUserControl
public partial class Apply
    : UserControl, IUpdateByOptions {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================
  
  /// コンストラクタ
  public Apply() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// AutoApply: Click
  private void AutoApply_Click(object sender, RoutedEventArgs e) {
    if (!this.AutoApply.IsChecked.HasValue) return;
    App.Options.AutoApply = (bool)this.AutoApply.IsChecked;
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByOptions::UpdateByOptions
  public void UpdateByOptions() {
    this.AutoApply.IsChecked = App.Options.AutoApply;
  }

  /// @copydoc IUpdateByOptions::DetachOptionsChangedEventHandlers
  public void DetachOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByOptions::AttachOptionsChangedEventHandlers
  public void AttachOptionsChangedEventHandlers() {
    // nop
  }
}
}   // namespace SCFF.GUI.Controls
