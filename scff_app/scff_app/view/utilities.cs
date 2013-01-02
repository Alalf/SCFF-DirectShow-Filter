// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
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

/// @file scff_app/view/utilities.cs
/// View用ユーティリティクラス

namespace scff_app.view {

using System;
using System.Drawing;

/// View用ユーティリティ
class Utilities {

  //===================================================================
  // 定数
  //===================================================================

  public const int kBorderSize = 16;

  //===================================================================
  // メソッド
  //===================================================================

  static int GetHitTestResult(Rectangle client_rect, Point client_point) {
    var on_top_border = client_point.Y < kBorderSize;
    var on_bottom_border = client_point.Y > client_rect.Height - kBorderSize;
    var on_left_border = client_point.X < kBorderSize;
    var on_right_border = client_point.X > client_rect.Width - kBorderSize;

    // リサイズ領域ではない場合はクライアント領域とする
    // TopよりはBottom、LeftよりはRightを優先すること
    int hittest_result = ExternalAPI.HTCLIENT;
    if (on_bottom_border && on_right_border) {
      // 優先順位1: 右下端。ここが一番選択しやすいように。
      hittest_result = ExternalAPI.HTBOTTOMRIGHT;
    } else if (on_bottom_border && on_left_border) {
      // 優先順位2: 左下端
      hittest_result = ExternalAPI.HTBOTTOMLEFT;
    } else if (on_top_border && on_right_border) {
      hittest_result = ExternalAPI.HTTOPRIGHT;
    } else if (on_top_border && on_left_border) {
      hittest_result = ExternalAPI.HTTOPLEFT;
    } else if (on_bottom_border) {
      hittest_result = ExternalAPI.HTBOTTOM;
    } else if (on_right_border) {
      hittest_result = ExternalAPI.HTRIGHT;
    } else if (on_top_border) {
      hittest_result = ExternalAPI.HTTOP;
    } else if (on_left_border) {
      hittest_result = ExternalAPI.HTLEFT;
    }
    return hittest_result;
  }

  static IntPtr MakeParam(int low, int high) {
    return new IntPtr((low & 0xFFFF) | (high << 16));
  }

  static void GetParam(IntPtr param, out int low, out int high) {
    low = unchecked((short)param);
    high = unchecked((short)((uint)param >> 16));
  }

  //protected override void WndProc(ref System.Windows.Forms.Message m) {
  //  switch(m.Msg) {
  //  case ExternalAPI.WM_NCHITTEST:
  //    // 結果の上書きなのでこちらが先
  //    base.WndProc(ref m);

  //    // コントロールの境界線付近をボーダー領域とみなす
  //    int screen_x, screen_y;
  //    GetParam(m.LParam, out screen_x, out screen_y);
  //    Point hittest_point = this.PointToClient(new Point(screen_x, screen_y));

  //    int ht_mode = GetHitTestResult(this.ClientRectangle, hittest_point);
  //    m.Result = new IntPtr(ht_mode);
  //    break;

  //  case ExternalAPI.WM_LBUTTONDOWN:
  //    // クライアント領域内での左クリックはキャプションへの左クリックとみなす
  //    m.Msg = ExternalAPI.WM_NCLBUTTONDOWN;
  //    m.WParam = new IntPtr(ExternalAPI.HTCAPTION);
  //    int client_x, client_y;
  //    GetParam(m.LParam, out client_x, out client_y);
  //    Point screen_lclick_point = this.PointToScreen(new Point(client_x, client_y));
  //    m.LParam = MakeParam(screen_lclick_point.X, screen_lclick_point.Y);

  //    // メッセージの送りなおし
  //    WndProc(ref m);
  //    break;

  //  default:
  //    base.WndProc(ref m);
  //    break;
  //  }
  //}
}
}
