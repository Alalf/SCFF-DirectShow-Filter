
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

/// @file base/constants.cc
/// @brief グローバル定数定義

#include "base/constants.h"
#include "scff-imaging/imaging.h"

//=====================================================================
// 定数
//=====================================================================

// フィルタ名
const TCHAR kFilterName[] = TEXT("SCFF DirectShow Filter");

// GUID
// {D64DB8AA-9055-418F-AFE9-A080A4FAE47A}
const GUID CLSID_SCFFSource = { 0xd64db8aa, 0x9055, 0x418f,
        {0xaf, 0xe9, 0xa0, 0x80, 0xa4, 0xfa, 0xe4, 0x7a }};

// 優先ビデオサイズ
const SIZE kPreferredSizes[] = {
  {-1, -1},                 // 起動時の最初のSetMediaTypeによって決定
                            // プログラムからはkPreferredSizes[0]は使わない！
  {640, 360},               // デフォルト＆ニコ生(β)＆Youtube
  {512, 288}, {512, 384},   // 2012/04時点のニコ生
  {640, 387},               // 2012/04時点のJustin.tv
  {608, 342},               // 2012/04時点のUstream
  {640, 480}, {720, 480},   // 480p
  {854, 480}                // ニコニコ公式生放送
};

// 優先ビデオサイズの数
const int kPreferredSizesCount =
  sizeof(kPreferredSizes) / sizeof(kPreferredSizes[0]);

/// @brief 対応ピクセルフォーマットの数
const int kSupportedPixelFormatsCount =
#if defined(FOR_KOTOENCODER)
  // KoToEncoderは基本的に1ピクセルフォーマットしか対応していない
  1;
#else
  scff_imaging::kSupportedPixelFormatsCount;
#endif

// 優先フォーマットの数
const int kSupportedFormatsCount =
    kPreferredSizesCount * kSupportedPixelFormatsCount;

// デフォルトFPS
const int kDefaultFPS = 30;

// 最小出力width
const int kMinOutputWidth   = 32;
// 最小出力height
const int kMinOutputHeight  = 32;
// 最大出力width
const int kMaxOutputWidth   = 4096;
// 最大出力height
const int kMaxOutputHeight  = 4096;

// 最高FPS(1/Sec)
const double kMaxFPS        = 120.0;
// 最低FPS(1/Sec)
const double kMinFPS        = 0.1;

// フレーム区間の最短の長さ(100nSec)
const REFERENCE_TIME kMinFrameInterval =
    static_cast<REFERENCE_TIME>(UNITS / kMaxFPS);   // 120FPS
// フレーム区間の最長の長さ(100nSec)
const REFERENCE_TIME kMaxFrameInterval =
    static_cast<REFERENCE_TIME>(UNITS / kMinFPS);   // 0.1FPS

// SCFFMonitorのポーリング間隔
const double kSCFFMonitorPollingInterval = 1.0; // 1Sec
