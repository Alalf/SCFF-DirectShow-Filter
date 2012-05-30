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

/// @file scff-app/data/layout-parameter-factory.cs
/// @brief scff_*.LayoutParameter生成・変換用メソッドの定義

namespace scff_app.data {

using System;
using System.Runtime.InteropServices;
using System.Text;

// scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
partial class LayoutParameter {

  /// @brief デフォルトコンストラクタ
  public LayoutParameter() {
    this.Init();
  }

  /// @brief 変換コンストラクタ
  public LayoutParameter(scff_interprocess.LayoutParameter input, int bound_width, int bound_height) {
    this.InitByInterprocess(input, bound_width, bound_height);
  }

  /// @brief scff_interprocess用に変換
  public scff_interprocess.LayoutParameter ToInterprocess(int bound_width, int bound_height) {
    scff_interprocess.LayoutParameter output = new scff_interprocess.LayoutParameter();

    // 相対比率→ピクセル値変換
    output.bound_x = (Int32)(bound_width * this.BoundRelativeLeft) / 100;
    output.bound_y = (Int32)(bound_height * this.BoundRelativeTop) / 100;
    output.bound_width =
        (Int32)(bound_width * this.BoundRelativeRight) / 100 - output.bound_x;
    output.bound_height =
        (Int32)(bound_height * this.BoundRelativeBottom) / 100 - output.bound_y;

    output.window = (UInt64)this.Window;
    output.clipping_x = this.ClippingX;
    output.clipping_y = this.ClippingY;
    output.clipping_width = this.ClippingWidth;
    output.clipping_height = this.ClippingHeight;
    output.show_cursor = Convert.ToByte(this.ShowCursor);
    output.show_layered_window = Convert.ToByte(this.ShowLayeredWindow);

    // 拡大縮小設定
    output.swscale_config = this.SWScaleConfig.ToInterprocess();
   
    output.stretch = Convert.ToByte(this.Stretch);
    output.keep_aspect_ratio = Convert.ToByte(this.KeepAspectRatio);
    output.rotate_direction = (Int32)this.RotateDirection;

    return output;
  }

  //-------------------------------------------------------------------

  /// @brief デフォルトパラメータを設定
  void Init() {
    this.Window = ExternalAPI.GetDesktopWindow();
    ExternalAPI.RECT window_rect;
    ExternalAPI.GetClientRect(this.Window, out window_rect);
    this.ClippingX = window_rect.left;
    this.ClippingY = window_rect.top;
    this.ClippingWidth = window_rect.right;
    this.ClippingHeight = window_rect.bottom;

    // 拡大縮小設定
    this.SWScaleConfig = new SWScaleConfig();

    this.Stretch = true;
    this.KeepAspectRatio = true;

    this.RotateDirection = scff_interprocess.RotateDirection.kNoRotate;
    
    // GUIクライアント固有の設定
    this.BoundRelativeLeft = 0.0;
    this.BoundRelativeRight = 100.0;
    this.BoundRelativeTop = 0.0;
    this.BoundRelativeBottom = 100.0;
    this.Fit = true;
  }

  /// @brief scff_interprocessから変換
  void InitByInterprocess(scff_interprocess.LayoutParameter input, int bound_width, int bound_height) {
    this.Window = unchecked((UIntPtr)input.window);
    this.ClippingX = input.clipping_x;
    this.ClippingY = input.clipping_y;
    this.ClippingWidth = input.clipping_width;
    this.ClippingHeight = input.clipping_height;
    this.ShowCursor = Convert.ToBoolean(input.show_cursor);
    this.ShowLayeredWindow = Convert.ToBoolean(input.show_layered_window);
 
    // 拡大縮小設定
    this.SWScaleConfig = new SWScaleConfig(input.swscale_config);

    this.Stretch = Convert.ToBoolean(input.stretch);
    this.KeepAspectRatio = Convert.ToBoolean(input.keep_aspect_ratio);

    this.RotateDirection = (scff_interprocess.RotateDirection)
        Enum.ToObject(typeof(scff_interprocess.RotateDirection), input.rotate_direction);

    // GUIクライアント固有の設定
    this.BoundRelativeLeft = (double)input.bound_x * 100 / bound_width;
    this.BoundRelativeTop = (double)input.bound_y * 100 / bound_height;
    this.BoundRelativeRight = (double)(input.bound_width - input.bound_x) * 100 / bound_width;
    this.BoundRelativeBottom = (double)(input.bound_height - input.bound_y) * 100 / bound_height;
  }
}
}
