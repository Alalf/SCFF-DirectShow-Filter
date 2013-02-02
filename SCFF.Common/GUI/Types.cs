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
using System.Diagnostics;
using SCFF.Common.Ext;
using SCFF.Common.Profile;

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

/// マウス座標(相対座標系)がレイアウト要素の上下左右とどれだけ離れているか
public class RelativeMouseOffset : RelativeLTRB {
  /// コンストラクタ
  public RelativeMouseOffset(ILayoutElementView layoutElement,
                             RelativePoint mousePoint)
      : base(mousePoint.X - layoutElement.BoundRelativeLeft,
             mousePoint.Y - layoutElement.BoundRelativeTop,
             mousePoint.X - layoutElement.BoundRelativeRight,
             mousePoint.Y - layoutElement.BoundRelativeBottom) {
    // nop
  }
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

  /// Dispose
  public void Dispose() {
    if (this.Bitmap != IntPtr.Zero) {
      GDI32.DeleteObject(this.Bitmap);
      this.Bitmap = IntPtr.Zero;
    }
    GC.SuppressFinalize(this);
  }

  /// デストラクタ
  ~BitmapHandle() {
    this.Dispose();
  }

  /// ラップしているHBitmap
  public IntPtr Bitmap { get; private set; }
}

/// スタイルNULLのPenハンドルのラッパークラス
/// @todo(me) SafeHandleで実装したほうがいいのか考える
public class NullPen : IDisposable {
  /// コンストラクタ
  public NullPen() {
    this.Pen = GDI32.CreatePen(GDI32.PS_NULL, 1, 0x00000000);
    Debug.WriteLine("NullPen", "*** MEMORY[NEW] ***");
  }

  /// Dispose
  public void Dispose() {
    if (this.Pen != IntPtr.Zero) {
      GDI32.DeleteObject(this.Pen);
      this.Pen = IntPtr.Zero;
      Debug.WriteLine("NullPen", "*** MEMORY[DISPOSE] ***");
    }
    GC.SuppressFinalize(this);
  }

  /// デストラクタ
  ~NullPen() {
    this.Dispose();
  }

  /// ラップしているHPen
  public IntPtr Pen { get; private set; }
}
}   // namespace SCFF.Common.GUI
