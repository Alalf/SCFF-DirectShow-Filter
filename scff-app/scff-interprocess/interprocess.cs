// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
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

/// @file scff-interprocess/interprocess.cs
/// @brief SCFFのプロセス間通信に関する定数、型の宣言
/// @warning To me: このファイルの中から別のファイルへのusingは禁止！
///- 別の言語に移植する場合も最大2ファイルでお願いします

using System;
using System.Runtime.InteropServices;

/// @brief scff_interprocessモジュールのC#版(オリジナルはC++)
namespace scff_interprocess {

//=====================================================================
// SCFF Messaging Protocol v1 (by 2012/05/22 Alalf)
//
// [全体的な注意点]
// - Windows固有の型名はビットサイズが分かりにくいので
///  System.***で置き換える
//   - 対応表
//     - DWORD        = UInt32 32bit
//     - HWND(void*)  = UInt64 64bit
//       - SCFHから変更: 念のため32bitから64bitに
//     - bool         = Byte 8bit
// - 不動小数点数は基本的にはdoubleでやりとりすること
//   - Double = 64bit
//   - Single = 32bit
// - すべての構造体はPOD(Plain Old Data)であること
//   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
//=====================================================================

/// @brief プロセス間通信を担当するクラス
partial class Interprocess {
  /// @brief 共有メモリ名: SCFFエントリを格納するディレクトリ
  const string kDirectoryName = "scff-v1-directory";

  /// @brief Directoryの保護用Mutex名
  const string kDirectoryMutexName = "mutex-scff-v1-directory";

  /// @brief 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
  const string kMessageNamePrefix = "scff-v1-message-";

  /// @brief Messageの保護用Mutex名の接頭辞
  const string kMessageMutexNamePrefix = "mutex-scff-v1-message-";

  //-------------------------------------------------------------------

  /// @brief Path文字列の長さ
  public const int kMaxPath = 260;

  /// @brief Directoryに格納されるEntryの最大の数
  public const int kMaxEntry = 8;

  /// @brief ComplexLayout利用時の最大の要素数
  /// @sa imaging::kMaxProcessorSize
  public const int kMaxComplexLayoutElements = 8;
}

//-------------------------------------------------------------------

/// @brief レイアウトの種類
public enum LayoutType {
  /// @brief 何も表示しない
  kNullLayout = 0,
  /// @brief 取り込み範囲1個で、境界は出力に強制的に合わせられる
  kNativeLayout,
  /// @brief 取り込み範囲が複数ある
  kComplexLayout
}

//-------------------------------------------------------------------

/// @brief ピクセルフォーマットの種類
/// @sa scff-imaging/imaging-types.h
/// @sa scff_imaging::ImagePixelFormat
public enum ImagePixelFormat {
  /// @brief 不正なピクセルフォーマット
  kInvalidPixelFormat = -1,
  /// @brief I420(12bit)
  kI420 = 0,
  /// @brief IYUV(12bit)
  kIYUV,
  /// @brief YV12(12bit)
  kYV12,
  /// @brief UYVY(16bit)
  kUYVY,
  /// @brief YUY2(16bit)
  kYUY2,
  /// @brief RGB0(32bit)
  kRGB0,
  /// @brief 対応ピクセルフォーマット数
  kSupportedPixelFormatsCount
}

//-------------------------------------------------------------------

/// @brief 拡大縮小メソッドをあらわす定数
/// @sa scff-imaging/imaging-types.h
/// @sa scff_imaging::SWScaleFlags
public enum SWScaleFlags {
  /// @brief fast bilinear
  kFastBilinear = 1,
  /// @brief bilinear
  kBilinear = 2,
  /// @brief bicubic
  kBicubic = 4,
  /// @brief experimental
  kX = 8,
  /// @brief nearest neighbor
  kPoint = 0x10,
  /// @brief averaging area
  kArea = 0x20,
  /// @brief luma bicubic, chroma bilinear
  kBicublin = 0x40,
  /// @brief gaussian
  kGauss = 0x80,
  /// @brief sinc
  kSinc = 0x100,
  /// @brief lanczos
  kLanczos = 0x200,
  /// @brief natural bicubic spline
  kSpline = 0x400
}

/// @brief 回転方向を表す定数
/// @sa scff-imaging/imaging-types.h
/// @sa scff_imaging::RotateDirection
public enum RotateDirection {
  /// @brief 回転なし
  kNoRotate = 0,
  /// @brief 時計回り90度
  k90Degrees,
  /// @brief 時計回り180度
  k180Degrees,
  /// @brief 時計回り270度
  k270Degrees
}

/// @brief 共有メモリ(Directory)に格納する構造体のエントリ
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Entry {
  /// @brief SCFF DSFのDLLが使われれているプロセスID
  public UInt32 process_id;
  /// @brief SCFF DSFのDLLが使われているプロセス名
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Interprocess.kMaxPath)]
  public string process_name;
  /// @brief サンプルの出力width
  public Int32 sample_width;
  /// @brief サンプルの出力height
  public Int32 sample_height;
  /// @brief サンプルの出力ピクセルフォーマット
  /// @attention ImagePixelFormatを操作に使うこと
  public Int32 sample_pixel_format;
  /// @brief 目標fps
  public Double fps;
}

