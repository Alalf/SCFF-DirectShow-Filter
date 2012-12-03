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
/// SCFFOutputPinの実装(基底クラスのみ)

#include "base/scff-output-pin.h"

#include <math.h>

#include <algorithm>

#include "base/constants.h"
#include "base/debug.h"
#include "base/scff-source.h"
#include "base/scff-monitor.h"

//=====================================================================
// SCFFOutputPin
//=====================================================================

SCFFOutputPin::SCFFOutputPin(HRESULT *result, CSource *source)
  : CSourceStream(kFilterName, result, source, L"Capture"),
    width_(kPreferredSizes[1].cx),    // 0はダミーなので1
    height_(kPreferredSizes[1].cy),   // 0はダミーなので1
    fps_(kDefaultFPS),
    pixel_format_(scff_imaging::ImagePixelFormat::kI420),
    offset_(0) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete,
    TEXT("SCFFOutputPin: NEW(%d, %d, %.1ffps)"),
    width_, height_, fps_));
}

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

/// - 以後ピンに渡されるmedia_typeは必ず以下になることが保障される:
///   - biCompression:  I420/IYUV/YV12/UYVY/YUY2/RGB0
///   - biBitCount:     12/16/32
///   - biWidth:        width_
///   - biHeight:       height_
///   - biSizeImage:    CalculateDataSize(*)
///   - biPlanes:       1
///   - FPS(AvgTimePerFrame): 1/fps_
///   - Type:           MEDIATYPE_Video
///   - FormatType:     FORMAT_VideoInfo
///   - SampleSize:     (== biSizeImage)
///
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval VFW_S_NO_MORE_ITEMS
/// @retval E_OUTOFMEMORY
/// @retval S_OK
HRESULT SCFFOutputPin::GetMediaType(int position, CMediaType *media_type) {
  // 引数チェック
  CheckPointer(media_type, E_POINTER);
  if (position < 0) {
    return E_INVALIDARG;
  }
  if (position >= kSupportedFormatsCount) {
    return VFW_S_NO_MORE_ITEMS;
  }

  // ロック: m_pFilter->pStateLock()
  CAutoLock lock(m_pFilter->pStateLock());

  // ビデオ設定を表すVIDEOINFOの作成
  VIDEOINFO *video_info = reinterpret_cast<VIDEOINFO *>
                          (media_type->AllocFormatBuffer(sizeof(VIDEOINFO)));
  CheckPointer(video_info, E_OUTOFMEMORY);
  ZeroMemory(video_info, sizeof(VIDEOINFO));

  //-------------------------------------------------------------------
  // １フレームの設定を表すBITMAPINFOHEADERの設定

  scff_imaging::ImagePixelFormat current_pixel_format;
  int current_width;
  int current_height;
  if (position == 0) {
    // ピクセルフォーマット指定付き
    // positionからピクセルフォーマットを計算
#if defined(FOR_KOTOENCODER)
    current_pixel_format = scff_imaging::kI420;
#else
    current_pixel_format = pixel_format_;
#endif
    current_width = width_;
    current_height = height_;
  } else {
    const int fixed_position = position - 1;

    // サイズおよびピクセルフォーマット設定用
    const int position_in_preferred_sizes =
        fixed_position % kPreferredSizesCount;
    const int position_in_pixel_formats =
        fixed_position / kPreferredSizesCount;

    // positionからピクセルフォーマットを計算
#if defined(FOR_KOTOENCODER)
    current_pixel_format = scff_imaging::kI420;
#else
    current_pixel_format =
        scff_imaging::Utilities::IndexToPixelFormat(position_in_pixel_formats);
#endif
    // サイズ設定の優先度は以下のとおり:
    //  0.  SetMediaTypeでピン接続先から指定されたサイズ
    //  1-. 優先サイズの配列順
    if (position_in_preferred_sizes == 0) {
      // 0. SetMediaTypeでピン接続先から指定されたサイズ
      current_width = width_;
      current_height = height_;
    } else {
      // 1-. 優先サイズの配列順
      current_width = kPreferredSizes[position_in_preferred_sizes].cx;
      current_height = kPreferredSizes[position_in_preferred_sizes].cy;
    }
  }

  // ピクセルフォーマットからBITMAPINFOを取得
  BITMAPINFO current_info;
  scff_imaging::Utilities::ToWindowsBitmapInfo(current_pixel_format,
                                               current_width,
                                               current_height,
                                               false,
                                               &current_info);

  // 現在すべての形式でカラーテーブルは必要ないのでbmiHeaderのみコピー
  memcpy(&(video_info->bmiHeader),
         &(current_info.bmiHeader),
         sizeof(BITMAPINFOHEADER));

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

/// - さまざまなアプリケーションがこのメソッドを利用しているので
///   優先メディアタイプが一つであっても実装したほうがよさそうだ
///
/// @todo(me) ワイルドカード的なことを処理する場合はS_FALSE
/// @code
/// // We treat MEDIASUBTYPE_NULL subtype as a wild card
/// if((m_pReader->LoadType()->majortype == pType->majortype) &&
///    (m_pReader->LoadType()->subtype == MEDIASUBTYPE_NULL   ||
///    m_pReader->LoadType()->subtype == pType->subtype))
/// {
///   return S_OK;
/// }
/// return S_FALSE;
/// @endcode
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
  const GUID IYUV_guid = MEDIASUBTYPE_IYUV;
  const GUID YV12_guid = MEDIASUBTYPE_YV12;
  const GUID UYVY_guid = MEDIASUBTYPE_UYVY;
  const GUID YUY2_guid = MEDIASUBTYPE_YUY2;
  const GUID RGB0_guid = MEDIASUBTYPE_RGB32;

  if (subtype != I420_guid &&
      subtype != IYUV_guid &&
      subtype != YV12_guid &&
      subtype != UYVY_guid &&
      subtype != YUY2_guid &&
      subtype != RGB0_guid) {
    return E_INVALIDARG;
  }

  // 以下VIDEOINFOのチェック
  VIDEOINFO *video_info =
    reinterpret_cast<VIDEOINFO*>(media_type->Format());
  CheckPointer(video_info, E_UNEXPECTED);

  // サイズとFPSの設定が正しくされているか？
  if (video_info->bmiHeader.biWidth == 0 ||
      video_info->bmiHeader.biHeight <= 0 ||
      video_info->AvgTimePerFrame == 0) {
    return E_INVALIDARG;
  }

  /// @todo(me) Skypeに対応する場合はサイズ(0,0)を許さなければならない
  // if (video_info->bmiHeader.biWidth == 0 &&
  //     video_info->bmiHeader.biHeight == 0) {
  //   // nop
  // } else if (video_info->bmiHeader.biWidth == 0 ||
  //            video_info->bmiHeader.biHeight <= 0 ||
  //            video_info->AvgTimePerFrame == 0) {
  //   // Skypeでなければきっちりエラー処理する
  //   return E_INVALIDARG;
  // }

  MyDbgLog((LOG_TRACE, kDbgTrace,
    TEXT("[pin] <- check: mediatype(%dbpp, %d, %d, %.1ffps)"),
    video_info->bmiHeader.biBitCount,
    video_info->bmiHeader.biWidth,
    video_info->bmiHeader.biHeight,
    ToFPS(video_info->AvgTimePerFrame)));
  return S_OK;
}

