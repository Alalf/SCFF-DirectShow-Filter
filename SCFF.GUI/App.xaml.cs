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

  /// Singleton: アプリケーション設定
  private static Options options = new Options();
  /// アプリケーション設定を取得
  public static Options Options {
    get { return App.options; }
  }

  /// Singleton: アプリケーション実行時設定
  private static RuntimeOptions runtimeOptions = new RuntimeOptions();
  /// アプリケーション実行時設定を取得
  public static RuntimeOptions RuntimeOptions {
    get { return App.runtimeOptions; }
  }

  /// Singleton: 現在編集中のプロファイル
  private static Profile profile = new Profile();
  /// 現在編集中のプロファイルを取得
  public static Profile Profile {
    get { return App.profile; }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// アプリケーション起動時
  /// @param sender 使用しない
  /// @param e コマンドライン引数(Args)を参照可能
  private void OnStartup(object sender, StartupEventArgs e) {
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
  /// @param sender 使用しない
  /// @param e 終了コード(ApplicationExitCode)の参照・設定が可能
  private void OnExit(object sender, ExitEventArgs e) {
    // Profileの保存は明示的にMainWindow上で行うのでここでは何もしない

    // Options
    OptionsINIFile.Save(App.Options);
  }
}
}   // namespace SCFF.GUI
