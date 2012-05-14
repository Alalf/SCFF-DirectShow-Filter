using System.Windows.Forms;

namespace scff_app.forms {
  public partial class LayoutForm : Form {
    private BindingSource layoutParameterBindingSource_;
    private BindingSource entryBindingSource_;

    public LayoutForm(BindingSource layoutParameterBindingSource, BindingSource entryBindingSource) {
      InitializeComponent();

      layoutParameterBindingSource_ = layoutParameterBindingSource;
      entryBindingSource_ = entryBindingSource;

      int bound_width, bound_height;
      if (entryBindingSource_.Current != null) {
        bound_width = ((Entry)entryBindingSource_.Current).SampleWidth;
        bound_height = ((Entry)entryBindingSource_.Current).SampleHeight;
      } else {
        // ダミーの値。こちらが使われることがあってはならない！
        bound_width = 640;
        bound_height = 480;
      }

      layout_panel.Width = bound_width;
      layout_panel.Height = bound_height;

      PreviewControl preview = new PreviewControl(bound_width, bound_height);
      layout_panel.Controls.Add(preview);
    }

    private void LayoutForm_Load(object sender, System.EventArgs e) {

    }

    private void add_item_Click(object sender, System.EventArgs e) {
      int bound_width, bound_height;
      if (entryBindingSource_.Current != null) {
        bound_width = ((Entry)entryBindingSource_.Current).SampleWidth;
        bound_height = ((Entry)entryBindingSource_.Current).SampleHeight;
      } else {
        // ダミーの値。こちらが使われることがあってはならない！
        bound_width = 640;
        bound_height = 480;
      }

      PreviewControl preview = new PreviewControl(bound_width, bound_height);
      layout_panel.Controls.Add(preview);
      preview.BringToFront();
    }
  }
}
