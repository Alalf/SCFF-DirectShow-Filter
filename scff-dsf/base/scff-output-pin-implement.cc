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

/// @file base/scff-output-pin-implement.cc
/// @brief SCFFOutputPinの実装(Interfaceのみ)

#include "base/scff-output-pin.h"

#include "base/debug.h"
#include "base/constants.h"

//=====================================================================
// 各種インターフェースの実装
//=====================================================================

//---------------------------------------------------------------------
// IQualityControl
//---------------------------------------------------------------------

// 品質の変更が要求されたことをフィルタに通知
/// @retval E_NOTIMPL
STDMETHODIMP SCFFOutputPin::Notify(IBaseFilter *self, Quality quality) {
  /// @attention Notifyは別スレッドから呼ばれることを確認
  return E_NOTIMPL;
}

//---------------------------------------------------------------------
// IAMStreamConfig
//---------------------------------------------------------------------

// 優先出力フォーマットを取得
// CoTaskMemAllocではプロセス単位の既定ヒープを使用してメモリを確保する
// COM内ではmallocよりもCoTaskMemAllocを使うべきである
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval VFW_S_NO_MORE_ITEMS
/// @retval E_OUTOFMEMORY
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::GetPreferredFormat(int position,
                                 AM_MEDIA_TYPE **media_type) {
  CheckPointer(media_type, E_POINTER);

  CMediaType media_type_from_pin;
  HRESULT result = GetMediaType(position, &media_type_from_pin);
  if (result != S_OK) return result;

  // ここで*media_typeに確保したメモリは呼び出し元が開放してくれる
  *media_type = static_cast<AM_MEDIA_TYPE*>(
                  CoTaskMemAlloc(sizeof(AM_MEDIA_TYPE)));

  // CMediaTypeのインスタンスは代入でAM_MEDIA_TYPEへコピー可能
  **media_type = media_type_from_pin;

  // pbFormatはポインタなので新しく生成して内容をコピーしておく
  (**media_type).pbFormat = static_cast<BYTE*>(
                  CoTaskMemAlloc((**media_type).cbFormat));
  memcpy((**media_type).pbFormat, media_type_from_pin.Format(),
          (**media_type).cbFormat);

  return S_OK;
}

// 現在の出力フォーマットまたは優先出力フォーマットを取得
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval VFW_S_NO_MORE_ITEMS
/// @retval E_OUTOFMEMORY
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::GetFormat(AM_MEDIA_TYPE **media_type) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("[pin]<IAMStreamConfig> -> format")));
  return GetPreferredFormat(0, media_type);
}

// このピンがサポートするフォーマット機能の数を取得
/// @retval S_OK
/// @retval E_POINTER
STDMETHODIMP SCFFOutputPin::GetNumberOfCapabilities(
                              int *count, int *size) {
  CheckPointer(size, E_POINTER);

  *count = kSupportedFormatsCount;
  *size = sizeof(VIDEO_STREAM_CONFIG_CAPS);

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("[pin]<IAMStreamConfig> -> num of caps(%d)"),
            *count));
  return S_OK;
}

