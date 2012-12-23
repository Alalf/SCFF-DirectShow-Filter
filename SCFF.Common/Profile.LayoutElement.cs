// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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
/// プロファイル内を参照・操作するためのカーソルクラス

namespace SCFF.Common {

using System;
using System.Diagnostics;
using System.Text;

public partial class Profile {
  /// プロファイル内を参照・操作するためのカーソルクラス
  /// 
  /// - C#のインナークラスはC++のフレンドクラスと似たようなことができる！
  /// - プログラムから直接は利用してはいけないもの(this.profile.appendicesの内容で上書きされるため)
  ///   - this.profile.message.layoutParameters[*].Bound*
  ///   - this.profile.message.layoutParameters[*].Clipping*
  /// - 以下の内容も最新のデータがあることは保障しない
  ///   - this.profile.message.layoutParameters[*].Window
  ///
  /// - ProfileはProcessに関連した情報を知ることはできない
  ///   - よってsampleWidth/sampleHeightの存在は仮定しないこと
  /// - 相対比率→ピクセル値変換
  ///   - Left/Topは切り捨て、Right/Bottomは切り上げ
  public class LayoutElement {

    // コンストラクタ
    public LayoutElement(Profile profile, int index) {
      this.profile = profile;
      this.index = index;
    }

    public int Index {
      get { return this.index; }
    }

    //=================================================================
    // アクセッサ(単純なもの)
    //=================================================================

