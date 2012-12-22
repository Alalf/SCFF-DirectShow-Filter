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

using System.Windows;
using System.Windows.Controls;
using SCFF.Common;

public partial class MainWindow : Window {

  //===================================================================
  // Options
  //===================================================================

  /// 最近使用したプロファイルメニューの更新
  private void UpdateRecentProfiles() {
    for (int i = 0; i < Constants.RecentProfilesLength; ++i ) {
      var isEmpty = App.Options.GetRecentProfile(i) == string.Empty;
      var header = (i+1) + " " + (isEmpty ? "" : App.Options.GetRecentProfile(i)) +
        "(_" + (i+1) + ")";

      switch (i) {
        case 0:
          this.recentProfile1.IsEnabled = !isEmpty;
          this.recentProfile1.Header = header;
          break;
        case 1:
          this.recentProfile2.IsEnabled = !isEmpty;
          this.recentProfile2.Header = header;
          break;
        case 2:
          this.recentProfile3.IsEnabled = !isEmpty;
          this.recentProfile3.Header = header;
          break;
        case 3:
          this.recentProfile4.IsEnabled = !isEmpty;
          this.recentProfile4.Header = header;
          break;
        case 4:
          this.recentProfile5.IsEnabled = !isEmpty;
          this.recentProfile5.Header = header;
          break;
      }
    }
  }

  /// 設定からUIを更新
  private void UpdateByOptions() {
    // Recent Profiles
    this.UpdateRecentProfiles();

    // MainWindow
    this.Left = App.Options.TmpMainWindowLeft;
    this.Top = App.Options.TmpMainWindowTop;
    this.Width = App.Options.TmpMainWindowWidth;
    this.Height = App.Options.TmpMainWindowHeight;
    this.WindowState = (WindowState)App.Options.TmpMainWindowState;
    
    // MainWindow Expanders
    this.areaExpander.IsExpanded = App.Options.TmpAreaIsExpanded;
    this.optionsExpander.IsExpanded = App.Options.TmpOptionsIsExpanded;
    this.resizeMethodExpander.IsExpanded = App.Options.TmpResizeMethodIsExpanded;
    this.layoutExpander.IsExpanded = App.Options.TmpLayoutIsExpanded;

    // SCFF Options
    this.autoApply.IsChecked = App.Options.AutoApply;
    this.layoutPreview.IsChecked = App.Options.LayoutPreview;
    this.layoutBorder.IsChecked = App.Options.LayoutBorder;
    this.layoutSnap.IsChecked = App.Options.LayoutSnap;

    // SCFF Menu Options
    this.compactView.IsChecked = App.Options.TmpCompactView;
    this.forceAeroOn.IsChecked = App.Options.ForceAeroOn;
    this.restoreLastProfile.IsChecked = App.Options.TmpRestoreLastProfile;
  }

  /// UIから設定にデータを保存
  private void SaveOptions() {
    // Tmp接頭辞のプロパティだけはここで更新する必要がある
    var isNormal = this.WindowState == WindowState.Normal;
    App.Options.TmpMainWindowLeft = isNormal ? this.Left : this.RestoreBounds.Left;
    App.Options.TmpMainWindowTop = isNormal ? this.Top : this.RestoreBounds.Top;
    App.Options.TmpMainWindowWidth = isNormal ? this.Width : this.RestoreBounds.Width;
    App.Options.TmpMainWindowHeight = isNormal ? this.Height : this.RestoreBounds.Height;
    App.Options.TmpMainWindowState = (Options.WindowState)this.WindowState;

    // MainWindow Expanders
    App.Options.TmpAreaIsExpanded = this.areaExpander.IsExpanded;
    App.Options.TmpOptionsIsExpanded = this.optionsExpander.IsExpanded;
    App.Options.TmpResizeMethodIsExpanded = this.resizeMethodExpander.IsExpanded;
    App.Options.TmpLayoutIsExpanded = this.layoutExpander.IsExpanded;

    // SCFF Menu Options
    App.Options.TmpCompactView = this.compactView.IsChecked;
    App.Options.TmpRestoreLastProfile = this.restoreLastProfile.IsChecked;
  }

  //===================================================================
  // Profile
  //===================================================================

  private void AttachClippingChangedEventHandlers() {
    this.clippingX.TextChanged += clippingX_TextChanged;
    this.clippingY.TextChanged += clippingY_TextChanged;
    this.clippingWidth.TextChanged += clippingWidth_TextChanged;
    this.clippingHeight.TextChanged += clippingHeight_TextChanged;
  }