/// - ここではじめて下位のフィルタが要求したフレームの画像サイズを取得できる
///
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
    TEXT("[pin] <- mediatype(%dbpp, %d, %d, %.1ffps)"),
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
  if (FAILED(result)) return result;

  /// @todo(me) Skype対応
  // if (video_info->bmiHeader.biWidth == 0 &&
  //     video_info->bmiHeader.biHeight == 0) {
  //   // 書き換えるのはまずい気がするのでコピーを行う
  //   CMediaType media_type_from_pin(*media_type);
  //   VIDEOINFO *video_info_from_pin =
  //     reinterpret_cast<VIDEOINFO*>(media_type_from_pin.Format());
  //   CheckPointer(video_info, E_UNEXPECTED);
  //
  //   video_info_from_pin->bmiHeader.biWidth = 256;
  //   video_info_from_pin->bmiHeader.biHeight = 256;
  //   video_info_from_pin->AvgTimePerFrame = ToFrameInterval(30.0);
  //
  //   // m_mtに設定
  //   HRESULT result = CSourceStream::SetMediaType(&media_type_from_pin);
  //   if (FAILED(result)) return result;
  //
  // } else if (video_info->bmiHeader.biWidth == 0 ||
  //            video_info->bmiHeader.biHeight == 0 ||
  //            video_info->AvgTimePerFrame == 0) {
  //   // Skypeでなければきっちりエラー処理する
  //   return E_INVALIDARG;
  //
  // } else {
  //   // m_mtに設定
  //   HRESULT result = CSourceStream::SetMediaType(media_type);
  //   if (FAILED(result)) return result;
  // }

  // 新しい設定をチェック
  VIDEOINFO *specified_video_info =
    reinterpret_cast<VIDEOINFO*>(m_mt.Format());
  CheckPointer(specified_video_info, E_UNEXPECTED);

  if (specified_video_info->AvgTimePerFrame <= 0) {
    return E_INVALIDARG;
  }

  // 対応形式であるか確認
  if (!scff_imaging::Utilities::IsSupportedPixelFormat(
          specified_video_info->bmiHeader)) {
    return E_INVALIDARG;
  }

  // ここで実際の値を取得
  width_  = specified_video_info->bmiHeader.biWidth;
  height_ = abs(specified_video_info->bmiHeader.biHeight);
  fps_    = ToFPS(specified_video_info->AvgTimePerFrame);

  const GUID subtype = *(m_mt.Subtype());
  const GUID I420_guid = FOURCCMap(MAKEFOURCC('I', '4', '2', '0'));
  const GUID IYUV_guid = MEDIASUBTYPE_IYUV;
  const GUID YV12_guid = MEDIASUBTYPE_YV12;
  const GUID UYVY_guid = MEDIASUBTYPE_UYVY;
  const GUID YUY2_guid = MEDIASUBTYPE_YUY2;
  const GUID RGB0_guid = MEDIASUBTYPE_RGB32;
  if (subtype == I420_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kI420;
  } else if (subtype == IYUV_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kIYUV;
  } else if (subtype == YV12_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kYV12;
  } else if (subtype == UYVY_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kUYVY;
  } else if (subtype == YUY2_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kYUY2;
  } else if (subtype == RGB0_guid) {
    pixel_format_ = scff_imaging::ImagePixelFormat::kRGB0;
  } else {
    ASSERT(false);
    pixel_format_ = scff_imaging::ImagePixelFormat::kI420;
  }

  return S_OK;
}

