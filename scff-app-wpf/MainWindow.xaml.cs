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
    public MainWindow() {
      InitializeComponent();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e) {
      using (var dc = myDrawingGroup.Open()) {
        dc.DrawEllipse(null, new Pen(Brushes.Red, 1), new Point(320, 240), 640, 480);

        dc.DrawText(new FormattedText("Hello!",
        System.Globalization.CultureInfo.CurrentUICulture,
        FlowDirection.LeftToRight, new Typeface("Verdana" ),
        16, Brushes.Black), new Point(40, 40));
      }
    }

    private void Image_MouseDown_1(object sender, MouseButtonEventArgs e) {
      MessageBox.Show("hoge");
    }
  }
}
