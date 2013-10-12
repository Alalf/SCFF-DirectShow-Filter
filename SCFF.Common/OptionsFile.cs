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

/// @file SCFF.Common/OptionsFile.cs
/// @copydoc SCFF::Common::OptionsFile

namespace SCFF.Common {

using System;
using System.Diagnostics;
using System.IO;

/// OptionsのFile入出力機能
public class OptionsFile : TinyKeyValueFile {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public OptionsFile(Options options) : base() {
    this.options = options;
  }

  //===================================================================
  // ファイル出力
  //===================================================================

  /// ファイル出力
  /// @param path 出力先
  /// @return 出力成功
  public override bool WriteFile(string path) {
    // nop
    var baseResult = base.WriteFile(path);
    if (!baseResult) return false;

    try {
      using (var writer = new StreamWriter(path)) {
        writer.WriteLine(Constants.OptionsHeader);
        for (int i = 0; i < Constants.RecentProfilesLength; ++i) {
          writer.WriteLine("RecentProfile{0}={1}", i,
                           this.options.GetRecentProfile(i));
        }
        writer.WriteLine("FFmpegPath={0}", this.options.FFmpegPath);
        writer.WriteLine("FFmpegArguments={0}", this.options.FFmpegArguments);
        writer.WriteLine("TmpLeft={0}", this.options.TmpLeft);
        writer.WriteLine("TmpTop={0}", this.options.TmpTop);

        writer.WriteLine("TmpNormalWidth={0}", this.options.TmpNormalWidth);
        writer.WriteLine("TmpNormalHeight={0}", this.options.TmpNormalHeight);
        writer.WriteLine("TmpNormalLayoutWidth={0}", this.options.TmpNormalLayoutWidth);
        writer.WriteLine("TmpNormalLayoutHeight={0}", this.options.TmpNormalLayoutHeight);
        writer.WriteLine("TmpCompactWidth={0}", this.options.TmpCompactWidth);
        writer.WriteLine("TmpCompactHeight={0}", this.options.TmpCompactHeight);
        writer.WriteLine("TmpCompactLayoutWidth={0}", this.options.TmpCompactLayoutWidth);
        writer.WriteLine("TmpCompactLayoutHeight={0}", this.options.TmpCompactLayoutHeight);

        writer.WriteLine("WindowState={0}", (int)this.options.WindowState);
        writer.WriteLine("AreaIsExpanded={0}", this.options.AreaIsExpanded);
        writer.WriteLine("OptionsIsExpanded={0}", this.options.OptionsIsExpanded);
        writer.WriteLine("ResizeMethodIsExpanded={0}",
                         this.options.ResizeMethodIsExpanded);
        writer.WriteLine("LayoutIsExpanded={0}", this.options.LayoutIsExpanded);
        writer.WriteLine("AutoApply={0}", this.options.AutoApply);
        writer.WriteLine("LayoutPreview={0}", this.options.LayoutPreview);
        writer.WriteLine("LayoutPreviewInterval={0}", this.options.LayoutPreviewInterval);
        writer.WriteLine("LayoutBorder={0}", this.options.LayoutBorder);
        writer.WriteLine("LayoutSnap={0}", this.options.LayoutSnap);
        writer.WriteLine("CompactView={0}", this.options.CompactView);
        writer.WriteLine("ForceAeroOn={0}", this.options.ForceAeroOn);
        writer.WriteLine("SaveProfileOnExit={0}", this.options.SaveProfileOnExit);
        writer.WriteLine("RestoreLastProfile={0}", this.options.RestoreLastProfile);
        writer.WriteLine("RestoreMissingWindowWhenOpeningProfile={0}",
                         this.options.RestoreMissingWindowWhenOpeningProfile);
        writer.WriteLine("EnableGPUPreviewRendering={0}", this.options.EnableGPUPreviewRendering);
        return true;
      }
    } catch (Exception) {
      // 特に何も警告はしない
      Debug.WriteLine("Cannot save options", "OptionsINIFile.Save");
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
    var baseResult = base.ReadFile(path);
    if (!baseResult) return false;

    // 使いまわすので注意
    string stringValue;
    int intValue;
    double doubleValue;
    bool boolValue;

    // Dictionaryを調べながら値を設定する
    string prefix = "RecentProfile";
    for (int i = 0; i < Constants.RecentProfilesLength; ++i) {
      if (this.TryGetString(prefix + i, out stringValue)) {
        this.options.SetRecentProfile(i, stringValue);
      }
    }
    if (this.TryGetString("FFmpegPath", out stringValue)) {
      this.options.FFmpegPath = stringValue;
    }
    if (this.TryGetString("FFmpegArguments", out stringValue)) {
      this.options.FFmpegArguments = stringValue;
    }
    if (this.TryGetDouble("TmpLeft", out doubleValue)) {
      this.options.TmpLeft = doubleValue;
    }
    if (this.TryGetDouble("TmpTop", out doubleValue)) {
      this.options.TmpTop = doubleValue;
    }

    if (this.TryGetDouble("TmpNormalWidth", out doubleValue)) {
      this.options.TmpNormalWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalHeight", out doubleValue)) {
      this.options.TmpNormalHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalLayoutWidth", out doubleValue)) {
      this.options.TmpNormalLayoutWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalLayoutHeight", out doubleValue)) {
      this.options.TmpNormalLayoutHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactWidth", out doubleValue)) {
      this.options.TmpCompactWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactHeight", out doubleValue)) {
      this.options.TmpCompactHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactLayoutWidth", out doubleValue)) {
      this.options.TmpCompactLayoutWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactLayoutHeight", out doubleValue)) {
      this.options.TmpCompactLayoutHeight = doubleValue;
    }

    WindowState windowState;
    if (this.TryGetWindowState("WindowState", out windowState)) {
      this.options.WindowState = windowState;
    }
    if (this.TryGetBool("AreaIsExpanded", out boolValue)) {
      this.options.AreaIsExpanded = boolValue;
    }
    if (this.TryGetBool("OptionsIsExpanded", out boolValue)) {
      this.options.OptionsIsExpanded = boolValue;
    }
    if (this.TryGetBool("ResizeMethodIsExpanded", out boolValue)) {
      this.options.ResizeMethodIsExpanded = boolValue;
    }
    if (this.TryGetBool("LayoutIsExpanded", out boolValue)) {
      this.options.LayoutIsExpanded = boolValue;
    }
    if (this.TryGetBool("AutoApply", out boolValue)) {
      this.options.AutoApply = boolValue;
    }
    if (this.TryGetBool("LayoutPreview", out boolValue)) {
      this.options.LayoutPreview = boolValue;
    }
    if (this.TryGetInt("LayoutPreviewInterval", out intValue)) {
      // 範囲チェック
      if (intValue < Constants.MinimumLayoutPreviewInterval) {
        intValue = Constants.MinimumLayoutPreviewInterval;
      } else if (Constants.MaximumLayoutPreviewInterval < intValue) {
        intValue = Constants.MaximumLayoutPreviewInterval;
      }
      this.options.LayoutPreviewInterval = intValue;
    }
    if (this.TryGetBool("LayoutBorder", out boolValue)) {
      this.options.LayoutBorder = boolValue;
    }
    if (this.TryGetBool("LayoutSnap", out boolValue)) {
      this.options.LayoutSnap = boolValue;
    }
    if (this.TryGetBool("CompactView", out boolValue)) {
      this.options.CompactView = boolValue;
    }
    if (this.TryGetBool("ForceAeroOn", out boolValue)) {
      this.options.ForceAeroOn = boolValue;
    }
    if (this.TryGetBool("SaveProfileOnExit", out boolValue)) {
      this.options.SaveProfileOnExit = boolValue;
    }
    if (this.TryGetBool("RestoreLastProfile", out boolValue)) {
      this.options.RestoreLastProfile = boolValue;
    }
    if (this.TryGetBool("RestoreMissingWindowWhenOpeningProfile", out boolValue)) {
      this.options.RestoreMissingWindowWhenOpeningProfile = boolValue;
    }
    if (this.TryGetBool("EnableGPUPreviewRendering", out boolValue)) {
      this.options.EnableGPUPreviewRendering = boolValue;
    }

    return true;
  }

  //===================================================================
  // private メソッド
  //===================================================================

  /// valueをOptions.WindowStateで取得
  /// @param key Key
  /// @param value WindowState型に変換されたValue
  /// @return 取得成功
  private bool TryGetWindowState(string key, out WindowState value) {
    int internalValue;
    if (this.TryGetInt(key, out internalValue)) {
      value = (WindowState)internalValue;
    } else {
      value = WindowState.Normal;
      return false;
    }
    /// @warning 範囲チェックはEnum.IsDefinedを使ってはいけない
    switch (value) {
      case WindowState.Normal:
      case WindowState.Minimized:
      case WindowState.Maximized: return true;
      default: return false;
    }
  }

  //===================================================================
  // フィールド
  //===================================================================
  
  /// Optionsへの参照
  private readonly Options options;
}
}   // namespace SCFF.Common
