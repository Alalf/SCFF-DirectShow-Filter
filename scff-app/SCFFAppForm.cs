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

/// @file SCFFAppForm.cs
/// @brief SCFFAppFormのUIに関連するメソッドの定義
/// @todo(progre) 移植未達部分が完了次第名称含め全体をリファクタリング

namespace scff_app {

using System;
using System.Drawing;
using System.Windows.Forms;

using scff_app.view;

/// @brief メインウィンドウ
public partial class SCFFAppForm : Form {

  /// @brief コンストラクタ
  public SCFFAppForm() {
    //---------------------------------------------------------------
    // DO NOT DELETE THIS!!!
    InitializeComponent();
    //---------------------------------------------------------------

    // SCFFAppインスタンスを生成
    app_ = new SCFFApp(this.entries, this.layoutParameters);
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // フォーム
  //-------------------------------------------------------------------

  private void SCFFAppForm_Load(object sender, EventArgs e) {
    //アプリケーションの設定を読み込む
    Properties.Settings.Default.Reload();

    // リサイズメソッドのコンボボックスのデータソースを設定
    this.resizeMethodList.DisplayMember = "Value";
    this.resizeMethodList.ValueMember = "Key";
    this.resizeMethodList.DataSource = app_.ResizeMethodList;

    app_.OnLoad();
  }

  private void SCFFAppForm_Shown(object sender, EventArgs e) {
    // 起動時のチェック
    if (!app_.CheckEnvironment()) {
      this.Close();
      return;
    }

    // AeroをOffにしようと試みる
    app_.DWMAPIOff();
  }

  private void SCFFAppForm_FormClosed(object sender, FormClosedEventArgs e) {
    // Aeroの状態を元に戻す
    app_.DWMAPIRestore();

    //アプリケーションの設定を保存する
    Properties.Settings.Default.Save();
  }

  //-------------------------------------------------------------------
  // メニュー
  //-------------------------------------------------------------------

  private void aeroOn_Click(object sender, EventArgs e) {
    // Aeroの状態を切り替える
    app_.DWMAPIFlip(aeroOn.Checked);

    this.aeroOn.Checked = !this.aeroOn.Checked;
  }

  //-------------------------------------------------------------------
  // OK/CANCEL
  //-------------------------------------------------------------------
  private void splash_Click(object sender, EventArgs e) {
    app_.SendNull(true);
    this.apply.BackColor = Color.Orange;
  }

  private void apply_Click(object sender, EventArgs e) {
    app_.SendMessage(true);
    this.apply.BackColor = SystemColors.Control;
  }

  //-------------------------------------------------------------------
  // Process
  //-------------------------------------------------------------------
  private void processRefresh_Click(object sender, EventArgs e) {
    // entiresを更新
    app_.UpdateDirectory();
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
    this.windowDragHere.BackColor = Color.Orange;
  }

  private void windowDragHere_MouseUp(object sender, MouseEventArgs e) {
    this.windowDragHere.BackColor = SystemColors.Control;

    Point screen_location = windowDragHere.PointToScreen(e.Location);
    app_.SetWindowFromPoint(screen_location.X, screen_location.Y);
  }

  private void windowDesktop_Click(object sender, EventArgs e) {
    app_.SetDesktopWindow();
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
    using (AreaSelectForm form = new AreaSelectForm(this.layoutParameters)) {
      form.ShowDialog();
      // DialogResult result = form.DialogResult;
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  private void layoutEdit_Click(object sender, EventArgs e) {
    using (LayoutForm form = new LayoutForm(this.entries, this.layoutParameters)) {
      form.ShowDialog();
      // DialogResult result = form.DialogResult;
    }
  }

  private void resizeMethodIsFilterEnabled_CheckedChanged(object sender, EventArgs e) {
    bool is_filter_enabled = this.resizeMethodIsFilterEnabled.Checked;

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
    bool can_add = this.layoutParameters.Count < SCFFApp.kMaxLayoutElements;
    bool can_remove = this.layoutParameters.Count > 1;

    this.layoutEdit.Enabled = can_edit;
    this.layoutBoundRelativeTop.Enabled = can_edit;
    this.layoutBoundRelativeBottom.Enabled = can_edit;
    this.layoutBoundRelativeLeft.Enabled = can_edit;
    this.layoutBoundRelativeRight.Enabled = can_edit;

    this.layoutAdd.Enabled = can_add;
    this.layoutRemove.Enabled = can_remove;
  }

  //-------------------------------------------------------------------
  // BindingSource
  //-------------------------------------------------------------------

  private void entries_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    bool entry_exists = this.entries.Count != 0;

    this.processList.Enabled = entry_exists;
    this.splash.Enabled = entry_exists;
    this.apply.Enabled = entry_exists;
    this.autoApply.Enabled = entry_exists;
  }

  private void layoutParameters_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
    this.apply.BackColor = Color.Orange;
  }

  //-------------------------------------------------------------------
  // Timer
  //-------------------------------------------------------------------
  private void mainTimer_Tick(object sender, EventArgs e) {
    // @todo(me) プロセスの生死をチェック
    // @todo(me) ウィンドウをチェック
    // @todo(me) ウィンドウサイズをチェック

    // メッセージ送信
    /// @todo(me) 汚すぎる！dirtyフラグは別に管理するべき
    /// @warning タイマーでちょうど具合の悪いタイミング（ClippingXとY設定後、WidthHeight設定前）
    ///          でMessageをSendしてしまう恐れがある。ちゃんとやるならLockを使うしかない。
    bool dirty = this.apply.BackColor == Color.Orange;
    if (app_ != null && this.autoApply.Checked && dirty) {
      app_.SendMessage(false);
      this.apply.BackColor = SystemColors.Control;
    }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  /// @brief MVCパターンにおけるController
  SCFFApp app_;
}
}   // namespace scff_app

