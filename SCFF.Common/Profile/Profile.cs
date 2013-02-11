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
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  /// @warning コンストラクタでは実際の値の読み込みなどは行わない
  public Profile() {
    const int length = Interprocess.MaxComplexLayoutElements;

    // LayoutParametersの初期化
    this.message.LayoutParameters = new LayoutParameter[length];
    
    // カーソルの初期化
    for (int i = 0; i < length; ++i) {
      this.layoutElements[i] = new LayoutElement(this, i);
    }
  }

  //===================================================================
  // LayoutElementsの一括更新
  //===================================================================

  /// 全てのレイアウト要素のBackupParametersを更新する
  public void UpdateBackupParameters() {
    for (int i = 0; i < this.LayoutElementCount; ++i) {
      var layoutElement = this.layoutElements[i];
      if (layoutElement.WindowType != WindowTypes.Normal) continue;
      Debug.Assert(layoutElement.IsWindowValid);
      layoutElement.UpdateBackupParameters();
    }
  }
  /// 全てのレイアウト要素のBackupParametersを復元する
  public void RestoreBackupParameters() {
    for (int i = 0; i < this.LayoutElementCount; ++i) {
      var layoutElement = this.layoutElements[i];
      if (layoutElement.WindowType == WindowTypes.Normal &&
          !layoutElement.IsWindowValid) {
        layoutElement.RestoreBackupParameters();
      }
    }
  }

  //===================================================================
  // Validate/SendMessage
  //===================================================================

  /// サンプルの幅と高さを指定してmessageを生成する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return 共有メモリにそのまま設定可能なMessage
  public Message ToMessage(int sampleWidth, int sampleHeight) {
    // this.messageで最新の情報になっていない部分を編集する
    this.message.LayoutType = (int)this.LayoutType;
    for (int i = 0; i < this.LayoutElementCount; ++i) {
      var boundRect = this.layoutElements[i].GetBoundRect(sampleWidth, sampleHeight);
      this.message.LayoutParameters[i].BoundX = boundRect.X;
      this.message.LayoutParameters[i].BoundY = boundRect.Y;
      this.message.LayoutParameters[i].BoundWidth = boundRect.Width;
      this.message.LayoutParameters[i].BoundHeight = boundRect.Height;
      this.message.LayoutParameters[i].ClippingX = this.layoutElements[i].ClippingXWithFit;
      this.message.LayoutParameters[i].ClippingY = this.layoutElements[i].ClippingYWithFit;
      this.message.LayoutParameters[i].ClippingWidth = this.layoutElements[i].ClippingWidthWithFit;
      this.message.LayoutParameters[i].ClippingHeight = this.layoutElements[i].ClippingHeightWithFit;
    }
    return this.message;
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
  public int CurrentIndex { get; set; }

  /// 現在選択中のレイアウト要素を参照モードで返す
  public ILayoutElementView CurrentView {
    get { return this.layoutElements[this.CurrentIndex]; }
  }

  /// 現在選択中のレイアウト要素を編集モードで返す
  public ILayoutElement Current {
    get { return this.layoutElements[this.CurrentIndex]; }
  }

  /// foreach用Enumerator(参照モード)を返す
  public IEnumerator<ILayoutElementView> GetEnumerator() {
    for (int i = 0; i < this.message.LayoutElementCount; ++i) {
      yield return this.layoutElements[i];
    }
  }

  /// レイアウト要素を参照・編集モードで返す
  internal LayoutElement GetLayoutElement(int index) {
    return this.layoutElements[index];
  }

  //-------------------------------------------------------------------
  // デフォルトに戻す
  //-------------------------------------------------------------------

  /// メンバ配列を空にする
  /// @pre メンバ配列自体は生成済み(not null)
  private void ClearArrays() {
    /// 配列をクリア
    const int length = Interprocess.MaxComplexLayoutElements;
    Array.Clear(this.message.LayoutParameters, 0, length);
    Array.Clear(this.additionalLayoutParameters, 0, length);
  }

  /// デフォルトに戻す
  /// @post タイムスタンプ更新するがRaiseChangedは発生させない
  public void RestoreDefault() {
    this.ClearArrays();
    
    // Profileのプロパティの初期化
    this.LayoutElementCount = 1;
    this.UpdateTimestamp();

    // currentの生成
    this.CurrentIndex = 0;
    this.Current.RestoreDefault();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の追加
  //-------------------------------------------------------------------

  /// レイアウト要素を追加可能か
  public bool CanAdd {
    get { return this.message.LayoutElementCount < Interprocess.MaxComplexLayoutElements; }
  }

  /// レイアウト要素を追加
  /// @post タイムスタンプ更新
  public void Add() {
    var nextIndex = this.LayoutElementCount;
    ++this.LayoutElementCount;
    this.UpdateTimestamp();

    // currentを新たに生成したものに切り替える
    this.CurrentIndex = nextIndex;
    this.Current.RestoreDefault();

    this.RaiseChanged();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の削除
  //-------------------------------------------------------------------

  /// レイアウト要素を削除可能か
  public bool CanRemoveCurrent {
    get { return this.message.LayoutElementCount > 1; }
  }

  /// 現在選択中のレイアウト要素を削除
  /// @post タイムスタンプ更新
  public void RemoveCurrent() {
    // ややこしいので良く考えて書くこと！
    // とりあえず一番簡単なのは全部コピーして全部に書き戻すことだろう
    // また、全体的にスレッドセーフではないとおもうので何とかしたいところ
    var layoutParameterList = new List<LayoutParameter>();
    var additionalLayoutPararameterList = new List<AdditionalLayoutParameter>();

    var removedIndex = this.CurrentIndex;
    for (int i = 0; i < this.LayoutElementCount; ++i) {
      if (i != removedIndex) {
        layoutParameterList.Add(this.message.LayoutParameters[i]);
        additionalLayoutPararameterList.Add(this.additionalLayoutParameters[i]);
      }
    }

    // 後は配列を消去した上で書き戻す
    this.ClearArrays();
    layoutParameterList.CopyTo(this.message.LayoutParameters);
    additionalLayoutPararameterList.CopyTo(this.additionalLayoutParameters);

    // Profileメンバの更新
    --this.LayoutElementCount;
    this.UpdateTimestamp();

    // currentIndexを新しい場所に移して終了
    if (removedIndex < this.LayoutElementCount) {
      // なにもしない
    } else {
      this.CurrentIndex = removedIndex - 1;
    }

    // 変更イベントの発生
    this.RaiseChanged();
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// タイムスタンプ
  public Int64 Timestamp {
    get { return this.message.Timestamp; }
  } 
  /// レイアウト要素数
  public int LayoutElementCount {
    get { return this.message.LayoutElementCount; }
    set { this.message.LayoutElementCount = value; }
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
    this.message.Timestamp = DateTime.Now.Ticks;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// カーソルのキャッシュ
  private readonly LayoutElement[] layoutElements =
      new LayoutElement[Interprocess.MaxComplexLayoutElements];

  /// レイアウトパラメータを格納したメッセージ
  ///
  /// 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  /// (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Message message = new Message();

  /// 追加レイアウトパラメータをまとめた配列
  /// @attention messageLayoutParametersにあわせて非初期化済みにした
  private readonly AdditionalLayoutParameter[] additionalLayoutParameters =
      new AdditionalLayoutParameter[Interprocess.MaxComplexLayoutElements];
}
}   // namespace SCFF.Common.Profile
