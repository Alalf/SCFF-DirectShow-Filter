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

/// @file SCFF.GUI/Controls/LayoutToolbar.cs
/// レイアウト追加・削除・プレビューオプション設定用ツールバー

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// レイアウト追加・削除・プレビューオプション設定用ツールバー
public partial class LayoutToolbar : UserControl, IUpdateByOptions {

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutToolbar() {
    InitializeComponent();
  }

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  public void UpdateByOptions() {
    this.LayoutPreview.IsChecked = App.Options.LayoutPreview;
    this.LayoutBorder.IsChecked = App.Options.LayoutBorder;
    this.LayoutSnap.IsChecked = App.Options.LayoutSnap;
  }

  public void DetachOptionsChangedEventHandlers() {
    // nop
  }

  public void AttachOptionsChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void layoutPreview_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutPreview.IsChecked.HasValue) return;
    App.Options.LayoutPreview = (bool)this.LayoutPreview.IsChecked;

    UpdateCommands.UpdateLayoutEditCommand.Execute(null, null);
  }

  private void layoutSnap_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutSnap.IsChecked.HasValue) return;
    App.Options.LayoutSnap = (bool)this.LayoutSnap.IsChecked;

    // LayoutEditの能動的な更新は必要ない
  }

  private void layoutBorder_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutBorder.IsChecked.HasValue) return;
    App.Options.LayoutBorder = (bool)this.LayoutBorder.IsChecked;

    UpdateCommands.UpdateLayoutEditCommand.Execute(null, null);
  }

  private const double boundOffset = 0.05;

  private void Add_Click(object sender, RoutedEventArgs e) {
    App.Profile.AddLayoutElement();
    App.Profile.CurrentOutputLayoutElement.BoundRelativeLeft =
      boundOffset * App.Profile.CurrentInputLayoutElement.Index;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeTop =
      boundOffset * App.Profile.CurrentInputLayoutElement.Index;

    this.Add.IsEnabled = App.Profile.CanAddLayoutElement();
    this.Remove.IsEnabled = App.Profile.CanRemoveLayoutElement();

    UpdateCommands.UpdateMainWindowCommand.Execute(null, null);
  }

  private void Remove_Click(object sender, RoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();

    this.Add.IsEnabled = App.Profile.CanAddLayoutElement();
    this.Remove.IsEnabled = App.Profile.CanRemoveLayoutElement();

    UpdateCommands.UpdateMainWindowCommand.Execute(null, null);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------
}
}   // namespace SCFF.GUI.Controls
