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

/// @file scff_app/viewmodel/entry.cs
/// scff_app.viewmodel.Entryのメソッドの定義

namespace scff_app.viewmodel {

using System;
using System.Collections.Generic;

// scff_interprocess.Entryのビューモデル
partial class Entry {

  /// 変換コンストラクタ
  public Entry(scff_interprocess.Entry input) {
    LoadFromInterprocess(input);
  }

  /// 人間が読みやすい文字列に変換
  public override string ToString() {
    return
        "[" + this.ProcessID + "] " +
        this.ProcessName + " " +
        "(" + pixel_format_dictionary_[this.SamplePixelFormat] + " " +
        this.SampleWidth + "x" + this.SampleHeight + " " +
        this.FPS.ToString("F0") + "fps)";
  }

  //-------------------------------------------------------------------

  /// scff_interprocessから変換
  void LoadFromInterprocess(scff_interprocess.Entry input) {
    this.ProcessID = input.process_id;
    this.ProcessName = input.process_name;
    this.SamplePixelFormat = (scff_interprocess.ImagePixelFormats)
        Enum.ToObject(typeof(scff_interprocess.ImagePixelFormats), input.sample_pixel_format);
    this.SampleWidth = input.sample_width;
    this.SampleHeight = input.sample_height;
    this.FPS = input.fps;
  }

  /// Enum->String用辞書
  Dictionary<scff_interprocess.ImagePixelFormats, string> pixel_format_dictionary_ =
      new Dictionary<scff_interprocess.ImagePixelFormats, string>() {
    {scff_interprocess.ImagePixelFormats.kI420, "I420"},
    {scff_interprocess.ImagePixelFormats.kIYUV, "IYUV"},
    {scff_interprocess.ImagePixelFormats.kYV12, "YV12"},
    {scff_interprocess.ImagePixelFormats.kUYVY, "UYVY"},
    {scff_interprocess.ImagePixelFormats.kYUY2, "YUY2"},
    {scff_interprocess.ImagePixelFormats.kRGB0, "RGB0"}
  };
}
}
