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

/// @file SCFF.Common/ClientApplication.cs
/// @copydoc SCFF::Common::ClientApplication

namespace SCFF.Common {

using System;
using System.Diagnostics;
using System.IO;
using SCFF.Common.Profile;
using SCFF.Interprocess;

/// SCFF DirectShow Filterの設定用クライアントアプリケーション
public class ClientApplication {
  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// コンストラクタ
  public ClientApplication() {
    this.Interprocess = new Interprocess();
    this.DSFMonitor = new DSFMonitor(this.Interprocess);

    this.Options = new Options();
    this.RuntimeOptions = new RuntimeOptions();
    this.Profile = new Profile.Profile();

    this.Profile.OnChanged += this.Profile_OnChanged;
    this.DSFMonitor.OnErrorOccured += this.DSFMonitor_OnErrorOccured;
  }

  /// デストラクタ
  ~ClientApplication() {
    this.DSFMonitor.OnErrorOccured -= this.DSFMonitor_OnErrorOccured;
    this.Profile.OnChanged -= this.Profile_OnChanged;
  }

  //===================================================================
  // イベント
  //===================================================================
  
  /// スタートアップエラー発生後
  public event EventHandler<ErrorOccuredEventArgs> OnStartupErrorOccured;
  /// DSFエラー発生後
  public event EventHandler<ErrorOccuredEventArgs> OnDSFErrorOccured;

  /// エラー発生後
  public event EventHandler<ErrorOccuredEventArgs> OnErrorOccured;
  /// プロファイル内容変更後
  public event EventHandler OnProfileChanged;

  /// プロファイルを閉じる前
  public event EventHandler<ClosingProfileEventArgs> OnClosingProfile;
  /// プロファイルを新規作成した後
  public event EventHandler OnNewProfile;
  /// プロファイルを開く前
  public event EventHandler<OpeningProfileEventArgs> OnOpeningProfile;
  /// プロファイルを開いた後
  public event EventHandler OnOpenedProfile;
  /// プロファイルを保存する前
  public event EventHandler<SavingProfileEventArgs> OnSavingProfile;
  /// プロファイルを保存した後
  public event EventHandler OnSavedProfile;
  /// プロファイルを仮想メモリに書き込んだ後
  public event EventHandler OnSentProfile;

  //-------------------------------------------------------------------

  private void DSFMonitor_OnErrorOccured(object sender, DSFErrorOccuredEventArgs e) {
    if (!Utilities.IsProcessAlive(e.ProcessID)) return;

    // Event: DSFErrorOccured
    {
      var message = string.Format("SCFF DirectShow Filter({0}) has encountered a problem.",
                                  e.ProcessID);
      var args = new ErrorOccuredEventArgs(message, false);
      var handler = this.OnDSFErrorOccured;
      if (handler != null) handler(this, args);
    }
  }

  private void Profile_OnChanged(object sender, EventArgs e) {
    if (this.Options.AutoApply && !this.RuntimeOptions.IsEntryListEmpty) {
      this.SendProfile(true, false);
    }
    
    // Event: ProfileChanged
    {
      var handler = this.OnProfileChanged;
      if (handler != null) handler(this, EventArgs.Empty);
    }
  }

  //===================================================================
  // 外部インタフェース
  //===================================================================

  /// Argsからの読み込み/LastProfile/新規作成から選んでProfileを読み込む
  public void Startup(string path) {
    // SCFF DirectShow Filterがインストールされているか
    string message;
    var result = EnvironmentChecker.CheckSCFFDSFInstalled(out message);
    if (!result) {
      // Event: StartupErrorOccured
      {
        var args = new ErrorOccuredEventArgs(message, false);
        var handler = this.OnStartupErrorOccured;
        if (handler != null) handler(this, args);
      }
    }

    /// @todo(me) WPFではカラーチェックを簡単にやる方法が見つからなかったので要調査

    // Options
    var optionsFile = new OptionsFile(this.Options);
    var optionsFilePath = Utilities.ApplicationDirectory + Constants.OptionsFileName;
    optionsFile.ReadFile(optionsFilePath);

    // RuntimeOptions
    this.RuntimeOptions.RefreshDirectory(this.Interprocess);

    // 起動時にAeroがOnだったかを記録
    this.RuntimeOptions.SaveStartupAeroState();

    // Profile
    this.InitProfileInternal(path);
  }

