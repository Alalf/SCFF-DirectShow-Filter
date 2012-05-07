
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

/// @file base/scff-dsf.cc
/// @brief DirectShow Filterに必要なデータをまとめたファイル

#include <streams.h>

#include "base/constants.h"
#include "base/scff-source.h"
#include "imaging/imaging.h"

//=====================================================================
// フィルタ/ピン設定
//=====================================================================

/// @brief メディアサブタイプGUID: I420
static const GUID kMediaSubtypeI420 =
    static_cast<GUID>(FOURCCMap(MAKEFOURCC('I', '4', '2', '0')));

/// @brief メディアサブタイプGUID: UYVY
static const GUID kMediaSubtypeUYVY =
    static_cast<GUID>(FOURCCMap(MAKEFOURCC('U', 'Y', 'V', 'Y')));

/// @brief メディアサブタイプGUID: RGB0
static const GUID kMediaSubtypeRGB0 =
    MEDIASUBTYPE_RGB32;   // リトルエンディアンであることに注意

/// @brief メディアタイプ: Video
static const AMOVIESETUP_MEDIATYPE kMediaTypes[] = {
#if defined(FOR_KOTOENCODER)
  {&MEDIATYPE_Video, &kMediaSubtypeI420},
#else
  {&MEDIATYPE_Video, &kMediaSubtypeI420},
  {&MEDIATYPE_Video, &kMediaSubtypeUYVY},
  {&MEDIATYPE_Video, &kMediaSubtypeRGB0},
#endif
};

/// @brief ピン: 出力ピンは必ず１個存在する
/// - コメントのないフィールドは使われていない
static const AMOVIESETUP_PIN kOutputPins[] = {
  {
    L"",
    FALSE,        // このピンはレンダリングされるか?
    TRUE,         // 出力ピンか?
    FALSE,        // フィルタはゼロ個のインスタンスを作成できるか?
    FALSE,        // フィルタは複数のインスタンスを作成できるか?
    &GUID_NULL,
    NULL,
    kSupportedPixelFormatsCount,  // メディア タイプの数。
    kMediaTypes   // メディア タイプへのポインタ。
  }
};

/// @brief フィルタ: メリット設定なし。IDは<CLSID_SCFFSource>。
static const AMOVIESETUP_FILTER kSCFFSource = {
  &CLSID_SCFFSource,  // フィルタのクラス識別子。
  kFilterName,        // フィルタの名前。
  MERIT_DO_NOT_USE,   // フィルタのメリット。
                      // IGraphBuilder インターフェイスが
                      // フィルタ グラフの作成時に使う。
  1,                  // フィルタのピンの数。
  kOutputPins         // AMOVIESETUP_PIN 構造体の配列へのポインタ。
                      // 配列のサイズは nPins。
};

//---------------------------------------------------------------------
// キャプチャデバイス設定(IFilterMapper2)
//---------------------------------------------------------------------

/// @brief ピン(IFM2): 出力ピン(カテゴリ: Capture)は必ず１個存在する。
/// - REGPINMEDIUMは使用していない。WDMデバイスではないので必要ない？
static const REGFILTERPINS2 kOutputPinsIFM2[] = {
  {
    REG_PINFLAG_B_OUTPUT,   // REG_PINFLAG フラグのビットごとの組み合わせ
                            // (0 個でもよい)。
    1,                      // ピンのインスタンス数。
    kSupportedPixelFormatsCount,  // このピンがサポートするメディア タイプ数。
    kMediaTypes,            // REGPINTYPES 構造体の配列へのポインタ。
                            // 配列のサイズは nMediaTypes。
    0,                      // メディア数。0 も指定できる。
    NULL,                   // REGPINMEDIUM 構造体の配列へのポインタ。
                            // 配列のサイズは nMediaTypes。
    &PIN_CATEGORY_CAPTURE   // ピン プロパティ セットから得られる
                            // ピン カテゴリ (省略可能)。
  }
};

/// @brief フィルタ(IFM2): Version 2。ピン数は1。
static const REGFILTER2 kSCFFSourceIFM2 = {
  2,                        // フィルタ登録のフォーマット。Version 2。
  MERIT_DO_NOT_USE,         // メリット値が高いフィルタほど先に列挙される。
                            // （ソースフィルタなので列挙されないように）
  1,                        // ピンの数。
  reinterpret_cast<const REGFILTERPINS *>
    (kOutputPinsIFM2)       // ピン情報へのポインタ。
};

