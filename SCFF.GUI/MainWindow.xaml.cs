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

/// @file SCFF.GUI/MainWindow.xaml.cs
/// @copydoc SCFF::GUI::MainWindow

namespace SCFF.GUI {

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Windows.Shell;
using SCFF.Common;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

/// MainWindowのコードビハインド
public partial class MainWindow
    : Window, IBindingProfile, IBindingOptions, IBindingRuntimeOptions {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public MainWindow() {
    this.InitializeComponent();

    this.NotifyOptionsChanged();
    this.NotifyRuntimeOptionsChanged();
    this.NotifyProfileChanged();

    App.Profile.OnChanged += this.OnProfileChanged;
  }

  /// デストラクタ
  ~MainWindow() {
    App.Profile.OnChanged -= this.OnProfileChanged;
  }

  //===================================================================
  // ProfileDocument: New/Close/Save/Open/SendProfile
  //===================================================================

  /// 保存失敗時のダイアログを表示
  private void ShowSaveFailedDialog(string path) {
    var errorMessage = "Couldn't save the profile";
    if (path != null && path != string.Empty) {
      errorMessage = string.Format("Couldn't save the profile to {0}.", path);
    }
    MessageBox.Show(errorMessage, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }
  /// 読み込み失敗時のダイアログを表示
  private void ShowOpenFailedDialog(string path) {
    var errorMessage = "Couldn't open the profile";
    if (path != null && path != string.Empty) {
      errorMessage = string.Format("Couldn't open the profile from {0}.", path);
    }
    MessageBox.Show(errorMessage, "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }
  /// 共有メモリ設定失敗時のダイアログを表示
  private void ShowSendFailedDialog(ValidationErrors errors = null) {
    var errorMessage = new StringBuilder();
    errorMessage.AppendLine("Couldn't send the profile to shared memory.");
    if (errors != null) {
      foreach (var error in errors) {
        errorMessage.AppendLine("  - " + error.Message);
      }
    }
    MessageBox.Show(errorMessage.ToString(), "SCFF.GUI",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
  }

  //-------------------------------------------------------------------

  /// Profileを閉じる
  private bool CloseProfile() {
    // 編集がされていなければそのまま[閉じる]ことが可能
    if (!App.ProfileDocument.HasModified) return true;

    // ダイアログを表示
    var message = string.Format("Do you want to save changes to {0}?",
        App.ProfileDocument.HasSaved ? App.RuntimeOptions.ProfileName
                                     : "Untitled");
    var result =  MessageBox.Show(message,
                                  "SCFF.GUI",
                                  MessageBoxButton.YesNoCancel,
                                  MessageBoxImage.Warning,
                                  MessageBoxResult.Yes);
    switch (result) {
      case MessageBoxResult.Yes: return this.SaveProfile(SaveType.Save);
      case MessageBoxResult.No: return true;
      case MessageBoxResult.Cancel: return false;
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
  }

  /// Profileの新規作成
  private bool NewProfile() {
    if (!this.CloseProfile()) return false;

    App.ProfileDocument.New();

    //-----------------------------------------------------------------
    // Notify self
    this.OnRuntimeOptionsChanged();
    // Notify other controls
    this.NotifyProfileChanged();
    //-----------------------------------------------------------------

    return true;
  }

  /// Profileの読み込み
  private bool OpenProfile(string path) {
    if (!this.CloseProfile()) return false;
    
    // ファイル名が指定されていない場合はダイアログを表示する
    if (path == null || path == string.Empty) {
      var dialog = new OpenFileDialog();
      dialog.Title = "SCFF.GUI";
      dialog.Filter = "SCFF Profile|*" + ProfileINIFile.ProfileExtension;
      if (App.ProfileDocument.HasSaved) {
        dialog.InitialDirectory = Path.GetDirectoryName(App.RuntimeOptions.ProfilePath);
      } else {
        dialog.InitialDirectory = Utilities.GetDefaultFilePath;
      }
      var result = dialog.ShowDialog(this);
      if (result.HasValue && (bool)result) {
        path = dialog.FileName;
      } else {
        return false;
      }
    }
    // データの読み込み
    var openResult = App.ProfileDocument.Open(path);
    if (!openResult) {
      this.ShowOpenFailedDialog(path);
      return false;
    }

    //-----------------------------------------------------------------
    // Notify self
    this.OnRuntimeOptionsChanged();
    // Notify other controls
    this.MainMenu.OnOptionsChanged();
    this.NotifyProfileChanged();
    //-----------------------------------------------------------------
    
    return true;
  }

  /// Profileの保存・別名で保存のどちらかを示す列挙型
  private enum SaveType {
    Save,     ///< 保存
    SaveAs,   ///< 別名で保存
  }

  /// Profileの保存
  private bool SaveProfile(SaveType type) {
    var dialog = new SaveFileDialog();
    dialog.Title = "SCFF.GUI";
    dialog.Filter = "SCFF Profile|*" + ProfileINIFile.ProfileExtension;
    // [保存]の場合は現在編集中のパスを使う
    var path = App.RuntimeOptions.ProfilePath;
    // [別名で保存]か、まだ保存していない場合はダイアログを表示する
    if (type == SaveType.SaveAs || !App.ProfileDocument.HasSaved) {
      if (App.ProfileDocument.HasSaved) {
        // [別名で保存]の場合は元のProfileの場所を表示
        dialog.InitialDirectory = Path.GetDirectoryName(App.RuntimeOptions.ProfilePath);
        dialog.FileName = App.RuntimeOptions.ProfileName;
      } else {
        dialog.InitialDirectory = Utilities.GetDefaultFilePath;
        dialog.FileName = "Untitled";
      }
      var result = dialog.ShowDialog(this);
      if (result.HasValue && (bool)result) {
        path = dialog.FileName;
      } else {
        return false;
      }
    }
    // データの書き込み
    var saveResult = App.ProfileDocument.Save(path);
    if (!saveResult) {
      this.ShowSaveFailedDialog(path);
      return false;
    }

    //-----------------------------------------------------------------
    // Notify self
    this.OnRuntimeOptionsChanged();
    // Notify other controls
    this.MainMenu.OnOptionsChanged();
    //-----------------------------------------------------------------

    return true;
  }

  /// Profileの共有メモリへの書き込み
  private void SendProfile(bool quiet, bool forceNullLayout) {
    // 検証
    var errors = App.ProfileDocument.Validate();
    if (!errors.IsNoError) {
      if (!quiet) this.ShowSendFailedDialog(errors);
      return;
    }
    // 共有メモリに書き込み
    var result = App.ProfileDocument.SendMessage(App.Interprocess, forceNullLayout);
    if (!result) {
      if (!quiet) this.ShowSendFailedDialog(null);
      return;
    }

    //-----------------------------------------------------------------
    // Notify self
    // Notify other controls
    this.Apply.OnRuntimeOptionsChanged();
    //-----------------------------------------------------------------
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  protected override void OnClosing(System.ComponentModel.CancelEventArgs e) {
    base.OnClosing(e);
    if (!this.CloseProfile()) {
      e.Cancel = true;
      return;
    }

    this.SaveTemporaryOptions();
  }

  /// Deactivated
  /// @param e 使用しない
  protected override void OnDeactivated(System.EventArgs e) {
    base.OnDeactivated(e);

    /// @todo(me) スクリーンキャプチャをの更新頻度を下げる
    ///           App.RuntimeOptionsに該当するデータを保存しておく感じかな？
    // Debug.WriteLine("Deactivated", "MainWindow");
  }

  /// Activated
  /// @param e 使用しない
  protected override void OnActivated(System.EventArgs e) {
    base.OnActivated(e);

    /// @todo(me) スクリーンキャプチャを更新頻度を元に戻す
    ///           App.RuntimeOptionsに該当するデータを保存しておく感じかな？
    // Debug.WriteLine("Activated", "MainWindow");
  }

  /// Drop
  /// @param e ドラッグアンドドロップされた内容が入っている
  protected override void OnDrop(DragEventArgs e) {
    base.OnDrop(e);
    string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
    if (files == null || files.Length == 0) return;
    var path = files[0];
    if (Path.GetExtension(path) != ProfileINIFile.ProfileExtension) return;

    this.OpenProfile(path);
  }

  /// Profile.OnChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnProfileChanged(object sender, System.EventArgs e) {
    if (App.Options.AutoApply) {
      this.SendProfile(true, false);
    }

    this.OnRuntimeOptionsChanged();
    this.Apply.OnRuntimeOptionsChanged();
  }

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// AreaExpander: Collapsed
  private void AreaExpander_Collapsed(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.AreaIsExpanded = false;
  }
  /// AreaExpander: Expanded
  private void AreaExpander_Expanded(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.AreaIsExpanded = true;
  }
  /// OptionsExpander: Collapsed
  private void OptionsExpander_Collapsed(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.OptionsIsExpanded = false;
  }
  /// OptionsExpander: Expanded
  private void OptionsExpander_Expanded(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.OptionsIsExpanded = true;
  }
  /// ResizeMethodExpander: Collapsed
  private void ResizeMethodExpander_Collapsed(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.ResizeMethodIsExpanded = false;
  }
  /// ResizeMethodExpander: Expanded
  private void ResizeMethodExpander_Expanded(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;
    App.Options.ResizeMethodIsExpanded = true;
  }
  /// LayoutExpander: Collapsed
  private void LayoutExpander_Collapsed(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;

    //-----------------------------------------------------------------
    // Notify self
    this.UpdateTmpSize();
    App.Options.LayoutIsExpanded = false;
    this.FixMinMaxSize();
    this.FixSize();
    // Notify other controls
    Commands.ProfileVisualChanged.Execute(null, this);
    //-----------------------------------------------------------------
  }
  /// LayoutExpander: Expanded
  private void LayoutExpander_Expanded(object sender, RoutedEventArgs e) {
    if (!this.CanChangeOptions) return;

    //-----------------------------------------------------------------
    // Notify self
    this.UpdateTmpSize();
    App.Options.LayoutIsExpanded = true;
    this.FixMinMaxSize();
    this.FixSize();
    // Notify other controls
    Commands.ProfileVisualChanged.Execute(null, this);
    //-----------------------------------------------------------------
  }

  //===================================================================
  // IBindingOptionsの実装
  //===================================================================

  /// AeroをON/OFF
  private void SetAero() {
    if (!this.CanUseAero()) return;
    if (App.Options.ForceAeroOn) {
      // @todo(me) 実装
    } else {
      // @todo(me) 実装
    }
  }

  /// AeroのON/OFFが可能か
  private bool CanUseAero() {
    // @todo(me) 実装
    return true;
  }

  // 1. Normal        : !LayoutIsExpanded && !CompactView
  // 2. NormalLayout  : LayoutIsExpanded && !CompactView
  // 3. Compact       : !LayoutIsExpanded && CompactView
  // 4. CompactLayout : LayoutIsExpanded && CompactView

  /// Expanderの表示を調整
  private void FixExpanders() {
    if (App.Options.CompactView) {
      this.OptionsExpander.Visibility = Visibility.Collapsed;
      this.ResizeMethodExpander.Visibility = Visibility.Collapsed;
    } else {
      this.OptionsExpander.Visibility = Visibility.Visible;
      this.ResizeMethodExpander.Visibility = Visibility.Visible;
    }
  }

  /// Tmp*の更新
  private void UpdateTmpSize() {
    var isNormal = this.WindowState == System.Windows.WindowState.Normal;
    if (!App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      App.Options.TmpNormalWidth = isNormal ? this.Width : this.RestoreBounds.Width;
      App.Options.TmpNormalHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    } else if (App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      App.Options.TmpNormalLayoutWidth = isNormal ? this.Width : this.RestoreBounds.Width;
      App.Options.TmpNormalLayoutHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    } else if (!App.Options.LayoutIsExpanded && App.Options.CompactView) {
      App.Options.TmpCompactWidth = isNormal ? this.Width : this.RestoreBounds.Width;
      App.Options.TmpCompactHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    } else {
      App.Options.TmpCompactLayoutWidth = isNormal ? this.Width : this.RestoreBounds.Width;
      App.Options.TmpCompactLayoutHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    }
  }

  /// Width/Heightの設定
  private void FixSize() {
    if (!App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      this.Width = App.Options.TmpNormalWidth;
      this.Height = App.Options.TmpNormalHeight;
    } else if (App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      this.Width = App.Options.TmpNormalLayoutWidth;
      this.Height = App.Options.TmpNormalLayoutHeight;
    } else if (!App.Options.LayoutIsExpanded && App.Options.CompactView) {
      this.Width = App.Options.TmpCompactWidth;
      this.Height = App.Options.TmpCompactHeight;
    } else {
      this.Width = App.Options.TmpCompactLayoutWidth;
      this.Height = App.Options.TmpCompactLayoutHeight;
    }
  }

  /// Max/MinWidthの設定
  private void FixMinMaxSize() {
    this.MinHeight = Constants.MinHeight;
    if (!App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      this.MinWidth = Constants.NoLayoutMinWidth;
      this.MaxWidth = Constants.NoLayoutMaxWidth;
      this.MaxHeight = Constants.NormalMaxHeight;
    } else if (App.Options.LayoutIsExpanded && !App.Options.CompactView) {
      this.MinWidth = Constants.LayoutMinWidth;
      this.MaxWidth = Constants.LayoutMaxWidth;;
      this.MaxHeight = Constants.LayoutMaxHeight;
    } else if (!App.Options.LayoutIsExpanded && App.Options.CompactView) {
      this.MinWidth = Constants.NoLayoutMinWidth;
      this.MaxWidth = Constants.NoLayoutMaxWidth;
      this.MaxHeight = Constants.CompactMaxHeight;
    } else {
      this.MinWidth = Constants.LayoutMinWidth;
      this.MaxWidth = Constants.LayoutMaxWidth;;
      this.MaxHeight = Constants.LayoutMaxHeight;
    }
  }

  //-------------------------------------------------------------------

  /// thisを含むすべての子コントロールにOptionsChangedイベント発生を伝える
  public void NotifyOptionsChanged() {
    this.OnOptionsChanged();
    this.Apply.OnOptionsChanged();
    this.LayoutToolbar.OnOptionsChanged();
    this.LayoutEdit.OnOptionsChanged();
    this.MainMenu.OnOptionsChanged();
  }

  /// @copydoc Common::GUI::IBindingOptions::CanChangeOptions
  public bool CanChangeOptions { get; private set; }
  /// @copydoc Common::GUI::IBindingOptions::OnOptionsChanged
  public void OnOptionsChanged() {
    this.CanChangeOptions = false;

    // Temporary
    this.Left         = App.Options.TmpLeft;
    this.Top          = App.Options.TmpTop;
    this.WindowState  = (System.Windows.WindowState)App.Options.TmpWindowState;

    // MainWindow.Controls
    this.AreaExpander.IsExpanded          = App.Options.AreaIsExpanded;
    this.OptionsExpander.IsExpanded       = App.Options.OptionsIsExpanded;
    this.ResizeMethodExpander.IsExpanded  = App.Options.ResizeMethodIsExpanded;
    this.LayoutExpander.IsExpanded        = App.Options.LayoutIsExpanded;

    this.FixMinMaxSize();
    this.FixSize();
    this.FixExpanders();

    this.SetAero();

    this.CanChangeOptions = true;
  }

  /// UIから設定にデータを保存
  private void SaveTemporaryOptions() {
    // Tmp接頭辞のプロパティだけはここで更新する必要がある
    var isNormal = this.WindowState == System.Windows.WindowState.Normal;
    App.Options.TmpLeft = isNormal ? this.Left : this.RestoreBounds.Left;
    App.Options.TmpTop = isNormal ? this.Top : this.RestoreBounds.Top;
    this.UpdateTmpSize();
    App.Options.TmpWindowState = (SCFF.Common.WindowState)this.WindowState;
  }

  //===================================================================
  // IBindingRuntimeOptionsの実装
  //===================================================================

  /// thisを含むすべての子コントロールにRuntimeOptionsChangedイベント発生を伝える
  public void NotifyRuntimeOptionsChanged() {
    this.OnRuntimeOptionsChanged();
    this.Apply.OnRuntimeOptionsChanged();
    this.LayoutEdit.OnRuntimeOptionsChanged();
    this.LayoutParameter.OnRuntimeOptionsChanged();
    this.SCFFEntries.OnRuntimeOptionsChanged();
  }

  /// @copydoc Common::GUI::IBindingRuntimeOptions::CanChangeRuntimeOptions
  public bool CanChangeRuntimeOptions { get; private set; }
  /// @copydoc Common::GUI::IBindingRuntimeOptions::OnRuntimeOptionsChanged
  public void OnRuntimeOptionsChanged() {
    this.CanChangeRuntimeOptions = false;

    this.WindowTitle.Content = App.ProfileDocument.Title;
    this.Title = App.ProfileDocument.Title;

    this.CanChangeRuntimeOptions = true;
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// thisを含むすべての子コントロールにCurrentLayoutElementChangedイベント発生を伝える
  public void NotifyCurrentLayoutElementChanged() {
    this.OnCurrentLayoutElementChanged();
    this.TargetWindow.OnCurrentLayoutElementChanged();
    this.Area.OnCurrentLayoutElementChanged();
    this.Options.OnCurrentLayoutElementChanged();
    this.ResizeMethod.OnCurrentLayoutElementChanged();
    this.LayoutParameter.OnCurrentLayoutElementChanged();
    this.LayoutTab.OnCurrentLayoutElementChanged();
    this.LayoutEdit.OnCurrentLayoutElementChanged();
  }

  /// thisを含むすべての子コントロールにRuntimeOptionsChangedイベント発生を伝える
  public void NotifyProfileChanged() {
    this.OnProfileChanged();
    this.TargetWindow.OnProfileChanged();
    this.Area.OnProfileChanged();
    this.Options.OnProfileChanged();
    this.ResizeMethod.OnProfileChanged();
    this.LayoutParameter.OnProfileChanged();
    this.LayoutTab.OnProfileChanged();
    this.LayoutEdit.OnProfileChanged();
  }

  /// @copydoc Common::GUI::IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }

  /// @copydoc Common::GUI::IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    // Currentのみの更新には対応していない
    this.OnProfileChanged();
  }

  /// @copydoc Common::GUI::IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    this.CanChangeProfile = false;
    // nop
    this.CanChangeProfile = true;
  }

  //===================================================================
  // コマンドイベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // ApplicationCommands
  //-------------------------------------------------------------------

  /// New
  private void OnNew(object sender, ExecutedRoutedEventArgs e) {
    this.NewProfile();
  }

  /// Open
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnOpen(object sender, ExecutedRoutedEventArgs e) {
    var path = e.Parameter as string;
    this.OpenProfile(path);
  }

  /// Save
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnSave(object sender, ExecutedRoutedEventArgs e) {
    this.SaveProfile(SaveType.Save);
  }

  /// SaveAs
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnSaveAs(object sender, ExecutedRoutedEventArgs e) {
    this.SaveProfile(SaveType.SaveAs);
  }

  //-------------------------------------------------------------------
  // Windows.Shell.SystemCommands
  //-------------------------------------------------------------------
  
  /// CloseWindow
	private void OnCloseWindow(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.CloseWindow(this);
	}
  /// MaximizeWindow
	private void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MaximizeWindow(this);
	}
  /// MinimizeWindow
	private void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MinimizeWindow(this);
	}
  /// RestoreWindow
	private void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.RestoreWindow(this);
	}

  //-------------------------------------------------------------------
  // SCFF.GUI.Commands
  //-------------------------------------------------------------------

  /// @copybrief Commands::CurrentLayoutElementVisualChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnCurrentLayoutElementVisualChanged(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutEdit.OnCurrentLayoutElementChanged();
  }
  /// @copybrief Commands::ProfileVisualChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnProfileVisualChanged(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutEdit.OnOptionsChanged();
    // 内部でOnProfileChangedと同じ処理が走る
  }
  /// @copybrief Commands::ProfileStructureChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnProfileStructureChanged(object sender, ExecutedRoutedEventArgs e) {
    // tabの選択を変えないといけないのでEntireじゃなければいけない
    this.NotifyProfileChanged();
  }
  /// @copybrief Commands::LayoutParameterChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnLayoutParameterChanged(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutParameter.OnCurrentLayoutElementChanged();
  }
  /// @copybrief Commands::TargetWindowChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnTargetWindowChanged(object sender, ExecutedRoutedEventArgs e) {
    this.TargetWindow.OnCurrentLayoutElementChanged();
    // CurrentLayoutElementVisualChanged
    this.LayoutEdit.OnCurrentLayoutElementChanged();
  }
  /// @copybrief Commands::AreaChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnAreaChanged(object sender, ExecutedRoutedEventArgs e) {
    this.Area.OnCurrentLayoutElementChanged();
    // CurrentLayoutElementVisualChanged
    this.LayoutEdit.OnCurrentLayoutElementChanged();
  }
  /// @copybrief Commands::SampleSizeChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnSampleSizeChanged(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutEdit.OnRuntimeOptionsChanged();
    this.LayoutParameter.OnRuntimeOptionsChanged();
  }

  //-------------------------------------------------------------------

  /// @copybrief Commands::AddLayoutElement
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnAddLayoutElement(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.Add();
    // tabの選択を変えないといけないので全てに通知
    this.NotifyProfileChanged();
  }

  /// @copybrief Commands::AddLayoutElement
  /// @warning CanExecuteは処理負荷が高い
  /// @param sender 使用しない
  /// @param e 実行可能かどうかをCanExecuteに設定可能
  private void CanAddLayoutElement(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanAdd;
  }

  /// @copybrief Commands::RemoveCurrentLayoutElement
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnRemoveCurrentLayoutElement(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.RemoveCurrent();
    // tabの選択を変えないといけないので全てに通知
    this.NotifyProfileChanged();
  }

  /// @copybrief Commands::RemoveCurrentLayoutElement
  /// @warning CanExecuteは処理負荷が高い
  /// @param sender 使用しない
  /// @param e 実行可能かどうかをCanExecuteに設定可能
  private void CanRemoveCurrentLayoutElement(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanRemoveCurrent;
  }

  /// @copybrief Commands::FitCurrentBoundRect
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnFitCurrentBoundRect(object sender, ExecutedRoutedEventArgs e) {
    if (!App.Profile.CurrentView.IsWindowValid) {
      Debug.WriteLine("Invalid Window", "[Command] FitCurrentBoundRect");
      return;
    }

    // Profileの設定を変える
    App.Profile.Current.Open();
    App.Profile.Current.FitBoundRelativeRect(
        App.RuntimeOptions.CurrentSampleWidth,
        App.RuntimeOptions.CurrentSampleHeight);
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    // Notify other controls
    this.LayoutParameter.OnCurrentLayoutElementChanged();
    this.LayoutEdit.OnCurrentLayoutElementChanged();
    //-----------------------------------------------------------------
  }

  //-------------------------------------------------------------------

  /// @copybrief Commands::SendProfile
  private void OnSendProfile(object sender, ExecutedRoutedEventArgs e) {
    this.SendProfile(false, false);
  }
  /// @copybrief Commands::SendProfile
  private void CanSendProfile(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = !App.RuntimeOptions.IsEntryListEmpty;
  }
  /// @copybrief Commands::SendNullProfile
  private void OnSendNullProfile(object sender, ExecutedRoutedEventArgs e) {
    this.SendProfile(false, true);
  }
  /// @copybrief Commands::SendNullProfile
  private void CanSendNullProfile(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = !App.RuntimeOptions.IsEntryListEmpty;
  }

  //-------------------------------------------------------------------

  /// @copybrief SetAero
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnSetAero(object sender, ExecutedRoutedEventArgs e) {
    if (!CanUseAero()) return;

    App.Options.ForceAeroOn = (bool)e.Parameter;
    Debug.WriteLine("Execute", "[Command] SetAero");
    this.SetAero();
  }

  /// Compact表示に変更する
  /// @param sender 使用しない
  /// @param e 使用しない
  private void OnSetCompactView(object sender, ExecutedRoutedEventArgs e) {
    Debug.WriteLine("Execute", "[Command] SetCompactView");

    // App.Options.CompactViewの変更前にウィンドウの幅と高さを保存しておく
    this.UpdateTmpSize();
    App.Options.CompactView = (bool)e.Parameter;
    this.FixMinMaxSize();
    this.FixSize();
    this.FixExpanders();
  }
}
}   // namespace SCFF.GUI
