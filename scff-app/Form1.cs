// Copyright 2012 Progre <djyayutto_at_gmail.com>
//
// This file is part of SCFF DSF.
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

/// @file Form1.cs
/// @brief Form1のUIに関連するメソッドの定義
/// @todo(progre) 移植未達部分が完了次第名称含め全体をリファクタリング

namespace scff_app {

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

/// @brief メインウィンドウ
public partial class Form1 : Form {

  /// @brief アプリケーションの実装(MVCパターンにおけるModel/Controller)
  AppImplementation impl_;

  /// @brief コンストラクタ
  public Form1() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    // リサイズメソッドのコンボボックスのデータソースを設定
    resizeMethodList.DisplayMember = "Value";
    resizeMethodList.ValueMember = "Key";
    List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> resize_method_list =
        new List<KeyValuePair<scff_interprocess.SWScaleFlags,string>>
            (data.SWScaleConfig.ResizeMethodList);
    resizeMethodList.DataSource = resize_method_list;

    // AppImplementationインスタンスを生成
    impl_ = new AppImplementation();
    
    // 初期設定
    impl_.UpdateDirectory(ref this.entries);

    // デフォルトの設定を書き込む
    this.layoutParameters.AddNew();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------
  private void Form1_Shown(object sender, EventArgs e) {
    // 起動時のチェック
    if (!impl_.CheckEnvironment()) {
      Close();
      return;
    }

    // AeroをOffにしようと試みる
    impl_.DWMAPIOff();
  }
  private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
    // Aeroの状態を元に戻す
    impl_.DWMAPIRestore();
  }

  //-------------------------------------------------------------------
  // メニュー
  //-------------------------------------------------------------------

  private void aeroOn_Click(object sender, EventArgs e) {
    // Aeroの状態を切り替える
    impl_.DWMAPIFlip(aeroOn.Checked);

    aeroOn.Checked = !aeroOn.Checked;
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    impl_.SendNull(ref entries, true);
  }

  private void apply_Click(object sender, EventArgs e) {
    impl_.SendMessage(ref entries,
                      ref layoutParameters, true);
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void processRefresh_Click(object sender, EventArgs e) {
    // entiresを更新
    impl_.UpdateDirectory(ref this.entries);
  }

  //-------------------------------------------------------------------
  // Layout Profile
  //-------------------------------------------------------------------
  private void profileAdd_Click(object sender, EventArgs e) {
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  private void windowDragHere_MouseDown(object sender, MouseEventArgs e) {
    windowDragHere.BackColor = Color.Orange;
  }

  private void windowDragHere_MouseUp(object sender, MouseEventArgs e) {
    windowDragHere.BackColor = SystemColors.Control;

    Point screen_location = windowDragHere.PointToScreen(e.Location);
    impl_.SetWindowFromPoint(ref layoutParameters, screen_location.X, screen_location.Y);
  }

  private void windowDesktop_Click(object sender, EventArgs e) {
    impl_.SetDesktopWindow(ref layoutParameters);
  }

  //-------------------------------------------------------------------
  // Target/Area
  //-------------------------------------------------------------------
  private void areaFit_CheckedChanged(object sender, EventArgs e) {
    bool flag = !this.areaFit.Checked;
    this.areaClippingX.Enabled = flag;
    this.areaClippingY.Enabled = flag;
    this.areaClippingWidth.Enabled = flag;
    this.areaClippingHeight.Enabled = flag;
  }

  private void targetAreaSelect_Click(object sender, EventArgs e) {
    // AreaSelectFormを利用してクリッピング領域を取得
    int raw_x, raw_y, raw_width, raw_height;

    /// @todo(me) マルチモニタ対応
    ExternalAPI.RECT window_rect;
    ExternalAPI.GetClientRect(ExternalAPI.GetDesktopWindow(), out window_rect);
    int bound_width = window_rect.right;
    int bound_height = window_rect.bottom;

    // ダイアログの表示
    using (gui.AreaSelectForm form =
        new gui.AreaSelectForm(bound_width, bound_height, ref layoutParameters)) {
      form.ShowDialog();
      form.GetResult(out raw_x, out raw_y, out raw_width, out raw_height);
    }

    // 修正
    int clipping_x, clipping_y, clipping_width, clipping_height;
    impl_.JustifyClippingRegion(
        bound_width, bound_height,
        raw_x, raw_y, raw_width, raw_height,
        out clipping_x, out clipping_y, out clipping_width, out clipping_height);

    // プロパティに設定
    ((data.LayoutParameter)layoutParameters.Current).SetWindowFromPtr(
      ExternalAPI.GetDesktopWindow(),
      false,
      clipping_x, clipping_y, clipping_width, clipping_height);
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  private void layoutEdit_Click(object sender, EventArgs e) {
    int bound_width, bound_height;
    if (entries.Count == 0) {
      // 一応ダミーで調整できるように
      bound_width = 640;
      bound_height = 360;
    } else {
      bound_width = ((data.Entry)entries.Current).SampleWidth;
      bound_height = ((data.Entry)entries.Current).SampleHeight;
    }

    using (gui.LayoutForm layout_form = new gui.LayoutForm(ref layoutParameters, bound_width, bound_height)) {
      layout_form.ShowDialog();
      layout_form.GetResult();
    }
  }

  private void resizeMethodIsFilterEnabled_CheckedChanged(object sender, EventArgs e) {
    bool state = resizeMethodIsFilterEnabled.Checked;
    resizeMethodLGBlur.Enabled = state;
    resizeMethodCGBlur.Enabled = state;
    resizeMethodLSharpen.Enabled = state;
    resizeMethodCSharpen.Enabled = state;
    resizeMethodCHShift.Enabled = state;
    resizeMethodCVShift.Enabled = state;
  }

  private void kLayoutParametersNavigator_RefreshItems(object sender, EventArgs e) {
    if (layoutParameters.Count <= 1) {
      // １個以下にはできない
      layoutBoundRelativeTop.Enabled = false;
      layoutBoundRelativeBottom.Enabled = false;
      layoutBoundRelativeLeft.Enabled = false;
      layoutBoundRelativeRight.Enabled = false;

      layoutAdd.Enabled = true;
      layoutRemove.Enabled = false;
      layoutEdit.Enabled = false;
    } else if (layoutParameters.Count <
        scff_interprocess.Interprocess.kMaxComplexLayoutElements) {
      // 最大値になるまでは要素を追加できるし削除もできる
      layoutBoundRelativeTop.Enabled = true;
      layoutBoundRelativeBottom.Enabled = true;
      layoutBoundRelativeLeft.Enabled = true;
      layoutBoundRelativeRight.Enabled = true;

      layoutEdit.Enabled = true;
      layoutAdd.Enabled = true;
      layoutRemove.Enabled = true;
    } else {
      // 最大値になったので追加はできない
      layoutAdd.Enabled = false;
      layoutRemove.Enabled = true;
      layoutEdit.Enabled = true;
    }
  }

  private void entries_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    bool entry_exists = this.entries.Count != 0;

    processList.Enabled = entry_exists;
    splash.Enabled = entry_exists;
    apply.Enabled = entry_exists;
    autoApply.Enabled = entry_exists;
  }

  private void layoutParameters_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    if (this.autoApply.Checked) {
      MessageBox.Show("layoutParameters_CurrentItemChanged");
    }
  }
}
}   // namespace scff_app

