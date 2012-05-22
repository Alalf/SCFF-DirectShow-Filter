
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

/// @file scff-app/data/entry-factory.cs
/// @brief scff_*.Entryを生成・変換するためのクラスの定義

using System;

namespace scff_app.data {

/// @brief scff_*.Entryを生成・変換するためのクラス
public class EntryFactory {

  /// @brief scff_interprocessモジュールのパラメータから生成
  public static Entry FromInterprocess(scff_interprocess.Entry input) {
    Entry output = new Entry();

    output.ProcessID = input.process_id;
    output.ProcessName = input.process_name;
    output.SamplePixelFormat = (scff_interprocess.ImagePixelFormat)
        Enum.ToObject(typeof(scff_interprocess.ImagePixelFormat), input.sample_pixel_format);
    output.SampleWidth = input.sample_width;
    output.SampleHeight = input.sample_height;
    output.FPS = input.fps;

    return output;
  }

  /// @brief scff_interprocessモジュールのパラメータを生成
  public static scff_interprocess.Entry ToInterprocessEntry(Entry input) {
    scff_interprocess.Entry output = new scff_interprocess.Entry();

    output.process_id = input.ProcessID;
    output.process_name = input.ProcessName;
    output.sample_pixel_format = (Int32)input.SamplePixelFormat;
    output.sample_width = input.SampleWidth;
    output.sample_height = input.SampleHeight;
    output.fps = input.FPS;

    return output;
  }
}
}
