

namespace SCFF.GUI.Controls {

using SCFF.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

/// レイアウトエディタ
public partial class LayoutEdit : UserControl, IProfileEditor {

  /// コンストラクタ
  public LayoutEdit() {
    InitializeComponent();
    RenderOptions.SetBitmapScalingMode(this.drawingGroup, BitmapScalingMode.LowQuality);
    this.Draw(string.Empty);
  }

  private void Draw(string text) {
    using (var dc = this.drawingGroup.Open()) {
      dc.DrawRectangle(Brushes.Black, null, new Rect(0,0,Constants.DefaultPreviewWidth,Constants.DefaultPreviewHeight));
      dc.DrawRectangle(Brushes.DarkGray, null, new Rect(10,10,100,100));
      if (text != string.Empty) {
        var formattedText = new FormattedText(text,
            System.Globalization.CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface("Meiryo"),
            10,
            Brushes.White);
        dc.DrawText(formattedText, new Point(10,200));
      }
    }
  }

  public void UpdateByProfile() {
    this.Draw("Update");
  }


  public void AttachChangedEventHandlers() {
    throw new System.NotImplementedException();
  }

  public void DetachChangedEventHandlers() {
    throw new System.NotImplementedException();
  }


  private void Image_MouseDown_1(object sender, MouseButtonEventArgs e) {
    var pt = e.GetPosition((IInputElement)sender);
    var x = (int)pt.X;
    var y = (int)pt.Y;
    this.Draw(x + ", " + y);
  }

  private void Image_MouseMove_1(object sender, MouseEventArgs e) {
    // iroiro
  }


}
}
