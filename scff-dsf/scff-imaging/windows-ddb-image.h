
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

/// @file scff-imaging/windows-ddb-image.h
/// @brief scff_imaging::WindowsDDBImageの宣言

#ifndef SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_
#define SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_

#include "scff-imaging/image.h"

namespace scff_imaging {

/// @brief Windowsビットマップ(HBITMAP)の実体を管理するクラス
class WindowsDDBImage: public Image {
 public:
  /// @brief Windowsビットマップの生成方法
  enum Source {
    /// @brief ありえない値
    kInvalidSource,
    /// @brief WindowハンドルからCreateCompatibleBitmapで生成
    kFromWindow,
    /// @brief DLLリソースから生成
    kFromResource
  };

  /// @brief コンストラクタ
  WindowsDDBImage();
  /// @brief デストラクタ
  ~WindowsDDBImage();

  //-------------------------------------------------------------------
  /// @brief Create()などによって実体がまだ生成されていない場合
  bool IsEmpty() const;
  /// @brief リソースから実体を作る
  ErrorCode CreateFromResource(int width, int height, WORD resource_id);
  /// @brief 与えられたWindowからCompatibleBitmapを作成する
  ErrorCode CreateFromWindow(int width, int height, HWND window);
  //-------------------------------------------------------------------

  /// @brief Getter: Windowsビットマップハンドル
  HBITMAP windows_ddb() const;

 private:
  // コピー＆代入禁止
  DISALLOW_COPY_AND_ASSIGN(WindowsDDBImage);

  /// @brief Windowsビットマップの生成方法
  Source from_;

  /// @brief Windowsビットマップハンドル
  HBITMAP windows_ddb_;
};
}   // namespace scff_imaging

#endif  // SCFF_DSF_SCFF_IMAGING_WINDOWS_DDB_IMAGE_H_
