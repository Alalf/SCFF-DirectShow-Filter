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

/// @file SCFF.Common/Types.cs
/// SCFF.Commonモジュールで利用する型

namespace SCFF.Common {

/// Options用WindowState。System.Windows.WindowStateと相互に変換する。
public enum WindowState {
  Normal,
  Minimized,
  Maximized
}

/// Profile用Windowの種類
public enum WindowTypes {
  Normal,
  DesktopListView,
  Desktop,
}

/// @copydoc SCFF::Interprocess::LayoutTypes
public enum LayoutTypes {
  NullLayout    = Interprocess.LayoutTypes.NullLayout,
  NativeLayout  = Interprocess.LayoutTypes.NativeLayout,
  ComplexLayout = Interprocess.LayoutTypes.ComplexLayout
}

/// @copydoc SCFF::Interprocess::SWScaleFlags
public enum SWScaleFlags {
  FastBilinear  = Interprocess.SWScaleFlags.FastBilinear,
  Bilinear      = Interprocess.SWScaleFlags.Bilinear,
  Bicubic       = Interprocess.SWScaleFlags.Bicubic,
  X             = Interprocess.SWScaleFlags.X,
  Point         = Interprocess.SWScaleFlags.Point,
  Area          = Interprocess.SWScaleFlags.Area,
  Bicublin      = Interprocess.SWScaleFlags.Bicublin,
  Gauss         = Interprocess.SWScaleFlags.Gauss,
  Sinc          = Interprocess.SWScaleFlags.Sinc,
  Lanczos       = Interprocess.SWScaleFlags.Lanczos,
  Spline        = Interprocess.SWScaleFlags.Spline
}

/// @copydoc SCFF::Interprocess::RotateDirections
public enum RotateDirections {
  NoRotate      = Interprocess.RotateDirections.NoRotate,
  Degrees90     = Interprocess.RotateDirections.Degrees90,
  Degrees180    = Interprocess.RotateDirections.Degrees180,
  Degrees270    = Interprocess.RotateDirections.Degrees270,
}

/// 追加レイアウトパラメータ
public class AdditionalLayoutParameter {
  /// ウィンドウタイプ: 標準状態 or 最小化 or 最大化
  public WindowTypes WindowType { get; set; }
  /// ウィンドウ内容を示す文字列(クラス名)
  public string WindowCaption { get; set; }

  /// クリッピング領域を自動的にウィンドウサイズに合わせるか
  public bool Fit { get; set; }
  /// レイアウト要素が配置される左上端の相対座標(x)
  public double BoundRelativeLeft { get; set; }
  /// レイアウト要素が配置される左上端の相対座標(y)
  public double BoundRelativeTop { get; set; }
  /// レイアウト要素が配置される右下端の相対座標(x)
  public double BoundRelativeRight { get; set; }
  /// レイアウト要素が配置される右下端の相対座標(y)
  public double BoundRelativeBottom { get; set; }

  /// Fitオプションを考慮したクリッピング領域の左上端の座標(x)
  public int ClippingXWithoutFit { get; set; }
  /// Fitオプションを考慮したクリッピング領域の左上端の座標(y)
  public int ClippingYWithoutFit { get; set; }
  /// Fitオプションを考慮したクリッピング領域の幅
  public int ClippingWidthWithoutFit { get; set; }
  /// Fitオプションを考慮したクリッピング領域の高さ
  public int ClippingHeightWithoutFit { get; set; }

  /// 保存用: クリッピング領域の左上端のスクリーン座標(x)
  public int BackupDesktopClippingX { get; set; }
  /// 保存用: クリッピング領域の左上端のスクリーン座標(y)
  public int BackupDesktopClippingY { get; set; }
  /// 保存用: クリッピング領域の幅
  public int BackupDesktopClippingWidth { get; set; }
  /// 保存用: クリッピング領域の高さ
  public int BackupDesktopClippingHeight { get; set; }
}
}   // namespace SCFF.Common
