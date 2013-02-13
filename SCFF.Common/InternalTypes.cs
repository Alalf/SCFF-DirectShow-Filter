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

/// @file SCFF.Common/InternalTypes.cs
/// SCFF.Interprocessの構造体をクラスに書き換え
/// @attention C#のStructは値型のため、Dictionaryから取り出すごとにコピーが発生する
///            これを回避するにはStructを使うのをやめClassで取り扱うしかない

namespace SCFF.Common {

using System;
using SCFF.Interprocess;

/// @copydoc SCFF::Interprocess::Entry
class InternalEntry {
  /// コピーコンストラクタ
  public InternalEntry(Entry interprocessEntry) {
    this.ProcessID = interprocessEntry.ProcessID;
    this.ProcessName = interprocessEntry.ProcessName;
    this.SampleWidth = interprocessEntry.SampleWidth;
    this.SampleHeight = interprocessEntry.SampleHeight;
    this.SamplePixelFormat = (ImagePixelFormats)interprocessEntry.SamplePixelFormat;
    this.FPS = interprocessEntry.FPS;
  }
  /// @copydoc SCFF::Interprocess::Entry::ProcessID
  public UInt32 ProcessID { get; set; }
  /// @copydoc SCFF::Interprocess::Entry::ProcessName
  public string ProcessName { get; set; }
  /// @copydoc SCFF::Interprocess::Entry::SampleWidth
  public int SampleWidth { get; set; }
  /// @copydoc SCFF::Interprocess::Entry::SampleHeight
  public int SampleHeight { get; set; }
  /// @copydoc SCFF::Interprocess::Entry::SamplePixelFormat
  public ImagePixelFormats SamplePixelFormat { get; set; }
  /// @copydoc SCFF::Interprocess::Entry::FPS
  public double FPS { get; set; }
}

/// @copydoc SCFF::Interprocess::LayoutParameter
class InternalLayoutParameter {
  /// コンストラクタ
  public InternalLayoutParameter() {
    this.SWScaleConfig = new InternalSWScaleConfig();
  }
  /// 変換
  public virtual LayoutParameter ToLayoutParameter() {
    LayoutParameter result;
    result.BoundX = this.BoundX;
    result.BoundY = this.BoundY;
    result.BoundWidth = this.BoundWidth;
    result.BoundHeight = this.BoundHeight;
    result.Window = this.Window.ToUInt64();
    result.ClippingX = this.ClippingX;
    result.ClippingY = this.ClippingY;
    result.ClippingWidth = this.ClippingWidth;
    result.ClippingHeight = this.ClippingHeight;
    result.ShowCursor = (Byte)(this.ShowCursor ? 1 : 0);
    result.ShowLayeredWindow = (Byte)(this.ShowLayeredWindow ? 1 : 0);
    result.SWScaleConfig = this.SWScaleConfig.ToSWScaleConfig();
    result.Stretch = (Byte)(this.Stretch ? 1 : 0);
    result.KeepAspectRatio = (Byte)(this.KeepAspectRatio ? 1 : 0);
    result.RotateDirection = (int)this.RotateDirection;
    return result;
  }
  /// @copydoc SCFF::Interprocess::LayoutParameter::BoundX
  public int BoundX { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::BoundY
  public int BoundY { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::BoundWidth
  public int BoundWidth { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::BoundHeight
  public int BoundHeight { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::Window
  public UIntPtr Window { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ClippingX
  public int ClippingX { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ClippingY
  public int ClippingY { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ClippingWidth
  public int ClippingWidth { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ClippingHeight
  public int ClippingHeight { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowCursor
  public bool ShowCursor { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::ShowLayeredWindow
  public bool ShowLayeredWindow { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::SWScaleConfig
  public InternalSWScaleConfig SWScaleConfig { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::Stretch
  public bool Stretch { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::KeepAspectRatio
  public bool KeepAspectRatio { get; set; }
  /// @copydoc SCFF::Interprocess::LayoutParameter::RotateDirection
  public RotateDirections RotateDirection { get; set; }
}

/// @copybrief SCFF::Interprocess::SWScaleConfig
class InternalSWScaleConfig {
  /// 変換
  public SWScaleConfig ToSWScaleConfig() {
    SWScaleConfig result;
    result.Flags = (int)this.Flags;
    result.AccurateRnd = (Byte)(this.AccurateRnd ? 1 : 0);
    result.IsFilterEnabled = (Byte)(this.IsFilterEnabled ? 1 : 0);
    result.LumaGblur = this.LumaGblur;
    result.ChromaGblur = this.ChromaGblur;
    result.LumaSharpen = this.LumaSharpen;
    result.ChromaSharpen = this.ChromaSharpen;
    result.ChromaHShift = this.ChromaHShift;
    result.ChromaVShift = this.ChromaVShift;
    return result;
  }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::Flags
  public SWScaleFlags Flags { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::AccurateRnd
  public bool AccurateRnd { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::IsFilterEnabled
  public bool IsFilterEnabled { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaGblur
  public float LumaGblur { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaGblur
  public float ChromaGblur { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::LumaSharpen
  public float LumaSharpen { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaSharpen
  public float ChromaSharpen { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaHShift
  public float ChromaHShift { get; set; }
  /// @copydoc SCFF::Interprocess::SWScaleConfig::ChromaVShift
  public float ChromaVShift { get; set; }
}
}   // namespace SCFF.Common
