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

/// @file SCFF.Common/Profile/LayoutElement.cs
/// @copydoc SCFF::Common::Profile::LayoutElement

namespace SCFF.Common.Profile {

using System;
using System.Diagnostics;
using System.Text;
using SCFF.Common.Ext;
using SCFF.Interprocess;

/// プロファイル内を参照・操作するためのカーソルクラス
/// 
/// - C#のインナークラスはC++のフレンドクラスと似たようなことができる！
/// - プログラムから直接は利用してはいけないもの(this.profile.ToMessage()で上書きされるため)
///   - this.profile.layoutElementDatabase[*].Bound*
///   - this.profile.layoutElementDatabase[*].Clipping*
/// - 以下の内容も最新のデータがあることは保障しない
///   - this.profile.layoutElementDatabase[*].Window
/// - ProfileはProcessに関連した情報を知ることはできない
///   - よってsampleWidth/sampleHeightの存在は仮定しないこと
public class LayoutElement {
  //=================================================================
  // コンストラクタ
  //=================================================================

  /// コンストラクタ
  /// @param index インデックス
  public LayoutElement(int index) {
    // newで参照型をゼロクリア
    this.rawData = new InternalLayoutParameter();
    this.additionalData = new AdditionalLayoutParameter();

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
        Constants.MinimumBoundRelativeSize * index;
    this.BoundRelativeTop =
        Constants.MinimumBoundRelativeSize * index;
    this.BoundRelativeRight = 1.0;
    this.BoundRelativeBottom = 1.0;
  }

  //=================================================================
  // 変換
  //=================================================================

  /// 共有メモリに書き込める形に変換
  public LayoutParameter ToLayoutParameter(int sampleWidth, int sampleHeight) {
    // Bound*とClipping*以外のデータをコピー
    var result = this.rawData.ToLayoutParameter();

    // Bound*
    var boundRect = this.GetBoundRect(sampleWidth, sampleHeight);
    result.BoundX = boundRect.X;
    result.BoundY = boundRect.Y;
    result.BoundWidth = boundRect.Width;
    result.BoundHeight = boundRect.Height;

    // Clipping*
    result.ClippingX = this.ClippingXWithFit;
    result.ClippingY = this.ClippingYWithFit;
    result.ClippingWidth = this.ClippingWidthWithFit;
    result.ClippingHeight = this.ClippingHeightWithFit;

    return result;
  }

  //=================================================================
  // TargetWindow
  //=================================================================

  /// @copydoc SCFF::Common::InternalLayoutParameter::Window
  public UIntPtr Window {
    get { return this.rawData.Window; }
  }

  /// 正常なWindowハンドルかどうか
  public bool IsWindowValid {
    get { return Common.Utilities.IsWindowValid(this.Window); }
  }

  /// @copydoc AdditionalLayoutParameter::WindowType
  public WindowTypes WindowType {
    get { return this.additionalData.WindowType; }
  }

  /// @copydoc AdditionalLayoutParameter::WindowCaption
  public string WindowCaption {
    get { return this.additionalData.WindowCaption; }
  }

  /// WindowタイプをNormalに＋Windowハンドルを設定する
  /// @param window 設定するWindowハンドル
  public void SetWindow(UIntPtr window) {
    this.additionalData.WindowType = WindowTypes.Normal;
    this.rawData.Window = window;

    var windowCaption = "n/a";
    if (Common.Utilities.IsWindowValid(window)) {
      StringBuilder className = new StringBuilder(256);
      User32.GetClassName(window, className, 256);
      windowCaption = className.ToString();
    }
    this.additionalData.WindowCaption = windowCaption;
  }
  /// WindowタイプをDesktopにする
  public void SetWindowToDesktop() {
    this.additionalData.WindowType = WindowTypes.Desktop;
    this.rawData.Window = User32.GetDesktopWindow();
    this.additionalData.WindowCaption = "(Desktop)";
  }
  /// WindowタイプをDXGIにする
  public void SetWindowToDXGI() {
    this.additionalData.WindowType = WindowTypes.DXGI;
    this.rawData.Window = Utilities.DXGIWindow;
    this.additionalData.WindowCaption = "(DXGI)";
  }

  //=================================================================
  // Area
  //=================================================================

