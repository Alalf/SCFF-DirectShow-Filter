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
using SCFF.Interprocess;

/// Applicationクラス
public partial class App : Application {
  //===================================================================
  // シングルトン
  //===================================================================

  /// Singleton: 共有メモリアクセス用オブジェクト
  public static Interprocess Interprocess { get; private set; }

  /// Singleton: アプリケーション設定
  public static Options Options { get; private set; }
  /// Singleton: アプリケーション実行時設定
  public static RuntimeOptions RuntimeOptions { get; private set; }
  /// Singleton: 現在編集中のプロファイル
  public static Profile Profile { get; private set; }

  /// Singleton: プロファイルドキュメント
  public static ProfileDocument ProfileDocument { get; private set; }

  /// Singleton: スクリーンキャプチャ用タイマー
  public static ScreenCaptureTimer ScreenCaptureTimer { get; private set; }

  //-------------------------------------------------------------------
  // 非マネージドリソース
  //-------------------------------------------------------------------
  
  /// Singleton: NullPen
  public static NullPen NullPen { get; private set; }

  //-------------------------------------------------------------------
  
  /// staticコンストラクタ
  static App() {
    App.Interprocess = new Interprocess();

    App.Options = new Options();
    App.RuntimeOptions = new RuntimeOptions();
    App.Profile = new Profile();

    App.ProfileDocument = new ProfileDocument(
        App.Options, App.RuntimeOptions, App.Profile);

    App.ScreenCaptureTimer = new ScreenCaptureTimer();
    
    App.NullPen = new NullPen();

    // 1回仮想メモリから読み込み
    App.RuntimeOptions.RefreshDirectory(App.Interprocess);
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
    var path = e.Args.Length > 0 ? e.Args[0] : null;
    App.ProfileDocument.Init(path);
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
