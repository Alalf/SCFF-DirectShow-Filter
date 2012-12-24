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

/// @file SCFF.Common/Profile.InputLayoutElement.cs
/// プロファイル内を参照するためのカーソルクラス

namespace SCFF.Common {

using System;
using System.Diagnostics;
using System.Text;

public partial class Profile {

  /// プロファイル内を参照するためのカーソルクラス
  public class InputLayoutElement : LayoutElement {

    /// コンストラクタ
    public InputLayoutElement(Profile profile, int index) : base(profile, index) {
      // nop
    }

    //=================================================================
    // アクセッサ(単純なもの)
    //=================================================================

    // Show *
    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowCursor); }
    }
    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow); }
    }

    // SWScale *
    public SWScaleFlags SWScaleFlags {
      get { return (SWScaleFlags)this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags; }
    }
    public bool SWScaleAccurateRnd {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd); }
    }
    public bool SWScaleIsFilterEnabled {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled); }
    }
    public float SWScaleLumaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur; }
    }
    public float SWScaleChromaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur; }
    }
    public float SWScaleLumaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen; }
    }
    public float SWScaleChromaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen; }
    }
    public float SWScaleChromaHshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift; }
    }
    public float SWScaleChromaVshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift; }
    }

    // Other Options
    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].Stretch); }
    }
    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].KeepAspectRatio); }
    }
    public RotateDirections RotateDirection {
      get { return (RotateDirections)this.profile.message.LayoutParameters[this.Index].RotateDirection; }
    }

    // Fit
    public bool Fit {
      get { return this.profile.additionalLayoutParameters[this.Index].Fit; }
    }

    // Bound *
    // - 相対比率→ピクセル値変換
    //   - Left/Topは切り捨て、Right/Bottomは切り上げ
    public double BoundRelativeLeft {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft; }
    }
    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft * sampleWidth);
    }
    public double BoundRelativeTop {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop; }
    }
    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop * sampleHeight);
    }
    public double BoundRelativeRight {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight; }
    }
    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight * sampleWidth);
    }
    public double BoundRelativeBottom {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom; }
    }
    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom * sampleHeight);
    }

    // Clipping*WithoutFit
    public int ClippingXWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit; }
    }
    public int ClippingYWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit; }
    }
    public int ClippingWidthWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit; }
    }
    public int ClippingHeightWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit; }
    }

    //=================================================================
    // アクセッサ(他のアクセッサやWin32APIを必要とするもの)
    // あまり呼び出し回数が増えるようならキャッシングを考えること
    //=================================================================
    
    // Window
    internal UIntPtr Window {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            return (UIntPtr)this.profile.message.LayoutParameters[this.Index].Window;
          }
          case WindowTypes.DesktopListView: {
            return Utilities.DesktopListViewWindow;
          }
          case WindowTypes.Desktop: {
            return Utilities.DesktopWindow;
          }
          default: {
            Debug.Fail("Window: Invalid WindowTypes");
            return UIntPtr.Zero;
          }
        }
      }
    }
    public WindowTypes WindowType {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowType; }
    }

    public int BoundWidth(int sampleWidth) {
      return this.BoundRight(sampleWidth) - this.BoundLeft(sampleWidth);
    }
    public int BoundHeight(int sampleHeight) {
      return this.BoundBottom(sampleHeight) - this.BoundTop(sampleHeight);
    }

    public string WindowCaption {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
              return "*** INVALID WINDOW ***";
            }
            StringBuilder className = new StringBuilder(256);
            ExternalAPI.GetClassName(this.Window, className, 256);
            return className.ToString();
          }
          case WindowTypes.DesktopListView: {
            return "(DesktopListView)";
          }
          case WindowTypes.Desktop: {
            return "(Desktop)";
          }
          default: {
            Debug.Fail("WindowCaption: Unknown Type");
            return "*** UNKNOWN TYPE WINDOW ***";
          }
        }
      }
    }

    public int WindowWidth {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
              Debug.WriteLine("WindowWidth: Invalid Window");
              return -1;
            }
            ExternalAPI.RECT windowRect;
            ExternalAPI.GetClientRect(this.Window, out windowRect);
            return windowRect.Right - windowRect.Left;
          }
          case WindowTypes.DesktopListView:
          case WindowTypes.Desktop: {
            return ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CXVIRTUALSCREEN);
          }
          default: {
            Debug.Fail("WindowWidth: Invalid WindowType");
            return -1;
          }
        }
      }
    }
    public int WindowHeight {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
              Debug.WriteLine("WindowHeight: Invalid Window");
              return -1;
            }
            ExternalAPI.RECT windowRect;
            ExternalAPI.GetClientRect(this.Window, out windowRect);
            return windowRect.Bottom - windowRect.Top;
          }
          case WindowTypes.DesktopListView:
          case WindowTypes.Desktop: {
            return ExternalAPI.GetSystemMetrics(ExternalAPI.SM_CYVIRTUALSCREEN);
          }
          default: {
            Debug.Fail("WindowHeight: Invalid WindowType");
            return -1;
          }
        }
      }
    }

    // clipping*WithFit {
    public int ClippingXWithFit {
      get {
        if (this.Fit) {
          return 0;
        } else {
          return this.ClippingXWithoutFit;
        }
      }
    }
    public int ClippingYWithFit {
      get {
        if (this.Fit) {
          return 0;
        } else {
          return this.ClippingYWithoutFit;
        }
      }
    }
    public int ClippingWidthWithFit {
      get {
        if (this.Fit) {
          return this.WindowWidth;
        } else {
          return this.ClippingWidthWithoutFit;
        }
      }
    }
    public int ClippingHeightWithFit {
      get {
        if (this.Fit) {
          return this.WindowHeight;
        } else {
          return this.ClippingHeightWithoutFit;
        }
      }
    }

    // BackupDesktopClippingXParameters
    public int BackupDesktopClippingX {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingX; }
    }
    public int BackupDesktopClippingY {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingY; }
    }
    public int BackupDesktopClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingWidth; }
    }
    public int BackupDesktopClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingHeight; }
    }
  }
}
}