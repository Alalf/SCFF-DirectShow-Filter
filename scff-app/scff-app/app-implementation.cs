
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

using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Text;

namespace scff_app {

/// @brief Form1(メインウィンドウ)から利用する実装クラス
public partial class AppImplementation {

  /// @brief コンストラクタ
  public AppImplementation() {
    can_use_dwmapi_dll_ = false;
    was_dwm_enabled_on_start_ = false;
    interprocess_ = null;

    // DWMAPI.DLLが利用可能かどうか調べる
    if (System.Environment.OSVersion.Platform == System.PlatformID.Win32NT &&
        System.Environment.OSVersion.Version.Major >= 6) {
      can_use_dwmapi_dll_ = true;
    }

    // プロセス間通信に必要なオブジェクトの生成
    interprocess_ = new scff_interprocess.Interprocess();

    // レイアウトパラメータを格納するためのオブジェクトを生成
    LayoutParameters = new List<LayoutParameter>();
    // とりあえず一個追加
    LayoutParameters.Add(new LayoutParameter());

    // リサイズ方法のリストを作成
    BuildResizeMethodList();

    // 編集中のレイアウトインデックス
    EditingLayoutIndex = 0;

    // ディレクトリ取得
    UpdateDirectory();

    // デフォルトの設定を書き込む
    DoCaptureDesktopWindow();
  }

  // enum格納用
  // Tupleの動作が不安定との情報を聞いたのでしょうがなく作った
  public class ResizeMethod {
    public ResizeMethod(string name, scff_interprocess.SWScaleFlags flags) {
      MethodName = name;
      SWScaleFlags = flags;
    }
    public string MethodName { get; set; }
    public scff_interprocess.SWScaleFlags SWScaleFlags { get; set; }
  }