    // Show *
    public bool ShowCursor {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].ShowCursor); }
      set { this.profile.message.LayoutParameters[this.index].ShowCursor = Convert.ToByte(value); }
    }
    public bool ShowLayeredWindow {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].ShowLayeredWindow); }
      set { this.profile.message.LayoutParameters[this.index].ShowLayeredWindow = Convert.ToByte(value); }
    }

    // SWScale *
    public SWScaleFlags SWScaleFlags {
      get { return (SWScaleFlags)this.profile.message.LayoutParameters[this.index].SWScaleConfig.Flags; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.Flags = Convert.ToInt32(value); }
    }
    public bool SWScaleAccurateRnd {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].SWScaleConfig.AccurateRnd); }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.AccurateRnd = Convert.ToByte(value); }
    }
    public bool SWScaleIsFilterEnabled {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].SWScaleConfig.IsFilterEnabled); }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.IsFilterEnabled = Convert.ToByte(value); }
    }
    public float SWScaleLumaGBlur {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaGblur; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaGblur = value; }
    }
    public float SWScaleChromaGBlur {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaGblur; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaGblur = value; }
    }
    public float SWScaleLumaSharpen {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaSharpen; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.LumaSharpen = value; }
    }
    public float SWScaleChromaSharpen {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaSharpen; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaSharpen = value; }
    }
    public float SWScaleChromaHshift {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaHshift; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaHshift = value; }
    }
    public float SWScaleChromaVshift {
      get { return this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaVshift; }
      set { this.profile.message.LayoutParameters[this.index].SWScaleConfig.ChromaVshift = value; }
    }

    // Other Options
    public bool Stretch {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].Stretch); }
      set { this.profile.message.LayoutParameters[this.index].Stretch = Convert.ToByte(value); }
    }
    public bool KeepAspectRatio {
      get { return Convert.ToBoolean(this.profile.message.LayoutParameters[this.index].KeepAspectRatio); }
      set { this.profile.message.LayoutParameters[this.index].KeepAspectRatio = Convert.ToByte(value); }
    }
    public RotateDirections RotateDirection {
      get { return (RotateDirections)this.profile.message.LayoutParameters[this.index].RotateDirection; }
      set { this.profile.message.LayoutParameters[this.index].RotateDirection = Convert.ToInt32(value); }
    }

    // Window
    public UIntPtr Window {
      get {
        switch (this.WindowType) {
          case WindowTypes.Normal: {
            return (UIntPtr)this.profile.message.LayoutParameters[this.index].Window;
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
      get { return this.profile.additionalLayoutParameters[this.index].WindowType; }
    }
    public void SetWindow(UIntPtr window) {
      this.profile.additionalLayoutParameters[this.index].WindowType = WindowTypes.Normal;
      this.profile.message.LayoutParameters[this.index].Window = window.ToUInt64();
    }
    public void SetWindowToDesktop() {
      this.profile.additionalLayoutParameters[this.index].WindowType = WindowTypes.Desktop;
      this.profile.message.LayoutParameters[this.index].Window = 0;
    }
    public void SetWindowToDesktopListView() {
      this.profile.additionalLayoutParameters[this.index].WindowType = WindowTypes.DesktopListView;
      this.profile.message.LayoutParameters[this.index].Window = 0;
    }

    // Fit
    public bool Fit {
      get { return this.profile.additionalLayoutParameters[this.index].Fit; }
      set { this.profile.additionalLayoutParameters[this.index].Fit = value; }
    }

    // Bound *
    public double BoundRelativeLeft {
      get { return this.profile.additionalLayoutParameters[this.index].BoundRelativeLeft; }
      set { this.profile.additionalLayoutParameters[this.index].BoundRelativeLeft = value; }
    }
    public int BoundLeft(int sampleWidth) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.index].BoundRelativeLeft * sampleWidth);
    }
    public double BoundRelativeTop {
      get { return this.profile.additionalLayoutParameters[this.index].BoundRelativeTop; }
      set { this.profile.additionalLayoutParameters[this.index].BoundRelativeTop = value; }
    }
    public int BoundTop(int sampleHeight) {
      return (int)Math.Ceiling(this.profile.additionalLayoutParameters[this.index].BoundRelativeTop * sampleHeight);
    }
    public double BoundRelativeRight {
      get { return this.profile.additionalLayoutParameters[this.index].BoundRelativeRight; }
      set { this.profile.additionalLayoutParameters[this.index].BoundRelativeRight = value; }
    }
    public int BoundRight(int sampleWidth) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.index].BoundRelativeRight * sampleWidth);
    }
    public double BoundRelativeBottom {
      get { return this.profile.additionalLayoutParameters[this.index].BoundRelativeBottom; }
      set { this.profile.additionalLayoutParameters[this.index].BoundRelativeBottom = value; }
    }
    public int BoundBottom(int sampleHeight) {
      return (int)Math.Floor(this.profile.additionalLayoutParameters[this.index].BoundRelativeBottom * sampleHeight);
    }

    //=================================================================
    // アクセッサ(他のアクセッサやWin32APIを必要とするもの)
    // あまり呼び出し回数が増えるようならキャッシングを考えること
    //=================================================================
    
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

    // Clipping*WithoutFit
    public int ClippingXWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.index].ClippingXWithoutFit; }
      set { this.profile.additionalLayoutParameters[this.index].ClippingXWithoutFit = value; }
    }
    public int ClippingYWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.index].ClippingYWithoutFit; }
      set { this.profile.additionalLayoutParameters[this.index].ClippingYWithoutFit = value; }
    }
    public int ClippingWidthWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.index].ClippingWidthWithoutFit; }
      set { this.profile.additionalLayoutParameters[this.index].ClippingWidthWithoutFit = value; }
    }
    public int ClippingHeightWithoutFit {
      get { return this.profile.additionalLayoutParameters[this.index].ClippingHeightWithoutFit; }
      set { this.profile.additionalLayoutParameters[this.index].ClippingHeightWithoutFit = value; }
    }

    public void FixClippingParametersWithoutFit() {
      if (this.ClippingXWithoutFit < 0) {
        this.ClippingXWithoutFit = 0;
      } else if (this.ClippingXWithoutFit > this.WindowWidth) {
        this.ClippingXWithoutFit = this.WindowWidth;
      }
      if (this.ClippingYWithoutFit < 0) {
        this.ClippingYWithoutFit = 0;
      } else if (this.ClippingYWithoutFit > this.WindowHeight) {
        this.ClippingYWithoutFit = this.WindowHeight;
      }
      if (this.ClippingWidthWithoutFit < 0) {
        this.ClippingWidthWithoutFit = 0;
      } else if (this.ClippingWidthWithoutFit > this.WindowWidth) {
        this.ClippingWidthWithoutFit = this.WindowWidth;
      }
      if (this.ClippingHeightWithoutFit < 0) {
        this.ClippingHeightWithoutFit = 0;
      } else if (this.ClippingHeightWithoutFit > this.WindowHeight) {
        this.ClippingHeightWithoutFit = this.WindowHeight;
      }
    }

    // BackupDesktopClippingXParameters
    public int BackupDesktopClippingX {
      get { return this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingX; }
    }
    public int BackupDesktopClippingY {
      get { return this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingY; }
    }
    public int BackupDesktopClippingWidth {
      get { return this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingWidth; }
    }
    public int BackupDesktopClippingHeight {
      get { return this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingHeight; }
    }

    public bool TryUpdateBackupDesktopClippingParameters() {
      switch (this.WindowType) {
        case WindowTypes.Normal: {
          if (this.Window == UIntPtr.Zero || !ExternalAPI.IsWindow(this.Window)) {
            Debug.Fail("TryUpdateBackupDesktopClippingParameters: Invalid Window");
            return false;
          }
          ExternalAPI.POINT windowPoint = new ExternalAPI.POINT { X = this.ClippingXWithFit, Y = this.ClippingYWithFit };
          ExternalAPI.ClientToScreen(this.Window, ref windowPoint);
          var clippingX = windowPoint.X - ExternalAPI.GetSystemMetrics(ExternalAPI.SM_XVIRTUALSCREEN);
          var clippingY = windowPoint.Y - ExternalAPI.GetSystemMetrics(ExternalAPI.SM_YVIRTUALSCREEN);
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingX = clippingX;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingY = clippingY;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingWidth = this.ClippingWidthWithFit;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingHeight = this.ClippingHeightWithFit;
          return true;
        }
        case WindowTypes.DesktopListView:
        case WindowTypes.Desktop: {
          //　特に変換の必要はないのでClippingの結果でそのまま代入
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingX = this.ClippingXWithFit;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingY = this.ClippingYWithFit;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingWidth = this.ClippingWidthWithFit;
          this.profile.additionalLayoutParameters[this.index].BackupDesktopClippingHeight = this.ClippingHeightWithFit;
          return true;
        }
        default: {
          Debug.Fail("TryUpdateBackupDesktopClippingParameters: Invalid WindowType");
          return false;
        }
      }
    }

    //=================================================================
    // メンバ変数
    //=================================================================
    private readonly int index;
    private readonly Profile profile;
  }
}
}