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

/// @file SCFF.Common/Profile.LayoutElement.cs
/// @copydoc SCFF::Common::Profile::LayoutElement

namespace SCFF.Common {

using System;
using System.Diagnostics;
using System.Text;
using SCFF.Common.Ext;

public partial class Profile {
  /// プロファイル内を参照・操作するためのカーソルクラス
  /// 
  /// - C#のインナークラスはC++のフレンドクラスと似たようなことができる！
  /// - プログラムから直接は利用してはいけないもの(this.profile.appendicesの内容で上書きされるため)
  ///   - this.profile.message.layoutParameters[*].Bound*
  ///   - this.profile.message.layoutParameters[*].Clipping*
  /// - 以下の内容も最新のデータがあることは保障しない
  ///   - this.profile.message.layoutParameters[*].Window
  /// - ProfileはProcessに関連した情報を知ることはできない
  ///   - よってsampleWidth/sampleHeightの存在は仮定しないこと
  public class LayoutElement : ILayoutElementView, ILayoutElement {
    //=================================================================
    // コンストラクタ/Dispose
    //=================================================================

    /// コンストラクタ
    public LayoutElement(Profile profile, int index) {
      this.profile = profile;
      this.Index = index;
    }

    //=================================================================
    // プロパティ
    //=================================================================

    /// 対象プロファイル
    private Profile profile { get; set; }

    //=================================================================
    // ILayoutElementView: プロパティ
    //=================================================================

    /// @copydoc ILayoutElementView::Index
    public int Index { get; private set; }

    //=================================================================
    // ILayoutElement: メソッド
    //=================================================================

    /// @copydoc ILayoutElement::Open
    public void Open() {
      // nop
      /// @todo(me) OpenしてないとCloseできないようにするとバグが見つけやすい？
    }

    /// @copydoc ILayoutElement::Close
    public void Close() {
      this.profile.UpdateTimestamp();
    }

    /// @copydoc ILayoutElement::RestoreDefault
    /// @todo(me) インスタンスを生成する形でゼロクリアしているが非効率的？
    public void RestoreDefault() {
      // newで参照型をゼロクリア
      var layoutParameter = new Interprocess.LayoutParameter();
      layoutParameter.SWScaleConfig = new Interprocess.SWScaleConfig();
      this.profile.message.LayoutParameters[this.Index] = layoutParameter;
      this.profile.additionalLayoutParameters[this.Index] = new AdditionalLayoutParameter();

      this.SetKeepAspectRatio = true;
      this.SetRotateDirection = RotateDirections.NoRotate;
      this.SetStretch = true;
      this.SetSWScaleFlags = SWScaleFlags.Area;
      this.SetSWScaleIsFilterEnabled = false;

      // プライマリモニタを表示
      this.SetWindowToDesktop();
      this.SetClippingXWithoutFit       = 0 - User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN);
      this.SetClippingYWithoutFit       = 0 - User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN);
      this.SetClippingWidthWithoutFit   = User32.GetSystemMetrics(User32.SM_CXSCREEN);
      this.SetClippingHeightWithoutFit  = User32.GetSystemMetrics(User32.SM_CYSCREEN);
    
      this.SetFit = false;

      /// @todo(me) (IndexでRelativeLeft/Topをずらす)
      ///           レイアウト配置時に毎回サイズと場所がかぶっているのはわかりづらいのでずらしたい

      this.SetBoundRelativeRight = 1.0;
      this.SetBoundRelativeBottom = 1.0;
    }

    //=================================================================
    // TargetWindow
    //=================================================================

