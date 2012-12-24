// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
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

namespace SCFF.GUI {

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Shell;
using SCFF.Common;
using Microsoft.Win32;

/// MainWindowのコードビハインド
public partial class MainWindow : Window {

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// コンストラクタ
  public MainWindow() {    
    //-----------------------------------------------------------------
    // Controls
    //-----------------------------------------------------------------
    this.InitializeComponent();
  }

  /// 全ウィンドウ表示前に一度だけ起こるLoadedイベントハンドラ
  private void mainWindow_Loaded(object sender, RoutedEventArgs e) {
    this.UpdateByOptions();
    this.UpdateByProfile();
  }

  /// アプリケーション終了時に発生するClosingイベントハンドラ
  private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
    this.SaveOptions();
  }

  //===================================================================
  // ApplicationCommandsハンドラ
  //===================================================================

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    /// @todo(me) すでに保存されていない場合はダイアログをだす
    if ((string)this.Tag == "") {
      var save = new SaveFileDialog();
      save.Title = "SCFF.GUI";
      save.Filter = "SCFF.GUI Profile|*.SCFF.GUI.profile";
      var saveResult = save.ShowDialog();
      if (saveResult.HasValue && (bool)saveResult) {
        /// @todo(me) 実装
        MessageBox.Show(save.FileName);
        App.Options.AddRecentProfile(save.FileName);
        this.UpdateRecentProfiles();
      }
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
        this.LayoutTab.ResetTabs();
        this.UpdateByProfile();
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
          this.UpdateRecentProfiles();

          App.Profile.ResetProfile();
          this.LayoutTab.ResetTabs();
          this.UpdateByProfile();
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
      this.UpdateRecentProfiles();
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

  private void AddLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.AddLayoutElement();
    this.LayoutTab.AddTab();

    this.UpdateByProfile();
  }

  private void AddLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanAddLayoutElement();
  }

  private void RemoveLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();
    this.LayoutTab.RemoveCurrentTab();
    
    this.UpdateByProfile();
  }

  private void RemoveLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanRemoveLayoutElement();
  }

  private void ChangeCurrentLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    this.UpdateByProfile();
  }

  //===================================================================
  // *Changedではないが、値が変更したときに発生するイベントハンドラ
  // プロパティへの代入で発生するので、App.Profileの変更は禁止
  // このためthis.UpdateByProfile()の必要はない(呼び出しても良い)
  //===================================================================

  private void compactView_Checked(object sender, RoutedEventArgs e) {
    this.OptionsExpander.Visibility = Visibility.Collapsed;
    this.ResizeMethodExpander.Visibility = Visibility.Collapsed;
    this.LayoutExpander.IsExpanded = false;
    this.Width = Constants.CompactMainWindowWidth;
    this.Height = Constants.CompactMainWindowHeight;
  }

  private void compactView_Unchecked(object sender, RoutedEventArgs e) {
    this.OptionsExpander.Visibility = Visibility.Visible;
    this.ResizeMethodExpander.Visibility = Visibility.Visible;
  }

  //===================================================================
  // *Changed以外のイベントハンドラ
  // プロパティへの代入では発生しないので、
  // この中でのApp.Optionsの変更は許可
  //===================================================================

  private void forceAeroOn_Click(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = this.ForceAeroOn.IsChecked;
  }

  private void autoApply_Click(object sender, RoutedEventArgs e) {
    if (this.AutoApply.IsChecked.HasValue) {
      App.Options.AutoApply = (bool)this.AutoApply.IsChecked;
    }
  }

  private void layoutPreview_Click(object sender, RoutedEventArgs e) {
    if (this.LayoutPreview.IsChecked.HasValue) {
      App.Options.LayoutPreview = (bool)this.LayoutPreview.IsChecked;
    }
  }

  private void layoutSnap_Click(object sender, RoutedEventArgs e) {
    if (this.LayoutSnap.IsChecked.HasValue) {
      App.Options.LayoutSnap = (bool)this.LayoutSnap.IsChecked;
    }
  }

  private void layoutBorder_Click(object sender, RoutedEventArgs e) {
    if (this.LayoutBorder.IsChecked.HasValue) {
      App.Options.LayoutBorder = (bool)this.LayoutBorder.IsChecked;
    }
  }

}
}
