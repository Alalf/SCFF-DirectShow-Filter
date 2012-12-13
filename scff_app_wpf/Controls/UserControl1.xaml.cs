using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
  /// UserControl1.xaml の相互作用ロジック
  /// </summary>
  public partial class UserControl1 : UserControl {
    public UserControl1() {
      InitializeComponent();
    }

    private void Viewbox_Loaded_1(object sender, RoutedEventArgs e) {
      var myApp = App.GetMyApp();
    }
  }
}
