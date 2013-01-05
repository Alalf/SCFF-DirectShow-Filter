// Copyright 2012-2013 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter(SCFF DSF).
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

/// @file SCFF.GUI/App.xaml.cs
/// Applicationクラス

/// SCFF DSFのGUIクライアント
namespace SCFF.GUI {

using System.Windows;
using SCFF.Common;

/// Applicationクラス
public partial class App : Application {
  //===================================================================
  // シングルトン
  //===================================================================

  /// アプリケーションの設定を格納するシングルトン
  private static Options options = new Options();
  /// アプリケーションの設定を格納するシングルトンを取得
  public static Options Options {
    get { return options; }
  }

  /// 現在編集中のプロファイルを格納するシングルトン
  private static Profile profile = new Profile();
  /// 現在編集中のプロファイルを格納するシングルトンを取得
  public static Profile Profile {
    get { return profile; }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// アプリケーション起動時
  private void App_Startup(object sender, StartupEventArgs e) {
    // Options
    OptionsINIFile.Load(App.Options);

    // Profile
    if (App.Options.RestoreLastProfile) {
      /// @todo(me) プロファイル読み込み
      App.Profile.ResetProfile();
    } else {
      App.Profile.ResetProfile();
    }
  }

  /// アプリケーション終了時
  private void App_Exit(object sender, ExitEventArgs e) {
    // Profileの保存は明示的にMainWindow上で行うのでここでは何もしない

    // Options
    OptionsINIFile.Save(App.Options);
  }
}
}
