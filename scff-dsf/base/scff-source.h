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

/// @file base/scff-source.h
/// SCFFSourceの宣言

#ifndef SCFF_DSF_BASE_SCFF_SOURCE_H_
#define SCFF_DSF_BASE_SCFF_SOURCE_H_

#include <streams.h>

/// DirectShowビデオキャプチャフィルタ
class SCFFSource : public CSource {
 public:
  /// DLL読み込み中に呼ばれるインスタンス生成関数
  /// @sa g_Templates(strmbase.lib)
  static CUnknown* WINAPI CreateInstance(IUnknown *unknown, HRESULT *result);

  /// DLL読み込み中に呼ばれる初期化関数
  /// @sa g_Templates(strmbase.lib)
  static void WINAPI Init(BOOL loading, const CLSID *clsid);

  /// m_tStart.m_timeへのアクセッサ
  REFERENCE_TIME GetStartTime() {
    return m_tStart.m_time;
  }

 private:
  /// コンストラクタ
  SCFFSource(IUnknown *unknown, HRESULT *result);
  /// デストラクタ
  ~SCFFSource();
};

#endif  // SCFF_DSF_BASE_SCFF_SOURCE_H_
