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

/// @file scff-app/data/entry-view.cs
/// @brief Entryの表示用メソッドの定義

namespace scff_app.data {

using System.Collections.Generic;

// scff_interprocess.Entryをマネージドクラス化したクラス
partial class Entry {

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

  /// @brief 人間が読みやすい文字列に変換
  public override string ToString() {
    return
        "[" + ProcessID + "] " +
        ProcessName + " " +
        "(" + pixel_format_dictionary_[SamplePixelFormat] + " " +
        SampleWidth + "x" + SampleHeight + " " +
        FPS.ToString("F0") + "fps)";
  }

  /// @brief コンボボックス表示用
  public string ProcessInformation {
    get { return ToString(); }
  }
};
}
