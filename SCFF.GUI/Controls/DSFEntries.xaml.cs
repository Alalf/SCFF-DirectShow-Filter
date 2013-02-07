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

/// @file SCFF.GUI/Controls/DSFEntries.xaml.cs
/// @copydoc SCFF::GUI::Controls::DSFEntries

namespace SCFF.GUI.Controls {

using System.Diagnostics;
using System.Windows.Controls;
using SCFF.Common;
using SCFF.Common.GUI;

/// SCFF DirectShow Filterエントリを仮想メモリから読み込み・表示・選択するためのUserControl
public partial class DSFEntries : UserControl, IBindingRuntimeOptions {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================
  
  /// コンストラクタ
  public DSFEntries() {
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
    App.Impl.RefreshDirectory();

    //-----------------------------------------------------------------
    // Notify self
    this.OnRuntimeOptionsChanged();
    // Notify other controls
    Commands.SampleSizeChanged.Execute(null, this);
    //-----------------------------------------------------------------
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// Processes: SelectionChanged
  private void Processes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    if (!this.CanChangeRuntimeOptions) return;
    App.RuntimeOptions.SelectedEntryIndex = this.Processes.SelectedIndex;

    //-----------------------------------------------------------------
    // Notify self
    // Notify other controls
    Commands.SampleSizeChanged.Execute(null, this);
    //-----------------------------------------------------------------
  }

  //===================================================================
  // IBindingRuntimeOptionsの実装
  //===================================================================

  /// @copydoc Common::GUI::IBindingRuntimeOptions::CanChangeRuntimeOptions
  public bool CanChangeRuntimeOptions { get; private set; }
  /// @copydoc Common::GUI::IBindingRuntimeOptions::OnRuntimeOptionsChanged
  public void OnRuntimeOptionsChanged() {
    this.CanChangeRuntimeOptions = false;

    // コンボボックスの更新
    this.Processes.Items.Clear();
    foreach (var entryString in App.RuntimeOptions.EntryStringList) {
      var item = new ComboBoxItem();
      item.Content = entryString;
      this.Processes.Items.Add(item.Content);
    }

    // SelectedIndexの調整
    if (this.Processes.Items.Count == 0) {
      this.Processes.IsEnabled = false;
      this.Processes.SelectedIndex = -1;
    } else {
      this.Processes.IsEnabled = true;
      Debug.Assert(0 <= App.RuntimeOptions.SelectedEntryIndex &&
                   App.RuntimeOptions.SelectedEntryIndex < this.Processes.Items.Count);
      this.Processes.SelectedIndex = App.RuntimeOptions.SelectedEntryIndex;
    }

    this.CanChangeRuntimeOptions = true;
  }
}
}   // namespace SCFF.GUI.Controls
