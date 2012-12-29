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

/// @file SCFF.GUI/Controls/LayoutTab.cs
/// レイアウト要素の切り替えと個数の表示を行うタブ

namespace SCFF.GUI.Controls {

using System.Diagnostics;
using System.Windows.Controls;

/// レイアウト要素の切り替えと個数の表示を行うタブ
public partial class LayoutTab : UserControl, IProfileToControl {

  /// コンストラクタ
  public LayoutTab() {
    InitializeComponent();
  }

  /// TabItemを一気に生成する
  public void CreateTabs() {
    this.DetachChangedEventHandlers();

    // 必要な分を追加する
    this.LayoutElementTab.Items.Clear();
    for (int i = 0; i < App.Profile.LayoutElementCount; ++i) {
      var item = new TabItem();
      item.Header = (i+1).ToString();
      this.LayoutElementTab.Items.Add(item);
    }
    this.LayoutElementTab.SelectedIndex = App.Profile.CurrentInputLayoutElement.Index;
    
    this.AttachChangedEventHandlers();
  }

  /// TabItemをリセットする
  public void ResetTabs() {
    this.DetachChangedEventHandlers();

    // 最初の一個はそのまま残す
    this.LayoutElementTab.SelectedIndex = 0;
    for (int i = this.LayoutElementTab.Items.Count - 1; i >= 1; --i) {
      this.LayoutElementTab.Items.RemoveAt(i);
    }

    this.AttachChangedEventHandlers();
  }

  /// TabItemを末尾にひとつ追加する
  public void AddTab() {
    this.DetachChangedEventHandlers();

    var item = new TabItem();
    // 1-basedなのでCount+1
    item.Header = this.LayoutElementTab.Items.Count + 1;
    this.LayoutElementTab.Items.Add(item);
    // 最後に追加されたので末尾を選択
    this.LayoutElementTab.SelectedIndex = this.LayoutElementTab.Items.Count - 1;

    Debug.Assert(this.LayoutElementTab.SelectedIndex == App.Profile.CurrentInputLayoutElement.Index);
    Debug.WriteLine("*****Add!*****");
    Debug.WriteLine("Current Index: " + (App.Profile.CurrentInputLayoutElement.Index+1));

    this.AttachChangedEventHandlers();
  }

  /// 現在のタブをひとつ削除して後ろのタブの名前を変える
  public void RemoveCurrentTab() {
    this.DetachChangedEventHandlers();

    // 末尾を一個削除
    // 末尾削除なら改名すらいらない
    var last = this.LayoutElementTab.Items.Count - 1;
    this.LayoutElementTab.Items.RemoveAt(last);

    Debug.Assert(this.LayoutElementTab.SelectedIndex == App.Profile.CurrentInputLayoutElement.Index);
    Debug.WriteLine("=====Remove!=====");
    Debug.WriteLine("Current Index: " + (App.Profile.CurrentInputLayoutElement.Index+1));

    this.AttachChangedEventHandlers();
  }

  /// 現在のタブ選択を変更する(SelectionChangedと同じ動作)
  /// @todo(me) エンバグしそうな場所。要注意。
  public void ChangeCurrentTab() {
    Debug.Assert(0 <= App.Profile.CurrentInputLayoutElement.Index);
    Debug.Assert(App.Profile.CurrentInputLayoutElement.Index < this.LayoutElementTab.Items.Count);
    this.DetachChangedEventHandlers();
    this.LayoutElementTab.SelectedIndex = App.Profile.CurrentInputLayoutElement.Index;
    this.AttachChangedEventHandlers();
  }

  //===================================================================
  // IProfileToControlの実装
  //===================================================================

  /// @copydoc IProfileToControl.UpdateByProfile
  public void UpdateByProfile() {
    /// @todo(me) 本当はこのメソッドはつかわないはず
    /// this.CreateTabs();
  }

  /// @copydoc IProfileToControl.AttachChangedEventHandlers
  public void AttachChangedEventHandlers() {
    this.LayoutElementTab.SelectionChanged += layoutElementTab_SelectionChanged;
  }

  /// @copydoc IProfileToControl.DetachChangedEventHandlers
  public void DetachChangedEventHandlers() {
    this.LayoutElementTab.SelectionChanged -= layoutElementTab_SelectionChanged;
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
  // *Changed
  //-------------------------------------------------------------------

  private void layoutElementTab_SelectionChanged(object sender, SelectionChangedEventArgs e) {
    var original = App.Profile.CurrentInputLayoutElement.Index;
    var next = this.LayoutElementTab.SelectedIndex;
    App.Profile.ChangeCurrentIndex(next);

    Debug.WriteLine("-----Index Changed!-----");
    Debug.WriteLine("Current Index: " + (original+1) + "->" + (next+1));
    Debug.Assert(this.LayoutElementTab.SelectedIndex == App.Profile.CurrentInputLayoutElement.Index);

    // 他のコントロールのデータの更新はWindowに任せる
    Commands.ChangeCurrentLayoutElementCommand.Execute(null, null);
  }
}
}
