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

/// @file scff-app/view/movable_and_resizable.cs
/// @brief コントロールに対し、ドラッグでの移動/リサイズ機能を付加するためのクラスの定義

namespace scff_app.view {

using System;
using System.Drawing;
using System.Windows.Forms;

/// @brief コントロールに対し、ドラッグでの移動/リサイズ機能を付加するためのクラス
// メモ:
//  座標系はいくつかあるが、注意しないといけない点
//  ControlLocation:      これはTarget内の座標系を意味する
//  ContainerLocation:    これはTargetを内包するContainer内の座標系を意味する
//  なお、サイズ(幅、高さ)は無次元数なので特に区別をする必要はない。
class MovableAndResizable : IDisposable {

  /// @brief リサイズとする領域のサイズ
  const int kBorder = 16;

  /// @brief 最小サイズ
  const int kMinimumSize = kBorder * 3;

  /// @brief 現在の状態を表す定数
  enum Mode {
    kNop,
    kMove,
    kResizeTopLeft,
    kResizeTop,
    kResizeTopRight,
    kResizeLeft,
    kResizeRight,
    kResizeBottomLeft,
    kResizeBottom,
    kResizeBottomRight
  }

  //-------------------------------------------------------------------

  void Init(Control target) {
    target_ = target;

    // イベントハンドラの登録
    target_.MouseDown += target_MouseDown;
    target_.MouseMove += target_MouseMove;
    target_.MouseUp += target_MouseUp;
    target_.SizeChanged += target_SizeChanged;
  }

  /// @brief コンストラクタ
  public MovableAndResizable(Control target) {
    is_parent_exists_ = true;
    bounds_ = new Rectangle();
    Init(target);
  }

  /// @brief コンストラクタ
  public MovableAndResizable(Control target, Rectangle bounds) {
    is_parent_exists_ = false;
    bounds_ = bounds;
    Init(target);
  }

