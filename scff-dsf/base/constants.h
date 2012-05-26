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

/// @file base/constants.h
/// @brief グローバル定数宣言

#ifndef SCFF_DSF_BASE_CONSTANTS_H_
#define SCFF_DSF_BASE_CONSTANTS_H_

#include <streams.h>
#include <tchar.h>
#include <Windows.h>

//=====================================================================
// プリプロセッサ
//=====================================================================

/// @brief KoToEncoder(KTE)を利用してYUV出力を利用する場合
// #define FOR_KOTOENCODER

//=====================================================================
// 定数
//=====================================================================

/// @brief フィルタ名: "SCFF DirectShow Filter"
extern const TCHAR kFilterName[];

/// @brief SCFF DirectShow FilterのGUID
extern const GUID CLSID_SCFFSource;

/// @brief 優先ビデオサイズ
extern const SIZE kPreferredSizes[];

/// @brief 優先ビデオサイズの数
extern const int kPreferredSizesCount;

/// @brief 対応ピクセルフォーマットの数
extern const int kSupportedPixelFormatsCount;

/// @brief 対応フォーマットの数
extern const int kSupportedFormatsCount;

/// @brief デフォルトFPS
extern const int kDefaultFPS;

/// @brief 最小出力width
extern const int kMinOutputWidth;
/// @brief 最小出力height
extern const int kMinOutputHeight;
/// @brief 最大出力width
extern const int kMaxOutputWidth;
/// @brief 最大出力height
extern const int kMaxOutputHeight;

/// @brief 最高FPS(1/Sec)
extern const double kMaxFPS;
/// @brief 最低FPS(1/Sec)
extern const double kMinFPS;

/// @brief フレーム区間の最短の長さ(100nSec)
extern const REFERENCE_TIME kMinFrameInterval;
/// @brief フレーム区間の最長の長さ(100nSec)
extern const REFERENCE_TIME kMaxFrameInterval;

/// @brief SCFFMonitorのポーリング間隔
extern const double kSCFFMonitorPollingInterval;

#endif  // SCFF_DSF_BASE_CONSTANTS_H_
