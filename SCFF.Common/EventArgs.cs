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

/// @file SCFF.Common/EventArgs.cs
/// SCFF.Commonで利用するEventArgsと関連する列挙型

namespace SCFF.Common {

using System;

//=====================================================================
// 列挙型
//=====================================================================

/// Profileの保存・別名で保存のどちらかを示す列挙型
public enum SaveActions {
  Save,     ///< 保存
  SaveAs,   ///< 別名で保存
}

/// Close時の選択(保存/破棄/キャンセル)を表す列挙形
public enum CloseActions {
  Save,     ///< 保存
  Abandon,  ///< 破棄
  Cancel    ///< キャンセル
}

//=====================================================================
// EventArgs
//=====================================================================

/// SCFF DirectShow Filterでエラー発生時用EventArgs
public class DSFErrorOccuredEventArgs : EventArgs {
  /// コンストラクタ
  public DSFErrorOccuredEventArgs(UInt32 processID) {
    this.ProcessID = processID;
  }
  /// プロパティ: [in] エラーが発生したDSFがロードされているプロセスID
  public uint ProcessID { get; private set; }
}

/// エラー発生時用EventArgs
public class ErrorOccuredEventArgs : EventArgs {
  /// コンストラクタ
  public ErrorOccuredEventArgs(string message, bool quiet) {
    this.Message = message;
    this.Quiet = quiet;
  }
  /// プロパティ: [in] メッセージ
  public string Message { get; private set; }
  /// プロパティ: [in] メッセージ表示用フラグ
  public bool Quiet { get; private set; }
}

/// Profileを[閉じる]時用EventArgs
public class ClosingProfileEventArgs : EventArgs {
  /// コンストラクタ
  public ClosingProfileEventArgs(CloseActions action, string profileName) {
    this.Action = action;
    this.ProfileName = profileName;
  }
  /// プロパティ: [in/out] Closeアクション
  public CloseActions Action { get; set; }
  /// プロパティ: [in] プロファイル名
  public string ProfileName { get; private set; }
}

/// Profileを[開く]時用EventArgs
public class OpeningProfileEventArgs : EventArgs {
  /// コンストラクタ
  public OpeningProfileEventArgs(string path, string initialDirectory) {
    // 初期値＝キャンセルしない
    this.Cancel = false;

    this.Path = path;
    this.InitialDirectory = initialDirectory;
  }
  /// プロパティ: [out] 読み込みをキャンセル
  public bool Cancel { get; set; }
  /// プロパティ: [in/out] プロファイルのパス
  public string Path { get; set; }
  /// プロパティ: [in] パスを設定するために使える初期ディレクトリ
  public string InitialDirectory { get; private set; }
}

/// Profileを[保存]時用EventArgs
public class SavingProfileEventArgs : EventArgs {
  /// コンストラクタ
  public SavingProfileEventArgs(SaveActions action, string path, string fileName, string initialDirectory) {
    // 初期値＝キャンセルしない
    this.Cancel = false;

    this.Action = action;
    this.Path = path;
    this.FileName = fileName;
    this.InitialDirectory = initialDirectory;
  }
  /// プロパティ: [out] 保存をキャンセル
  public bool Cancel { get; set; }
  /// プロパティ: [in] Saveアクション
  public SaveActions Action { get; private set; }
  /// プロパティ: [in/out] プロファイルのパス
  public string Path { get; set; }
  /// プロパティ: [in] ファイル名
  public string FileName { get; private set; }
  /// プロパティ: パスを設定するために使える初期ディレクトリ
  public string InitialDirectory { get; set; }
}
}   // namespace SCFF.Common