  /// @brief Dispose（GCを考慮したデストラクタ）
  public void Dispose() {
    // イベントハンドラの登録解除
    target_.MouseDown -= target_MouseDown;
    target_.MouseMove -= target_MouseMove;
    target_.MouseUp -= target_MouseUp;
    target_.SizeChanged -= target_SizeChanged;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void target_MouseDown(object sender, MouseEventArgs e) {
    var mouse_down_control_location = e.Location;

    mode_ = GetModeFromMouseControlLocation(mouse_down_control_location);
    last_mouse_container_location_ = ControlToContainer(mouse_down_control_location);
    current_container_location_ = target_.Location;
    current_size_ = target_.Size;

    if (mode_ != Mode.kNop && mode_ != Mode.kMove) {
      ExternalAPI.ReleaseCapture();
      ExternalAPI.SystemDefinedMessage message = GetMessageFromMode(mode_);
      ExternalAPI.SendMessage(target_.Handle, ExternalAPI.WM_NCLBUTTONDOWN, new IntPtr((int)message), IntPtr.Zero);
    }
  }

  private void target_MouseMove(object sender, MouseEventArgs e) {
    var mouse_control_location = e.Location;

    // カーソルを変更
    Mode current_mode = GetModeFromMouseControlLocation(mouse_control_location);
    target_.Cursor = GetCursorFromMode(current_mode);

    // 何もしない
    if (mode_ == Mode.kNop) {
      return;
    } 
    
    // 移動モード
    if (mode_ == Mode.kMove) {
      // 差分の計算
      var mouse_container_location = ControlToContainer(mouse_control_location);
      int delta_x = mouse_container_location.X - last_mouse_container_location_.X;
      int delta_y = mouse_container_location.Y - last_mouse_container_location_.Y;

      // メンバ変数の更新
      int current_container_x = current_container_location_.X;
      int current_container_y = current_container_location_.Y;
      current_container_location_ = new Point(current_container_x + delta_x,
                                              current_container_y + delta_y);
      last_mouse_container_location_ = mouse_container_location;

      // 必ず境界内に収まるように
      int justified_container_x = current_container_location_.X;
      int justified_container_y = current_container_location_.Y;
      if (is_parent_exists_) {
        justified_container_x = Math.Max(0, justified_container_x);
        justified_container_x = Math.Min(target_.Parent.Width - current_size_.Width, justified_container_x);
        justified_container_y = Math.Max(0, justified_container_y);
        justified_container_y = Math.Min(target_.Parent.Height - current_size_.Height, justified_container_y);
      } else {
        justified_container_x = Math.Max(bounds_.Left, justified_container_x);
        justified_container_x = Math.Min(bounds_.Right - current_size_.Width, justified_container_x);
        justified_container_y = Math.Max(bounds_.Top, justified_container_y);
        justified_container_y = Math.Min(bounds_.Bottom - current_size_.Height, justified_container_y);
      }

      // 実際にコントロールを移動
      target_.Location = new Point(justified_container_x, justified_container_y);
    }
  }

  private void target_MouseUp(object sender, MouseEventArgs e) {
    /// @warning SendMessageの場合はMouseUpを捕まえられないことに注意
    mode_ = Mode.kNop;
  }

  private void target_SizeChanged(object sender, EventArgs e) {
    // 何もしない
    if (mode_ == Mode.kNop || mode_ == Mode.kMove) {
      return;
    } 

    // とりあえず小さくなりすぎないように変更
    bool size_changed = false;
    int new_width = target_.Width;
    int new_height = target_.Height;
    int new_x = target_.Left;
    int new_y = target_.Top;
    if (new_width < kMinimumSize) {
      // 無理やりサイズを増加させた分を計算
      int delta_x = kMinimumSize - new_width;
      new_x = target_.Left - delta_x;
      // 右側をリサイズしているときに右端が境界外へいかないように
      if (mode_ == Mode.kResizeRight || mode_ == Mode.kResizeTopRight || mode_ == Mode.kResizeBottomRight) {
        new_x = Math.Max(new_x, target_.Left);
      }
      new_width = kMinimumSize;
      size_changed = true;
    }
    if (new_height < kMinimumSize) {
      // 無理やりサイズを増加させた分を計算
      int delta_y = kMinimumSize - new_height;
      new_y = target_.Top - delta_y;
      // 下側をリサイズしているときに下端が境界外へいかないように
      if (mode_ == Mode.kResizeBottom || mode_ == Mode.kResizeBottomRight || mode_ == Mode.kResizeBottomLeft) {
        new_y = Math.Max(new_y, target_.Top);
      }
      new_height = kMinimumSize;
      size_changed = true;
    }
    if (size_changed) {
      target_.Location = new Point(new_x, new_y);
      target_.Size = new Size(new_width, new_height);;
    }
  }

  //-------------------------------------------------------------------

  // 座標変換、型変換

  /// @brief ModeからSendMessage用のメッセージに変換する
  ExternalAPI.SystemDefinedMessage GetMessageFromMode(Mode mode) {
    switch (mode) {
    case Mode.kResizeTopLeft:
      return ExternalAPI.SystemDefinedMessage.HTTOPLEFT;
    case Mode.kResizeBottomRight:
      return ExternalAPI.SystemDefinedMessage.HTBOTTOMRIGHT;
    case Mode.kResizeBottomLeft:
      return ExternalAPI.SystemDefinedMessage.HTBOTTOMLEFT;
    case Mode.kResizeTopRight:
      return ExternalAPI.SystemDefinedMessage.HTTOPRIGHT;
    case Mode.kResizeTop:
      return ExternalAPI.SystemDefinedMessage.HTTOP;
    case Mode.kResizeBottom:
      return ExternalAPI.SystemDefinedMessage.HTBOTTOM;
    case Mode.kResizeLeft:
      return ExternalAPI.SystemDefinedMessage.HTLEFT;
    case Mode.kResizeRight:
      return ExternalAPI.SystemDefinedMessage.HTRIGHT;
    case Mode.kMove:
      return ExternalAPI.SystemDefinedMessage.HTTRANSPARENT;
    default:
      return ExternalAPI.SystemDefinedMessage.NULL;
    }
  }

  /// @brief マウスが押された時のコントロール座標が上下左右斜めのどの領域にあるか
  Mode GetModeFromMouseControlLocation(Point control_location) {
    var on_top_border = control_location.Y < kBorder;
    var on_bottom_border = control_location.Y > target_.Height - kBorder;
    var on_left_border = control_location.X < kBorder;
    var on_right_border = control_location.X > target_.Width - kBorder;

    // リサイズ領域ではない場合は移動モード
    // TopよりはBottom、LeftよりはRightを優先すること
    Mode tmp_mode = Mode.kMove;
    if (on_bottom_border && on_right_border) {
      // 優先順位1: 右下端。ここが一番選択しやすいように。
      tmp_mode = Mode.kResizeBottomRight;
    } else if (on_bottom_border && on_left_border) {
      // 優先順位2: 左下端
      tmp_mode = Mode.kResizeBottomLeft;
    } else if (on_top_border && on_right_border) {
      tmp_mode = Mode.kResizeTopRight;
    } else if (on_top_border && on_left_border) {
      tmp_mode = Mode.kResizeTopLeft;
    } else if (on_bottom_border) {
      tmp_mode = Mode.kResizeBottom;
    } else if (on_right_border) {
      tmp_mode = Mode.kResizeRight;
    } else if (on_top_border) {
      tmp_mode = Mode.kResizeTop;
    } else if (on_left_border) {
      tmp_mode = Mode.kResizeLeft;
    }
    return tmp_mode;
  }

  /// @brief モードからカーソルを取得
  Cursor GetCursorFromMode(Mode mode) {
    switch (mode) {
    case Mode.kResizeTopLeft:
    case Mode.kResizeBottomRight:
      return Cursors.SizeNWSE;
    case Mode.kResizeBottomLeft:
    case Mode.kResizeTopRight:
      return Cursors.SizeNESW;
    case Mode.kResizeTop:
    case Mode.kResizeBottom:
      return Cursors.SizeNS;
    case Mode.kResizeLeft:
    case Mode.kResizeRight:
      return Cursors.SizeWE;
    case Mode.kMove:
      return Cursors.SizeAll;
    default:
      return Cursors.Default;
    }
  }

  // 座標系をControlからBoundへ変換する
  Point ControlToContainer(Point control_location) {
    return new Point(target_.Left + control_location.X, target_.Top + control_location.Y);
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  readonly bool is_parent_exists_;

  Rectangle bounds_;

  Mode mode_;
  Point last_mouse_container_location_;

  Point current_container_location_;
  Size current_size_;

  /// @brief 操作の対象となるコントロール
  Control target_;
}
}
