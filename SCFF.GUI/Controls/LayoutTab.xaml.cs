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

/// @file SCFF.GUI/Controls/LayoutTab.xaml.cs
/// @copydoc SCFF::GUI::Controls::LayoutTab

namespace SCFF.GUI.Controls {

using System.Diagnostics;
using System.Windows.Controls;
using SCFF.Common.GUI;

/// レイアウト要素の切り替えと個数の表示を行うTabControl管理用UserControl
public partial class LayoutTab : UserControl, IBindingProfile {
  //===================================================================
  // コンストラクタ/Dispose/デストラクタ
  //===================================================================

  /// コンストラクタ
  public LayoutTab() {
    InitializeComponent();
  }

  //===================================================================
  // イベントハンドラ
  //===================================================================

  //-------------------------------------------------------------------
  // *Changed/Checked/Unchecked以外
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // Checked/Unchecked
  //-------------------------------------------------------------------

  //-------------------------------------------------------------------
  // *Changed/Collapsed/Expanded
  //-------------------------------------------------------------------

  /// LayoutElementTab: SelectionChanged
  /// @param sender 使用しない
  /// @param e 使用しない
  private void LayoutElementTab_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    if (!this.CanChangeProfile) return;
    var next = this.LayoutElementTab.SelectedIndex;
    App.Profile.CurrentIndex = next;

    // 他のコントロールのデータの更新はWindowに任せる
    UpdateCommands.UpdateMainWindowByEntireProfile.Execute(null, null);
  }

  //===================================================================
  // IBindingProfileの実装
  //===================================================================

  /// @copydoc Common::GUI::IBindingProfile::OnCurrentProfileChanged
  public bool CanChangeProfile { get; private set; }
  /// @copydoc Common::GUI::IBindingProfile::OnCurrentProfileChanged
  public void OnCurrentProfileChanged() {
    // CurrentProfileの値が変わってもTabの内容は変わらない
  }
  /// Profileを見ながら必要な分を足し、必要な分を削る
  private void UpdateLayoutElementTab() {
    // コントロール編集開始
    this.CanChangeProfile = false;

    var tabIndex = this.LayoutElementTab.SelectedIndex;
    var tabCount = this.LayoutElementTab.Items.Count;
    var profileIndex = App.Profile.CurrentIndex;
    var profileCount = App.Profile.LayoutElementCount;
    
    // まず数を合わせる
    if (tabCount == profileCount) {
      // 特に増減なし
    } else if (tabCount < profileCount) {
      // profileCountのほうが多い
      var delta = profileCount - tabCount;
      // 足りない分を足す
      for (int i = 0; i < delta; i++) {
        var item = new TabItem();
        // 1-basedなのでCount+1
        item.Header = this.LayoutElementTab.Items.Count + 1;
        this.LayoutElementTab.Items.Add(item);
      }
    } else {
      // tabCountのほうが少ない
      var delta = tabCount - profileCount;
      // 多すぎる文は削る
      for (int i = 0; i < delta; i++) {
        // 末尾から差の文だけを削除
        this.LayoutElementTab.Items.RemoveAt(tabCount - 1 - i);
      }
    }

    // 次に選択しているところを同期させる
    this.LayoutElementTab.SelectedIndex = App.Profile.CurrentIndex;

    // コントロール編集終了
    this.CanChangeProfile = true;

    Debug.Assert(App.Profile.LayoutElementCount == this.LayoutElementTab.Items.Count);
    Debug.Assert(App.Profile.CurrentIndex == this.LayoutElementTab.SelectedIndex);
  }
  /// @copydoc Common::GUI::IBindingProfile::OnEntireProfileChanged
  public void OnEntireProfileChanged() {
    this.UpdateLayoutElementTab();
  }
}
}   // namespace SCFF.GUI.Controls