/// @brief 共有メモリ(Directory)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Directory {
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxEntry)]
  public Entry[] entries;
}

/// @brief 拡大縮小設定
/// @sa scff_imaging::SWScaleConfig
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SWScaleConfig {
  /// @brief 拡大縮小メソッド(Chroma/Luma共通)
  /// @attention 操作にはSWScaleFlagsを使うこと！
  public Int32 flags;
  /// @brief 正確な丸め処理
  public Byte accurate_rnd;
  /// @brief 変換前にフィルタをかけるか
  public Byte is_filter_enabled;
  /// @brief 輝度のガウスぼかし
  public Single luma_gblur;
  /// @brief 色差のガウスぼかし
  public Single chroma_gblur;
  /// @brief 輝度のシャープ化
  public Single luma_sharpen;
  /// @brief 色差のシャープ化
  public Single chroma_sharpen;
  /// @brief 水平方向のワープ
  public Single chroma_hshift;
  /// @brief 垂直方向のワープ
  public Single chroma_vshift;
};

/// @brief レイアウトパラメータ
/// @sa scff_imaging::ScreenCaptureParameter
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LayoutParameter {
  /// @brief サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_x;
  /// @brief サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_y;
  /// @brief サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_width;
  /// @brief サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_height;
  /// @brief キャプチャを行う対象となるウィンドウ
  public UInt64 window;
  /// @brief 取り込み範囲の開始X座標
  public Int32 clipping_x;
  /// @brief 取り込み範囲の開始y座標
  public Int32 clipping_y;
  /// @brief 取り込み範囲の幅
  public Int32 clipping_width;
  /// @brief 取り込み範囲の高さ
  public Int32 clipping_height;
  /// @brief マウスカーソルの表示
  public Byte show_cursor;
  /// @brief レイヤードウィンドウの表示
  public Byte show_layered_window;
  /// @brief 拡大縮小設定
  public SWScaleConfig swscale_config;
  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  public Byte stretch;
  /// @brief アスペクト比の保持
  public Byte keep_aspect_ratio;
  /// @brief 回転方向
  /// @attention RotateDirectionを操作に使うこと
  public Int32 rotate_direction;
}

/// @brief 共有メモリ(Message)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Message {
  /// @brief タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件
  /// @warning (0および負数は無効なメッセージを示す)
  public Int64 timestamp;
  /// @brief レイアウトの種類
  /// @attention LayoutTypeを操作に使うこと
  public Int32 layout_type;
  /// @brief 有効なレイアウト要素の数
  public Int32 layout_element_count;
  /// @brief レイアウトパラメータの配列
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxComplexLayoutElements)]
  public LayoutParameter[] layout_parameters;
}
}
