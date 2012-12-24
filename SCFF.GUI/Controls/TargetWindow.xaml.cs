// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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

/// @file SCFF.GUI/Controls/TargetWindow.cs
/// ウィンドウ取り込み対象の設定用コントロール

namespace SCFF.GUI.Controls {

using System.Windows.Controls;

/// ウィンドウ取り込み対象の設定用コントロール
public partial class TargetWindow : UserControl, IProfileToControl {

  /// コンストラクタ
  public TargetWindow() {
    InitializeComponent();
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  public void UpdateByProfile() {
    // *Changedイベントハンドラがないのでそのまま代入するだけ
    this.WindowCaption.Text = App.Profile.CurrentLayoutElement.WindowCaption;
  }

  public void AttachChangedEventHandlers() {
    // nop
  }

  public void DetachChangedEventHandlers() {
    // nop
  }
}
}
