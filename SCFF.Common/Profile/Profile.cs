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

/// レイアウト設定などをまとめたプロファイル
public partial class Profile {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// カーソルの初期化
  private void BuildLayoutElements() {
    var length = Constants.MaxLayoutElementCount;
    this.layoutElements = new LayoutElement[length];
    for (int i = 0; i < length; ++i) {
      this.layoutElements[i] = new LayoutElement(this, i);
    }
  }

  /// コンストラクタ
  /// @warning コンストラクタでは実際の値の読み込みなどは行わない
  public Profile() {
    // 配列の初期化
    var length = Constants.MaxLayoutElementCount;
    this.message.LayoutParameters = new Interprocess.LayoutParameter[length];
    this.additionalLayoutParameters = new AdditionalLayoutParameter[length];
    
    // カーソルの初期化
    this.BuildLayoutElements();
  }

  /// サンプルの幅と高さを指定してmessageを生成する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @param forceNullLayout 強制的にスプラッシュを表示させるか
  /// @return 共有メモリにそのまま設定可能なMessage
  public Interprocess.Message ToMessage(int sampleWidth, int sampleHeight,
                                        bool forceNullLayout) {
    /// @todo(me) 実装する。ただしこれは仮想メモリ出力用イテレータの機能な気がする
    throw new NotImplementedException();
  }

  //===================================================================
  // 外部インタフェース
  //===================================================================

  /// メンバ配列を空にする
  /// @pre メンバ配列自体は生成済み(not null)
  private void ClearArrays() {
    /// 配列をクリア
    var length = Constants.MaxLayoutElementCount;
    Array.Clear(this.message.LayoutParameters, 0, length);
    Array.Clear(this.additionalLayoutParameters, 0, length);
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

  /// レイアウト要素を編集モードで返す
  internal ILayoutElement GetLayoutElement(int index) {
    return this.layoutElements[index];
  }

  //-------------------------------------------------------------------
  // デフォルトに戻す
  //-------------------------------------------------------------------

  /// デフォルトに戻す
  /// @post タイムスタンプ更新
  public void RestoreDefault() {
    this.ClearArrays();
    
    // Profileのプロパティの初期化
    this.LayoutElementCount = 1;
    // this.UpdateTimestamp();

    // currentの生成
    this.CurrentIndex = 0;
    this.Current.Open();
    this.Current.RestoreDefault();
    this.Current.Close();
  }

  //-------------------------------------------------------------------
  // レイアウト要素の追加
  //-------------------------------------------------------------------

  /// レイアウト要素を追加可能か
  public bool CanAdd {
    get { return this.message.LayoutElementCount < Constants.MaxLayoutElementCount; }
  }

  /// レイアウト要素を追加
  /// @post タイムスタンプ更新
  public void Add() {
    var nextIndex = this.LayoutElementCount;
    ++this.LayoutElementCount;
    // this.UpdateTimestamp();

    // currentを新たに生成したものに切り替える
    this.CurrentIndex = nextIndex;
    this.Current.Open();
    this.Current.RestoreDefault();
    this.Current.Close();
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
    var layoutParameterList = new List<Interprocess.LayoutParameter>();
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
    this.message.Timestamp = DateTime.UtcNow.Ticks;
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// カーソルのキャッシュ
  private LayoutElement[] layoutElements;

  /// レイアウトパラメータを格納したメッセージ
  ///
  /// 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  /// (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  /// 追加レイアウトパラメータをまとめた配列
  /// @attention messageLayoutParametersにあわせて非初期化済みにした
  private AdditionalLayoutParameter[] additionalLayoutParameters;
}
}   // namespace SCFF.Common.Profile
