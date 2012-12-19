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
using System.Diagnostics;

/// Profileをまとめたクラス。ここからデータを切り出してDSFに送ったりする。
public class Profile {

  //===================================================================
  // 定数
  //===================================================================

  public enum WindowType {
    Normal,
    Desktop,
    Root,
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

  /// @copydoc SCFF.Common.Interprocess.RotateDirection
  public enum RotateDirection {
    NoRotate      = Interprocess.RotateDirection.NoRotate,
    Degrees90     = Interprocess.RotateDirection.Degrees90,
    Degrees180    = Interprocess.RotateDirection.Degrees180,
    Degrees270    = Interprocess.RotateDirection.Degrees270,
  }

  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  public Profile() {
    int length = Interprocess.Interprocess.MaxComplexLayoutElements;

    // this.message.LayoutParametersの生成
    this.message.LayoutParameters = new Interprocess.LayoutParameter[length];
    for (int i = 0; i < length; ++i) {
      this.message.LayoutParameters[i] = new Interprocess.LayoutParameter();
      // this.message.LayoutParameters[i].SWScaleConfigの生成
      this.message.LayoutParameters[i].SWScaleConfig = new Interprocess.SWScaleConfig();
    }
  }

  //===================================================================
  // アクセッサ
  // 基本的には現在編集対象のデータ以外の読み書きはできない
  // ただしTabやLayoutエディタでは必要になるのでgetterだけは用意すること
  //===================================================================

  // プログラムから直接は利用してはいけないもの(this.appendicesの内容で上書きされるため)
  // this.message.layoutParameters[*].Bound*


  //-------------------------------------------------------------------
  // Window*
  //-------------------------------------------------------------------

  public UIntPtr CurrentWindow {
    get { return (UIntPtr)this.message.LayoutParameters[this.currentIndex].Window; }
    set { this.message.LayoutParameters[this.currentIndex].Window = value.ToUInt64(); }
  }
  public UIntPtr GetWindow(int index) {
    return (UIntPtr)this.message.LayoutParameters[index].Window;
  }
  internal void SetWindow(int index, UIntPtr value) {
    this.message.LayoutParameters[index].Window = value.ToUInt64();
  }

  //-------------------------------------------------------------------
  // Clipping*
  //-------------------------------------------------------------------

  public int CurrentClippingX {
    get { return this.message.LayoutParameters[this.currentIndex].ClippingX; }
    set { this.message.LayoutParameters[this.currentIndex].ClippingX = value; }
  }
  public int GetClippingX(int index) {
    return this.message.LayoutParameters[index].ClippingX;
  }
  internal void SetClippingX(int index, int value) {
    this.message.LayoutParameters[index].ClippingX = value;
  }

  public int CurrentClippingY {
    get { return this.message.LayoutParameters[this.currentIndex].ClippingY; }
    set { this.message.LayoutParameters[this.currentIndex].ClippingY = value; }
  }
  public int GetClippingY(int index) {
    return this.message.LayoutParameters[index].ClippingY;
  }
  internal void SetClippingY(int index, int value) {
    this.message.LayoutParameters[index].ClippingY = value;
  }

  public int CurrentClippingWidth {
    get { return this.message.LayoutParameters[this.currentIndex].ClippingWidth; }
    set { this.message.LayoutParameters[this.currentIndex].ClippingWidth = value; }
  }
  public int GetClippingWidth(int index) {
    return this.message.LayoutParameters[index].ClippingWidth;
  }
  internal void SetClippingWidth(int index, int value) {
    this.message.LayoutParameters[index].ClippingWidth = value;
  }

  public int CurrentClippingHeight {
    get { return this.message.LayoutParameters[this.currentIndex].ClippingHeight; }
    set { this.message.LayoutParameters[this.currentIndex].ClippingHeight = value; }
  }
  public int GetClippingHeight(int index) {
    return this.message.LayoutParameters[this.currentIndex].ClippingHeight;
  }
  internal void SetClippingHeight(int index, int value) {
    this.message.LayoutParameters[index].ClippingHeight = value;
  }

  //-------------------------------------------------------------------

