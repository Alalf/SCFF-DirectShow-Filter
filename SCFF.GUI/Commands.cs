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

/// @file SCFF.GUI/Commands.cs
/// 機能の実行を依頼するためのRoutedCommands

namespace SCFF.GUI {

using System.Windows.Input;

/// 機能の実行を依頼するためのRoutedCommands
public static class Commands {
  /// AeroのON/OFF
  public static readonly RoutedCommand SetAero = new RoutedCommand();
  /// コンパクト表示の切り替え
  public static readonly RoutedCommand SetCompactView = new RoutedCommand();
}
}   // namespace SCFF.GUI
