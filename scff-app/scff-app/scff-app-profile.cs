// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
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

/// @file scff-app/scff-ap-profile.cs
/// @brief SCFFAppのプロファイル機能関連のメソッドの定義

namespace scff_app {

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;

/// @brief MVCパターンにおけるController
partial class SCFFApp {

  // 定数
  const string kDefaultProfileName = "[Default]";
  const string kLastProfileName = "[Last]";
  const string kSeparator = "----------------------";

  //-------------------------------------------------------------------
  // Profile
  //-------------------------------------------------------------------

  void UpdateProfileList() {
    profile_list_.Clear();

    // DEFAULT、前回終了時設定、罫線を追加する
    profile_list_.Add(kDefaultProfileName);
    // profile_list_.Add(kLastProfileName);
    profile_list_.Add(kSeparator);

    // プロファイルディレクトリが存在するかを確認する
    if (!Directory.Exists(profiles_path_)) {
      return;
    }

    // ディレクトリからプロファイル一覧を取得する
    string[] profile_filename_list = Directory.GetFiles(profiles_path_);
    foreach (string filename in profile_filename_list) {
      profile_list_.Add(Path.GetFileNameWithoutExtension(filename));
    }
  }

  void LoadDefaultProfile() {
    // DataSourceの更新
    layout_parameters_.Clear();
    layout_parameters_.AddNew();
  }

  void LoadCustomProfile(string profile_name) {
    // XMLファイルから読み込み
    string profile_file_path = profiles_path_ + profile_name + ".xml";
    object[] loaded_array;

    using (FileStream file = new FileStream(profile_file_path, FileMode.Open))
    using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(file, XmlDictionaryReaderQuotas.Max)) {
      DataContractSerializer serializer = new DataContractSerializer(typeof(IList), known_types_);
      loaded_array = (object[])serializer.ReadObject(reader);
    }

    // DataSourceの更新
    layout_parameters_.Clear();
    foreach (viewmodel.LayoutParameter i in loaded_array) {
      layout_parameters_.Add(i);
    }
  }

  public bool ValidProfileName(string profile_name) {
    if (profile_name == "" || profile_name == kSeparator) {
      // プロファイル名が不正
      return false;
    }

    if (profile_name == kDefaultProfileName) {
      // デフォルトなのでカスタムではない
      return false;
    }

    return true;
  }

  public bool LoadProfile(string profile_name) {
    if (profile_name == "" || profile_name == kSeparator) {
      // プロファイル名が不正
      return false;
    }

    if (profile_name == kDefaultProfileName) {
      // デフォルトを読み込む
      LoadDefaultProfile();
      return true;
    }

    if (!profile_list_.Contains(profile_name)) {
      // カスタムプロファイルが存在しない
      return false;
    }

    LoadCustomProfile(profile_name);
    return true;
  }

  public bool AddProfile(string profile_name) {
    if (profile_name == "" || profile_name == kSeparator) {
      // プロファイル名が不正
      return false;
    }

    if (profile_name == kDefaultProfileName) {
      // デフォルトは置き換えられない
      return false;
    }

    // ディレクトリ生成
    Directory.CreateDirectory(profiles_path_);

    // XMLファイルを書き込み
    string profile_file_path = profiles_path_ + profile_name + ".xml";
    using (FileStream file = new FileStream(profile_file_path, FileMode.Create))
    using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(file)) {
      DataContractSerializer serializer = new DataContractSerializer(typeof(IList), known_types_);
      serializer.WriteObject(writer, layout_parameters_);
    }

    if (!profile_list_.Contains(profile_name)) {
      // リストにない場合だけ追加する
      profile_list_.Add(profile_name);
    }

    return true;
  }

  public bool RemoveProfile(string profile_name) {
    if (profile_name == "" || profile_name == kSeparator) {
      // プロファイル名が不正
      return false;
    }

    if (profile_name == kDefaultProfileName) {
      // デフォルトは消せない
      return false;
    }

    if (!profile_list_.Contains(profile_name)) {
      // リストにない場合はなにもしない
      return false;
    }

    // ファイルを消す
    string profile_file_path = profiles_path_ + profile_name + ".xml";
    File.Delete(profile_file_path);

    // リストからも消す
    profile_list_.Remove(profile_name);

    return true;
  }

  public BindingList<string> ProfileList {
    get { return profile_list_; }
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  readonly string profiles_path_;
  BindingList<string> profile_list_;
  List<Type> known_types_;
}
}   // namespace scff_app