  /// @copydoc AdditionalLayoutParameter::Fit
  public bool Fit {
    get { return this.additionalData.Fit; }
    set { this.additionalData.Fit = value; }
  }
  /// @copydoc AdditionalLayoutParameter::ClippingXWithoutFit
  public int ClippingXWithoutFit {
    get { return this.additionalData.ClippingXWithoutFit; }
    set { this.additionalData.ClippingXWithoutFit = value; }
  }
  /// @copydoc AdditionalLayoutParameter::ClippingYWithoutFit
  public int ClippingYWithoutFit {
    get { return this.additionalData.ClippingYWithoutFit; }
    set { this.additionalData.ClippingYWithoutFit = value; }
  }
  /// @copydoc AdditionalLayoutParameter::ClippingWidthWithoutFit
  public int ClippingWidthWithoutFit {
    get { return this.additionalData.ClippingWidthWithoutFit; }
    set { this.additionalData.ClippingWidthWithoutFit = value; }
  }
  /// @copydoc AdditionalLayoutParameter::ClippingHeightWithoutFit
  public int ClippingHeightWithoutFit {
    get { return this.additionalData.ClippingHeightWithoutFit; }
    set { this.additionalData.ClippingHeightWithoutFit = value; }
  }

  /// Fitオプションを考慮したクリッピング領域左上端のX座標
  /// @pre IsWindowValid == True
  public int ClippingXWithFit {
    get {
      return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).X
                      : this.ClippingXWithoutFit;
    }
  }
  /// Fitオプションを考慮したクリッピング領域左上端のY座標
  /// @pre IsWindowValid == True
  public int ClippingYWithFit {
    get {
      return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Y
                      : this.ClippingYWithoutFit;
    }
  }
  /// Fitオプションを考慮したクリッピング領域の幅
  /// @pre IsWindowValid == True
  public int ClippingWidthWithFit {
    get {
      return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Width
                      : this.ClippingWidthWithoutFit;
    }
  }
  /// Fitオプションを考慮したクリッピング領域の高さ
  /// @pre IsWindowValid == True
  public int ClippingHeightWithFit {
    get {
      return this.Fit ? Utilities.GetWindowRect(this.WindowType, this.Window).Height
                      : this.ClippingHeightWithoutFit;
    }
  }

  /// クリッピング領域が正常か
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

  /// @copydoc SCFF::Common::InternalLayoutParameter::ShowCursor
  public bool ShowCursor {
    get { return this.rawData.ShowCursor; }
    set { this.rawData.ShowCursor = value; }
  }
  /// @copydoc SCFF::Common::InternalLayoutParameter::ShowLayeredWindow
  public bool ShowLayeredWindow {
    get { return this.rawData.ShowLayeredWindow; }
    set { this.rawData.ShowLayeredWindow = value; }
  }
  /// @copydoc SCFF::Common::InternalLayoutParameter::Stretch
  public bool Stretch {
    get { return this.rawData.Stretch; }
    set { this.rawData.Stretch = value; }
  }
  /// @copydoc SCFF::Common::InternalLayoutParameter::KeepAspectRatio
  public bool KeepAspectRatio {
    get { return this.rawData.KeepAspectRatio; }
    set { this.rawData.KeepAspectRatio = value; }
  }
  /// @copydoc SCFF::Common::InternalLayoutParameter::RotateDirection
  public RotateDirections RotateDirection {
    get { return this.rawData.RotateDirection; }
    set { this.rawData.RotateDirection = value; }
  }

  //=================================================================
  // ResizeMethod
  //=================================================================