//---------------------------------------------------------------------
// CBaseOutputPin
//---------------------------------------------------------------------

/// @attention バッファの数はSCFH DSFに準拠
int SCFFOutputPin::CalcBufferCount() {
  return max(static_cast<int>(ceil(fps_ * 0.3)), 4);
}

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
  if (FAILED(result)) return result;

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

/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadCreate(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadCreate")));
  // スレッド終了させるにはE_を返す
  return S_OK;
}

/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadDestroy(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadDestroy")));
  // スレッド終了させるにはE_を返す
  return S_OK;
}

/// @retval S_OK
HRESULT SCFFOutputPin::OnThreadStartPlay(void) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: OnThreadStartPlay")));
  CAutoLock lock(&filling_buffer_);
  // タイムマネージャをリセット
  clock_time_.Reset(fps_, m_pFilter);
  return S_OK;
}

/// - デフォルトの動作だとまったく待たずに最速でFillBufferを埋めてしまうので
///   適切なWaitをはさむ様に置き換えている。
/// - また、ワーカースレッドを作成しFillBufferのためのデータをバックグラウンドで
///   生成するようにしている。
///
/// @retval S_OK
/// @retval S_FALSE ループ正常終了
HRESULT SCFFOutputPin::DoBufferProcessingLoop(void) {
  Command command;

  MyDbgLog((LOG_TRACE, kDbgImportant,
    TEXT("SCFFOutputPin: DoBufferProcessingLoop starts")));

  OnThreadStartPlay();

  //-------------------------------------------------------------------
  // scff_imaging::Engineを作成
  //-------------------------------------------------------------------

  // m_mtからVIDEOINFOHEADERを取得
  ASSERT(m_mt.formattype == FORMAT_VideoInfo);
  VIDEOINFOHEADER *video_info =
    reinterpret_cast<VIDEOINFOHEADER*>(m_mt.pbFormat);

  // BITMAPINFOHEADERからピクセルフォーマットを取得
  scff_imaging::ImagePixelFormat pixel_format =
      scff_imaging::Utilities::WindowsBitmapInfoHeaderToPixelFormat(
          video_info->bmiHeader);

  // Engineを作成
  scff_imaging::Engine engine(pixel_format, width_, height_, fps_);
  const scff_imaging::ErrorCode error = engine.Init();
  ASSERT(error == scff_imaging::ErrorCode::kNoError);

  //-------------------------------------------------------------------

  // SCFFMonitorとリクエスト格納用変数を作成
  SCFFMonitor monitor;
  monitor.Init(pixel_format, width_, height_, fps_);
  scff_imaging::Request *request = nullptr;

  // ループ開始
  do {
    while (!CheckRequest(&command)) {
      /// @warning ダウンキャスト: dynamic_castではなくstatic_castで行っている
      REFERENCE_TIME filter_zero =
          static_cast<SCFFSource*>(m_pFilter)->GetStartTime();

      // 接続先のピンからバッファを受け取る
      IMediaSample *sample;
      HRESULT result = GetDeliveryBuffer(&sample, nullptr, nullptr, 0);
      if (FAILED(result)) {
        ::Sleep(1);   // 分解能1mSec
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

        // ClockTime(以下ct)を更新
        REFERENCE_TIME start_for_ct;
        REFERENCE_TIME end_for_ct;
        clock_time_.GetTimestamp(filter_zero,
                                 &start_for_ct, &end_for_ct);

        // サンプルにデータを詰める (FillBuffer()は使わない)
        result_fill_buffer = FillBufferWithImagingEngine(engine, sample);

        // タイムスタンプ設定
        sample->SetTime(&start_for_ct, &end_for_ct);
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
      clock_time_.Sleep(filter_zero);

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

/// @pre GetMediaTypeによってm_mtには適切なフォーマットが格納済み
/// @retval S_OK
/// @retval S_FALSE ストリーム終了
HRESULT SCFFOutputPin::FillBuffer(IMediaSample *sample) {
  // S_FALSE(ストリーム終了)になることはない
  return S_OK;
}

/// @retval S_OK
/// @retval S_FALSE ストリーム終了
HRESULT SCFFOutputPin::FillBufferWithImagingEngine(
    scff_imaging::Engine &engine,
    IMediaSample *sample) {
  CheckPointer(sample, E_POINTER);

  // m_mtをチェック
  ASSERT(m_mt.formattype == FORMAT_VideoInfo);
  VIDEOINFOHEADER *video_info =
    reinterpret_cast<VIDEOINFOHEADER*>(m_mt.pbFormat);

  // サンプルのサイズを設定してポインタを取得
  BYTE *data = nullptr;
  DWORD data_size = 0;
  sample->GetPointer(&data);
  data_size = sample->GetSize();
  sample->SetActualDataLength(data_size);

  // sampleにデータを書き込み
  engine.CopyFrontImage(data, data_size);

  /// @attention SetTimeおよびSetSyncは外部で行っている

  return S_OK;
}
