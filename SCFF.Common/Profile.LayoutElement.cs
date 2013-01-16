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

    /// Clipping*WithoutFitの補正
    private void FixClippingWithoutFit() {
      var windowRect = Utilities.GetWindowRect(this.WindowType, this.Window);
      this.SetClippingXWithoutFit = windowRect.X;
      this.SetClippingYWithoutFit = windowRect.Y;
      this.SetClippingWidthWithoutFit =
          Math.Min(this.ClippingWidthWithoutFit, windowRect.Width);
      this.SetClippingHeightWithoutFit =
          Math.Min(this.ClippingHeightWithoutFit, windowRect.Height);
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
        return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).X
                        : this.ClippingXWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingYWithFit
    public int ClippingYWithFit {
      get {
        return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Y
                        : this.ClippingYWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingWidthWithFit
    public int ClippingWidthWithFit {
      get {
        return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Width
                        : this.ClippingWidthWithoutFit;
      }
    }
    /// @copydoc ILayoutElementView::ClippingHeightWithFit
    public int ClippingHeightWithFit {
      get {
        return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Height
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

      // クリッピング領域が存在していなければそのまま戻る
      if (this.ClippingWidthWithoutFit <= 0 || this.ClippingHeightWithoutFit <= 0) {
        Debug.WriteLine("Invalid Clipping Size", "LayoutElement.FitBoundRelativeRect");
        return;
      }

      // サンプル座標系での領域を求める
      var boundRect = this.GetBoundRect(sampleWidth, sampleHeight);
      int x, y, width, height;
      Imaging.Utilities.CalculateLayout(
          boundRect.X, boundRect.Y,
          boundRect.Width, boundRect.Height,
          this.ClippingWidthWithFit, this.ClippingHeightWithFit,
          this.Stretch, this.KeepAspectRatio,
          out x, out y, out width, out height);

      // 相対座標系に直す
      var nextRelativeLeft = (double)x / sampleWidth;
      var nextRelativeTop = (double)y / sampleHeight;
      var nextRelativeRight = (double)(x + width) / sampleWidth;
      var nextRelativeBottom = (double)(y + height) / sampleHeight;

      // 調整量を計算する
      var deltaHorizontal = sampleWidth *
          (Math.Abs(nextRelativeLeft - this.BoundRelativeLeft) +
           Math.Abs(nextRelativeRight - this.BoundRelativeRight));
      var deltaVertical = sampleHeight *
          (Math.Abs(nextRelativeTop - this.BoundRelativeTop) +
           Math.Abs(nextRelativeBottom - this.BoundRelativeBottom));

      // 相対座標系に戻してProfileに反映
      if (deltaHorizontal < 2.0 && deltaVertical < 2.0) {
        // ただし、調整量が左右方向に2ピクセル未満かつ上下方向に2ピクセル未満の場合は調整しない
        Debug.WriteLine("Deference is too small", "LayoutElement.FitBoundRelativeRect");
        return;
      }

      this.SetBoundRelativeLeft   = nextRelativeLeft;
      this.SetBoundRelativeRight  = nextRelativeRight;
      this.SetBoundRelativeTop    = nextRelativeTop;
      this.SetBoundRelativeBottom = nextRelativeBottom;
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

    /// @copydoc ILayoutElement::SetClippingRectByScreenRect
    public void SetClippingRectByScreenRect(ScreenRect nextScreenRect) {
      Debug.Assert(this.IsWindowValid, "Invalid Window", "LayoutElement.SetClippingRectByScreenRect");

      // ウィンドウの領域とIntersectをとる
      var boundScreenRect = Utilities.GetWindowScreenRect(this.WindowType, this.Window);
      if (!nextScreenRect.IntersectsWith(boundScreenRect)) {
        Debug.WriteLine("No Intersection", "LayoutElement.SetClippingRectByScreenRect");
        return;
      }
      /// @warning nextScreenRectが更新されるので注意
      nextScreenRect.Intersect(boundScreenRect);

      // プロパティを変更
      this.SetFit = false;
      switch (this.WindowType) {
        case WindowTypes.Normal:
        case WindowTypes.DesktopListView: {
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
        default : {
          Debug.Fail("Invalid WindowTypes", "LayoutElement.SetClippingRectByScreenRect");
          break;
        }
      }
      // Width/Heightは共通
      this.SetClippingWidthWithoutFit = nextScreenRect.Width;
      this.SetClippingHeightWithoutFit = nextScreenRect.Height;
    }

    //=================================================================
    // SampleWidth/Heightの値が必要
    //=================================================================

    /// @copydoc ILayoutElementView::GetBoundRect
    public SampleRect GetBoundRect(int sampleWidth, int sampleHeight) {
      return SampleRect.FromDouble(
        this.BoundRelativeLeft * sampleWidth,
        this.BoundRelativeTop * sampleHeight,
        this.BoundRelativeWidth * sampleWidth,
        this.BoundRelativeHeight * sampleHeight);
    }
    /// @copydoc ILayoutElementView::GetActualBoundRect
    public SampleRect GetActualBoundRect(int sampleWidth, int sampleHeight) {
      Debug.Assert(this.IsWindowValid, "Invalid Window", "LayoutElement.GetActualSampleRect");
      var boundRect = this.GetBoundRect(sampleWidth, sampleHeight);
      int x, y, width, height;
      Imaging.Utilities.CalculateLayout(
          boundRect.X, boundRect.Y,
          boundRect.Width, boundRect.Height,
          this.ClippingWidthWithFit, this.ClippingHeightWithFit,
          this.Stretch, this.KeepAspectRatio,
          out x, out y, out width, out height);
      /// @attention 切捨て・切り上げ計算をここでしている
      return new SampleRect(x, y, width, height);
    }

    //=================================================================
    // Validate
    //=================================================================

    /// @copydoc ILayoutElementView::ValidateClippingX
    public bool ValidateClippingX(int value, out int fixedX, out int fixedWidth) {
      Debug.Assert(this.IsWindowValid);

      var result = true;
      fixedX = value;
      fixedWidth = this.ClippingWidthWithoutFit;
      var windowRect = Utilities.GetWindowRect(this.WindowType, this.Window);

      // FitならValidateは常にtrue
      if (this.Fit) {
        fixedX = windowRect.X;
        fixedWidth = windowRect.Width;
        return true;
      }
 
      // 上限・下限の補正
      if (fixedX < windowRect.X) {
        // Xが左にはみでている場合、Widthは保持＆Xを右にずらす
        fixedX = windowRect.X;
        result = false;
      } else if (windowRect.Right < fixedX) {
        // Xが右にはみでている場合、Xを左にずらしてWidthを0に
        fixedX = windowRect.Right;
        fixedWidth = 0;
        result = false;
      }
      
      // Widthの補正
      if (fixedWidth < 0) {
        // Widthは0以上
        fixedWidth = 0;
        result = false;
      }
      if (windowRect.Right < fixedX + fixedWidth) {
        // 領域が右にはみでている場合、Xは保持＆Widthを縮める
        fixedWidth = windowRect.Right - fixedX;
        result = false;
      }
  
      // 出力
      Debug.Assert(windowRect.X <= fixedX && fixedX + fixedWidth <= windowRect.Right);
      return result;
    }
    /// @copydoc ILayoutElementView::ValidateClippingWidth
    public bool ValidateClippingWidth(int value, out int fixedX, out int fixedWidth) {
      Debug.Assert(this.IsWindowValid);

      var result = true;
      fixedX = this.ClippingXWithoutFit;
      fixedWidth = value;
      var windowRect = Utilities.GetWindowRect(this.WindowType, this.Window);

      // FitならValidateは常にtrue
      if (this.Fit) {
        fixedX = windowRect.X;
        fixedWidth = windowRect.Width;
        return true;
      }

      // 上限・下限の補正
      if (fixedWidth < 0) {
        // Widthは0以上
        fixedWidth = 0;
        result = false;
      } else if (windowRect.Width < fixedWidth) {
        // Widthが大きすぎる場合はFitさせる
        fixedX = windowRect.X;
        fixedWidth = windowRect.Width;
        result = false;
      }
 
      // Xの補正
      if (fixedX < windowRect.X) {
        // Xが左にはみでている場合、Widthは保持＆Xを右にずらす
        fixedX = windowRect.X;
        result = false;
      } else if (windowRect.Right < fixedX) {
        // Xが右にはみでている場合、Xを左にずらしてWidthを0に
        fixedX = windowRect.Right;
        fixedWidth = 0;
        result = false;
      }
      if (windowRect.Right < fixedX + fixedWidth) {
        // 領域が右にはみでている場合、Widthは保持＆Xを左にずらす
        fixedX = windowRect.Right - fixedWidth;
        result = false;
      }
  
      // 出力
      Debug.Assert(windowRect.X <= fixedX && fixedX + fixedWidth <= windowRect.Right);
      return result;
    }

    /// @copydoc ILayoutElementView::ValidateClippingY
    public bool ValidateClippingY(int value, out int fixedY, out int fixedHeight) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateClippingHeight
    public bool ValidateClippingHeight(int value, out int fixedY, out int fixedHeight) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateBoundRelativeLeft
    public bool ValidateBoundRelativeLeft(double value, out double fixedLeft, out double fixedRight) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateBoundRelativeRight
    public bool ValidateBoundRelativeRight(double value, out double fixedLeft, out double fixedRight) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateBoundRelativeTop
    public bool ValidateBoundRelativeTop(double value, out double fixedTop, out double fixedBottom) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateBoundRelativeBottom
    public bool ValidateBoundRelativeBottom(double value, out double fixedTop, out double fixedBottom) {
      throw new NotImplementedException();
    }

    /// @copydoc ILayoutElementView::ValidateSWScaleLumaGBlur
    public bool ValidateSWScaleLumaGBlur(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateSWScaleLumaSharpen
    public bool ValidateSWScaleLumaSharpen(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateSWScaleChromaHshift
    public bool ValidateSWScaleChromaHshift(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateSWScaleChromaGBlur
    public bool ValidateSWScaleChromaGBlur(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateSWScaleChromaSharpen
    public bool ValidateSWScaleChromaSharpen(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    /// @copydoc ILayoutElementView::ValidateSWScaleChromaVshift
    public bool ValidateSWScaleChromaVshift(double value, out double fixedValue) {
      throw new NotImplementedException();
    }
    
    //=================================================================
    // ToString
    //=================================================================

    /// @copydoc ILayoutElementView::GetHeaderString
    public string GetHeaderString(int maxLength) {
      var header = string.Format("Layout {0:D}: {1}",
                                 this.Index + 1,
                                 this.WindowCaption);
      return header.Substring(0, Math.Min(header.Length, maxLength));
    }

    /// @copydoc ILayoutElementView::GetHeaderStringForGUI
    public string GetHeaderStringForGUI(bool isCurrent, bool isDummy,
                                        int sampleWidth, int sampleHeight) {
      if (isCurrent && isDummy) {
        return string.Format(" [{0}] {1}", this.Index + 1, this.WindowCaption);
      } else if (isCurrent) {
        var boundRect = this.GetBoundRect(sampleWidth, sampleHeight);
        return string.Format(" [{0}] ({1}x{2}) {3}",
            this.Index + 1,
            boundRect.Width,
            boundRect.Height,
            this.WindowCaption);
      } else {
        return string.Format(" [{0}]", this.Index + 1);
      }
    }

    /// @copydoc ILayoutElementView::ClippingXString
    public string ClippingXString {
      get {
        if (!this.IsWindowValid && this.Fit) return "****";
        return this.ClippingXWithFit.ToString();
      }
    }
    /// @copydoc ILayoutElementView::ClippingYString
    public string ClippingYString {
      get {
        if (!this.IsWindowValid && this.Fit) return "****";
        return this.ClippingYWithFit.ToString();
      }
    }
    /// @copydoc ILayoutElementView::ClippingWidthString
    public string ClippingWidthString {
      get {
        if (!this.IsWindowValid && this.Fit) return "****";
        return this.ClippingWidthWithFit.ToString();
      }
    }
    /// @copydoc ILayoutElementView::ClippingHeightString
    public string ClippingHeightString {
      get {
        if (!this.IsWindowValid && this.Fit) return "****";
        return this.ClippingHeightWithFit.ToString();
      }
    }
    /// @copydoc ILayoutElementView::BoundRelativeLeftString
    public string BoundRelativeLeftString {
      get { return this.BoundRelativeLeft.ToString("F3"); }
    }
    /// @copydoc ILayoutElementView::BoundRelativeTopString
    public string BoundRelativeTopString {
      get { return this.BoundRelativeTop.ToString("F3"); }
    }
    /// @copydoc ILayoutElementView::BoundRelativeRightString
    public string BoundRelativeRightString {
      get { return this.BoundRelativeRight.ToString("F3"); }
    }
    /// @copydoc ILayoutElementView::BoundRelativeBottomString
    public string BoundRelativeBottomString {
      get { return this.BoundRelativeBottom.ToString("F3"); }
    }
    /// @copydoc ILayoutElementView::GetBoundLeftString
    public string GetBoundLeftString(bool isDummy, int sampleWidth) {
      var boundRect = this.GetBoundRect(sampleWidth, 100);  // dummy
      return isDummy ? string.Format("({0})", boundRect.X)
                     : boundRect.X.ToString();
    }
    /// @copydoc ILayoutElementView::GetBoundTopString
    public string GetBoundTopString(bool isDummy, int sampleHeight) {
      var boundRect = this.GetBoundRect(100, sampleHeight);  // dummy
      return isDummy ? string.Format("({0})", boundRect.Y)
                     : boundRect.Y.ToString();
    }
    /// @copydoc ILayoutElementView::GetBoundWidthString
    public string GetBoundWidthString(bool isDummy, int sampleWidth) {
      var boundRect = this.GetBoundRect(sampleWidth, 100);  // dummy
      return isDummy ? string.Format("({0})", boundRect.Width)
                     : boundRect.Width.ToString();
    }
    /// @copydoc ILayoutElementView::GetBoundHeightString
    public string GetBoundHeightString(bool isDummy, int sampleHeight) {
      var boundRect = this.GetBoundRect(100, sampleHeight);  // dummy
      return isDummy ? string.Format("({0})", boundRect.Height)
                     : boundRect.Height.ToString();
    }
    /// @copydoc ILayoutElementView::SWScaleLumaGBlurString
    public string SWScaleLumaGBlurString {
      get { return this.SWScaleLumaGBlur.ToString("F2"); }
    }
    /// @copydoc ILayoutElementView::SWScaleLumaSharpenString
    public string SWScaleLumaSharpenString {
      get { return this.SWScaleLumaSharpen.ToString("F2"); }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaHshiftString
    public string SWScaleChromaHshiftString {
      get { return this.SWScaleChromaHshift.ToString("F2"); }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaGBlurString
    public string SWScaleChromaGBlurString {
      get { return this.SWScaleChromaGBlur.ToString("F2"); }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaSharpenString
    public string SWScaleChromaSharpenString {
      get { return this.SWScaleChromaSharpen.ToString("F2"); }
    }
    /// @copydoc ILayoutElementView::SWScaleChromaVshiftString
    public string SWScaleChromaVshiftString {
      get { return this.SWScaleChromaVshift.ToString("F2"); }
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
