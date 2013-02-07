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

/// @file SCFF.Common/TinyKeyValueFile.cs
/// @copydoc SCFF::Common::TinyKeyValueFile

namespace SCFF.Common {

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using SCFF.Interprocess;

/// INIFileよりも更に低機能＆高速なファイル読み書き用オブジェクト
/// @warning データを一回Dictionaryに読み込むので大きなファイルには非対応
public abstract class TinyKeyValueFile {
  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================
  
  /// コンストラクタ
  public TinyKeyValueFile() {
    this.TinyDictionary = new Dictionary<string,string>();
  }

  //===================================================================
  // メソッド
  //===================================================================

  /// ファイルから読み込み
  public virtual bool ReadFile(string path) {
    this.TinyDictionary.Clear();

    // テキストファイルの読み込み
    var lines = new List<string>();
    try {
      using (var reader = new StreamReader(path)) {
        while (!reader.EndOfStream) {
          string key, value;
          if (this.TrySplit(reader.ReadLine(), out key, out value)) {
            this.TinyDictionary.Add(key, value); 
          }
        }
      }
      return true;
    } catch (Exception) {
      Debug.WriteLine("Cannot read file", "TinyKeyValueFile.Read");
      return false;
    }
  }

  /// ファイルへ書き込み
  public virtual bool WriteFile(string path) {
    return true;
  }

  //===================================================================
  // private メソッド
  //===================================================================

  /// ファイルから読み込んだ文字列をkeyとvalueに分割する
  private bool TrySplit(string line, out string key, out string value) {
    key = null;
    value = null;

    if (line.Length == 0 || line[0] == ';' || line[0] == '[') {
      // 空行、コメント行、セクション記述行は読み飛ばす
      return false;
    }

    var splitIndex = line.IndexOf('=');
    if (splitIndex == -1) {
      // '='が見つからなければ読みとばす
      return false;
    } else if (splitIndex == line.Length - 1) {
      // 空文字列なので読み飛ばす
      return false;
    }

    key = line.Substring(0, splitIndex).Trim();
    value = line.Substring(splitIndex+1);
    return true;
  }

  //===================================================================
  // protected メソッド
  //===================================================================

  /// valueをstringでそのまま取得
  protected bool TryGetString(string key, out string value) {
    return this.TinyDictionary.TryGetValue(key, out value);
  }

  /// valueをintで取得
  protected bool TryGetInt(string key, out int value) {
    value = 0;
    if (!this.TinyDictionary.ContainsKey(key)) return false;
    return int.TryParse(this.TinyDictionary[key], out value);
  }

  /// valueをdoubleで取得
  protected bool TryGetDouble(string key, out double value) {
    value = 0.0;
    if (!this.TinyDictionary.ContainsKey(key)) return false;
    return double.TryParse(this.TinyDictionary[key], out value);
  }

  /// valueをfloatで取得
  protected bool TryGetFloat(string key, out float value) {
    value = 0.0F;
    if (!this.TinyDictionary.ContainsKey(key)) return false;
    return float.TryParse(this.TinyDictionary[key], out value);
  }

  /// valueをboolで取得
  protected bool TryGetBool(string key, out bool value) {
    value = false;
    if (!this.TinyDictionary.ContainsKey(key)) return false;
    return bool.TryParse(this.TinyDictionary[key], out value);
  }

  /// valueをUIntPtrで取得
  protected bool TryGetUIntPtr(string key, out UIntPtr value) {
    value = UIntPtr.Zero;
    if (!this.TinyDictionary.ContainsKey(key)) return false;
    UInt64 internalValue;
    if (UInt64.TryParse(this.TinyDictionary[key], out internalValue)) {
      value = (UIntPtr)internalValue;
      return true;
    }
    return false;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// データを格納するためのDictionary
  private Dictionary<string,string> TinyDictionary { get; set; }
}
}   // namespace SCFF.Common
