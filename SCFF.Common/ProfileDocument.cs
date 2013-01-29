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

/// @file SCFF.Common/OptionsINIFile.cs
/// @copydoc SCFF::Common::OptionsINIFile

namespace SCFF.Common {

using System.IO;
using SCFF.Common.Profile;

/// Options,RuntimeOptions.ProfileをまとめるFacadeクラス
/// @warning Facadeクラスなのでメンバは参照のみに限ること
public class ProfileDocument {
  //===================================================================
  // コンストラクタ
  //===================================================================

  /// コンストラクタ
  public ProfileDocument(Options options,
                         RuntimeOptions runtimeOptions,
                         Profile.Profile profile) {
    this.Options = options;
    this.RuntimeOptions = runtimeOptions;
    this.Profile = profile;
  }

  //===================================================================
  // メソッド
  //===================================================================

  public void Init() {
    if (this.Options.RestoreLastProfile) {
      var lastProfilePath = this.Options.GetRecentProfile(0);
      if (lastProfilePath != string.Empty && System.IO.File.Exists(lastProfilePath)) {
        this.Open(lastProfilePath);
        return;
      }
    }
    
    this.New();
  }

  /// プロファイル新規作成
  public void New() {
    // Profile
    this.Profile.RestoreDefault();

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = string.Empty;
    this.RuntimeOptions.ProfileName = string.Empty;
    this.RuntimeOptions.LastSavedTimestamp = -1L;
    this.RuntimeOptions.LastAppliedTimestamp = -1L;
  }

  /// プロファイルの保存
  public bool Save(string path) {
    // Profile
    var result = ProfileINIFile.Save(this.Profile, path);
    if (!result) return false;

    // Options
    this.Options.AddRecentProfile(path);

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = path;
    this.RuntimeOptions.ProfileName = Path.GetFileNameWithoutExtension(path);
    this.RuntimeOptions.LastSavedTimestamp = this.Profile.Timestamp;

    return true;
  }

  /// プロファイルを開く
  public bool Open(string path) {
    // Profile
    var result = ProfileINIFile.Load(this.Profile, path);
    if (!result) return false;

    // Options
    this.Options.AddRecentProfile(path);

    // RuntimeOptions
    this.RuntimeOptions.ProfilePath = path;
    this.RuntimeOptions.ProfileName = Path.GetFileNameWithoutExtension(path);
    this.RuntimeOptions.LastSavedTimestamp = this.Profile.Timestamp;
    this.RuntimeOptions.LastAppliedTimestamp = -1L;

    return true;
  }

  //===================================================================
  // プロパティ
  //===================================================================

  /// 現在編集中のProfileがファイルに保存されているかかどうか
  public bool HasSaved {
    get {
      return (this.RuntimeOptions.ProfilePath != null &&
              this.RuntimeOptions.ProfilePath != string.Empty);
    }
  }

  /// プロファイルが変更されたか
  public bool HasModified {
    get {
      return (this.RuntimeOptions.LastSavedTimestamp != this.Profile.Timestamp);
    }
  }

  /// プロファイルが前回のApply移行に変更されたか
  public bool HasModifiedFromLastApply {
    get {
      return (this.RuntimeOptions.LastAppliedTimestamp != this.Profile.Timestamp);
    }
  }

  /// 現在の状態を文字列にして返す
  public string Title {
    get {
      var title = Constants.SCFFVersion;
      if (this.HasSaved) {
        title = string.Format("{0}{1} - {2}",
            this.RuntimeOptions.ProfileName,
            this.HasModified ? "*" : "",
            Constants.SCFFVersion);
      }
      return title;
    }
  }

  /// ファイル名
  public string FileName {
    get {
      return this.HasSaved ? this.RuntimeOptions.ProfileName : "Untitled";
    }
  }

  //-------------------------------------------------------------------

  private Options Options { get; set; }
  private RuntimeOptions RuntimeOptions { get; set; }
  private Profile.Profile Profile { get; set; }
}
}   // namespace SCFF.Common
