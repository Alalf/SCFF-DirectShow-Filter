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

/// @file SCFF.Common/GUI/Types.cs
/// SCFF.Common.GUIモジュールで利用する型

/// GUIに関連したクラス(ただしGUIアセンブリ非依存)をまとめた名前空間
namespace SCFF.Common.GUI {

using System;
using SCFF.Common.Ext;

//=====================================================================
// 列挙型
//=====================================================================

/// ヒットテストの結果をまとめた列挙型
public enum HitModes {
  Neutral,  ///< 移動中でも拡大縮小中でもない
  Move,     ///< 移動中
  SizeNW,   ///< 左と上を拡大縮小中
  SizeNE,   ///< 右と上を拡大縮小中
  SizeSW,   ///< 左と下を拡大縮小中
  SizeSE,   ///< 右と下を拡大縮小中
  SizeN,    ///< 上を拡大縮小中
  SizeW,    ///< 左を拡大縮小中
  SizeS,    ///< 下を拡大縮小中
  SizeE     ///< 右を拡大縮小中
}

//=====================================================================
// クラス・構造体
//=====================================================================

//---------------------------------------------------------------------
// 相対座標系([0-1], [0-1])のPoint/Rect etc.
//---------------------------------------------------------------------

/// ([0-1], [0-1])の相対座標系のPoint
public class RelativePoint {
  /// コンストラクタ
  public RelativePoint(double X, double Y) {
    this.X = X;
    this.Y = Y;
  }
  /// x座標
  public double X { get; set; }
  /// y座標
  public double Y { get; set; }
}

/// ([0-1], [0-1])の相対座標系に収まるRect
public class RelativeRect {
  /// 左上端のx座標
  public double X { get; set; }
  /// 左上端のy座標
  public double Y { get; set; }
  /// 幅
  public double Width { get; set; }
  /// 高さ
  public double Height { get; set; }

  /// 含有判定
  public bool Contains(RelativePoint point) {
    return this.X <= point.X && point.X <= this.X + this.Width &&
           this.Y <= point.Y && point.Y <= this.Y + this.Height;
  }
}

/// 相対マウス座標([0-1], [0-1])がレイアウト要素の上下左右とどれだけ離れているか
public class RelativeMouseOffset {
  /// コンストラクタ
  public RelativeMouseOffset(Profile.InputLayoutElement layoutElement, RelativePoint mousePoint) {
    this.Left = mousePoint.X - layoutElement.BoundRelativeLeft;
    this.Top = mousePoint.Y - layoutElement.BoundRelativeTop;
    this.Right = mousePoint.X - layoutElement.BoundRelativeRight;
    this.Bottom = mousePoint.Y - layoutElement.BoundRelativeBottom;
  }

  /// レイアウト要素の左端を原点とした相対マウス座標(x)
  public double Left { get; private set; }
  /// レイアウト要素の上端を原点とした相対マウス座標(y)
  public double Top { get; private set; }
  /// レイアウト要素の右端を原点とした相対マウス座標(x)
  public double Right { get; private set; }
  /// レイアウト要素の下端を原点とした相対マウス座標(y)
  public double Bottom { get; private set; }
}

//---------------------------------------------------------------------
// Win32 Handleのラッパークラス
//---------------------------------------------------------------------

/// Bitmapハンドルのラッパークラス
/// @todo(me) SafeHandleで実装したほうがいいのか考える
public class BitmapHandle : IDisposable {
  /// コンストラクタ
  public BitmapHandle (IntPtr bitmap) {
    this.Bitmap = bitmap;
  }

  /// デストラクタ
  public void Dispose() {
    if (this.Bitmap != IntPtr.Zero) {
      GDI32.DeleteObject(this.Bitmap);
      this.Bitmap = IntPtr.Zero;
    }
  }

  /// ラップしているHBitmap
  public IntPtr Bitmap { get; private set; }
}
}   // namespace SCFF.Common.GUI
