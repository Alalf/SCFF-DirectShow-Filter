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

/// @file SCFF.Common/Ext/GDI32.cs
/// @copydoc SCFF::Common::Ext::GDI32

namespace SCFF.Common.Ext {

using System;
using System.Runtime.InteropServices;

/// SCFFで利用するGDI32.dllのAPIをまとめたクラス
///
/// @warning HWNDはUIntPtr、それ以外のHandleはIntPtrで取り扱うこと
public class GDI32 {
  //===================================================================
  // 定数
  //===================================================================

  /// ラスタオペレーションコード: 単純なコピー
  public const int SRCCOPY    = 0x00CC0020;
  /// ラスタオペレーションコード: レイヤードウィンドウ含めコピー
  public const int CAPTUREBLT = 0x40000000;

  /// Pen Style: 描画されない
  public const int PS_NULL        = 0x00000005;
  /// 描画モード: XOR
  public const int R2_XORPEN      = 7;

  //===================================================================
  // API
  //===================================================================

  /// BitBlt(XP,NoAero Vista/7ではハードウェア支援付き)
  [DllImport("gdi32.dll")]
  public static extern int BitBlt(IntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      IntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);

  /// 互換DCを作成
  [DllImport("gdi32.dll")]
	public static extern IntPtr CreateCompatibleDC(IntPtr hdc);

  /// DCを削除
  [DllImport("gdi32.dll")]
  public static extern int DeleteDC(IntPtr hDC);

  /// VRAM上にビットマップ格納領域を生成(XP,NoAero Vista/7)
  [DllImport("gdi32.dll")]
  public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,
                                                     int nWidth, int nHeight);

  /// DCとビットマップハンドルを関連付け
  [DllImport("gdi32.dll")]
  public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

  /// ビットマップを解放
  [DllImport("gdi32.dll")]
  public static extern bool DeleteObject(IntPtr hObject);

  /// ペンを生成
  [DllImport("gdi32.dll")]
  public static extern IntPtr CreatePen(int fnPenStyle, int nWidth,
                                        uint crColor);

  /// 描画モードを設定
  [DllImport("gdi32.dll")]
  public static extern int SetROP2(IntPtr hdc, int fnDrawMode);

  /// 矩形を塗りつぶし
  [DllImport("gdi32.dll")]
  public static extern bool Rectangle(IntPtr hdc, int nLeftRect,
                                      int nTopRect, int nRightRect,
                                      int nBottomRect);
}
}   // namespace SCFF.Common.Ext
