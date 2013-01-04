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
/// MainWindowのコードビハインド

namespace SCFF.GUI {

using Microsoft.Win32;
using Microsoft.Windows.Shell;
using SCFF.Common;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

/// MainWindowのコードビハインド
public partial class MainWindow : Window, IUpdateByProfile, IUpdateByOptions {

  //===================================================================
  // コンストラクタ/Loaded/ShutdownStartedイベントハンドラ
  //===================================================================

  /// コンストラクタ
  public MainWindow() {    
    this.InitializeComponent();
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void OnLoaded(object sender, RoutedEventArgs e) {
    this.UpdateByOptions();
    this.UpdateByEntireProfile();

    // 必要な機能の実行
    this.SetAero();
    this.SetCompactView();
  }

  /// ウィンドウがアクティブになったとき
  private void OnActivated(object sender, System.EventArgs e) {
    /// @todo(me) スクリーンキャプチャを再開する・・・？まだ考慮の余地あり
    ///           App.RuntimeOptionsに該当するデータを保存しておく感じかな？
  }

  /// ウィンドウがアクティブでなくなったとき
  private void OnDeactivated(object sender, System.EventArgs e) {
    /// @todo(me) スクリーンキャプチャを停止する
    ///           App.RuntimeOptionsに該当するデータを保存しておく感じかな？
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
    this.SaveTemporaryOptions();
  }

  //===================================================================
  // IUpdateByOptionsの実装
  //===================================================================

  /// @copydoc IUpdateByOptions.UpdateByOptions
  public void UpdateByOptions() {
    // Temporary
    this.Left         = App.Options.TmpMainWindowLeft;
    this.Top          = App.Options.TmpMainWindowTop;
    this.Width        = App.Options.TmpMainWindowWidth;
    this.Height       = App.Options.TmpMainWindowHeight;
    this.WindowState  = (System.Windows.WindowState)App.Options.TmpMainWindowState;
    
    // MainWindow.Controls
    this.AutoApply.IsChecked = App.Options.AutoApply;
    this.DetachOptionsChangedEventHandlers();
    this.AreaExpander.IsExpanded          = App.Options.AreaIsExpanded;
    this.OptionsExpander.IsExpanded       = App.Options.OptionsIsExpanded;
    this.ResizeMethodExpander.IsExpanded  = App.Options.ResizeMethodIsExpanded;
    this.LayoutExpander.IsExpanded        = App.Options.LayoutIsExpanded;
    this.AttachOptionsChangedEventHandlers();

    // UserControls
    this.LayoutToolbar.UpdateByOptions();
    this.MainMenu.UpdateByOptions();
  }

  /// @copydoc IUpdateByOptions.DetachOptionsChangedEventHandlers
  public void DetachOptionsChangedEventHandlers() {
    this.AreaExpander.Collapsed -= this.AreaExpander_Collapsed;
    this.AreaExpander.Expanded -= this.AreaExpander_Expanded;
    this.OptionsExpander.Collapsed -= this.OptionsExpander_Collapsed;
    this.OptionsExpander.Expanded -= this.OptionsExpander_Expanded;
    this.ResizeMethodExpander.Collapsed -= this.ResizeMethodExpander_Collapsed;
    this.ResizeMethodExpander.Expanded -= this.ResizeMethodExpander_Expanded;
    this.LayoutExpander.Collapsed -= this.LayoutExpander_Collapsed;
    this.LayoutExpander.Expanded -= this.LayoutExpander_Expanded;
  }

  /// @copydoc IUpdateByOptions.AttachOptionsChangedEventHandlers
  public void AttachOptionsChangedEventHandlers() {
    this.AreaExpander.Collapsed += this.AreaExpander_Collapsed;
    this.AreaExpander.Expanded += this.AreaExpander_Expanded;
    this.OptionsExpander.Collapsed += this.OptionsExpander_Collapsed;
    this.OptionsExpander.Expanded += this.OptionsExpander_Expanded;
    this.ResizeMethodExpander.Collapsed += this.ResizeMethodExpander_Collapsed;
    this.ResizeMethodExpander.Expanded += this.ResizeMethodExpander_Expanded;
    this.LayoutExpander.Collapsed += this.LayoutExpander_Collapsed;
    this.LayoutExpander.Expanded += this.LayoutExpander_Expanded;
  }

  /// UIから設定にデータを保存
  private void SaveTemporaryOptions() {
    // Tmp接頭辞のプロパティだけはここで更新する必要がある
    var isNormal = this.WindowState == System.Windows.WindowState.Normal;
    App.Options.TmpMainWindowLeft = isNormal ? this.Left : this.RestoreBounds.Left;
    App.Options.TmpMainWindowTop = isNormal ? this.Top : this.RestoreBounds.Top;
    App.Options.TmpMainWindowWidth = isNormal ? this.Width : this.RestoreBounds.Width;
    App.Options.TmpMainWindowHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    App.Options.TmpMainWindowState = (SCFF.Common.WindowState)this.WindowState;
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  private void autoApply_Click(object sender, RoutedEventArgs e) {
    if (!this.AutoApply.IsChecked.HasValue) return;
    App.Options.AutoApply = (bool)this.AutoApply.IsChecked;
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed
  //-------------------------------------------------------------------

  private void AreaExpander_Collapsed(object sender, RoutedEventArgs e) {
    App.Options.AreaIsExpanded = false;
  }

  private void AreaExpander_Expanded(object sender, RoutedEventArgs e) {
    App.Options.AreaIsExpanded = true;
  }

  private void OptionsExpander_Collapsed(object sender, RoutedEventArgs e) {
    App.Options.OptionsIsExpanded = false;
  }

  private void OptionsExpander_Expanded(object sender, RoutedEventArgs e) {
    App.Options.OptionsIsExpanded = true;
  }

  private void ResizeMethodExpander_Collapsed(object sender, RoutedEventArgs e) {
    App.Options.ResizeMethodIsExpanded = false;
  }

  private void ResizeMethodExpander_Expanded(object sender, RoutedEventArgs e) {
    App.Options.ResizeMethodIsExpanded = true;
  }

  private void LayoutExpander_Collapsed(object sender, RoutedEventArgs e) {
    App.Options.LayoutIsExpanded = false;

    UpdateCommands.UpdateLayoutEditByOptions.Execute(null, null);
  }

  private void LayoutExpander_Expanded(object sender, RoutedEventArgs e) {
    App.Options.LayoutIsExpanded = true;

    UpdateCommands.UpdateLayoutEditByOptions.Execute(null, null);
  }

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IUpdateByProfile.UpdateByCurrentProfile
  public void UpdateByCurrentProfile() {
    this.TargetWindow.UpdateByCurrentProfile();
    this.Area.UpdateByCurrentProfile();
    this.Options.UpdateByCurrentProfile();
    this.ResizeMethod.UpdateByCurrentProfile();
    this.LayoutParameter.UpdateByCurrentProfile();
    this.LayoutTab.UpdateByCurrentProfile();
    this.LayoutEdit.UpdateByCurrentProfile();
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void UpdateByEntireProfile() {
    this.TargetWindow.UpdateByEntireProfile();
    this.Area.UpdateByEntireProfile();
    this.Options.UpdateByEntireProfile();
    this.ResizeMethod.UpdateByEntireProfile();
    this.LayoutParameter.UpdateByEntireProfile();
    this.LayoutTab.UpdateByEntireProfile();
    this.LayoutEdit.UpdateByEntireProfile();
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void AttachProfileChangedEventHandlers() {
    // nop
  }

  /// @copydoc IUpdateByProfile.UpdateByEntireProfile
  public void DetachProfileChangedEventHandlers() {
    // nop
  }

  //===================================================================
  // SCFF.GUI.UpdateCommandsハンドラ
  //===================================================================

  private void UpdateMainWindowByEntireProfile_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.UpdateByEntireProfile();
  }

  private void UpdateLayoutEditByEntireProfile_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutEdit.UpdateByEntireProfile();
  }

  private void UpdateTargetWindowByCurrentProfile_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.TargetWindow.UpdateByCurrentProfile();
    this.Area.UpdateByCurrentProfile();
    this.LayoutEdit.UpdateByCurrentProfile();
    this.LayoutParameter.UpdateByCurrentProfile();
  }

  private void UpdateLayoutParameterByCurrentProfile_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutParameter.UpdateByCurrentProfile();
  }

  private void UpdateLayoutEditByOptions_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.LayoutEdit.UpdateByOptions();
  }

