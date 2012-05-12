
// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF DSF.
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

/// @file scff-app/data-classes.cs
/// @brief scff-appで利用するマネージドデータクラス

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace scff_app {

/// @brief scff_inteprocess.Entryをマネージドクラス化したクラス
public class Entry {
  /// @brief コンストラクタ。引数はInterprocessの構造体。
  public Entry(scff_interprocess.Entry interprocess_entry) {
    ProcessID = interprocess_entry.process_id;
    ProcessName = interprocess_entry.process_name;
    switch (interprocess_entry.sample_pixel_format) {
    case (System.Int32)scff_interprocess.ImagePixelFormat.kI420:
      SamplePixelFormat = scff_interprocess.ImagePixelFormat.kI420;
      break;
    case (System.Int32)scff_interprocess.ImagePixelFormat.kUYVY:
      SamplePixelFormat = scff_interprocess.ImagePixelFormat.kUYVY;
      break;
    case (System.Int32)scff_interprocess.ImagePixelFormat.kRGB0:
      SamplePixelFormat = scff_interprocess.ImagePixelFormat.kRGB0;
      break;
    }
    SampleWidth = interprocess_entry.sample_width;
    SampleHeight = interprocess_entry.sample_height;
    FPS = interprocess_entry.fps;
  }
  /// @brief Interprocessで利用可能な構造体に変換
  public scff_interprocess.Entry ToInterprocessEntry() {
    scff_interprocess.Entry interprocess_entry = new scff_interprocess.Entry();
    interprocess_entry.process_id = ProcessID;
    interprocess_entry.process_name = ProcessName;
    interprocess_entry.sample_pixel_format = (System.Int32)SamplePixelFormat;
    interprocess_entry.sample_width = SampleWidth;
    interprocess_entry.sample_height = SampleHeight;
    interprocess_entry.fps = FPS;
    return interprocess_entry;
  }
  /// @brief 文字列に変換
  public override string ToString() {
    string pixel_format_string;
    switch (SamplePixelFormat) {
    case scff_interprocess.ImagePixelFormat.kI420:
      pixel_format_string = "I420";
      break;
    case scff_interprocess.ImagePixelFormat.kUYVY:
      pixel_format_string = "UYVY";
      break;
    case scff_interprocess.ImagePixelFormat.kRGB0:
      pixel_format_string = "RGB0";
      break;
    default:
      pixel_format_string = "(invalid)";
      break;
    }

    return "[" + ProcessID + "] " + ProcessName +
            " (" + pixel_format_string + " " + SampleWidth + "x" + SampleHeight +
            " " + FPS.ToString("F0") + "fps)";
  }
  public System.UInt32 ProcessID {get; set;}
  public string ProcessName {get; set;}
  public System.Int32 SampleWidth {get; set;}
  public System.Int32 SampleHeight {get; set;}
  public scff_interprocess.ImagePixelFormat SamplePixelFormat { get; set; }
  public System.Double FPS {get; set;}
};

/// @brief scff_inteprocess.Directoryをマネージドクラス化したクラス
public class Directory {
  /// @brief コンストラクタ。引数はInterprocessの構造体。
  public Directory(scff_interprocess.Directory interprocess_directory) {
    Entries = new List<Entry>();
    for (int i = 0; i < scff_interprocess.Interprocess.kMaxEntry; i++) {
      if (interprocess_directory.entries[i].process_id == 0)
        continue;
      Entries.Add(new Entry(interprocess_directory.entries[i]));
    }
  }
  public scff_interprocess.Directory ToInterprocessDirectory() {
    scff_interprocess.Directory interprocess_directory = new scff_interprocess.Directory();
    // Listの前から順番に書き込む
    for (int i = 0; i < scff_interprocess.Interprocess.kMaxEntry; i++) {
      if (i < Entries.Count) {
        interprocess_directory.entries[i] = Entries[i].ToInterprocessEntry();
      } else {
        // 念のためゼロクリア
        interprocess_directory.entries[i].process_id = 0;
        interprocess_directory.entries[i].process_name = "";
        interprocess_directory.entries[i].sample_pixel_format = 0;
        interprocess_directory.entries[i].sample_width = 0;
        interprocess_directory.entries[i].sample_height = 0;
        interprocess_directory.entries[i].fps = 0.0;
      }
    }
    return interprocess_directory;
  }

  public List<Entry> Entries {get; set;}
}

/// @brief scff_inteprocess.LayoutParameterをマネージドクラス化したクラス
public class LayoutParameter {
  /// @brief デフォルトコンストラクタ。
  public LayoutParameter() {
    // nop
  }
  /// @brief コンストラクタ。引数はInterprocessの構造体。
  public LayoutParameter(scff_interprocess.LayoutParameter interprocess_layout_parameter) {
    BoundX = interprocess_layout_parameter.bound_x;
    BoundY = interprocess_layout_parameter.bound_y;
    BoundWidth = interprocess_layout_parameter.bound_width;
    BoundHeight = interprocess_layout_parameter.bound_height;
    Window = interprocess_layout_parameter.window;
    ClippingX = interprocess_layout_parameter.clipping_x;
    ClippingY = interprocess_layout_parameter.clipping_y;
    ClippingWidth = interprocess_layout_parameter.clipping_width;
    ClippingHeight = interprocess_layout_parameter.clipping_height;
    if (interprocess_layout_parameter.show_cursor != 0) {
      ShowCursor = true;
    } else {
      ShowCursor = false;
    }
    if (interprocess_layout_parameter.show_layered_window != 0) {
      ShowLayeredWindow = true;
    } else {
      ShowLayeredWindow = false;
    }
    switch (interprocess_layout_parameter.sws_flags) {
    case (System.Int32)scff_interprocess.SWScaleFlags.kFastBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kFastBilinear;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kBilinear;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kBicubic:
      SwsFlags = scff_interprocess.SWScaleFlags.kBicubic;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kX:
      SwsFlags = scff_interprocess.SWScaleFlags.kX;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kPoint:
      SwsFlags = scff_interprocess.SWScaleFlags.kPoint;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kArea:
      SwsFlags = scff_interprocess.SWScaleFlags.kArea;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kBicublin:
      SwsFlags = scff_interprocess.SWScaleFlags.kBicublin;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kGauss:
      SwsFlags = scff_interprocess.SWScaleFlags.kGauss;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kSinc:
      SwsFlags = scff_interprocess.SWScaleFlags.kSinc;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kLanczos:
      SwsFlags = scff_interprocess.SWScaleFlags.kLanczos;
      break;
    case (System.Int32)scff_interprocess.SWScaleFlags.kSpline:
      SwsFlags = scff_interprocess.SWScaleFlags.kSpline;
      break;
    }
    if (interprocess_layout_parameter.stretch != 0) {
      Stretch = true;
    } else {
      Stretch = false;
    }
    if (interprocess_layout_parameter.keep_aspect_ratio != 0) {
      KeepAspectRatio = true;
    } else {
      KeepAspectRatio = false;
    }
    switch (interprocess_layout_parameter.rotate_direction) {
    case (System.Int32)scff_interprocess.RotateDirection.kNoRotate:
      RotateDirection = scff_interprocess.RotateDirection.kNoRotate;
      break;
    case (System.Int32)scff_interprocess.RotateDirection.k90Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k90Degrees;
      break;
    case (System.Int32)scff_interprocess.RotateDirection.k180Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k180Degrees;
      break;
    case (System.Int32)scff_interprocess.RotateDirection.k270Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k270Degrees;
      break;
    }
  }
  /// @brief Interprocessで利用可能な構造体に変換
  public scff_interprocess.LayoutParameter ToInterprocessLayoutParameter() {
    scff_interprocess.LayoutParameter interprocess_layout_parameter = new scff_interprocess.LayoutParameter();
    interprocess_layout_parameter.bound_x = BoundX;
    interprocess_layout_parameter.bound_y = BoundY;
    interprocess_layout_parameter.bound_width = BoundWidth;
    interprocess_layout_parameter.bound_height = BoundHeight;
    interprocess_layout_parameter.window = Window;
    interprocess_layout_parameter.clipping_x = ClippingX;
    interprocess_layout_parameter.clipping_y = ClippingY;
    interprocess_layout_parameter.clipping_width = ClippingWidth;
    interprocess_layout_parameter.clipping_height = ClippingHeight;
    if (ShowCursor) {
      interprocess_layout_parameter.show_cursor = 1;
    } else {
      interprocess_layout_parameter.show_cursor = 0;
    }
    if (ShowLayeredWindow) {
      interprocess_layout_parameter.show_layered_window = 1;
    } else {
      interprocess_layout_parameter.show_layered_window = 0;
    }
    interprocess_layout_parameter.sws_flags = (System.Int32)SwsFlags;
    if (Stretch) {
      interprocess_layout_parameter.stretch = 1;
    } else {
      interprocess_layout_parameter.stretch = 0;
    }
    if (KeepAspectRatio) {
      interprocess_layout_parameter.keep_aspect_ratio = 1;
    } else {
      interprocess_layout_parameter.keep_aspect_ratio = 0;
    }
    interprocess_layout_parameter.rotate_direction = (System.Int32)RotateDirection;

    return interprocess_layout_parameter;
  }

  public System.Int32 BoundX { get; set; }
  public System.Int32 BoundY { get; set; }
  public System.Int32 BoundWidth { get; set; }
  public System.Int32 BoundHeight { get; set; }
  public System.UInt64 Window { get; set; }
  public System.Int32 ClippingX { get; set; }
  public System.Int32 ClippingY { get; set; }
  public System.Int32 ClippingWidth { get; set; }
  public System.Int32 ClippingHeight { get; set; }
  public System.Boolean ShowCursor { get; set; }
  public System.Boolean ShowLayeredWindow { get; set; }
  public scff_interprocess.SWScaleFlags SwsFlags { get; set; }
  public System.Boolean Stretch { get; set; }
  public System.Boolean KeepAspectRatio { get; set; }
  public scff_interprocess.RotateDirection RotateDirection { get; set; }
}

/// @brief scff_inteprocess.Directoryをマネージドクラス化したクラス
public class Message {
 /// @brief コンストラクタ。引数はInterprocessの構造体。
  public Message(scff_interprocess.Message interprocess_message) {
    Timestamp = interprocess_message.timestamp;
    switch (interprocess_message.layout_type) {
    case (System.Int32)scff_interprocess.LayoutType.kNullLayout:
      LayoutType = scff_interprocess.LayoutType.kNullLayout;
      break;
    case (System.Int32)scff_interprocess.LayoutType.kNativeLayout:
      LayoutType = scff_interprocess.LayoutType.kNativeLayout;
      break;
    case (System.Int32)scff_interprocess.LayoutType.kComplexLayout:
      LayoutType = scff_interprocess.LayoutType.kComplexLayout;
      break;
    }
    LayoutElementCount = interprocess_message.layout_element_count;
    LayoutParameters = new List<LayoutParameter>();
    for (int i = 0; i < scff_interprocess.Interprocess.kMaxComplexLayoutElements; i++) {
      LayoutParameter layout_parameter = new LayoutParameter(interprocess_message.layout_parameters[i]);
      LayoutParameters.Add(layout_parameter);
    }
  }
  /// @brief Interprocessで利用可能な構造体に変換
  public scff_interprocess.Message ToInterprocessMessage() {
    scff_interprocess.Message interprocess_message = new scff_interprocess.Message();
    interprocess_message.timestamp = Timestamp;
    interprocess_message.layout_type = (System.Int32)LayoutType;
    interprocess_message.layout_element_count = LayoutElementCount;
    // Listの前から順番に書き込む
    for (int i = 0; i < scff_interprocess.Interprocess.kMaxComplexLayoutElements; i++) {
      if (i < LayoutParameters.Count) {
        interprocess_message.layout_parameters[i] = LayoutParameters[i].ToInterprocessLayoutParameter();
      } else {
        // 念のためゼロクリア
        interprocess_message.layout_parameters[i].bound_x = 0;
        interprocess_message.layout_parameters[i].bound_y = 0;
        interprocess_message.layout_parameters[i].bound_width = 0;
        interprocess_message.layout_parameters[i].bound_height = 0;
        interprocess_message.layout_parameters[i].window = 0;
        interprocess_message.layout_parameters[i].clipping_x = 0;
        interprocess_message.layout_parameters[i].clipping_y = 0;
        interprocess_message.layout_parameters[i].clipping_width = 0;
        interprocess_message.layout_parameters[i].clipping_height = 0;
        interprocess_message.layout_parameters[i].show_cursor = 0;
        interprocess_message.layout_parameters[i].show_layered_window = 0;
        interprocess_message.layout_parameters[i].sws_flags = 0;
        interprocess_message.layout_parameters[i].stretch = 0;
        interprocess_message.layout_parameters[i].keep_aspect_ratio = 0;
        interprocess_message.layout_parameters[i].rotate_direction = 0;
      }
    }
    return interprocess_message;
  }

  public System.Int64 Timestamp { get; set; }
  public scff_interprocess.LayoutType LayoutType { get; set; }
  public System.Int32 LayoutElementCount { get; set; }
  public List<LayoutParameter> LayoutParameters { get; set; }
}
}   // namespace scff_app
