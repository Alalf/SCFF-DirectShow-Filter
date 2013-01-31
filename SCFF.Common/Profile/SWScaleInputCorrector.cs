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

/// @file SCFF.Common/Profile/SWScaleInputCorrector.cs
/// @copydoc SCFF::Common::Profile::SWScaleInputCorrector

namespace SCFF.Common.Profile {

using System.Diagnostics;

/// SWScale*の入力エラーを判定するためのメソッドを集めたstaticクラス
/// @attention Decoratorパターンにすべきではない(newが多くなりすぎるので)
public static class SWScaleInputCorrector {
  //=================================================================
  // 列挙型
  //=================================================================

  /// LayoutElement.SWScale*の要素を表す列挙型
  public enum Names {
    LumaGBlur,
    LumaSharpen,
    ChromaHShift,
    ChromaGBlur,
    ChromaSharpen,
    ChromaVShift
  }

  //=================================================================
  // 訂正
  //=================================================================

  /// レイアウト要素のSWScale*の変更を試みる
  public static bool TryChange(Names target, float value, out float changed) {
    float lowerBound = 0.0F;
    float upperBound = 0.0F;
    switch (target) {
      case Names.LumaGBlur:
      case Names.ChromaGBlur: {
        lowerBound = 0.0F;
        upperBound = 2.0F;
        break;
      }
      case Names.LumaSharpen:
      case Names.ChromaSharpen: {
        lowerBound = 0.0F;
        upperBound = 4.0F;
        break;
      }
      case Names.ChromaHShift:
      case Names.ChromaVShift: {
        lowerBound = 0.0F;
        upperBound = 1.0F;
        break;
      }
      default: Debug.Fail("switch"); throw new System.ArgumentException();
    }
    
    /// @attention 浮動小数点数の比較
    if (value < lowerBound) {
      changed = lowerBound;
      return false;
    } else if (upperBound < value) {
      changed = upperBound;
      return false;
    } else {
      changed = value;
      return true;
    }
  }
}
}   // namespace SCFF.Common.Profile