  private void AttachChangedEventHandlers() {
    // CheckBoxはClickイベントとChecked/Uncheckedイベントの使い分けでいけるので除外
    this.AttachClippingChangedEventHandlers();

    this.swscaleFlags.SelectionChanged += swscaleFlags_SelectionChanged;
    this.swscaleLumaGBlur.TextChanged += swscaleLumaGBlur_TextChanged;
    this.swscaleLumaSharpen.TextChanged += swscaleLumaSharpen_TextChanged;
    this.swscaleChromaHshift.TextChanged += swscaleChromaHshift_TextChanged;
    this.swscaleChromaGBlur.TextChanged += swscaleChromaGBlur_TextChanged;
    this.swscaleChromaSharpen.TextChanged += swscaleChromaSharpen_TextChanged;
    this.swscaleChromaVshift.TextChanged += swscaleChromaVshift_TextChanged;
    this.boundRelativeLeft.TextChanged += boundRelativeLeft_TextChanged;
    this.boundRelativeTop.TextChanged += boundRelativeTop_TextChanged;
    this.boundRelativeRight.TextChanged += boundRelativeRight_TextChanged;
    this.boundRelativeBottom.TextChanged += boundRelativeBottom_TextChanged;
  }

  private void DetachClippingChangedEventHandlers() {
    this.clippingX.TextChanged -= clippingX_TextChanged;
    this.clippingY.TextChanged -= clippingY_TextChanged;
    this.clippingWidth.TextChanged -= clippingWidth_TextChanged;
    this.clippingHeight.TextChanged -= clippingHeight_TextChanged;
  }

  private void DetachChangedEventHandlers() {
    // CheckBoxはClickイベントとChecked/Uncheckedイベントの使い分けでいけるので除外
    this.DetachClippingChangedEventHandlers();

    this.swscaleFlags.SelectionChanged -= swscaleFlags_SelectionChanged;
    this.swscaleLumaGBlur.TextChanged -= swscaleLumaGBlur_TextChanged;
    this.swscaleLumaSharpen.TextChanged -= swscaleLumaSharpen_TextChanged;
    this.swscaleChromaHshift.TextChanged -= swscaleChromaHshift_TextChanged;
    this.swscaleChromaGBlur.TextChanged -= swscaleChromaGBlur_TextChanged;
    this.swscaleChromaSharpen.TextChanged -= swscaleChromaSharpen_TextChanged;
    this.swscaleChromaVshift.TextChanged -= swscaleChromaVshift_TextChanged;
    this.boundRelativeLeft.TextChanged -= boundRelativeLeft_TextChanged;
    this.boundRelativeTop.TextChanged -= boundRelativeTop_TextChanged;
    this.boundRelativeRight.TextChanged -= boundRelativeRight_TextChanged;
    this.boundRelativeBottom.TextChanged -= boundRelativeBottom_TextChanged;
  }

  private void UpdateClippingByProfile() {
    this.DetachClippingChangedEventHandlers();
    if (App.Profile.CurrentLayoutElement.Fit) {
      this.clippingX.Text = 0.ToString();
      this.clippingY.Text = 0.ToString();
      this.clippingWidth.Text = App.Profile.CurrentLayoutElement.WindowWidth.ToString();
      this.clippingHeight.Text = App.Profile.CurrentLayoutElement.WindowHeight.ToString();
    } else {
      this.clippingX.Text = App.Profile.CurrentLayoutElement.ClippingX.ToString();
      this.clippingY.Text = App.Profile.CurrentLayoutElement.ClippingY.ToString();
      this.clippingWidth.Text = App.Profile.CurrentLayoutElement.ClippingWidth.ToString();
      this.clippingHeight.Text = App.Profile.CurrentLayoutElement.ClippingHeight.ToString();
    }
    this.AttachClippingChangedEventHandlers();
  }

