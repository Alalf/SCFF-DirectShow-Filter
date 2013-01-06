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

/// @file SCFF.Common/Profile.OutputLayoutElement.cs
/// プロファイル内を操作するためのカーソルクラス

namespace SCFF.Common {

using System;
using System.Text;
using SCFF.Common.Ext;

public partial class Profile {

  /// プロファイル内を操作するためのカーソルクラス
  public class OutputLayoutElement : LayoutElement {
    //=================================================================
    // コンストラクタ
    //=================================================================

    /// コンストラクタ
    public OutputLayoutElement(Profile profile, int index)
        : base(profile, index) {
      // nop
    }

    //=================================================================
    // プロパティ
    //=================================================================

    /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
    public bool ShowCursor {
      set { this.profile.message.LayoutParameters[this.Index].ShowCursor = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
    public bool ShowLayeredWindow {
      set { this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
    public bool Stretch {
      set { this.profile.message.LayoutParameters[this.Index].Stretch = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
    public bool KeepAspectRatio {
      set { this.profile.message.LayoutParameters[this.Index].KeepAspectRatio = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
    public RotateDirections RotateDirection {
      set { this.profile.message.LayoutParameters[this.Index].RotateDirection = Convert.ToInt32(value); }
    }

    //-----------------------------------------------------------------

    /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
    public SWScaleFlags SWScaleFlags {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags = Convert.ToInt32(value); }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
    public bool SWScaleAccurateRnd {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
    public bool SWScaleIsFilterEnabled {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
    public float SWScaleLumaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur = value; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
    public float SWScaleChromaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur = value; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
    public float SWScaleLumaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen = value; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
    public float SWScaleChromaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen = value; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHshift
    public float SWScaleChromaHshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift = value; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVshift
    public float SWScaleChromaVshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift = value; }
    }

    //-----------------------------------------------------------------

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::Fit
    public bool Fit {
      set { this.profile.additionalLayoutParameters[this.Index].Fit = value; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeLeft
    public double BoundRelativeLeft {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeTop
    public double BoundRelativeTop {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeRight
    public double BoundRelativeRight {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeBottom
    public double BoundRelativeBottom {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom = value; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingXWithoutFit
    public int ClippingXWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingYWithoutFit
    public int ClippingYWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingWidthWithoutFit
    public int ClippingWidthWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit = value; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingHeightWithoutFit
    public int ClippingHeightWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit = value; }
    }

    //=================================================================
    // アクセサ
    //=================================================================
    
    /// WindowタイプをNormalに＋Windowハンドルを設定する
    public void SetWindow(UIntPtr window) {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Normal;
      this.profile.message.LayoutParameters[this.Index].Window = window.ToUInt64();

      var windowCaption = "*** INVALID WINDOW ***";
      if (window != UIntPtr.Zero && User32.IsWindow(window)) {
        StringBuilder className = new StringBuilder(256);
        User32.GetClassName(window, className, 256);
        windowCaption = className.ToString();
      }
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = windowCaption;
    }
    /// WindowタイプをDesktopにする
    public void SetWindowToDesktop() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Desktop;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(Desktop)";
    }
    /// WindowタイプをDesktopListViewにする
    public void SetWindowToDesktopListView() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.DesktopListView;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(DesktopListView)";
    }
  }
}
}