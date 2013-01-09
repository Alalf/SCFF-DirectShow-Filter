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
    // LayoutElement: プロパティ
    //=================================================================

    /// Index
    public int Index { get; private set; }

    /// 対象プロファイル
    private Profile profile { get; set; }

    //=================================================================
    // ILayoutElement: メソッド
    //=================================================================

    /// 編集開始
    public void Open() {
      // nop
      /// @todo(me) OpenしてないとCloseできないようにするとバグが見つけやすい？
    }

    /// 編集終了
    public void Close() {
      this.profile.UpdateTimestamp();
    }

    /// デフォルト値を設定
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

    public UIntPtr Window {
      get { return this.GetWindow(); }
    }

    public WindowTypes WindowType {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowType; }
    }

    public string WindowCaption {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowCaption; }
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

    public int WindowHeight {
      get { return this.GetWindowHeight(); }
    }

    public int ScreenWindowX {
      get { return this.ClientXToScreenX(0); }
    }

    public int ScreenWindowY {
      get { return this.ClientYToScreenY(0); }
    }

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

    public void SetWindowToDesktop() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Desktop;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(Desktop)";
    }

    public void SetWindowToDesktopListView() {
      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.DesktopListView;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(DesktopListView)";
    }

    //=================================================================
    // Area
    //=================================================================

    public bool Fit {
      get { return this.profile.additionalLayoutParameters[this.Index].Fit; }
    }

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

    public int ClippingXWithFit {
      get { return this.Fit ? 0 : this.ClippingXWithoutFit; }
    }

    public int ClippingYWithFit {
      get { return this.Fit ? 0 : this.ClippingYWithoutFit; }
    }

    public int ClippingWidthWithFit {
      get { return this.Fit ? this.WindowWidth : this.ClippingWidthWithoutFit; }
    }

    public int ClippingHeightWithFit {
      get { return this.Fit ? this.WindowHeight : this.ClippingHeightWithoutFit; }
    }


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

    public int ScreenClippingXWithFit {
      get { return this.ClientXToScreenX(this.ClippingXWithFit); }
    }

    public int ScreenClippingYWithFit {
      get { return this.ClientYToScreenY(this.ClippingYWithFit); }
    }

    public bool SetFit {
      set { this.profile.additionalLayoutParameters[this.Index].Fit = value; }
    }

    public int SetClippingXWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingXWithoutFit = value; }
    }

    public int SetClippingYWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingYWithoutFit = value; }
    }

    public int SetClippingWidthWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingWidthWithoutFit = value; }
    }

    public int SetClippingHeightWithoutFit {
      set { this.profile.additionalLayoutParameters[this.Index].ClippingHeightWithoutFit = value; }
    }

    //=================================================================
    // Options
    //=================================================================

    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowCursor); }
    }

    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow); }
    }

    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].Stretch); }
    }

    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.Index].KeepAspectRatio); }
    }

    public RotateDirections RotateDirection {
      get { return (RotateDirections)this.profile.message.LayoutParameters[this.Index].RotateDirection; }
    }

    public bool SetShowCursor {
      set { this.profile.message.LayoutParameters[this.Index].ShowCursor = Convert.ToByte(value); }
    }

    public bool SetShowLayeredWindow {
      set { this.profile.message.LayoutParameters[this.Index].ShowLayeredWindow = Convert.ToByte(value); }
    }

    public bool SetStretch {
      set { this.profile.message.LayoutParameters[this.Index].Stretch = Convert.ToByte(value); }
    }

    public bool SetKeepAspectRatio {
      set { this.profile.message.LayoutParameters[this.Index].KeepAspectRatio = Convert.ToByte(value); }
    }

    public RotateDirections SetRotateDirection {
      set { this.profile.message.LayoutParameters[this.Index].RotateDirection = Convert.ToInt32(value); }
    }

    //=================================================================
    // ResizeMethod
    //=================================================================


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

    public SWScaleFlags SetSWScaleFlags {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.Flags = Convert.ToInt32(value); }
    }

    public bool SetSWScaleAccurateRnd {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
    }

    public bool SetSWScaleIsFilterEnabled {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
    }

    public float SetSWScaleLumaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaGblur = value; }
    }

    public float SetSWScaleChromaGBlur {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaGblur = value; }
    }

    public float SetSWScaleLumaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.LumaSharpen = value; }
    }

    public float SetSWScaleChromaSharpen {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaSharpen = value; }
    }

    public float SetSWScaleChromaHshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHshift = value; }
    }

    public float SetSWScaleChromaVshift {
      set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVshift = value; }
    }

    //=================================================================
    // LayoutParameter
    //=================================================================



    public double BoundRelativeLeft {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft; }
    }

    public double BoundRelativeTop {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop; }
    }

    public double BoundRelativeRight {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight; }
    }

    public double BoundRelativeBottom {
      get { return this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom; }
    }

    public double BoundRelativeWidth {
      get { return this.BoundRelativeRight - this.BoundRelativeLeft; }
    }

    public double BoundRelativeHeight {
      get { return this.BoundRelativeBottom - this.BoundRelativeTop; }
    }

    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft * sampleWidth);
    }

    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop * sampleHeight);
    }

    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight * sampleWidth);
    }

    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom * sampleHeight);
    }

    public int BoundWidth(int sampleWidth) {
      return this.BoundRight(sampleWidth) - this.BoundLeft(sampleWidth);
    }

    public int BoundHeight(int sampleHeight) {
      return this.BoundBottom(sampleHeight) - this.BoundTop(sampleHeight);
    }

    public double SetBoundRelativeLeft {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeLeft = value; }
    }

    public double SetBoundRelativeTop {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeTop = value; }
    }

    public double SetBoundRelativeRight {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeRight = value; }
    }

    public double SetBoundRelativeBottom {
      set { this.profile.additionalLayoutParameters[this.Index].BoundRelativeBottom = value; }
    }

    //=================================================================
    // Backup
    //=================================================================

    public int BackupScreenClippingX {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX; }
    }

    public int BackupScreenClippingY {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY; }
    }

    public int BackupDesktopClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth; }
    }

    public int BackupDesktopClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight; }
    }

    public void UpdateBackupParameters() {
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX = this.ScreenClippingXWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY = this.ScreenClippingYWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth = this.ClippingWidthWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight = this.ClippingHeightWithFit;
    }
  }
}
}   // namespace SCFF.Common