  /// プロファイルからUIを更新
  private void UpdateByProfile() {
    //-----------------------------------------------------------------
    // *Changedがあるコントロール以外の更新
    //-----------------------------------------------------------------

    // Area
    this.fit.IsChecked = App.Profile.CurrentLayoutElement.Fit;

    // Options
    this.showCursor.IsChecked = App.Profile.CurrentLayoutElement.ShowCursor;
    this.showLayeredWindow.IsChecked = App.Profile.CurrentLayoutElement.ShowLayeredWindow;
    this.keepAspectRatio.IsChecked = App.Profile.CurrentLayoutElement.KeepAspectRatio;
    this.stretch.IsChecked = App.Profile.CurrentLayoutElement.Stretch;
    // @todo(me) overSampingとthreadCountはまだDSFでも実装されていない

    // Resize Method
    this.swscaleAccurateRnd.IsChecked = App.Profile.CurrentLayoutElement.SWScaleAccurateRnd;
    this.swscaleIsFilterEnabled.IsChecked = App.Profile.CurrentLayoutElement.SWScaleIsFilterEnabled;

    //-----------------------------------------------------------------
    // *Changedがあるコントロールの更新
    //-----------------------------------------------------------------

    // 一時的にイベントハンドラを無効にして値だけ設定する
    this.DetachChangedEventHandlers();

    // Window Caption
    this.windowCaption.Text = App.Profile.CurrentLayoutElement.WindowCaption;
    
    // Area
    if (App.Profile.CurrentLayoutElement.Fit) {
      this.clippingX.Text = 0.ToString();
      this.clippingY.Text = 0.ToString();
      this.clippingWidth.Text = App.Profile.CurrentLayoutElement.WindowWidth.ToString();
      this.clippingHeight.Text = App.Profile.CurrentLayoutElement.WindowHeight.ToString();
    } else {
      this.clippingX.Text = App.Profile.CurrentLayoutElement.ClippingX.ToString();
      this.clippingY.Text = App.Profile.CurrentLayoutElement.ClippingY.ToString();
      this.clippingWidth.Text = App.Profile.CurrentLayoutElement.ClippingWidth.ToString();
      this.clippingHeight.Text = App.Profile.CurrentLayoutElement.ClippingHeight.ToString();
    }

    // Resize Method
    var index = Constants.ResizeMethodIndexes[App.Profile.CurrentLayoutElement.SWScaleFlags];
    this.swscaleFlags.SelectedIndex = index;
    this.swscaleLumaGBlur.Text = App.Profile.CurrentLayoutElement.SWScaleLumaGBlur.ToString("F2");
    this.swscaleLumaSharpen.Text = App.Profile.CurrentLayoutElement.SWScaleLumaSharpen.ToString("F2");
    this.swscaleChromaHshift.Text = App.Profile.CurrentLayoutElement.SWScaleChromaHshift.ToString("F2");
    this.swscaleChromaGBlur.Text = App.Profile.CurrentLayoutElement.SWScaleChromaGBlur.ToString("F2");
    this.swscaleChromaSharpen.Text = App.Profile.CurrentLayoutElement.SWScaleChromaSharpen.ToString("F2");
    this.swscaleChromaVshift.Text = App.Profile.CurrentLayoutElement.SWScaleChromaVshift.ToString("F2");

    // Bound *
    this.boundRelativeLeft.Text = App.Profile.CurrentLayoutElement.BoundRelativeLeft.ToString("F3");
    this.boundRelativeTop.Text = App.Profile.CurrentLayoutElement.BoundRelativeTop.ToString("F3");
    this.boundRelativeRight.Text = App.Profile.CurrentLayoutElement.BoundRelativeRight.ToString("F3");
    this.boundRelativeBottom.Text = App.Profile.CurrentLayoutElement.BoundRelativeBottom.ToString("F3");

    /// @todo(me) プロセス情報はMainWindowから取ってこれるので、それを参照にしてBoundX/BoundYも更新
    this.boundX.Text = App.Profile.CurrentLayoutElement.BoundLeft(Constants.DefaultPreviewWidth).ToString();
    this.boundY.Text = App.Profile.CurrentLayoutElement.BoundTop(Constants.DefaultPreviewHeight).ToString();
    this.boundWidth.Text = App.Profile.CurrentLayoutElement.BoundWidth(Constants.DefaultPreviewWidth).ToString();
    this.boundHeight.Text = App.Profile.CurrentLayoutElement.BoundHeight(Constants.DefaultPreviewHeight).ToString();

    // イベントハンドラを戻す
    this.AttachChangedEventHandlers();
  }

  //-------------------------------------------------------------------
  // Tab
  //-------------------------------------------------------------------

  private void AttachTabSelectionChangedEventHandler() {
    this.layoutElementTab.SelectionChanged += layoutElementTab_SelectionChanged;
  }

  private void DetachTabSelectionChangedEventHandler() {
    this.layoutElementTab.SelectionChanged -= layoutElementTab_SelectionChanged;
  }

  /// TabItemを一気に生成する
  private void CreateLayoutElementTab() {
    this.DetachTabSelectionChangedEventHandler();

    // Clear()するとSelectedIndexが変わることに注意
    this.layoutElementTab.Items.Clear();
    for (int i = 0; i < App.Profile.LayoutElementCount; ++i) {
      var item = new TabItem();
      item.Header = (i+1).ToString();
      this.layoutElementTab.Items.Add(item);
    }

    this.AttachTabSelectionChangedEventHandler();
  }

  /// TabItemを末尾にひとつ追加する
  private void AddLayoutElementTab() {
    this.DetachTabSelectionChangedEventHandler();

    var item = new TabItem();
    // 1-basedなのでCount+1
    item.Header = this.layoutElementTab.Items.Count + 1;
    this.layoutElementTab.Items.Add(item);
    // 最後に追加されたので末尾を選択
    this.layoutElementTab.SelectedIndex = this.layoutElementTab.Items.Count - 1;

    this.AttachTabSelectionChangedEventHandler();
  }

  /// 現在のタブをひとつ削除して後ろのタブの名前を変える
  private void RemoveLayoutElementTab() {
    this.DetachTabSelectionChangedEventHandler();

    // 末尾を一個削除
    // 末尾削除なら改名すらいらない
    var last = this.layoutElementTab.Items.Count - 1;
    this.layoutElementTab.Items.RemoveAt(last);

    this.AttachTabSelectionChangedEventHandler();
  }
}
}