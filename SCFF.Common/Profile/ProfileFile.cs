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

/// @file SCFF.Common/Profile/ProfileFile.cs
/// @copydoc SCFF::Common::Profile::ProfileFile

namespace SCFF.Common.Profile {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SCFF.Interprocess;

/// ProfileのFile入出力機能
public class ProfileFile : TinyKeyValueFile {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public ProfileFile(Profile profile) : base() {
    this.Profile = profile;
  }

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @param profile 保存するProfile
  /// @param path 保存先
  /// @return 保存が成功したか
  public override bool WriteFile(string path) {
    // nop
    if (!base.WriteFile(path)) return false;   

    try {
      using (var writer = new StreamWriter(path)) {
        writer.WriteLine(Constants.ProfileHeader);
        writer.WriteLine("LayoutElementCount={0}", this.Profile.LayoutElementCount);
        writer.WriteLine("CurrentIndex={0}", this.Profile.CurrentIndex);
        foreach (var layoutElement in this.Profile) {
          var index = layoutElement.Index;
          writer.WriteLine("[LayoutElement{0}]", index);

          // TargetWindow
          writer.WriteLine("Window{0}={1}", index, layoutElement.Window);
          writer.WriteLine("WindowType{0}={1}", index, (int)layoutElement.WindowType);
          // Area
          writer.WriteLine("Fit{0}={1}", index, layoutElement.Fit);
          writer.WriteLine("ClippingXWithoutFit{0}={1}", index, layoutElement.ClippingXWithoutFit);
          writer.WriteLine("ClippingYWithoutFit{0}={1}", index, layoutElement.ClippingYWithoutFit);
          writer.WriteLine("ClippingWidthWithoutFit{0}={1}", index, layoutElement.ClippingWidthWithoutFit);
          writer.WriteLine("ClippingHeightWithoutFit{0}={1}", index, layoutElement.ClippingHeightWithoutFit);
          // Options
          writer.WriteLine("ShowCursor{0}={1}", index, layoutElement.ShowCursor);
          writer.WriteLine("ShowLayeredWindow{0}={1}", index, layoutElement.ShowLayeredWindow);
          writer.WriteLine("Stretch{0}={1}", index, layoutElement.Stretch);
          writer.WriteLine("KeepAspectRatio{0}={1}", index, layoutElement.KeepAspectRatio);
          writer.WriteLine("RotateDirection{0}={1}", index, (int)layoutElement.RotateDirection);
          // ResizeMethod
          writer.WriteLine("SWScaleFlags{0}={1}", index, (int)layoutElement.SWScaleFlags);
          writer.WriteLine("SWScaleAccurateRnd{0}={1}", index, layoutElement.SWScaleAccurateRnd);
          writer.WriteLine("SWScaleIsFilterEnabled{0}={1}", index, layoutElement.SWScaleIsFilterEnabled);
          writer.WriteLine("SWScaleLumaGBlur{0}={1}", index, layoutElement.SWScaleLumaGBlur);
          writer.WriteLine("SWScaleChromaGBlur{0}={1}", index, layoutElement.SWScaleChromaGBlur);
          writer.WriteLine("SWScaleLumaSharpen{0}={1}", index, layoutElement.SWScaleLumaSharpen);
          writer.WriteLine("SWScaleChromaSharpen{0}={1}", index, layoutElement.SWScaleChromaSharpen);
          writer.WriteLine("SWScaleChromaHShift{0}={1}", index, layoutElement.SWScaleChromaHShift);
          writer.WriteLine("SWScaleChromaVShift{0}={1}", index, layoutElement.SWScaleChromaVShift);
          // LayoutParameter
          writer.WriteLine("BoundRelativeLeft{0}={1}", index, layoutElement.BoundRelativeLeft);
          writer.WriteLine("BoundRelativeTop{0}={1}", index, layoutElement.BoundRelativeTop);
          writer.WriteLine("BoundRelativeRight{0}={1}", index, layoutElement.BoundRelativeRight);
          writer.WriteLine("BoundRelativeBottom{0}={1}", index, layoutElement.BoundRelativeBottom);
          // Backup
          writer.WriteLine("HasBackedUp{0}={1}", index, layoutElement.HasBackedUp);
          writer.WriteLine("BackupScreenClippingX{0}={1}", index, layoutElement.BackupScreenClippingX);
          writer.WriteLine("BackupScreenClippingY{0}={1}", index, layoutElement.BackupScreenClippingY);
          writer.WriteLine("BackupClippingWidth{0}={1}", index, layoutElement.BackupClippingWidth);
          writer.WriteLine("BackupClippingHeight{0}={1}", index, layoutElement.BackupClippingHeight);
        }
        return true;
      }
    } catch (Exception) {
      // 特に何も警告はしない
      Debug.WriteLine("Cannot save profile", "ProfileINIFile.Save");
      return false;
    }
  }

