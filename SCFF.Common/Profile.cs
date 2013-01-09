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

/// @file SCFF.Common/Profile.cs
/// @copydoc SCFF::Common::Profile

/// SCFF共有クラスライブラリ
namespace SCFF.Common {

using System;
using System.Collections.Generic;
using SCFF.Common.Ext;

/// レイアウト設定などをまとめたプロファイル
public partial class Profile {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public Profile() {
    // 配列の初期化
    var length = Constants.MaxLayoutElementCount;
    this.message.LayoutParameters = new Interprocess.LayoutParameter[length];
    this.additionalLayoutParameters = new AdditionalLayoutParameter[length];

    this.layoutElements = new LayoutElement[Constants.MaxLayoutElementCount];
  }

  //===================================================================
  // Profile全体のリセット
  //===================================================================

  /// 配列をクリア
  /// @pre 配列自体は生成済み
  private void ClearLayoutParameters() {
    var length = Constants.MaxLayoutElementCount;
    Array.Clear(this.message.LayoutParameters, 0, length);
    Array.Clear(this.additionalLayoutParameters, 0, length);
  }

  /// Profile全体のリセット
  public void ResetProfile() {
    // 配列の初期化をして中身をクリア
    this.ClearLayoutParameters();
    
    // Profileのプロパティの初期化
    this.LayoutElementCount = 1;
    this.LayoutType = LayoutTypes.NativeLayout;
    // this.UpdateTimestamp();

    // currentの生成
    this.currentIndex = 0;
    this.Current.Open();
    this.Current.RestoreDefault();
    this.Current.Close();
  }

  //===================================================================
  // LayoutElementの追加と削除
  //===================================================================

  /// レイアウト要素が追加可能か
  public bool CanAddLayoutElement() {
    var length = Constants.MaxLayoutElementCount;
    if (this.message.LayoutElementCount < length) {
      return true;
    } else {
      return false;
    }
  }

  /// レイアウト要素を追加
  public void AddLayoutElement() {
    var nextIndex = this.LayoutElementCount;
    ++this.LayoutElementCount;
    this.LayoutType = LayoutTypes.ComplexLayout;
    // this.UpdateTimestamp();

    // currentを新たに生成したものに切り替える
    this.currentIndex = nextIndex;
    this.Current.Open();
    this.Current.RestoreDefault();
    this.Current.Close();
  }

  /// レイアウト要素を削除可能か
  public bool CanRemoveLayoutElement() {
    if (this.message.LayoutElementCount > 1) {
      return true;
    } else {
      return false;
    }
  }

  /// 現在選択中のレイアウト要素を削除
  public void RemoveCurrentLayoutElement() {
    // ややこしいので良く考えて書くこと！
    // とりあえず一番簡単なのは全部コピーして全部に書き戻すことだろう
    // また、全体的にスレッドセーフではないとおもうので何とかしたいところ
    var layoutParameterList = new List<Interprocess.LayoutParameter>();
    var additionalLayoutPararameterList = new List<AdditionalLayoutParameter>();

    var removedIndex = this.currentIndex;
    for (int i = 0; i < this.LayoutElementCount; ++i) {
      if (i != removedIndex) {
        layoutParameterList.Add(this.message.LayoutParameters[i]);
        additionalLayoutPararameterList.Add(this.additionalLayoutParameters[i]);
      }
    }

    // 後は配列を消去した上で書き戻す
    this.ClearLayoutParameters();
    layoutParameterList.CopyTo(this.message.LayoutParameters);
    additionalLayoutPararameterList.CopyTo(this.additionalLayoutParameters);

    // Profileメンバの更新
    --this.LayoutElementCount;
    if (this.LayoutElementCount > 1) {
      this.LayoutType = LayoutTypes.ComplexLayout;
    } else {
      this.LayoutType = LayoutTypes.NativeLayout;
    }
    this.UpdateTimestamp();

    // currentIndexを新しい場所に移して終了
    if (removedIndex < this.LayoutElementCount) {
      // なにもしない
    } else {
      this.currentIndex = removedIndex - 1;
    }
  }

  //===================================================================
  // 現在選択中のIndexを変更する
  //===================================================================

  /// 現在選択中のIndexを変更する
  public void ChangeCurrentIndex(int index) {
    this.currentIndex = index;
  }

  //===================================================================
  // Messageを更新
  //===================================================================

  /// サンプルの幅と高さを指定してthis.messageを更新する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  public void UpdateMessage(int sampleWidth, int sampleHeight) {
  /// @todo(me) 実装する。ただしこれは仮想メモリ出力用イテレータの機能な気がする
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// タイムスタンプ
  public Int64 Timestamp {
    get { return this.message.Timestamp; }
  } 
  /// レイアウトの種類
  public LayoutTypes LayoutType {
    get { return (LayoutTypes)this.message.LayoutType; }
    set { this.message.LayoutType = Convert.ToInt32(value); }
  }
  /// レイアウト要素数
  public int LayoutElementCount {
    get { return this.message.LayoutElementCount; }
    set { this.message.LayoutElementCount = value; }
  }

  //===================================================================
  // アクセサ
  //===================================================================

  /// タイムスタンプを現在時刻で更新する
  public void UpdateTimestamp() {
    this.message.Timestamp = DateTime.UtcNow.Ticks;
  }

  //===================================================================
  // イテレータ
  //===================================================================

  /// 現在選択中のレイアウト要素を参照モードで返す
  public ILayoutElementView CurrentView {
    get {  
      if (this.layoutElements[this.currentIndex] == null) {
        this.layoutElements[this.currentIndex] =
            new LayoutElement(this, this.currentIndex);
      }
      return this.layoutElements[this.currentIndex];
    }
  }

  /// 現在選択中のレイアウト要素を編集モードで返す
  public ILayoutElement Current {
    get {  
      if (this.layoutElements[this.currentIndex] == null) {
        this.layoutElements[this.currentIndex] =
            new LayoutElement(this, this.currentIndex);
      }
      return this.layoutElements[this.currentIndex];
    }
  }

  /// foreach用Enumeratorを返す
  public IEnumerator<ILayoutElementView> GetEnumerator() {
    for (int i = 0; i < this.message.LayoutElementCount; ++i) {
      if (this.layoutElements[i] == null) {
        this.layoutElements[i] = new LayoutElement(this, i);
      }
      yield return this.layoutElements[i];
    }
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// 現在選択中のIndex
  private int currentIndex = 0;

  /// イテレータのキャッシュ
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
}   // namespace SCFF.Common
