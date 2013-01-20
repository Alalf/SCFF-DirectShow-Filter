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

/// @file SCFF.GUI/Controls/TextBoxError.cs
/// @copydoc SCFF::GUI::Controls::TextBoxError

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// エラー表示用メソッドをまとめたstaticクラス
public static class TextBoxError {
  /// TextBoxがエラー状態か
  public static bool HasError(TextBox textBox) {
    return textBox.Tag != null;
  }
  /// TextBoxのエラー状態解除
  public static void ResetError(TextBox textBox, ToolTip toolTip = null) {
    textBox.Tag = null;
    if (toolTip == null) return;
    toolTip.Visibility = Visibility.Hidden;
    toolTip.Content = null;
    toolTip.IsOpen = false;
  }
  /// TextBoxのエラー状態設定
  public static void SetError(TextBox textBox, ToolTip toolTip = null, string message = null) {
    textBox.Tag = "HasError";
    if (toolTip == null) return;
    if (message != null) {
      toolTip.Visibility = Visibility.Visible;
      toolTip.Content = message;
      toolTip.IsOpen = true;
    } else {
      toolTip.Visibility = Visibility.Hidden;
      toolTip.Content = null;
      toolTip.IsOpen = false;
    }
  }
  /// TextBoxの警告状態設定
  public static void SetWarning(TextBox textBox, ToolTip toolTip = null, string message = null) {
    textBox.Tag = "HasWarning";
    if (toolTip == null) return;
    if (message != null) {
      toolTip.Visibility = Visibility.Visible;
      toolTip.Content = message;
      toolTip.IsOpen = true;
    } else {
      toolTip.Visibility = Visibility.Hidden;
      toolTip.Content = null;
      toolTip.IsOpen = false;
    }
  }
}
}   // namespace SCFF.GUI.Controls