  /// 共有メモリからディレクトリを取得し、いろいろ処理
  public void UpdateDirectory() {
    // 共有メモリからデータを取得
    interprocess_.InitDirectory();
    scff_interprocess.Directory interprocess_directory;
    interprocess_.GetDirectory(out interprocess_directory);

    // リストを新しく作成する
    Directory directory = new Directory(interprocess_directory);
  }
  /// 共有メモリにNullLayoutリクエストを設定
  public void SendNullLayoutRequest() {
    // メッセージを書いて送る
    scff_interprocess.Message message = new scff_interprocess.Message();
    message.timestamp = System.DateTime.Now.Ticks;
    message.layout_type = (int)scff_interprocess.LayoutType.kNullLayout;

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message);
    }
  }
  /// 共有メモリにNativeLayoutリクエストを設定
  public void SendNativeLayoutRequest() {
    // メッセージを書いて送る
    scff_interprocess.Message message = new scff_interprocess.Message();
    message.timestamp = System.DateTime.Now.Ticks;
    message.layout_type = (int)scff_interprocess.LayoutType.kNativeLayout;
    // 無視される
    message.layout_element_count = 1;
    message.layout_parameters = new scff_interprocess.LayoutParameter[scff_interprocess.Interprocess.kMaxComplexLayoutElements];
    message.layout_parameters[0] = LayoutParameters[EditingLayoutIndex].ToInterprocessLayoutParameter();

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message);
    }
  }
  /// 共有メモリにComplexLayoutリクエストを設定
  public void SendComplexLayoutRequest() {
    // todo(Alalf) テスト中！あとで直す！

    // メッセージを書いて送る
    scff_interprocess.Message message = new scff_interprocess.Message();
    message.timestamp = System.DateTime.Now.Ticks;
    message.layout_type = (int)scff_interprocess.LayoutType.kComplexLayout;
    message.layout_element_count = 2;
    // 1個目の取り込み範囲
    message.layout_parameters[0].bound_x = 32;
    message.layout_parameters[0].bound_y = 32;
    message.layout_parameters[0].bound_width = 320;
    message.layout_parameters[0].bound_height = 240;
    message.layout_parameters[0].window = (ulong)(GetDesktopWindow());
    message.layout_parameters[0].clipping_x = 0;
    message.layout_parameters[0].clipping_y = 0;
    message.layout_parameters[0].clipping_width = 1000;
    message.layout_parameters[0].clipping_height = 500;
    message.layout_parameters[0].show_cursor = 0;
    message.layout_parameters[0].show_layered_window = 0;
    message.layout_parameters[0].sws_flags = (int)scff_interprocess.SWScaleFlags.kLanczos;
    message.layout_parameters[0].stretch = 1;
    message.layout_parameters[0].keep_aspect_ratio = 1;
    // 2個目の取り込み範囲
    message.layout_parameters[1].bound_x = 300;
    message.layout_parameters[1].bound_y = 0;
    message.layout_parameters[1].bound_width = 300;
    message.layout_parameters[1].bound_height = 100;
    message.layout_parameters[1].window = (ulong)(GetDesktopWindow());
    message.layout_parameters[1].clipping_x = 320;
    message.layout_parameters[1].clipping_y = 320;
    message.layout_parameters[1].clipping_width = 200;
    message.layout_parameters[1].clipping_height = 200;
    message.layout_parameters[1].show_cursor = 0;
    message.layout_parameters[1].show_layered_window = 0;
    message.layout_parameters[1].sws_flags = (int)scff_interprocess.SWScaleFlags.kLanczos;
    message.layout_parameters[1].stretch = 1;
    message.layout_parameters[1].keep_aspect_ratio = 1;

    // 共有メモリを開いて送る
    if (EditingProcessID != 0) {
      interprocess_.InitMessage(EditingProcessID);
      interprocess_.SendMessage(message);
    }
  }

  //-------------------------------------------------------------------

  /// @brief ResizeMethodコンボボックス用にリストを生成する
  private void BuildResizeMethodList() {
    // リストを新しく作成する
    ResizeMethodList = new List<System.Tuple<string,scff_interprocess.SWScaleFlags>>();
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

  //-------------------------------------------------------------------

  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  private static extern int GetClassName(System.IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
  /// ウィンドウを指定する
  public void SetWindow(System.IntPtr window_handle) {
    LayoutParameters[EditingLayoutIndex].Window = (ulong)window_handle;

    ResetClippingRegion();
  }
  [DllImport("user32.dll", SetLastError = false)]
  static extern System.IntPtr GetDesktopWindow();        /// デスクトップを全画面で取り込む
  public void DoCaptureDesktopWindow() {
    SetWindow(GetDesktopWindow());
  }
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsWindow(System.IntPtr hWnd);
  [StructLayout(LayoutKind.Sequential)]
  struct RECT {
    public int left, top, right, bottom;
  }
  [DllImport("user32.dll")]
  static extern bool GetClientRect(System.IntPtr hWnd, out RECT lpRect);
  /// クリッピング領域をリセットする
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

  /// パラメータのValidate
  public bool ValidateParameters() {
    // もっとも危険な状態になりやすいウィンドウからチェック
    if (LayoutParameters[EditingLayoutIndex].Window == 0) { // NULL
      MessageBox.Show("Specified window is invalid", "Invalid Window",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      return false;
    }
    var window_handle = (System.IntPtr)(LayoutParameters[EditingLayoutIndex].Window);
    if (!IsWindow(window_handle)) {
      MessageBox.Show("Specified window is invalid", "Invalid Window",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
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
      MessageBox.Show("Clipping region is invalid", "Invalid Clipping Region",
          MessageBoxButtons.OK, MessageBoxIcon.Error);
      return false;
    }

    return true;
  }

  /// @brief 現在編集中のレイアウト番号
  public int EditingLayoutIndex { get; set; }

  /// @brief 現在選択中のプロセス番号
  public System.UInt32 EditingProcessID { get; set; }

  /// @brief ResizeMethodコンボボックス用リスト
  public List<System.Tuple<string,scff_interprocess.SWScaleFlags>> ResizeMethodList { get; set; }

  /// @brief アプリケーションが管理するレイアウト変数
  public List<LayoutParameter> LayoutParameters { get; set; }

  /// @brief プロセス間通信用オブジェクト
  private scff_interprocess.Interprocess interprocess_;
}
}   // namespace scff_app