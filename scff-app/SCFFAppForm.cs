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

    // プロファイルリストのコンボボックスのデータソースを設定
    this.profileList.DataSource = app_.ProfileList;
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
    this.layoutParametersError.UpdateBinding();
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

  private void profileLoad_Click(object sender, EventArgs e) {
    string profile_name = this.profileList.Text; 
    bool no_error = app_.LoadProfile(profile_name);
    if (!no_error) {
      MessageBox.Show("Cannot load profile: " + profile_name);
      return;
    }

    // メッセージ送信
    QuietApply();
  }

  private void profileAdd_Click(object sender, EventArgs e) {
    string profile_name = (string)this.profileList.Text;
    if (!app_.ValidProfileName(profile_name)) {
      MessageBox.Show("Invalid profile name: " + profile_name);
      return;
    }

    bool no_error = app_.AddProfile(profile_name);
    if (no_error) {
      this.profileList.SelectedItem = profile_name;
    }
  }

  private void profileRemove_Click(object sender, EventArgs e) {
    string profile_name = (string)this.profileList.Text;
    bool no_error = app_.RemoveProfile(profile_name);
    if (no_error) {
    }
  }

  //-------------------------------------------------------------------
  // Target/Window
  //-------------------------------------------------------------------

  bool drag_here_mode_;
  UIntPtr current_window_;
  IntPtr current_dc_;
  Graphics current_graphics_;

  static Pen orange_pen_ = new Pen(Brushes.DarkOrange, 8);

  void ClearCurrentWindow() {
    // 描画内容を消しておく
    if (current_window_ != UIntPtr.Zero) {
      ExternalAPI.RECT current_rect;
      ExternalAPI.GetClientRect(current_window_, out current_rect);
      ExternalAPI.InvalidateRect(current_window_, ref current_rect, true);
    }
    // Graphicsを破棄
    if (current_graphics_ != null) {
      current_graphics_.Dispose();
      current_graphics_ = null;
    }
    // DCを破棄
    if (current_dc_ != IntPtr.Zero) {
      ExternalAPI.ReleaseDC(current_window_, current_dc_);
      current_dc_ = IntPtr.Zero;
    }
  }

  private void windowDragHere_MouseDown(object sender, MouseEventArgs e) {
    this.windowDragHere.BackColor = Color.Orange;

    drag_here_mode_ = true;
    current_window_ = UIntPtr.Zero;
    current_dc_ = IntPtr.Zero;
    current_graphics_ = null;
  }

  private void windowDragHere_MouseMove(object sender, MouseEventArgs e) {
    if (!drag_here_mode_) {
      // drag_hereモードでなければ何もしない
      return;
    }

    Point screen_location = this.windowDragHere.PointToScreen(e.Location);
    UIntPtr next_window = ExternalAPI.WindowFromPoint(screen_location.X, screen_location.Y);
    if (next_window == UIntPtr.Zero) {
      // nop
      return;
    }

    if (current_window_ == UIntPtr.Zero) {
      // 初回実行: どうせDragHereボタンが取得されているはずなので、一回は無視
      current_window_ = next_window;
    }

    if (next_window == current_window_) {
      // 対象ウィンドウが変わっていなければ何も描画する必要はない
      return;
    }

    // ウィンドウが異なる場合、描画していたウィンドウをアップデートして描画内容を消しておく
    ClearCurrentWindow();

    // 現在処理中のウィンドウを更新
    current_window_ = next_window;
    current_dc_ = ExternalAPI.GetDC(current_window_);
    current_graphics_ = System.Drawing.Graphics.FromHdc(current_dc_);
   
    // 描画
    ExternalAPI.RECT current_rect;
    ExternalAPI.GetClientRect(current_window_, out current_rect);
    current_graphics_.DrawRectangle(orange_pen_,
                                   current_rect.left, 
                                   current_rect.top,
                                   current_rect.right - current_rect.left,
                                   current_rect.bottom - current_rect.top);    
  }

  private void windowDragHere_MouseUp(object sender, MouseEventArgs e) {
    this.windowDragHere.BackColor = SystemColors.Control;

    // 後処理
    drag_here_mode_ = false;
    ClearCurrentWindow();

    // マウスカーソルからウィンドウを取得
    Point screen_location = this.windowDragHere.PointToScreen(e.Location);
    app_.SetWindowFromPoint(screen_location.X, screen_location.Y);

    // メッセージ送信
    QuietApply();
  }

  private void windowDesktop_Click(object sender, EventArgs e) {
    app_.SetDesktopWindow();

    // メッセージ送信
    QuietApply();
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
      if (form.DialogResult == System.Windows.Forms.DialogResult.OK) {
        // メッセージ送信
        QuietApply();
      }
    }
  }

  //-------------------------------------------------------------------
  // Layout
  //-------------------------------------------------------------------

  private void layoutEdit_Click(object sender, EventArgs e) {
    using (LayoutForm form = new LayoutForm(this.entries, this.layoutParameters)) {
      form.ShowDialog();
      if (form.DialogResult == System.Windows.Forms.DialogResult.OK) {
        // メッセージ送信
        QuietApply();
      }
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
  }

  //-------------------------------------------------------------------

  void QuietApply() {
    /// @todo(me) 汚すぎる！dirtyフラグは別に管理するべき
    /// @warning メッセージは上書きされるだけならば
    ///          処理時間以外の問題は発生しない
    bool dirty = this.apply.BackColor == Color.Orange;
    if (app_ != null && this.autoApply.Checked && dirty) {
      app_.SendMessage(false);
      this.layoutParametersError.UpdateBinding();
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

