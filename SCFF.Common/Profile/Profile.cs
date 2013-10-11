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
  DXGI,             ///< DXGI Desktop Duplication
  Desktop,          ///< ルートWindow
}

/// レイアウト設定などをまとめたプロファイル
public partial class Profile {
  //===================================================================
  // コンストラクタ
  //===================================================================
  
  /// コンストラクタ
  public Profile() {
    this.RestoreDefault();
  }

  /// デフォルトに戻す
  /// @post タイムスタンプ更新するがRaiseChangedは発生させない
  private void RestoreDefault() {
    this.LayoutElements = new List<LayoutElement>();
    this.Current = new LayoutElement(0);
    this.LayoutElements.Add(this.Current);

    // 変更イベントの発生はなし
    this.UpdateTimestamp();
  }

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
  public LayoutElement Current { get; set; }

  /// レイアウト要素編集開始
  public void Open() {
    // nop
  }

  /// レイアウト要素編集終了
  public void Close() {
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // カーソル(インデックス)
  //-------------------------------------------------------------------

  /// 現在選択中のレイアウト要素のインデックスを返す
  public int GetCurrentIndex() {
    return this.LayoutElements.IndexOf(this.Current);
  }

  /// 指定されたインデックスのレイアウト要素を選択する
  public void SetCurrentByIndex(int next) {
    /// @todo(me): 範囲チェック
    this.Current = this.LayoutElements[next];
  }

  //-------------------------------------------------------------------
  // レイアウト要素の追加
  //-------------------------------------------------------------------

  /// レイアウト要素を追加可能か
  public bool CanAdd {
    get { return this.LayoutElements.Count < Interprocess.MaxComplexLayoutElements; }
  }

  /// レイアウト要素を追加
  /// @post タイムスタンプ更新
  public void Add() {
    this.Current = new LayoutElement(this.LayoutElements.Count);
    this.LayoutElements.Add(this.Current);

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の削除
  //-------------------------------------------------------------------

  /// レイアウト要素を削除可能か
  public bool CanRemoveCurrent {
    get { return this.LayoutElements.Count > 1; }
  }

  /// 現在選択中のレイアウト要素を削除
  /// @post タイムスタンプ更新
  public void RemoveCurrent() {
    var removedIndex = this.GetCurrentIndex();
    this.LayoutElements.Remove(this.Current);

    // currentIndexを新しい場所に移して終了
    if (removedIndex < this.LayoutElements.Count) {
      this.Current = this.LayoutElements[removedIndex];
    } else {
      this.Current = this.LayoutElements[removedIndex - 1];
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
    foreach (var layoutElement in this.LayoutElements) {
      if (layoutElement.WindowType != WindowTypes.Normal) continue;
      Debug.Assert(layoutElement.IsWindowValid);
      layoutElement.UpdateBackupParameters();
    }
  }
  /// 全てのレイアウト要素のBackupParametersを復元する
  public void RestoreBackupParameters() {
    foreach (var layoutElement in this.LayoutElements) {
      if (layoutElement.WindowType == WindowTypes.Normal &&
          !layoutElement.IsWindowValid) {
        layoutElement.RestoreBackupParameters();
      }
    }
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
    result.LayoutElementCount = this.LayoutElements.Count;
    result.Timestamp = this.Timestamp;
    int index = 0;
    foreach (var layoutElement in this.LayoutElements) {
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

  /// レイアウトの種類
  public LayoutTypes LayoutType {
    get { return this.LayoutElements.Count == 1 ? LayoutTypes.NativeLayout : LayoutTypes.ComplexLayout; }
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
  public List<LayoutElement> LayoutElements { get; internal set; }
}
}   // namespace SCFF.Common.Profile
