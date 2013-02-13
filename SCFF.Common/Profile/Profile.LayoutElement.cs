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

  private LayoutElementData Data {
    get { return this.profile.layoutElementDatabase[this.Index]; }
    set { this.profile.layoutElementDatabase[this.Index] = value; }
  }

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
    this.Data = new LayoutElementData();

    this.KeepAspectRatio = true;
    this.RotateDirection = RotateDirections.NoRotate;
    this.Stretch = true;
    this.SWScaleFlags = SWScaleFlags.Area;
    this.SWScaleIsFilterEnabled = false;

    // プライマリモニタを表示
    this.SetWindowToDesktop();
    this.Fit = false;
    this.ClippingXWithoutFit       = 0;
    this.ClippingYWithoutFit       = 0;
    this.ClippingWidthWithoutFit   = User32.GetSystemMetrics(User32.SM_CXSCREEN);
    this.ClippingHeightWithoutFit  = User32.GetSystemMetrics(User32.SM_CYSCREEN);
    
    // 初期値を少しずつずらす
    this.BoundRelativeLeft =
        Constants.MinimumBoundRelativeSize * this.Index;
    this.BoundRelativeTop =
        Constants.MinimumBoundRelativeSize * this.Index;
    this.BoundRelativeRight = 1.0;
    this.BoundRelativeBottom = 1.0;
  }

  //=================================================================
  // TargetWindow
  //=================================================================

  /// @copydoc ILayoutElementView::Window
  public UIntPtr Window {
    get { return this.Data.Window; }
  }

  /// @copydoc ILayoutElementView::IsWindowValid
  public bool IsWindowValid {
    get { return Common.Utilities.IsWindowValid(this.Window); }
  }

  /// @copydoc ILayoutElementView::WindowType
  public WindowTypes WindowType {
    get { return this.Data.WindowType; }
  }

  /// @copydoc ILayoutElementView::WindowCaption
  public string WindowCaption {
    get { return this.Data.WindowCaption; }
  }

  /// @copydoc ILayoutElement::SetWindow
  public void SetWindow(UIntPtr window) {
    this.Data.WindowType = WindowTypes.Normal;
    this.Data.Window = window;

    var windowCaption = "n/a";
    if (Common.Utilities.IsWindowValid(window)) {
      StringBuilder className = new StringBuilder(256);
      User32.GetClassName(window, className, 256);
      windowCaption = className.ToString();
    }
    this.Data.WindowCaption = windowCaption;
  }
  /// @copydoc ILayoutElement::SetWindowToDesktop
  public void SetWindowToDesktop() {
    this.Data.WindowType = WindowTypes.Desktop;
    this.Data.Window = User32.GetDesktopWindow();
    this.Data.WindowCaption = "(Desktop)";
  }
  /// @copydoc ILayoutElement::SetWindowToDesktopListView
  public void SetWindowToDesktopListView() {
    this.Data.WindowType = WindowTypes.DesktopListView;
    this.Data.Window = Utilities.DesktopListViewWindow;
    this.Data.WindowCaption = "(DesktopListView)";
  }

  //=================================================================
  // Area
  //=================================================================

  /// @copydoc ILayoutElementView::Fit
  public bool Fit {
    get { return this.Data.Fit; }
    set { this.Data.Fit = value; }
  }
  /// @copydoc ILayoutElementView::ClippingXWithoutFit
  public int ClippingXWithoutFit {
    get { return this.Data.ClippingXWithoutFit; }
    set { this.Data.ClippingXWithoutFit = value; }
  }
  /// @copydoc ILayoutElementView::ClippingYWithoutFit
  public int ClippingYWithoutFit {
    get { return this.Data.ClippingYWithoutFit; }
    set { this.Data.ClippingYWithoutFit = value; }
  }
  /// @copydoc ILayoutElementView::ClippingWidthWithoutFit
  public int ClippingWidthWithoutFit {
    get { return this.Data.ClippingWidthWithoutFit; }
    set { this.Data.ClippingWidthWithoutFit = value; }
  }
  /// @copydoc ILayoutElementView::ClippingHeightWithoutFit
  public int ClippingHeightWithoutFit {
    get { return this.Data.ClippingHeightWithoutFit; }
    set { this.Data.ClippingHeightWithoutFit = value; }
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

  //=================================================================
  // Options
  //=================================================================

  /// @copydoc ILayoutElementView::ShowCursor
  public bool ShowCursor {
    get { return this.Data.ShowCursor; }
    set { this.Data.ShowCursor = value; }
  }
  /// @copydoc ILayoutElementView::ShowLayeredWindow
  public bool ShowLayeredWindow {
    get { return this.Data.ShowLayeredWindow; }
    set { this.Data.ShowLayeredWindow = value; }
  }
  /// @copydoc ILayoutElementView::Stretch
  public bool Stretch {
    get { return this.Data.Stretch; }
    set { this.Data.Stretch = value; }
  }
  /// @copydoc ILayoutElementView::KeepAspectRatio
  public bool KeepAspectRatio {
    get { return this.Data.KeepAspectRatio; }
    set { this.Data.KeepAspectRatio = value; }
  }
  /// @copydoc ILayoutElementView::RotateDirection
  public RotateDirections RotateDirection {
    get { return this.Data.RotateDirection; }
    set { this.Data.RotateDirection = value; }
  }

  //=================================================================
  // ResizeMethod
  //=================================================================

  /// @copydoc ILayoutElementView::SWScaleFlags
  public SWScaleFlags SWScaleFlags {
    get { return this.Data.SWScaleConfig.Flags; }
    set { this.Data.SWScaleConfig.Flags = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleAccurateRnd
  public bool SWScaleAccurateRnd {
    get { return this.Data.SWScaleConfig.AccurateRnd; }
    set { this.Data.SWScaleConfig.AccurateRnd = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleIsFilterEnabled
  public bool SWScaleIsFilterEnabled {
    get { return this.Data.SWScaleConfig.IsFilterEnabled; }
    set { this.Data.SWScaleConfig.IsFilterEnabled = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleLumaGBlur
  public float SWScaleLumaGBlur {
    get { return this.Data.SWScaleConfig.LumaGblur; }
    set { this.Data.SWScaleConfig.LumaGblur = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleChromaGBlur
  public float SWScaleChromaGBlur {
    get { return this.Data.SWScaleConfig.ChromaGblur; }
    set { this.Data.SWScaleConfig.ChromaGblur = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleLumaSharpen
  public float SWScaleLumaSharpen {
    get { return this.Data.SWScaleConfig.LumaSharpen; }
    set { this.Data.SWScaleConfig.LumaSharpen = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleChromaSharpen
  public float SWScaleChromaSharpen {
    get { return this.Data.SWScaleConfig.ChromaSharpen; }
    set { this.Data.SWScaleConfig.ChromaSharpen = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleChromaHShift
  public float SWScaleChromaHShift {
    get { return this.Data.SWScaleConfig.ChromaHShift; }
    set { this.Data.SWScaleConfig.ChromaHShift = value; }
  }
  /// @copydoc ILayoutElementView::SWScaleChromaVShift
  public float SWScaleChromaVShift {
    get { return this.Data.SWScaleConfig.ChromaVShift; }
    set { this.Data.SWScaleConfig.ChromaVShift = value; }
  }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc ILayoutElementView::BoundRelativeLeft
  public double BoundRelativeLeft {
    get { return this.Data.BoundRelativeLeft; }
    set { this.Data.BoundRelativeLeft = value; }
  }
  /// @copydoc ILayoutElementView::BoundRelativeTop
  public double BoundRelativeTop {
    get { return this.Data.BoundRelativeTop; }
    set { this.Data.BoundRelativeTop = value; }
  }
  /// @copydoc ILayoutElementView::BoundRelativeRight
  public double BoundRelativeRight {
    get { return this.Data.BoundRelativeRight; }
    set { this.Data.BoundRelativeRight = value; }
  }
  /// @copydoc ILayoutElementView::BoundRelativeBottom
  public double BoundRelativeBottom {
    get { return this.Data.BoundRelativeBottom; }
    set { this.Data.BoundRelativeBottom = value; }
  }
  /// @copydoc ILayoutElementView::BoundRelativeWidth
  public double BoundRelativeWidth {
    get { return this.BoundRelativeRight - this.BoundRelativeLeft; }
  }
  /// @copydoc ILayoutElementView::BoundRelativeHeight
  public double BoundRelativeHeight {
    get { return this.BoundRelativeBottom - this.BoundRelativeTop; }
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
    this.BoundRelativeLeft   = actualBoundRect.X * relativeXPerPixel;
    this.BoundRelativeTop    = actualBoundRect.Y * relativeYPerPixel;
    this.BoundRelativeRight  = actualBoundRect.Right * relativeXPerPixel;
    this.BoundRelativeBottom = actualBoundRect.Bottom * relativeYPerPixel;
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
    this.Fit = false;
    switch (this.WindowType) {
      case WindowTypes.Normal:
      case WindowTypes.DesktopListView: {
        // Screen->Client座標系変換
        this.ClippingXWithoutFit = nextScreenRect.X - boundScreenRect.X;
        this.ClippingYWithoutFit = nextScreenRect.Y - boundScreenRect.Y;
        break;
      }
      case WindowTypes.Desktop: {
        // 変換の必要なし
        this.ClippingXWithoutFit = nextScreenRect.X;
        this.ClippingYWithoutFit = nextScreenRect.Y;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
    // Width/Heightは共通
    this.ClippingWidthWithoutFit = nextScreenRect.Width;
    this.ClippingHeightWithoutFit = nextScreenRect.Height;
  }

  //=================================================================
  // SampleWidth/Heightの値が必要
  //=================================================================

  /// @copydoc ILayoutElementView::GetBoundRect
  public SampleRect GetBoundRect(int sampleWidth, int sampleHeight) {
    var realLeft = this.BoundRelativeLeft * sampleWidth;
    var realTop = this.BoundRelativeTop * sampleHeight;
    var realRight = this.BoundRelativeRight * sampleWidth;
    var realBottom = this.BoundRelativeBottom * sampleHeight;

    /// @attention 切捨て・切り上げ計算をここでしている
    // floor
    var x = (int)(realLeft + Imaging.Utilities.Epsilon);
    var y = (int)(realTop + Imaging.Utilities.Epsilon);
    var right = (int)(realRight + Imaging.Utilities.Epsilon);
    var bottom = (int)(realBottom + Imaging.Utilities.Epsilon);

    var width = right - x;
    var height = bottom - y;

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
    get { return this.Data.HasBackedUp; }
    set { this.Data.HasBackedUp = value; }
  }
  /// @copydoc ILayoutElementView::BackupScreenClippingX
  public int BackupScreenClippingX {
    get { return this.Data.BackupScreenClippingX; }
    set { this.Data.BackupScreenClippingX = value; }
  }
  /// @copydoc ILayoutElementView::BackupScreenClippingY
  public int BackupScreenClippingY {
    get { return this.Data.BackupScreenClippingY; }
    set { this.Data.BackupScreenClippingY = value; }
  }
  /// @copydoc ILayoutElementView::BackupClippingWidth
  public int BackupClippingWidth {
    get { return this.Data.BackupClippingWidth; }
    set { this.Data.BackupClippingWidth = value; }
  }
  /// @copydoc ILayoutElementView::BackupClippingHeight
  public int BackupClippingHeight {
    get { return this.Data.BackupClippingHeight; }
    set { this.Data.BackupClippingHeight = value; }
  }
  /// @copydoc ILayoutElement::UpdateBackupParameters
  public void UpdateBackupParameters() {
    var screenRect = this.ScreenClippingRectWithFit;
    this.BackupScreenClippingX = screenRect.X;
    this.BackupScreenClippingY = screenRect.Y;
    this.BackupClippingWidth = screenRect.Width;
    this.BackupClippingHeight = screenRect.Height;
    this.HasBackedUp = true;
  }
  /// @copydoc ILayoutElement::UpdateBackupParameters
  public void RestoreBackupParameters() {
    if (!this.HasBackedUp) return;
    Debug.Assert(!this.IsWindowValid);
    this.SetWindowToDesktop();
    this.Fit = false;
    this.ClippingXWithoutFit = this.BackupScreenClippingX;
    this.ClippingYWithoutFit = this.BackupScreenClippingY;
    this.ClippingWidthWithoutFit = this.BackupClippingWidth;
    this.ClippingHeightWithoutFit = this.BackupClippingHeight;
  }
  /// @copydoc ILayoutElement::ClearBackupParameters
  public void ClearBackupParameters() {
    this.BackupScreenClippingX = 0;
    this.BackupScreenClippingY = 0;
    this.BackupClippingWidth = 1;
    this.BackupClippingHeight = 1;
    this.HasBackedUp = false;
  }

  //=================================================================
  // フィールド
  //=================================================================

  /// 対象プロファイル
  private readonly Profile profile;
}
}   // (outerclass) SCFF.Common.Profile.Profile
}   // namespace SCFF.Common.Profile
