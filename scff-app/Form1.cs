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
  SCFFApp app_;

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

    // SCFFAppインスタンスを生成
    app_ = new SCFFApp();
    
    // 初期設定
    app_.UpdateDirectory(this.entries);

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
    if (!app_.CheckEnvironment()) {
      Close();
      return;
    }

    // AeroをOffにしようと試みる
    app_.DWMAPIOff();
  }
  private void Form1_FormClosed(object sender, FormClosedEventArgs e) {
    // Aeroの状態を元に戻す
    app_.DWMAPIRestore();
  }

  //-------------------------------------------------------------------
  // メニュー
  //-------------------------------------------------------------------

  private void aeroOn_Click(object sender, EventArgs e) {
    // Aeroの状態を切り替える
    app_.DWMAPIFlip(aeroOn.Checked);

    aeroOn.Checked = !aeroOn.Checked;
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    app_.SendNull(this.entries, true);
  }

  private void apply_Click(object sender, EventArgs e) {
    app_.SendMessage(this.entries, this.layoutParameters, true);
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void processRefresh_Click(object sender, EventArgs e) {
    // entiresを更新
    app_.UpdateDirectory(this.entries);
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
    app_.SetWindowFromPoint(this.layoutParameters, screen_location.X, screen_location.Y);
  }

  private void windowDesktop_Click(object sender, EventArgs e) {
    app_.SetDesktopWindow(this.layoutParameters);
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
    // AreaSelectFormの表示
    using (gui.AreaSelectForm form = new gui.AreaSelectForm(layoutParameters)) {
      form.ShowDialog();
      // DialogResult result = form.DialogResult;
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  private void layoutEdit_Click(object sender, EventArgs e) {
    // 一応ダミーで調整できるように
    /// @todo(me) ダミーではなくもっとよい方法がないか考え中
    int bound_width = SCFFApp.kDefaultBoundWidth;
    int bound_height = SCFFApp.kDefaultBoundHeight;
    if (entries.Count != 0) {
      bound_width = ((data.Entry)entries.Current).SampleWidth;
      bound_height = ((data.Entry)entries.Current).SampleHeight;
    }


    using (gui.LayoutForm layout_form = new gui.LayoutForm(this.layoutParameters, bound_width, bound_height)) {
      layout_form.ShowDialog();
      layout_form.GetResult();
    }
  }

  private void resizeMethodIsFilterEnabled_CheckedChanged(object sender, EventArgs e) {
    bool is_filter_enabled = resizeMethodIsFilterEnabled.Checked;

    this.resizeMethodLGBlur.Enabled = is_filter_enabled;
    this.resizeMethodCGBlur.Enabled = is_filter_enabled;
    this.resizeMethodLSharpen.Enabled = is_filter_enabled;
    this.resizeMethodCSharpen.Enabled = is_filter_enabled;
    this.resizeMethodCHShift.Enabled = is_filter_enabled;
    this.resizeMethodCVShift.Enabled = is_filter_enabled;
  }

  private void kLayoutParametersNavigator_RefreshItems(object sender, EventArgs e) {
    // １個以下にはできない
    // 最大値になるまでは追加できるし削除もできる
    // 最大値になったら追加はできない
    bool can_edit = this.layoutParameters.Count > 1;
    bool can_add = this.layoutParameters.Count <
        scff_interprocess.Interprocess.kMaxComplexLayoutElements;
    bool can_remove = this.layoutParameters.Count > 1;

    this.layoutEdit.Enabled = can_edit;
    this.layoutBoundRelativeTop.Enabled = can_edit;
    this.layoutBoundRelativeBottom.Enabled = can_edit;
    this.layoutBoundRelativeLeft.Enabled = can_edit;
    this.layoutBoundRelativeRight.Enabled = can_edit;

    this.layoutAdd.Enabled = can_add;
    this.layoutRemove.Enabled = can_remove;
  }

  private void entries_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    bool entry_exists = this.entries.Count != 0;

    this.processList.Enabled = entry_exists;
    this.splash.Enabled = entry_exists;
    this.apply.Enabled = entry_exists;
    this.autoApply.Enabled = entry_exists;
  }

  private void layoutParameters_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    if (this.autoApply.Checked) {
      MessageBox.Show("layoutParameters_CurrentItemChanged");
    }
  }
}
}   // namespace scff_app