//=====================================================================
// Strmbase.libの未定義部分の実装
//=====================================================================

/// @brief ファクトリ テンプレートの配列
CFactoryTemplate g_Templates[1] = {
  {
    kFilterName,                  // フィルタの名前。
    &CLSID_SCFFSource,            // オブジェクトの CLSID へのポインタ。
    SCFFSource::CreateInstance,   // オブジェクトのインスタンスを
                                  // 作成する関数へのポインタ。
    SCFFSource::Init,             // DLL エントリ ポイントから
                                  // 呼び出される関数のポインタ。
    &kSCFFSource                  // AMOVIESETUP_FILTER 構造体へのポインタ。
  }
};

/// @brief ファクトリ テンプレートの配列
int g_cTemplates = sizeof(g_Templates) / sizeof(g_Templates[0]);

//=====================================================================
// DLL Export
//=====================================================================

/// @brief regsvr*.exeから呼ばれる登録関数
STDAPI DllRegisterServer() {
  HRESULT result = AMovieDllRegisterServer2(TRUE);
  if (result != S_OK) return result;

  // フィルタマッパーを取得
  IFilterMapper2 *mapper = NULL;
  result = CoCreateInstance(
    CLSID_FilterMapper2,    // 作成するオブジェクトの CLSID
    NULL,                   // （オブジェクトを結合するときにだけ使う。）
    CLSCTX_INPROC_SERVER,   // DLLとして実装され、アプリケーションの
                            // 処理の一部として実行される
    IID_IFilterMapper2,     // 要求するインターフェイスの
                            // インターフェイス識別子 (IID) に設定
    reinterpret_cast<LPVOID*>(&mapper));
  if (result != S_OK) return result;

  // マッパーを使ってフィルタを登録
  result = mapper->RegisterFilter(
    CLSID_SCFFSource,                 // フィルタのクラス識別子
    kFilterName,                      // フィルタの内容を表すわかりやすい名前
    NULL,                             // このフィルタのデータ出力先を決める
                                      // デバイス モニカへのポインタ
    &CLSID_VideoInputDeviceCategory,  // フィルタ カテゴリ:
                                      //    Video Capture Sources
    kFilterName,                      // デバイス モニカの表示名
                                      // （NULL(CLSID)でもよい。）
    &kSCFFSourceIFM2);                // フィルタ情報
  mapper->Release();

  return result;
}

/// @brief regsvr*.exe /uから呼ばれる登録解除関数
STDAPI DllUnregisterServer() {
  HRESULT result = AMovieDllRegisterServer2(FALSE);
  if (result != S_OK) return result;

  // フィルタマッパーを取得
  IFilterMapper2 *mapper = NULL;
  result = CoCreateInstance(
    CLSID_FilterMapper2,    // 作成するオブジェクトの CLSID
    NULL,                   // （オブジェクトを結合するときにだけ使う。）
    CLSCTX_INPROC_SERVER,   // DLLとして実装され、アプリケーションの
                            // 処理の一部として実行される
    IID_IFilterMapper2,     // 要求するインターフェイスの
                            // インターフェイス識別子 (IID) に設定
    reinterpret_cast<LPVOID*>(&mapper));
  if (result != S_OK) return result;

  // マッパーを使ってフィルタの登録を解除
  result = mapper->UnregisterFilter(
    &CLSID_VideoInputDeviceCategory,  // フィルタ カテゴリ:
                                      //    Video Capture Sources
    kFilterName,                      // デバイス モニカの表示名
    CLSID_SCFFSource);                // フィルタのクラス識別子
  mapper->Release();

  return result;
}

/// @brief DLLEntryPointへの前方参照
extern "C" BOOL WINAPI DllEntryPoint(HINSTANCE, ULONG, LPVOID);

/// @brief DLLMain。DLLEntryPointを呼び出して処理をStrmbase.libに任せる。
BOOL APIENTRY DllMain(HANDLE hModule, DWORD dwReason, LPVOID lpReserved) {
  imaging::Utilities::set_dll_instance(static_cast<HINSTANCE>(hModule));
  return DllEntryPoint((HINSTANCE)(hModule), dwReason, lpReserved);
}
