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

/// @file SCFF.Common/Profile.cs
/// レイアウト設定などをまとめたプロファイル

/// SCFF.*で利用する共通クラスをまとめた名前空間
namespace SCFF.Common {

using System;
using System.Collections.Generic;

/// 追加レイアウトパラメータ
public class AdditionalLayoutParameter {
  public WindowTypes WindowType { get; set; }
  public bool Fit { get; set; }
  public double BoundRelativeLeft { get; set; }
  public double BoundRelativeTop { get; set; }
  public double BoundRelativeRight { get; set; }
  public double BoundRelativeBottom { get; set; }

  public int ClippingXWithoutFit { get; set; }
  public int ClippingYWithoutFit { get; set; }
  public int ClippingWidthWithoutFit { get; set; }
  public int ClippingHeightWithoutFit { get; set; }

  public int BackupDesktopClippingX { get; set; }
  public int BackupDesktopClippingY { get; set; }
  public int BackupDesktopClippingWidth { get; set; }
  public int BackupDesktopClippingHeight { get; set; }
}

/// レイアウト設定などをまとめたプロファイル
public partial class Profile {

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// コンストラクタ
  public Profile() {
    // 配列の初期化
    var length = Constants.MaxLayoutElementCount;
    this.message.LayoutParameters = new Interprocess.LayoutParameter[length];
    this.additionalLayoutParameters = new AdditionalLayoutParameter[length];
  }

  //===================================================================
  // メソッド
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
  private void ClearLayoutElement(OutputLayoutElement layout) {
    /// @todo(me) インスタンスを生成する形でゼロクリアしているが非効率的？
    var layoutParameter = new Interprocess.LayoutParameter();
    layoutParameter.SWScaleConfig = new Interprocess.SWScaleConfig();
    this.message.LayoutParameters[layout.Index] = layoutParameter;
    this.additionalLayoutParameters[layout.Index] = new AdditionalLayoutParameter();
  }

  /// レイアウトの初期値への設定
  /// @pre layoutはゼロクリア済み
  private void ResetLayoutElement(OutputLayoutElement layout) {
    this.ClearLayoutElement(layout);

    layout.KeepAspectRatio = true;
    layout.RotateDirection = RotateDirections.NoRotate;
    layout.Stretch = true;
    layout.SWScaleFlags = SWScaleFlags.Area;
    layout.SWScaleIsFilterEnabled = false;

    // プライマリモニタを表示
    layout.SetWindowToDesktop();
    //layout.SetWindowToDesktopListView();
    layout.ClippingXWithoutFit      = 0 - ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN);
    layout.ClippingYWithoutFit      = 0 - ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN);
    layout.ClippingWidthWithoutFit  = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXSCREEN);
    layout.ClippingHeightWithoutFit = ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CYSCREEN);
    /// @todo(me) クリッピング座標のバックアップをどこで取ればいいのか迷い中
    //layout.TryUpdateBackupDesktopClippingParameters();
    
    layout.Fit = false;
    layout.BoundRelativeRight = 1.0;
    layout.BoundRelativeBottom = 1.0;
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

  public bool CanAddLayoutElement() {
    var length = Constants.MaxLayoutElementCount;
    if (this.message.LayoutElementCount < length) {
      return true;
    } else {
      return false;
    }
  }

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

  public bool CanRemoveLayoutElement() {
    if (this.message.LayoutElementCount > 1) {
      return true;
    } else {
      return false;
    }
  }

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

  public void ChangeCurrentIndex(int index) {
    this.currentIndex = index;
  }

  //===================================================================
  // Update Message
  //===================================================================

  public void UpdateMessage(int sampleWidth, int sampleHeight) {}

  //===================================================================
  // アクセッサ
  //===================================================================

  public Int64 Timestamp {
    get { return this.message.Timestamp; }
  } 
  public void UpdateTimestamp() {
    this.message.Timestamp = DateTime.UtcNow.Ticks;
  }
  public LayoutTypes LayoutType {
    get { return (LayoutTypes)this.message.LayoutType; }
    set { this.message.LayoutType = Convert.ToInt32(value); }
  }
  public int LayoutElementCount {
    get { return this.message.LayoutElementCount; }
    set { this.message.LayoutElementCount = value; }
  }

  //===================================================================
  // イテレータ
  //===================================================================

  public InputLayoutElement CurrentInputLayoutElement {
    get { return new InputLayoutElement(this, this.currentIndex); }
  }

  public OutputLayoutElement CurrentOutputLayoutElement {
    get { return new OutputLayoutElement(this, this.currentIndex); }
  }

  public IEnumerator<InputLayoutElement> GetEnumerator() {
    for (int i = 0; i < this.message.LayoutElementCount; ++i) {
      yield return new InputLayoutElement(this, i);
    }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  // 現在編集中のインデックス
  private int currentIndex = 0;

  /// 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  /// (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  /// 追加レイアウトパラメータをまとめた配列
  /// messageLayoutParametersにあわせて非初期化済みにした
  private AdditionalLayoutParameter[] additionalLayoutParameters;
}
}
