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

namespace SCFF.Common {

using System;
using System.Diagnostics;

public partial class Profile {
  /// Layout(カーソル)
  /// 
  /// - C#のインナークラスはC++のフレンドクラスと似たようなことができる！
  /// - プログラムから直接は利用してはいけないもの(this.profile.appendicesの内容で上書きされるため)
  ///   - this.profile.message.layoutParameters[*].Bound*
  ///
  /// - ProfileはProcessに関連した情報を知ることはできない
  ///   - よってsampleWidth/sampleHeightの存在は仮定しないこと
  /// - 相対比率→ピクセル値変換
  ///   - Left/Topは切り捨て、Right/Bottomは切り上げ
  public class Layout {

    // コンストラクタ
    public Layout(Profile profile, int index) {
      this.profile = profile;
      this.index = index;
    }

    public int Index {
      get { return this.index; }
    }

    //=================================================================
    // アクセッサ(単純なもの)
    //=================================================================

    // Window
    public UIntPtr Window {
      get { return (UIntPtr)this.profile.message.LayoutParameters[this.index].Window; }
      set { this.profile.message.LayoutParameters[this.index].Window = value.ToUInt64(); }
    }

    // Clipping *
    public int ClippingX {
      get { return this.profile.message.LayoutParameters[this.index].ClippingX; }
      set { this.profile.message.LayoutParameters[this.index].ClippingX = value; }
    }
    public int ClippingY {
      get { return this.profile.message.LayoutParameters[this.index].ClippingY; }
      set { this.profile.message.LayoutParameters[this.index].ClippingY = value; }
    }
    public int ClippingWidth {
      get { return this.profile.message.LayoutParameters[this.index].ClippingWidth; }
      set { this.profile.message.LayoutParameters[this.index].ClippingWidth = value; }
    }
    public int ClippingHeight {
      get { return this.profile.message.LayoutParameters[this.index].ClippingHeight; }
      set { this.profile.message.LayoutParameters[this.index].ClippingHeight = value; }
    }

    // Show *
    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].ShowCursor); }
      set { this.profile.message.LayoutParameters[this.index].ShowCursor = Convert.ToByte(value); }
    }
    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].ShowLayeredWindow); }
      set { this.profile.message.LayoutParameters[this.index].ShowLayeredWindow = Convert.ToByte(value); }
    }

    // SWScale *
    public SWScaleFlags SWScaleFlags {
      get { return (SWScaleFlags)this.profile.message.LayoutParameters[this.index].SWScaleConfig.Flags; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.Flags = Convert.ToInt32(value); }
    }
    public bool SWScaleAccurateRnd {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].SWScaleConfig.AccurateRnd); }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
    }
    public bool SWScaleIsFilterEnabled {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].SWScaleConfig.IsFilterEnabled); }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
    }
    public float SWScaleLumaGBlur {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaGblur; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaGblur = value; }
    }
    public float SWScaleChromaGBlur {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaGblur; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaGblur = value; }
    }
    public float SWScaleLumaSharpen {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaSharpen; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaSharpen = value; }
    }
    public float SWScaleChromaSharpen {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaSharpen; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaSharpen = value; }
    }
    public float SWScaleChromaHshift {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaHshift; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaHshift = value; }
    }
    public float SWScaleChromaVshift {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaVshift; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaVshift = value; }
    }

    // Other Options
    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].Stretch); }
      set { this.profile.message.LayoutParameters[this.index].Stretch = Convert.ToByte(value); }
    }
    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].KeepAspectRatio); }
      set { this.profile.message.LayoutParameters[this.index].KeepAspectRatio = Convert.ToByte(value); }
    }
    public RotateDirection RotateDirection {
      get { return (RotateDirection)this.profile.message.LayoutParameters[this.index].RotateDirection; }
      set { this.profile.message.LayoutParameters[this.index].RotateDirection = Convert.ToInt32(value); }
    }

    // WindowTypes
    public WindowTypes WindowType {
      get { return this.profile.appendices[this.index].WindowType; }
      set { this.profile.appendices[this.index].WindowType = value; }
    }

    // Fit
    public bool Fit {
      get { return this.profile.appendices[this.index].Fit; }
      set { this.profile.appendices[this.index].Fit = value; }
    }

    // Bound *
    public double BoundRelativeLeft {
      get { return this.profile.appendices[this.index].BoundRelativeLeft; }
      set { this.profile.appendices[this.index].BoundRelativeLeft = value; }
    }
    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.appendices[this.index].BoundRelativeLeft * sampleWidth);
    }
    public double BoundRelativeTop {
      get { return this.profile.appendices[this.index].BoundRelativeTop; }
      set { this.profile.appendices[this.index].BoundRelativeTop = value; }
    }
    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.appendices[this.index].BoundRelativeTop * sampleHeight);
    }
    public double BoundRelativeRight {
      get { return this.profile.appendices[this.index].BoundRelativeRight; }
      set { this.profile.appendices[this.index].BoundRelativeRight = value; }
    }
    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.appendices[this.index].BoundRelativeRight * sampleWidth);
    }
    public double BoundRelativeBottom {
      get { return this.profile.appendices[this.index].BoundRelativeBottom; }
      set { this.profile.appendices[this.index].BoundRelativeBottom = value; }
    }
    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.appendices[this.index].BoundRelativeBottom * sampleHeight);
    }

    // Desktop Clipping X/Y
    public int DesktopClippingX {
      get { return this.profile.appendices[this.index].DesktopClippingX; }
      set { this.profile.appendices[this.index].DesktopClippingX = value; }
    }
    public int DesktopClippingY {
      get { return this.profile.appendices[this.index].DesktopClippingY; }
      set { this.profile.appendices[this.index].DesktopClippingY = value; }
    }

    // Root Clipping X/Y
    public int RootClippingX {
      get { return this.profile.appendices[this.index].RootClippingX; }
      set { this.profile.appendices[this.index].RootClippingX = value; }
    }
    public int RootClippingY {
      get { return this.profile.appendices[this.index].RootClippingY; }
      set { this.profile.appendices[this.index].RootClippingY = value; }
    }

    //=================================================================
    // アクセッサ(他のアクセッサやWin32APIを必要とするもの)
    // あまり呼び出し回数が増えるようならキャッシングを考えること
    //=================================================================
    
    public string WindowCaption {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            /// @todo(me) 実装
            return this.Window.ToString();
          }
          case WindowTypes.Desktop: {
            return "(Desktop)";
          }
          case WindowTypes.Root: {
            return "(Root)";
          }
        }
        return "*** Invalid Window ***";
      }
    }

    internal int BoundWidth(int sampleWidth) {
      return this.BoundRight(sampleWidth) - this.BoundLeft(sampleWidth);
    }
    internal int BoundHeight(int index, int sampleHeight) {
      return this.BoundBottom(sampleHeight) - this.BoundTop(sampleHeight);
    }
    internal int WindowWidth {
      get {
        /// @todo(me) 実装
        throw new NotImplementedException();
      }
    }
    internal int WindowHeight {
      get {
        /// @todo(me) 実装
        throw new NotImplementedException();
      }
    }

    //=================================================================
    // メンバ変数
    //=================================================================
    private readonly int index;
    private readonly Profile profile;
  }
}
}