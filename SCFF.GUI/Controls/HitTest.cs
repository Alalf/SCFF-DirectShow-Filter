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

/// @file SCFF.GUI/Controls/HitTest.cs
/// 与えられたマウスポイント(0-1,0-1)からレイアウトIndexとModeを返す

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

/// ヒットテストの結果をまとめた列挙型
///
/// N,W,S,Eは場合によっては消える場合もある
public enum HitModes {
  Neutral,
  Move,
  SizeNW,
  SizeNE,
  SizeSW,
  SizeSE,
  SizeN,
  SizeW,
  SizeS,
  SizeE
}

/// マウスポイントが選択したレイアウト要素のどの位置にあるかを示すオフセット
public class Offset {
  public Offset(Profile.InputLayoutElement layoutElement, Point relativeMousePoint) {
    this.Left = relativeMousePoint.X - layoutElement.BoundRelativeLeft;
    this.Top = relativeMousePoint.Y - layoutElement.BoundRelativeTop;
    this.Right = relativeMousePoint.X - layoutElement.BoundRelativeRight;
    this.Bottom = relativeMousePoint.Y - layoutElement.BoundRelativeBottom;
  }

  public double Left { get; private set; }
  public double Top { get; private set; }
  public double Right { get; private set; }
  public double Bottom { get; private set; }
}

/// 与えられたマウスポイント(0-1,0-1)からレイアウトIndexとModeを返す
///
/// このクラスの中では座標系はすべて正規化された相対値(0.0-1.0)なので
/// わざわざ変数名などにRelativeをつける必要はない
///
/// Borderは内側だけでなく外側にもあることに注意
///
/// ややこしいことになるのでこのクラスの中からApp.Profileの変更は禁止
public static class HitTest {
  //-------------------------------------------------------------------
  // 定数
  //-------------------------------------------------------------------

  private const double WEBorderThickness = 0.02;
  private const double NSBorderThickness = 0.02;
  private const double MinimumWidth = WEBorderThickness * 2;
  private const double MinimumHeight = NSBorderThickness * 2;

  /// カーソルをまとめたディクショナリ
  public static readonly Dictionary<HitModes, Cursor> HitModesToCursors =
      new Dictionary<HitModes,Cursor> {
    {HitModes.Neutral, null},
    {HitModes.Move, Cursors.SizeAll},
    {HitModes.SizeNW, Cursors.SizeNWSE},
    {HitModes.SizeNE, Cursors.SizeNESW},
    {HitModes.SizeSW, Cursors.SizeNESW},
    {HitModes.SizeSE, Cursors.SizeNWSE},
    {HitModes.SizeN, Cursors.SizeNS},
    {HitModes.SizeW, Cursors.SizeWE},
    {HitModes.SizeS, Cursors.SizeNS},
    {HitModes.SizeE, Cursors.SizeWE}
  };

  //-------------------------------------------------------------------
  // ヒットテスト
  //-------------------------------------------------------------------

  /// 最大外接矩形を返す
  private static Rect GetMaximumBoundRect(Profile.InputLayoutElement layoutElement) {
    return new Rect {
      X = layoutElement.BoundRelativeLeft - WEBorderThickness,
      Y = layoutElement.BoundRelativeTop - NSBorderThickness,
      Width = layoutElement.BoundRelativeWidth + WEBorderThickness * 4,
      Height = layoutElement.BoundRelativeHeight + NSBorderThickness * 4
    };
  }

  /// 移動用領域を返す
  private static Rect GetMoveRect(Profile.InputLayoutElement layoutElement) {
    return new Rect {
      X = layoutElement.BoundRelativeLeft + WEBorderThickness,
      Y = layoutElement.BoundRelativeTop + NSBorderThickness,
      Width = layoutElement.BoundRelativeWidth - WEBorderThickness * 2,
      Height = layoutElement.BoundRelativeHeight - NSBorderThickness * 2
    };
  }

