// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

namespace SCFF.GUI {

using System.Windows;
using SCFF.Common;

/// App.xaml の相互作用ロジック
public partial class App : Application {

  /// アプリケーションの設定を格納するインスタンス
  public static Options Options {
    get { return options; }
  }
  private static Options options = new Options();

  /// 現在編集中のプロファイルを格納するインスタンス
  public static Profile Profile {
    get { return profile; }
  }
  private static Profile profile = new Profile();
}
}
