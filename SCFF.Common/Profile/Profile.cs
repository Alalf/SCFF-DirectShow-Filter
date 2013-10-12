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
    this.CopyLock = new object();
    this.RestoreDefault();
  }

  /// デフォルトに戻す
  /// @post タイムスタンプ更新するがRaiseChangedは発生させない
  public void RestoreDefault() {
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
  // 編集開始・終了
  //-------------------------------------------------------------------

  /// イベント: 変更時
  /// @warning Profile.CurrentIndexでは発生しない
  public event EventHandler OnChanged;

  /// イベントハンドラの実行
  public void RaiseChanged() {
    var handler = this.OnChanged;
    if (handler != null) handler(this, EventArgs.Empty);
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

  //-------------------------------------------------------------------
  // レイアウト要素の追加・削除
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

  /// レイアウト要素を削除可能か
  public bool CanRemoveCurrent {
    get { return this.LayoutElements.Count > 1; }
  }

  /// 現在選択中のレイアウト要素を削除
  /// @post タイムスタンプ更新
  public void RemoveCurrent() {
    lock (this.CopyLock) {
      var removedIndex = this.GetCurrentIndex();
      this.LayoutElements.Remove(this.Current);

      // Currentを新しい場所に移して終了
      if (removedIndex < this.LayoutElements.Count) {
        this.Current = this.LayoutElements[removedIndex];
      } else {
        this.Current = this.LayoutElements[removedIndex - 1];
      }
    }

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の重ね順の調整
  //-------------------------------------------------------------------

  /// 現在編集中のレイアウト要素を一つ前面に
  public void BringCurrentForward() {
    lock (this.CopyLock) {
      var removedIndex = this.LayoutElements.IndexOf(this.Current);
      this.LayoutElements.Remove(this.Current);
      this.LayoutElements.Insert(removedIndex + 1, this.Current);
    }

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  /// 現在編集中のレイアウト要素を一つ前面に移動できるか
  public bool CanBringCurrentForward {
    get {
      return this.LayoutElements.Count != 1 &&
          this.LayoutElements[this.LayoutElements.Count - 1] != this.Current;
    }
  }

  /// 現在編集中のレイアウト要素を一つ背面に
  public void SendCurrentBackward() {
    lock (this.CopyLock) {
      var removedIndex = this.LayoutElements.IndexOf(this.Current);
      this.LayoutElements.Remove(this.Current);
      this.LayoutElements.Insert(removedIndex - 1, this.Current);
    }

    // 変更イベントの発生
    this.UpdateTimestamp();
    this.RaiseChanged();
  }

  /// 現在編集中のレイアウト要素を一つ背面に移動できるか
  public bool CanSendCurrentBackward {
    get {
      return this.LayoutElements.Count != 1 &&
          this.LayoutElements[0] != this.Current;
    }
  }

  //-------------------------------------------------------------------
  // BackupParameters
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

  /// レイアウトパラメータをまとめたリスト
  public List<LayoutElement> LayoutElements { get; private set; }

  /// 現在選択中のレイアウト要素
  public LayoutElement Current { get; set; }

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

  /// 現在選択中のレイアウト要素のインデックスを返す
  public int GetCurrentIndex() {
    return this.LayoutElements.IndexOf(this.Current);
  }

  /// 指定されたインデックスのレイアウト要素を選択する
  /// @param next 選択したいレイアウト要素のインデックス
  public void SetCurrentByIndex(int next) {
    /// @todo(me): 範囲チェック
    this.Current = this.LayoutElements[next];
  }

  /// 全てのレイアウト要素を一気に更新する
  public void SetLayoutElements(List<LayoutElement> layoutElements, LayoutElement current) {
    this.LayoutElements = layoutElements;
    this.Current = current;
  }
  /// 全てのレイアウト要素をコピーする
  public List<LayoutElement> CopyLayoutElements() {
    lock (this.CopyLock) {
      return new List<LayoutElement>(this.LayoutElements);
    }
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// コピー・削除時に使うロック
  private object CopyLock { get; set; }
}
}   // namespace SCFF.Common.Profile
