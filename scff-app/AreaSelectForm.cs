// Copyright 2012 Progre <djyayutto_at_gmail.com>
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace scff_app
{
    public partial class AreaSelectForm : Form
    {
        public AreaSelectForm()
        {
            InitializeComponent();

            resizing = false;
            last = new Point(0, 0);
            clipping_x = 0;
            clipping_y = 0;
            clipping_width = 32;
            clipping_height = 32;
        }
        public int clipping_x;
        public int clipping_y;
        public int clipping_width;
        public int clipping_height;

        private bool resizing;
        private Point last;
        private Point moving_start;

        private void AreaSelectForm_DoubleClick(object sender, EventArgs e)
        {
            Close();
        }
        private void AreaSelectForm_MouseDown(object sender, MouseEventArgs e)
        {
            resizing = true;
            last = e.Location;
            moving_start = e.Location;
        }
        private void AreaSelectForm_MouseUp(object sender, MouseEventArgs e)
        {
            resizing = false;
        }
        private void AreaSelectForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (resizing)
            {
                int w = Size.Width;
                int h = Size.Height;

                if (Cursor.Equals(Cursors.SizeNWSE))
                {
                    Size my_size = new Size(w + (e.Location.X - last.X), h + (e.Location.Y - last.Y));
                    if (my_size.Width > 32 && my_size.Height > 32)
                    {
                        Size = my_size;
                    }
                }
                else if (Cursor.Equals(Cursors.SizeWE))
                {
                    var my_size = new Size(w + (e.Location.X - last.X), h);
                    if (my_size.Width > 32 && my_size.Height > 32)
                    {
                        Size = my_size;
                    }
                }
                else if (Cursor.Equals(Cursors.SizeNS))
                {
                    var my_size = new Size(w, h + (e.Location.Y - last.Y));
                    if (my_size.Width > 32 && my_size.Height > 32)
                    {
                        this.Size = my_size;
                    }
                }
                else if (Cursor.Equals(Cursors.SizeAll))
                {
                    Left += e.Location.X - moving_start.X;
                    Top += e.Location.Y - moving_start.Y;
                }

                last = e.Location;
            }
            else
            {
                bool resize_x = e.X > (Width - 16);
                bool resize_y = e.Y > (Height - 16);

                if (resize_x && resize_y)
                {
                    Cursor = Cursors.SizeNWSE;
                }
                else if (resize_x)
                {
                    Cursor = Cursors.SizeWE;
                }
                else if (resize_y)
                {
                    Cursor = Cursors.SizeNS;
                }
                else
                {
                    Cursor = Cursors.SizeAll;
                }
            }
        }
        private void AreaSelectForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Point origin = new Point(0, 0);
            clipping_x = PointToScreen(origin).X;
            clipping_y = PointToScreen(origin).Y;
            clipping_width = Size.Width;
            clipping_height = Size.Height;
        }
    }
}
