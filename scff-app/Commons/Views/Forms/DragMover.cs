using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScffApp.Commons.Views.Forms
{
    class DragMover
    {
        private Control target;
        private bool dragging;
        private Point startLocation;

        public DragMover(Control target)
        {
            this.target = target;
        }

        public void OnMouseDown(Point location)
        {
            dragging = true;
            startLocation = location;
        }

        public void OnMouseMove(Point location)
        {
            if (dragging)
            {
                target.Left += location.X - startLocation.X;
                target.Top += location.Y - startLocation.Y;
            }
        }

        public void OnMouseUp()
        {
            dragging = false;
        }
    }
}