  //===================================================================
  // ApplicationCommandsハンドラ
  //===================================================================

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) すでに保存されていない場合はダイアログをだす
    var save = new SaveFileDialog();
    save.Title = "SCFF.GUI";
    save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
    var saveResult = save.ShowDialog();
    if (saveResult.HasValue && (bool)saveResult) {
      /// @todo(me) 実装
      MessageBox.Show(save.FileName);
      App.Options.AddRecentProfile(save.FileName);
      this.MainMenu.UpdateByOptions();
    }
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
    var result = MessageBox.Show("Do you want to save changes?",
                                 "SCFF.GUI",
                                 MessageBoxButton.YesNoCancel,
                                 MessageBoxImage.Warning,
                                 MessageBoxResult.Yes);
    switch (result) {
      case MessageBoxResult.No: {
        App.Profile.ResetProfile();
        this.UpdateByEntireProfile();
        break;
      }
      case MessageBoxResult.Yes: {
        var save = new SaveFileDialog();
        save.Title = "SCFF.GUI";
        save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
        var saveResult = save.ShowDialog();
        if (saveResult.HasValue && (bool)saveResult) {
          /// @todo(me) 実装
          MessageBox.Show(save.FileName);
          App.Options.AddRecentProfile(save.FileName);
          this.MainMenu.UpdateByOptions();

          App.Profile.ResetProfile();
          this.UpdateByEntireProfile();
        }
        break;
      }
    }
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) Newと似たコードが必要だがかなりめんどくさい。あとでかく
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
    var save = new SaveFileDialog();
    save.Title = "SCFF.GUI";
    save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
    var saveResult = save.ShowDialog();
    if (saveResult.HasValue && (bool)saveResult) {
      /// @todo(me) 実装
      MessageBox.Show(save.FileName);
      App.Options.AddRecentProfile(save.FileName);
      this.MainMenu.UpdateByOptions();
    }
  }

  //===================================================================
  // Windows.Shell.SystemCommandsハンドラ
  //===================================================================
  
	private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.CloseWindow(this);
	}

	private void MaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MaximizeWindow(this);
	}

	private void MinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MinimizeWindow(this);
	}

	private void RestoreWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.RestoreWindow(this);
	}

  //===================================================================
  // SCFF.GUI.Commandsハンドラ
  //===================================================================

  private void SetAero() {
    // @todo(me) 実装
  }

  private bool CanUseAero() {
    // @todo(me) 実装
    return true;
  }

  private void SetAero_Executed(object sender, ExecutedRoutedEventArgs e) {
    Debug.WriteLine("Command [SetAero]:");
    this.SetAero();
  }

  private void SetAero_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = this.CanUseAero();
  }

  //-------------------------------------------------------------------

  private void SetCompactView() {
    if (App.Options.CompactView) {
      this.OptionsExpander.Visibility = Visibility.Collapsed;
      this.ResizeMethodExpander.Visibility = Visibility.Collapsed;
      this.LayoutExpander.IsExpanded = false;
      this.Width = Constants.CompactMainWindowWidth;
      this.Height = Constants.CompactMainWindowHeight;
    } else {
      this.OptionsExpander.Visibility = Visibility.Visible;
      this.ResizeMethodExpander.Visibility = Visibility.Visible;
    }
  }

  private void SetCompactView_Executed(object sender, ExecutedRoutedEventArgs e) {
    Debug.WriteLine("Command [SetCompactView]:");
    this.SetCompactView();
  }
}
}   // namespace SCFF.GUI
