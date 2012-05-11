using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace scff_app {

public partial class Form1 : Form {
  scff_interprocess.Interprocess interprocess_;

  public Form1() {
    InitializeComponent();

    interprocess_ = new scff_interprocess.Interprocess();
  }
  private void button1_Click(object sender, EventArgs e) {
    interprocess_.InitDirectory();
    scff_interprocess.Interprocess.Directory directory;
    interprocess_.GetDirectory(out directory);
    MessageBox.Show(directory.entries[0].process_name + " " +
        directory.entries[0].process_id + " " +
        directory.entries[0].sample_pixel_format + " " +
        directory.entries[0].sample_width + " " +
        directory.entries[0].sample_height + " " +
        directory.entries[0].fps);
  }
}
}
