
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

/// @file scff-app/data/entry.cs
/// @brief scff_interprocess.Entryをマネージドクラス化したクラスの定義

using System;

namespace scff_app.data {

/// @brief scff_interprocess.Entryをマネージドクラス化したクラス
public partial class Entry {
  public UInt32 ProcessID {get; set;}
  public string ProcessName {get; set;}
  public Int32 SampleWidth {get; set;}
  public Int32 SampleHeight {get; set;}
  public scff_interprocess.ImagePixelFormat SamplePixelFormat { get; set; }
  public Double FPS {get; set;}
}
}
