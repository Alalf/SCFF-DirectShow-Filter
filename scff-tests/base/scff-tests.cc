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

#include "base/scff-tests.h"
#include <libavfilter/drawutils.h>

int _tmain(int argc, _TCHAR* argv[]) {
  FFDrawContext* test_context = new FFDrawContext;
  FFDrawColor* test_color = new FFDrawColor;
  uint8_t test_fill_color[4] = {255, 255, 255, 255};

  ff_draw_init(test_context, AV_PIX_FMT_RGB0, 0);
  ff_draw_color(test_context, test_color, test_fill_color);

  printf("Hello, World! %d\n", test_context->format);
  getchar();

  delete test_context;

  return 0;
}
