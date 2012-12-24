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

/// @file base/scff_source.cc
/// SCFFSourceの定義

#include "base/scff_source.h"

#include "base/constants.h"
#include "base/debug.h"
#include "base/scff_output_pin.h"

//=====================================================================
// SCFFSource
//=====================================================================

SCFFSource::SCFFSource(IUnknown *unknown, HRESULT *result)
  : CSource(kFilterName, unknown, CLSID_SCFFSource) {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("SCFFSource: NEW")));
  // newするだけでAddPinされる
  new SCFFOutputPin(result, this);
  ASSERT(GetPinCount() == 1);
}

SCFFSource::~SCFFSource() {
  MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("SCFFSource: DELETE")));
}

CUnknown* WINAPI SCFFSource::CreateInstance(IUnknown *unknown,
                                            HRESULT *result) {
  SCFFSource *source = new SCFFSource(unknown, result);
  if (result) {
    if (source == nullptr) {
      *result = E_OUTOFMEMORY;
    } else {
      *result = S_OK;
    }
  }
  return source;
}

void WINAPI SCFFSource::Init(BOOL loading, const CLSID *clsid) {
  if (loading) {
    // DLLがロードされた場合の処理
    MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("scff_dsf_*.ax: LOAD")));
  } else {
    // DLLがアンロードされた場合の処理
    MyDbgLog((LOG_MEMORY, kDbgNewDelete, TEXT("scff_dsf_*.ax: UNLOAD")));
  }
}
