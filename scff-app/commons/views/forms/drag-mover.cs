using System;
using System.Drawing;
using System.Windows.Forms;

namespace scff_app.Commons.Views.Forms {

/// @brief コントロールに対し、ドラッグでの移動/リサイズ機能を付加するためのクラス
/// @attention 座標系がレイアウトフォームとプレビュー領域に分散しないように注意する
/// @attention マウスのlocationはプレビュー領域の座標系なのでGetTargetsLocationメソッドで変換する
// メモ:
//  座標系はいくつかあるが、注意しないといけない点
//  ControlLocation:      これはTarget内の座標系を意味する
//  ContainerLocation:    これはTargetを内包するContainer内の座標系を意味する
//  なお、サイズ(幅、高さ)は無次元数なので特に区別をする必要はない。
public class DragMover : IDisposable {

  //-------------------------------------------------------------------
  // 定数
  //-------------------------------------------------------------------

  /// @brief リサイズとする領域のサイズ
  private const int kBorder = 16;
  /// @brief 最小サイズ
  private const int kMinimumSize = kBorder * 2;

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
  public DragMover(Control target, int bound_width, int bound_height) {
    target_ = target;
    bound_width_ = bound_width;
    bound_height_ = bound_height;

    // イベントハンドラの登録
    target_.MouseDown += target_MouseDown;
    target_.MouseMove += target_MouseMove;
    target_.MouseUp += target_MouseUp;
  }

  /// @brief Dispose（GCを考慮したデストラクタ）
  public void Dispose() {
    // イベントハンドラの登録解除
    target_.MouseDown -= target_MouseDown;
    target_.MouseMove -= target_MouseMove;
    target_.MouseUp -= target_MouseUp;
  }

  //-------------------------------------------------------------------
  // イベントハンドラ
  //-------------------------------------------------------------------

  private void target_MouseDown(object sender, MouseEventArgs e) {
    var mouse_down_control_location = e.Location;
    mode_ = GetModeFromControlLocation(mouse_down_control_location);
    last_mouse_container_location_ = ControlToContainer(mouse_down_control_location);
    start_rect_in_container_ = new Rectangle(target_.Location, target_.Size);
    current_container_location_ = target_.Location;
    current_size_ = target_.Size;
  }

  private void target_MouseMove(object sender, MouseEventArgs e) {
    var mouse_control_location = e.Location;
    if (mode_ == Mode.kNop) {
      target_.Cursor = GetCursorFromMode(GetModeFromControlLocation(mouse_control_location));
      return;
    }

    var mouse_container_location = ControlToContainer(mouse_control_location);
    UpdateCurrentRectInContainer(mouse_container_location);

    var rect = JustifyRectInContainer(current_container_location_, current_size_, mode_);
    target_.Size = rect.Size;
    target_.Location = rect.Location;
  }

  private void target_MouseUp(object sender, MouseEventArgs e) {
    mode_ = Mode.kNop;
  }

  //-------------------------------------------------------------------
  // 定数
  //-------------------------------------------------------------------

  private bool IsLeft(Mode mode) {
    return mode == Mode.kResizeTopLeft ||
           mode == Mode.kResizeLeft ||
           mode == Mode.kResizeBottomLeft;
  }
  private bool IsTop(Mode mode) {
    return mode == Mode.kResizeTopLeft ||
           mode == Mode.kResizeTop ||
           mode == Mode.kResizeTopRight;
  }
  private bool IsBottom(Mode mode) {
    return mode == Mode.kResizeBottomLeft ||
           mode == Mode.kResizeBottom ||
           mode == Mode.kResizeBottomRight;
  }
  private bool IsRight(Mode mode) {
    return mode == Mode.kResizeTopRight ||
           mode == Mode.kResizeRight ||
           mode == Mode.kResizeBottomRight;
  }

