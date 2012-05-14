using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace scff_app.forms {

/// @brief コントロールに対し、ドラッグでの移動/リサイズ機能を付加するためのクラス
// メモ:
//  座標系はいくつかあるが、注意しないといけない点
//  ControlLocation:      これはTarget内の座標系を意味する
//  ContainerLocation:    これはTargetを内包するContainer内の座標系を意味する
//  なお、サイズ(幅、高さ)は無次元数なので特に区別をする必要はない。
public class MovableAndResizable : IDisposable {

  //-------------------------------------------------------------------
  // Wrapper
  //-------------------------------------------------------------------
  [DllImport("user32.dll")]
  public static extern bool ReleaseCapture();
  [DllImport("user32.dll", CharSet = CharSet.Auto)]
  public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
  private const uint WM_NCLBUTTONDOWN = 0x00A1;
  private enum Message {
    NULL          = 0,
    HTTRANSPARENT = -1,
    HTLEFT        = 10,
    HTRIGHT       = 11,
    HTTOP         = 12,
    HTTOPLEFT     = 13,
    HTTOPRIGHT    = 14,
    HTBOTTOM      = 15,
    HTBOTTOMLEFT  = 16,
    HTBOTTOMRIGHT = 17,
  }
  //-------------------------------------------------------------------

  /// @brief リサイズとする領域のサイズ
  private const int kBorder = 16;

  /// @brief 最小サイズ
  private const int kMinimumSize = kBorder * 3;

  /// @brief 現在の状態を表す定数
  private enum Mode {
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

  /// @brief コンストラクタ
  public MovableAndResizable(Control target, int bound_width, int bound_height) {
    target_ = target;
    bound_width_ = bound_width;
    bound_height_ = bound_height;

    // イベントハンドラの登録
    target_.MouseDown += target_MouseDown;
    target_.MouseMove += target_MouseMove;
    target_.MouseUp += target_MouseUp;
    target_.SizeChanged += target_SizeChanged;
  }

  /// @brief Dispose（GCを考慮したデストラクタ）
  public void Dispose() {
    // イベントハンドラの登録解除
    target_.MouseDown -= target_MouseDown;
    target_.MouseMove -= target_MouseMove;
    target_.MouseUp -= target_MouseUp;
    target_.SizeChanged -= target_SizeChanged;
  }

  //-------------------------------------------------------------------
  // イベントハンドラ
  //-------------------------------------------------------------------

  private void target_MouseDown(object sender, MouseEventArgs e) {
    var mouse_down_control_location = e.Location;

    mode_ = GetModeFromMouseControlLocation(mouse_down_control_location);
    last_mouse_container_location_ = ControlToContainer(mouse_down_control_location);
    current_container_location_ = target_.Location;
    current_size_ = target_.Size;

    if (mode_ != Mode.kNop && mode_ != Mode.kMove) {
      ReleaseCapture();
      Message message = GetMessageFromMode(mode_);
      SendMessage(target_.Handle, WM_NCLBUTTONDOWN, new IntPtr((int)message), IntPtr.Zero);
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
      justified_container_x = Math.Max(0, justified_container_x);
      justified_container_x = Math.Min(bound_width_ - current_size_.Width, justified_container_x);
      justified_container_y = Math.Max(0, justified_container_y);
      justified_container_y = Math.Min(bound_height_ - current_size_.Height, justified_container_y);

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
  //-------------------------------------------------------------------

  /// @brief ModeからSendMessage用のメッセージに変換する
  private Message GetMessageFromMode(Mode mode) {
    switch (mode) {
    case Mode.kResizeTopLeft:
      return Message.HTTOPLEFT;
    case Mode.kResizeBottomRight:
      return Message.HTBOTTOMRIGHT;
    case Mode.kResizeBottomLeft:
      return Message.HTBOTTOMLEFT;
    case Mode.kResizeTopRight:
      return Message.HTTOPRIGHT;
    case Mode.kResizeTop:
      return Message.HTTOP;
    case Mode.kResizeBottom:
      return Message.HTBOTTOM;
    case Mode.kResizeLeft:
      return Message.HTLEFT;
    case Mode.kResizeRight:
      return Message.HTRIGHT;
    case Mode.kMove:
      return Message.HTTRANSPARENT;
    default:
      return Message.NULL;
    }
  }

  /// @brief マウスが押された時のコントロール座標が上下左右斜めのどの領域にあるか
  private Mode GetModeFromMouseControlLocation(Point control_location) {
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
  private Cursor GetCursorFromMode(Mode mode) {
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
  private Point ControlToContainer(Point control_location) {
    return new Point(target_.Left + control_location.X, target_.Top + control_location.Y);
  }

  //-------------------------------------------------------------------
  // メンバ変数
  //-------------------------------------------------------------------

  private int bound_width_;
  private int bound_height_;

  private Mode mode_;
  private Point last_mouse_container_location_;

  private Point current_container_location_;
  private Size current_size_;

  /// @brief 操作の対象となるコントロール
  private Control target_;
}
}
