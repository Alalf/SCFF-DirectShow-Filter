// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

namespace SCFF.GUI {

using System.Windows;
using SCFF.Common;

/// Applicationクラス
public partial class App : Application {
  //===================================================================
  // staticプロパティ
  //===================================================================

  /// アプリケーションの設定を格納するインスタンス
  public static Options Options {
    get { return options; }
  }
  /// 現在編集中のプロファイルを格納するインスタンス
  public static Profile Profile {
    get { return profile; }
  }
  private static Options options = new Options();
  private static Profile profile = new Profile();

  //===================================================================
  // イベントハンドラ
  //===================================================================

  private void App_Startup(object sender, StartupEventArgs e) {
    // Options
    OptionsINIFile.Load(App.Options);

    // Profile
    if (App.Options.TmpRestoreLastProfile) {
      /// @todo(me) プロファイル読み込み
      App.Profile.ResetProfile();
    } else {
      App.Profile.ResetProfile();
    }
  }

  private void App_Exit(object sender, ExitEventArgs e) {
    // Profileの保存は明示的にMainWindow上で行うのでここでは何もしない

    // Options
    OptionsINIFile.Save(App.Options);
  }
}
}
