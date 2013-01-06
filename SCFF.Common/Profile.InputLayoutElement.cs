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

/// @file SCFF.Common/Profile.InputLayoutElement.cs
/// @copydoc SCFF::Common::Profile::InputLayoutElement

namespace SCFF.Common {

using System;
using System.Diagnostics;
using SCFF.Common.Ext;

public partial class Profile {

  /// プロファイル内を参照するためのカーソルクラス
  public class InputLayoutElement : LayoutElement {
    //=================================================================
    // コンストラクタ
    //=================================================================

    /// コンストラクタ
    public InputLayoutElement(Profile profile, int index)
        : base(profile, index) {
      // nop
    }

    //=================================================================
    // プロパティ
    //=================================================================

    /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowCursor); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].Stretch); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].KeepAspectRatio); }
    }
    /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
    public RotateDirections RotateDirection {
      get { return (RotateDirections)this.profile.message.LayoutParameters[this.Index].RotateDirection; }
    }

    //-----------------------------------------------------------------

    /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
    public SWScaleFlags SWScaleFlags {
      get { return (SWScaleFlags)this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
    public bool SWScaleAccurateRnd {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd); }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
    public bool SWScaleIsFilterEnabled {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled); }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
    public float SWScaleLumaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
    public float SWScaleChromaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
    public float SWScaleLumaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
    public float SWScaleChromaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHshift
    public float SWScaleChromaHshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift; }
    }
    /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVshift
    public float SWScaleChromaVshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift; }
    }

    //-----------------------------------------------------------------

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::WindowType
    public WindowTypes WindowType {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowType; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::WindowCaption
    public string WindowCaption {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowCaption; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::Fit
    public bool Fit {
      get { return this.profile.additionalLayoutParameters[this.Index].Fit; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeLeft
    public double BoundRelativeLeft {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeTop
    public double BoundRelativeTop {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeRight
    public double BoundRelativeRight {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BoundRelativeBottom
    public double BoundRelativeBottom {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingXWithoutFit
    public int ClippingXWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingYWithoutFit
    public int ClippingYWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingWidthWithoutFit
    public int ClippingWidthWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::ClippingHeightWithoutFit
    public int ClippingHeightWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit; }
    }

    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupDesktopClippingX
    public int BackupDesktopClippingX {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingX; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupDesktopClippingY
    public int BackupDesktopClippingY {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupDesktopClippingY; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupClippingWidth
    public int BackupDesktopClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth; }
    }
    /// @copydoc SCFF::Common::AdditionalLayoutParameter::BackupClippingHeight
    public int BackupDesktopClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight; }
    }

    //=================================================================
    // 二次プロパティ
    //=================================================================

    // 他のプロパティやWin32APIを必要とするプロパティ
    // あまり呼び出し回数が増えるようならキャッシングを考えること
    
    /// Windowハンドルをタイプ別に取得する
    private UIntPtr GetWindow() {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          return (UIntPtr)this.profile.message.LayoutParameters[this.Index].Window;
        }
        case WindowTypes.DesktopListView: {
          return Utilities.DesktopListViewWindow;
        }
        case WindowTypes.Desktop: {
          return User32.GetDesktopWindow();
        }
        default: {
          Debug.Fail("Window: Invalid WindowTypes");
          return UIntPtr.Zero;
        }
      }
    }

    /// Windowハンドル
    public UIntPtr Window {
      get { return this.GetWindow(); }
    }

    /// Windowの幅をタイプ別に取得する
    private int GetWindowWidth() {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (this.Window == UIntPtr.Zero || !User32.IsWindow(this.Window)) {
            Debug.WriteLine("WindowWidth: Invalid Window");
            return -1;
          }
          User32.RECT windowRect;
          User32.GetClientRect(this.Window, out windowRect);
          return windowRect.Right - windowRect.Left;
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          return User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN);
        }
        default: {
          Debug.Fail("WindowWidth: Invalid WindowType");
          return -1;
        }
      }
    }

    /// Windowの幅
    public int WindowWidth {
      get { return this.GetWindowWidth(); }
    }

    /// Windowの高さをタイプ別に取得する
    private int GetWindowHeight() {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (this.Window == UIntPtr.Zero || !User32.IsWindow(this.Window)) {
            Debug.WriteLine("GetWindowHeight: Invalid Window");
            return -1;
          }
          User32.RECT windowRect;
          User32.GetClientRect(this.Window, out windowRect);
          return windowRect.Bottom - windowRect.Top;
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          return User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN);
        }
        default: {
          Debug.Fail("GetWindowHeight: Invalid WindowType");
          return -1;
        }
      }
    }

    /// Windowの高さ
    public int WindowHeight {
      get { return this.GetWindowHeight(); }
    }

    //-----------------------------------------------------------------

    /// 相対座標系でのレイアウト要素の幅
    public double BoundRelativeWidth {
      get { return this.BoundRelativeRight - this.BoundRelativeLeft; }
    }
    /// 相対座標系でのレイアウト要素の高さ
    public double BoundRelativeHeight {
      get { return this.BoundRelativeBottom - this.BoundRelativeTop; }
    }

    //-----------------------------------------------------------------

    /// Fitオプションを考慮したクリッピング領域左上端のX座標
    public int ClippingXWithFit {
      get { return this.Fit ? 0 : this.ClippingXWithoutFit; }
    }
    /// Fitオプションを考慮したクリッピング領域左上端のY座標
    public int ClippingYWithFit {
      get { return this.Fit ? 0 : this.ClippingYWithoutFit; }
    }
    /// Fitオプションを考慮したクリッピング領域の幅
    public int ClippingWidthWithFit {
      get { return this.Fit ? this.WindowWidth : this.ClippingWidthWithoutFit; }
    }
    /// Fitオプションを考慮したクリッピング領域の高さ
    public int ClippingHeightWithFit {
      get { return this.Fit ? this.WindowHeight : this.ClippingHeightWithoutFit; }
    }

    //-----------------------------------------------------------------

    /// Client座標系のX座標をWindowタイプ別にScreen座標系に変換する
    private int ClientXToScreenX(int clientX) {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (this.Window == UIntPtr.Zero || !User32.IsWindow(this.Window)) {
            Debug.Fail("ClientXToScreenX: Invalid Window");
            return -1;
          }
          User32.POINT windowPoint = new User32.POINT { X = clientX, Y = 0 };
          User32.ClientToScreen(this.Window, ref windowPoint);
          return windowPoint.X;
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          //　仮想デスクトップ座標なので補正を戻す
          return clientX + User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN);
        }
        default: {
          Debug.Fail("ClientXToScreenX: Invalid WindowType");
          return -1;
        }
      }
    }

    /// Client座標系のY座標をWindowタイプ別にScreen座標系に変換する
    private int ClientYToScreenY(int clientY) {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (this.Window == UIntPtr.Zero || !User32.IsWindow(this.Window)) {
            Debug.Fail("ClientYToScreenY: Invalid Window");
            return -1;
          }
          User32.POINT windowPoint = new User32.POINT { X = 0, Y = clientY };
          User32.ClientToScreen(this.Window, ref windowPoint);
          return windowPoint.Y;
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          //　仮想デスクトップ座標なので補正を戻す
          return clientY + User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN);
        }
        default: {
          Debug.Fail("ClientYToScreenY: Invalid WindowType");
          return -1;
        }
      }
    }

    /// Screen座標系でのWindow左上端のX座標
    public int ScreenWindowX {
      get { return this.ClientXToScreenX(0); }
    }
    /// Screen座標系でのWindow左上端のY座標
    public int ScreenWindowY {
      get { return this.ClientYToScreenY(0); }
    }
    /// Screen座標系でのクリッピング領域左上端のX座標
    public int ScreenClippingXWithFit {
      get { return this.ClientXToScreenX(this.ClippingXWithFit); }
    }
    /// Screen座標系でのクリッピング領域左上端のY座標
    public int ScreenClippingYWithFit {
      get { return this.ClientYToScreenY(this.ClippingYWithFit); }
    }

    //=================================================================
    // アクセサ
    //=================================================================

    /// サンプル上のレイアウト要素左上端のX座標
    /// @attention 小数点以下切り捨て
    /// @param sampleWidth サンプルの幅
    /// @return サンプル上のレイアウト要素左上端のX座標
    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft * sampleWidth);
    }
    /// サンプル上のレイアウト要素左上端のY座標
    /// @attention 小数点以下切り捨て
    /// @param sampleHeight サンプルの高さ
    /// @return サンプル上のレイアウト要素左上端のY座標
    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop * sampleHeight);
    }
    /// サンプル上のレイアウト要素右下端のX座標
    /// @attention 小数点以下切り上げ
    /// @param sampleWidth サンプルの幅
    /// @return サンプル上のレイアウト要素右下端のX座標
    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight * sampleWidth);
    }
    /// サンプル上のレイアウト要素右下端のY座標
    /// @attention 小数点以下切り上げ
    /// @param sampleHeight サンプルの高さ
    /// @return サンプル上のレイアウト要素右下端のY座標
    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom * sampleHeight);
    }

    /// サンプル上のレイアウト要素の幅
    /// @param sampleWidth サンプルの幅
    /// @return サンプル上のレイアウト要素の幅
    public int BoundWidth(int sampleWidth) {
      return this.BoundRight(sampleWidth) - this.BoundLeft(sampleWidth);
    }
    /// サンプル上のレイアウト要素の高さ
    /// @param sampleHeight サンプルの高さ
    /// @return サンプル上のレイアウト要素の高さ
    public int BoundHeight(int sampleHeight) {
      return this.BoundBottom(sampleHeight) - this.BoundTop(sampleHeight);
    }
  }
}
}   // namespace SCFF.Common
