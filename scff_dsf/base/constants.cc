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

/// @file base/constants.cc
/// グローバル定数定義

#include "base/constants.h"
#include "scff_imaging/imaging.h"

//=====================================================================
// 定数
//=====================================================================

const TCHAR kFilterName[] = TEXT("SCFF DirectShow Filter");

const GUID CLSID_SCFFSource = { 0xd64db8aa, 0x9055, 0x418f,
        {0xaf, 0xe9, 0xa0, 0x80, 0xa4, 0xfa, 0xe4, 0x7a }};

const SIZE kPreferredSizes[] = {
  {-1, -1},                   // 起動時の最初のSetMediaTypeによって決定
                              // プログラムからはkPreferredSizes[0]は使わない！
  // {256, 256},              // Skype
  {640, 360},                 // デフォルト＆ニコ生(β)＆Youtube
  {640, 480}, {720, 480},     // 480p
  {512, 288}, {512, 384},     // 2012/04時点のニコ生
  // {640, 387},              // 2012/04時点のJustin.tv
  {608, 342},                 // 2012/04時点のUstream
  {854, 480},                 // ニコニコ公式生放送
  {960, 540}, {1280, 780},    // リクエストのあった解像度
  {1600, 900}, {1920, 1080}   // リクエストのあった解像度2
};

const int kPreferredSizesCount =
  sizeof(kPreferredSizes) / sizeof(kPreferredSizes[0]);

const int kSupportedPixelFormatsCount =
#if defined(FOR_KOTOENCODER)
  // KoToEncoderは基本的に1ピクセルフォーマットしか対応していない
  1;
#else
  /// @attention enum->int
  static_cast<int>(
      scff_imaging::ImagePixelFormats::kSupportedPixelFormatsCount);
#endif

/// @warning 最初の1個はSetFormat用
const int kSupportedFormatsCount =
    kPreferredSizesCount * kSupportedPixelFormatsCount + 1;

const int kDefaultFPS = 30;

const int kMinOutputWidth   = 32;
const int kMinOutputHeight  = 32;
const int kMaxOutputWidth   = 4096;
const int kMaxOutputHeight  = 4096;

const double kMaxFPS        = 120.0;
const double kMinFPS        = 0.1;

const REFERENCE_TIME kMinFrameInterval =
    static_cast<REFERENCE_TIME>(UNITS / kMaxFPS);   // 120FPS
const REFERENCE_TIME kMaxFrameInterval =
    static_cast<REFERENCE_TIME>(UNITS / kMinFPS);   // 0.1FPS

const double kSCFFMonitorPollingInterval = 1.0; // 1Sec
