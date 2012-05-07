
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

/// @file base/scff-output-pin.cc
/// @brief SCFFOutputPinの実装(基底クラスのみ)

#include "base/scff-output-pin.h"

#include <math.h>

#include <algorithm>

#include "base/constants.h"
#include "base/debug.h"
#include "base/scff-source.h"
#include "base/scff-quality-controlled-time.h"
#include "base/scff-monitor.h"

#include "imaging/imaging.h"

//=====================================================================
// SCFFOutputPin
//=====================================================================

// コンストラクタ
SCFFOutputPin::SCFFOutputPin(HRESULT *result, CSource *source)
  : CSourceStream(kFilterName, result, source, L"Capture"),
    width_(kPreferredSizes[1].cx),    // 0はダミーなので1
    height_(kPreferredSizes[1].cy),   // 0はダミーなので1
    fps_(kDefaultFPS),
    can_use_quality(false),
    offset_(0) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
    TEXT("SCFFOutputPin: NEW(%d, %d, %.1ffps)"),
    width_, height_, fps_));
}

// デストラクタ
SCFFOutputPin::~SCFFOutputPin() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
    TEXT("SCFFOutputPin: DELETE(%d, %d, %.1ffps)"),
    width_, height_, fps_));
}

//=====================================================================
// 基底クラスの実装
//=====================================================================

//---------------------------------------------------------------------
// CBasePin
//---------------------------------------------------------------------

// インデックス値で優先メディア タイプを取得
//
///- 以後ピンに渡されるmedia_typeは必ず以下になることが保障される:
///  - biCompression:  RGB0/I420/UYVY
///  - biBitCount:     32/12/16
///  - biWidth:        width_
///  - biHeight:       height_
///  - biSizeImage:    (...)
///  - biPlanes:       1
///  - FPS(AvgTimePerFrame): 1/fps_
///  - Type:           MEDIATYPE_Video
///  - FormatType:     FORMAT_VideoInfo
///  - SampleSize:     (== biSizeImage)
///
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval VFW_S_NO_MORE_ITEMS
/// @retval E_OUTOFMEMORY
/// @retval S_OK
HRESULT SCFFOutputPin::GetMediaType(int position, CMediaType *media_type) {
  // 引数チェック
  CheckPointer(media_type, E_POINTER);
  if (position < 0) return E_INVALIDARG;
  if (position >= kSupportedFormatsCount) return VFW_S_NO_MORE_ITEMS;

  // サイズおよびピクセルフォーマット設定用
  const int position_in_preferred_sizes = position % kPreferredSizesCount;
  const int position_in_pixel_formats = position / kPreferredSizesCount;

  // ロック: m_pFilter->pStateLock()
  CAutoLock lock(m_pFilter->pStateLock());

  // ビデオ設定を表すVIDEOINFOの作成
  VIDEOINFO *video_info = reinterpret_cast<VIDEOINFO *>
                          (media_type->AllocFormatBuffer(sizeof(VIDEOINFO)));
  CheckPointer(video_info, E_OUTOFMEMORY);
  ZeroMemory(video_info, sizeof(VIDEOINFO));

  //-------------------------------------------------------------------
  // １フレームの設定を表すBITMAPINFOHEADERの設定

  // カラーテーブルは必要ない
  video_info->bmiHeader.biSize          = sizeof(BITMAPINFOHEADER);

  // サイズ設定の優先度は以下のとおり:
  //  0.  SetMediaTypeでピン接続先から指定されたサイズ
  //  1-. 優先サイズの配列順
  switch (position_in_preferred_sizes) {
  case 0:
    video_info->bmiHeader.biWidth   = width_;
    video_info->bmiHeader.biHeight  = height_;
    break;
  default:
    video_info->bmiHeader.biWidth   =
        kPreferredSizes[position_in_preferred_sizes].cx;
    video_info->bmiHeader.biHeight  =
        kPreferredSizes[position_in_preferred_sizes].cy;
    break;
  }

  video_info->bmiHeader.biPlanes        = 1;
  video_info->bmiHeader.biClrImportant  = 0;

  int data_size = -1;
#if defined(FOR_KOTOENCODER)
  // I420(12bit)
  video_info->bmiHeader.biCompression   = MAKEFOURCC('I', '4', '2', '0');
  video_info->bmiHeader.biBitCount      = 12;
  data_size =
      imaging::Utilities::CalcDataSize(imaging::kI420,
                                        video_info->bmiHeader.biWidth,
                                        video_info->bmiHeader.biHeight);
#else
  switch (position_in_pixel_formats) {
  case imaging::kI420:
    // I420(12bit)
    video_info->bmiHeader.biCompression   = MAKEFOURCC('I', '4', '2', '0');
    video_info->bmiHeader.biBitCount      = 12;
    data_size =
        imaging::Utilities::CalcDataSize(imaging::kI420,
                                         video_info->bmiHeader.biWidth,
                                         video_info->bmiHeader.biHeight);
    break;
  case imaging::kUYVY:
    // UYVY(16bit)
    video_info->bmiHeader.biCompression   = MAKEFOURCC('U', 'Y', 'V', 'Y');
    video_info->bmiHeader.biBitCount      = 16;
    data_size =
        imaging::Utilities::CalcDataSize(imaging::kUYVY,
                                         video_info->bmiHeader.biWidth,
                                         video_info->bmiHeader.biHeight);
    break;
  case imaging::kRGB0:
    // RGB0(32bit)
    video_info->bmiHeader.biCompression   = BI_RGB;
    video_info->bmiHeader.biBitCount      = 32;
    data_size =
        imaging::Utilities::CalcDataSize(imaging::kRGB0,
                                         video_info->bmiHeader.biWidth,
                                         video_info->bmiHeader.biHeight);
    break;
  }
#endif

  ASSERT(data_size != -1);
  video_info->bmiHeader.biSizeImage     = data_size;
  // @attention GetBitmapSize(&video_info->bmiHeader)のほうが小さい？

  //-------------------------------------------------------------------
  // FPSなどのVIDEOINFOの設定
  video_info->AvgTimePerFrame = ToFrameInterval(fps_);

  // クリッピングしない
  SetRectEmpty(&video_info->rcSource);
  SetRectEmpty(&video_info->rcTarget);

  media_type->SetType(&MEDIATYPE_Video);
  media_type->SetSubtype(&GetBitmapSubtype(&video_info->bmiHeader));
  // 非正方形ピクセルやインターレースは使わない
  media_type->SetFormatType(&FORMAT_VideoInfo);
  media_type->SetSampleSize(video_info->bmiHeader.biSizeImage);
  media_type->SetTemporalCompression(FALSE);

  MyDbgLog((LOG_TRACE, kDbgTrace,
    TEXT("[pin] -> mediatype(%dbpp, %d, %d, %.1ffps)"),
    video_info->bmiHeader.biBitCount,
    video_info->bmiHeader.biWidth,
    video_info->bmiHeader.biHeight,
    ToFPS(video_info->AvgTimePerFrame)));

  return S_OK;
}

