
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
using System.Windows.Forms;

namespace scff_app {

/// @brief アプリケーションのエントリポイントを格納するクラス
static class Program {
  /// @brief アプリケーションのエントリポイント
  [STAThread]
  static void Main() {
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);

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
