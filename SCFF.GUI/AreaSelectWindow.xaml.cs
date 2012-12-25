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
using System.Windows.Shapes;

namespace SCFF.GUI {
  /// <summary>
  /// AreaSelectWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class AreaSelectWindow : Window {
    public AreaSelectWindow() {
      InitializeComponent();

      this.MouseLeftButtonDown += (sender, e) => this.DragMove();
    }

    private void CloseWindow_Executed(object sender, ExecutedRoutedEventArgs e) {
      this.DialogResult = false;
      this.Close();
    }

    private void AreaSelectWindow_MouseDown(object sender, MouseButtonEventArgs e) {
      if (e.ClickCount == 2) {
        // Double Click
        this.DialogResult = true;
        this.Close();
      }
    }
  }
}
