// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file scff_imaging/windows_ddb_image.h
/// scff_imaging::WindowsDDBImageの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_
#define SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_

#include "scff_imaging/image.h"

namespace scff_imaging {

/// Windowsビットマップ(HBITMAP)の実体を管理するクラス
class WindowsDDBImage: public Image {
 public:
  /// Windowsビットマップの生成方法
  enum class Source {
    /// ありえない値
    kInvalidSource,
    /// WindowハンドルからCreateCompatibleBitmapで生成
    kFromWindow,
    /// DLLリソースから生成
    kFromResource
  };

  /// コンストラクタ
  WindowsDDBImage();
  /// デストラクタ
  ~WindowsDDBImage();

  //-------------------------------------------------------------------
  /// @copydoc Image::IsEmpty
  bool IsEmpty() const;
  /// リソースから実体を作る
  /// @sa Image::Create
  ErrorCode CreateFromResource(int width, int height, WORD resource_id);
  /// 与えられたWindowからCompatibleBitmapを作成する
  /// @sa Image::Create
  ErrorCode CreateFromWindow(int width, int height, HWND window);
  //-------------------------------------------------------------------

  /// Getter: Windowsビットマップハンドル
  HBITMAP windows_ddb() const;

 private:
  /// Windowsビットマップの生成方法
  Source from_;

  /// Windowsビットマップハンドル
  HBITMAP windows_ddb_;

  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(WindowsDDBImage);
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_
