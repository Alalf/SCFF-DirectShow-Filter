
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

/// @file scffappnet/data_source.h
/// @brief scffappnet::***Wrapperなどの宣言

#ifndef scffappnet_SCFFAPPNET_DATA_SOURCE_H_
#define scffappnet_SCFFAPPNET_DATA_SOURCE_H_

#include "scff-interprocess/interprocess.h"

namespace scffappnet {

using namespace System;

/// @brief Entryをマネージドクラス化したもの
ref class ManagedEntry {
 public:
  ManagedEntry(const scff_interprocess::Entry &raw_entry) {
    entry_ = new scff_interprocess::Entry;
    *entry_ = raw_entry;
  }
  ~ManagedEntry() {
    if (entry_ != 0) {
      delete entry_;
    }
  }
  scff_interprocess::Entry* get_raw_entry() {
    return entry_;
  }
  property String^ Info {
    String^ get() {
      String^ pixel_format_string;
      switch (SamplePixelFormat) {
      case scff_interprocess::kI420:
        pixel_format_string = "I420";
        break;
      case scff_interprocess::kUYVY:
        pixel_format_string = "UYVY";
        break;
      case scff_interprocess::kRGB0:
        pixel_format_string = "RGB0";
        break;
      }

      return "[" + ProcessID +"] " + ProcessName +
             " (" + pixel_format_string + " " + SampleWidth + "x" + SampleHeight +
             " " + FPS.ToString("F0") + "fps)";
    }
  }
  property DWORD ProcessID {
    DWORD get() {
      return entry_->process_id;
    }
    void set(DWORD value) {
      entry_->process_id = value;
    }
  }
  property String^ ProcessName {
    String^ get() {
      return gcnew String(entry_->process_name);
    }
    void set(String^ value) {
      using namespace System::Runtime::InteropServices;
      IntPtr value_ptr;
      try {
        value_ptr = Marshal::StringToHGlobalAnsi(value);
        char* valur_char_ptr = static_cast<char*>(value_ptr.ToPointer());
        strcpy_s(entry_->process_name, kMaxPath, valur_char_ptr);
      }
      finally {
        Marshal::FreeHGlobal(value_ptr);
      }
    }
  }
  property int SampleWidth {
    int get() {
      return entry_->sample_width;
    }
    void set(int value) {
      entry_->sample_width = value;
    }
  }
  property int SampleHeight {
    int get() {
      return entry_->sample_height;
    }
    void set(int value) {
      entry_->sample_height = value;
    }
  }
  property int SamplePixelFormat {
    int get() {
      return entry_->sample_image_pixel_format;
    }
    void set(int value) {
      entry_->sample_image_pixel_format = value;
    }
  }
  property double FPS {
    double get() {
      return entry_->fps;
    }
    void set(double value) {
      entry_->fps = value;
    }
  }

 private:
  scff_interprocess::Entry *entry_;
};
}

#endif  // scffappnet_SCFFAPPNET_DATA_SOURCE_H_
