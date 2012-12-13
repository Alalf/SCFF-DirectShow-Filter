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

/// @file scff-app/scff-app.cs
/// MVCパターンにおけるControllerの定義

namespace scff_app {

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

/// MVCパターンにおけるController
partial class SCFFApp {

  // 定数
  public const int kDefaultBoundWidth = 640;
  public const int kDefaultBoundHeight = 360;
  public const int kMaxLayoutElements = scff_interprocess.Interprocess.kMaxComplexLayoutElements;
  const string kSCFFSourceGUID = "D64DB8AA-9055-418F-AFE9-A080A4FAE47A";
  const string kRegistryKey = "CLSID\\{" + kSCFFSourceGUID + "}";

  /// ResizeMethodコンボボックス用リスト
  static SortedList<scff_interprocess.SWScaleFlags, string> kResizeMethodSortedList =
      new SortedList<scff_interprocess.SWScaleFlags, string> {
    {scff_interprocess.SWScaleFlags.kFastBilinear, "FastBilinear (fast bilinear)"},
    {scff_interprocess.SWScaleFlags.kBilinear, "Bilinear (bilinear)"},
    {scff_interprocess.SWScaleFlags.kBicubic, "Bicubic (bicubic)"},
    {scff_interprocess.SWScaleFlags.kX, "X (experimental)"},
    {scff_interprocess.SWScaleFlags.kPoint, "Point (nearest neighbor)"},
    {scff_interprocess.SWScaleFlags.kArea, "Area (averaging area)"},
    {scff_interprocess.SWScaleFlags.kBicublin, "Bicublin (luma bicubic, chroma bilinear)"},
    {scff_interprocess.SWScaleFlags.kGauss, "Gauss (gaussian)"},
    {scff_interprocess.SWScaleFlags.kSinc, "Sinc (sinc)"},
    {scff_interprocess.SWScaleFlags.kLanczos, "Lanczos (lanczos)"},
    {scff_interprocess.SWScaleFlags.kSpline, "Spline (natural bicubic spline)"}
  };

  //-------------------------------------------------------------------

  /// コンストラクタ
  public SCFFApp(BindingSource entries, BindingSource layout_parameters) {
    interprocess_ = new scff_interprocess.Interprocess();
    directory_ = new viewmodel.Directory();
    message_ = new viewmodel.Message();

    // データソースを設定
    entries_ = entries;
    layout_parameters_ = layout_parameters;
    entries.DataSource = directory_.Entries;
    layout_parameters.DataSource = message_.LayoutParameters;

    resize_method_list_ =
        new List<KeyValuePair<scff_interprocess.SWScaleFlags,string>>(kResizeMethodSortedList);

    profiles_path_ = Application.UserAppDataPath + "\\profiles\\";
    profile_list_ = new BindingList<string>();
    known_types_ = new List<Type> {
      typeof(viewmodel.LayoutParameter)
    };
  }

  /// メインフォームのLoad時に呼ばれることを想定
  public void OnLoad() {
    // ディレクトリとBindingSourceを共有メモリから更新
    UpdateDirectory();

    // プロファイルリストを読み込む
    UpdateProfileList();
    LoadDefaultProfile();
  }

