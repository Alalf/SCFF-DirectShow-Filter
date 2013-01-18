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

/// @file SCFF.Common/Profile/ValidationErrors.cs
/// @copydoc SCFF::Common::Profile::ValidationErrors

namespace SCFF.Common.Profile {

using System.Collections.Generic;

/// 検証エラータイプ
public enum ValidationErrorTypes {
  TargetWindowError,
  AreaError
}

/// 検証エラー
public struct ValidationError {
  public ValidationErrorTypes Type { get; set; }
  public string Message { get; set; }
}

/// 検証エラーをまとめたクラス
public class ValidationErrors {
  // コンストラクタ
  public ValidationErrors() {
    // nop
  }

  /// Clear
  public void Clear() {
    this.errors.Clear();
  }

  /// Add
  public void Add(int index, ValidationError error) {
    this.errors.Add(index, error);
  }

  /// 参照
  /// @return エラーが無ければnullがかえる
  public ValidationError GetError(int index) {
    ValidationError result;
    this.errors.TryGetValue(index, out result);
    return result;
  }

  /// エラーがないかどうか
  public bool IsNoError {
    get { return this.errors.Count == 0; }
  }

  /// Index->ValidationErrorのディクショナリ
  private Dictionary<int,ValidationError> errors =
      new Dictionary<int,ValidationError>();
}
}   // namespace SCFF.Common.Profile