// ピンが特定のメディア タイプを受け入れるかどうかを判定
//
// さまざまなアプリケーションがこのメソッドを利用しているので
// 優先メディアタイプが一つであっても実装したほうがよさそうだ
/// @todo(me) ワイルドカード的なことを処理する場合はS_FALSE
///
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~{.cc}
/// // We treat MEDIASUBTYPE_NULL subtype as a wild card
/// if((m_pReader->LoadType()->majortype == pType->majortype) &&
///    (m_pReader->LoadType()->subtype == MEDIASUBTYPE_NULL   ||
///    m_pReader->LoadType()->subtype == pType->subtype))
/// {
///   return S_OK;
/// }
/// return S_FALSE;
/// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval E_UNEXPECTED
/// @retval S_OK
HRESULT SCFFOutputPin::CheckMediaType(const CMediaType *media_type) {
  // 引数チェック
  CheckPointer(media_type, E_POINTER);

  // サイズ固定のビデオか？
  const GUID type = *(media_type->Type());
  if (type != MEDIATYPE_Video || !media_type->IsFixedSize()) {
    return E_INVALIDARG;
  }

  // ピクセルフォーマットが適切か？
  CheckPointer(media_type->Subtype(), E_INVALIDARG);
  const GUID subtype = *(media_type->Subtype());
  const GUID I420_guid = FOURCCMap(MAKEFOURCC('I', '4', '2', '0'));
  const GUID UYVY_guid = FOURCCMap(MAKEFOURCC('U', 'Y', 'V', 'Y'));
  const GUID RGB0_guid = MEDIASUBTYPE_RGB32;

  if (subtype != I420_guid && subtype != UYVY_guid && subtype != RGB0_guid) {
    return E_INVALIDARG;
  }

  // 以下VIDEOINFOのチェック
  VIDEOINFO *video_info =
    reinterpret_cast<VIDEOINFO*>(media_type->Format());
  CheckPointer(video_info, E_UNEXPECTED);

  // サイズとFPSの設定が正しくされているか？
  /// @attention Skypeに対応する場合はサイズ(0,0)を許さなければならない
  if (video_info->bmiHeader.biWidth == 0 ||
      video_info->bmiHeader.biHeight <= 0 ||
      video_info->AvgTimePerFrame == 0) {
    return E_INVALIDARG;
  }

  MyDbgLog((LOG_TRACE, kDbgTrace,
    TEXT("[pin] <- check: mediatype(%dbpp, %d, %d, %.1ffps)"),
    video_info->bmiHeader.biBitCount,
    video_info->bmiHeader.biWidth,
    video_info->bmiHeader.biHeight,
    ToFPS(video_info->AvgTimePerFrame)));
  return S_OK;
}

