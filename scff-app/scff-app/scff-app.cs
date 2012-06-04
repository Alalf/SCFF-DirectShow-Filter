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

/// @file scff-app/scff-app.cs
/// @brief MVCパターンにおけるControllerの定義

namespace scff_app {

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

/// @brief SCFFAppForm(メインウィンドウ)から利用する実装クラス
partial class SCFFApp {

  // 定数
  public const int kDefaultBoundWidth = 640;
  public const int kDefaultBoundHeight = 360;
  public const int kMaxLayoutElements = scff_interprocess.Interprocess.kMaxComplexLayoutElements;
  const string kSCFFSourceGUID = "D64DB8AA-9055-418F-AFE9-A080A4FAE47A";
  const string kRegistryKey = "CLSID\\{" + kSCFFSourceGUID + "}";

  const string kDefaultProfileName = "DEFAULT";
  const string kSeparator = "----------------------";

  /// @brief ResizeMethodコンボボックス用リスト
  static SortedList<scff_interprocess.SWScaleFlags, string> kResizeMethodSortedList =
      new SortedList<scff_interprocess.SWScaleFlags, string> {
    {scff_interprocess.SWScaleFlags.kFastBilinear, "FastBilinear (fast bilinear)"},
    {scff_interprocess.SWScaleFlags.kBilinear, "Bilinear (bilinear)"},
    {scff_interprocess.SWScaleFlags.kBicubic, "Bicubic (bicubic)"},
    {scff_interprocess.SWScaleFlags.kX, "X (experimental)"},
    {scff_interprocess.SWScaleFlags.kPoint, "Point (nearest neighbor)"},
    {scff_interprocess.SWScaleFlags.kArea, "Area (averaging area)"},
    {scff_interprocess.SWScaleFlags.kBicublin, "Bicublin (luma bicubic, chroma bilinear)"},
    {scff_interprocess.SWScaleFlags.kGauss, "Gauss (gaussian)"},
    {scff_interprocess.SWScaleFlags.kSinc, "Sinc (sinc)"},
    {scff_interprocess.SWScaleFlags.kLanczos, "Lanczos (lanczos)"},
    {scff_interprocess.SWScaleFlags.kSpline, "Spline (natural bicubic spline)"}
  };

  //-------------------------------------------------------------------

  /// @brief コンストラクタ
  public SCFFApp(BindingSource entries, BindingSource layout_parameters) {
    interprocess_ = new scff_interprocess.Interprocess();
    directory_ = new viewmodel.Directory();
    message_ = new viewmodel.Message();

    // データソースを設定
    entries_ = entries;
    layout_parameters_ = layout_parameters;
    entries.DataSource = directory_.Entries;
    layout_parameters.DataSource = message_.LayoutParameters;

    resize_method_list_ =
        new List<KeyValuePair<scff_interprocess.SWScaleFlags,string>>(kResizeMethodSortedList);

    profiles_path_ = Application.UserAppDataPath + "\\profiles\\";
    profile_list_ = new BindingList<string>();
    known_types_ = new List<Type> {
      typeof(viewmodel.LayoutParameter)
    };
  }

  /// @brief メインフォームのLoad時に呼ばれることを想定
  public void OnLoad() {
    // ディレクトリとBindingSourceを共有メモリから更新
    UpdateDirectory();

    // プロファイルリストを読み込む
    UpdateProfileList();
    LoadDefaultProfile();
  }

