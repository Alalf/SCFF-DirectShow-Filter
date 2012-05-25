
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

// scff_interprocess.Entryをマネージドクラス化したクラス
public partial class Entry {

  /// @brief 表示用
  public string EntryInfo {
    get {
      string pixel_format_string;
      switch (SamplePixelFormat) {
      case scff_interprocess.ImagePixelFormat.kIYUV:
        pixel_format_string = "IYUV";
        break;
      case scff_interprocess.ImagePixelFormat.kI420:
        pixel_format_string = "I420";
        break;
      case scff_interprocess.ImagePixelFormat.kYV12:
        pixel_format_string = "YV12";
        break;
      case scff_interprocess.ImagePixelFormat.kYUY2:
        pixel_format_string = "YUY2";
        break;
      case scff_interprocess.ImagePixelFormat.kUYVY:
        pixel_format_string = "UYVY";
        break;
      case scff_interprocess.ImagePixelFormat.kYVYU:
        pixel_format_string = "YVYU";
        break;
      case scff_interprocess.ImagePixelFormat.kYVU9:
        pixel_format_string = "YVU9";
        break;
      case scff_interprocess.ImagePixelFormat.kRGB24:
        pixel_format_string = "RGB24";
        break;
      case scff_interprocess.ImagePixelFormat.kRGB0:
        pixel_format_string = "RGB0";
        break;
      case scff_interprocess.ImagePixelFormat.kRGB555:
        pixel_format_string = "RGB555";
        break;
      case scff_interprocess.ImagePixelFormat.kRGB565:
        pixel_format_string = "RGB565";
        break;
      case scff_interprocess.ImagePixelFormat.kRGB8:
        pixel_format_string = "RGB8";
        break;
      default:
        pixel_format_string = "(invalid)";
        break;
      }

      return "[" + ProcessID + "] " + ProcessName +
              " (" + pixel_format_string + " " + SampleWidth + "x" + SampleHeight +
              " " + FPS.ToString("F0") + "fps)";
    }
  }
};
}
