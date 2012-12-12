using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace scff_app_wpf {
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window {
    private Point circle_point_ = new Point(50, 50);
    private Point text_point_ = new Point(40, 40);

    private static Pen pen_ = new Pen(Brushes.Red, 1);
    private static Typeface text_font_ = new Typeface("Verdana");
    private static FormattedText text_ = new FormattedText(
        "Hello!",
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight,
        text_font_,
        16,
        Brushes.Black);

    public MainWindow() {
      InitializeComponent();
      this.MouseLeftButtonDown += (sender, e) => this.DragMove();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
      using (var dc = myDrawingGroup.Open()) {
        dc.DrawEllipse(null, pen_, circle_point_, 100, 100);
        dc.DrawText(text_, text_point_);

        ++circle_point_.X;
        ++circle_point_.Y;
        --text_point_.X;
        --text_point_.Y;
      }
    }

    private void Image_MouseDown_1(object sender, MouseButtonEventArgs e) {
      
    }

    private void Button_Click_2(object sender, RoutedEventArgs e) {
      if (myCanvas.Visibility == Visibility.Visible) {
        myCanvas.Visibility = Visibility.Collapsed;
      } else {
        myCanvas.Visibility = Visibility.Visible;
      }
    }
  }
}
