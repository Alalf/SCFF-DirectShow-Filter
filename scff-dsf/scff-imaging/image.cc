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

/// @file scff-imaging/image.cc
/// scff_imaging::Imageの定義

#include "scff-imaging/image.h"

#include "scff-imaging/debug.h"
#include "scff-imaging/imaging-types.h"
#include "scff-imaging/utilities.h"

namespace scff_imaging {

//=====================================================================
// scff_imaging::Image
//=====================================================================

Image::Image()
    : pixel_format_(ImagePixelFormat::kInvalidPixelFormat),   // ありえない値
      width_(-1),                           // ありえない値
      height_(-1) {                         // ありえない値
  // nop
}

Image::~Image() {
  // nop
}

ImagePixelFormat Image::pixel_format() const {
  return pixel_format_;
}

AVPixelFormat Image::av_pixel_format() const {
  return utilities::ToAVPicturePixelFormat(pixel_format());
}

int Image::width() const {
  return width_;
}

int Image::height() const {
  return height_;
}

//-------------------------------------------------------------------
ErrorCode Image::Create(ImagePixelFormat pixel_format, int width, int height) {
  ASSERT(IsEmpty());
  pixel_format_ = pixel_format;
  width_ = width;
  height_ = height;
  return ErrorCode::kNoError;
}
//-------------------------------------------------------------------
}   // namespace scff_imaging