// フォーマット機能のセットを取得
/// @retval E_POINTER
/// @retval E_INVALIDARG
/// @retval E_OUTOFMEMORY
/// @retval S_FALSE 指定されたインデックスの値が大きすぎる
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::GetStreamCaps(
                              int position,
                              AM_MEDIA_TYPE **media_type,
                              BYTE *stream_caps) {
  CheckPointer(media_type, E_POINTER);
  CheckPointer(stream_caps, E_POINTER);

  if (position < 0) return E_INVALIDARG;

  HRESULT result = GetPreferredFormat(position, media_type);
  if (result == VFW_S_NO_MORE_ITEMS) return S_FALSE;
  if (result != S_OK) return result;

  ASSERT((**media_type).formattype == FORMAT_VideoInfo);

  VIDEO_STREAM_CONFIG_CAPS *config_caps =
    reinterpret_cast<VIDEO_STREAM_CONFIG_CAPS*>(stream_caps);
  VIDEOINFO *video_info =
    reinterpret_cast<VIDEOINFO*>((**media_type).pbFormat);

  ZeroMemory(config_caps, sizeof(VIDEO_STREAM_CONFIG_CAPS));
  config_caps->VideoStandard        = 0;
  config_caps->guid                 = FORMAT_VideoInfo;
  config_caps->CropAlignX           = 1;
  config_caps->CropAlignY           = 1;
  config_caps->CropGranularityX     = 1;
  config_caps->CropGranularityY     = 1;
  config_caps->OutputGranularityX   = 1;
  config_caps->OutputGranularityY   = 1;
  config_caps->InputSize.cx         = video_info->bmiHeader.biWidth;
  config_caps->InputSize.cy         = abs(video_info->bmiHeader.biHeight);
  config_caps->MinOutputSize.cx     = kMinOutputWidth;
  config_caps->MinOutputSize.cy     = kMinOutputHeight;
  config_caps->MaxOutputSize.cx     = kMaxOutputWidth;
  config_caps->MaxOutputSize.cy     = kMaxOutputHeight;
  config_caps->MinCroppingSize      = config_caps->InputSize;
  config_caps->MaxCroppingSize      = config_caps->InputSize;
  config_caps->MinBitsPerSecond     = 1;
  config_caps->MaxBitsPerSecond     = 1;
  config_caps->MinFrameInterval     = kMinFrameInterval;
  config_caps->MaxFrameInterval     = kMaxFrameInterval;
  config_caps->StretchTapsX         = 0;
  config_caps->StretchTapsY         = 0;
  config_caps->ShrinkTapsX          = 0;
  config_caps->ShrinkTapsY          = 0;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("[pin]<IAMStreamConfig> -> stream caps(%d)"),
            position));
  return S_OK;
}

// ピン上で出力フォーマットを設定
/// @retval E_POINTER
/// @retval E_UNEXPECTED
/// @retval E_INVALIDARG
/// @retval E_UNEXPECTED
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::SetFormat(
                              AM_MEDIA_TYPE *media_type) {
  CheckPointer(media_type, E_POINTER);
  // コピーコンストラクタを利用してコピー
  CMediaType media_type_instance = *media_type;

  HRESULT result = SetMediaType(&media_type_instance);
  if (result != S_OK) return result;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("[pin]<IAMStreamConfig> <- format")));
  return S_OK;
}

//---------------------------------------------------------------------
// IKsPropertySet
//---------------------------------------------------------------------

// プロパティ セットのプロパティの値を設定
/// @retval E_NOTIMPL
STDMETHODIMP SCFFOutputPin::Set(REFGUID property_set_guid, DWORD property_id,
                              LPVOID instance_data, DWORD instance_data_size,
                              LPVOID property_data, DWORD property_data_size) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: Set")));
  return E_NOTIMPL;
}

// プロパティ セットの項目に対するデータを取得
/// @retval E_PROP_SET_UNSUPPORTED
/// @retval E_PROP_ID_UNSUPPORTED
/// @retval E_POINTER
/// @retval E_UNEXPECTED
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::Get(REFGUID property_set_guid, DWORD property_id,
                              LPVOID instance_data, DWORD instance_data_size,
                              LPVOID property_data, DWORD property_data_size,
                              DWORD *returned_data_size) {
  if (property_set_guid != AMPROPSETID_Pin) return E_PROP_SET_UNSUPPORTED;
  if (property_id != AMPROPERTY_PIN_CATEGORY) return E_PROP_ID_UNSUPPORTED;
  CheckPointer(property_data, E_POINTER);
  CheckPointer(returned_data_size, E_POINTER);

  if (returned_data_size) *returned_data_size = sizeof(GUID);

  // 呼び出し元はサイズだけ知りたい。
  CheckPointer(property_data, S_OK);

  // バッファが小さすぎる。
  if (property_data_size < sizeof(GUID)) return E_UNEXPECTED;

  // このPinはPIN_CATEGORY_CAPTUREである
  *(reinterpret_cast<GUID*>(property_data)) = PIN_CATEGORY_CAPTURE;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: Get")));

  return S_OK;
}

