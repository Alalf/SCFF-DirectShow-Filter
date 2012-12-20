// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// SCFF.*で利用する共通クラスをまとめた名前空間
namespace SCFF.Common {

using System;
using System.Collections.Generic;

/// Profileをまとめたクラス。ここからデータを切り出してDSFに送ったりする。
public partial class Profile {

  //===================================================================
  // 定数
  //===================================================================

  public enum WindowTypes {
    Normal,
    Desktop,
    Root,
  }

  /// @copydoc SCFF.Common.Interprocess.LayoutTypes
  public enum LayoutTypes {
    NullLayout    = Interprocess.LayoutTypes.NullLayout,
    NativeLayout  = Interprocess.LayoutTypes.NativeLayout,
    ComplexLayout = Interprocess.LayoutTypes.ComplexLayout
  }

  /// @copydoc SCFF.Common.Interprocess.SWScaleFlags
  public enum SWScaleFlags {
    FastBilinear  = Interprocess.SWScaleFlags.FastBilinear,
    Bilinear      = Interprocess.SWScaleFlags.Bilinear,
    Bicubic       = Interprocess.SWScaleFlags.Bicubic,
    X             = Interprocess.SWScaleFlags.X,
    Point         = Interprocess.SWScaleFlags.Point,
    Area          = Interprocess.SWScaleFlags.Area,
    Bicublin      = Interprocess.SWScaleFlags.Bicublin,
    Gauss         = Interprocess.SWScaleFlags.Gauss,
    Sinc          = Interprocess.SWScaleFlags.Sinc,
    Lanczos       = Interprocess.SWScaleFlags.Lanczos,
    Spline        = Interprocess.SWScaleFlags.Spline
  }

  /// @copydoc SCFF.Common.Interprocess.RotateDirections
  public enum RotateDirection {
    NoRotate      = Interprocess.RotateDirections.NoRotate,
    Degrees90     = Interprocess.RotateDirections.Degrees90,
    Degrees180    = Interprocess.RotateDirections.Degrees180,
    Degrees270    = Interprocess.RotateDirections.Degrees270,
  }

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  public Profile() {
    Reset();
  }

  //===================================================================
  // メソッド
  //===================================================================

  private Layout CreateDefaultLayout(int index) {
    /// @todo(me) ここからデフォルトウィンドウの指定などを行う・・・？やっぱりWin32いるじゃねーかwww
    var layout = new Layout(this, index);
    return layout;
  }

  public void Reset() {
    int length = Interprocess.Interprocess.MaxComplexLayoutElements;

    // this.message.LayoutParametersの生成
    this.message.LayoutParameters = new Interprocess.LayoutParameter[length];
    for (int i = 0; i < length; ++i) {
      this.message.LayoutParameters[i] = new Interprocess.LayoutParameter();
      // this.message.LayoutParameters[i].SWScaleConfigの生成
      this.message.LayoutParameters[i].SWScaleConfig = new Interprocess.SWScaleConfig();
    }

    // this.messageの初期化
    this.LayoutElementCount = 1;
    this.LayoutType = LayoutTypes.NativeLayout;
    this.UpdateTimestamp();

    // currentの生成(0は絶対にあることが保障されている)
    this.current = CreateDefaultLayout(0);
  }

  public void AddLayout() {}
  public void RemoveCurrentLayout() {}

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

  public Layout Current {
    get { return this.current; }
  }
  public IEnumerator<Layout> GetEnumerator() {
    for (int i = 0; i < this.message.LayoutElementCount; ++i) {
      yield return new Layout(this, i);
    }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  // 現在編集中のレイアウト
  private Layout current = null;

  // 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  // (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  // そのほかのデータをまとめたもの
  public class Appendix {
    public WindowTypes WindowType { get; set; }
    public bool Fit { get; set; }
    public double BoundRelativeLeft { get; set; }
    public double BoundRelativeTop { get; set; }
    public double BoundRelativeRight { get; set; }
    public double BoundRelativeBottom { get; set; }
    public int DesktopClippingX { get; set; }
    public int DesktopClippingY { get; set; }
    public int RootClippingX { get; set; }
    public int RootClippingY { get; set; }

    public Appendix() {
      this.WindowType = WindowTypes.Root;
      this.Fit = true;
      this.BoundRelativeLeft = 0.0;
      this.BoundRelativeTop = 0.0;
      this.BoundRelativeRight = 1.0;
      this.BoundRelativeBottom = 1.0;
      this.DesktopClippingX = -1;
      this.DesktopClippingY = -1;
      this.RootClippingX = -1;
      this.RootClippingY = -1;
    }
  }
  private Appendix[] appendices = new Appendix[Interprocess.Interprocess.MaxComplexLayoutElements] {
    new Appendix(), new Appendix(), new Appendix(), new Appendix(),
    new Appendix(), new Appendix(), new Appendix(), new Appendix(),
  };
}
}
