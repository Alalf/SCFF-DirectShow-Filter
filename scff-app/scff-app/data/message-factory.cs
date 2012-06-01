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

/// @file scff-app/data/message-factory.cs
/// @brief scff_*.Message生成・変換用メソッドの定義

namespace scff_app.data {

using System;
using System.Collections.Generic;

// scff_inteprocess.Messageをマネージドクラス化したクラス
partial class Message {

  /// @brief デフォルトコンストラクタ
  public Message() {
    this.Init();
  }

  //-------------------------------------------------------------------

  /// @brief デフォルトパラメータを設定
  void Init() {
    this.Timestamp = DateTime.Now.Ticks;
    this.LayoutType = scff_interprocess.LayoutType.kNullLayout;
    this.LayoutElementCount = 0;
    this.LayoutParameters = new List<LayoutParameter>();
  }
}
}
