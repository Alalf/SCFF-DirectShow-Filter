// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

using Microsoft.Windows.Shell;
using System.Windows;
using System.Windows.Input;
using SCFF.Common;
using System.Windows.Controls;
  using System.Diagnostics;
  using System;

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

    // resizeMethod
    this.resizeMethod.Items.Clear();
    foreach (var method in Constants.ResizeMethods) {
      var item = new ComboBoxItem();
      item.Tag = method.Key;
      item.Content = method.Value;
      this.resizeMethod.Items.Add(item);
    }
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
  // メニューイベントハンドラ
  //===================================================================

  private void MenuItem_Unchecked_1(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Visible;
    this.resizeMethodExpander.Visibility = Visibility.Visible;
  }

  private void MenuItem_Checked_1(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Collapsed;
    this.resizeMethodExpander.Visibility = Visibility.Collapsed;
    this.layoutExpander.IsExpanded = false;
    this.Width = Constants.CompactMainWindowWidth;
    this.Height = Constants.CompactMainWindowHeight;
  }

  //===================================================================
  // コントロールイベントハンドラ
  //===================================================================

  private void autoApply_Checked(object sender, RoutedEventArgs e) {
    App.Options.AutoApply = true;
  }

  private void autoApply_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.AutoApply = false;
  }

  private void forceAeroOn_Checked(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = true;
  }

  private void forceAeroOn_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = false;
  }

  private void layoutPreview_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutPreview = true;
  }

  private void layoutPreview_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutPreview = false;
  }

  private void layoutBorder_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutBorder = true;
  }

  private void layoutBorder_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutBorder = false;
  }

  private void layoutSnap_Unchecked(object sender, RoutedEventArgs e) {
    App.Options.LayoutSnap = false;
  }

  private void layoutSnap_Checked(object sender, RoutedEventArgs e) {
    App.Options.LayoutSnap = true;
  }

  private void enableFilter_Checked(object sender, RoutedEventArgs e) {
    this.swscaleLumaGBlur.IsEnabled = true;
    this.swscaleLumaSharpen.IsEnabled = true;
    this.swscaleChromaGBlur.IsEnabled = true;
    this.swscaleChromaSharpen.IsEnabled = true;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  private void enableFilter_Unchecked(object sender, RoutedEventArgs e) {
    this.swscaleLumaGBlur.IsEnabled = false;
    this.swscaleLumaSharpen.IsEnabled = false;
    this.swscaleChromaGBlur.IsEnabled = false;
    this.swscaleChromaSharpen.IsEnabled = false;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  private void fit_Checked(object sender, RoutedEventArgs e) {
    this.clippingX.IsEnabled = false;
    this.clippingY.IsEnabled = false;
    this.clippingWidth.IsEnabled = false;
    this.clippingHeight.IsEnabled = false;
  }

  private void fit_Unchecked(object sender, RoutedEventArgs e) {
    this.clippingX.IsEnabled = true;
    this.clippingY.IsEnabled = true;
    this.clippingWidth.IsEnabled = true;
    this.clippingHeight.IsEnabled = true;
  }

  private void layoutElementTab_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    if (!App.Profile.IsInitialized) {
      return;
    }

    // Parse
    var index = this.layoutElementTab.SelectedIndex;

    // Validate
    if (index < 0) {
      // SelectedIndexは-1に一時的になる可能性があるので、その場合は処理は行わない。
      return;
    }

    // コピー
    Debug.WriteLine("Index Changed: " + (App.Profile.CurrentLayoutElement.Index+1) + "->" + (index+1));
    App.Profile.ChangeCurrentLayoutElement(index);
    Debug.Assert(this.layoutElementTab.SelectedIndex == App.Profile.CurrentLayoutElement.Index);
    this.UpdateByProfile();
  }


  private void clippingX_TextChanged(object sender, TextChangedEventArgs e) {
    if (!App.Profile.IsInitialized) {
      return;
    }

    // Parse
    int clippingX;
    if (!int.TryParse(this.clippingX.Text, out clippingX)) {
      this.clippingX.Text = "0";
      return;
    }

    // Validation
    /// @todo(me) Validation処理も自分で書くならここしかない
    if (clippingX < 0) {
      this.clippingX.Text = "0";
      return;
    }

    // Profileに書き込み
    var original = App.Profile.CurrentLayoutElement.ClippingX;
    if (original != clippingX) {
      Debug.WriteLine("ClippingX: " + original + "->" + clippingX);
      App.Profile.CurrentLayoutElement.ClippingX = clippingX;
    }
  }

  //===================================================================
  // ApplicationCommandsハンドラ
  //===================================================================

  private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
    //App.Options.AddRecentProfile(e.ToString());
    //this.UpdateRecentProfiles();
  }

  private void New_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void Open_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  private void SaveAs_Executed(object sender, ExecutedRoutedEventArgs e) {
  }

  //===================================================================
  // Windows.Shell.SystemCommandsハンドラ
  //===================================================================
  
	private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.CloseWindow((Window)e.Parameter);
	}

	private void MaximizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MaximizeWindow((Window)e.Parameter);
	}

	private void MinimizeWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.MinimizeWindow((Window)e.Parameter);
	}

	private void RestoreWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
		SystemCommands.RestoreWindow((Window)e.Parameter);
	}

  //===================================================================
  // SCFF.GUI.Commandsハンドラ
  //===================================================================

  private void AddLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.AddLayoutElement();
    this.AddLayoutElementTab();
    Debug.WriteLine("*********Add!**********");
    this.layoutElementTab.SelectedIndex = App.Profile.CurrentLayoutElement.Index;
    this.UpdateByProfile();
  }

  private void AddLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanAddLayoutElement();
  }

  private void RemoveLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();
    Debug.WriteLine("========Remove=========");
    this.RemoveLayoutElementTab();
    Debug.WriteLine("--------Remove/Tab---------");
    Debug.Assert(this.layoutElementTab.SelectedIndex == App.Profile.CurrentLayoutElement.Index);
    this.UpdateByProfile();
  }

  private void RemoveLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanRemoveLayoutElement();
  }
}
}
