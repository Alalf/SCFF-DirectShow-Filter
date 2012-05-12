
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

using System.Runtime.InteropServices;

/// @brief scff-interprocessモジュールのC#版(オリジナルはC++)
namespace scff_interprocess {

//=====================================================================
// SCFF Messaging Protocol v0 (by 2012/05/02 Alalf)
//
// [全体的な注意点]
// - Windows固有の型名はビットサイズが分かりにくいので
///  System.***で置き換える
//   - 対応表
//     - DWORD        = System.UInt32 32bit
//     - HWND(void*)  = System.UInt64 64bit
//       - SCFHから変更: 念のため32bitから64bitに
//     - bool         = System.Byte 8bit
// - 不動小数点数はdoubleに統一すること
//   - System.Double: 64bit
// - すべての構造体はPOD(Plain Old Data)であること
//   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
//=====================================================================

/// @brief プロセス間通信を担当するクラス
partial class Interprocess {

  /// @brief 共有メモリ名: SCFFエントリを格納するディレクトリ
  const string kDirectoryName = "scff-v0-directory";

  /// @brief Directoryの保護用Mutex名
  const string kDirectoryMutexName = "mutex-scff-v0-directory";

  /// @brief 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
  const string kMessageNamePrefix = "scff-v0-message-";

  /// @brief Messageの保護用Mutex名の接頭辞
  const string kMessageMutexNamePrefix = "mutex-scff-v0-message-";

  //-------------------------------------------------------------------

  /// @brief Path文字列の長さ
  public const int kMaxPath = 260;

  /// @brief Directoryに格納されるEntryの最大の数
  public const int kMaxEntry = 8;

  /// @brief ComplexLayout利用時の最大の要素数
  /// @sa imaging::kMaxProcessorSize
  public const int kMaxComplexLayoutElements = 8;
}

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
  /// @brief UYVY(16bit)
  kUYVY,
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
  /// @brief natural
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

// アラインメントをコンパイラに変えられないように
[StructLayout(LayoutKind.Sequential, Pack = 1)]
/// @brief 共有メモリ(Directory)に格納する構造体のエントリ
public struct Entry {
  /// @brief SCFF DSFのDLLが使われれているプロセスID
  public System.UInt32 process_id;
  /// @brief SCFF DSFのDLLが使われているプロセス名
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Interprocess.kMaxPath)]
  public string process_name;
  /// @brief サンプルの出力width
  public System.Int32 sample_width;
  /// @brief サンプルの出力height
  public System.Int32 sample_height;
  /// @brief サンプルの出力ピクセルフォーマット
  /// @attention ImagePixelFormatを操作に使うこと
  public System.Int32 sample_pixel_format;
  /// @brief 目標fps
  public System.Double fps;
}

/// @brief 共有メモリ(Directory)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Directory {
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxEntry)]
  public Entry[] entries;
}

/// @brief レイアウトパラメータ
/// @sa scff_imaging::ScreenCaptureParameter
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LayoutParameter {
  /// @brief サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public System.Int32 bound_x;
  /// @brief サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public System.Int32 bound_y;
  /// @brief サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  public System.Int32 bound_width;
  /// @brief サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  public System.Int32 bound_height;
  /// @brief キャプチャを行う対象となるウィンドウ
  public System.UInt64 window;
  /// @brief 取り込み範囲の開始X座標
  public System.Int32 clipping_x;
  /// @brief 取り込み範囲の開始y座標
  public System.Int32 clipping_y;
  /// @brief 取り込み範囲の幅
  public System.Int32 clipping_width;
  /// @brief 取り込み範囲の高さ
  public System.Int32 clipping_height;
  /// @brief マウスカーソルの表示
  public System.Byte show_cursor;
  /// @brief レイヤードウィンドウの表示
  public System.Byte show_layered_window;
  /// @brief 拡大縮小アルゴリズムの選択
  /// @attention SWScaleFlagsを操作に使うこと
  public System.Int32 sws_flags;
  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  public System.Byte stretch;
  /// @brief アスペクト比の保持
  public System.Byte keep_aspect_ratio;
  /// @brief 回転方向
  /// @attention RotateDirectionを操作に使うこと
  public System.Int32 rotate_direction;
}

/// @brief 共有メモリ(Message)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Message {
  /// @brief タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件
  /// @warning (0および負数は無効なメッセージを示す)
  public System.Int64 timestamp;
  /// @brief レイアウトの種類
  /// @attention LayoutTypeを操作に使うこと
  public System.Int32 layout_type;
  /// @brief 有効なレイアウト要素の数
  public System.Int32 layout_element_count;
  /// @brief レイアウトパラメータの配列
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxComplexLayoutElements)]
  public LayoutParameter[] layout_parameters;
}
}
