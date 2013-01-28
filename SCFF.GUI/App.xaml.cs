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
/// @copydoc SCFF::GUI::App

/// SCFF DSFのGUIクライアント
namespace SCFF.GUI {

using System.Windows;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

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

  /// Singleton: スクリーンキャプチャ用タイマー
  private static ScreenCaptureTimer screenCaptureTimer = new ScreenCaptureTimer();
  /// スクリーンキャプチャ用タイマー
  public static ScreenCaptureTimer ScreenCaptureTimer {
    get { return App.screenCaptureTimer; }
  }

  //===================================================================
  // 非マネージドリソース
  //===================================================================
  
  /// Singleton: NullPen
  private static NullPen nullPen = new NullPen();
  /// NullPen
  public static NullPen NullPen {
    get { return App.nullPen; }
  }

  //===================================================================
  // Profile/Options/RuntimeOptionsを利用したNew/Open/Save/SaveAs
  //===================================================================

  /// プロファイル新規作成
  public static void NewProfile() {
    // Profile
    App.Profile.RestoreDefault();

    // RuntimeOptions
    App.RuntimeOptions.ProfilePath = string.Empty;
    App.RuntimeOptions.LastSavedTimestamp = -1L;
    App.RuntimeOptions.LastAppliedTimestamp = -1L;
  }

  /// プロファイルの保存
  public static bool SaveProfile(string path) {
    // Profile
    var result = ProfileINIFile.Save(App.Profile, path);
    if (!result) return false;

    // Options
    App.Options.AddRecentProfile(path);

    // RuntimeOptions
    App.RuntimeOptions.ProfilePath = path;
    App.RuntimeOptions.LastSavedTimestamp = App.Profile.Timestamp;
    return true;
  }

  /// プロファイルを開く
  public static bool OpenProfile(string path) {
    // Profile
    var result = ProfileINIFile.Load(App.Profile, path);
    if (!result) return false;

    // RuntimeOptions
    App.RuntimeOptions.ProfilePath = path;
    App.RuntimeOptions.LastSavedTimestamp = App.Profile.Timestamp;
    App.RuntimeOptions.LastAppliedTimestamp = -1L;
    return true;
  }

  /// プロファイルが変更されたか
  public static bool IsProfileModified {
    get { return (App.RuntimeOptions.LastSavedTimestamp != App.Profile.Timestamp); }
  }

  /// プロファイルが前回のApply移行に変更されたか
  public static bool IsProfileModifiedFromApply {
    get { return (App.RuntimeOptions.LastAppliedTimestamp != App.Profile.Timestamp); }
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// Startup: アプリケーション開始時
  /// @param e コマンドライン引数(Args)を参照可能
  protected override void OnStartup(StartupEventArgs e) {
    base.OnStartup(e);

    // Options
    OptionsINIFile.Load(App.Options);

    // Profile
    if (App.Options.RestoreLastProfile) {
      var lastProfilePath = App.Options.GetRecentProfile(0);
      if (lastProfilePath != string.Empty && System.IO.File.Exists(lastProfilePath)) {
        App.OpenProfile(lastProfilePath);
      }
    } else {
      App.NewProfile();
    }
  }

  /// Exit: アプリケーション終了時
  /// @param e 終了コード(ApplicationExitCode)の参照・設定が可能
  protected override void OnExit(ExitEventArgs e) {
    base.OnExit(e);

    // Profileの保存は明示的にMainWindow上で行うのでここでは何もしない

    // Options
    OptionsINIFile.Save(App.Options);
  }
}
}   // namespace SCFF.GUI
