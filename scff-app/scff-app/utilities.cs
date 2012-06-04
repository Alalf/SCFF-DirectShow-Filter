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

/// @file scff-app/utilities.cs
/// @brief scff_app用ユーティリティクラスの定義

namespace scff_app {

using System;
using System.Drawing;

/// @brief scff_app用ユーティリティクラス
class Utilities {
  /// @brief マルチモニタを考慮してウィンドウ領域を求める
  public static Rectangle GetWindowRectangle(UIntPtr window) {
    Rectangle window_rectangle = new Rectangle(0, 0, 0, 0);
    if (window == ExternalAPI.GetDesktopWindow()) {
      window_rectangle.X = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN);
      window_rectangle.Y = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN);
      window_rectangle.Width = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXVIRTUALSCREEN);
      window_rectangle.Height = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CYVIRTUALSCREEN);
    } else if (ExternalAPI.IsWindow(window)) {
      ExternalAPI.RECT window_rect;
      ExternalAPI.GetClientRect(window, out window_rect);
      window_rectangle.X = window_rect.left;
      window_rectangle.Y = window_rect.top;
      window_rectangle.Width = window_rect.right - window_rect.left;
      window_rectangle.Height = window_rect.bottom - window_rect.top;
    }
    return window_rectangle;
  }

  /// @brief クライアント領域のスクリーン座標を得る
  public static void GetScreenClientRect(UIntPtr window,
      out int screen_x, out int screen_y, out int width, out int height) {
    ExternalAPI.RECT window_rect;
    ExternalAPI.GetClientRect(window, out window_rect);
    ExternalAPI.POINT window_screen_origin;
    window_screen_origin.x = 0;
    window_screen_origin.y = 0;
    ExternalAPI.ClientToScreen(window, ref window_screen_origin);

    screen_x = window_screen_origin.x;
    screen_y = window_screen_origin.y;
    width = window_rect.right - window_rect.left;
    height = window_rect.bottom - window_rect.top;
  }
}
}