// 接続のメディア タイプを設定
//
// ここではじめて下位のフィルタが要求したフレームの画像サイズを取得できる
/// @pre media_typeはCheckMediaTypeによってチェック済み
/// @retval E_POINTER
/// @retval E_UNEXPECTED
/// @retval E_INVALIDARG
/// @retval E_UNEXPECTED
/// @retval S_OK
HRESULT SCFFOutputPin::SetMediaType(const CMediaType *media_type) {
  CheckPointer(media_type, E_POINTER);

  // ロック: m_pFilter->pStateLock()
  CAutoLock lock(m_pFilter->pStateLock());

  VIDEOINFO *video_info =
    reinterpret_cast<VIDEOINFO*>(media_type->Format());
  CheckPointer(video_info, E_UNEXPECTED);

  MyDbgLog((LOG_TRACE, kDbgTrace,
    TEXT("[pin] <- mediatype(%d, %d, %d, %.1ffps)"),
    video_info->bmiHeader.biBitCount,
    video_info->bmiHeader.biWidth,
    video_info->bmiHeader.biHeight,
    ToFPS(video_info->AvgTimePerFrame)));

  /// @attention ここもSkypeで引っかかるかも
  if (video_info->bmiHeader.biWidth == 0 ||
      video_info->bmiHeader.biHeight == 0 ||
      video_info->AvgTimePerFrame == 0) {
    return E_INVALIDARG;
  }

  // m_mtに設定
  HRESULT result = CSourceStream::SetMediaType(media_type);
  if (result != S_OK) return result;

  // 新しい設定をチェック
  VIDEOINFO *specified_video_info =
    reinterpret_cast<VIDEOINFO*>(m_mt.Format());
  CheckPointer(specified_video_info, E_UNEXPECTED);

  if (specified_video_info->AvgTimePerFrame <= 0) {
    return E_INVALIDARG;
  }

  /// @warning biBitCountでピクセルフォーマットを判断しているが、
  /// @warning これは正確ではない。
  /// @warning 本来はbiCompressionで判断するべきであるが、
  /// @warning GUIDの比較関数が見当たらないので
  /// @warning 現時点ではbiBitCountで判断することにした。
  if (specified_video_info->bmiHeader.biBitCount != 12 &&
      specified_video_info->bmiHeader.biBitCount != 16 &&
      specified_video_info->bmiHeader.biBitCount != 32) {
    return E_INVALIDARG;
  }

  // ここで実際の値を取得
  width_  = specified_video_info->bmiHeader.biWidth;
  height_ = specified_video_info->bmiHeader.biHeight;
  fps_    = ToFPS(specified_video_info->AvgTimePerFrame);

  return S_OK;
}

//---------------------------------------------------------------------
// CBaseOutputPin
//---------------------------------------------------------------------

// fpsから適切なバッファの数を求める
/// @attention バッファの数はSCFH DSFに準拠
int SCFFOutputPin::CalcBufferCount() {
  return max(static_cast<int>(ceil(fps_ * 0.3)), 4);
}

