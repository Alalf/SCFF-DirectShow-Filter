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

/// @file base/constants.h
/// グローバル定数宣言

#ifndef SCFF_DSF_BASE_CONSTANTS_H_
#define SCFF_DSF_BASE_CONSTANTS_H_

#include <streams.h>
#include <tchar.h>
#include <Windows.h>

//=====================================================================
// プリプロセッサ
//=====================================================================

/// KoToEncoder(KTE)を利用してYUV出力を利用する場合
// #define FOR_KOTOENCODER

//=====================================================================
// 定数
//=====================================================================

/// フィルタ名: "SCFF DirectShow Filter"
extern const TCHAR kFilterName[];

/// SCFF DirectShow FilterのGUID
/// - {D64DB8AA-9055-418F-AFE9-A080A4FAE47A}
extern const GUID CLSID_SCFFSource;

/// 優先ビデオサイズ
extern const SIZE kPreferredSizes[];

/// 優先ビデオサイズの数
extern const int kPreferredSizesCount;

/// 対応ピクセルフォーマットの数
extern const int kSupportedPixelFormatsCount;

/// 対応フォーマットの数
extern const int kSupportedFormatsCount;

/// デフォルトFPS
extern const int kDefaultFPS;

/// 最小出力width
extern const int kMinOutputWidth;
/// 最小出力height
extern const int kMinOutputHeight;
/// 最大出力width
extern const int kMaxOutputWidth;
/// 最大出力height
extern const int kMaxOutputHeight;

/// 最高FPS(1/Sec)
extern const double kMaxFPS;
/// 最低FPS(1/Sec)
extern const double kMinFPS;

/// フレーム区間の最短の長さ(100nSec)
extern const REFERENCE_TIME kMinFrameInterval;
/// フレーム区間の最長の長さ(100nSec)
extern const REFERENCE_TIME kMaxFrameInterval;

/// SCFFMonitorのポーリング間隔
extern const double kSCFFMonitorPollingInterval;

#endif  // SCFF_DSF_BASE_CONSTANTS_H_
