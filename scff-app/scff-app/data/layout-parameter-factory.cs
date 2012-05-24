
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
/// @brief scff_*.LayoutParameterを生成・変換するためのクラスの定義

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace scff_app.data {

/// @brief scff_*.LayoutParameterを生成・変換するためのクラス
class LayoutParameterFactory {
  //-------------------------------------------------------------------
  // Wrappers
  //-------------------------------------------------------------------
  [DllImport("user32.dll", SetLastError = false)]
  static extern UIntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)]
  struct RECT { public int left, top, right, bottom; }
  [DllImport("user32.dll")]
  static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);
  //-------------------------------------------------------------------

  /// @brief デフォルトパラメータを持ったLayoutParameterを生成
  public static LayoutParameter Default() {
    LayoutParameter output = new LayoutParameter();

    // デフォルト値を設定
    output.KeepAspectRatio = true;
    output.Stretch = true;

    // 拡大縮小設定
    output.SWScaleConfig.Flags = scff_interprocess.SWScaleFlags.kArea;
    output.SWScaleConfig.IsFilterEnabled = false;
    output.SWScaleConfig.ChromaHShift = 1.0F;
    output.SWScaleConfig.ChromaVShift = 1.0F;

    // Windowまわり
    output.Window = GetDesktopWindow();
    RECT window_rect;
    GetClientRect(output.Window, out window_rect);
    output.ClippingX = window_rect.left;
    output.ClippingY = window_rect.top;
    output.ClippingWidth = window_rect.right;
    output.ClippingHeight = window_rect.bottom;
    
    output.Fit = true;

    // GUIクライアントからは使わないが一応
    output.BoundX = 0;
    output.BoundY = 0;
    output.BoundWidth = 1;
    output.BoundHeight = 1;
    //----

    output.BoundRelativeLeft = 0.0;
    output.BoundRelativeRight = 100.0;
    output.BoundRelativeTop = 0.0;
    output.BoundRelativeBottom = 100.0;

    return output;
  }

  /// @brief scff_interprocessモジュールのパラメータから生成
  public static LayoutParameter FromInterprocess(scff_interprocess.LayoutParameter input) {
    LayoutParameter output = new LayoutParameter();

    output.BoundX = input.bound_x;
    output.BoundY = input.bound_y;
    output.BoundWidth = input.bound_width;
    output.BoundHeight = input.bound_height;
    output.Window = unchecked((UIntPtr)input.window);
    output.ClippingX = input.clipping_x;
    output.ClippingY = input.clipping_y;
    output.ClippingWidth = input.clipping_width;
    output.ClippingHeight = input.clipping_height;
    output.ShowCursor = Convert.ToBoolean(input.show_cursor);
    output.ShowLayeredWindow = Convert.ToBoolean(input.show_layered_window);
 
    // 拡大縮小設定
    output.SWScaleConfig = SWScaleConfigFactory.FromInterprocess(input.swscale_config);

    output.Stretch = Convert.ToBoolean(input.stretch);
    output.KeepAspectRatio = Convert.ToBoolean(input.keep_aspect_ratio);

    output.RotateDirection = (scff_interprocess.RotateDirection)
        Enum.ToObject(typeof(scff_interprocess.RotateDirection), input.rotate_direction);

    return output;
  }

  /// @brief scff_interprocessモジュールのパラメータを生成
  public static scff_interprocess.LayoutParameter ToInterprocess(LayoutParameter input, int bound_width, int bound_height) {
    scff_interprocess.LayoutParameter output = new scff_interprocess.LayoutParameter();

    //-- GUIクライアント限定の処理！
    output.bound_x = (Int32)(bound_width * input.BoundRelativeLeft) / 100;
    output.bound_y = (Int32)(bound_height * input.BoundRelativeTop) / 100;
    output.bound_width =
        (Int32)(bound_width * input.BoundRelativeRight) / 100 - output.bound_x;
    output.bound_height =
        (Int32)(bound_height * input.BoundRelativeBottom) / 100 - output.bound_y;
    //--

    output.window = (UInt64)input.Window;
    output.clipping_x = input.ClippingX;
    output.clipping_y = input.ClippingY;
    output.clipping_width = input.ClippingWidth;
    output.clipping_height = input.ClippingHeight;
    output.show_cursor = Convert.ToByte(input.ShowCursor);
    output.show_layered_window = Convert.ToByte(input.ShowLayeredWindow);

    // 拡大縮小設定
    output.swscale_config = SWScaleConfigFactory.ToInterprocess(input.SWScaleConfig);
   
    output.stretch = Convert.ToByte(input.Stretch);
    output.keep_aspect_ratio = Convert.ToByte(input.KeepAspectRatio);
    output.rotate_direction = (Int32)input.RotateDirection;

    return output;
  }
}
}