// バッファ要求を設定
//
/// @pre SetMediaTypeによってm_mtには適切なフォーマットが格納済み
/// @retval E_POINTER
/// @retval E_FAIL
/// @retval S_OK
HRESULT SCFFOutputPin::DecideBufferSize(IMemAllocator *allocator,
                                  ALLOCATOR_PROPERTIES *request) {
  // 引数チェック
  CheckPointer(allocator, E_POINTER);
  CheckPointer(request, E_POINTER);

  // ロック: m_pFilter->pStateLock()
  CAutoLock lock(m_pFilter->pStateLock());

  // FPSからバッファの数を計算
  /// @todo(me) サイズを考慮したバッファカウントにするべきだろうか？
  VIDEOINFO *video_info = reinterpret_cast<VIDEOINFO*>(m_mt.Format());
  request->cBuffers = CalcBufferCount();
  request->cbBuffer = video_info->bmiHeader.biSizeImage;
  ASSERT(request->cbBuffer);

  ALLOCATOR_PROPERTIES actual_allocator;
  HRESULT result = allocator->SetProperties(request, &actual_allocator);
  if (result != S_OK) return result;

  // 接続先の入力ピンから要求されたバッファの数よりも
  // 実際の割り当て数が小さかった場合
  if (actual_allocator.cbBuffer < request->cbBuffer) {
    return E_FAIL;
  }

  ASSERT(actual_allocator.cBuffers == CalcBufferCount());
  MyDbgLog((LOG_TRACE, kDbgTrace,
    TEXT("[pin] -> (%ld buffers we need)"),
    actual_allocator.cBuffers));

  return S_OK;
}

//---------------------------------------------------------------------
// CSourceStream
//---------------------------------------------------------------------

//-----------------------------------------------------------------
// ストリーミング スレッドが初期化されたときに呼び出される
// スレッド終了させるにはE_を返す
/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadCreate(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadCreate")));

  CAutoLock lock(&filling_buffer_);
  // タイムマネージャをリセット
  can_use_quality = false;
  quality_controlled_time_.Reset(fps_);
  clock_time_.Reset(fps_);

  return S_OK;
}

// ストリーミング スレッドが間もなく終了するときに呼び出される
// スレッド終了させるにはE_を返す
/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadDestroy(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadDestroy")));
  return S_OK;
}

// CSourceStream::DoBufferProcessingLoop メソッドの
// 処理が開始されたときに呼び出される
/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadStartPlay(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadStartPlay")));
  return S_OK;
}

