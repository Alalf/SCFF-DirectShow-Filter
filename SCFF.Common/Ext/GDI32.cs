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
  public const int SRCCOPY          = 0x00CC0020;
  /// ラスタオペレーションコード: レイヤードウィンドウ含めコピー
  public const int CAPTUREBLT       = 0x40000000;

  /// Pen Style: 描画されない
  public const int PS_NULL          = 0x00000005;
  /// 描画モード: XOR
  public const int R2_XORPEN        = 7;

  /// ビットマップ圧縮モード: 圧縮なしRGB
  public const uint BI_RGB          = 0;
  /// カラーテーブル: RGB
  public const uint DIB_RGB_COLORS  = 0;

  /// 能力のインデックス: 画面の水平方向での、論理インチ当たりのピクセル数
  public const int LOGPIXELSX       = 88;
  /// 能力のインデックス: 画面の垂直方向での、論理インチ当たりのピクセル数
  public const int LOGPIXELSY       = 90;

  //===================================================================
  // 構造体
  //===================================================================

  /// BITMAPINFOHEADER
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct BITMAPINFOHEADER {
    public uint biSize;         ///< 必要とするバイト数
    public int biWidth;         ///< ビットマップの幅
    public int biHeight;        ///< ビットマップの高さ
    public ushort biPlanes;     ///< プレーンの数
    public ushort biBitCount;   ///< 1ピクセルあたりのビット数
    public uint biCompression;  ///< ビットマップの圧縮モード
    public uint biSizeImage;    ///< イメージのサイズ(非圧縮の場合0でOK)
    public int biXPelsPerMeter; ///< 水平解像度
    public int biYPelsPerMeter; ///< 垂直解像度
    public uint biClrUsed;      ///< 使用しているインデックスカラーの数
    public uint biClrImportant; ///< 重要なインデックスカラーの数
  }

  /// BITMAPINFO
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct BITMAPINFO {
    public BITMAPINFOHEADER bmih; ///< BITMAPINFOHEADER
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
    public uint[] bmiColors;      ///< 色定義
  }

  //===================================================================
  // API
  //===================================================================

  [DllImport("gdi32.dll")]
  public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

  /// BitBlt(XP,NoAero Vista/7ではハードウェア支援付き)
  [DllImport("gdi32.dll")]
  public static extern int BitBlt(IntPtr hDestDC,
      int x, int y,
      int nWidth, int nHeight,
      IntPtr hSrcDC,
      int xSrc, int ySrc,
      int dwRop);

  /// HBitmapからメインメモリに転送
  [DllImport("gdi32.dll")]
  public static extern int GetDIBits(IntPtr hdc,
      IntPtr hbmp, uint uStartScan, uint cScanLines,
      [Out] byte [] lpvBits, ref BITMAPINFO lpbi, uint uUsage);

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
