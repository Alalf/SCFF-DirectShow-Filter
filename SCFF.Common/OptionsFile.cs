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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

/// OptionsのFile入出力機能
public class OptionsFile : TinyKeyValueFile {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public OptionsFile(Options options) : base() {
    this.Options = options;
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
        for (int i = 0; i < 5; ++i) {
          writer.WriteLine("RecentProfile{0}={1}", i,
                           this.Options.GetRecentProfile(i));
        }
        writer.WriteLine("FFmpegPath={0}", this.Options.FFmpegPath);
        writer.WriteLine("FFmpegArguments={0}", this.Options.FFmpegArguments);
        writer.WriteLine("TmpLeft={0}", this.Options.TmpLeft);
        writer.WriteLine("TmpTop={0}", this.Options.TmpTop);

        writer.WriteLine("TmpNormalWidth={0}", this.Options.TmpNormalWidth);
        writer.WriteLine("TmpNormalHeight={0}", this.Options.TmpNormalHeight);
        writer.WriteLine("TmpNormalLayoutWidth={0}", this.Options.TmpNormalLayoutWidth);
        writer.WriteLine("TmpNormalLayoutHeight={0}", this.Options.TmpNormalLayoutHeight);
        writer.WriteLine("TmpCompactWidth={0}", this.Options.TmpCompactWidth);
        writer.WriteLine("TmpCompactHeight={0}", this.Options.TmpCompactHeight);
        writer.WriteLine("TmpCompactLayoutWidth={0}", this.Options.TmpCompactLayoutWidth);
        writer.WriteLine("TmpCompactLayoutHeight={0}", this.Options.TmpCompactLayoutHeight);

        writer.WriteLine("TmpWindowState={0}", (int)this.Options.TmpWindowState);
        writer.WriteLine("AreaIsExpanded={0}", this.Options.AreaIsExpanded);
        writer.WriteLine("OptionsIsExpanded={0}", this.Options.OptionsIsExpanded);
        writer.WriteLine("ResizeMethodIsExpanded={0}",
                         this.Options.ResizeMethodIsExpanded);
        writer.WriteLine("LayoutIsExpanded={0}", this.Options.LayoutIsExpanded);
        writer.WriteLine("AutoApply={0}", this.Options.AutoApply);
        writer.WriteLine("LayoutPreview={0}", this.Options.LayoutPreview);
        writer.WriteLine("LayoutBorder={0}", this.Options.LayoutBorder);
        writer.WriteLine("LayoutSnap={0}", this.Options.LayoutSnap);
        writer.WriteLine("CompactView={0}", this.Options.CompactView);
        writer.WriteLine("ForceAeroOn={0}", this.Options.ForceAeroOn);
        writer.WriteLine("RestoreLastProfile={0}", this.Options.RestoreLastProfile);
        writer.WriteLine("RestoreMissingWindowWhenOpeningProfile={0}",
                         this.Options.RestoreMissingWindowWhenOpeningProfile);
        writer.WriteLine("EnableGPUPreviewRendering={0}", this.Options.EnableGPUPreviewRendering);
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
    double doubleValue;
    bool boolValue;

    // Dictionaryを調べながら値を設定する
    string prefix = "RecentProfile";
    for (int i = 0; i < 5; ++i) {
      if (this.TryGetString(prefix + i, out stringValue)) {
        this.Options.SetRecentProfile(i, stringValue);
      }
    }
    if (this.TryGetString("FFmpegPath", out stringValue)) {
      this.Options.FFmpegPath = stringValue;
    }
    if (this.TryGetString("FFmpegArguments", out stringValue)) {
      this.Options.FFmpegArguments = stringValue;
    }
    if (this.TryGetDouble("TmpLeft", out doubleValue)) {
      this.Options.TmpLeft = doubleValue;
    }
    if (this.TryGetDouble("TmpTop", out doubleValue)) {
      this.Options.TmpTop = doubleValue;
    }

    if (this.TryGetDouble("TmpNormalWidth", out doubleValue)) {
      this.Options.TmpNormalWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalHeight", out doubleValue)) {
      this.Options.TmpNormalHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalLayoutWidth", out doubleValue)) {
      this.Options.TmpNormalLayoutWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpNormalLayoutHeight", out doubleValue)) {
      this.Options.TmpNormalLayoutHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactWidth", out doubleValue)) {
      this.Options.TmpCompactWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactHeight", out doubleValue)) {
      this.Options.TmpCompactHeight = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactLayoutWidth", out doubleValue)) {
      this.Options.TmpCompactLayoutWidth = doubleValue;
    }
    if (this.TryGetDouble("TmpCompactLayoutHeight", out doubleValue)) {
      this.Options.TmpCompactLayoutHeight = doubleValue;
    }

    WindowState windowState;
    if (this.TryGetWindowState("TmpWindowState", out windowState)) {
      this.Options.TmpWindowState = windowState;
    }
    if (this.TryGetBool("AreaIsExpanded", out boolValue)) {
      this.Options.AreaIsExpanded = boolValue;
    }
    if (this.TryGetBool("OptionsIsExpanded", out boolValue)) {
      this.Options.OptionsIsExpanded = boolValue;
    }
    if (this.TryGetBool("ResizeMethodIsExpanded", out boolValue)) {
      this.Options.ResizeMethodIsExpanded = boolValue;
    }
    if (this.TryGetBool("LayoutIsExpanded", out boolValue)) {
      this.Options.LayoutIsExpanded = boolValue;
    }
    if (this.TryGetBool("AutoApply", out boolValue)) {
      this.Options.AutoApply = boolValue;
    }
    if (this.TryGetBool("LayoutPreview", out boolValue)) {
      this.Options.LayoutPreview = boolValue;
    }
    if (this.TryGetBool("LayoutBorder", out boolValue)) {
      this.Options.LayoutBorder = boolValue;
    }
    if (this.TryGetBool("LayoutSnap", out boolValue)) {
      this.Options.LayoutSnap = boolValue;
    }
    if (this.TryGetBool("CompactView", out boolValue)) {
      this.Options.CompactView = boolValue;
    }
    if (this.TryGetBool("ForceAeroOn", out boolValue)) {
      this.Options.ForceAeroOn = boolValue;
    }
    if (this.TryGetBool("RestoreLastProfile", out boolValue)) {
      this.Options.RestoreLastProfile = boolValue;
    }
    if (this.TryGetBool("RestoreMissingWindowWhenOpeningProfile", out boolValue)) {
      this.Options.RestoreMissingWindowWhenOpeningProfile = boolValue;
    }
    if (this.TryGetBool("EnableGPUPreviewRendering", out boolValue)) {
      this.Options.EnableGPUPreviewRendering = boolValue;
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
  // プロパティ
  //===================================================================
  
  /// Optionsへの参照
  private Options Options { get; set; }
}
}   // namespace SCFF.Common
