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

/// @file SCFF.GUI/AreaSelectWindow.xaml.cs
/// @copydoc SCFF::GUI::AreaSelectWindow

namespace SCFF.GUI {

using System.Windows;
using System.Windows.Input;

/// AreaSelectWindowのコードビハインド
public partial class AreaSelectWindow : Window {
  //===================================================================
  // コンストラクタ/Loaded/Closing/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public AreaSelectWindow() {
    InitializeComponent();

    this.MouseLeftButtonDown += (sender, e) => this.DragMove();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// MouseDown
  private void OnMouseDown(object sender, MouseButtonEventArgs e) {
    if (e.ClickCount != 2) return;

    // Double Click
    this.DialogResult = true;
    this.Close();
  }

  //===================================================================
  // コマンドイベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // ApplicationCommands
  //-------------------------------------------------------------------

  /// Save
  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.DialogResult = true;
    this.Close();
  } 

  //-------------------------------------------------------------------
  // Windows.Shell.SystemCommands
  //-------------------------------------------------------------------

  /// CloseWindow
  private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.DialogResult = false;
    this.Close();
  }
}
}   // namespace SCFF.GUI