  //===================================================================
  // ファイル入力
  //===================================================================

  /// ファイル入力
  /// @param[in] path ファイルパス
  /// @return 正常終了
  /// @warning 継承クラスでのreturn falseは禁止
  public override bool ReadFile(string path) {
    // ファイル->ディクショナリ
    if (!base.ReadFile(path)) return false;

    // 使いまわすので注意
    int intValue;
    double doubleValue;
    float floatValue;
    bool boolValue;

    // Dictionaryを調べながら値を設定する
    var originalLayoutElementCount = this.Profile.LayoutElementCount;
    if (this.TryGetInt("LayoutElementCount", out intValue)) {
      // 範囲チェック
      if (intValue < 1)
        intValue = 1;
      if (Interprocess.MaxComplexLayoutElements < intValue)
        intValue = Interprocess.MaxComplexLayoutElements;
      this.Profile.LayoutElementCount = intValue;
    }

    if (this.TryGetInt("CurrentIndex", out intValue)) {
      //　範囲チェック
      if (intValue < 0)
        intValue = 0;
      if (this.Profile.LayoutElementCount - 1 < intValue)
        intValue = this.Profile.LayoutElementCount - 1;
      this.Profile.CurrentIndex = intValue;
    }
    
    for (int i = 0; i < this.Profile.LayoutElementCount; ++i) {
      var layoutElement = this.Profile.GetLayoutElement(i);
      layoutElement.RestoreDefault();

      // TargetWindow
      WindowTypes windowTypes;
      if (this.TryGetWindowTypes("WindowType" + i, out windowTypes)) {
        switch (windowTypes) {
          case WindowTypes.Normal: {
            UIntPtr uintptrValue;
            if (this.TryGetUIntPtr("Window" + i, out uintptrValue)) {
              layoutElement.SetWindow(uintptrValue);
            }
            break;
          }
          case WindowTypes.DesktopListView: {
            layoutElement.SetWindowToDesktopListView();
            break;
          }
          case WindowTypes.Desktop: {
            layoutElement.SetWindowToDesktop();
            break;
          }
        }
      }

      // Area
      if (this.TryGetBool("Fit" + i, out boolValue)) {
        layoutElement.SetFit = boolValue;
      }
      if (this.TryGetInt("ClippingXWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingXWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingYWithoutFit" + i, out intValue)) {
        layoutElement.SetClippingYWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingWidthWithoutFit" + i, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.SetClippingWidthWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingHeightWithoutFit" + i, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.SetClippingHeightWithoutFit = intValue;
      }

      // Options
      if (this.TryGetBool("ShowCursor" + i, out boolValue)) {
        layoutElement.SetShowCursor = boolValue;
      }
      if (this.TryGetBool("ShowLayeredWindow" + i, out boolValue)) {
        layoutElement.SetShowLayeredWindow = boolValue;
      }
      if (this.TryGetBool("Stretch" + i, out boolValue)) {
        layoutElement.SetStretch = boolValue;
      }
      if (this.TryGetBool("KeepAspectRatio" + i, out boolValue)) {
        layoutElement.SetKeepAspectRatio = boolValue;
      }
      RotateDirections rotateDirections;
      if (this.TryGetRotateDirections("RotateDirection" + i, out rotateDirections)) {
        layoutElement.SetRotateDirection = rotateDirections;
      }
      // ResizeMethod
      SWScaleFlags swscaleFlags;
      if (this.TryGetSWScaleFlags("SWScaleFlags" + i, out swscaleFlags)) {
        layoutElement.SetSWScaleFlags = swscaleFlags;
      }
      if (this.TryGetBool("SWScaleAccurateRnd" + i, out boolValue)) {
        layoutElement.SetSWScaleAccurateRnd = boolValue;
      }
      if (this.TryGetBool("SWScaleIsFilterEnabled" + i, out boolValue)) {
        layoutElement.SetSWScaleIsFilterEnabled = boolValue;
      }
      if (this.TryGetFloat("SWScaleLumaGBlur" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.LumaGBlur,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleLumaGBlur = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaGBlur" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaGBlur,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleChromaGBlur = floatValue;
      }
      if (this.TryGetFloat("SWScaleLumaSharpen" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.LumaSharpen,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleLumaSharpen = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaSharpen" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaSharpen,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleChromaSharpen = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaHShift" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaHShift,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleChromaHShift = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaVShift" + i, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaVShift,
                                        floatValue,
                                        out floatValue);
        layoutElement.SetSWScaleChromaVShift = floatValue;
      }

      // LayoutParameter
      if (this.TryGetDouble("BoundRelativeLeft" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeLeft = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeTop" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeTop = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeRight" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeRight = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeBottom" + i, out doubleValue)) {
        layoutElement.SetBoundRelativeBottom = doubleValue;
      }
      var boundRelativeRect = BoundRelativeInputCorrector.Correct(layoutElement);
      layoutElement.SetBoundRelativeLeft = boundRelativeRect.Left;
      layoutElement.SetBoundRelativeTop = boundRelativeRect.Top;
      layoutElement.SetBoundRelativeRight = boundRelativeRect.Right;
      layoutElement.SetBoundRelativeBottom = boundRelativeRect.Bottom;

      // Backup
      if (this.TryGetBool("HasBackedUp" + i, out boolValue)) {
        layoutElement.SetHasBackedUp = boolValue;
      }
      if (this.TryGetInt("BackupScreenClippingX" + i, out intValue)) {
        layoutElement.SetBackupScreenClippingX = intValue;
      }
      if (this.TryGetInt("BackupScreenClippingY" + i, out intValue)) {
        layoutElement.SetBackupScreenClippingY = intValue;
      }
      if (this.TryGetInt("BackupClippingWidth" + i, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.SetBackupClippingWidth = intValue;
      }
      if (this.TryGetInt("BackupClippingHeight" + i, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.SetBackupClippingHeight = intValue;
      }
    }

    return true;
  }

  //===================================================================
  // private メソッド
  //===================================================================

  /// valueをRotateDirectionsで取得
  private bool TryGetRotateDirections(string key, out RotateDirections value) {
    int internalValue;
    if (this.TryGetInt(key, out internalValue)) {
      value = (RotateDirections)internalValue;
    } else {
      value = RotateDirections.NoRotate;
      return false;
    }
    /// @warning 範囲チェックはEnum.IsDefinedを使ってはいけない
    switch (value) {
      case RotateDirections.NoRotate:
      case RotateDirections.Degrees90:
      case RotateDirections.Degrees180:
      case RotateDirections.Degrees270: return true;
      default: return false;
    }
  }

  /// valueをSWScaleFlagsで取得
  private bool TryGetSWScaleFlags(string key, out SWScaleFlags value) {
    int internalValue;
    if (this.TryGetInt(key, out internalValue)) {
      value = (SWScaleFlags)internalValue;
    } else {
      value = SWScaleFlags.FastBilinear;
      return false;
    }
    /// @warning 範囲チェックはEnum.IsDefinedを使ってはいけない
    switch (value) {
      case SWScaleFlags.FastBilinear:
      case SWScaleFlags.Bilinear:
      case SWScaleFlags.Bicubic:
      case SWScaleFlags.X:
      case SWScaleFlags.Point:
      case SWScaleFlags.Area:
      case SWScaleFlags.Bicublin:
      case SWScaleFlags.Gauss:
      case SWScaleFlags.Sinc:
      case SWScaleFlags.Lanczos:
      case SWScaleFlags.Spline: return true;
      default: return false;
    }
  }

  /// valueをWindowTypesで取得
  private bool TryGetWindowTypes(string key, out WindowTypes value) {
    int internalValue;
    if (this.TryGetInt(key, out internalValue)) {
      value = (WindowTypes)internalValue;
    } else {
      value = WindowTypes.Normal;
      return false;
    }
    /// @warning 範囲チェックはEnum.IsDefinedを使ってはいけない
    switch (value) {
      case WindowTypes.Normal:
      case WindowTypes.DesktopListView:
      case WindowTypes.Desktop: return true;
      default: return false;
    }
  }

  //===================================================================
  // プロパティ
  //===================================================================
  
  /// Profileへの参照
  private Profile Profile { get; set; }
}
}   // namespace SCFF.Common.Profile
