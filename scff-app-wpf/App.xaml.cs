using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace scff_app_wpf {
  /// <summary>
  /// App.xaml の相互作用ロジック
  /// </summary>
  public partial class App : Application {
    private static Application myApp;
    
    public App() {
      myApp = this;
    }

    public static Application GetMyApp() {
      return myApp;
    }
  }
}