  public bool CurrentShowCursor {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].ShowCursor); }
    set { this.message.LayoutParameters[this.currentIndex].ShowCursor = Convert.ToByte(value); }
  }
  public bool GetShowCursor(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].ShowCursor);
  }
  internal void SetShowCursor(int index, bool value) {
    this.message.LayoutParameters[index].ShowCursor = Convert.ToByte(value);
  }

  public bool CurrentShowLayeredWindow {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].ShowLayeredWindow); }
    set { this.message.LayoutParameters[this.currentIndex].ShowLayeredWindow = Convert.ToByte(value); }
  }
  public bool GetShowLayeredWindow(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].ShowLayeredWindow);
  }
  internal void SetShowLayeredWindow(int index, bool value) {
    this.message.LayoutParameters[index].ShowLayeredWindow = Convert.ToByte(value);
  } 

  //-------------------------------------------------------------------
  // SWScaleConfig
  //-------------------------------------------------------------------

  public SWScaleFlags CurrentSWScaleFlags {
    get { return (SWScaleFlags)this.message.LayoutParameters[this.currentIndex].SWScaleConfig.Flags; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.Flags = Convert.ToInt32(value); }
  }
  public SWScaleFlags GetSWScaleFlags(int index) {
    return (SWScaleFlags)this.message.LayoutParameters[index].SWScaleConfig.Flags;
  }
  internal void SetSWScaleFlags(int index, SWScaleFlags value) {
    this.message.LayoutParameters[index].SWScaleConfig.Flags = Convert.ToInt32(value);
  }

  public bool CurrentSWScaleAccurateRnd {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].SWScaleConfig.AccurateRnd); }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
  }
  public bool GetSWScaleAccurateRnd(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].SWScaleConfig.AccurateRnd);
  }
  internal void SetSWScaleAccurateRnd(int index, bool value) {
    this.message.LayoutParameters[index].SWScaleConfig.AccurateRnd = Convert.ToByte(value);
  }

  public bool CurrentSWScaleIsFilterEnabled {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].SWScaleConfig.IsFilterEnabled); }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
  }
  public bool GetSWScaleIsFilterEnabled(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].SWScaleConfig.IsFilterEnabled);
  }
  internal void SetSWScaleIsFilterEnabled(int index, bool value) {
    this.message.LayoutParameters[index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value);
  }

  public float CurrentSWScaleLumaGBlur {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.LumaGblur; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.LumaGblur = value; }
  }
  public float GetSWScaleLumaGBlur(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.LumaGblur;
  }
  internal void SetSWScaleLumaGBlur(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.LumaGblur = value;
  }

  public float CurrentSWScaleChromaGBlur {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaGblur; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaGblur = value; }
  }
  public float GetSWScaleChromaGBlur(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.ChromaGblur;
  }
  internal void SetSWScaleChromaGBlur(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.ChromaGblur = value;
  }

  public float CurrentSWScaleLumaSharpen {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.LumaSharpen; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.LumaSharpen = value; }
  }
  public float GetSWScaleLumaSharpen(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.LumaSharpen;
  }
  internal void SetSWScaleLumaSharpen(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.LumaSharpen = value;
  }

  public float CurrentSWScaleChromaSharpen {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaSharpen; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaSharpen = value; }
  }
  public float GetSWScaleChromaSharpen(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.ChromaSharpen;
  }
  internal void SetSWScaleChromaSharpen(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.ChromaSharpen = value;
  }

  public float CurrentSWScaleChromaHshift {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaHshift; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaHshift = value; }
  }
  public float GetSWScaleChromaHshift(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.ChromaHshift;
  }
  internal void SetSWScaleChromaHshift(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.ChromaHshift = value;
  }

  public float CurrentSWScaleChromaVshift {
    get { return this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaVshift; }
    set { this.message.LayoutParameters[this.currentIndex].SWScaleConfig.ChromaVshift = value; }
  }
  public float GetSWScaleChromaVshift(int index) {
    return this.message.LayoutParameters[index].SWScaleConfig.ChromaVshift;
  }
  internal void SetSWScaleChromaVshift(int index, float value) {
    this.message.LayoutParameters[index].SWScaleConfig.ChromaVshift = value;
  }

  //-------------------------------------------------------------------

  public bool CurrentStretch {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].Stretch); }
    set { this.message.LayoutParameters[this.currentIndex].Stretch = Convert.ToByte(value); }
  }
  public bool GetStretch(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].Stretch);
  }
  internal void SetStretch(int index, bool value) {
    this.message.LayoutParameters[index].Stretch = Convert.ToByte(value);
  }

  public bool CurrentKeepAspectRatio {
    get { return Convert.ToBoolean(this.message.LayoutParameters[this.currentIndex].KeepAspectRatio); }
    set { this.message.LayoutParameters[this.currentIndex].KeepAspectRatio = Convert.ToByte(value); }
  }
  public bool GetKeepAspectRatio(int index) {
    return Convert.ToBoolean(this.message.LayoutParameters[index].KeepAspectRatio);
  }
  internal void SetKeepAspectRatio(int index, bool value) {
    this.message.LayoutParameters[index].KeepAspectRatio = Convert.ToByte(value);
  }
  
  public RotateDirection CurrentRotateDirection {
    get { return (RotateDirection)this.message.LayoutParameters[this.currentIndex].RotateDirection; }
    set { this.message.LayoutParameters[this.currentIndex].RotateDirection = Convert.ToInt32(value); }
  }
  public RotateDirection GetRotateDirection(int index) {
    return (RotateDirection)this.message.LayoutParameters[index].RotateDirection;
  }
  internal void SetRotateDirection(int index, RotateDirection value) {
    this.message.LayoutParameters[index].RotateDirection = Convert.ToInt32(value);
  }

  //-------------------------------------------------------------------
  // Fit
  //-------------------------------------------------------------------

  public bool CurrentFit {
    get { return this.appendices[this.currentIndex].Fit; }
    set { this.appendices[this.currentIndex].Fit = value; }
  }
  public bool GetFit(int index) {
    return this.appendices[index].Fit;
  }
  internal void SetFit(int index, bool value) {
    this.appendices[index].Fit = value;
  }

  //-------------------------------------------------------------------
  // Bound*
  //-------------------------------------------------------------------

  // ProfileはProcessに関連した情報を知ることはできない
  // よってsampleWidth/sampleHeightの存在は仮定しないこと
  // 相対比率→ピクセル値変換
  // Left/Topは切り捨て、Right/Bottomは切り上げ

  public double CurrentBoundRelativeLeft {
    get { return this.appendices[this.currentIndex].BoundRelativeLeft; }
    set { this.appendices[this.currentIndex].BoundRelativeLeft = value; }
  }
  public int CurrentBoundLeft(int sampleWidth) {
    return (int)Math.Ceiling(this.appendices[this.currentIndex].BoundRelativeLeft * sampleWidth);
  }
  public double GetBoundRelativeLeft(int index) {
    return this.appendices[index].BoundRelativeLeft;
  }
  public int GetBoundLeft(int index, int sampleWidth) {
    return (int)Math.Ceiling(this.appendices[index].BoundRelativeLeft * sampleWidth);
  }
  internal void SetBoundRelativeLeft(int index, double value) {
    this.appendices[index].BoundRelativeLeft = value;
  }

  public double CurrentBoundRelativeTop {
    get { return this.appendices[this.currentIndex].BoundRelativeTop; }
    set { this.appendices[this.currentIndex].BoundRelativeTop = value; }
  }
  public int CurrentBoundTop(int sampleHeight) {
    return (int)Math.Ceiling(this.appendices[this.currentIndex].BoundRelativeTop * sampleHeight);
  }
  public double GetBoundRelativeTop(int index) {
    return this.appendices[index].BoundRelativeTop;
  }
  public int GetBoundTop(int index, int sampleHeight) {
    return (int)Math.Ceiling(this.appendices[index].BoundRelativeTop * sampleHeight);
  }
  internal void SetBoundRelativeTop(int index, double value) {
    this.appendices[index].BoundRelativeTop = value;
  }

  public double CurrentBoundRelativeRight {
    get { return this.appendices[this.currentIndex].BoundRelativeRight; }
    set { this.appendices[this.currentIndex].BoundRelativeRight = value; }
  }
  public int CurrentBoundRight(int sampleWidth) {
    return (int)Math.Floor(this.appendices[this.currentIndex].BoundRelativeRight * sampleWidth);
  }
  public double GetBoundRelativeRight(int index) {
    return this.appendices[index].BoundRelativeRight;
  }
  public int GetBoundRight(int index, int sampleWidth) {
    return (int)Math.Ceiling(this.appendices[index].BoundRelativeRight * sampleWidth);
  }
  internal void SetBoundRelativeRight(int index, double value) {
    this.appendices[index].BoundRelativeRight = value;
  }

  public double CurrentBoundRelativeBottom {
    get { return this.appendices[this.currentIndex].BoundRelativeBottom; }
    set { this.appendices[this.currentIndex].BoundRelativeBottom = value; }
  }
  public int CurrentBoundBottom(int sampleHeight) {
    return (int)Math.Floor(this.appendices[this.currentIndex].BoundRelativeBottom * sampleHeight);
  }
  public double GetBoundRelativeBottom(int index) {
    return this.appendices[index].BoundRelativeBottom;
  }
  public int GetBoundBottom(int index, int sampleHeight) {
    return (int)Math.Ceiling(this.appendices[index].BoundRelativeBottom * sampleHeight);
  }
  internal void SetBoundRelativeBottom(int index, double value) {
    this.appendices[index].BoundRelativeBottom = value;
  }

  internal int GetBoundWidth(int index, int sampleWidth) {
    return this.GetBoundRight(index, sampleWidth) - this.GetBoundLeft(index, sampleWidth);
  }
  internal int GetBoundHeight(int index, int sampleHeight) {
    return this.GetBoundBottom(index, sampleHeight) - this.GetBoundTop(index, sampleHeight);
  }
 
  //-------------------------------------------------------------------
  // Desktop/Root ClippingX/ClippingY
  //-------------------------------------------------------------------
  
  public int CurrentDesktopClippingX {
    get { return this.appendices[this.currentIndex].DesktopClippingX; }
    set { this.appendices[this.currentIndex].DesktopClippingX = value; }
  }
  public int GetDesktopClippingX(int index) {
    return this.appendices[index].DesktopClippingX;
  }
  internal void SetDesktopClippingX(int index, int value) {
    this.appendices[index].DesktopClippingX = value;
  }

  public int CurrentDesktopClippingY {
    get { return this.appendices[this.currentIndex].DesktopClippingY; }
    set { this.appendices[this.currentIndex].DesktopClippingY = value; }
  } 
  public int GetDesktopClippingY(int index) {
    return this.appendices[index].DesktopClippingY;
  }
  internal void SetDesktopClippingY(int index, int value) {
    this.appendices[index].DesktopClippingY = value;
  }

  public int CurrentRootClippingX {
    get { return this.appendices[this.currentIndex].RootClippingX; }
    set { this.appendices[this.currentIndex].RootClippingX = value; }
  }
  public int GetRootClippingX(int index) {
    return this.appendices[index].RootClippingX;
  }
  internal void SetRootClippingX(int index, int value) {
    this.appendices[index].RootClippingX = value;
  }

  public int CurrentRootClippingY {
    get { return this.appendices[this.currentIndex].RootClippingY; }
    set { this.appendices[this.currentIndex].RootClippingY = value; }
  } 
  public int GetRootClippingY(int index) {
    return this.appendices[index].RootClippingY;
  }
  internal void SetRootClippingY(int index, int value) {
    this.appendices[index].RootClippingY = value;
  }

  //-------------------------------------------------------------------
  // CurrentIndex
  //-------------------------------------------------------------------
  public int CurrentIndex {
    get { return this.currentIndex; }
    set { this.currentIndex = value; }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  // 現在編集対象のインデックス
  private int currentIndex = 0;

  // 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  // (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  // そのほかのデータをまとめたもの
  public class Appendix {
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
