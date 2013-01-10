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

/// @file SCFF.GUI/IUpdateByRuntimeOptions.cs
/// @copydoc SCFF::GUI::IUpdateByRuntimeOptions

namespace SCFF.GUI {

/// 自前RuntimeOptions->Controlデータバインディング用インタフェース
///
/// @attention どうしてもINotifyPropertyChangedなどの
///            リフレクションを使いたくなかったためやむなく用意した
interface IUpdateByRuntimeOptions {
  /// Control->RuntimeOptions可能にする
  bool IsEnabledByRuntimeOptions { get; }

  /// RuntimeOptions->Control書き換え
  void UpdateByRuntimeOptions();
}
}   // namespace SCFF.GUI