    /// @copydoc ILayoutElementView::Window
    public UIntPtr Window {
      get {
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
            Debug.Fail("Invalid WindowTypes", "LayoutElement.Window");
            return UIntPtr.Zero;
          }
        }
      }
    }
    /// @copydoc ILayoutElementView::IsWindowValid
    public bool IsWindowValid {
      get {
        switch(this.WindowType) {
          case WindowTypes.Normal: {
            return (this.Window != UIntPtr.Zero && User32.IsWindow(this.Window));
          }
          case WindowTypes.DesktopListView:
          case WindowTypes.Desktop: {
            return true;
          }
          default: {
            Debug.Fail("Invalid WindowTypes", "LayoutElement.IsWindowValid");
            return false;
          }
        }
      }
    }

    /// @copydoc ILayoutElementView::WindowType
    public WindowTypes WindowType {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowType; }
    }

    /// @copydoc ILayoutElementView::WindowCaption
    public string WindowCaption {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowCaption; }
    }

    /// @copydoc ILayoutElementView::WindowWidth
    public int WindowWidth {
      get { return this.windowSize.Item1; }
    }
    /// @copydoc ILayoutElementView::WindowHeight
    public int WindowHeight {
      get { return this.windowSize.Item2; }
    }

    /// @copydoc ILayoutElementView::ScreenWindowX
    public int ScreenWindowX {
      get { return this.ClientToScreen(0, 0).Item1; }
    }
    /// @copydoc ILayoutElementView::ScreenWindowY
    public int ScreenWindowY {
      get { return this.ClientToScreen(0, 0).Item2; }
    }

    /// @copydoc ILayoutElement::SetWindow
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
    /// @copydoc ILayoutElement::SetWindowToDesktop
    public void SetWindowToDesktop() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Desktop;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(Desktop)";
    }
    /// @copydoc ILayoutElement::SetWindowToDesktopListView
    public void SetWindowToDesktopListView() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.DesktopListView;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(DesktopListView)";
    }

    //=================================================================
    // Area
    //=================================================================

    /// @copydoc ILayoutElementView::Fit
    public bool Fit {
      get { return this.profile.additionalLayoutParameters[this.Index].Fit; }
    }
    /// @copydoc ILayoutElementView::ClippingXWithoutFit
    public int ClippingXWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingYWithoutFit
    public int ClippingYWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingWidthWithoutFit
    public int ClippingWidthWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingHeightWithoutFit
    public int ClippingHeightWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingXWithFit
    public int ClippingXWithFit {
      get { return this.Fit ? 0 : this.ClippingXWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingYWithFit
    public int ClippingYWithFit {
      get { return this.Fit ? 0 : this.ClippingYWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingWidthWithFit
    public int ClippingWidthWithFit {
      get { return this.Fit ? this.WindowWidth : this.ClippingWidthWithoutFit; }
    }
    /// @copydoc ILayoutElementView::ClippingHeightWithFit
    public int ClippingHeightWithFit {
      get { return this.Fit ? this.WindowHeight : this.ClippingHeightWithoutFit; }
    }

    /// @copydoc ILayoutElementView::ScreenClippingXWithFit
    public int ScreenClippingXWithFit {
      get { return this.ClientToScreen(this.ClippingXWithFit, this.ClippingYWithFit).Item1; }
    }
    /// @copydoc ILayoutElementView::ScreenClippingYWithFit
    public int ScreenClippingYWithFit {
      get { return this.ClientToScreen(this.ClippingXWithFit, this.ClippingYWithFit).Item2; }
    }

    /// @copydoc ILayoutElement::SetFit
    public bool SetFit {
      set { this.profile.additionalLayoutParameters[this.Index].Fit = value; }
    }
    /// @copydoc ILayoutElement::SetClippingXWithoutFit
    public int SetClippingXWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit = value; }
    }
    /// @copydoc ILayoutElement::SetClippingYWithoutFit
    public int SetClippingYWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit = value; }
    }
    /// @copydoc ILayoutElement::SetClippingWidthWithoutFit
    public int SetClippingWidthWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit = value; }
    }
    /// @copydoc ILayoutElement::SetClippingHeightWithoutFit
    public int SetClippingHeightWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit = value; }
    }

    //=================================================================
    // Options
    //=================================================================

    /// @copydoc ILayoutElementView::ShowCursor
    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowCursor); }
    }
    /// @copydoc ILayoutElementView::ShowLayeredWindow
    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow); }
    }
    /// @copydoc ILayoutElementView::Stretch
    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].Stretch); }
    }
    /// @copydoc ILayoutElementView::KeepAspectRatio
    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].KeepAspectRatio); }
    }
    /// @copydoc ILayoutElementView::RotateDirection
    public RotateDirections RotateDirection {
      get { return (RotateDirections)this.profile.message.LayoutParameters[this.Index].RotateDirection; }
    }

    /// @copydoc ILayoutElement::SetShowCursor
    public bool SetShowCursor {
      set { this.profile.message.LayoutParameters[this.Index].ShowCursor = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetShowLayeredWindow
    public bool SetShowLayeredWindow {
      set { this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetStretch
    public bool SetStretch {
      set { this.profile.message.LayoutParameters[this.Index].Stretch = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetKeepAspectRatio
    public bool SetKeepAspectRatio {
      set { this.profile.message.LayoutParameters[this.Index].KeepAspectRatio = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetRotateDirection
    public RotateDirections SetRotateDirection {
      set { this.profile.message.LayoutParameters[this.Index].RotateDirection = Convert.ToInt32(value); }
    }

    //=================================================================
    // ResizeMethod
    //=================================================================

    /// @copydoc ILayoutElementView::SWScaleFlags
    public SWScaleFlags SWScaleFlags {
      get { return (SWScaleFlags)this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags; }
    }
    /// @copydoc ILayoutElementView::SWScaleAccurateRnd
    public bool SWScaleAccurateRnd {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd); }
    }
    /// @copydoc ILayoutElementView::SWScaleIsFilterEnabled
    public bool SWScaleIsFilterEnabled {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled); }
    }
    /// @copydoc ILayoutElementView::SWScaleLumaGBlur
    public float SWScaleLumaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur; }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaGBlur
    public float SWScaleChromaGBlur {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur; }
    }
    /// @copydoc ILayoutElementView::SWScaleLumaSharpen
    public float SWScaleLumaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen; }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaSharpen
    public float SWScaleChromaSharpen {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen; }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaHshift
    public float SWScaleChromaHshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift; }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaVshift
    public float SWScaleChromaVshift {
      get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift; }
    }

    /// @copydoc ILayoutElement::SetSWScaleFlags
    public SWScaleFlags SetSWScaleFlags {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags = Convert.ToInt32(value); }
    }
    /// @copydoc ILayoutElement::SetSWScaleAccurateRnd
    public bool SetSWScaleAccurateRnd {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetSWScaleIsFilterEnabled
    public bool SetSWScaleIsFilterEnabled {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
    }
    /// @copydoc ILayoutElement::SetSWScaleLumaGBlur
    public float SetSWScaleLumaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur = value; }
    }
    /// @copydoc ILayoutElement::SetSWScaleChromaGBlur
    public float SetSWScaleChromaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur = value; }
    }
    /// @copydoc ILayoutElement::SetSWScaleLumaSharpen
    public float SetSWScaleLumaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen = value; }
    }
    /// @copydoc ILayoutElement::SetSWScaleChromaSharpen
    public float SetSWScaleChromaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen = value; }
    }
    /// @copydoc ILayoutElement::SetSWScaleChromaHshift
    public float SetSWScaleChromaHshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift = value; }
    }
    /// @copydoc ILayoutElement::SetSWScaleChromaVshift
    public float SetSWScaleChromaVshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift = value; }
    }

    //=================================================================
    // LayoutParameter
    //=================================================================

    /// @copydoc ILayoutElementView::BoundRelativeLeft
    public double BoundRelativeLeft {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft; }
    }
    /// @copydoc ILayoutElementView::BoundRelativeTop
    public double BoundRelativeTop {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop; }
    }
    /// @copydoc ILayoutElementView::BoundRelativeRight
    public double BoundRelativeRight {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight; }
    }
    /// @copydoc ILayoutElementView::BoundRelativeBottom
    public double BoundRelativeBottom {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom; }
    }
    /// @copydoc ILayoutElementView::BoundRelativeWidth
    public double BoundRelativeWidth {
      get { return this.BoundRelativeRight - this.BoundRelativeLeft; }
    }
    /// @copydoc ILayoutElementView::BoundRelativeHeight
    public double BoundRelativeHeight {
      get { return this.BoundRelativeBottom - this.BoundRelativeTop; }
    }
    /// @copydoc ILayoutElementView::BoundLeft
    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft * sampleWidth);
    }
    /// @copydoc ILayoutElementView::BoundTop
    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop * sampleHeight);
    }
    /// @copydoc ILayoutElementView::BoundRight
    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight * sampleWidth);
    }
    /// @copydoc ILayoutElementView::BoundBottom
    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom * sampleHeight);
    }
    /// @copydoc ILayoutElementView::BoundWidth
    public int BoundWidth(int sampleWidth) {
      return this.BoundRight(sampleWidth) - this.BoundLeft(sampleWidth);
    }
    /// @copydoc ILayoutElementView::BoundHeight
    public int BoundHeight(int sampleHeight) {
      return this.BoundBottom(sampleHeight) - this.BoundTop(sampleHeight);
    }

    /// @copydoc ILayoutElement::SetBoundRelativeLeft
    public double SetBoundRelativeLeft {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft = value; }
    }
    /// @copydoc ILayoutElement::SetBoundRelativeTop
    public double SetBoundRelativeTop {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop = value; }
    }
    /// @copydoc ILayoutElement::SetBoundRelativeRight
    public double SetBoundRelativeRight {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight = value; }
    }
    /// @copydoc ILayoutElement::SetBoundRelativeBottom
    public double SetBoundRelativeBottom {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom = value; }
    }

    //=================================================================
    // Backup
    //=================================================================

    /// @copydoc ILayoutElementView::BackupScreenClippingX
    public int BackupScreenClippingX {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX; }
    }
    /// @copydoc ILayoutElementView::BackupScreenClippingY
    public int BackupScreenClippingY {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY; }
    }
    /// @copydoc ILayoutElementView::BackupDesktopClippingWidth
    public int BackupDesktopClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth; }
    }
    /// @copydoc ILayoutElementView::BackupDesktopClippingHeight
    public int BackupDesktopClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight; }
    }
    /// @copydoc ILayoutElement::UpdateBackupParameters
    public void UpdateBackupParameters() {
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX = this.ScreenClippingXWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY = this.ScreenClippingYWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth = this.ClippingWidthWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight = this.ClippingHeightWithFit;
    }

    //=================================================================
    // private メソッド
    //=================================================================

    /// Windowの幅をタイプ別に取得する
    private Tuple<int, int> windowSize {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            if (!this.IsWindowValid) {
              Debug.Fail("Invalid Window", "LayoutElement.windowSize");
              return null;
            }
            User32.RECT windowRect;
            User32.GetClientRect(this.Window, out windowRect);
            return new Tuple<int,int>(windowRect.Right - windowRect.Left,
                                      windowRect.Bottom - windowRect.Top);
          }
          case WindowTypes.DesktopListView:
          case WindowTypes.Desktop: {
            return new Tuple<int,int>(User32.GetSystemMetrics(User32.SM_CXVIRTUALSCREEN),
                                      User32.GetSystemMetrics(User32.SM_CYVIRTUALSCREEN));
          }
          default: {
            Debug.Fail("Invalid WindowTypes", "LayoutElement.windowSize");
            return null;
          }
        }
      }
    }

    /// Client座標系のX座標をWindowタイプ別にScreen座標系に変換する
    private Tuple<int, int> ClientToScreen(int clientX, int clientY) {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (!this.IsWindowValid) {
            Debug.Fail("Invalid Window", "LayoutElement.ClientToScreen");
            return null;
          }
          User32.POINT windowPoint = new User32.POINT { X = clientX, Y = clientY };
          User32.ClientToScreen(this.Window, ref windowPoint);
          return new Tuple<int, int>(windowPoint.X, windowPoint.Y);
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          //　仮想デスクトップ座標なので補正を戻す
          return new Tuple<int, int>(clientX + User32.GetSystemMetrics(User32.SM_XVIRTUALSCREEN),
                                     clientY + User32.GetSystemMetrics(User32.SM_YVIRTUALSCREEN));
        }
        default: {
          Debug.Fail("Invalid WindowTypes", "LayoutElement.ClientToScreen");
          return null;
        }
      }
    }
  }
}
}   // namespace SCFF.Common
