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

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using SCFF.Common.GUI;

/// SCFF DirectShow Filterエントリを仮想メモリから読み込み・表示・選択するためのUserControl
public partial class DSFEntries : UserControl, IBindingRuntimeOptions {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================
  
  /// コンストラクタ
  public DSFEntries() {
    InitializeComponent();

    this.Processes.ItemsSource = this.processesSource;
    this.Processes.DisplayMemberPath = "Item2";
    this.Processes.SelectedValuePath = "Item1";
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
    App.RuntimeOptions.CurrentProcessID = (UInt32)this.Processes.SelectedValue;

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
    this.processesSource.Clear();
    foreach (var tuple in App.RuntimeOptions.EntryLabels) {
      this.processesSource.Add(tuple);
    }

    // SelectedIndexの調整
    if (this.processesSource.Count == 0) {
      this.Processes.IsEnabled = false;
    } else {
      this.Processes.IsEnabled = true;
      this.Processes.SelectedValue = App.RuntimeOptions.CurrentProcessID;
    }

    this.CanChangeRuntimeOptions = true;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// Processes.ItemsSource用のObservableCollection
  private readonly ObservableCollection<Tuple<UInt32,string>> processesSource =
      new ObservableCollection<Tuple<uint,string>>();
}
}   // namespace SCFF.GUI.Controls
