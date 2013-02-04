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

/// @file scff_interprocess/interprocess.h
/// SCFFのプロセス間通信に関するクラス、定数、型の宣言
/// @warning To me: このファイルの中から別のファイルへのIncludeは禁止！
/// - 別の言語に移植する場合も最大2ファイルでお願いします

#ifndef SCFF_DSF_SCFF_INTERPROCESS_INTERPROCESS_H_
#define SCFF_DSF_SCFF_INTERPROCESS_INTERPROCESS_H_

#include <Windows.h>
#include <cstdint>

/// SCFFのプロセス間通信モジュール
namespace scff_interprocess {

// ====================================================================
/// @page smp SCFF Messaging Protocol v1 (by 2012/05/22 Alalf)
/// SCFF_DSFおよびそのクライアントで共有する共有メモリ内のデータ配置の仕様
///
/// ## C++ 版の実装
/// - Windows固有の型名はビットサイズが分かりにくいのでcstdintで置き換える
///   - 対応表
///     - DWORD        = uint32_t (32bit)
///     - HWND(void*)  = uint64_t (64bit)
///       - SCFHから変更: 念のため32bitから64bitに
///     - bool         = int8_t (8bit)
/// - 不動小数点数は基本的にはdoubleで(floatも利用可)
///   - 対応表
///     - double       = double (64bit)
///     - float        = float (32bit)
/// - すべての構造体はPOD(Plain Old Data)であること
///   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
// ====================================================================

/// Path文字列の長さ
static const int kMaxPath = 260;

/// Directoryに格納されるEntryの最大の数
static const int kMaxEntry = 8;

/// ComplexLayout利用時の最大の要素数
/// @sa scff_imaging::kMaxProcessorSize
static const int kMaxComplexLayoutElements = 8;

//---------------------------------------------------------------------

/// 共有メモリ名: SCFFエントリを格納するディレクトリ
static const char kDirectoryName[] = "scff_v1_directory";

/// Directoryの保護用Mutex名
static const char kDirectoryMutexName[] = "mutex_scff_v1_directory";

/// 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
static const char kMessageNamePrefix[] = "scff_v1_message_";

/// Messageの保護用Mutex名の接頭辞
static const char kMessageMutexNamePrefix[] = "mutex_scff_v1_message_";

/// イベント名の接頭辞
static const TCHAR kErrorEventNamePrefix[] = TEXT("scff_v1_error_event_");

//---------------------------------------------------------------------

/// レイアウトの種類
enum class LayoutTypes {
  kNullLayout = 0,  ///< 何も表示しない
  kNativeLayout,    ///< 取り込み範囲1個で、境界は出力に強制的に合わせられる
  kComplexLayout    ///< 取り込み範囲が複数ある
};

//---------------------------------------------------------------------

/// ピクセルフォーマットの種類
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::ImagePixelFormats
enum class ImagePixelFormats {
  kInvalidPixelFormat = -1,   ///< 不正なピクセルフォーマット
  kI420 = 0,                  ///< I420(12bit)
  kIYUV,                      ///< IYUV(12bit)
  kYV12,                      ///< YV12(12bit)
  kUYVY,                      ///< UYVY(16bit)
  kYUY2,                      ///< YUY2(16bit)
  kRGB0,                      ///< RGB0(32bit)
  kSupportedPixelFormatsCount ///</// 対応ピクセルフォーマット数
};

/// 拡大縮小メソッドをあらわす定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::SWScaleFlags
enum class SWScaleFlags {
  kFastBilinear = 1,      ///< fast bilinear
  kBilinear     = 2,      ///< bilinear
  kBicubic      = 4,      ///< bicubic
  kX            = 8,      ///< experimental
  kPoint        = 0x10,   ///< nearest neighbor
  kArea         = 0x20,   ///< averaging area
  kBicublin     = 0x40,   ///< luma bicubic, chroma bilinear
  kGauss        = 0x80,   ///< gaussian
  kSinc         = 0x100,  ///< sinc
  kLanczos      = 0x200,  ///< lanczos
  kSpline       = 0x400   ///< natural bicubic spline
};

/// 回転方向を表す定数
/// @sa scff_imaging/imaging_types.h
/// @sa scff_imaging::RotateDirections
enum class RotateDirections {
  kNoRotate = 0,    ///< 回転なし
  kDegrees90,       ///< 時計回り90度
  kDegrees180,      ///< 時計回り180度
  kDegrees270       ///< 時計回り270度
};

//---------------------------------------------------------------------

// アラインメントをコンパイラに変えられないように
#pragma pack(push, 1)
/// 共有メモリ(Directory)に格納する構造体のエントリ
struct Entry {
  /// SCFF DSFのDLLが使われれているプロセスID
  uint32_t process_id;
  /// SCFF DSFのDLLが使われているプロセス名
  /// @warning 長さが260バイトに制限されているので注意！
  char process_name[kMaxPath];
  /// サンプルの出力width
  int32_t sample_width;
  /// サンプルの出力height
  int32_t sample_height;
  /// サンプルの出力ピクセルフォーマット
  /// @attention ImagePixelFormatを操作に使うこと
  int32_t sample_image_pixel_format;
  /// 目標fps
  double fps;
};

/// 共有メモリ(Directory)に格納する構造体
struct Directory {
  Entry entries[kMaxEntry];
};

/// 拡大縮小設定
/// @sa scff_imaging::SWScaleConfig
struct SWScaleConfig {
  /// 拡大縮小メソッド(Chroma/Luma共通)
  int32_t flags;
  /// 正確な丸め処理
  int8_t accurate_rnd;
  /// 変換前にフィルタをかけるか
  int8_t is_filter_enabled;
  /// 輝度のガウスぼかし
  float luma_gblur;
  /// 色差のガウスぼかし
  float chroma_gblur;
  /// 輝度のシャープ化
  float luma_sharpen;
  /// 色差のシャープ化
  float chroma_sharpen;
  /// 水平方向のワープ
  float chroma_hshift;
  /// 垂直方向のワープ
  float chroma_vshift;
};

/// レイアウトパラメータ
/// @sa scff_imaging::LayoutParameter
struct LayoutParameter {
  /// サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_x;
  /// サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_y;
  /// サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_width;
  /// サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_height;
  /// キャプチャを行う対象となるウィンドウ
  uint64_t window;
  /// 取り込み範囲左上端のX座標
  int32_t clipping_x;
  /// 取り込み範囲左上端のY座標
  int32_t clipping_y;
  /// 取り込み範囲の幅
  int32_t clipping_width;
  /// 取り込み範囲の高さ
  int32_t clipping_height;
  /// マウスカーソルの表示
  int8_t show_cursor;
  /// レイヤードウィンドウの表示
  int8_t show_layered_window;
  /// 拡大縮小設定
  SWScaleConfig swscale_config;
  /// 取り込み範囲が出力サイズより小さい場合拡張
  int8_t stretch;
  /// アスペクト比の保持
  int8_t keep_aspect_ratio;
  /// 回転方向
  /// @attention RotateDirectionを操作に使うこと
  int32_t rotate_direction;
};

/// 共有メモリ(Message)に格納する構造体
struct Message {
  /// タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件
  ///          (0および負数は無効なメッセージを示す)
  int64_t timestamp;
  /// レイアウトの種類
  /// @attention LayoutTypeを操作に使うこと
  int32_t layout_type;
  /// 有効なレイアウト要素の数
  int32_t layout_element_count;
  /// レイアウトパラメータの配列
  LayoutParameter
      layout_parameters[kMaxComplexLayoutElements];
};
#pragma pack(pop)

//---------------------------------------------------------------------

/// SCFFのプロセス間通信を担当するクラス
class Interprocess {
 public:
  /// コンストラクタ
  Interprocess();
  /// デストラクタ
  ~Interprocess();

