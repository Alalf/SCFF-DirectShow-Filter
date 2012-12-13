// Copyright 2012 Alalf <alalf.iQLc_at_gmail.com>
//
// This file is part of SCFF-DirectShow-Filter.
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

/// @file base/debug.cc
/// デバッグ用定数、関数定義

#include "base/debug.h"

#include <tchar.h>

//=====================================================================
// デバッグ用定数
//=====================================================================

const int kDbgTrace     = 1;
const int kDbgRare      = 2;
const int kDbgImportant = 3;
const int kDbgNewDelete = 4;
const int kDbgMax       = 5;

const int kDbgCurrentLevel = kDbgTrace;

//=====================================================================
// デバッグ関数
//=====================================================================

#ifdef _DEBUG
int MyDebugLog(DWORD types, DWORD level, LPCTSTR format, ...) {
  if (level < kDbgCurrentLevel) return 0;

  TCHAR new_format[512];
  ZeroMemory(new_format, sizeof(new_format));
  new_format[0] = '\0';
  TCHAR spaces[32];
  ZeroMemory(spaces, sizeof(spaces));
  spaces[0] = '\0';

  for (int i = kDbgMax - level; i > 1; i--) _tcscat_s(spaces, 32, TEXT(" "));

  switch (types) {
    case LOG_TRACE: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG_TRACE"), format);
      break;
    }
    case LOG_ERROR: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG_ERROR"), format);
      break;
    }
    case LOG_LOCKING: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG_LOCKING"), format);
      break;
    }
    case LOG_MEMORY: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG_MEMORY"), format);
      break;
    }
    case LOG_TIMING: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG_TIMING"), format);
      break;
    }
    default: {
      _stprintf_s(new_format, 512, TEXT("%s%s: %s\r\n"),
                  spaces, TEXT("LOG"), format);
      break;
    }
  }
  TCHAR buffer[512];
  ZeroMemory(buffer, sizeof(buffer));
  buffer[0] = '\0';

  va_list ap;
  va_start(ap, format);
  int iret = _vstprintf_s(buffer, 512, new_format, ap);
  va_end(ap);
  DbgOutString(buffer);
  return iret;
}

void MyDebugMessageBox(LPCTSTR format, ...) {
  TCHAR buffer[512];
  ZeroMemory(buffer, sizeof(buffer));

  va_list ap;
  va_start(ap, format);
  _vstprintf_s(buffer, 512, format, ap);
  va_end(ap);

  MessageBox(nullptr, buffer, TEXT("debug info"), MB_OK);
}
#endif  // _DEBUG
