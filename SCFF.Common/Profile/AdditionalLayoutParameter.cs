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

/// @file SCFF.Common/Profile/AdditionalLayoutParameter.cs
/// @copydoc SCFF::Common::Profile::AdditionalLayoutParameter

namespace SCFF.Common.Profile {

/// SCFF.Interprocess.LayoutParameter以外のレイアウト要素に必要なデータ
public class AdditionalLayoutParameter {
  /// Windowタイプ: 標準状態 or 最小化 or 最大化
  public WindowTypes WindowType { get; set; }
  /// Window内容を示す文字列(≒Class名)
  public string WindowCaption { get; set; }

  /// クリッピング領域を自動的にウィンドウサイズに合わせるか
  public bool Fit { get; set; }
  /// 相対座標系でのレイアウト要素左上端のX座標
  public double BoundRelativeLeft { get; set; }
  /// 相対座標系でのレイアウト要素左上端のY座標
  public double BoundRelativeTop { get; set; }
  /// 相対座標系でのレイアウト要素右下端のX座標
  public double BoundRelativeRight { get; set; }
  /// 相対座標系でのレイアウト要素右下端のY座標
  public double BoundRelativeBottom { get; set; }

  /// Fit=Falseの時のクリッピング領域左上端のX座標
  public int ClippingXWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域左上端のY座標
  public int ClippingYWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域の幅
  public int ClippingWidthWithoutFit { get; set; }
  /// Fit=Falseの時のクリッピング領域の高さ
  public int ClippingHeightWithoutFit { get; set; }

  /// 保存用の値が設定されたか
  public bool HasBackedUp { get; set; }
  /// 保存用: Screen座標系でのクリッピング領域左上端のX座標
  public int BackupScreenClippingX { get; set; }
  /// 保存用: Screen座標系でのクリッピング領域左上端のY座標
  public int BackupScreenClippingY { get; set; }
  /// 保存用: クリッピング領域の幅
  public int BackupClippingWidth { get; set; }
  /// 保存用: クリッピング領域の高さ
  public int BackupClippingHeight { get; set; }
}
}   // namespace SCFF.Common.Profile
