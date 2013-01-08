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

/// @file SCFF.GUI/Controls/SCFFDSFEntries.xaml.cs
/// @copydoc SCFF::GUI::Controls::SCFFDSFEntries

namespace SCFF.GUI.Controls {

  using System.Windows;
  using System.Windows.Controls;

/// SCFFDSFエントリを仮想メモリから読み込み・表示・選択するためのUserControl
public partial class SCFFDSFEntries
    : UserControl, IUpdateByRuntimeOptions {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================
  
  /// コンストラクタ
  public SCFFDSFEntries() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// Refresh: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Refresh_Click(object sender, System.Windows.RoutedEventArgs e) {
    /// @todo(me) プロセスリストを読み込み
    App.RuntimeOptions.Refresh();

    /// メインウィンドウに必要な更新を伝える
    UpdateCommands.UpdateMainWindowByRuntimeOptions.Execute(null, null);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IUpdateByRuntimeOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByRuntimeOptions::UpdateByRuntimeOptions
  public void UpdateByRuntimeOptions() {
    //// @todo(me) 仮想メモリからの読み込み
  }

  /// @copydoc IUpdateByRuntimeOptions::DetachRuntimeOptionsChangedEventHandlers
  public void DetachRuntimeOptionsChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByRuntimeOptions::AttachRuntimeOptionsChangedEventHandlers
  public void AttachRuntimeOptionsChangedEventHandlers() {
    // nop
  }
}
}   // namespace SCFF.GUI.Controls