// プロパティ セットのプロパティが、
// ポートまたはデバイスでサポートされているかどうかを確認
/// @retval E_PROP_SET_UNSUPPORTED
/// @retval E_PROP_ID_UNSUPPORTED
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::QuerySupported(REFGUID property_set_guid,
                              DWORD property_id, DWORD *support_type) {
  if (property_set_guid != AMPROPSETID_Pin) return E_PROP_SET_UNSUPPORTED;
  if (property_id != AMPROPERTY_PIN_CATEGORY) return E_PROP_ID_UNSUPPORTED;

  // このプロパティの取得はサポートしているが、設定はサポートしていない
  if (support_type) *support_type = KSPROPERTY_SUPPORT_GET;
  return S_OK;
}

//---------------------------------------------------------------------
// IAMLatency
//---------------------------------------------------------------------

// フィルタに関連付けられている、予期される遅延時間を取得
/// @retval S_OK
/// @retval E_POINTER
STDMETHODIMP SCFFOutputPin::GetLatency(REFERENCE_TIME *latency) {
  CheckPointer(latency, E_POINTER);

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: GetLatency")));

  /// @todo(me) 値が不正確。固定値ではないはず。
  if (fps_ > 0.0) {
    *latency = ToFrameInterval(fps_);
  } else {
    *latency = 0;
  }

  return S_OK;
}

//---------------------------------------------------------------------
// IAMPushSource
//---------------------------------------------------------------------

// フィルタの動作を表すフラグの組み合わせを取得
/// @retval E_POINTER
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::GetPushSourceFlags(ULONG *flags) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: GetPushSourceFlags")));

  CheckPointer(flags, E_POINTER);
  *flags = 0;   // 設定なし = ライブソース
  return S_OK;
}

// フィルタの動作を指定するフラグを設定
/// @retval E_NOTIMPL
STDMETHODIMP SCFFOutputPin::SetPushSourceFlags(ULONG flags) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: SetPushSourceFlags")));

  return E_NOTIMPL;
}

// フィルタがサポートできる最大ストリーム オフセットを取得
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::GetMaxStreamOffset(REFERENCE_TIME *max_offset) {
  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: GetMaxStreamOffset")));

  CheckPointer(max_offset, E_POINTER);
  // ダミーとして非常に大きな値を設定(１年)
  *max_offset = UNITS * (60 * 60 * 24 * 365);
  return S_OK;
}

// 最大ストリーム オフセットを指定する基準タイム
/// @retval E_NOTIMPL
STDMETHODIMP SCFFOutputPin::SetMaxStreamOffset(REFERENCE_TIME max_offset) {
  // max_offsetは設定できない
  /// @warning SCFH DSFではS_OKを返していた。
  //return E_NOTIMPL;

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: SetMaxStreamOffset")));

  return S_OK;
}

// タイム スタンプを生成するときにフィルタが使うオフセットを取得
/// @retval S_OK
/// @retval E_POINTER
STDMETHODIMP SCFFOutputPin::GetStreamOffset(REFERENCE_TIME *offset) {
  CheckPointer(offset, E_POINTER);

  MyDbgLog((LOG_TRACE, kDbgImportant,
            TEXT("SCFFOutputPin: GetStreamOffset")));

  *offset = offset_;
  return S_OK;
}

// このフィルタで生成されるタイム スタンプのオフセットを設定
/// @retval S_OK
STDMETHODIMP SCFFOutputPin::SetStreamOffset(REFERENCE_TIME offset) {
  MyDbgLog((LOG_TRACE, kDbgTrace,
            TEXT("SCFFOutputPin: SetStreamOffset(%llu)"), offset));
  offset_ = offset;
  return S_OK;
}
