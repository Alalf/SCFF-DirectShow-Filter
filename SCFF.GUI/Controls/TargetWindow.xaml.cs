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

/// @file SCFF.GUI/Controls/TargetWindow.xaml.cs
/// @copydoc SCFF::GUI::Controls::TargetWindow

namespace SCFF.GUI.Controls {

using System;
using System.Diagnostics;
using System.Windows.Controls;
using SCFF.Common.Ext;
using SCFF.Common.GUI;
using SCFF.Common.Profile;

/// Window取り込み対象の設定用UserControl
public partial class TargetWindow : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public TargetWindow() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  /// プロファイルを更新
  private void ModifyProfile(WindowTypes nextWindowType, UIntPtr nextTargetWindow) {
    App.Profile.Current.Open();

    // Window
    switch (nextWindowType) {
      case WindowTypes.Normal: {
        App.Profile.Current.SetWindow(nextTargetWindow);
        break;
      }
      case WindowTypes.DesktopListView: {
        App.Profile.Current.SetWindowToDesktopListView();
        break;
      }
      case WindowTypes.Desktop: {
        App.Profile.Current.SetWindowToDesktop();
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
    App.Profile.Current.Fit = true;
    App.Profile.Current.ClearBackupParameters();
    App.Profile.Current.Close();

    //-----------------------------------------------------------------
    // Notify self
    this.OnCurrentLayoutElementChanged();
    // Notify other controls
    Commands.AreaChanged.Execute(null, this);
    //-----------------------------------------------------------------
  }

  /// Desktop: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void Desktop_Click(object sender, System.Windows.RoutedEventArgs e) {
    // Profileを更新
    this.ModifyProfile(WindowTypes.Desktop, UIntPtr.Zero);
  }

  /// ListView: Click
  /// @param sender 使用しない
  /// @param e 使用しない
  private void ListView_Click(object sender, System.Windows.RoutedEventArgs e) {
    // Profileを更新
    this.ModifyProfile(WindowTypes.DesktopListView, UIntPtr.Zero);
  }

  //-------------------------------------------------------------------

  /// 現在の対象ウィンドウのClient領域をXORで塗りつぶす
  private void XorTargetWindowRect() {
    User32.RECT currentTargetRect;
    User32.GetClientRect(this.currentTargetWindow, out currentTargetRect);
    
    var originalDrawMode = GDI32.SetROP2(this.currentTargetDC, GDI32.R2_XORPEN);
    var originalPen = GDI32.SelectObject(this.currentTargetDC, App.NullPen.Pen);

    GDI32.Rectangle(this.currentTargetDC,
        currentTargetRect.Left, currentTargetRect.Top,
        currentTargetRect.Right, currentTargetRect.Bottom);

    GDI32.SelectObject(this.currentTargetDC, originalPen);
    GDI32.SetROP2(this.currentTargetDC, originalDrawMode);
  }

  /// 現在の対象Windowの塗りつぶし(XOR)を解除する
  private void ClearTargetRect() {
    if (this.currentTargetWindow != UIntPtr.Zero) {
      // 取り込み対象範囲のXOR描画(もともとXORされていたので元に戻る)
      this.XorTargetWindowRect();
    }
    // DCを破棄
    if (this.currentTargetDC != IntPtr.Zero) {
      User32.ReleaseDC(this.currentTargetWindow, this.currentTargetDC);
      this.currentTargetDC = IntPtr.Zero;
    }
  }

  /// DragHere: PreviewMouseDown
  /// @param sender 使用しない
  /// @param e 使用しない
  private void DragHere_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
    if (this.dragHereMode) return;

    this.DragHere.Tag = "Emphasize";

    this.dragHereMode = true;
    this.currentTargetWindow = UIntPtr.Zero;
    this.currentTargetDC = IntPtr.Zero;
  }

  /// DragHere: PreviewMouseMove
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void DragHere_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
   if (!this.dragHereMode) return;

    var screenPoint = this.DragHere.PointToScreen(e.GetPosition(this.DragHere));
    UIntPtr nextTargetWindow = User32.WindowFromPoint((int)screenPoint.X, (int)screenPoint.Y);
    if (nextTargetWindow == UIntPtr.Zero) {
      // nop
      return;
    }

    if (this.currentTargetWindow == UIntPtr.Zero) {
      // 初回実行: DragHereボタンになっているはずなので無視
      this.currentTargetWindow = nextTargetWindow;
    }

    if (nextTargetWindow == this.currentTargetWindow) {
      // 対象が変わってないのでnop
      return;
    }

    // ウィンドウが異なる場合、枠線を描画していたウィンドウの枠線を消しておく
    this.ClearTargetRect();

    // 現在の対象ウィンドウを更新
    this.currentTargetWindow = nextTargetWindow;
    this.currentTargetDC = User32.GetDC(this.currentTargetWindow);

    // 取り込み対象範囲の描画
    this.XorTargetWindowRect();
  }

  /// DragHere: PreviewMouseUp
  /// @param sender 使用しない
  /// @param e Client座標系でのマウス座標(GetPosition(...))の取得が可能
  private void DragHere_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
    if (!this.dragHereMode) return;

    this.DragHere.Tag = null;

    // 後処理
    this.dragHereMode = false;
    this.ClearTargetRect();

    // マウス座標(Screen座標系)からWindowを取得
    var screenPoint = this.DragHere.PointToScreen(e.GetPosition(this.DragHere));
    UIntPtr nextTargetWindow = User32.WindowFromPoint((int)screenPoint.X, (int)screenPoint.Y);

    // Profileを更新
    this.ModifyProfile(WindowTypes.Normal, nextTargetWindow);
  }

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  //===================================================================
  // IUpdateByProfileの実装
  //===================================================================

  /// @copydoc IBindingProfile::CanChangeProfile
  public bool CanChangeProfile { get; private set; }
  /// @copydoc IBindingProfile::OnCurrentLayoutElementChanged
  public void OnCurrentLayoutElementChanged() {
    this.CanChangeProfile = false;
    if (!App.Profile.CurrentView.IsWindowValid) {
      TextBoxError.SetError(this.WindowCaption);
    } else {
      TextBoxError.ResetError(this.WindowCaption);
    }
    this.WindowCaption.Text = StringConverter.GetWindowCaption(App.Profile.CurrentView);
    this.CanChangeProfile = true;
  }
  /// @copydoc IBindingProfile::OnProfileChanged
  public void OnProfileChanged() {
    // 編集するのはCurrentのみ
    this.OnCurrentLayoutElementChanged();
  }

  //===================================================================
  // フィールド
  //===================================================================

  //-------------------------------------------------------------------
  // DragHere_PreviewMouseDown/Move/Up時の状態変数
  //-------------------------------------------------------------------

  /// DragHereボタンが押されたか
  private bool dragHereMode;
  /// 現在のマウスカーソル上のWindowハンドル
  private UIntPtr currentTargetWindow;
  /// 現在のマウスカーソル上のWindowハンドルのDC
  private IntPtr currentTargetDC;
}
}   // namespace SCFF.GUI.Controls
