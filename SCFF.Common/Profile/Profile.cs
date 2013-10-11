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

/// @file SCFF.Common/Profile/Profile.cs
/// @copydoc SCFF::Common::Profile::Profile

/// プロファイルに関連したクラスをまとめた名前空間
namespace SCFF.Common.Profile {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using SCFF.Interprocess;

/// Profile用Windowの種類
public enum WindowTypes {
  Normal,           ///< 標準のWindow
  DesktopListView,  ///< OS別デスクトップWindow
  Desktop,          ///< ルートWindow
}

/// レイアウト設定などをまとめたプロファイル
public partial class Profile {
  //===================================================================
  // 外部インタフェース
  //===================================================================

  //-------------------------------------------------------------------
  // イベント
  //-------------------------------------------------------------------

  /// イベント: 変更時
  /// @warning Profile.CurrentIndexでは発生しない
  public event EventHandler OnChanged;

  /// イベントハンドラの実行
  public void RaiseChanged() {
    var handler = this.OnChanged;
    if (handler != null) handler(this, EventArgs.Empty);
  }

  //-------------------------------------------------------------------
  // カーソル
  //-------------------------------------------------------------------

  /// 現在選択中のレイアウト要素
  public LayoutElement CurrentElement { get; set; }
  

  /// 現在選択中のレイアウト要素を参照モードで返す
  public ILayoutElementView CurrentView {
    get { return this.CurrentElement; }
  }

  /// 現在選択中のレイアウト要素を編集モードで返す
  public ILayoutElement Current {
    get { return this.CurrentElement; }
  }

  /// レイアウト要素編集開始
  public void Open() {
    // nop
  }

  /// レイアウト要素編集終了
  public void Close() {
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  /// foreach用Enumerator(参照モード)を返す
  public IEnumerator<ILayoutElementView> GetEnumerator() {
    foreach (var layoutElement in this.layoutElements) {
      yield return layoutElement;
    }
  }

  //-------------------------------------------------------------------
  // カーソル(インデックス)
  //-------------------------------------------------------------------

  /// 現在選択中のレイアウト要素のインデックスを返す
  public int GetCurrentIndex() {
    return this.layoutElements.IndexOf(this.CurrentElement);
  }

  /// 指定されたインデックスのレイアウト要素を選択する
  public void SetCurrentByIndex(int next) {
    /// @todo(me): 範囲チェック
    this.CurrentElement = this.layoutElements[next];
  }

  //-------------------------------------------------------------------
  // デフォルトに戻す
  //-------------------------------------------------------------------

  /// デフォルトに戻す
  /// @post タイムスタンプ更新するがRaiseChangedは発生させない
  public void RestoreDefault() {
    this.layoutElements.Clear();
    this.CurrentElement = new LayoutElement(0);
    this.layoutElements.Add(this.CurrentElement);

    // 変更イベントの発生はなし
    this.UpdateTimestamp();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の追加
  //-------------------------------------------------------------------

  /// レイアウト要素を追加可能か
  public bool CanAdd {
    get { return this.LayoutElementCount < Interprocess.MaxComplexLayoutElements; }
  }

  /// レイアウト要素を追加
  /// @post タイムスタンプ更新
  public void Add() {
    this.CurrentElement = new LayoutElement(this.LayoutElementCount);
    this.layoutElements.Add(this.CurrentElement);

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の削除
  //-------------------------------------------------------------------

  /// レイアウト要素を削除可能か
  public bool CanRemoveCurrent {
    get { return this.LayoutElementCount > 1; }
  }

  /// 現在選択中のレイアウト要素を削除
  /// @post タイムスタンプ更新
  public void RemoveCurrent() {
    var removedIndex = this.GetCurrentIndex();
    this.layoutElements.Remove(this.CurrentElement);

    // currentIndexを新しい場所に移して終了
    if (removedIndex < this.LayoutElementCount) {
      this.CurrentElement = this.layoutElements[removedIndex];
    } else {
      this.CurrentElement = this.layoutElements[removedIndex - 1];
    }

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // LayoutElementsの一括更新
  //-------------------------------------------------------------------

  /// 全てのレイアウト要素のBackupParametersを更新する
  public void UpdateBackupParameters() {
    foreach (var layoutElement in this.layoutElements) {
      if (layoutElement.WindowType != WindowTypes.Normal) continue;
      Debug.Assert(layoutElement.IsWindowValid);
      layoutElement.UpdateBackupParameters();
    }
  }
  /// 全てのレイアウト要素のBackupParametersを復元する
  public void RestoreBackupParameters() {
    foreach (var layoutElement in this.layoutElements) {
      if (layoutElement.WindowType == WindowTypes.Normal &&
          !layoutElement.IsWindowValid) {
        layoutElement.RestoreBackupParameters();
      }
    }
  }

  /// LayoutElementsを一気に更新する
  internal void SetLayoutElements(List<LayoutElement> layoutElements, LayoutElement current) {
    this.layoutElements = layoutElements;
    this.CurrentElement = current;
  }

  //-------------------------------------------------------------------
  // 変換
  //-------------------------------------------------------------------

  /// サンプルの幅と高さを指定してmessageを生成する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return 共有メモリにそのまま設定可能なMessage
  public Message ToMessage(int sampleWidth, int sampleHeight) {
    // インスタンス生成
    Message result;
    result.LayoutParameters = new LayoutParameter[Interprocess.MaxComplexLayoutElements];

    // 更新
    result.LayoutType = (int)this.LayoutType;
    result.LayoutElementCount = this.LayoutElementCount;
    result.Timestamp = this.Timestamp;
    int index = 0;
    foreach (var layoutElement in this.layoutElements) {
      // Bound*とClipping*以外のデータをコピー
      result.LayoutParameters[index] = layoutElement.ToLayoutParameter(sampleWidth, sampleHeight);
      ++index;
    }
    return result;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// タイムスタンプ
  public Int64 Timestamp { get; private set; }
  /// レイアウト要素数
  public int LayoutElementCount { 
    get { return this.layoutElements.Count; }
  }
  /// レイアウトの種類
  public LayoutTypes LayoutType {
    get { return this.LayoutElementCount == 1 ? LayoutTypes.NativeLayout : LayoutTypes.ComplexLayout; }
  }

  //===================================================================
  // アクセサ
  //===================================================================

  /// タイムスタンプを現在時刻で更新する
  public void UpdateTimestamp() {
    this.Timestamp = DateTime.Now.Ticks;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// レイアウトパラメータをまとめたリスト
  private List<LayoutElement> layoutElements = new List<LayoutElement>();
}
}   // namespace SCFF.Common.Profile
