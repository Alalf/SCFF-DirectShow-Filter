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

        private void innerPanel_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void innerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            OnMouseMove(e);
        }

        private void innerPanel_MouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(e);
        }
    }
}
