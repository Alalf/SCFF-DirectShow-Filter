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

using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Windows.Shell;
using SCFF.Common;

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

    // swscaleFlags
    this.swscaleFlags.Items.Clear();
    foreach (var method in Constants.ResizeMethodLabels) {
      var item = new ComboBoxItem();
      item.Content = method;
      this.swscaleFlags.Items.Add(item);
    }

    //-----------------------------------------------------------------
    // EventHandlers
    //-----------------------------------------------------------------
    this.AttachChangedEventHandlers();
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
    // @todo(me) 実装
    App.Options.AddRecentProfile(System.DateTime.UtcNow.ToString());
    this.UpdateRecentProfiles();
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

    Debug.Assert(this.layoutElementTab.SelectedIndex == App.Profile.CurrentLayoutElement.Index);
    Debug.WriteLine("*********Add!**********");
    Debug.WriteLine("Current Index: " + (App.Profile.CurrentLayoutElement.Index+1));

    this.UpdateByProfile();
  }

  private void AddLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanAddLayoutElement();
  }

  private void RemoveLayoutElement_Executed(object sender, ExecutedRoutedEventArgs e) {
    App.Profile.RemoveCurrentLayoutElement();
    this.RemoveLayoutElementTab();

    Debug.Assert(this.layoutElementTab.SelectedIndex == App.Profile.CurrentLayoutElement.Index);
    Debug.WriteLine("========Remove!=========");
    Debug.WriteLine("Current Index: " + (App.Profile.CurrentLayoutElement.Index+1));
    
    this.UpdateByProfile();
  }

  private void RemoveLayoutElement_CanExecute(object sender, CanExecuteRoutedEventArgs e) {
    e.CanExecute = App.Profile.CanRemoveLayoutElement();
  }

  //===================================================================
  // *Changedではないが、値が変更したときに発生するイベントハンドラ
  // プロパティへの代入で発生するので、App.Profileの変更は禁止
  // このためthis.UpdateByProfile()の必要はない(呼び出しても良い)
  //===================================================================

  private void compactView_Checked(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Collapsed;
    this.resizeMethodExpander.Visibility = Visibility.Collapsed;
    this.layoutExpander.IsExpanded = false;
    this.Width = Constants.CompactMainWindowWidth;
    this.Height = Constants.CompactMainWindowHeight;
  }

  private void compactView_Unchecked(object sender, RoutedEventArgs e) {
    this.optionsExpander.Visibility = Visibility.Visible;
    this.resizeMethodExpander.Visibility = Visibility.Visible;
  }

  private void swscaleIsFilterEnabled_Checked(object sender, RoutedEventArgs e) {
    this.swscaleLumaGBlur.IsEnabled = true;
    this.swscaleLumaSharpen.IsEnabled = true;
    this.swscaleChromaGBlur.IsEnabled = true;
    this.swscaleChromaSharpen.IsEnabled = true;
    /// @todo(me) HshiftおよびVshiftの使い方がわかるまで設定できないように
  }

  private void swscaleIsFilterEnabled_Unchecked(object sender, RoutedEventArgs e) {
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

  //===================================================================
  // *Changed以外のイベントハンドラ
  // プロパティへの代入では発生しないので、
  // この中でのApp.Optionsの変更は許可
  //===================================================================

  private void forceAeroOn_Click(object sender, RoutedEventArgs e) {
    App.Options.ForceAeroOn = this.forceAeroOn.IsChecked;
  }

  private void autoApply_Click(object sender, RoutedEventArgs e) {
    if (this.autoApply.IsChecked.HasValue) {
      App.Options.AutoApply = (bool)this.autoApply.IsChecked;
    }
  }

  private void layoutPreview_Click(object sender, RoutedEventArgs e) {
    if (this.layoutPreview.IsChecked.HasValue) {
      App.Options.LayoutPreview = (bool)this.layoutPreview.IsChecked;
    }
  }

  private void layoutSnap_Click(object sender, RoutedEventArgs e) {
    if (this.layoutSnap.IsChecked.HasValue) {
      App.Options.LayoutSnap = (bool)this.layoutSnap.IsChecked;
    }
  }

  private void layoutBorder_Click(object sender, RoutedEventArgs e) {
    if (this.layoutBorder.IsChecked.HasValue) {
      App.Options.LayoutBorder = (bool)this.layoutBorder.IsChecked;
    }
  }

  private void fit_Click(object sender, RoutedEventArgs e) {
    if (this.fit.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.Fit = (bool)this.fit.IsChecked;

      this.UpdateClippingByProfile();
    }
  }

  private void showCursor_Click(object sender, RoutedEventArgs e) {
    if (this.showCursor.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.ShowCursor = (bool)this.showCursor.IsChecked;
    }
  }

  private void showLayeredWindow_Click(object sender, RoutedEventArgs e) {
    if (this.showLayeredWindow.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.ShowLayeredWindow = (bool)this.showLayeredWindow.IsChecked;
    }
  }

  private void keepAspectRatio_Click(object sender, RoutedEventArgs e) {
    if (this.keepAspectRatio.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.KeepAspectRatio = (bool)this.keepAspectRatio.IsChecked;
    }
  }

  private void stretch_Click(object sender, RoutedEventArgs e) {
    if (this.stretch.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.Stretch = (bool)this.stretch.IsChecked;
    }
  }

  private void swscaleAccurateRnd_Click(object sender, RoutedEventArgs e) {
    if (this.swscaleAccurateRnd.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.SWScaleAccurateRnd = (bool)this.swscaleAccurateRnd.IsChecked;
    }
  }

  private void swscaleIsFilterEnabled_Click(object sender, RoutedEventArgs e) {
    if (this.swscaleIsFilterEnabled.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.SWScaleIsFilterEnabled = (bool)this.swscaleIsFilterEnabled.IsChecked;
    }
  }

  //===================================================================
  // *Changedイベントハンドラ
  // プロパティへの代入で発生する
  // App.Profileの変更も許可するが、これらのイベントハンドラが割り当てられた
  // コントロールのプロパティへの代入はイベントハンドラの一時削除後に
  // 行わなければならない
  //
  /// @todo(me) もうすこしリッチなバリデーションをしても良い
  //===================================================================

  private void layoutElementTab_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    var original = App.Profile.CurrentLayoutElement.Index;
    var next = this.layoutElementTab.SelectedIndex;
    App.Profile.ChangeCurrentLayoutElement(next);

    Debug.WriteLine("Index Changed: " + (original+1) + "->" + (next+1));
    Debug.Assert(this.layoutElementTab.SelectedIndex == App.Profile.CurrentLayoutElement.Index);

    this.UpdateByProfile();
  }

  private void swscaleFlags_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    Profile.SWScaleFlags flags = Constants.ResizeMethodArray[this.swscaleFlags.SelectedIndex];
    App.Profile.CurrentLayoutElement.SWScaleFlags = flags;
  }

  private void clippingX_TextChanged(object sender, TextChangedEventArgs e) {
    // Parse
    int clippingX;
    if (!int.TryParse(this.clippingX.Text, out clippingX)) {
      this.clippingX.Text = "0";
      return;
    }

    // Validation
    if (clippingX < 0) {
      this.clippingX.Text = "0";
      return;
    } else if (App.Profile.CurrentLayoutElement.WindowWidth < clippingX) {
      this.clippingX.Text = App.Profile.CurrentLayoutElement.WindowWidth.ToString();
      return;
    }

    // Profileに書き込み
    App.Profile.CurrentLayoutElement.ClippingX = clippingX;
  }

  private void clippingY_TextChanged(object sender, TextChangedEventArgs e) {
    // Parse
    int clippingY;
    if (!int.TryParse(this.clippingY.Text, out clippingY)) {
      this.clippingY.Text = "0";
      return;
    }

    // Validation
    if (clippingY < 0) {
      this.clippingY.Text = "0";
      return;
    } else if (App.Profile.CurrentLayoutElement.WindowHeight < clippingY) {
      this.clippingY.Text = App.Profile.CurrentLayoutElement.WindowHeight.ToString();
      return;
    }

    // Profileに書き込み
    App.Profile.CurrentLayoutElement.ClippingY = clippingY;
  }

  private void clippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    // Parse
    int clippingWidth;
    if (!int.TryParse(this.clippingWidth.Text, out clippingWidth)) {
      this.clippingWidth.Text = "0";
      return;
    }

    // Validation
    if (clippingWidth < 0) {
      this.clippingWidth.Text = "0";
      return;
    } else if (App.Profile.CurrentLayoutElement.WindowWidth < clippingWidth) {
      this.clippingWidth.Text = App.Profile.CurrentLayoutElement.WindowWidth.ToString();
      return;
    }

    // Profileに書き込み
    App.Profile.CurrentLayoutElement.ClippingWidth = clippingWidth;
  }

  private void clippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    // Parse
    int clippingHeight;
    if (!int.TryParse(this.clippingHeight.Text, out clippingHeight)) {
      this.clippingHeight.Text = "0";
      return;
    }

    // Validation
    if (clippingHeight < 0) {
      this.clippingHeight.Text = "0";
      return;
    } else if (App.Profile.CurrentLayoutElement.WindowHeight < clippingHeight) {
      this.clippingHeight.Text = App.Profile.CurrentLayoutElement.WindowHeight.ToString();
      return;
    }

    // Profileに書き込み
    App.Profile.CurrentLayoutElement.ClippingHeight = clippingHeight;
  }

  private void swscaleLumaGBlur_TextChanged(object sender, TextChangedEventArgs e) {
    //にたようなかんじでかけます
  }

  private void swscaleChromaGBlur_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void swscaleLumaSharpen_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void swscaleChromaSharpen_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void swscaleChromaHshift_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void swscaleChromaVshift_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void boundRelativeLeft_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void boundRelativeTop_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void boundRelativeRight_TextChanged(object sender, TextChangedEventArgs e) {

  }

  private void boundRelativeBottom_TextChanged(object sender, TextChangedEventArgs e) {

  }
}
}
