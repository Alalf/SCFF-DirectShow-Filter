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

/// DbgOut用: トレース表示
extern const int kDbgTrace;
/// DbgOut用: あまり実行されない操作
extern const int kDbgRare;
/// DbgOut用: 重要な操作
extern const int kDbgImportant;
/// DbgOut用: New/Delete/Load/Unloadなどの深刻バグが発生しやすい部分
extern const int kDbgNewDelete;
/// DbgOut用: 最大デバッグレベル
extern const int kDbgMax;

/// DbgOut用: 現在のデバッグレベル。これ"以下"は表示しない。
extern const int kDbgCurrentLevel;

//=====================================================================
// デバッグ関数
//=====================================================================

#ifdef _DEBUG

/// DbgLogの代わり。改行をはさみ、少しフォーマットを変える
/// @todo(me) DbgOutの使い方が分かり次第置き換え
int MyDebugLog(DWORD types, DWORD level, LPCTSTR format, ...);

/// MessageBoxの代わり。フォーマット対応。
/// @todo(me) DbgOutの使い方が分かり次第置き換え
void MyDebugMessageBox(LPCTSTR format, ...);

#define MyDbgLog(_x_) MyDebugLog _x_
#define MyDbgMsgBox MyDebugMessageBox
#define MyDbgOutString DbgOutString

#else  // _DEBUG

#define MyDbgLog DbgLog
#define MyDbgMsgBox 0
#define MyDbgOutString DbgOutString

#endif  // _DEBUG

#endif  // SCFF_DSF_BASE_DEBUG_H_