  /// @brief 起動時のチェックを行うメソッド
  public bool CheckEnvironment() {
    //------------------
    // 32bit版のチェック
    //------------------
    bool is_correctly_installed_x86 = false;
    bool is_dll_found_x86 = false;
    string dll_path_x86 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry32).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_x86 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_x86 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_x86)) {
        is_dll_found_x86 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //------------------
    // 64bit版のチェック
    //------------------
    bool is_correctly_installed_amd64 = false;
    bool is_dll_found_amd64 = false;
    string dll_path_amd64 = "";
    try {
      RegistryKey scff_dsf_key =
          RegistryKey.OpenBaseKey(
              RegistryHive.ClassesRoot,
              RegistryView.Registry64).OpenSubKey(kRegistryKey);
      if (scff_dsf_key != null) {
        is_correctly_installed_amd64 = true;
      }

      RegistryKey scff_dsf_path_key = scff_dsf_key.OpenSubKey("InprocServer32");
      dll_path_amd64 = scff_dsf_path_key.GetValue("").ToString();
      if (File.Exists(dll_path_amd64)) {
        is_dll_found_amd64 = true;
      }
    } catch {
      // 念のためエラーが出た場合も考慮
    }

    //----------------------
    // エラーダイアログの表示
    // （若干不正確だがないよりましだろう）
    //----------------------
    if (!is_correctly_installed_x86 && !is_correctly_installed_amd64) {
      // 32bit版も64bit版もインストールされていない場合
      MessageBox.Show("scff-*.ax is not correctly installed.\nPlease re-install SCFF DirectShow Filter.",
                      "Not correctly installed",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return false;
    }

    if (!is_dll_found_x86 && !is_dll_found_amd64) {
      // 32bit版のDLLも64bit版のDLLも指定された場所に存在していない場合
      string message = "scff-*.ax is not found:\n";
      message += "\n";
      message += "  32bit: " + dll_path_x86 + "\n";
      message += "  64bit: " + dll_path_amd64 + "\n"; 
      message += "\n";
      message += "Check your SCFF directory.";
      MessageBox.Show(message,
                      "DLL is not found",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
      return false;
    }

    //------------------
    // カラーチェック
    //------------------
    if (Screen.PrimaryScreen.BitsPerPixel != 32) {
      MessageBox.Show("SCFF requires primary screen is configured 32bit color mode.",
                      "Not 32bit color mode",
                      MessageBoxButtons.OK, MessageBoxIcon.Error);
      return false;
    }

    // 起動OK
    return true;
  }

  //-------------------------------------------------------------------
  // UI要素作成用
  //-------------------------------------------------------------------

  public List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> ResizeMethodList {
    get { return resize_method_list_; }
  }

  //-------------------------------------------------------------------
  // Directory
  //-------------------------------------------------------------------

  public void UpdateDirectory() {
    // 共有メモリにアクセス
    interprocess_.InitDirectory();
    scff_interprocess.Directory interprocess_directory;
    interprocess_.GetDirectory(out interprocess_directory);

    // Directoryに設定
    directory_.LoadFromInterprocess(interprocess_directory);
  }

  //-------------------------------------------------------------------
  // Message
  //-------------------------------------------------------------------

  void Send(bool show_message, bool force_null_layout) {
    if (entries_.Count == 0) {
      // 書き込み先が存在しない
      if (show_message) {
        MessageBox.Show("No process to send message.");
      }
      return;
    }

    viewmodel.Entry current_entry = (viewmodel.Entry)entries_.Current;

    try {
      /// @warning DWORD->int変換！オーバーフローの可能性あり
      Process.GetProcessById((int)current_entry.ProcessID);
    } catch {
      // プロセスが存在しない場合
      if (show_message) {
        MessageBox.Show("Cannot find process(" + current_entry.ProcessID + ").");
      }
      return;
    }

    // Messageを変換
    scff_interprocess.Message interprocess_message =
        message_.ToInterprocess(current_entry.SampleWidth, current_entry.SampleHeight, force_null_layout);
    
    // 共有メモリにアクセス
    interprocess_.InitMessage(current_entry.ProcessID);
    interprocess_.SendMessage(interprocess_message);
  }

  public void SendNull(bool show_message) {
    Send(show_message, true);
  }

  public void SendMessage(bool show_message) {
    if (!message_.IsValid()) {
      if (show_message) {
        MessageBox.Show(message_.Error, "Layout Parameters Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
      return;
    }
    Send(show_message, false);
  }

  //-------------------------------------------------------------------
  // Window
  //-------------------------------------------------------------------

  public void SetDesktopWindow() {
    ((viewmodel.LayoutParameter)layout_parameters_.Current).SetWindow(ExternalAPI.GetDesktopWindow());
  }

  public void SetWindowFromPoint(int screen_x, int screen_y) {
    UIntPtr window = ExternalAPI.WindowFromPoint(screen_x, screen_y);
    ((viewmodel.LayoutParameter)layout_parameters_.Current).SetWindow(window);
  }

  //-------------------------------------------------------------------
  // Profile
  //-------------------------------------------------------------------

  void UpdateProfileList() {
    profile_list_.Clear();

    // DEFAULTと罫線を追加する
    profile_list_.Add(kDefaultProfileName);
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

  scff_interprocess.Interprocess interprocess_;
  viewmodel.Directory directory_;
  viewmodel.Message message_;

  BindingSource layout_parameters_;
  BindingSource entries_;

  List<KeyValuePair<scff_interprocess.SWScaleFlags,string>> resize_method_list_;

  readonly string profiles_path_;
  BindingList<string> profile_list_;
  List<Type> known_types_;
}
}   // namespace scff_app