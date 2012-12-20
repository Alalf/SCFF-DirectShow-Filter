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

/// @file scff_app/viewmodel/message_properties.cs
/// scff_app.viewmodel.Messageのプロパティの定義

namespace scff_app.viewmodel {

using System;
using System.ComponentModel;

/// scff_inteprocess.Messageのビューモデル
partial class Message {
  public Int64 Timestamp {
    get {
      return DateTime.Now.Ticks;
    }
  }
  public scff_interprocess.LayoutTypes LayoutType {
    get {
      if (this.LayoutParameters.Count == 0) {
        return scff_interprocess.LayoutTypes.kNullLayout;
      } else if (this.LayoutParameters.Count == 1) {
        return scff_interprocess.LayoutTypes.kNativeLayout;
      } else {
        return scff_interprocess.LayoutTypes.kComplexLayout;
      }
    }
  }

  public Int32 LayoutElementCount {
    get {
      return this.LayoutParameters.Count;
    }
  }
  public BindingList<LayoutParameter> LayoutParameters { get; set; }
}
}