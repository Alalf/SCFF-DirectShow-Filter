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

/// @file SCFF.Common/Profile/Validator.cs
/// @copydoc SCFF::Common::Profile::Validator

namespace SCFF.Common.Profile {

using System.Collections.Generic;
using System.Text;

  /// 検証エラータイプ
public enum ValidationErrorTypes {
  TargetWindowError,  ///< ターゲットWindowに関するエラー
  AreaError           ///< Clipping領域に関するエラー
}

/// 検証エラー
public struct ValidationError {
  /// コンストラクタ
  public ValidationError(ValidationErrorTypes type, string message) : this() {
    this.Type = type;
    this.Message = message;
  }

  /// 検証エラータイプ
  public ValidationErrorTypes Type { get; set; }
  /// エラーメッセージ
  public string Message { get; set; }
}

/// 検証エラーをまとめたクラス
public class ValidationErrors : List<ValidationError> {
  /// エラーが検出されなかった
  public bool IsNoError {
    get { return this.Count == 0; }
  }
  /// TargetWindowに関するエラーを追加
  public void AddTargetWindowError(string message) {
    var error = new ValidationError(ValidationErrorTypes.TargetWindowError, message);
    this.Add(error);
  }
  /// Areaに関するエラーを追加
  public void AddAreaError(string message) {
    var error = new ValidationError(ValidationErrorTypes.AreaError, message);
    this.Add(error);
  }
  /// エラーを文字列化
  public string ToErrorMessage(string header) {
    var errorMessage = new StringBuilder();
    if (header != null && header != string.Empty) errorMessage.AppendLine(header);
    foreach (var error in this) {
      errorMessage.AppendLine("  - " + error.Message);
    }
    return errorMessage.ToString();
  }
}

/// 検証メソッドをまとめたstaticクラス
public static class Validator {
  /// レイアウト要素単位での検証
  /// @param layoutElement 対象のレイアウト要素
  /// @param index レイアウト要素のインデックス
  /// @return 検証エラーリスト
  private static ValidationErrors ValidateLayoutElement(ILayoutElementView layoutElement, int index) {
    var result = new ValidationErrors();
    
    // TargetWindow
    if (!layoutElement.IsWindowValid) {
      var message = string.Format("Layout{0}: Specified window is invalid", index + 1);
      result.AddTargetWindowError(message);
      return result;
    }

    // Area
    if (!layoutElement.IsClippingParametersValid) {
      var message = string.Format("Layout{0}: Clipping parameters are invalid", index + 1);
      result.AddAreaError(message);
      return result;
    }

    return result;
  }

  /// プロファイル単位での検証
  /// @param profile 対象のプロファイル
  /// @return 検証エラーリスト
  public static ValidationErrors ValidateProfile(Profile profile) {
    var result = new ValidationErrors();
    int index = 0;
    foreach (var layoutElement in profile) {
      result.AddRange(Validator.ValidateLayoutElement(layoutElement, index));
      ++index;
    }
    return result;
  }
}
}   // namespace SCFF.Common.Profile
