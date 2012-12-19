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

/// SCFF.*で利用する共通クラスをまとめた名前空間
using System;
using System.Diagnostics;
namespace SCFF.Common {

/// Profileをまとめたクラス。ここからデータを切り出してDSFに送ったりする。
public class Profile {

  //===================================================================
  // 定数
  //===================================================================

  private const string ProfilePathPrefix = "SCFF.Common.Profile.";
  private const string ProfilePathSuffix = ".ini";

  // ファイル入出力に用いるキー
  public enum Key {
    
  }

  public enum WindowType {
    Normal,
    Desktop,
    Root,
  }

  //===================================================================
  // アクセス変数
  // 本当は配列を返したいのだが、残念だがそれは非推奨なのです
  //===================================================================
  private const int CurrentIndexAlias = -1;

  public int BoundX(int index = CurrentIndexAlias) {
    if (index == CurrentIndexAlias) {
      index = currentIndex;
    }
    Debug.Assert(0 <= index &&
        index < Interprocess.Interprocess.MaxComplexLayoutElements);
    return message.LayoutParameters[index].BoundX;
  }
  public int BoundY(int index = CurrentIndexAlias) {
    if (index == CurrentIndexAlias) {
      index = currentIndex;
    }
    Debug.Assert(0 <= index &&
        index < Interprocess.Interprocess.MaxComplexLayoutElements);
    return message.LayoutParameters[index].BoundY;
  }

  //===================================================================
  // メンバ変数
  //===================================================================

  // 現在編集対象のインデックス
  private int currentIndex = 0;

  // 大半の情報はこのmessage内部にあるが、messageの中身は実行時のみ有効な値が含まれている
  // (UIntPtr windowなど)。このために、messageとは別にいくらかの情報を追加して保存しなければならない。
  private Interprocess.Message message = new Interprocess.Message();

  // そのほかのデータをまとめたもの
  public class Appendix {
    public bool Fit { get; set; }
    public double RelativeBoundX { get; set; }
    public double RelativeBoundY { get; set; }
    public double RelativeBoundWidth { get; set; }
    public double RelativeBoundHeight { get; set; }
    public int DesktopClippingX { get; set; }
    public int DesktopClippingY { get; set; }
    public int DesktopClippingWidth { get; set; }
    public int DesktopClippingHeight { get; set; }
    public int RootClippingX { get; set; }
    public int RootClippingY { get; set; }
    public int RootClippingWidth { get; set; }
    public int RootClippingHeight { get; set; }

    public Appendix() {
      this.Fit = true;
      this.RelativeBoundX = 0.0;
      this.RelativeBoundY = 0.0;
      this.RelativeBoundWidth = 1.0;
      this.RelativeBoundHeight = 1.0;
      this.DesktopClippingX = -1;
      this.DesktopClippingY = -1;
      this.DesktopClippingWidth = -1;
      this.DesktopClippingHeight = -1;
      this.RootClippingX = -1;
      this.RootClippingY = -1;
      this.RootClippingWidth = -1;
      this.RootClippingHeight = -1;
    }
  }
  private Appendix[] appendixes = new Appendix[Interprocess.Interprocess.MaxComplexLayoutElements];
}
}
