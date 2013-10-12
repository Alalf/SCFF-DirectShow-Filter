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
    this.profile = profile;
  }

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @param path 保存先
  /// @return 保存が成功したか
  public override bool WriteFile(string path) {
    // nop
    if (!base.WriteFile(path)) return false;   

    try {
      using (var writer = new StreamWriter(path)) {
        writer.WriteLine(Constants.ProfileHeader);
        writer.WriteLine("LayoutElementCount={0}", this.profile.LayoutElements.Count);
        writer.WriteLine("CurrentIndex={0}", this.profile.GetCurrentIndex());
        int index = 0;
        foreach (var layoutElement in this.profile.LayoutElements) {
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

          ++index;
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
    var layoutElements = new List<LayoutElement>();
    int layoutElementCount = 0;
    int currentIndex = 0;
    LayoutElement current = null;

    if (this.TryGetInt("LayoutElementCount", out intValue)) {
      // 範囲チェック
      if (intValue < 1)
        intValue = 1;
      if (Interprocess.MaxComplexLayoutElements < intValue)
        intValue = Interprocess.MaxComplexLayoutElements;
      layoutElementCount = intValue;
    }

    if (this.TryGetInt("CurrentIndex", out intValue)) {
      //　範囲チェック
      if (intValue < 0)
        intValue = 0;
      if (layoutElementCount - 1 < intValue)
        intValue = layoutElementCount - 1;
      currentIndex = intValue;
    }
    
    for (int index = 0; index < layoutElementCount; ++index) {
      var layoutElement = new LayoutElement(index);

      // TargetWindow
      WindowTypes windowTypes;
      if (this.TryGetWindowTypes("WindowType" + index, out windowTypes)) {
        switch (windowTypes) {
          case WindowTypes.Normal: {
            UIntPtr uintptrValue;
            if (this.TryGetUIntPtr("Window" + index, out uintptrValue)) {
              layoutElement.SetWindow(uintptrValue);
            }
            break;
          }
          case WindowTypes.DXGI: {
            layoutElement.SetWindowToDXGI();
            break;
          }
          case WindowTypes.Desktop: {
            layoutElement.SetWindowToDesktop();
            break;
          }
        }
      }

      // Area
      if (this.TryGetBool("Fit" + index, out boolValue)) {
        layoutElement.Fit = boolValue;
      }
      if (this.TryGetInt("ClippingXWithoutFit" + index, out intValue)) {
        layoutElement.ClippingXWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingYWithoutFit" + index, out intValue)) {
        layoutElement.ClippingYWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingWidthWithoutFit" + index, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.ClippingWidthWithoutFit = intValue;
      }
      if (this.TryGetInt("ClippingHeightWithoutFit" + index, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.ClippingHeightWithoutFit = intValue;
      }

      // Options
      if (this.TryGetBool("ShowCursor" + index, out boolValue)) {
        layoutElement.ShowCursor = boolValue;
      }
      if (this.TryGetBool("ShowLayeredWindow" + index, out boolValue)) {
        layoutElement.ShowLayeredWindow = boolValue;
      }
      if (this.TryGetBool("Stretch" + index, out boolValue)) {
        layoutElement.Stretch = boolValue;
      }
      if (this.TryGetBool("KeepAspectRatio" + index, out boolValue)) {
        layoutElement.KeepAspectRatio = boolValue;
      }
      RotateDirections rotateDirections;
      if (this.TryGetRotateDirections("RotateDirection" + index, out rotateDirections)) {
        layoutElement.RotateDirection = rotateDirections;
      }
      // ResizeMethod
      SWScaleFlags swscaleFlags;
      if (this.TryGetSWScaleFlags("SWScaleFlags" + index, out swscaleFlags)) {
        layoutElement.SWScaleFlags = swscaleFlags;
      }
      if (this.TryGetBool("SWScaleAccurateRnd" + index, out boolValue)) {
        layoutElement.SWScaleAccurateRnd = boolValue;
      }
      if (this.TryGetBool("SWScaleIsFilterEnabled" + index, out boolValue)) {
        layoutElement.SWScaleIsFilterEnabled = boolValue;
      }
      if (this.TryGetFloat("SWScaleLumaGBlur" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.LumaGBlur,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleLumaGBlur = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaGBlur" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaGBlur,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleChromaGBlur = floatValue;
      }
      if (this.TryGetFloat("SWScaleLumaSharpen" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.LumaSharpen,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleLumaSharpen = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaSharpen" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaSharpen,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleChromaSharpen = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaHShift" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaHShift,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleChromaHShift = floatValue;
      }
      if (this.TryGetFloat("SWScaleChromaVShift" + index, out floatValue)) {
        SWScaleInputCorrector.TryChange(SWScaleInputCorrector.Names.ChromaVShift,
                                        floatValue,
                                        out floatValue);
        layoutElement.SWScaleChromaVShift = floatValue;
      }

      // LayoutParameter
      if (this.TryGetDouble("BoundRelativeLeft" + index, out doubleValue)) {
        layoutElement.BoundRelativeLeft = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeTop" + index, out doubleValue)) {
        layoutElement.BoundRelativeTop = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeRight" + index, out doubleValue)) {
        layoutElement.BoundRelativeRight = doubleValue;
      }
      if (this.TryGetDouble("BoundRelativeBottom" + index, out doubleValue)) {
        layoutElement.BoundRelativeBottom = doubleValue;
      }
      var boundRelativeRect = BoundRelativeInputCorrector.Correct(layoutElement);
      layoutElement.BoundRelativeLeft = boundRelativeRect.Left;
      layoutElement.BoundRelativeTop = boundRelativeRect.Top;
      layoutElement.BoundRelativeRight = boundRelativeRect.Right;
      layoutElement.BoundRelativeBottom = boundRelativeRect.Bottom;

      // Backup
      if (this.TryGetBool("HasBackedUp" + index, out boolValue)) {
        layoutElement.HasBackedUp = boolValue;
      }
      if (this.TryGetInt("BackupScreenClippingX" + index, out intValue)) {
        layoutElement.BackupScreenClippingX = intValue;
      }
      if (this.TryGetInt("BackupScreenClippingY" + index, out intValue)) {
        layoutElement.BackupScreenClippingY = intValue;
      }
      if (this.TryGetInt("BackupClippingWidth" + index, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.BackupClippingWidth = intValue;
      }
      if (this.TryGetInt("BackupClippingHeight" + index, out intValue)) {
        // 範囲チェック
        if (intValue < Constants.MinimumClippingSize)
          intValue = Constants.MinimumClippingSize;
        layoutElement.BackupClippingHeight = intValue;
      }

      layoutElements.Add(layoutElement);
      if (index == currentIndex) {
        current = layoutElement;
      }
    }

    this.profile.SetLayoutElements(layoutElements, current);

    return true;
  }

  //===================================================================
  // private メソッド
  //===================================================================

  /// valueをRotateDirectionsで取得
  /// @param key Key
  /// @param value RotateDirections型に変換されたValue
  /// @return 取得成功
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
  /// @param key Key
  /// @param value SWScaleFlags型に変換されたValue
  /// @return 取得成功
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
  /// @param key Key
  /// @param value WindowTypes型に変換されたValue
  /// @return 取得成功
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
      case WindowTypes.DXGI:
      case WindowTypes.Desktop: return true;
      default: return false;
    }
  }

  //===================================================================
  // フィールド
  //===================================================================
  
  /// Profileへの参照
  private readonly Profile profile;
}
}   // namespace SCFF.Common.Profile