  /// 指定した座標からHitModesを返す
  /// @pre 必ずHitModes.Resize*を返すことが可能なlayoutElement
  private static HitModes GetHitMode(Profile.InputLayoutElement layoutElement, Point point) {
    // ---------------
    // |  |1     |2  |
    // ---------------

    // H1
    var borderWRight = layoutElement.BoundRelativeLeft + WEBorderThickness;
    // H2
    var borderELeft = layoutElement.BoundRelativeRight - WEBorderThickness;

    // V1
    var borderNBottom = layoutElement.BoundRelativeTop + NSBorderThickness;
    // v2
    var borderSTop = layoutElement.BoundRelativeBottom - NSBorderThickness;
    
    // x座標→Y座標
    if (point.X <= borderWRight) {
      // W
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeNW;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        return HitModes.SizeW;
      } else {
        // S
        return HitModes.SizeSW;
      }
    } else if (point.X <= borderELeft) {
      // (W)-(E)
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeN;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        Debug.Fail("GetHitMode: Move??");
        return HitModes.Move;
      } else {
        // S
        return HitModes.SizeS;
      }
    } else {
      // E
      if (point.Y <= borderNBottom) {
        // N
        return HitModes.SizeNE;
      } else if (point.Y <= borderSTop) {
        // (N)-(S)
        return HitModes.SizeE;
      } else {
        // S
        return HitModes.SizeSE;
      }
    }
  }

  /// ヒットテスト
  public static bool TryHitTest(Point point, out int hitIndex, out HitModes hitMode) {
    // 計算途中の結果をまとめるスタック
    var moveStack = new Stack<Profile.InputLayoutElement>();
    var resizeStack = new Stack<Profile.InputLayoutElement>();

    // レイアウト要素を線形探索
    foreach (var layoutElement in App.Profile) {
      // 最大外接矩形に入っていなければヒットテスト対象外
      var maximumBoundRect = HitTest.GetMaximumBoundRect(layoutElement);
      if (!maximumBoundRect.Contains(point)) continue;

      var moveRect = HitTest.GetMoveRect(layoutElement);
      if (moveRect.Contains(point)) {
        // 移動用領域に入ってればStackに積む
        moveStack.Push(layoutElement);
      } else {
        // 最大外接矩形に入っていて、移動用領域でないということはサイズ変更用領域に入ってる
        resizeStack.Push(layoutElement);
      }
    }
   
    // resizeStack優先
    foreach (var layoutElement in resizeStack) {
      // 見つかり次第終了
      hitMode = HitTest.GetHitMode(layoutElement, point);
      hitIndex = layoutElement.Index;
      return true;
    }

    // moveStack
    foreach (var layoutElement in moveStack) {
      // 見つかり次第終了
      hitMode = HitModes.Move;
      hitIndex = layoutElement.Index;
      return true;
    }

    hitIndex = -1;
    hitMode = HitModes.Neutral;
    return false;
  }

  /// Move
  public static void Move(Profile.InputLayoutElement layoutElement,
      Point point, Offset offset,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // まず領域外に行かないようにする
    var tryNextLeft = point.X - offset.Left;
    if (tryNextLeft < 0.0) tryNextLeft = 0.0;
    var tryNextTop = point.Y - offset.Top;
    if (tryNextTop < 0.0) tryNextTop = 0.0;
    var tryNextRight = tryNextLeft + layoutElement.BoundRelativeWidth;
    if (tryNextRight > 1.0) {
      tryNextRight = 1.0;
      tryNextLeft = tryNextRight - layoutElement.BoundRelativeWidth;
    } 
    var tryNextBottom = tryNextTop + layoutElement.BoundRelativeHeight;
    if (tryNextBottom > 1.0) {
      tryNextBottom = 1.0;
      tryNextTop = tryNextBottom - layoutElement.BoundRelativeHeight;
    }

    nextLeft = tryNextLeft;
    nextTop = tryNextTop;
    nextRight = tryNextRight;
    nextBottom = tryNextBottom;
  }

  /// SizeN
  public static void SizeN(Profile.InputLayoutElement layoutElement,
      Point point, Offset offset,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Top以外は固定
    nextLeft = layoutElement.BoundRelativeLeft;
    nextRight = layoutElement.BoundRelativeRight;
    nextBottom = layoutElement.BoundRelativeBottom;

    // Top
    // 領域外に出ないように
    var tryNextTop = point.Y - offset.Top;
    if (tryNextTop < 0.0) tryNextTop = 0.0;
    var upperBound = nextBottom - MinimumHeight;
    if (tryNextTop > upperBound) tryNextTop = upperBound;

    nextTop = tryNextTop;
  }

  /// SizeW
  public static void SizeW(Profile.InputLayoutElement layoutElement,
      Point point, Offset offset,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Left以外は固定
    nextTop = layoutElement.BoundRelativeTop;
    nextRight = layoutElement.BoundRelativeRight;
    nextBottom = layoutElement.BoundRelativeBottom;

    // Left
    // 領域外に出ないように
    var tryNextLeft = point.X - offset.Left;
    if (tryNextLeft < 0.0) tryNextLeft = 0.0;
    var upperBound = nextRight - MinimumWidth;
    if (tryNextLeft > upperBound) tryNextLeft = upperBound;

    nextLeft = tryNextLeft;
  }

  /// SizeS
  public static void SizeS(Profile.InputLayoutElement layoutElement,
      Point point, Offset offset,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Bottom以外は固定
    nextTop = layoutElement.BoundRelativeTop;
    nextLeft = layoutElement.BoundRelativeLeft;
    nextRight = layoutElement.BoundRelativeRight;

    // Bottom
    // 領域外に出ないように
    var tryNextBottom = point.Y - offset.Bottom;
    if (tryNextBottom > 1.0) tryNextBottom = 1.0;
    var lowerBound = nextTop + MinimumHeight;
    if (tryNextBottom < lowerBound) tryNextBottom = lowerBound;

    nextBottom = tryNextBottom;
  }

  /// SizeE
  public static void SizeE(Profile.InputLayoutElement layoutElement,
      Point point, Offset offset,
      out double nextLeft, out double nextTop,
      out double nextRight, out double nextBottom) {
    // Right以外は固定
    nextTop = layoutElement.BoundRelativeTop;
    nextLeft = layoutElement.BoundRelativeLeft;
    nextBottom = layoutElement.BoundRelativeBottom;

    // Right
    // 領域外に出ないように
    var tryNextRight = point.X - offset.Right;
    if (tryNextRight > 1.0) tryNextRight = 1.0;
    var lowerBound = nextLeft + MinimumWidth;
    if (tryNextRight < lowerBound) tryNextRight = lowerBound;

    nextRight = tryNextRight;
  }
}
}   // namespace SCFF.GUI.Controls