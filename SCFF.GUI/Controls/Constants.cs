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

/// @file SCFF.GUI/Controls/Constants.cs
/// ユーザコントロール共通の定数

namespace SCFF.GUI.Controls {

using System.Windows.Media;

/// ブラシとペン
public static class BrushesAndPens {
  /// WindowTypes.NormalでCurrent時のブラシ
  public static readonly Brush CurrentNormalBrush;
  /// WindowTypes.Normalのブラシ
  public static readonly Brush NormalBrush;
  /// WindowTypes.DesktopListViewでCurrent時のブラシ
  public static readonly Brush CurrentDesktopListViewBrush;
  /// WindowTypes.DesktopListViewのブラシ
  public static readonly Brush DesktopListViewBrush;
  /// WindowTypes.DesktopでCurrent時のブラシ
  public static readonly Brush CurrentDesktopBrush;
  /// WindowTypes.Desktopのブラシ
  public static readonly Brush DesktopBrush;

  /// ペンの太さ
  /// @todo(me) 本当はここにあってはいけないのだが・・・
  private const double penThickness = 0.005 * 100.0;

  /// WindowTypes.NormalでCurrent時のペン
  public static readonly Pen CurrentNormalPen;
  /// WindowTypes.Normalのペン
  public static readonly Pen NormalPen;
  /// WindowTypes.DesktopListViewでCurrent時のペン
  public static readonly Pen CurrentDesktopListViewPen;
  /// WindowTypes.DesktopListViewのペン
  public static readonly Pen DesktopListViewPen;
  /// WindowTypes.DesktopでCurrent時のペン
  public static readonly Pen CurrentDesktopPen;
  /// WindowTypes.Desktopのペン
  public static readonly Pen DesktopPen;

  /// ドロップシャドウ描画用ペン（正確には黒色の縁取りだが・・・）
  public static readonly Pen DropShadowPen;

  /// staticコンストラクタ
  static BrushesAndPens() {
    // Brushes
    CurrentNormalBrush = Brushes.DarkOrange;
    CurrentNormalBrush.Freeze();
    NormalBrush = new SolidColorBrush(Color.FromArgb(0x99, 0xFF, 0x8C, 0x00));
    NormalBrush.Freeze();
    CurrentDesktopListViewBrush = Brushes.DarkCyan;
    CurrentDesktopListViewBrush.Freeze();
    DesktopListViewBrush = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x8B, 0x8B));
    DesktopListViewBrush.Freeze();
    CurrentDesktopBrush = Brushes.DarkGreen;
    CurrentDesktopBrush.Freeze();
    DesktopBrush = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x64, 0x00));
    DesktopBrush.Freeze();

    // Pens
    CurrentNormalPen = new Pen(CurrentNormalBrush, penThickness);
    CurrentNormalPen.Freeze();
    NormalPen = new Pen(NormalBrush, penThickness);
    NormalPen.Freeze();
    CurrentDesktopListViewPen = new Pen(CurrentDesktopListViewBrush, penThickness);
    CurrentDesktopListViewPen.Freeze();
    DesktopListViewPen = new Pen(DesktopListViewBrush, penThickness);
    DesktopListViewPen.Freeze();
    CurrentDesktopPen = new Pen(CurrentDesktopBrush, penThickness);
    CurrentDesktopPen.Freeze();
    DesktopPen = new Pen(DesktopBrush, penThickness);
    DesktopPen.Freeze();

    DropShadowPen = new Pen(Brushes.Black, penThickness);
    DropShadowPen.Freeze();
  }
}
}
