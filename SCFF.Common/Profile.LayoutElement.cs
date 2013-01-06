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

/// @file SCFF.Common/Profile.LayoutElement.cs
/// プロファイル内を参照・操作するためのカーソルクラス(仮想)

namespace SCFF.Common {

public partial class Profile {
  /// プロファイル内を参照・操作するためのカーソルクラス(仮想)
  /// 
  /// - C#のインナークラスはC++のフレンドクラスと似たようなことができる！
  /// - プログラムから直接は利用してはいけないもの(this.profile.appendicesの内容で上書きされるため)
  ///   - this.profile.message.layoutParameters[*].Bound*
  ///   - this.profile.message.layoutParameters[*].Clipping*
  /// - 以下の内容も最新のデータがあることは保障しない
  ///   - this.profile.message.layoutParameters[*].Window
  ///
  /// - ProfileはProcessに関連した情報を知ることはできない
  ///   - よってsampleWidth/sampleHeightの存在は仮定しないこと
  public abstract class LayoutElement {

    /// コンストラクタ
    public LayoutElement(Profile profile, int index) {
      this.profile = profile;
      this.index = index;
    }

    // プロパティ

    /// Index
    public int Index {
      get { return this.index; }
    }

    // フィールド
    private int index;          ///< Index
    protected Profile profile;  ///< 捜査対象のProfile
  }
}
}   // namespace SCFF.Common