  public void Exit() {
    // DSFエラー監視を停止
    this.DSFMonitor.Exit();

    // Profileの保存は明示的に行うのでここでは何もしない

    // RuntimeOptions
    this.RuntimeOptions.RestoreStartupAeroState();

    // Options
    var optionsFile = new OptionsFile(this.Options);
    var optionsFilePath = Utilities.ApplicationDirectory + Constants.OptionsFileName;
    optionsFile.WriteFile(optionsFilePath);
  }

  /// Profileを閉じる
  public bool CloseProfile() {
    // 編集がされていなければそのまま[閉じる]ことが可能
    if (!this.HasModified) return true;

    // Event: ClosingProfile
    var action = CloseActions.Save;
    {
      var profileName = this.HasSaved ? this.RuntimeOptions.ProfileName
                                      : "Untitled";
      var args = new ClosingProfileEventArgs(action, profileName);
      var handler = this.OnClosingProfile;
      if (handler != null) handler(this, args);
      action = args.Action;
    }

    // Actionごとに動作を変える
    switch (action) {
      case CloseActions.Save: return this.SaveProfile(SaveActions.Save);
      case CloseActions.Abandon: return true;
      case CloseActions.Cancel: return false;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Profileの新規作成
  public bool NewProfile() {
    if (!this.CloseProfile()) return false;

    this.NewProfileInternal();

    // Event: NewProfile
    {
      var handler = this.OnNewProfile;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    return true;
  }

  /// Profileの読み込み
  public bool OpenProfile(string path) {
    if (!this.CloseProfile()) return false;
    
    // 拡張子のチェック
    if (path != null && path != string.Empty &&
        Path.GetExtension(path) != Constants.ProfileExtension) {
      // Event: ErrorOccured
      {
        var message = string.Format("{0} must be SCFF profile(*.{1})",
                                    path, Constants.ProfileExtension);
        var args = new ErrorOccuredEventArgs(message, false);
        var handler = this.OnErrorOccured;
        if (handler != null) handler(this, args);
      }
      return false;
    }

    // Event: OpeningProfile
    {
      var initialDirectory = this.HasSaved
          ? Path.GetDirectoryName(this.RuntimeOptions.ProfilePath)
          : Utilities.ApplicationDirectory;
      var args = new OpeningProfileEventArgs(path, initialDirectory);
      var handler = this.OnOpeningProfile;
      if (handler != null) handler(this, args);

      if (args.Cancel) {
        return false;
      } else {
        path = args.Path;
      }
    }
    
    // データの読み込み
    var success = this.OpenProfileInternal(path);
  
    if (!success) {
      // Event: ErrorOccured
      {
        var message = "Couldn't open the profile";
        if (path != null && path != string.Empty) {
          message = string.Format("Couldn't open the profile from {0}.", path);
        }
        var args = new ErrorOccuredEventArgs(message, false);
        var handler = this.OnErrorOccured;
        if (handler != null) handler(this, args);
      }
      return false;
    }

    // Event: OpenedProfile
    {
      var handler = this.OnOpenedProfile;
      if (handler != null) handler(this, EventArgs.Empty);
    }
    
    return true;
  }


  /// Profileの保存
  public bool SaveProfile(SaveActions action) {
    // Event: SavingProfile
    var path = this.RuntimeOptions.ProfilePath;
    {
      var fileName = this.HasSaved
          ? this.RuntimeOptions.ProfileName
          : "Untitled";
      var initialDirectory = this.HasSaved
          ? Path.GetDirectoryName(this.RuntimeOptions.ProfilePath)
          : Utilities.ApplicationDirectory;

      var args = new SavingProfileEventArgs(action, path, fileName, initialDirectory);
      var handler = this.OnSavingProfile;
      if (handler != null) handler(this, args);

      if (args.Cancel) {
        return false;
      } else {
        path = args.Path;
      }
    }

    // データの書き込み
    var success = this.SaveProfileInternal(path);
    if (!success) {
      // Event: ErrorOccured
      {
        var message = "Couldn't save the profile";
        if (path != null && path != string.Empty) {
          message = string.Format("Couldn't save the profile to {0}.", path);
        }
        var args = new ErrorOccuredEventArgs(message, false);
        var handler = this.OnErrorOccured;
        if (handler != null) handler(this, args);
      }
      return false;
    }

    // Event: SavedProfile
    {
      var handler = this.OnSavedProfile;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    return true;
  }

  /// Profileの共有メモリへの書き込み
  public bool SendProfile(bool quiet, bool forceNullLayout) {
    // 送る先のプロセスが存在しているか
    var processID = this.RuntimeOptions.CurrentProcessID;
    if (!Utilities.IsProcessAlive(processID)) {
      // Event: ErrorOccured
      {
        var header = "Couldn't send the profile to shared memory: ";
        var message = header + "\n  - Couldn't find process: " + processID;
        var args = new ErrorOccuredEventArgs(message, quiet);
        var handler = this.OnErrorOccured;
        if (handler != null) handler(this, args);
      }
      // モニターを解除
      this.DSFMonitor.Cleanup(processID);
      return false;
    }

    // 検証
    if (!forceNullLayout) {
      var errors = this.ValidateProfileInternal();
      if (!errors.IsNoError) {
        // Event: ErrorOccured
        {
          var header = "Couldn't send the profile to shared memory: ";
          var message = errors.ToErrorMessage(header);
          var args = new ErrorOccuredEventArgs(message, quiet);
          var handler = this.OnErrorOccured;
          if (handler != null) handler(this, args);
        }
        return false;
      }
    }
    // 共有メモリに書き込み
    var success = this.SendProfileInternal(forceNullLayout);
    if (!success) {
      // Event: ErrorOccured
      { 
        var header = "Couldn't send the profile to shared memory: ";
        var message = header + "\n  - Couldn't access shared memory.";
        var args = new ErrorOccuredEventArgs(message, quiet);
        var handler = this.OnErrorOccured;
        if (handler != null) handler(this, args);
      }
      return false;
    }

    // DSFエラー監視開始
    this.DSFMonitor.Start(this.RuntimeOptions.CurrentProcessID);

    // Event: SentProfile
    {
      var handler = this.OnSentProfile;
      if (handler != null) handler(this, EventArgs.Empty);
    }

    return true;
  }

  public void RefreshDirectory() {
    this.RuntimeOptions.RefreshDirectory(this.Interprocess);
    this.DSFMonitor.CleanupAll();
  }

  public void SetAero() {
    if (!this.RuntimeOptions.CanSetAero) return;
    if (this.Options.ForceAeroOn) {
      this.RuntimeOptions.SetAeroOn();
    } else {
      this.RuntimeOptions.SetAeroOff();
    }
  }

  //===================================================================
  // private メソッド
  //===================================================================

  /// 起動時のProfile生成
  private void InitProfileInternal(string path) {
    // 指定されたパスが開ければそのまま開く
    if (path != null && path != string.Empty &&
        System.IO.File.Exists(path) &&
        Path.GetExtension(path) == Constants.ProfileExtension) {
      this.OpenProfileInternal(path);
      return;
    }

    // パスが指定されておらず、RestoreLastProfileオプションがONならそれを読み込む
    if (this.Options.RestoreLastProfile) {
      var lastProfilePath = this.Options.GetRecentProfile(0);
      if (lastProfilePath != null && lastProfilePath != string.Empty &&
          System.IO.File.Exists(lastProfilePath) &&
          Path.GetExtension(lastProfilePath) == Constants.ProfileExtension) {
        this.OpenProfileInternal(lastProfilePath);
        return;
      }
    }
    
    // 新規作成
    this.NewProfileInternal();
  }

  /// Profileの新規作成
  private void NewProfileInternal() {
    // Profile
    this.Profile.RestoreDefault();

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = string.Empty;
    this.RuntimeOptions.ProfileName = string.Empty;
    this.RuntimeOptions.LastSavedTimestamp = this.Profile.Timestamp;
    this.RuntimeOptions.LastAppliedTimestamp = RuntimeOptions.InvalidTimestamp;

    this.Profile.RaiseChanged();
  }

  /// Profileの保存
  private bool SaveProfileInternal(string path) {
    // Profile
    var profileFile = new ProfileFile(this.Profile);
    var result = profileFile.WriteFile(path);
    if (!result) return false;

    // Options
    this.Options.AddRecentProfile(path);

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = path;
    this.RuntimeOptions.ProfileName = Path.GetFileNameWithoutExtension(path);
    this.RuntimeOptions.LastSavedTimestamp = this.Profile.Timestamp;

    return true;
  }

  /// Profileを読み込む
  private bool OpenProfileInternal(string path) {
    // Profile
    var profileFile = new ProfileFile(this.Profile);
    var result = profileFile.ReadFile(path);
    if (!result) return false;

    // バックアップパラメータの復元
    if (this.Options.RestoreMissingWindowWhenOpeningProfile) {
      this.Profile.RestoreBackupParameters();
    }

    // Options
    this.Options.AddRecentProfile(path);

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = path;
    this.RuntimeOptions.ProfileName = Path.GetFileNameWithoutExtension(path);
    this.RuntimeOptions.LastSavedTimestamp = this.Profile.Timestamp;
    this.RuntimeOptions.LastAppliedTimestamp = RuntimeOptions.InvalidTimestamp;

    this.Profile.RaiseChanged();

    return true;
  }

  /// Profileを検証
  private ValidationErrors ValidateProfileInternal() {
    return Validator.ValidateProfile(this.Profile);
  } 

  /// Profileを共有メモリに書き込み
  /// @pre Validate済み
  /// @param interprocess プロセス間通信用オブジェクト
  /// @param forceNullLayout Splash画像を表示させる
  /// @return 共有メモリに書き込み成功
  private bool SendProfileInternal(bool forceNullLayout) {
    Message message;
    if (forceNullLayout) {
      message.LayoutParameters = new LayoutParameter[Interprocess.MaxComplexLayoutElements];
      message.LayoutType = (int)LayoutTypes.NullLayout;
      message.LayoutElementCount = 0;
    } else {
      message = this.Profile.ToMessage(this.RuntimeOptions.CurrentSampleWidth,
                                       this.RuntimeOptions.CurrentSampleHeight);
    }
    /// @todo(me) もう少し一貫した対応策があるかもしれない
    // 現在時刻で上書き
    message.Timestamp = DateTime.Now.Ticks;
    var initResult = this.Interprocess.InitMessage(this.RuntimeOptions.CurrentProcessID);
    if (!initResult) return false;
    var sendResult = this.Interprocess.SendMessage(message);
    if (!sendResult) return false;

    // バックアップを更新
    this.Profile.UpdateBackupParameters();

    // タイムスタンプ更新
    if (forceNullLayout) {
      this.RuntimeOptions.LastAppliedTimestamp = RuntimeOptions.InvalidTimestamp;
    } else {
      this.RuntimeOptions.LastAppliedTimestamp = this.Profile.Timestamp;
    }
    return true;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 現在編集中のProfileがファイルに保存されているかかどうか
  public bool HasSaved {
    get { return (this.RuntimeOptions.ProfilePath != string.Empty); }
  }

  /// プロファイルが変更されたか
  public bool HasModified {
    get { return (this.RuntimeOptions.LastSavedTimestamp != this.Profile.Timestamp); }
  }

  /// プロファイルが前回のApply移行に変更されたか
  public bool HasModifiedFromLastApply {
    get { return (this.RuntimeOptions.LastAppliedTimestamp != this.Profile.Timestamp); }
  }

  /// 現在の状態を文字列にして返す
  public string Title {
    get {
      var title = Constants.SCFFVersion;
      if (this.HasSaved) {
        title = string.Format("{0}{1} - {2}",
            this.RuntimeOptions.ProfileName,
            this.HasModified ? "*" : "",
            Constants.SCFFVersion);
      }
      return title;
    }
  }

  //-------------------------------------------------------------------

  /// Singleton: プロセス間通信用オブジェクト
  public Interprocess Interprocess { get; private set; }
  /// Singleton: DirectShow Filter監視用オブジェクト
  public DSFMonitor DSFMonitor { get; private set; }

  /// Optionsへの参照
  public Options Options { get; set; }
  /// RuntimeOptionsへの参照
  public RuntimeOptions RuntimeOptions { get; set; }
  /// Profileへの参照
  public Profile.Profile Profile { get; private set; }
}
}   // namespace SCFF.Common
