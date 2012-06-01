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

/// @file scff-app/viewmodel/entry.cs
/// @brief scff_app.viewmodel.Entryのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.Collections.Generic;

// scff_interprocess.Entryのビューモデル
partial class Entry {

  /// @brief 変換コンストラクタ
  public Entry(scff_interprocess.Entry input) {
    LoadFromInterprocess(input);
  }

  /// @brief 人間が読みやすい文字列に変換
  public override string ToString() {
    return
        "[" + this.ProcessID + "] " +
        this.ProcessName + " " +
        "(" + pixel_format_dictionary_[this.SamplePixelFormat] + " " +
        this.SampleWidth + "x" + this.SampleHeight + " " +
        this.FPS.ToString("F0") + "fps)";
  }

  //-------------------------------------------------------------------

  /// @brief scff_interprocessから変換
  void LoadFromInterprocess(scff_interprocess.Entry input) {
    this.ProcessID = input.process_id;
    this.ProcessName = input.process_name;
    this.SamplePixelFormat = (scff_interprocess.ImagePixelFormat)
        Enum.ToObject(typeof(scff_interprocess.ImagePixelFormat), input.sample_pixel_format);
    this.SampleWidth = input.sample_width;
    this.SampleHeight = input.sample_height;
    this.FPS = input.fps;
  }

  /// @brief Enum->String用辞書
  Dictionary<scff_interprocess.ImagePixelFormat, string> pixel_format_dictionary_ =
      new Dictionary<scff_interprocess.ImagePixelFormat, string>() {
    {scff_interprocess.ImagePixelFormat.kI420, "I420"},
    {scff_interprocess.ImagePixelFormat.kIYUV, "IYUV"},
    {scff_interprocess.ImagePixelFormat.kYV12, "YV12"},
    {scff_interprocess.ImagePixelFormat.kUYVY, "UYVY"},
    {scff_interprocess.ImagePixelFormat.kYUY2, "YUY2"},
    {scff_interprocess.ImagePixelFormat.kRGB0, "RGB0"}
  };
}
}
