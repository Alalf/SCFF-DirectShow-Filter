
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

/// @file scff-app/form1-scff.cs
/// @brief Form1(メインウィンドウ)のイベントハンドラ以外の定義

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace scff_app {

/// @brief Form1(メインウィンドウ)から利用する実装クラス
public partial class AppImplementation {
  //-------------------------------------------------------------------
  // Wrappers
  //-------------------------------------------------------------------
  [StructLayout(LayoutKind.Sequential)]
  struct RECT { public int left, top, right, bottom; }
  [DllImport("user32.dll", SetLastError = false)]
  static extern System.IntPtr GetDesktopWindow();
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsWindow(System.IntPtr hWnd);
  [DllImport("user32.dll")]
  static extern bool GetClientRect(System.IntPtr hWnd, out RECT lpRect);
  [DllImport("user32.dll")]
  private static extern System.IntPtr WindowFromPoint(int xPoint, int yPoint);
  //-------------------------------------------------------------------

  /// @brief コンストラクタ
  public AppImplementation() {
    // 初期化
    interprocess_ = null;

    // プロセス間通信に必要なオブジェクトの生成
    interprocess_ = new scff_interprocess.Interprocess();

    // レイアウトパラメータを格納するためのオブジェクトを生成
    LayoutParameters = new List<LayoutParameter>();

    // ディレクトリを用意しておく
    CurrentDirectory = new Directory();

    // リサイズ方法のリストを作成
    BuildResizeMethodList();

    // 編集中のレイアウトインデックス
    EditingLayoutIndex = 0;

    // デフォルトの設定を書き込む
    LayoutParameter layout_parameter = new LayoutParameter();
    LayoutParameters.Add(layout_parameter);
    SetWindowToDesktop();
    ResetClippingRegion();
  }

  /// @brief ResizeMethodコンボボックス用にリストを生成する
  private void BuildResizeMethodList() {
    // リストを新しく作成する
    ResizeMethodList = new List<System.Tuple<string, scff_interprocess.SWScaleFlags>>();
    ResizeMethodList.Add(System.Tuple.Create("FastBilinear (fast bilinear)", scff_interprocess.SWScaleFlags.kFastBilinear));
    ResizeMethodList.Add(System.Tuple.Create("Bilinear (bilinear)", scff_interprocess.SWScaleFlags.kBilinear));
    ResizeMethodList.Add(System.Tuple.Create("Bicubic (bicubic)", scff_interprocess.SWScaleFlags.kBicubic));
    ResizeMethodList.Add(System.Tuple.Create("X (experimental)", scff_interprocess.SWScaleFlags.kX));
    ResizeMethodList.Add(System.Tuple.Create("Point (nearest neighbor)", scff_interprocess.SWScaleFlags.kPoint));
    ResizeMethodList.Add(System.Tuple.Create("Area (averaging area)", scff_interprocess.SWScaleFlags.kArea));
    ResizeMethodList.Add(System.Tuple.Create("Bicublin (luma bicubic, chroma bilinear)", scff_interprocess.SWScaleFlags.kBicublin));
    ResizeMethodList.Add(System.Tuple.Create("Gauss (gaussian)", scff_interprocess.SWScaleFlags.kGauss));
    ResizeMethodList.Add(System.Tuple.Create("Sinc (sinc)", scff_interprocess.SWScaleFlags.kSinc));
    ResizeMethodList.Add(System.Tuple.Create("Lanczos (natural)", scff_interprocess.SWScaleFlags.kLanczos));
    ResizeMethodList.Add(System.Tuple.Create("Spline (natural bicubic spline)", scff_interprocess.SWScaleFlags.kSpline));
  }

  /// @brief 共有メモリからディレクトリを取得し、CurrentDirectoryを更新
  public void UpdateCurrentDirectory() {
    // 共有メモリからデータを取得
    interprocess_.InitDirectory();
    scff_interprocess.Directory interprocess_directory;
    interprocess_.GetDirectory(out interprocess_directory);

    // リストを新しく作成する
    CurrentDirectory = new Directory(interprocess_directory);
  }

  //-------------------------------------------------------------------
  // ウィンドウ指定
  //-------------------------------------------------------------------

  /// @brief スクリーン座標からウィンドウハンドルを設定する
  public void SetWindowFromPoint(int screen_x, int screen_y) {
    System.Diagnostics.Trace.WriteLine("Cursor: " + screen_x + "," + screen_y);
    System.IntPtr window_handle = WindowFromPoint(screen_x, screen_y);
    if (window_handle != null) {
      // 見つかった場合
      LayoutParameters[EditingLayoutIndex].Window = (System.UInt64)window_handle;
    } else {
      // nop
    }
  }

  /// @brief デスクトップを全画面で取り込む
  public void SetWindowToDesktop() {
    LayoutParameters[EditingLayoutIndex].Window = (System.UInt64)GetDesktopWindow();
  }

  /// @brief クリッピング領域をリセットする
  public void ResetClippingRegion() {
    System.IntPtr window_handle =
        (System.IntPtr)LayoutParameters[EditingLayoutIndex].Window;
    if (window_handle == null || !IsWindow(window_handle)) {
      return;
    }

    RECT window_rect;
    GetClientRect(window_handle, out window_rect);
    LayoutParameters[EditingLayoutIndex].ClippingX = window_rect.left;
    LayoutParameters[EditingLayoutIndex].ClippingY = window_rect.top;
    LayoutParameters[EditingLayoutIndex].ClippingWidth = window_rect.right;
    LayoutParameters[EditingLayoutIndex].ClippingHeight = window_rect.bottom;
  }

  /// @brief クリッピング領域を適切な値に調整してから設定
  public void JustifyClippingRegion(int src_x, int src_y, int src_width, int src_height) {
    System.IntPtr window_handle =
        (System.IntPtr)LayoutParameters[EditingLayoutIndex].Window;
    if (window_handle == null || !IsWindow(window_handle)) {
      return;
    }

    RECT window_rect;
    GetClientRect(window_handle, out window_rect);

    int dst_x = src_x;
    int dst_y = src_y;
    int dst_width = src_width;
    int dst_height = src_height;
    if (src_x < 0) {
      dst_width += src_x;
      dst_x = 0;
    }
    if (src_y < 0) {
      dst_height += src_y;
      dst_y = 0;
    }
    if (src_x > window_rect.right) {
      dst_x = window_rect.right - src_width;
    }
    if (src_y > window_rect.bottom) {
      dst_y = window_rect.bottom - src_height;
    }

    if (dst_x + dst_width > window_rect.right) {
      dst_width = window_rect.right - dst_x;
    }
    if (dst_y + dst_height > window_rect.bottom) {
      dst_height = window_rect.bottom - dst_y;
    }

    LayoutParameters[EditingLayoutIndex].ClippingX = dst_x;
    LayoutParameters[EditingLayoutIndex].ClippingY = dst_y;
    LayoutParameters[EditingLayoutIndex].ClippingWidth = dst_width;
    LayoutParameters[EditingLayoutIndex].ClippingHeight = dst_height;
  }

  /// @brief パラメータのValidate
  public bool ValidateParameters(bool show_message) {
    // もっとも危険な状態になりやすいウィンドウからチェック
    if (LayoutParameters[EditingLayoutIndex].Window == 0) { // NULL
      if (show_message) {
        MessageBox.Show("Specified window is invalid", "Invalid Window",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }
    var window_handle = (System.IntPtr)(LayoutParameters[EditingLayoutIndex].Window);
    if (!IsWindow(window_handle)) {
      if (show_message) {
        MessageBox.Show("Specified window is invalid", "Invalid Window",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }

    // クリッピングリージョンの判定
    RECT window_rect;
    GetClientRect(window_handle, out window_rect);
    if (LayoutParameters[EditingLayoutIndex].ClippingX +
        LayoutParameters[EditingLayoutIndex].ClippingWidth
        <= window_rect.right &&
        LayoutParameters[EditingLayoutIndex].ClippingY +
        LayoutParameters[EditingLayoutIndex].ClippingHeight
        <= window_rect.bottom &&
        LayoutParameters[EditingLayoutIndex].ClippingWidth > 0 &&
        LayoutParameters[EditingLayoutIndex].ClippingHeight > 0) {
      // nop 問題なし
    } else {
      if (show_message) {
        MessageBox.Show("Clipping region is invalid", "Invalid Clipping Region",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return false;
    }

    return true;
  }

  //-------------------------------------------------------------------

  /// @brief 共有メモリにNullLayoutリクエストを設定
  public void SendNullLayoutRequest() {
    // メッセージを書いて送る
    Message message = new Message();
    message.LayoutType = scff_interprocess.LayoutType.kNullLayout;

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message.ToInterprocessMessage());
    }
  }

  /// @brief 共有メモリにLayoutリクエストを設定
  public void SendLayoutRequest() {
    if (LayoutParameters.Count == 0) {
      SendNullLayoutRequest();
    } else if (LayoutParameters.Count == 1) {
      SendNativeLayoutRequest();
    } else {
      SendComplexLayoutRequest();
    }
  }

  /// @brief 共有メモリにNativeLayoutリクエストを設定
  private void SendNativeLayoutRequest() {
    // メッセージを書いて送る
    Message message = new Message();
    message.LayoutType = scff_interprocess.LayoutType.kNativeLayout;
    message.LayoutElementCount = 1;
    message.LayoutParameters.Add(LayoutParameters[EditingLayoutIndex]);

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message.ToInterprocessMessage());
    }
  }

  /// @brief 共有メモリにComplexLayoutリクエストを設定
  private void SendComplexLayoutRequest() {
    // メッセージを書いて送る
    Message message = new Message();
    message.LayoutType = scff_interprocess.LayoutType.kComplexLayout;
    message.LayoutElementCount = LayoutParameters.Count;

    foreach (LayoutParameter i in LayoutParameters) {
      message.LayoutParameters.Add(i);
    }

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message.ToInterprocessMessage());
    }
  }

  //-------------------------------------------------------------------

  /// @brief 現在編集中のレイアウト番号
  public int EditingLayoutIndex { get; set; }

  /// @brief 現在選択中のプロセス番号
  public System.UInt32 EditingProcessID { get; set; }

  /// @brief ResizeMethodコンボボックス用リスト
  public List<System.Tuple<string,scff_interprocess.SWScaleFlags>> ResizeMethodList { get; private set; }

  /// @brief アプリケーションが管理するレイアウト変数
  public List<LayoutParameter> LayoutParameters { get; set; }

  /// @brief 最後に取得したディレクトリ
  public Directory CurrentDirectory { get; private set; }

  /// @brief プロセス間通信用オブジェクト
  private scff_interprocess.Interprocess interprocess_;
}
}   // namespace scff_app