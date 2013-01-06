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

/// @file SCFF.GUI/Controls/LayoutToolbar.xaml.cs
/// @copydoc SCFF::GUI::Controls::LayoutToolbar

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// レイアウト編集用ツールバーを管理するUserControl
public partial class LayoutToolbar : UserControl, IUpdateByOptions {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public LayoutToolbar() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// LayoutPreview: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void LayoutPreview_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutPreview.IsChecked.HasValue) return;
    App.Options.LayoutPreview = (bool)this.LayoutPreview.IsChecked;

    UpdateCommands.UpdateLayoutEditByEntireProfile.Execute(null, null);
    UpdateCommands.UpdateLayoutEditByOptions.Execute(null, null);
  }

  /// LayoutSnap: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void LayoutSnap_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutSnap.IsChecked.HasValue) return;
    App.Options.LayoutSnap = (bool)this.LayoutSnap.IsChecked;

    // LayoutEditの能動的な更新は必要ない
  }

  /// LayoutBorder: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void LayoutBorder_Click(object sender, RoutedEventArgs e) {
    if (!this.LayoutBorder.IsChecked.HasValue) return;
    App.Options.LayoutBorder = (bool)this.LayoutBorder.IsChecked;

    UpdateCommands.UpdateLayoutEditByEntireProfile.Execute(null, null);
  }

  /// レイアウト要素追加時に重なっていると醜いので少しずらす用のオフセット
  private const double BoundOffset = 0.05;

  /// Add: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Add_Click(object sender, RoutedEventArgs e) {
    App.Profile.AddLayoutElement();
    App.Profile.CurrentOutputLayoutElement.BoundRelativeLeft =
      LayoutToolbar.BoundOffset * App.Profile.CurrentInputLayoutElement.Index;
    App.Profile.CurrentOutputLayoutElement.BoundRelativeTop =
      LayoutToolbar.BoundOffset * App.Profile.CurrentInputLayoutElement.Index;

    this.Add.IsEnabled = App.Profile.CanAddLayoutElement();
    this.Remove.IsEnabled = App.Profile.CanRemoveLayoutElement();

    UpdateCommands.UpdateMainWindowByEntireProfile.Execute(null, null);
  }

  /// Remove: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Remove_Click(object sender, RoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();

    this.Add.IsEnabled = App.Profile.CanAddLayoutElement();
    this.Remove.IsEnabled = App.Profile.CanRemoveLayoutElement();

    UpdateCommands.UpdateMainWindowByEntireProfile.Execute(null, null);
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
    this.LayoutPreview.IsChecked = App.Options.LayoutPreview;
    this.LayoutBorder.IsChecked = App.Options.LayoutBorder;
    this.LayoutSnap.IsChecked = App.Options.LayoutSnap;
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
