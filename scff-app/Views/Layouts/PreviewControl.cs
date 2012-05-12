using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ScffApp.Commons.Views.Forms;

namespace ScffApp.Views.Layouts
{
    public partial class PreviewControl : UserControl
    {
        private DragMover dragMover;

        public PreviewControl()
        {
            InitializeComponent();

            dragMover = new DragMover(this);
        }

        private void PreviewControl_MouseDown(object sender, MouseEventArgs e)
        {
            dragMover.OnMouseDown(e.Location);
        }

        private void PreviewControl_MouseMove(object sender, MouseEventArgs e)
        {
            dragMover.OnMouseMove(e.Location);
        }

        private void PreviewControl_MouseUp(object sender, MouseEventArgs e)
        {
            dragMover.OnMouseUp();
        }
    }
}
