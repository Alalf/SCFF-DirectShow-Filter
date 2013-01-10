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
      this.SetFit = false;
      this.SetClippingXWithoutFit       = 0;
      this.SetClippingYWithoutFit       = 0;
      this.SetClippingWidthWithoutFit   = User32.GetSystemMetrics(User32.SM_CXSCREEN);
      this.SetClippingHeightWithoutFit  = User32.GetSystemMetrics(User32.SM_CYSCREEN);
    
      // 初期値を少しずつずらす
      this.SetBoundRelativeLeft =
          Constants.MinimumBoundRelativeSize * this.Index;
      this.SetBoundRelativeTop =
          Constants.MinimumBoundRelativeSize * this.Index;
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
      get { return Utilities.IsWindowValid(this.WindowType, this.Window); }
    }

    /// @copydoc ILayoutElementView::WindowType
    public WindowTypes WindowType {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowType; }
    }

    /// @copydoc ILayoutElementView::WindowCaption
    public string WindowCaption {
      get { return this.profile.additionalLayoutParameters[this.Index].WindowCaption; }
    }

    // Clipping*WithoutFitの補正
    private void FixClippingWithoutFit() {
      this.SetClippingXWithoutFit = Utilities.GetWindowOrigin(this.WindowType).Item1;
      this.SetClippingYWithoutFit = Utilities.GetWindowOrigin(this.WindowType).Item2;
      this.SetClippingWidthWithoutFit = Math.Min(
          this.ClippingWidthWithoutFit,
          Utilities.GetWindowSize(this.WindowType, this.Window).Item1);
      this.SetClippingHeightWithoutFit = Math.Min(
          this.ClippingHeightWithoutFit,
          Utilities.GetWindowSize(this.WindowType, this.Window).Item2);
    }

    /// @copydoc ILayoutElement::SetWindow
    public void SetWindow(UIntPtr window) {
      var windowChanged = this.WindowType != WindowTypes.Normal;

      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Normal;
      this.profile.message.LayoutParameters[this.Index].Window = window.ToUInt64();

      var windowCaption = "*** INVALID WINDOW ***";
      if (window != UIntPtr.Zero && User32.IsWindow(window)) {
        StringBuilder className = new StringBuilder(256);
        User32.GetClassName(window, className, 256);
        windowCaption = className.ToString();
      }
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = windowCaption;

      if (windowChanged) {
        this.FixClippingWithoutFit();
      }
    }
    /// @copydoc ILayoutElement::SetWindowToDesktop
    public void SetWindowToDesktop() {
      var windowChanged = this.WindowType != WindowTypes.Desktop;

      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.Desktop;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(Desktop)";

      if (windowChanged) {
        this.FixClippingWithoutFit();
      }
    }
    /// @copydoc ILayoutElement::SetWindowToDesktopListView
    public void SetWindowToDesktopListView() {
      var windowChanged = this.WindowType != WindowTypes.DesktopListView;

      this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.DesktopListView;
      this.profile.message.LayoutParameters[this.Index].Window = 0;
      this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(DesktopListView)";

      if (windowChanged) {
        this.FixClippingWithoutFit();
      }
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
      get {
        return this.Fit ? Utilities.GetWindowOrigin(this.WindowType).Item1
                        : this.ClippingXWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingYWithFit
    public int ClippingYWithFit {
      get {
        return this.Fit ? Utilities.GetWindowOrigin(this.WindowType).Item2
                        : this.ClippingYWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingWidthWithFit
    public int ClippingWidthWithFit {
      get {
        return this.Fit ? Utilities.GetWindowSize(this.WindowType, this.Window).Item1
                        : this.ClippingWidthWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingHeightWithFit
    public int ClippingHeightWithFit {
      get {
        return this.Fit ? Utilities.GetWindowSize(this.WindowType, this.Window).Item2
                        : this.ClippingHeightWithoutFit;
      }
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
    /// @copydoc ILayoutElement::FitBoundRelativeRect
    public void FitBoundRelativeRect(int sampleWidth, int sampleHeight) {
      Debug.Assert(this.IsWindowValid, "Invalid Window", "LayoutElement.FitBoundRelativeRect");

      // サンプル座標系でのパディングサイズを求める
      double paddingTop, paddingBottom, paddingLeft, paddingRight;
      Imaging.Utilities.CalculatePaddingSize(
          this.BoundWidth(sampleWidth),
          this.BoundHeight(sampleHeight),
          this.ClippingWidthWithFit,
          this.ClippingHeightWithFit,
          this.Stretch,
          this.KeepAspectRatio,
          out paddingTop, out paddingBottom,
          out paddingLeft, out paddingRight);
      
      // パディングサイズを相対座標系に戻す
      /// @todo(me) Fitを連続で押すと変更がとまらない可能性あり
      var paddingRelativeTop = 0.0;
      var paddingRelativeBottom = 0.0;
      var paddingRelativeLeft = 0.0;
      var paddingRelativeRight = 0.0;
      if (paddingTop + paddingBottom >= 1.0) {
        // 単位ピクセル未満の調整はしない
        paddingRelativeTop = paddingTop / sampleHeight;
        paddingRelativeBottom = paddingBottom / sampleHeight;
      }
      if (paddingLeft + paddingRight >= 1.0) {
        // 単位ピクセル未満の調整はしない
        paddingRelativeLeft = paddingLeft / sampleWidth;
        paddingRelativeRight = paddingRight / sampleWidth;
      }

      // Profileの設定を変える
      this.SetBoundRelativeLeft   = this.BoundRelativeLeft + paddingRelativeLeft;;
      this.SetBoundRelativeTop    = this.BoundRelativeTop + paddingRelativeTop;
      this.SetBoundRelativeRight  = this.BoundRelativeRight - paddingRelativeRight;
      this.SetBoundRelativeBottom = this.BoundRelativeBottom - paddingRelativeBottom;
    }

    //=================================================================
    // Screen
    //=================================================================

    /// @copydoc ILayoutElementView::ScreenClippingRectWithFit
    public ScreenRect ScreenClippingRectWithFit {
      get {
        var screenPoint = Utilities.ClientToScreen(
            this.WindowType, this.Window, this.ClippingXWithFit, this.ClippingYWithFit);
        return new ScreenRect(screenPoint.X, screenPoint.Y,
            this.ClippingWidthWithFit, this.ClippingHeightWithFit);
      }
    }

    /// @copydoc ILayoutElement::SetClippingRect
    public void SetClippingRect(ScreenRect intersectRect) {
      // ウィンドウの領域とIntersectをとる
      var boundScreenRect = Utilities.GetWindowScreenRect(this.WindowType, this.Window);
      var nextScreenRect = intersectRect.Intersect(boundScreenRect);
      if (nextScreenRect == null) {
        Debug.WriteLine("No Intersection", "LayoutElement.SetClippingRect");
        return;
      }

      // プロパティを変更
      this.SetFit = false;
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          // Screen->Client座標系変換
          this.SetClippingXWithoutFit = nextScreenRect.X - boundScreenRect.X;
          this.SetClippingYWithoutFit = nextScreenRect.Y - boundScreenRect.Y;
          break;
        }
        case WindowTypes.Desktop: {
          // 変換の必要なし
          this.SetClippingXWithoutFit = nextScreenRect.X;
          this.SetClippingYWithoutFit = nextScreenRect.Y;
          break;
        }
        case WindowTypes.DesktopListView: {
          // Screen->Client座標系変換
          this.SetClippingXWithoutFit = nextScreenRect.X - boundScreenRect.X;
          this.SetClippingYWithoutFit = nextScreenRect.Y - boundScreenRect.Y;
          break;
        }
        default : {
          Debug.Fail("Invalid WindowTypes", "LayoutElement.SetClippingRect");
          break;
        }
      }
      // Width/Heightは共通
      this.SetClippingWidthWithoutFit = nextScreenRect.Width;
      this.SetClippingHeightWithoutFit = nextScreenRect.Height;
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
    /// @copydoc ILayoutElementView::BackupClippingWidth
    public int BackupClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth; }
    }
    /// @copydoc ILayoutElementView::BackupClippingHeight
    public int BackupClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight; }
    }
    /// @copydoc ILayoutElement::UpdateBackupParameters
    public void UpdateBackupParameters() {
      var screenRect = this.ScreenClippingRectWithFit;
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX = screenRect.X;
      this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY = screenRect.Y;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth = screenRect.Width;
      this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight = screenRect.Height;
    }
  }
}
}   // namespace SCFF.Common
