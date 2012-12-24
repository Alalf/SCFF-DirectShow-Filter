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

/// @file scff_interprocess/interprocess.cs
/// SCFFのプロセス間通信に関する定数、型の宣言
/// @warning To me: このファイルの中から別のファイルへのusingは禁止！
/// - 別の言語に移植する場合も最大2ファイルでお願いします

/// scff_interprocessモジュールのC#版(オリジナルはC++)
namespace scff_interprocess {

using System;
using System.Runtime.InteropServices;

//=====================================================================
/// @page smp SCFF Messaging Protocol v1 (by 2012/05/22 Alalf)
/// SCFF_DSFおよびそのクライアントで共有する共有メモリ内のデータ配置の仕様
///
/// [全体的な注意点]
/// - Windows固有の型名はビットサイズが分かりにくいのでSystem.***で置き換える
///   - 対応表
///     - DWORD        = UInt32 (32bit)
///     - HWND(void*)  = UInt64 (64bit)
///       - SCFHから変更: 念のため32bitから64bitに
///     - bool         = Byte (8bit)
/// - 不動小数点数は基本的にはdoubleで(floatも利用可)
///   - 対応表
///     - double       = Double (64bit)
///     - float        = Single (32bit)
/// - すべての構造体はPOD(Plain Old Data)であること
///   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
//=====================================================================

/// プロセス間通信を担当するクラス
partial class Interprocess {
  /// 共有メモリ名: SCFFエントリを格納するディレクトリ
  const string kDirectoryName = "scff_v1_directory";

  /// Directoryの保護用Mutex名
  const string kDirectoryMutexName = "mutex_scff_v1_directory";

  /// 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
  const string kMessageNamePrefix = "scff_v1_message_";

  /// Messageの保護用Mutex名の接頭辞
  const string kMessageMutexNamePrefix = "mutex_scff_v1_message_";

  //-------------------------------------------------------------------

  /// Path文字列の長さ
  public const int kMaxPath = 260;

  /// Directoryに格納されるEntryの最大の数
  public const int kMaxEntry = 8;

  /// ComplexLayout利用時の最大の要素数
  /// @sa scff_imaging::kMaxProcessorSize
  public const int kMaxComplexLayoutElements = 8;
}

//-------------------------------------------------------------------

/// レイアウトの種類
public enum LayoutTypes {
  /// 何も表示しない
  kNullLayout = 0,
  /// 取り込み範囲1個で、境界は出力に強制的に合わせられる
  kNativeLayout,
  /// 取り込み範囲が複数ある
  kComplexLayout
}

//-------------------------------------------------------------------

/// ピクセルフォーマットの種類
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::ImagePixelFormats
public enum ImagePixelFormats {
  /// 不正なピクセルフォーマット
  kInvalidPixelFormat = -1,
  /// I420(12bit)
  kI420 = 0,
  /// IYUV(12bit)
  kIYUV,
  /// YV12(12bit)
  kYV12,
  /// UYVY(16bit)
  kUYVY,
  /// YUY2(16bit)
  kYUY2,
  /// RGB0(32bit)
  kRGB0,
  /// 対応ピクセルフォーマット数
  kSupportedPixelFormatsCount
}

//-------------------------------------------------------------------

/// 拡大縮小メソッドをあらわす定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::SWScaleFlags
public enum SWScaleFlags {
  /// fast bilinear
  kFastBilinear = 1,
  /// bilinear
  kBilinear = 2,
  /// bicubic
  kBicubic = 4,
  /// experimental
  kX = 8,
  /// nearest neighbor
  kPoint = 0x10,
  /// averaging area
  kArea = 0x20,
  /// luma bicubic, chroma bilinear
  kBicublin = 0x40,
  /// gaussian
  kGauss = 0x80,
  /// sinc
  kSinc = 0x100,
  /// lanczos
  kLanczos = 0x200,
  /// natural bicubic spline
  kSpline = 0x400
}

/// 回転方向を表す定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::RotateDirections
public enum RotateDirections {
  /// 回転なし
  kNoRotate = 0,
  /// 時計回り90度
  kDegrees90,
  /// 時計回り180度
  kDegrees180,
  /// 時計回り270度
  kDegrees270
}

/// 共有メモリ(Directory)に格納する構造体のエントリ
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Entry {
  /// SCFF DSFのDLLが使われれているプロセスID
  public UInt32 process_id;
  /// SCFF DSFのDLLが使われているプロセス名
  [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Interprocess.kMaxPath)]
  public string process_name;
  /// サンプルの出力width
  public Int32 sample_width;
  /// サンプルの出力height
  public Int32 sample_height;
  /// サンプルの出力ピクセルフォーマット
  /// @attention ImagePixelFormatを操作に使うこと
  public Int32 sample_pixel_format;
  /// 目標fps
  public Double fps;
}

/// 共有メモリ(Directory)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Directory {
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxEntry)]
  public Entry[] entries;
}

/// 拡大縮小設定
/// @sa scff_imaging::SWScaleConfig
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SWScaleConfig {
  /// 拡大縮小メソッド(Chroma/Luma共通)
  /// @attention 操作にはSWScaleFlagsを使うこと！
  public Int32 flags;
  /// 正確な丸め処理
  public Byte accurate_rnd;
  /// 変換前にフィルタをかけるか
  public Byte is_filter_enabled;
  /// 輝度のガウスぼかし
  public Single luma_gblur;
  /// 色差のガウスぼかし
  public Single chroma_gblur;
  /// 輝度のシャープ化
  public Single luma_sharpen;
  /// 色差のシャープ化
  public Single chroma_sharpen;
  /// 水平方向のワープ
  public Single chroma_hshift;
  /// 垂直方向のワープ
  public Single chroma_vshift;
};

/// レイアウトパラメータ
/// @sa scff_imaging::ScreenCaptureParameter
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct LayoutParameter {
  /// サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_x;
  /// サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_y;
  /// サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_width;
  /// サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  public Int32 bound_height;
  /// キャプチャを行う対象となるウィンドウ
  public UInt64 window;
  /// 取り込み範囲の開始X座標
  public Int32 clipping_x;
  /// 取り込み範囲の開始y座標
  public Int32 clipping_y;
  /// 取り込み範囲の幅
  public Int32 clipping_width;
  /// 取り込み範囲の高さ
  public Int32 clipping_height;
  /// マウスカーソルの表示
  public Byte show_cursor;
  /// レイヤードウィンドウの表示
  public Byte show_layered_window;
  /// 拡大縮小設定
  public SWScaleConfig swscale_config;
  /// 取り込み範囲が出力サイズより小さい場合拡張
  public Byte stretch;
  /// アスペクト比の保持
  public Byte keep_aspect_ratio;
  /// 回転方向
  /// @attention RotateDirectionを操作に使うこと
  public Int32 rotate_direction;
}

/// 共有メモリ(Message)に格納する構造体
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Message {
  /// タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件
  /// @warning (0および負数は無効なメッセージを示す)
  public Int64 timestamp;
  /// レイアウトの種類
  /// @attention LayoutTypeを操作に使うこと
  public Int32 layout_type;
  /// 有効なレイアウト要素の数
  public Int32 layout_element_count;
  /// レイアウトパラメータの配列
  [MarshalAs(UnmanagedType.ByValArray, SizeConst = Interprocess.kMaxComplexLayoutElements)]
  public LayoutParameter[] layout_parameters;
}
}
