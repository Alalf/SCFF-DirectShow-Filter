using System.Windows.Forms;
using scff_app.Commons.Views.Forms;

namespace scff_app.views.layouts {
  public partial class PreviewControl : UserControl {
    private DragMover drag_mover_;

    public PreviewControl(int bound_width, int bound_height) {
      InitializeComponent();

      drag_mover_ = new DragMover(this, bound_width, bound_height);
    }

    private void innerPanel_MouseDown(object sender, MouseEventArgs e) {
      OnMouseDown(e);
    }

    private void innerPanel_MouseMove(object sender, MouseEventArgs e) {
      OnMouseMove(e);
    }

    private void innerPanel_MouseUp(object sender, MouseEventArgs e) {
      OnMouseUp(e);
    }
  }
}
