
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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace scff_app {

/// @brief scff_inteprocess.Entryをマネージドクラス化したクラス
public class Entry {
  /// @brief コンストラクタ。引数はInterprocessの構造体。
  public Entry(scff_interprocess.Entry interprocess_entry) {
    ProcessID = interprocess_entry.process_id;
    ProcessName = interprocess_entry.process_name;
    switch (interprocess_entry.sample_pixel_format) {
    case (Int32)scff_interprocess.ImagePixelFormat.kI420:
      SamplePixelFormat = scff_interprocess.ImagePixelFormat.kI420;
      break;
    case (Int32)scff_interprocess.ImagePixelFormat.kUYVY:
      SamplePixelFormat = scff_interprocess.ImagePixelFormat.kUYVY;
      break;
    case (Int32)scff_interprocess.ImagePixelFormat.kRGB0:
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
    interprocess_entry.sample_pixel_format = (Int32)SamplePixelFormat;
    interprocess_entry.sample_width = SampleWidth;
    interprocess_entry.sample_height = SampleHeight;
    interprocess_entry.fps = FPS;
    return interprocess_entry;
  }
  /// @brief 文字列に変換
  public string EntryInfo {
    get {
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
  }
  public UInt32 ProcessID {get; set;}
  public string ProcessName {get; set;}
  public Int32 SampleWidth {get; set;}
  public Int32 SampleHeight {get; set;}
  public scff_interprocess.ImagePixelFormat SamplePixelFormat { get; set; }
  public Double FPS {get; set;}
};

/// @brief scff_inteprocess.Directoryをマネージドクラス化したクラス
public class Directory {
  /// @brief デフォルトコンストラクタ。
  public Directory() {
    // Entryゼロのリストを作る
    Entries = new List<Entry>();
  }
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
    interprocess_directory.entries =
        new scff_interprocess.Entry[scff_interprocess.Interprocess.kMaxEntry];
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
  //-------------------------------------------------------------------
  // Wrappers
  //-------------------------------------------------------------------
  [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  private static extern int GetClassName(UIntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
  [DllImport("user32.dll", SetLastError = false)]
  static extern UIntPtr GetDesktopWindow();
  [StructLayout(LayoutKind.Sequential)]
  struct RECT { public int left, top, right, bottom; }
  [DllImport("user32.dll")]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool IsWindow(UIntPtr hWnd);
  [DllImport("user32.dll")]
  static extern bool GetClientRect(UIntPtr hWnd, out RECT lpRect);
  //-------------------------------------------------------------------

  /// @brief デフォルトコンストラクタ。
  public LayoutParameter() {
    // デフォルト値を設定
    KeepAspectRatio = true;
    Stretch = true;
    SwsFlags = scff_interprocess.SWScaleFlags.kLanczos;

    // Windowまわり
    Window = GetDesktopWindow();
    RECT window_rect;
    GetClientRect(Window, out window_rect);
    ClippingX = window_rect.left;
    ClippingY = window_rect.top;
    ClippingWidth = window_rect.right;
    ClippingHeight = window_rect.bottom;
    Fit = true;

    // GUIクライアントからは使わないが一応
    BoundX = 0;
    BoundY = 0;
    BoundWidth = 1;
    BoundHeight = 1;
    //----

    BoundRelativeLeft = 0.0;
    BoundRelativeRight = 100.0;
    BoundRelativeTop = 0.0;
    BoundRelativeBottom = 100.0;
  }
  /// @brief コンストラクタ。引数はInterprocessの構造体。
  public LayoutParameter(scff_interprocess.LayoutParameter interprocess_layout_parameter) {
    BoundX = interprocess_layout_parameter.bound_x;
    BoundY = interprocess_layout_parameter.bound_y;
    BoundWidth = interprocess_layout_parameter.bound_width;
    BoundHeight = interprocess_layout_parameter.bound_height;
    Window = unchecked((UIntPtr)interprocess_layout_parameter.window);
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
    case (Int32)scff_interprocess.SWScaleFlags.kFastBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kFastBilinear;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kBilinear;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kBicubic:
      SwsFlags = scff_interprocess.SWScaleFlags.kBicubic;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kX:
      SwsFlags = scff_interprocess.SWScaleFlags.kX;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kPoint:
      SwsFlags = scff_interprocess.SWScaleFlags.kPoint;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kArea:
      SwsFlags = scff_interprocess.SWScaleFlags.kArea;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kBicublin:
      SwsFlags = scff_interprocess.SWScaleFlags.kBicublin;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kGauss:
      SwsFlags = scff_interprocess.SWScaleFlags.kGauss;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kSinc:
      SwsFlags = scff_interprocess.SWScaleFlags.kSinc;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kLanczos:
      SwsFlags = scff_interprocess.SWScaleFlags.kLanczos;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kSpline:
      SwsFlags = scff_interprocess.SWScaleFlags.kSpline;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQFastBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQFastBilinear;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQBilinear:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQBilinear;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQBicubic:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQBicubic;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQX:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQX;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQPoint:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQPoint;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQArea:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQArea;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQBicublin:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQBicublin;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQGauss:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQGauss;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQSinc:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQSinc;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQLanczos:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQLanczos;
      break;
    case (Int32)scff_interprocess.SWScaleFlags.kHQSpline:
      SwsFlags = scff_interprocess.SWScaleFlags.kHQSpline;
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
    case (Int32)scff_interprocess.RotateDirection.kNoRotate:
      RotateDirection = scff_interprocess.RotateDirection.kNoRotate;
      break;
    case (Int32)scff_interprocess.RotateDirection.k90Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k90Degrees;
      break;
    case (Int32)scff_interprocess.RotateDirection.k180Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k180Degrees;
      break;
    case (Int32)scff_interprocess.RotateDirection.k270Degrees:
      RotateDirection = scff_interprocess.RotateDirection.k270Degrees;
      break;
    }
  }
  /// @brief Interprocessで利用可能な構造体に変換
  public scff_interprocess.LayoutParameter ToInterprocessLayoutParameter(int bound_width, int bound_height) {
    scff_interprocess.LayoutParameter interprocess_layout_parameter = new scff_interprocess.LayoutParameter();

    //-- GUIクライアント限定の処理！
    interprocess_layout_parameter.bound_x = (Int32)(bound_width * BoundRelativeLeft) / 100;
    interprocess_layout_parameter.bound_y = (Int32)(bound_height * BoundRelativeTop) / 100;
    interprocess_layout_parameter.bound_width =
        (Int32)(bound_width * BoundRelativeRight) / 100 - interprocess_layout_parameter.bound_x;
    interprocess_layout_parameter.bound_height =
        (Int32)(bound_height * BoundRelativeBottom) / 100 - interprocess_layout_parameter.bound_y;
    //--

    interprocess_layout_parameter.window = (UInt64)Window;
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
    interprocess_layout_parameter.sws_flags = (Int32)SwsFlags;
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
    interprocess_layout_parameter.rotate_direction = (Int32)RotateDirection;

    return interprocess_layout_parameter;
  }

  public Double BoundRelativeLeft { get; set; }
  public Double BoundRelativeRight { get; set; }
  public Double BoundRelativeTop { get; set; }
  public Double BoundRelativeBottom { get; set; }

  public Int32 BoundX { get; set; }
  public Int32 BoundY { get; set; }
  public Int32 BoundWidth { get; set; }
  public Int32 BoundHeight { get; set; }

  public string WindowText {
    get {
      if (Window == UIntPtr.Zero) {
        return "(Splash)";
      } else if (Window == GetDesktopWindow()) {
        return "(Desktop)";
      } else {
        if (!IsWindow(Window)) {
          return "*** INVALID WINDOW ***";
        } else {
          StringBuilder class_name = new StringBuilder(256);
          GetClassName(Window, class_name, 256);
          return class_name.ToString();
        }
      }
    }
  }
  public UIntPtr Window { get; set; }

  public Boolean Fit { get; set; }

  public Int32 ClippingX { get; set; }
  public Int32 ClippingY { get; set; }
  public Int32 ClippingWidth { get; set; }
  public Int32 ClippingHeight { get; set; }
  public Boolean ShowCursor { get; set; }
  public Boolean ShowLayeredWindow { get; set; }
  public scff_interprocess.SWScaleFlags SwsFlags { get; set; }
  public Boolean Stretch { get; set; }
  public Boolean KeepAspectRatio { get; set; }
  public scff_interprocess.RotateDirection RotateDirection { get; set; }
}

/// @brief scff_inteprocess.Directoryをマネージドクラス化したクラス
public class Message {
  /// @brief デフォルトコンストラクタ。
  public Message() {
    // デフォルト値を設定
    Timestamp = DateTime.Now.Ticks;
    LayoutType = scff_interprocess.LayoutType.kNullLayout;
    LayoutElementCount = 0;
    LayoutParameters = new List<LayoutParameter>();
  }
 /// @brief コンストラクタ。引数はInterprocessの構造体。
  public Message(scff_interprocess.Message interprocess_message) {
    Timestamp = interprocess_message.timestamp;
    switch (interprocess_message.layout_type) {
    case (Int32)scff_interprocess.LayoutType.kNullLayout:
      LayoutType = scff_interprocess.LayoutType.kNullLayout;
      break;
    case (Int32)scff_interprocess.LayoutType.kNativeLayout:
      LayoutType = scff_interprocess.LayoutType.kNativeLayout;
      break;
    case (Int32)scff_interprocess.LayoutType.kComplexLayout:
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
  public scff_interprocess.Message ToInterprocessMessage(int bound_width, int bound_height) {
    scff_interprocess.Message interprocess_message = new scff_interprocess.Message();
    interprocess_message.timestamp = Timestamp;
    interprocess_message.layout_type = (Int32)LayoutType;
    interprocess_message.layout_element_count = LayoutElementCount;
    // Listの前から順番に書き込む
    interprocess_message.layout_parameters =
        new scff_interprocess.LayoutParameter[scff_interprocess.Interprocess.kMaxComplexLayoutElements];
    for (int i = 0; i < scff_interprocess.Interprocess.kMaxComplexLayoutElements; i++) {
      if (i < LayoutParameters.Count) {
        interprocess_message.layout_parameters[i] =
            LayoutParameters[i].ToInterprocessLayoutParameter(bound_width, bound_height);
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

  public Int64 Timestamp { get; set; }
  public scff_interprocess.LayoutType LayoutType { get; set; }
  public Int32 LayoutElementCount { get; set; }
  public List<LayoutParameter> LayoutParameters { get; set; }
}
}   // namespace scff_app