// メディア データを生成し、ダウンストリームの入力ピンに提供
//
// デフォルトの動作だとまったく待たずに最速でFillBufferを埋めてしまうので
// 適切なWaitをはさむ様に置き換えている。
// また、ワーカースレッドを作成しFillBufferのためのデータをバックグラウンドで
// 生成するようにしている。
//
/// @retval S_OK
/// @retval S_FALSE ループ正常終了
HRESULT SCFFOutputPin::DoBufferProcessingLoop(void) {
  Command command;

  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: DoBufferProcessingLoop starts")));

  OnThreadStartPlay();

  // ImagingEngineを作成

  /// @warning biBitCountでピクセルフォーマットを判断しているが、
  /// @warning これは正確ではない。
  /// @warning 本来はbiCompressionで判断するべきであるが、
  /// @warning GUIDの比較関数が見当たらないので
  /// @warning 現時点ではbiBitCountで判断することにした。
  ASSERT(m_mt.formattype == FORMAT_VideoInfo);
  VIDEOINFOHEADER *video_info =
    reinterpret_cast<VIDEOINFOHEADER*>(m_mt.pbFormat);
  imaging::ImagePixelFormat pixel_format = imaging::kInvalidPixelFormat;
  switch (video_info->bmiHeader.biBitCount) {
  case 12:
    pixel_format = imaging::kI420;
    break;
  case 16:
    pixel_format = imaging::kUYVY;
    break;
  case 32:
    pixel_format = imaging::kRGB0;
    break;
  }
  ASSERT(pixel_format != imaging::kInvalidPixelFormat);

  imaging::Engine engine(pixel_format, width_, height_, fps_);
  const imaging::ErrorCode error = engine.Init();
  ASSERT(error == imaging::kNoError);

  // SCFFMonitorとリクエスト格納用変数を作成
  SCFFMonitor monitor;
  monitor.Init(pixel_format, width_, height_, fps_);
  imaging::Request *request = 0;

  // タイムマネージャ用
  bool is_first_loop = true;

  // ループ開始
  do {
    while (!CheckRequest(&command)) {
      // 接続先のピンからバッファを受け取る
      IMediaSample *sample;
      HRESULT result = GetDeliveryBuffer(&sample, NULL, NULL, 0);
      if (result != S_OK) {
        Sleep(1);   // 分解能1mSec
        continue;
      }

      // Requestを生成してEngineに受け渡す
      request = monitor.CreateRequest();
      engine.Accept(request);
      monitor.ReleaseRequest(request);

      // サンプルに開始時間と終了時間を設定
      HRESULT result_fill_buffer;
      {
        CAutoLock lock(&filling_buffer_);

        // QualityControlledTime(以下qct)を
        // いつでも切り替えられるように更新しておく
        REFERENCE_TIME start_for_qct;
        REFERENCE_TIME end_for_qct;
        quality_controlled_time_.GetTimestamp(&start_for_qct, &end_for_qct);
        quality_controlled_time_.UpdateNow(end_for_qct);

        // ClockTime(以下ct)を
        // いつでも切り替えられるように更新しておく
        REFERENCE_TIME start_for_ct;
        REFERENCE_TIME end_for_ct;
        clock_time_.GetTimestamp(&start_for_ct, &end_for_ct);

        // サンプルにデータを詰める (FillBuffer()は使わない)
        result_fill_buffer = FillBufferWithImagingEngine(engine, sample);

        if (is_first_loop || can_use_quality) {
          // 正確なタイムマネージャで時間あわせが可能
          sample->SetTime(&start_for_qct, &end_for_qct);
        } else {
          // 原始的なのでタイムスタンプがまともに設定できない
          sample->SetTime(&start_for_ct, &end_for_ct);
        }
      }
      sample->SetSyncPoint(TRUE);

      if (result_fill_buffer == S_OK) {
        // FillBuffer成功なのでSampleをDeliver
        HRESULT result_deliver = Deliver(sample);
        sample->Release();
        if (result_deliver != S_OK) {
          MyDbgLog((LOG_TRACE, kDbgRare,
            TEXT("SCFFOutputPin: Deliver() returned %08x; stopping"),
            result_deliver));
          return S_OK;
        }
      } else if (result_fill_buffer == S_FALSE) {
        // ストリーム終了(＝FillBuffer失敗)
        sample->Release();
        DeliverEndOfStream();
        return S_OK;
      } else {
        // FillBufferでエラー発生
        sample->Release();
        MyDbgLog((LOG_ERROR, kDbgImportant,
                  TEXT("SCFFOutputPin: Error %08lX from FillBuffer!!!"),
                  result_fill_buffer));
        DeliverEndOfStream();
        m_pFilter->NotifyEvent(EC_ERRORABORT, result_fill_buffer, 0);
        return result_fill_buffer;
      }

      // Clockタイムマネージャを使っている場合はSleepをはさむ
      if (!(is_first_loop || can_use_quality)) {
        clock_time_.Sleep();
      }

      is_first_loop = false;

      /// @attention sampleが絶対にリリースされてないと駄目！
    }

    // すべてのコマンドに対応
    if (command == CMD_RUN || command == CMD_PAUSE) {
      Reply(S_OK);
    } else if (command != CMD_STOP) {
      Reply((DWORD) E_UNEXPECTED);
      MyDbgLog((LOG_ERROR, kDbgImportant,
                TEXT("SCFFOutputPin: Unexpected command!!!")));
    }
  } while (command != CMD_STOP);

  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: DoBufferProcessingLoop ends")));

  return S_FALSE;
}

// 取得した空のメディア サンプルにデータを挿入
//
/// @pre GetMediaTypeによってm_mtには適切なフォーマットが格納済み
/// @retval S_OK
/// @retval S_FALSE ストリーム終了
HRESULT SCFFOutputPin::FillBuffer(IMediaSample *sample) {
  // S_FALSE(ストリーム終了)になることはない
  return S_OK;
}

// 取得した空のメディア サンプルにデータを挿入
/// @retval S_OK
/// @retval S_FALSE ストリーム終了
HRESULT SCFFOutputPin::FillBufferWithImagingEngine(
    imaging::Engine &engine,
    IMediaSample *sample) {
  CheckPointer(sample, E_POINTER);

  // m_mtをチェック
  ASSERT(m_mt.formattype == FORMAT_VideoInfo);
  VIDEOINFOHEADER *video_info =
    reinterpret_cast<VIDEOINFOHEADER*>(m_mt.pbFormat);

  // サンプルのサイズを設定してポインタを取得
  BYTE *data = NULL;
  DWORD data_size = 0;
  sample->GetPointer(&data);
  data_size = sample->GetSize();
  sample->SetActualDataLength(data_size);

  // sampleにデータを書き込み
  engine.PullFrontImage(data, data_size);

  /// @attention SetTimeおよびSetSyncは外部で行っている

  return S_OK;
}
