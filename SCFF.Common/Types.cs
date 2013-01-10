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

/// @file SCFF.Common/Types.cs
/// SCFF.Commonモジュールで利用する型

namespace SCFF.Common {

using System;
using System.Diagnostics;

//=====================================================================
// 列挙型
//=====================================================================

/// Options用WindowState。System.Windows.WindowStateと相互に変換する。
public enum WindowState {
  Normal,     ///< 標準状態
  Minimized,  ///< 最小化
  Maximized   ///< 最大化
}

/// Profile用Windowの種類
public enum WindowTypes {
  Normal,           ///< 標準のWindow
  DesktopListView,  ///< OS別デスクトップWindow
  Desktop,          ///< ルートWindow
}

/// @copydoc SCFF::Interprocess::LayoutTypes
public enum LayoutTypes {
  NullLayout    = Interprocess.LayoutTypes.NullLayout,
  NativeLayout  = Interprocess.LayoutTypes.NativeLayout,
  ComplexLayout = Interprocess.LayoutTypes.ComplexLayout
}

/// @copydoc SCFF::Interprocess::SWScaleFlags
public enum SWScaleFlags {
  FastBilinear  = Interprocess.SWScaleFlags.FastBilinear,
  Bilinear      = Interprocess.SWScaleFlags.Bilinear,
  Bicubic       = Interprocess.SWScaleFlags.Bicubic,
  X             = Interprocess.SWScaleFlags.X,
  Point         = Interprocess.SWScaleFlags.Point,
  Area          = Interprocess.SWScaleFlags.Area,
  Bicublin      = Interprocess.SWScaleFlags.Bicublin,
  Gauss         = Interprocess.SWScaleFlags.Gauss,
  Sinc          = Interprocess.SWScaleFlags.Sinc,
  Lanczos       = Interprocess.SWScaleFlags.Lanczos,
  Spline        = Interprocess.SWScaleFlags.Spline
}

/// @copydoc SCFF::Interprocess::RotateDirections
public enum RotateDirections {
  NoRotate      = Interprocess.RotateDirections.NoRotate,
  Degrees90     = Interprocess.RotateDirections.Degrees90,
  Degrees180    = Interprocess.RotateDirections.Degrees180,
  Degrees270    = Interprocess.RotateDirections.Degrees270,
}

//=====================================================================
// クラス・構造体
//=====================================================================

//---------------------------------------------------------------------
// スクリーン座標系のPoint/Rect etc.
//---------------------------------------------------------------------

/// スクリーン座標系のPoint
public class ScreenPoint {
  /// コンストラクタ
  public ScreenPoint(int x, int y) {
    this.X = x;
    this.Y = y;
  }
  /// x座標
  public int X { get; private set; }
  /// y座標
  public int Y { get; private set; }
}

/// スクリーン座標系のRect
public class ScreenRect {
  /// コンストラクタ
  public ScreenRect(int x, int y, int width, int height) {
    this.X = x;
    this.Y = y;
    this.Width = width;
    this.Height = height;
  }
  /// 左上端のx座標
  public int X { get; private set; }
  /// 左上端のy座標
  public int Y { get; private set; }
  /// 幅
  public int Width { get; private set; }
  /// 高さ
  public int Height { get; private set; }
  /// 右下端のx座標
  public int Right { get { return this.X + this.Width; } }
  /// 右下端のy座標
  public int Bottom { get { return this.Y + this.Height; } }

