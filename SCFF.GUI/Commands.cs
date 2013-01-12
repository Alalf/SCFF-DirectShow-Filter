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
/// @copydoc SCFF::GUI::Commands

namespace SCFF.GUI {

using System.Windows.Input;

/// 機能の実行を依頼するためのRoutedCommands
public static class Commands {
  //===================================================================
  // イベント
  //===================================================================

  // プレビューの更新が必要
  public static readonly RoutedCommand NeedUpdateCurrentPreview = new RoutedCommand();

  // レイアウト要素すべての再描画が必要
  public static readonly RoutedCommand NeedRedrawAll = new RoutedCommand();

  // 現在のレイアウト要素の再描画が必要
  public static readonly RoutedCommand NeedRedrawCurrent = new RoutedCommand();

  // LayoutParametersのみの更新が必要
  public static readonly RoutedCommand LayoutParameterChanged = new RoutedCommand();

  // LayoutEditそのもの表示・非表示が切り替わった
  public static readonly RoutedCommand LayoutEditVisibilityChanged = new RoutedCommand();

  // TargetWindowが変更された
  public static readonly RoutedCommand TargetWindowChanged = new RoutedCommand();

  // Areaの情報が変更された
  public static readonly RoutedCommand AreaChanged = new RoutedCommand();

  // CurrentIndexが変更された
  public static readonly RoutedCommand CurrentIndexChanged = new RoutedCommand();

  // LayoutElementが追加された
  public static readonly RoutedCommand LayoutElementAdded = new RoutedCommand();

  // 現在のLayoutElementが削除された
  public static readonly RoutedCommand CurrentLayoutElementRemoved = new RoutedCommand();

  // サンプルサイズが変更された
  public static readonly RoutedCommand SampleSizeChanged = new RoutedCommand();

  //===================================================================
  // 機能
  //===================================================================
  
  /// AeroのON/OFF
  public static readonly RoutedCommand SetAero = new RoutedCommand();
  /// コンパクト表示の切り替え
  public static readonly RoutedCommand SetCompactView = new RoutedCommand();
}
}   // namespace SCFF.GUI
