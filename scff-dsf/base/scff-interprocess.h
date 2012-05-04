
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

/// @file base/scff-interprocess.h
/// @brief SCFFInterprocessの宣言
/// @warning To me: このファイルの中から別のファイルへのIncludeは禁止！
///- 別の言語に移植する場合もh+srcで最大2ファイルでお願いします

#ifndef SCFF_DSF_BASE_SCFF_INTERPROCESS_H_
#define SCFF_DSF_BASE_SCFF_INTERPROCESS_H_

#include <Windows.h>
#include <cstdint>

//=====================================================================
// SCFF Messaging Protocol v0 (by 2012/05/02 Alalf)
//
// [全体的な注意点]
// - Windows固有の型名はビットサイズが分かりにくいのでcstdintで置き換える
//   - 対応表
//     - DWORD        = uint32_t 32bit
//     - HWND(void*)  = uint64_t 64bit
//       - SCFHから変更: 念のため32bitから64bitに
//     - bool         = int8_t 8bit
// - 不動小数点数はdoubleに統一すること
//   - double: 64bit
// - すべての構造体はPOD(Plain Old Data)であること
//   - 基本型、コンストラクタ、デストラクタ、仮想関数を持たない構造体のみ
//=====================================================================

/// @brief 共有メモリ名: SCFFエントリを格納するディレクトリ
static const char kSCFFDirectoryName[] = "scff-v0-directory";

/// @brief SCFFDirectoryの保護用Mutex名
static const char kSCFFDirectoryMutexName[] = "mutex-scff-v0-directory";

/// @brief 共有メモリ名の接頭辞: SCFFで使うメッセージを格納する
static const char kSCFFMessageNamePrefix[] = "scff-v0-message-";

/// @brief SCFFMessageの保護用Mutex名の接頭辞
static const char kSCFFMessageMutexNamePrefix[] = "mutex-scff-v0-message-";

//---------------------------------------------------------------------

/// @brief プロセス名(BaseName)の最大の長さ
static const int kSCFFMaxProcessNameLength = 32;

/// @brief SCFFDirectoryに格納されるSCFFEntryの最大の数
static const int kSCFFMaxEntry = 8;

/// @brief ComplexLayout利用時の最大の要素数
static const int kSCFFMaxComplexLayoutElements = 8;

//---------------------------------------------------------------------

/// @brief ピクセルフォーマットの種類
/// @sa imaging::ImagePixelFormat
enum SCFFPixelFormat {
  /// @brief RGB0(32bit)
  kSCFFRGB0 = 0,
  /// @brief I420(12bit)
  kSCFFI420,
  /// @brief UYVY(16bit)
  kSCFFUYVY
};

/// @brief レイアウトの種類
enum SCFFLayoutType {
  /// @brief 何も表示しない
  kSCFFNullLayout = 0,
  /// @brief 取り込み範囲1個で、境界は出力に強制的に合わせられる
  kSCFFNativeLayout,
  /// @brief 取り込み範囲が複数ある
  /// @warning NOT IMPLEMENTED!
  kSCFFComplexLayout
};

/// @brief 拡大縮小メソッドをあらわす定数(from swscale.h)
/// @sa imaging::SWScaleFlags
enum SCFFSWScaleFlags {
  /// @brief fast bilinear
  kSCFFFastBilinear = 1,
  /// @brief bilinear
  kSCFFBilinear     = 2,
  /// @brief bicubic
  kSCFFBicubic      = 4,
  /// @brief experimental
  kSCFFX            = 8,
  /// @brief nearest neighbor
  kSCFFPoint        = 0x10,
  /// @brief averaging area
  kSCFFArea         = 0x20,
  /// @brief luma bicubic, chroma bilinear
  kSCFFBicublin     = 0x40,
  /// @brief gaussian
  kSCFFGauss        = 0x80,
  /// @brief sinc
  kSCFFSinc         = 0x100,
  /// @brief natural
  kSCFFLanczos      = 0x200,
  /// @brief natural bicubic spline
  kSCFFSpline       = 0x400
};

//---------------------------------------------------------------------

// アラインメントをコンパイラに変えられないように
#pragma pack(push, 1)
/// @brief 共有メモリ(SCFFDirectory)に格納する構造体のエントリ
struct SCFFEntry {
  /// @brief SCFF DSFのDLLが使われれているプロセスID
  uint32_t process_id;
  /// @brief SCFF DSFのDLLが使われているプロセス名
  /// @warning 長さが制限されているので注意！
  char process_name[MAX_PATH];
  /// @brief サンプルの出力width
  int32_t sample_width;
  /// @brief サンプルの出力height
  int32_t sample_height;
  /// @brief サンプルの出力ピクセルフォーマット
  /// @attention SCFFPixelFormatを操作に使うこと
  int32_t sample_pixel_format;
  /// @brief 目標fps
  double fps;
};