  /// Directory初期化
  bool InitDirectory();
  /// Directoryの初期化が成功したか
  bool IsDirectoryInitialized();

  /// Message初期化
  bool InitMessage(uint32_t process_id);
  /// Messageの初期化が成功したか
  bool IsMessageInitialized();

  /// ErrorEvent初期化
  bool InitErrorEvent(uint32_t process_id);
  /// ErrorEventの初期化
  bool IsErrorEventInitialized();

  //-------------------------------------------------------------------
  // for SCFF DirectShow Filter
  //-------------------------------------------------------------------
  /// エントリを追加する
  bool AddEntry(const Entry &entry);
  /// エントリを削除する
  bool RemoveEntry(uint32_t process_id);
  /// メッセージを受け取る
  /// @pre 事前にInitMessageが実行されている必要がある
  bool ReceiveMessage(Message *message);
  /// エラーイベントをシグナル状態にする
  bool RaiseErrorEvent();
  //-------------------------------------------------------------------

  /// ディレクトリを取得する
  bool GetDirectory(Directory *directory);
  /// メッセージを作成する
  /// @pre 事前にInitMessageが実行されている必要がある
  bool SendMessage(const Message &message);
  /// エラーイベントを待機する
  bool WaitUntilErrorEventOccured();

 private:
  /// Directory解放
  void ReleaseDirectory();
  /// Message解放
  void ReleaseMessage();
  /// ErrorEvent解放
  void ReleaseErrorEvent();

  /// 共有メモリ: Directory
  HANDLE directory_;
  /// ビュー: Directory
  LPVOID view_of_directory_;
  /// Mutex: Directory
  HANDLE mutex_directory_;

  /// 共有メモリ: Message
  HANDLE message_;
  /// ビュー: Message
  LPVOID view_of_message_;
  /// Mutex: Message
  HANDLE mutex_message_;

  /// イベント: ErrorEvent
  HANDLE error_event_;
};
}   // namespace scff_interprocess

#endif  // SCFF_DSF_SCFF_INTERPROCESS_INTERPROCESS_H_