  /// 起動時のチェックを行うメソッド
  public bool CheckEnvironment() {
    //------------------
    // 32bit版のチェック
    //------------------
    bool is_correctly_installed_Win32 = false;
    bool is_dll_found_Win32 = false;
    string dll_path_Win32 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry32).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_Win32 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_Win32 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_Win32)) {
        is_dll_found_Win32 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //------------------
    // 64bit版のチェック
    //------------------
    bool is_correctly_installed_x64 = false;
    bool is_dll_found_x64 = false;
    string dll_path_x64 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry64).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_x64 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_x64 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_x64)) {
        is_dll_found_x64 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //----------------------
    // エラーダイアログの表示
    // （若干不正確だがないよりましだろう）
    //----------------------
    if (!is_correctly_installed_Win32 && !is_correctly_installed_x64) {
      // 32bit版も64bit版もインストールされていない場合
      MessageBox.Show("scff-*.ax is not correctly installed.\nPlease re-install SCFF DirectShow Filter.",
                      "Not correctly installed",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return false;
    }

    if (!is_dll_found_Win32 && !is_dll_found_x64) {
      // 32bit版のDLLも64bit版のDLLも指定された場所に存在していない場合
      string message = "scff-*.ax is not found:\n";
      message += "\n";
      message += "  32bit: " + dll_path_Win32 + "\n";
      message += "  64bit: " + dll_path_x64 + "\n"; 
      message += "\n";
      message += "Check your SCFF directory.";
      MessageBox.Show(message,
                      "DLL is not found",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return false;
    }

    //------------------
    // カラーチェック
    //------------------
    if (Screen.PrimaryScreen.BitsPerPixel != 32) {
      MessageBox.Show("SCFF requires primary screen is configured 32bit color mode.",
                      "Not 32bit color mode",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
      return false;
    }

    // 起動OK
    return true;
  }

  //-------------------------------------------------------------------
  // UI要素作成用
  //-------------------------------------------------------------------

  public List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> ResizeMethodList {
    get { return resize_method_list_; }
  }

  //-------------------------------------------------------------------
  // Directory
  //-------------------------------------------------------------------

  public void UpdateDirectory() {
    // 共有メモリにアクセス
    interprocess_.InitDirectory();
    scff_interprocess.Directory interprocess_directory;
    interprocess_.GetDirectory(out interprocess_directory);

    // Directoryに設定
    directory_.LoadFromInterprocess(interprocess_directory);
  }

  //-------------------------------------------------------------------
  // Message
  //-------------------------------------------------------------------

  void Send(bool show_message, bool force_null_layout) {
    if (entries_.Count == 0) {
      // 書き込み先が存在しない
      if (show_message) {
        MessageBox.Show("No process to send message.");
      }
      return;
    }

    viewmodel.Entry current_entry = (viewmodel.Entry)entries_.Current;

    try {
      /// @warning DWORD->int変換！オーバーフローの可能性あり
      Process.GetProcessById((int)current_entry.ProcessID);
    } catch {
      // プロセスが存在しない場合
      if (show_message) {
        MessageBox.Show("Cannot find process(" + current_entry.ProcessID + ").");
      }
      return;
    }

    // Messageを変換
    scff_interprocess.Message interprocess_message =
        message_.ToInterprocess(current_entry.SampleWidth, current_entry.SampleHeight, force_null_layout);
    
    // 共有メモリにアクセス
    interprocess_.InitMessage(current_entry.ProcessID);
    interprocess_.SendMessage(interprocess_message);
  }

  public void SendNull(bool show_message) {
    Send(show_message, true);
  }

  public void SendMessage(bool show_message) {
    if (!message_.IsValid()) {
      if (show_message) {
        MessageBox.Show(message_.Error, "Layout Parameters Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return;
    }
    Send(show_message, false);
  }

  //-------------------------------------------------------------------
  // Window
  //-------------------------------------------------------------------

  public void SetDesktopWindow() {
    ((viewmodel.LayoutParameter)layout_parameters_.Current).SetPrimaryDesktopWindow();
  }

  public void SetWindowFromPoint(int screen_x, int screen_y) {
    UIntPtr window = ExternalAPI.WindowFromPoint(screen_x, screen_y);
    ((viewmodel.LayoutParameter)layout_parameters_.Current).SetWindow(window);
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  scff_interprocess.Interprocess interprocess_;
  viewmodel.Directory directory_;
  viewmodel.Message message_;

  BindingSource layout_parameters_;
  BindingSource entries_;

  List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> resize_method_list_;
}
}   // namespace scff_app