/// @brief 共有メモリ(SCFFDirectory)に格納する構造体
struct SCFFDirectory {
  SCFFEntry entries[kSCFFMaxEntry];
};

/// @brief レイアウトパラメータ
/// @sa imaging::ScreenCaptureParameter
struct SCFFLayoutParameter {
  /// @brief サンプル内の原点のX座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_x;
  /// @brief サンプル内の原点のY座標
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_y;
  /// @brief サンプル内の幅
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_width;
  /// @brief サンプル内の高さ
  /// @warning NullLayout,NativeLayoutでは無視される
  int32_t bound_height;
  /// @brief キャプチャを行う対象となるウィンドウ
  uint64_t window;
  /// @brief 取り込み範囲の開始X座標
  int32_t clipping_x;
  /// @brief 取り込み範囲の開始y座標
  int32_t clipping_y;
  /// @brief 取り込み範囲の幅
  int32_t clipping_width;
  /// @brief 取り込み範囲の高さ
  int32_t clipping_height;
  /// @brief マウスカーソルの表示
  int8_t show_cursor;
  /// @brief レイヤードウィンドウの表示
  int8_t show_layered_window;
  /// @brief 拡大縮小アルゴリズムの選択
  /// @attention SCFFSWScaleFlagsを操作に使うこと
  int32_t sws_flags;
  /// @brief 取り込み範囲が出力サイズより小さい場合拡張
  int8_t stretch;
  /// @brief アスペクト比の保持
  int8_t keep_aspect_ratio;
};

/// @brief 共有メモリ(SCFFMessage)に格納する構造体
struct SCFFMessage {
  /// @brief タイムスタンプ(time()で求められたものを想定)
  /// @warning 1ではじまり単調増加が必須条件(0および負数は無効なメッセージを示す)
  int64_t timestamp;
  /// @brief レイアウトの種類
  /// @attention SCFFLayoutTypeを操作に使うこと
  int32_t layout_type;
  /// @brief 有効なレイアウト要素の数
  int32_t layout_element_count;
  /// @brief レイアウトパラメータの配列
  SCFFLayoutParameter
      layout_parameters[kSCFFMaxComplexLayoutElements];
};
#pragma pack(pop)

//---------------------------------------------------------------------

/// @brief SCFFのプロセス間通信を担当するクラス
class SCFFInterprocess {
 public:
  /// @brief コンストラクタ
  SCFFInterprocess();
  /// @brief デストラクタ
  ~SCFFInterprocess();

  /// @brief SCFFDirectory初期化
  bool InitDirectory();
  /// @brief SCFFDirectoryの初期化が成功したか
  bool IsDirectoryInitialized();

  /// @brief SCFFMessage初期化
  bool InitMessage(uint32_t process_id);
  /// @brief SCFFMessageの初期化が成功したか
  bool IsMessageInitialized();

  //-------------------------------------------------------------------
  // for SCFF DirectShow Filter
  //-------------------------------------------------------------------
  /// @brief エントリを作成する
  bool AddEntry(const SCFFEntry &entry);
  /// @brief エントリを削除する
  bool RemoveEntry();
  /// @brief メッセージを受け取る
  bool ReceiveMessage(SCFFMessage *message);
  //-------------------------------------------------------------------

  /// @brief ディレクトリを取得する
  bool GetDirectory(SCFFDirectory *directory);
  /// @brief メッセージを作成する
  bool SendMessage(const SCFFMessage &message);

 private:
  /// @brief SCFFDirectory解放
  void ReleaseDirectory();
  /// @brief SCFFMessage解放
  void ReleaseMessage();

  /// @brief 共有メモリ: SCFFDirectory
  HANDLE scff_directory_;
  /// @brief ビュー: SCFFDirectory
  LPVOID view_of_scff_directory_;
  /// @brief Mutex: SCFFDirectory
  HANDLE mutex_scff_directory_;

  /// @brief プロセスID
  uint32_t process_id_;

  /// @brief 共有メモリ: SCFFMessage
  HANDLE scff_message_;
  /// @brief ビュー: SCFFMessage
  LPVOID view_of_scff_message_;
  /// @brief Mutex: SCFFMessage
  HANDLE mutex_scff_message_;
};

#endif  // SCFF_DSF_BASE_SCFF_INTERPROCESS_H_
