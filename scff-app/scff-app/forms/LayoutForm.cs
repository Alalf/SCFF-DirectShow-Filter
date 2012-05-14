using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;

namespace scff_app.gui {

public partial class LayoutForm : Form {
  private BindingSource layoutParameterBindingSource_;
  private BindingSource entryBindingSource_;

  public LayoutForm(BindingSource layoutParameterBindingSource, BindingSource entryBindingSource) {
    InitializeComponent();

    layoutParameterBindingSource_ = layoutParameterBindingSource;
    entryBindingSource_ = entryBindingSource;

    int bound_width, bound_height;
    Debug.Assert(entryBindingSource_.Current != null);

    bound_width = ((Entry)entryBindingSource_.Current).SampleWidth;
    bound_height = ((Entry)entryBindingSource_.Current).SampleHeight;

    layout_panel.Width = bound_width;
    layout_panel.Height = bound_height;

    // BindingSourceを見て必要な分だけ
    int index = 0;
    foreach (LayoutParameter i in layoutParameterBindingSource_.List) {
      PreviewControl preview = new PreviewControl(bound_width, bound_height, index, i);
      int x = (int)((i.BoundRelativeLeft * bound_width) / 100);
      int y = (int)((i.BoundRelativeTop * bound_height) / 100);
      int width = (int)(((i.BoundRelativeRight - i.BoundRelativeLeft) * bound_width) / 100);
      int height = (int)(((i.BoundRelativeBottom - i.BoundRelativeTop) * bound_height) / 100);
      preview.Location = new Point(x, y);
      preview.Size = new Size(width, height);
      
      layout_panel.Controls.Add(preview);
      preview.BringToFront();
      ++index;
    }
  }

  private void LayoutForm_Load(object sender, System.EventArgs e) {

  }

  private void add_item_Click(object sender, System.EventArgs e) {

  }

  private void apply_item_Click(object sender, System.EventArgs e) {
    Close();
  }
}
}
