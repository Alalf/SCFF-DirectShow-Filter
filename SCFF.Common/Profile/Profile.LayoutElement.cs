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

/// @file SCFF.Common/Profile/Profile.LayoutElement.cs
/// @copydoc SCFF::Common::Profile::Profile::LayoutElement

namespace SCFF.Common.Profile {

using System;
using System.Diagnostics;
using System.Text;
using SCFF.Common.Ext;
using SCFF.Interprocess;

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
    this.profile.RaiseChanged();
  }

  /// @copydoc ILayoutElement::RestoreDefault
  /// @todo(me) インスタンスを生成する形でゼロクリアしているが非効率的？
  public void RestoreDefault() {
    // newで参照型をゼロクリア
    var layoutParameter = new LayoutParameter();
    layoutParameter.SWScaleConfig = new SWScaleConfig();
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
    get { return (UIntPtr)this.profile.message.LayoutParameters[this.Index].Window; }
  }

  /// @copydoc ILayoutElementView::IsWindowValid
  public bool IsWindowValid {
    get { return (this.Window != UIntPtr.Zero && User32.IsWindow(this.Window)); }
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
    if (!this.IsWindowValid) {
      Debug.WriteLine("Invalid Window", "FixClippingWithoutFit");
      return;
    }
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

    var windowCaption = "n/a";
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
    this.profile.message.LayoutParameters[this.Index].Window = User32.GetDesktopWindow().ToUInt64();
    this.profile.additionalLayoutParameters[this.Index].WindowCaption = "(Desktop)";

    if (windowChanged) {
      this.FixClippingWithoutFit();
    }
  }
  /// @copydoc ILayoutElement::SetWindowToDesktopListView
  public void SetWindowToDesktopListView() {
    var windowChanged = this.WindowType != WindowTypes.DesktopListView;

    this.profile.additionalLayoutParameters[this.Index].WindowType = WindowTypes.DesktopListView;
    this.profile.message.LayoutParameters[this.Index].Window = Utilities.DesktopListViewWindow.ToUInt64();
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

  /// @copydoc ILayoutElementView::IsClippingParametersValid
  public bool IsClippingParametersValid {
    get {
      /// @todo(me) まだValidateのチェックが甘い。
      Debug.Assert(this.IsWindowValid);
      if (this.Fit) return true;
      var windowRect = Utilities.GetWindowRect(this.WindowType, this.Window);
      var clippingRect = new ClientRect(this.ClippingXWithoutFit,
                                        this.ClippingYWithoutFit,
                                        this.ClippingWidthWithoutFit,
                                        this.ClippingHeightWithoutFit);
      if (windowRect.Width <= 0 || windowRect.Height <= 0 ||
          clippingRect.Width <= 0 || clippingRect.Width <= 0) {
        return false;
      }
      return windowRect.Contains(clippingRect);
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
  /// @copydoc ILayoutElementView::SWScaleChromaHShift
  public float SWScaleChromaHShift {
    get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHShift; }
  }
  /// @copydoc ILayoutElementView::SWScaleChromaVShift
  public float SWScaleChromaVShift {
    get { return this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVShift; }
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
  /// @copydoc ILayoutElement::SetSWScaleChromaHShift
  public float SetSWScaleChromaHShift {
    set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaHShift = value; }
  }
  /// @copydoc ILayoutElement::SetSWScaleChromaVShift
  public float SetSWScaleChromaVShift {
    set { this.profile.message.LayoutParameters[this.Index].SWScaleConfig.ChromaVShift = value; }
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
    var actualBoundRect = this.GetActualBoundRect(sampleWidth, sampleHeight);

    // 1ピクセルあたりの相対値を求める
    var relativeXPerPixel = 1.0 / sampleWidth;
    var relativeYPerPixel = 1.0 / sampleHeight;

    // Profileに反映
    this.SetBoundRelativeLeft   = actualBoundRect.X * relativeXPerPixel;
    this.SetBoundRelativeTop    = actualBoundRect.Y * relativeYPerPixel;
    this.SetBoundRelativeRight  = actualBoundRect.Right * relativeXPerPixel;
    this.SetBoundRelativeBottom = actualBoundRect.Bottom * relativeYPerPixel;
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

    // Screen座標系でのウィンドウのクライアント領域とIntersectをとる
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
      default: Debug.Fail("switch"); throw new System.ArgumentException();
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
    var realX = this.BoundRelativeLeft * sampleWidth;
    var realY = this.BoundRelativeTop * sampleHeight;
    var realWidth = this.BoundRelativeWidth * sampleWidth;
    var realHeight = this.BoundRelativeHeight * sampleHeight;

    /// @attention 切捨て・切り上げ計算をここでしている
    // floor
    var x = (int)(realX + Imaging.Utilities.Epsilon);
    var y = (int)(realY + Imaging.Utilities.Epsilon);
    // ceil
    var width = (int)Math.Ceiling(realWidth - Imaging.Utilities.Epsilon);
    var height = (int)Math.Ceiling(realHeight - Imaging.Utilities.Epsilon);

    return new SampleRect(x, y, width, height);
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
    return new SampleRect(x, y, width, height);
  }

  //=================================================================
  // Backup
  //=================================================================

  /// @copydoc ILayoutElementView::HasBackedUp
  public bool HasBackedUp {
    get { return this.profile.additionalLayoutParameters[this.Index].HasBackedUp; }
  }
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
    this.SetBackupScreenClippingX = screenRect.X;
    this.SetBackupScreenClippingY = screenRect.Y;
    this.SetBackupClippingWidth = screenRect.Width;
    this.SetBackupClippingHeight = screenRect.Height;
    this.SetHasBackedUp = true;
  }
  /// @copydoc ILayoutElement::UpdateBackupParameters
  public void RestoreBackupParameters() {
    if (!this.HasBackedUp) return;
    Debug.Assert(!this.IsWindowValid);
    this.SetWindowToDesktop();
    this.SetFit = false;
    this.SetClippingXWithoutFit = this.BackupScreenClippingX;
    this.SetClippingYWithoutFit = this.BackupScreenClippingY;
    this.SetClippingWidthWithoutFit = this.BackupClippingWidth;
    this.SetClippingHeightWithoutFit = this.BackupClippingHeight;
  }
  /// @copydoc ILayoutElement::ClearBackupParameters
  public void ClearBackupParameters() {
    this.SetBackupScreenClippingX = 0;
    this.SetBackupScreenClippingY = 0;
    this.SetBackupClippingWidth = 1;
    this.SetBackupClippingHeight = 1;
    this.SetHasBackedUp = false;
  }
  /// @copydoc ILayoutElement::SetHasBackedUp
  public bool SetHasBackedUp {
    set { this.profile.additionalLayoutParameters[this.Index].HasBackedUp = value; }
  }
  /// @copydoc ILayoutElement::SetBackupScreenClippingX
  public int SetBackupScreenClippingX {
    set { this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingX = value; }
  }
  /// @copydoc ILayoutElement::SetBackupScreenClippingY
  public int SetBackupScreenClippingY {
    set { this.profile.additionalLayoutParameters[this.Index].BackupScreenClippingY = value; }
  }
  /// @copydoc ILayoutElement::SetBackupClippingWidth
  public int SetBackupClippingWidth {
    set { this.profile.additionalLayoutParameters[this.Index].BackupClippingWidth = value; }
  }
  /// @copydoc ILayoutElement::SetBackupClippingHeight
  public int SetBackupClippingHeight {
    set { this.profile.additionalLayoutParameters[this.Index].BackupClippingHeight = value; }
  }
}
}   // (outerclass) SCFF.Common.Profile.Profile
}   // namespace SCFF.Common.Profile
