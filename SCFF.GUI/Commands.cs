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

  /// CurrentViewを可視化(LayoutEdit)したときの内容が変更された
  public static readonly RoutedCommand CurrentLayoutElementVisualChanged = new RoutedCommand();

  /// Profileを可視化(LayoutEdit)したときの内容が変更された
  public static readonly RoutedCommand ProfileVisualChanged = new RoutedCommand();

  /// Profileの構造(Profile.CurrentIndex/LayoutElementCount)が変更された
  public static readonly RoutedCommand ProfileStructureChanged = new RoutedCommand();

  /// LayoutParametersのみの更新が必要
  public static readonly RoutedCommand LayoutParameterChanged = new RoutedCommand();

  /// TargetWindowが変更された(→CurrentLayoutElementVisualChanged)
  public static readonly RoutedCommand TargetWindowChanged = new RoutedCommand();

  /// Areaの情報が変更された(→CurrentLayoutElementVisualChanged)
  public static readonly RoutedCommand AreaChanged = new RoutedCommand();

  /// サンプルサイズが変更された
  public static readonly RoutedCommand SampleSizeChanged = new RoutedCommand();

  //===================================================================
  // 機能
  //===================================================================
  
  /// レイアウト要素の追加
  public static readonly RoutedCommand AddLayoutElement = new RoutedCommand();
  /// 現在編集中のレイアウト要素の削除
  public static readonly RoutedCommand RemoveCurrentLayoutElement = new RoutedCommand();

  /// 現在編集中のレイアウト要素を一つ前面に
  public static readonly RoutedCommand BringCurrentLayoutElementForward = new RoutedCommand();
  /// 現在編集中のレイアウト要素を一つ背面に
  public static readonly RoutedCommand SendCurrentLayoutElementBackward = new RoutedCommand();

  /// 現在編集中のレイアウト要素の境界を取り込み内容に合わせて調整する
  public static readonly RoutedCommand FitCurrentBoundRect = new RoutedCommand();
  
  /// プロファイルを共有メモリに書き込み
  public static readonly RoutedCommand SendProfile = new RoutedCommand();
  /// NullLayoutプロファイルを共有メモリに書き込み
  public static readonly RoutedCommand SendNullProfile = new RoutedCommand();

  /// AeroのON/OFF
  public static readonly RoutedCommand SetAero = new RoutedCommand();
  /// コンパクト表示の切り替え
  public static readonly RoutedCommand SetCompactView = new RoutedCommand();
}
}   // namespace SCFF.GUI
