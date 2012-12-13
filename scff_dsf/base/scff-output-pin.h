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

/// @file base/scff-output-pin.h
/// SCFFOutputPinの宣言

#ifndef SCFF_DSF_BASE_SCFF_OUTPUT_PIN_H_
#define SCFF_DSF_BASE_SCFF_OUTPUT_PIN_H_

#include <streams.h>
#include "base/scff-clock-time.h"
#include "scff-imaging/imaging.h"

/// DirectShowビデオキャプチャフィルタの出力ピン
class SCFFOutputPin : public CSourceStream,
                      public IKsPropertySet,
                      public IAMStreamConfig,
                      public IAMPushSource {
 public:
  /// コンストラクタ
  SCFFOutputPin(HRESULT *result, CSource *source);
  /// デストラクタ
  ~SCFFOutputPin();

  //-----------------------------------------------------------------
  /// インデックス値で優先メディア タイプを取得
  /// @sa CBasePin::GetMediaType
  HRESULT GetMediaType(int position, CMediaType *media_type);
  /// ピンが特定のメディア タイプを受け入れるかどうかを判定
  /// @sa CBasePin::CheckMediaType
  HRESULT CheckMediaType(const CMediaType *media_type);
  /// 接続のメディア タイプを設定
  /// @sa CBasePin::SetMediaType
  HRESULT SetMediaType(const CMediaType *media_type);

  //-----------------------------------------------------------------
  /// バッファ要求を設定
  /// @sa CBaseOutputPin::DecideBufferSize
  HRESULT DecideBufferSize(IMemAllocator *allocator,
                            ALLOCATOR_PROPERTIES *request);

  //-----------------------------------------------------------------
  /// 品質の変更が要求されたことをフィルタに通知
  /// @sa IQualityControl::Notify
  STDMETHODIMP Notify(IBaseFilter *self, Quality quality);

  //-----------------------------------------------------------------
  /// IUnknownの実装
  /// @sa IUnknown::QueryInterface, IUnknown::AddRef, IUnknown::Release
  DECLARE_IUNKNOWN;
  /// IUnknownの実装
  /// @sa IUnknown::NonDelegatingQueryInterface
  STDMETHODIMP NonDelegatingQueryInterface(REFIID id, void **self) {
    if (id == IID_IKsPropertySet) {
      return GetInterface(static_cast<IKsPropertySet*>(this), self);
    }
    if (id == IID_IAMStreamConfig) {
      return GetInterface(static_cast<IAMStreamConfig*>(this), self);
    }
    if (id == IID_IAMPushSource) {
      return GetInterface(static_cast<IAMPushSource*>(this), self);
    }
    return CSourceStream::NonDelegatingQueryInterface(id, self);
  }

  //-----------------------------------------------------------------
  /// 現在の出力フォーマットまたは優先出力フォーマットを取得
  /// @sa IAMStreamConfig::GetFormat
  STDMETHODIMP GetFormat(AM_MEDIA_TYPE **media_type);
  /// このピンがサポートするフォーマット機能の数を取得
  /// @sa IAMStreamConfig::GetNumberOfCapabilities
  STDMETHODIMP GetNumberOfCapabilities(int *count, int *size);
  /// フォーマット機能のセットを取得
  /// @sa IAMStreamConfig::GetStreamCaps
  STDMETHODIMP GetStreamCaps(int position, AM_MEDIA_TYPE **media_type,
                             BYTE *stream_caps);
  /// ピン上で出力フォーマットを設定
  /// @sa IAMStreamConfig::SetFormat
  STDMETHODIMP SetFormat(AM_MEDIA_TYPE *media_type);

  //-----------------------------------------------------------------
  /// プロパティ セットのプロパティの値を設定
  /// @sa IKsPropertySet::Set
  STDMETHODIMP Set(REFGUID property_set_guid, DWORD property_id,
                   LPVOID instance_data, DWORD instance_data_size,
                   LPVOID property_data, DWORD property_data_size);
  /// プロパティ セットの項目に対するデータを取得
  /// @sa IKsPropertySet::Get
  STDMETHODIMP Get(REFGUID property_set_guid, DWORD property_id,
                   LPVOID instance_data, DWORD instance_data_size,
                   LPVOID property_data, DWORD property_data_size,
                   DWORD *returned_data_size);
  /// プロパティ セットのプロパティが、
  /// ポートまたはデバイスでサポートされているかどうかを確認
  /// @sa IKsPropertySet::QuerySupport
  STDMETHODIMP QuerySupported(REFGUID property_set_guid,
                              DWORD property_id,
                              DWORD *support_type);

  //-----------------------------------------------------------------
  /// フィルタに関連付けられている、予期される遅延時間を取得
  /// @sa IAMLatency::GetLatency
  STDMETHODIMP GetLatency(REFERENCE_TIME *latency);

  //-----------------------------------------------------------------
  /// フィルタの動作を表すフラグの組み合わせを取得
  /// @sa IAMPushSource::GetPushSourceFlags
  STDMETHODIMP GetPushSourceFlags(ULONG *flags);
  /// フィルタの動作を指定するフラグを設定
  /// @sa IAMPushSource::SetPushSourceFlags
  STDMETHODIMP SetPushSourceFlags(ULONG flags);
  /// フィルタがサポートできる最大ストリーム オフセットを取得
  /// @sa IAMPushSource::GetMaxStreamOffset
  STDMETHODIMP GetMaxStreamOffset(REFERENCE_TIME *max_offset);
  /// 最大ストリーム オフセットを指定する基準タイム
  /// @sa IAMPushSource::SetMaxStreamOffset
  STDMETHODIMP SetMaxStreamOffset(REFERENCE_TIME max_offset);
  /// タイム スタンプを生成するときにフィルタが使うオフセットを取得
  /// @sa IAMPushSource::GetStreamOffset
  STDMETHODIMP GetStreamOffset(REFERENCE_TIME *offset);
  /// このフィルタで生成されるタイム スタンプのオフセットを設定
  /// @sa IAMPushSource::SetStreamOffset
  STDMETHODIMP SetStreamOffset(REFERENCE_TIME offset);

  //-----------------------------------------------------------------
  /// ストリーミング スレッドが初期化されたときに呼び出される
  /// @sa CSourceStream::OnThreadCreate
  HRESULT OnThreadCreate(void);
  /// ストリーミング スレッドが間もなく終了するときに呼び出される
  /// @sa CSourceStream::OnThreadDestroy
  HRESULT OnThreadDestroy(void);
  /// CSourceStream::DoBufferProcessingLoop メソッドの
  /// 処理が開始されたときに呼び出される
  /// @sa CSourceStream::OnThreadStartPlay
  HRESULT OnThreadStartPlay(void);
  /// メディア データを生成し、ダウンストリームの入力ピンに提供
  /// @sa CSourceStream::DoBufferProcessingLoop
  HRESULT DoBufferProcessingLoop(void);
  /// 取得した空のメディア サンプルにデータを挿入
  /// @warning 使わないので空でオーバーライドしてる
  /// @sa CSourceStream::FillBuffer
  HRESULT FillBuffer(IMediaSample *sample);

 private:
  /// 取得した空のメディア サンプルにデータを挿入
  /// （データの作成はすべてimaging::Engineに委譲）
  /// @sa CSourceStream::FillBuffer
  HRESULT FillBufferWithImagingEngine(
      scff_imaging::Engine &engine,
      IMediaSample *sample);

  /// 優先出力フォーマットを取得
  /// @sa GetFormat
  STDMETHODIMP GetPreferredFormat(int position, AM_MEDIA_TYPE **media_type);

  /// fpsから適切なバッファの数を求める
  /// @attention バッファの数はSCFH DSFに準拠
  int CalcBufferCount();

  /// frame_intervalからfpsを求める
  double ToFPS(const REFERENCE_TIME frame_interval) {
    ASSERT(frame_interval != 0LL);
    return static_cast<double>(UNITS) / frame_interval;
  }

  /// fpsからframe_intervalを求める
  REFERENCE_TIME ToFrameInterval(const double fps) {
    ASSERT(fps != 0.0);
    return static_cast<REFERENCE_TIME>(UNITS / fps_);
  }

  // 画面解像度,FPS,ピクセルフォーマット
  /// width(下流フィルタの要求から決まる)
  int width_;
  /// height(下流フィルタの要求から決まる)
  int height_;
  /// fps(下流フィルタの要求から決まる)
  double fps_;
  /// pixel_format(下流フィルタの要求から決まる)
  scff_imaging::ImagePixelFormat pixel_format_;

  /// 単純にSleepするだけの原始的なタイムマネージャ
  SCFFClockTime clock_time_;

  /// このフィルタで生成されるタイム スタンプのオフセット
  /// @sa IAMPushSource::SetStreamOffset, IAMPushSource::GetStreamOffset
  REFERENCE_TIME offset_;

  // タイムマネージャのロックに必要
  /// FillBufferの排他制御用
  CCritSec filling_buffer_;
};

#endif  // SCFF_DSF_BASE_SCFF_OUTPUT_PIN_H_
