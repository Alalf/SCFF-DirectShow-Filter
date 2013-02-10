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

using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

/// Applicationクラス
public partial class App : Application {
  //===================================================================
  // 多重起動防止
  //===================================================================

  /// Mutex名
  private const string mutexName = "SCFF.GUI-{3C55C868-E1E9-4E76-B46C-6D07458C5993}";
  /// Singleton: Mutex
  private Mutex Mutex { get; set; }

  //===================================================================
  // staticプロパティ
  //===================================================================

  /// アプリケーションの実装
  public static ClientApplication Impl { get; private set; }

  /// スクリーンキャプチャ用タイマー
  public static ScreenCaptureTimer ScreenCaptureTimer { get; private set; }
  /// NullPen
  public static NullPen NullPen { get; private set; }

  //-------------------------------------------------------------------

  /// アプリケーション設定
  public static Options Options {
    get { return App.Impl.Options; }
  }
  /// アプリケーション実行時設定
  public static RuntimeOptions RuntimeOptions {
    get { return App.Impl.RuntimeOptions; }
  }
  /// 現在編集中のプロファイル
  public static Profile Profile {
    get { return App.Impl.Profile; }
  }

  //-------------------------------------------------------------------

  /// Staticプロパティの生成
  private void ConstructStaticProperties() {
    App.Impl                        = new ClientApplication();
    App.ScreenCaptureTimer          = new ScreenCaptureTimer();
    App.NullPen                     = new NullPen();
    App.Impl.OnStartupErrorOccured  += App.OnStartupErrorOccured;
    App.Impl.OnDSFErrorOccured      += App.OnDSFErrorOccured;
  }
  /// Staticプロパティの解放
  private void DestructStaticProperties() {
    // 他のプロパティはファイナライザに任せる
    App.Impl.OnStartupErrorOccured  -= App.OnStartupErrorOccured;
    App.Impl.OnDSFErrorOccured      -= App.OnDSFErrorOccured;
  }

  //===================================================================
  // SCFF.Common.ClientApplicationイベントハンドラ
  //===================================================================

  /// @copybrief SCFF::Common::ClientApplication::OnStartupErrorOccured
  /// @param[in] sender 使用しない
  /// @param[in] e エラー表示用のデータが格納されたオブジェクト
  private static void OnStartupErrorOccured(object sender, ErrorOccuredEventArgs e) {
    if (e.Quiet) return;
    MessageBox.Show(e.Message, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }

  /// @copybrief SCFF::Common::ClientApplication::OnDSFErrorOccured
  /// @param[in] sender 使用しない
  /// @param[in] e エラー表示用のデータが格納されたオブジェクト
  private static void OnDSFErrorOccured(object sender, ErrorOccuredEventArgs e) {
    if (e.Quiet) return;
    /// @todo(me) 最前面に表示する方法を調べる
    ///           MessageBoxOptions.DefaultDesktopOnlyは明らかに手抜きなのでやめる
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
    // 引数取得
    var path = e.Args.Length > 0 ? e.Args[0] : null;

    // Mutexのチェック(Closeはファイナライザに任せる)
    bool createdNew;
    var mutex = new Mutex(false, App.mutexName, out createdNew);
    if (!createdNew) {
      this.Shutdown();
      return;
    }
    this.Mutex = mutex;

    // 正常起動
    base.OnStartup(e);
    this.ConstructStaticProperties();
    App.Impl.Startup(path);

    // ProcessRenderModeの設定
    if (App.Options.EnableGPUPreviewRendering) {
      // Default
    } else {
      RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
    }
    
    // Window生成
    var mainWindow = new MainWindow();
    mainWindow.Show();
  }

  /// Exit: アプリケーション終了時
  /// @param e 終了コード(ApplicationExitCode)の参照・設定が可能
  protected override void OnExit(ExitEventArgs e) {
    // Mutexがない = 複数起動
    if (this.Mutex == null) {
      Debug.WriteLine(Constants.SCFFVersion + " is already running.", "App");
      e.ApplicationExitCode = -1;
      return;
    }

    // 正常終了
    base.OnExit(e);
    App.Impl.Exit();
    this.DestructStaticProperties();
  }
}
}   // namespace SCFF.GUI
