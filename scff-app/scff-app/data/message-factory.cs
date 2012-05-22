
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
/// @brief scff_*.Messageを生成・変換するためのクラスの定義

using System;
using System.Collections.Generic;

namespace scff_app.data {

/// @brief scff_*.Messageを生成・変換するためのクラス
class MessageFactory {

  /// @brief デフォルトパラメータを持ったMessageを生成
  public static Message Default() {
    Message output = new Message();

    // デフォルト値を設定
    output.Timestamp = DateTime.Now.Ticks;
    output.LayoutType = scff_interprocess.LayoutType.kNullLayout;
    output.LayoutElementCount = 0;
    
    return output;
  }

  /// @brief scff_interprocessモジュールのパラメータから生成
  public static Message FromInterprocess(scff_interprocess.Message input) {
    Message output = new Message();

    output.Timestamp = input.timestamp;
    output.LayoutType = (scff_interprocess.LayoutType)
        Enum.ToObject(typeof(scff_interprocess.LayoutType), input.layout_type);

    output.LayoutElementCount = input.layout_element_count;
    const int kMaxComplexLayoutElements = scff_interprocess.Interprocess.kMaxComplexLayoutElements;
    for (int i = 0; i < kMaxComplexLayoutElements; i++) {
      LayoutParameter layout_parameter =
          LayoutParameterFactory.FromInterprocess(input.layout_parameters[i]);
      output.LayoutParameters.Add(layout_parameter);
    }

    return output;
  }

  /// @brief Interprocessで利用可能な構造体に変換
  public static scff_interprocess.Message ToInterprocess(Message input, int bound_width, int bound_height) {
    scff_interprocess.Message output = new scff_interprocess.Message();

    output.timestamp = input.Timestamp;
    output.layout_type = (Int32)input.LayoutType;
    output.layout_element_count = input.LayoutElementCount;
    
    // Listの前から順番に書き込む
    const int kMaxComplexLayoutElements = scff_interprocess.Interprocess.kMaxComplexLayoutElements;
    output.layout_parameters = new scff_interprocess.LayoutParameter[kMaxComplexLayoutElements];
    for (int i = 0; i < kMaxComplexLayoutElements; i++) {
      if (i < input.LayoutParameters.Count) {
        output.layout_parameters[i] = LayoutParameterFactory.ToInterprocess(
            input.LayoutParameters[i], bound_width, bound_height);
      } else {
        // C#はインスタンスは勝手にゼロクリアされる
        output.layout_parameters[i] = new scff_interprocess.LayoutParameter();
      }
    }

    return output;
  }
}
}
