
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

/// @file scff-imaging/scale.h
/// @brief scff_imaging::Scaleの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_SCALE_H_
#define SCFF_DSF_SCFF_IMAGING_SCALE_H_

#include "scff-imaging/common.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/processor.h"

struct SwsContext;

namespace scff_imaging {

/// @brief SWScaleを利用してイメージの拡大・縮小・ピクセルフォーマット変換を行う
class Scale : public Processor<AVPictureWithFillImage, AVPictureImage> {
 public:
  /// @brief コンストラクタ
  explicit Scale(SWScaleConfig swscale_config);
  /// @brief デストラクタ
  ~Scale();

  //-------------------------------------------------------------------
  /// @copydoc Processor::Init
  ErrorCode Init();
  /// @copydoc Processor::Run
  ErrorCode Run();
  //-------------------------------------------------------------------

 private:
  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(Scale);

  /// @brief 拡大縮小パラメータ
  const SWScaleConfig swscale_config_;

  /// @brief 拡大縮小用のコンテキスト
  struct SwsContext *scaler_;
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_SCALE_H_
