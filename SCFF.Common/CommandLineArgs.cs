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

/// @file SCFF.Common/CommandLineArgs.cs
/// @copydoc SCFF::Common::CommandLineArgs

namespace SCFF.Common {

using System.Collections.Generic;
using System.IO;
using System.Text;

/// コマンドライン引数を処理するためのクラス
/// - path: 指定した場所のProfileを開く
/// - /p pid: プロセスリフトリフレッシュ後、プロセスIDを選択する(なければそのまま)
/// - /s: /pで指定されたプロセスが存在した場合、スプラッシュボタンを可能なら押す(/aよりも優先)
/// - /a: /pで指定されたプロセスが存在した場合、Applyを可能なら押す
public class CommandLineArgs {
  //===================================================================
  // コンストラクタ/デストラクタ
  //===================================================================

  /// コンストラクタ
  public CommandLineArgs() {
    // nop
  }
  public CommandLineArgs(string[] args) {
    this.ParseArgs(args);
  }
  public CommandLineArgs(byte[] data, int actualLength) {
    this.ParseUnicodeData(data, actualLength);
  }

  //===================================================================
  // 外部インタフェース
  //===================================================================

  /// string[]を解析
  private void ParseArgs(string[] args) {
    var isPIDParsing = false;
    foreach (var arg in args) {
      if (arg == "/p") {
        isPIDParsing = true;
      } else if (arg == "/s") {
        this.SplashOption = true;
        isPIDParsing = false;
      } else if (arg == "/a") {
        this.ApplyOption = true;
        isPIDParsing = false;
      } else if (isPIDParsing) {
        this.ProcessIDOption = true;
        this.ProcessID = uint.Parse(arg);
        isPIDParsing = false;
      } else {
        this.ProfilePathOption = true;
        /// この時点でフルパスに戻す
        var fullPath = Path.GetFullPath(arg);
        this.ProfilePath = fullPath;
      }
    }
  }

  /// (改行区切りstring[]をUnicodeでEncodeした)byte[]を解析
  private void ParseUnicodeData(byte[] data, int actualLength) {
    var rawData = Encoding.Unicode.GetString(data, 0, actualLength);
    var args = rawData.Split('\n');
    this.ParseArgs(args);
  }

  //-------------------------------------------------------------------

  /// 改行区切りのUnicode文字列に変換し、byte[]に変えて返す
  public byte[] ToUnicodeData() {
    var args = new List<string>();
    if (this.ProfilePathOption) {
      args.Add(this.ProfilePath);
    }
    if (this.ProcessIDOption) {
      args.Add("/p");
      args.Add(this.ProcessID.ToString());
    }
    if (this.SplashOption) {
      args.Add("/s");
    }
    if (this.ApplyOption) {
      args.Add("/a");
    }
    var rawData = string.Join<string>("\n", args);
    return Encoding.Unicode.GetBytes(rawData);
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// オプションが全く指定されていない
  public bool IsEmpty {
    get {
      return  !this.ProfilePathOption &&
              !this.ProcessIDOption &&
              !this.SplashOption &&
              !this.ApplyOption;
    }
  }

  //-----------------------------------------------------------------

  /// path: プロファイルのパスを指定
  public bool ProfilePathOption { get; private set; }
  /// プロファイルのパス(フルパス)
  public string ProfilePath { get; private set; }

  /// /p pid: プロセスIDを選択
  public bool ProcessIDOption { get; private set; }
  /// プロセスID
  public uint ProcessID { get; private set; }

  /// /s: スプラッシュ表示
  public bool SplashOption { get; private set; }
  
  /// /a: プロファイル適用
  public bool ApplyOption { get; private set; }
}
}   // namespace SCFF.Common
