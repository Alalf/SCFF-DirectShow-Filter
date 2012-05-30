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

/// @file scff-app/data/message.cs
/// @brief scff_interprocess.Messageをマネージドクラス化したクラスの定義

namespace scff_app.data {

using System;
using System.Collections.Generic;

/// @brief scff_inteprocess.Messageをマネージドクラス化したクラス
partial class Message {
  public Int64 Timestamp { get; set; }
  public scff_interprocess.LayoutType LayoutType { get; set; }
  public Int32 LayoutElementCount { get; set; }

  List<LayoutParameter> layout_parameters_ = new List<LayoutParameter>();
  public List<LayoutParameter> LayoutParameters {
    get { return layout_parameters_; }
    set { layout_parameters_ = value; }
  }
}
}