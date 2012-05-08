
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

#ifndef SCFF_APP_NET_SCFFAPPNET_DATA_SOURCE_H_
#define SCFF_APP_NET_SCFFAPPNET_DATA_SOURCE_H_

#include "base/scff-interprocess.h"

namespace scffappnet {

using namespace System;

/// @brief SCFFEntryをマネージドクラス化したもの
ref class ManagedSCFFEntry {
 public:
  ManagedSCFFEntry(const SCFFEntry &raw_entry) {
    scff_entry_ = new SCFFEntry;
    *scff_entry_ = raw_entry;
  }
  ~ManagedSCFFEntry() {
    if (scff_entry_ != 0) {
      delete scff_entry_;
    }
  }
  SCFFEntry* get_raw_entry() {
    return scff_entry_;
  }
  property String^ Info {
    String^ get() {
      return "[" + ProcessID +"] " + ProcessName +
             " (" + SampleWidth + "x" + SampleHeight +
             " " + FPS.ToString("F0") + "fps)";
    }
  }
  property DWORD ProcessID {
    DWORD get() {
      return scff_entry_->process_id;
    }
    void set(DWORD value) {
      scff_entry_->process_id = value;
    }
  }
  property String^ ProcessName {
    String^ get() {
      return gcnew String(scff_entry_->process_name);
    }
    void set(String^ value) {
      using namespace System::Runtime::InteropServices;
      IntPtr value_ptr;
      try {
        value_ptr = Marshal::StringToHGlobalAnsi(value);
        char* valur_char_ptr = static_cast<char*>(value_ptr.ToPointer());
        strcpy_s(scff_entry_->process_name, kSCFFMaxProcessNameLength, valur_char_ptr);
      }
      finally {
        Marshal::FreeHGlobal(value_ptr);
      }
    }
  }
  property int SampleWidth {
    int get() {
      return scff_entry_->sample_width;
    }
    void set(int value) {
      scff_entry_->sample_width = value;
    }
  }
  property int SampleHeight {
    int get() {
      return scff_entry_->sample_height;
    }
    void set(int value) {
      scff_entry_->sample_height = value;
    }
  }
  property int SamplePixelFormat {
    int get() {
      return scff_entry_->sample_pixel_format;
    }
    void set(int value) {
      scff_entry_->sample_pixel_format = value;
    }
  }
  property double FPS {
    double get() {
      return scff_entry_->fps;
    }
    void set(double value) {
      scff_entry_->fps = value;
    }
  }

 private:
  SCFFEntry *scff_entry_;
};
}

#endif  // SCFF_APP_NET_SCFFAPPNET_DATA_SOURCE_H_
