using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScffApp.Views.Layouts
{
    public partial class LayoutForm : Form
    {
        public LayoutForm()
        {
            InitializeComponent();
        }

        private void addingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var previewControl = new PreviewControl();
            previewControl.ContextMenu = null;
            Controls.Add(previewControl);
        }
    }
}