  /// 交差判定
  public bool IntersectsWith(ScreenRect other) {
    return !(this.X > other.Right || other.X > this.Right ||
             this.Y > other.Bottom || other.Y > this.Bottom);
  }
  /// 交差
  public ScreenRect Intersect(ScreenRect other) {
    if (!this.IntersectsWith(other)) return null;
    var newX = Math.Max(this.X, other.X);
    var newY = Math.Max(this.Y, other.Y);
    var newRight = Math.Min(this.Right, other.Right);
    var newBottom = Math.Min(this.Bottom, other.Bottom);
    return new ScreenRect(newX, newY, newRight - newX, newBottom - newY);
  }
}

//---------------------------------------------------------------------
// 相対座標系([0-1], [0-1])のPoint/Rect etc.
//---------------------------------------------------------------------

/// ([0-1], [0-1])の相対座標系のPoint
public class RelativePoint {
  /// コンストラクタ
  public RelativePoint(double x, double y) {
    this.X = x;
    this.Y = y;
  }
  /// x座標
  public double X { get; private set; }
  /// y座標
  public double Y { get; private set; }
}

/// ([0-1], [0-1])の相対座標系内の領域を示すLTRB
public class RelativeLTRB {
  /// コンストラクタ
  public RelativeLTRB(double left, double top, double right, double bottom) {
    this.Left = left;
    this.Top = top;
    this.Right = right;
    this.Bottom = bottom;
  }
  /// 左上端のx座標
  public double Left { get; private set; }
  /// 左上端のy座標
  public double Top { get; private set; }
  /// 右下端のx座標
  public double Right { get; private set; }
  /// 右上端のy座標
  public double Bottom { get; private set; }
}

/// ([0-1], [0-1])の相対座標系に収まるRect
public class RelativeRect {
  /// コンストラクタ
  public RelativeRect(double x, double y, double width, double height) {
    this.X = x;
    this.Y = y;
    this.Width = width;
    this.Height = height;
  }
  /// 左上端のx座標
  public double X { get; private set; }
  /// 左上端のy座標
  public double Y { get; private set; }
  /// 幅
  public double Width { get; private set; }
  /// 高さ
  public double Height { get; private set; }
  /// 右下端のx座標
  public double Right { get { return this.X + this.Width; } }
  /// 右下端のy座標
  public double Bottom { get { return this.Y + this.Height; } }

  /// 含有判定
  public bool Contains(RelativePoint point) {
    return this.X <= point.X && point.X <= this.Right &&
           this.Y <= point.Y && point.Y <= this.Bottom;
  }
}

//---------------------------------------------------------------------
// レイアウトパラメータ
//---------------------------------------------------------------------

/// SCFF.Interprocess.LayoutParameter以外のレイアウト要素に必要なデータ
public class AdditionalLayoutParameter {
  /// Windowタイプ: 標準状態 or 最小化 or 最大化
  public WindowTypes WindowType { get; set; }
  /// Window内容を示す文字列(≒Class名)
  public string WindowCaption { get; set; }

  /// クリッピング領域を自動的にウィンドウサイズに合わせるか
  public bool Fit { get; set; }
  /// 相対座標系でのレイアウト要素左上端のX座標
  public double BoundRelativeLeft { get; set; }
  /// 相対座標系でのレイアウト要素左上端のY座標
  public double BoundRelativeTop { get; set; }
  /// 相対座標系でのレイアウト要素右下端のX座標
  public double BoundRelativeRight { get; set; }
  /// 相対座標系でのレイアウト要素右下端のY座標
  public double BoundRelativeBottom { get; set; }

  /// Fit=Falseの時のクリッピング領域左上端のX座標
  public int ClippingXWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域左上端のY座標
  public int ClippingYWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域の幅
  public int ClippingWidthWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域の高さ
  public int ClippingHeightWithoutFit { get; set; }

  /// 保存用: Screen座標系でのクリッピング領域左上端のX座標
  public int BackupScreenClippingX { get; set; }
  /// 保存用: Screen座標系でのクリッピング領域左上端のY座標
  public int BackupScreenClippingY { get; set; }
  /// 保存用: クリッピング領域の幅
  public int BackupClippingWidth { get; set; }
  /// 保存用: クリッピング領域の高さ
  public int BackupClippingHeight { get; set; }
}
}   // namespace SCFF.Common