  /// @copydoc SCFF::Common::InternalSWScaleConfig::Flags
  public SWScaleFlags SWScaleFlags {
    get { return this.rawData.SWScaleConfig.Flags; }
    set { this.rawData.SWScaleConfig.Flags = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::AccurateRnd
  public bool SWScaleAccurateRnd {
    get { return this.rawData.SWScaleConfig.AccurateRnd; }
    set { this.rawData.SWScaleConfig.AccurateRnd = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::IsFilterEnabled
  public bool SWScaleIsFilterEnabled {
    get { return this.rawData.SWScaleConfig.IsFilterEnabled; }
    set { this.rawData.SWScaleConfig.IsFilterEnabled = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::LumaGBlur
  public float SWScaleLumaGBlur {
    get { return this.rawData.SWScaleConfig.LumaGblur; }
    set { this.rawData.SWScaleConfig.LumaGblur = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::ChromaGBlur
  public float SWScaleChromaGBlur {
    get { return this.rawData.SWScaleConfig.ChromaGblur; }
    set { this.rawData.SWScaleConfig.ChromaGblur = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::LumaSharpen
  public float SWScaleLumaSharpen {
    get { return this.rawData.SWScaleConfig.LumaSharpen; }
    set { this.rawData.SWScaleConfig.LumaSharpen = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::ChromaSharpen
  public float SWScaleChromaSharpen {
    get { return this.rawData.SWScaleConfig.ChromaSharpen; }
    set { this.rawData.SWScaleConfig.ChromaSharpen = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::ChromaHShift
  public float SWScaleChromaHShift {
    get { return this.rawData.SWScaleConfig.ChromaHShift; }
    set { this.rawData.SWScaleConfig.ChromaHShift = value; }
  }
  /// @copydoc SCFF::Common::InternalSWScaleConfig::ChromaVShift
  public float SWScaleChromaVShift {
    get { return this.rawData.SWScaleConfig.ChromaVShift; }
    set { this.rawData.SWScaleConfig.ChromaVShift = value; }
  }

  //=================================================================
  // LayoutParameter
  //=================================================================

  /// @copydoc AdditionalLayoutParameter::BoundRelativeLeft
  public double BoundRelativeLeft {
    get { return this.additionalData.BoundRelativeLeft; }
    set { this.additionalData.BoundRelativeLeft = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BoundRelativeTop
  public double BoundRelativeTop {
    get { return this.additionalData.BoundRelativeTop; }
    set { this.additionalData.BoundRelativeTop = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BoundRelativeRight
  public double BoundRelativeRight {
    get { return this.additionalData.BoundRelativeRight; }
    set { this.additionalData.BoundRelativeRight = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BoundRelativeBottom
  public double BoundRelativeBottom {
    get { return this.additionalData.BoundRelativeBottom; }
    set { this.additionalData.BoundRelativeBottom = value; }
  }
  /// 相対座標系でのレイアウト要素の幅
  public double BoundRelativeWidth {
    get { return this.BoundRelativeRight - this.BoundRelativeLeft; }
  }
  /// 相対座標系でのレイアウト要素の高さ
  public double BoundRelativeHeight {
    get { return this.BoundRelativeBottom - this.BoundRelativeTop; }
  }

  /// LayoutParameterをサンプル座標系で調整する
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
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

  /// Screen座標系で表されたクリッピング領域(Fit考慮)を取得する
  public ScreenRect ScreenClippingRectWithFit {
    get {
      var screenPoint = Utilities.ClientToScreen(
          this.WindowType, this.Window, this.ClippingXWithFit, this.ClippingYWithFit);
      return new ScreenRect(screenPoint.X, screenPoint.Y,
          this.ClippingWidthWithFit, this.ClippingHeightWithFit);
    }
  }

  /// Screen座標系のRectとIntersectを取り、新しいClipping領域に設定する。
  /// @param nextScreenRect 新しいClipping領域(Intersect前)
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
      case WindowTypes.DXGI: {
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

  /// サンプル座標系でのレイアウト要素の領域
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return サンプル座標系でのレイアウト要素の領域
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
  /// サンプル座標系でのレイアウト要素の画像部分が占める領域
  /// @param sampleWidth サンプルの幅
  /// @param sampleHeight サンプルの高さ
  /// @return サンプル座標系でのレイアウト要素の画像部分が占める領域
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

  /// @copydoc AdditionalLayoutParameter::HasBackedUp
  public bool HasBackedUp {
    get { return this.additionalData.HasBackedUp; }
    set { this.additionalData.HasBackedUp = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BackupScreenClippingX
  public int BackupScreenClippingX {
    get { return this.additionalData.BackupScreenClippingX; }
    set { this.additionalData.BackupScreenClippingX = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BackupScreenClippingY
  public int BackupScreenClippingY {
    get { return this.additionalData.BackupScreenClippingY; }
    set { this.additionalData.BackupScreenClippingY = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BackupClippingWidth
  public int BackupClippingWidth {
    get { return this.additionalData.BackupClippingWidth; }
    set { this.additionalData.BackupClippingWidth = value; }
  }
  /// @copydoc AdditionalLayoutParameter::BackupClippingHeight
  public int BackupClippingHeight {
    get { return this.additionalData.BackupClippingHeight; }
    set { this.additionalData.BackupClippingHeight = value; }
  }
  /// Backup*をすべて更新
  public void UpdateBackupParameters() {
    var screenRect = this.ScreenClippingRectWithFit;
    this.BackupScreenClippingX = screenRect.X;
    this.BackupScreenClippingY = screenRect.Y;
    this.BackupClippingWidth = screenRect.Width;
    this.BackupClippingHeight = screenRect.Height;
    this.HasBackedUp = true;
  }
  /// BackupからWindow/Clipping*を設定
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
  /// Backup*をクリア
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

  /// レイアウトパラメータ
  private InternalLayoutParameter rawData =
      new InternalLayoutParameter();

  /// レイアウトパラメータ追加分
  private AdditionalLayoutParameter additionalData =
      new AdditionalLayoutParameter();
}
}   // namespace SCFF.Common.Profile
