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
using Microsoft.Win32;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

/// Applicationクラス
public partial class App : Application {
  //===================================================================
  // シングルトン
  //===================================================================

  /// Singleton: アプリケーションの実装
  public static ClientApplication Impl { get; private set; }

  /// Singleton: スクリーンキャプチャ用タイマー
  public static ScreenCaptureTimer ScreenCaptureTimer { get; private set; }
  /// Singleton: NullPen
  public static NullPen NullPen { get; private set; }

  //-------------------------------------------------------------------

  /// Singleton: アプリケーション設定
  public static Options Options {
    get { return App.Impl.Options; }
  }
  /// Singleton: アプリケーション実行時設定
  public static RuntimeOptions RuntimeOptions {
    get { return App.Impl.RuntimeOptions; }
  }
  /// Singleton: 現在編集中のプロファイル
  public static Profile Profile {
    get { return App.Impl.Profile; }
  }

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// staticコンストラクタ
  static App() {
    App.Impl = new ClientApplication();

    App.ScreenCaptureTimer = new ScreenCaptureTimer();
    App.NullPen = new NullPen();

    // SCFF.Common.ClientApplicationのイベントハンドラ登録
    App.Impl.OnErrorOccured += App.OnErrorOccured;
  }

  ~App() {
    App.Impl.OnErrorOccured -= App.OnErrorOccured;
  }

  //===================================================================
  // SCFF.Common.ClientApplicationイベントハンドラ
  //===================================================================

  /// @copydoc SCFF::Common::ClientApplication::OnErrorOccured
  private static void OnErrorOccured(object sender, ErrorOccuredEventArgs e) {
    if (e.Quiet) return;
    MessageBox.Show(e.Message, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// Startup: アプリケーション開始時
  /// @param e コマンドライン引数(Args)を参照可能
  protected override void OnStartup(StartupEventArgs e) {
    base.OnStartup(e);
    var path = e.Args.Length > 0 ? e.Args[0] : null;
    App.Impl.Startup(path);
  }

  /// Exit: アプリケーション終了時
  /// @param e 終了コード(ApplicationExitCode)の参照・設定が可能
  protected override void OnExit(ExitEventArgs e) {
    base.OnExit(e);
    App.Impl.Exit();
  }
}
}   // namespace SCFF.GUI
