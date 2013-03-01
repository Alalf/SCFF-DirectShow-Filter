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

/// @file base/debug.h
/// デバッグ用定数、関数宣言

#ifndef SCFF_DSF_BASE_DEBUG_H_
#define SCFF_DSF_BASE_DEBUG_H_

#ifdef _DEBUG
#include <Windows.h>
#endif  // _DEBUG
#include <streams.h>  // for ASSERT, LOG_TRACE, ...

//=====================================================================
// デバッグ用定数
//=====================================================================

/// DbgOut用: アプリケーションの進捗の概要が分かるメッセージ
extern const int kTraceInfo;
/// DbgOut用: アプリケーションをデバッグするのに役立つ詳細なイベント情報
extern const int kTraceDebug;
/// DbgOut用: kTraceDebugより詳細なイベント情報
extern const int kTrace;

/// DbgOut用: 現在のTraceレベル。この数値より上は表示しない。
extern const int kTraceCurrentLevel;

/// DbgOut用: アプリケーションを異常終了させるような非常に深刻なイベント
extern const int kErrorFatal;
/// DbgOut用: アプリケーションの稼働が継続できる程度のエラー
extern const int kError;
/// DbgOut用: 潜在的に害を及ぼすような状況
extern const int kErrorWarn;

/// DbgOut用: 現在のErrorレベル。この数値より上は表示しない。
extern const int kErrorCurrentLevel;


#endif  // SCFF_DSF_BASE_DEBUG_H_
