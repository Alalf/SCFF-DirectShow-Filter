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
/// @brief scff_*.Entry生成・変換用メソッドの定義

namespace scff_app.data {

using System;

// scff_interprocess.Entryをマネージドクラス化したクラス
partial class Entry {

  /// @brief デフォルトコンストラクタ
  public Entry() {
    // nop
  }

  /// @brief 変換コンストラクタ
  public Entry(scff_interprocess.Entry input) {
    InitByInterprocess(input);
  }

  /// @brief scff_interprocessモジュールのパラメータを生成
  public scff_interprocess.Entry ToInterprocessEntry() {
    scff_interprocess.Entry output = new scff_interprocess.Entry();

    output.process_id = this.ProcessID;
    output.process_name = this.ProcessName;
    output.sample_pixel_format = (Int32)this.SamplePixelFormat;
    output.sample_width = this.SampleWidth;
    output.sample_height = this.SampleHeight;
    output.fps = this.FPS;

    return output;
  }

  //-------------------------------------------------------------------

  /// @brief scff_interprocessモジュールのパラメータから生成
  void InitByInterprocess(scff_interprocess.Entry input) {
    this.ProcessID = input.process_id;
    this.ProcessName = input.process_name;
    this.SamplePixelFormat = (scff_interprocess.ImagePixelFormat)
        Enum.ToObject(typeof(scff_interprocess.ImagePixelFormat), input.sample_pixel_format);
    this.SampleWidth = input.sample_width;
    this.SampleHeight = input.sample_height;
    this.FPS = input.fps;
  }
}
}
