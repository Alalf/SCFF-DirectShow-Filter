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

/// @file Program.cs
/// @brief アプリケーションのエントリポイント

using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace scff_app {

/// @brief アプリケーションのエントリポイントを格納するクラス
static class Program {
  /// @brief アプリケーションのエントリポイント
  [STAThread]
  static void Main() {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

    //-----------------------------------------------------------------
    // 起動時のチェック
    //-----------------------------------------------------------------

    // 現在のOSが32bitが64bitかを判定する
    /// @attention XP SP3以降でないと使えない
    RegistryView registry_view;
    if (Environment.Is64BitOperatingSystem) {
      registry_view = RegistryView.Registry64;
    } else {
      registry_view = RegistryView.Registry32;
    }

    // GUID
    const string kGUID = "D64DB8AA-9055-418F-AFE9-A080A4FAE47A";
    // Registry Key
    const string kRegistryKey = "CLSID\\{" + kGUID + "}";

    // SCFF DirectShow Filterがインストールされているかチェック
    RegistryKey scff_dsf_key =
        RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, registry_view).OpenSubKey(kRegistryKey);
    if (scff_dsf_key == null) {
      MessageBox.Show("scff-*.ax is not correctly installed. Please re-install SCFF DirectShow Filter.",
                      "Not correctly installed",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return;
    }

    // インストールパスが正しいかもチェック
    RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
    string scff_dsf_path = scff_dsf_path_key.GetValue("").ToString();
    if (!File.Exists(scff_dsf_path)) {
      MessageBox.Show("scff-*.ax is not found. Check your SCFF directory.",
                      "DLL is not found",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return;
    }

    // 32bitカラー以外では起動しない
    if (Screen.PrimaryScreen.BitsPerPixel != 32) {
      MessageBox.Show("SCFF requires primary screen is configured 32bit color mode.",
                      "Not 32bit color mode",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
      return;
    }

    Application.Run(new Form1());
  }
}
}
