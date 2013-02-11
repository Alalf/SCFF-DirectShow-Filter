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
using System.IO.Pipes;
using System.Text;
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
  // 定数
  //===================================================================

  /// Mutex名
  private const string mutexName = "SCFF.GUI-{3C55C868-E1E9-4E76-B46C-6D07458C5993}";
  /// NamedPipe名
  private const string namedPipeName = mutexName + "-pipe";

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// Staticプロパティの生成
  private void ConstructStaticProperties() {
    App.Impl                        = new ClientApplication();
    App.ScreenCaptureTimer          = new ScreenCaptureTimer();
    App.NullPen                     = new NullPen();
    App.Impl.OnStartupErrorOccured  += this.OnStartupErrorOccured;
    App.Impl.OnDSFErrorOccured      += this.OnDSFErrorOccured;
  }
  /// Staticプロパティの解放
  private void DestructStaticProperties() {
    // 他のプロパティはファイナライザに任せる
    App.Impl.OnStartupErrorOccured  -= this.OnStartupErrorOccured;
    App.Impl.OnDSFErrorOccured      -= this.OnDSFErrorOccured;
  }

  //-------------------------------------------------------------------

  /// Startup: アプリケーション開始時
  /// @param e コマンドライン引数(Args)を参照可能
  protected override void OnStartup(StartupEventArgs e) {
    // 引数取得
    var path = e.Args.Length > 0 ? e.Args[0] : null;

    // Mutexのチェック(Closeはファイナライザに任せる)
    bool createdNew;
    var mutex = new Mutex(false, App.mutexName, out createdNew);
    if (!createdNew) {
      this.StartPipeClient(path);
      this.Shutdown(-1);
      return;
    }
    this.mutex = mutex;

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
    this.MainWindow = new MainWindow();
    this.MainWindow.Show();

    // NamedPipe作成
    var context = SynchronizationContext.Current;
    this.StartPipeServer(context);
  }

  /// Exit: アプリケーション終了時
  /// @param e 終了コード(ApplicationExitCode)の参照・設定が可能
  protected override void OnExit(ExitEventArgs e) {
    // Mutexがない = 複数起動
    if (this.mutex == null) {
      Debug.WriteLine(Constants.SCFFVersion + " is already running.", "App");
      return;
    }

    // 正常終了
    base.OnExit(e);
    App.Impl.Exit();
    this.DestructStaticProperties();
  }


  //===================================================================
  // SCFF.Common.ClientApplicationイベントハンドラ
  //===================================================================

  /// @copybrief SCFF::Common::ClientApplication::OnStartupErrorOccured
  /// @param[in] sender 使用しない
  /// @param[in] e エラー表示用のデータが格納されたオブジェクト
  private void OnStartupErrorOccured(object sender, ErrorOccuredEventArgs e) {
    if (e.Quiet) return;
    MessageBox.Show(e.Message, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }

  /// @copybrief SCFF::Common::ClientApplication::OnDSFErrorOccured
  /// @param[in] sender 使用しない
  /// @param[in] e エラー表示用のデータが格納されたオブジェクト
  private void OnDSFErrorOccured(object sender, ErrorOccuredEventArgs e) {
    if (e.Quiet) return;
    MessageBox.Show(e.Message, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }

  //===================================================================
  // 多重起動防止
  //===================================================================

  /// NamedPipeServerStreamを生成する
  private void StartPipeServer(SynchronizationContext context) {
    Debug.WriteLine("[OPEN]", "NamedPipe");
    var pipe = new NamedPipeServerStream(App.namedPipeName,
        PipeDirection.In, -1, PipeTransmissionMode.Message,
        PipeOptions.Asynchronous);

    try {
      pipe.BeginWaitForConnection((result) => {
        try {
          pipe.EndWaitForConnection(result);
          // 新しいPipeを生成
          this.StartPipeServer(context);

          // Read
          var data = new byte[520]; // 260文字(MAX_PATH)x2byte
          pipe.BeginRead(data, 0, data.Length, (readResult) => {
            string path = "";
            try {
              var actualLength = pipe.EndRead(readResult);
              path = Encoding.Unicode.GetString(data, 0, actualLength);
            } catch {
              // 出来なければできないでOK
              Debug.WriteLine("Read named pipe failed", "App.StartPipeServer");
              return;
            } finally {
              pipe.Close();
            }

            // ProfileをUIスレッドで開く
            Debug.WriteLine("Open profile requested: " + path);
            context.Post((state) => {
              App.Impl.OpenProfile(path);
            }, null);
        
            Debug.WriteLine("[CLOSE]", "NamedPipe");
          }, null);
        } catch {
          // 出来なければできないでOK
          Debug.WriteLine("Connect named pipe failed", "App.StartPipeServer");
          pipe.Close();
        }
      }, null);
    } catch {
      // 出来なければできないでOK
      Debug.WriteLine("Start named pipe failed", "App.StartPipeServer");
      pipe.Close();
    }
  }

  /// NamedPipeClientStreamを生成する
  private void StartPipeClient(string path) {
    if (path == null || path == string.Empty || path.Length > 260) {
      Debug.WriteLine("Argument is invalid", "App.StartPipeClient");
      return;
    }

    try {
      using (var pipe = new NamedPipeClientStream(".", App.namedPipeName, PipeDirection.Out)) {
        if (pipe == null) return;
        pipe.Connect();
        if (!pipe.IsConnected) return;
        var data = Encoding.Unicode.GetBytes(path);
        pipe.Write(data, 0, data.Length);
      }
    } catch {
      // 出来なければできないでOK
      Debug.WriteLine("Send profile path failed", "App.StartPipeClient");
    }
  }

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

  //===================================================================
  // フィールド
  //===================================================================

  /// Mutex
  private Mutex mutex;
}
}   // namespace SCFF.GUI