  private void UpdateCurrentRectInContainer(Point container_location) {
    int current_container_x = current_container_location_.X;
    int current_container_y = current_container_location_.Y;
    int current_width = current_size_.Width;
    int current_height = current_size_.Height;

    // 真ん中
    if (mode_ == Mode.kMove) {
      current_container_x += container_location.X - last_mouse_container_location_.X;
      current_container_y += container_location.Y - last_mouse_container_location_.Y;
    } else {
      // 左
      if (IsLeft(mode_)) {
        current_container_x += container_location.X - last_mouse_container_location_.X;
        current_width -= container_location.X - last_mouse_container_location_.X;
      }
      // 上
      if (IsTop(mode_)) {
        current_container_y += container_location.Y - last_mouse_container_location_.Y;
        current_height -= container_location.Y - last_mouse_container_location_.Y;
      }
      // 下
      if (IsBottom(mode_)) {
        current_height += container_location.Y - last_mouse_container_location_.Y;
      }
      // 右
      if (IsRight(mode_)) {
        current_width += container_location.X - last_mouse_container_location_.X;
      }
    }
    current_container_location_ = new Point(current_container_x, current_container_y);
    current_size_ = new Size(current_width, current_height);

    // 与えられた座標が領域内なら更新
    last_mouse_container_location_ = container_location;
  }

  private Rectangle JustifyRectInContainer(Point container_location, Size size, Mode mode) {
    int new_container_x = container_location.X;
    int new_container_y = container_location.Y;
    int new_width = size.Width;
    int new_height = size.Height;

    // 真ん中
    if (mode == Mode.kMove) {
      new_container_x = Math.Max(0, new_container_x);
      new_container_x = Math.Min(bound_width_ - new_width, new_container_x);
      new_container_y = Math.Max(0, new_container_y);
      new_container_y = Math.Min(bound_height_ - new_height, new_container_y);
      return new Rectangle(new_container_x, new_container_y, new_width, new_height);
    }

    // 左
    if (IsLeft(mode) && size.Width < kMinimumSize) {
      new_width = kMinimumSize;
      new_container_x = start_rect_in_container_.X + start_rect_in_container_.Width - kMinimumSize;
    }
    // 上
    if (IsTop(mode) && size.Height < kMinimumSize) {
      new_height = kMinimumSize;
      new_container_y = start_rect_in_container_.Y + start_rect_in_container_.Height - kMinimumSize;
    }
    // 下
    if (IsBottom(mode) && size.Height < kMinimumSize) {
      new_height = kMinimumSize;
    }
    // 右
    if (IsRight(mode) && size.Width < kMinimumSize) {
      new_width = kMinimumSize;
    }

    new_container_x = Math.Max(0, new_container_x);
    new_container_x = Math.Min(bound_width_ - new_width, new_container_x);
    new_container_y = Math.Max(0, new_container_y);
    new_container_y = Math.Min(bound_height_ - new_height, new_container_y);

    return new Rectangle(new_container_x, new_container_y, new_width, new_height);
  }

  /// @brief locationがターゲットコントロールの上下左右斜めのどの領域にあるか
  private Mode GetModeFromControlLocation(Point control_location) {
    var on_top_border = control_location.Y < kBorder;
    var on_bottom_border = control_location.Y > target_.Height - kBorder;

    var on_left_border = control_location.X < kBorder;
    var on_right_border = control_location.X > target_.Width - kBorder;

    // リサイズ領域ではない場合は移動モード
    Mode tmp_mode = Mode.kMove;
    if (on_top_border && on_left_border) {
      tmp_mode = Mode.kResizeTopLeft;
    } else if (on_top_border && on_right_border) {
      tmp_mode = Mode.kResizeTopRight;
    } else if (on_bottom_border && on_left_border) {
      tmp_mode = Mode.kResizeBottomLeft;
    } else if (on_bottom_border && on_right_border) {
      tmp_mode = Mode.kResizeBottomRight;
    } else if (on_top_border) {
      tmp_mode = Mode.kResizeTop;
    } else if (on_bottom_border) {
      tmp_mode = Mode.kResizeBottom;
    } else if (on_left_border) {
      tmp_mode = Mode.kResizeLeft;
    } else if (on_right_border) {
      tmp_mode = Mode.kResizeRight;
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
  private Rectangle start_rect_in_container_;
  private Point current_container_location_;
  private Size current_size_;

  /// @brief 操作の対象となるコントロール
  private Control target_;
}
}
