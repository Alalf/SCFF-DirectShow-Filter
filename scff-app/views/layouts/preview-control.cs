using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using scff_app.Commons.Views.Forms;

namespace scff_app.views.layouts
{
    public partial class PreviewControl : UserControl
    {
        private DragMover drag_mover_;

        public PreviewControl()
        {
            InitializeComponent();

            drag_mover_ = new DragMover(this);
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
