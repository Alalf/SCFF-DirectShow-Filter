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
/// レイアウト設定などをまとめたプロファイル

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

    this.inputLayoutElements = new InputLayoutElement[Constants.MaxLayoutElementCount];
    this.outputLayoutElements = new OutputLayoutElement[Constants.MaxLayoutElementCount];
  }

  //===================================================================
  // OutputLayoutElementの操作
  //===================================================================

  /// レイアウトのインスタンスを生成
  private OutputLayoutElement CreateLayoutElement(int index) {
    var layoutParameter = new Interprocess.LayoutParameter();
    layoutParameter.SWScaleConfig = new Interprocess.SWScaleConfig();
    this.message.LayoutParameters[index] = layoutParameter;
    this.additionalLayoutParameters[index] = new AdditionalLayoutParameter();

    return new OutputLayoutElement(this, index);
  }

  /// レイアウトのゼロクリア
  /// @param[in, out] layoutElement ゼロクリアされるレイアウト要素
  private void ClearLayoutElement(OutputLayoutElement layoutElement) {
    /// @todo(me) インスタンスを生成する形でゼロクリアしているが非効率的？
    var layoutParameter = new Interprocess.LayoutParameter();
    layoutParameter.SWScaleConfig = new Interprocess.SWScaleConfig();
    this.message.LayoutParameters[layoutElement.Index] = layoutParameter;
    this.additionalLayoutParameters[layoutElement.Index] = new AdditionalLayoutParameter();
  }

  /// レイアウトの初期値への設定
  /// @pre layoutはゼロクリア済み
  /// @param[in, out] layoutElement 初期値設定されるレイアウト要素
  private void ResetLayoutElement(OutputLayoutElement layoutElement) {
    this.ClearLayoutElement(layoutElement);

    layoutElement.KeepAspectRatio = true;
    layoutElement.RotateDirection = RotateDirections.NoRotate;
    layoutElement.Stretch = true;
    layoutElement.SWScaleFlags = SWScaleFlags.Area;
    layoutElement.SWScaleIsFilterEnabled = false;

    // プライマリモニタを表示
    layoutElement.SetWindowToDesktop();
    //layoutElement.SetWindowToDesktopListView();
    layoutElement.ClippingXWithoutFit      = 0 - User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN);
    layoutElement.ClippingYWithoutFit      = 0 - User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN);
    layoutElement.ClippingWidthWithoutFit  = User32.GetSystemMetrics(User32.SM_CXSCREEN);
    layoutElement.ClippingHeightWithoutFit = User32.GetSystemMetrics(User32.SM_CYSCREEN);
    /// @todo(me) クリッピング座標のバックアップをどこで取ればいいのか迷い中
    //layoutElement.TryUpdateBackupDesktopClippingParameters();
    
    layoutElement.Fit = false;
    layoutElement.BoundRelativeRight = 1.0;
    layoutElement.BoundRelativeBottom = 1.0;
  }

  //===================================================================
  // ResetProfile
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
    this.UpdateTimestamp();

    // currentの生成
    this.currentIndex = 0;
    var layoutElement = CreateLayoutElement(this.currentIndex);
    this.ResetLayoutElement(layoutElement);
  }

  //===================================================================
  // Add/Remove LayoutElement
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
    this.UpdateTimestamp();

    // currentを新たに生成したものに切り替える
    this.currentIndex = nextIndex;
    var layoutElement = CreateLayoutElement(this.currentIndex);
    this.ResetLayoutElement(layoutElement);
    /// @todo(me) レイアウト配置時に毎回サイズと場所がかぶっているのはわかりづらいのでずらしたい
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
  // Change Current
  //===================================================================

  /// 現在選択中のIndexを変更する
  public void ChangeCurrentIndex(int index) {
    this.currentIndex = index;
  }

  //===================================================================
  // Update Message
  //===================================================================

  /// サンプルの幅と高さを指定してthis.messageを更新する
  public void UpdateMessage(int sampleWidth, int sampleHeight) {}

  //===================================================================
  // アクセッサ
  //===================================================================

  /// タイムスタンプ
  public Int64 Timestamp {
    get { return this.message.Timestamp; }
  } 
  /// タイムスタンプを現在自国で更新する
  public void UpdateTimestamp() {
    this.message.Timestamp = DateTime.UtcNow.Ticks;
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
  // イテレータ
  //===================================================================

  /// 現在選択中のレイアウト要素を読み込み可能な状態で返す
  public InputLayoutElement CurrentInputLayoutElement {
    get {  
      if (this.inputLayoutElements[this.currentIndex] == null) {
        this.inputLayoutElements[this.currentIndex] =
            new InputLayoutElement(this, this.currentIndex);
      }
      return this.inputLayoutElements[this.currentIndex];
    }
  }

  /// 現在選択中のレイアウト要素を書き込み可能な状態で返す
  public OutputLayoutElement CurrentOutputLayoutElement {
    get {  
      if (this.outputLayoutElements[this.currentIndex] == null) {
        this.outputLayoutElements[this.currentIndex] =
            new OutputLayoutElement(this, this.currentIndex);
      }
      return this.outputLayoutElements[this.currentIndex];
    }
  }

  /// foreach用Enumeratorを返す
  public IEnumerator<InputLayoutElement> GetEnumerator() {
    for (int i = 0; i < this.message.LayoutElementCount; ++i) {
      if (this.inputLayoutElements[i] == null) {
        this.inputLayoutElements[i] = new InputLayoutElement(this, i);
      }
      yield return this.inputLayoutElements[i];
    }
  }

  //===================================================================
  // フィールド
  //===================================================================

  /// 現在選択中のIndex
  private int currentIndex = 0;

  /// 参照用イテレータのキャッシュ
  private InputLayoutElement[] inputLayoutElements;

  /// 操作用イテレータのキャッシュ
  private OutputLayoutElement[] outputLayoutElements;

  /// レイアウトパラメータを格納したメッセージ
  ///
  /// 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  /// (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  /// 追加レイアウトパラメータをまとめた配列
  /// @attention messageLayoutParametersにあわせて非初期化済みにした
  private AdditionalLayoutParameter[] additionalLayoutParameters;
}
}
