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

namespace scff_app {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

/// @brief Form1(メインウィンドウ)から利用する実装クラス
partial class AppImplementation {

  //-------------------------------------------------------------------
  // Directory
  //-------------------------------------------------------------------

  /// @brief 現在のディレクトリ
  data.Directory directory_ = new data.Directory();
  public data.Directory Directory {
    get { return directory_; }
  }
  public void UpdateDirectory(ref BindingSource entries) {
    directory_.Update(ref interprocess_);
    directory_.UpdateBindingSource(ref entries);
  }

  //-------------------------------------------------------------------
  // Message
  //-------------------------------------------------------------------

  /// @brief 現在編集中のメッセージ
  data.Message message_ = new data.Message();
  public data.Message Message {
    get { return message_; }
  }

  public void SendNull(ref BindingSource entries, bool show_message) {
    message_.Reset();
    message_.Send(ref interprocess_, ref entries, show_message);
  }

  public void SendMessage(ref BindingSource entries,
                          ref BindingSource layout_parameters,
                          bool show_message) {
    message_.Uppate(ref layout_parameters);
    if (!message_.Validate(show_message)) {
      return;
    }

    message_.Send(ref interprocess_, ref entries, show_message);
  }
  
  //-------------------------------------------------------------------
  // Interprocess
  //-------------------------------------------------------------------

  /// @brief プロセス間通信用
  scff_interprocess.Interprocess interprocess_ = new scff_interprocess.Interprocess();
  public scff_interprocess.Interprocess Interprocess {
    get { return interprocess_; }
  }

  //-------------------------------------------------------------------
  // ウィンドウ指定
  //-------------------------------------------------------------------

  public void SetDesktopWindow(ref BindingSource layout_parameters) {
    ((data.LayoutParameter)layout_parameters.Current).SetWindowFromPtr(ExternalAPI.GetDesktopWindow(), true);
  }
  public void SetWindowFromPoint(ref BindingSource layout_parameters, int screen_x, int screen_y) {
    UIntPtr window = ExternalAPI.WindowFromPoint(screen_x, screen_y);
    ((data.LayoutParameter)layout_parameters.Current).SetWindowFromPtr(window, true);
  }

  //-------------------------------------------------------------------

  /// @brief コンストラクタ
  public AppImplementation() {
    // nop
  }

  /// @brief 起動時のチェックを行うメソッド
  public bool CheckEnvironment() {
    // GUID
    const string kGUID = "D64DB8AA-9055-418F-AFE9-A080A4FAE47A";
    // Registry Key
    const string kRegistryKey = "CLSID\\{" + kGUID + "}";

    //------------------
    // 32bit版のチェック
    //------------------
    bool is_correctly_installed_x86 = false;
    bool is_dll_found_x86 = false;
    string dll_path_x86 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry32).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_x86 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_x86 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_x86)) {
        is_dll_found_x86 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //------------------
    // 64bit版のチェック
    //------------------
    bool is_correctly_installed_amd64 = false;
    bool is_dll_found_amd64 = false;
    string dll_path_amd64 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry64).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_amd64 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_amd64 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_amd64)) {
        is_dll_found_amd64 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //----------------------
    // エラーダイアログの表示
    // （若干不正確だがないよりましだろう）
    //----------------------
    if (!is_correctly_installed_x86 && !is_correctly_installed_amd64) {
      // 32bit版も64bit版もインストールされていない場合
      MessageBox.Show("scff-*.ax is not correctly installed.\nPlease re-install SCFF DirectShow Filter.",
                      "Not correctly installed",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return false;
    }

    if (!is_dll_found_x86 && !is_dll_found_amd64) {
      // 32bit版のDLLも64bit版のDLLも指定された場所に存在していない場合
      string message = "scff-*.ax is not found:\n";
      message += "\n";
      message += "  32bit: " + dll_path_x86 + "\n";
      message += "  64bit: " + dll_path_amd64 + "\n"; 
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
  // ウィンドウ指定
  //-------------------------------------------------------------------

  /// @brief クリッピング領域を適切な値に調整してから設定
  public void JustifyClippingRegion(
      int bound_width, int bound_height,
      int src_x, int src_y, int src_width, int src_height,
      out int clipping_x, out int clipping_y,
      out int clipping_width, out int clipping_height) {
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
    if (src_x > bound_width) {
      dst_x = bound_width - src_width;
    }
    if (src_y > bound_height) {
      dst_y = bound_height - src_height;
    }

    if (dst_x + dst_width > bound_width) {
      dst_width = bound_width - dst_x;
    }
    if (dst_y + dst_height > bound_height) {
      dst_height = bound_height - dst_y;
    }

    clipping_x = dst_x;
    clipping_y = dst_y;
    clipping_width = dst_width;
    clipping_height = dst_height;
  }
}
}   // namespace scff_app