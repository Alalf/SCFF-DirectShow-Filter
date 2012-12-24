

namespace SCFF.GUI.Controls {

using System.Windows;
using System.Windows.Controls;

/// クリッピング領域設定用
public partial class Area : UserControl, IProfileEditor {

  /// コンストラクタ
  public Area() {
    InitializeComponent();
  }

  //-------------------------------------------------------------------
  // IProfileEditorの実装
  //-------------------------------------------------------------------

  public void UpdateByProfile() {
    // checkboxはclickがあるのでeventハンドラをattach/detachする必要はない
    this.fit.IsChecked = App.Profile.CurrentLayoutElement.Fit;
    this.DetachChangedEventHandlers();
    this.clippingX.Text = App.Profile.CurrentLayoutElement.ClippingXWithFit.ToString();
    this.clippingY.Text = App.Profile.CurrentLayoutElement.ClippingYWithFit.ToString();
    this.clippingWidth.Text = App.Profile.CurrentLayoutElement.ClippingWidthWithFit.ToString();
    this.clippingHeight.Text = App.Profile.CurrentLayoutElement.ClippingHeightWithFit.ToString();
    this.AttachChangedEventHandlers();
  }

  public void AttachChangedEventHandlers() {
    this.clippingX.TextChanged += clippingX_TextChanged;
    this.clippingY.TextChanged += clippingY_TextChanged;
    this.clippingWidth.TextChanged += clippingWidth_TextChanged;
    this.clippingHeight.TextChanged += clippingHeight_TextChanged;
  }

  public void DetachChangedEventHandlers() {
    this.clippingX.TextChanged -= clippingX_TextChanged;
    this.clippingY.TextChanged -= clippingY_TextChanged;
    this.clippingWidth.TextChanged -= clippingWidth_TextChanged;
    this.clippingHeight.TextChanged -= clippingHeight_TextChanged;
  }

  //-------------------------------------------------------------------
  // イベントハンドラ(*Changed以外)
  //-------------------------------------------------------------------

  private void fit_Click(object sender, RoutedEventArgs e) {
    if (this.fit.IsChecked.HasValue) {
      App.Profile.CurrentLayoutElement.Fit = (bool)this.fit.IsChecked;

      this.UpdateByProfile();
    }
  }

  //-------------------------------------------------------------------
  // イベントハンドラ(Checked/Unchecked)
  //-------------------------------------------------------------------

  private void fit_Checked(object sender, RoutedEventArgs e) {
    this.clippingX.IsEnabled = false;
    this.clippingY.IsEnabled = false;
    this.clippingWidth.IsEnabled = false;
    this.clippingHeight.IsEnabled = false;
  }

  private void fit_Unchecked(object sender, RoutedEventArgs e) {
    this.clippingX.IsEnabled = true;
    this.clippingY.IsEnabled = true;
    this.clippingWidth.IsEnabled = true;
    this.clippingHeight.IsEnabled = true;
  }

  //-------------------------------------------------------------------
  // イベントハンドラ(*Changed)
  //-------------------------------------------------------------------

  private bool TryParseClippingParameters(TextBox textBox, int lowerBound, int upperBound, out int parsedValue) {
    // Parse
    if (!int.TryParse(textBox.Text, out parsedValue)) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString();
      return false;
    }

    // Validation
    if (parsedValue < lowerBound) {
      parsedValue = lowerBound;
      textBox.Text = lowerBound.ToString();
      return false;
    } else if (parsedValue > upperBound) {
      parsedValue = upperBound;
      textBox.Text = upperBound.ToString();
      return false;
    }

    return true;
  }

  private void clippingX_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.clippingX, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.ClippingXWithoutFit = parsedValue;
    }
  }

  private void clippingY_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.clippingY, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.ClippingYWithoutFit = parsedValue;
    }
  }

  private void clippingWidth_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentLayoutElement.WindowWidth;
    int parsedValue;
    if (this.TryParseClippingParameters(this.clippingWidth, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.ClippingWidthWithoutFit = parsedValue;
    }
  }

  private void clippingHeight_TextChanged(object sender, TextChangedEventArgs e) {
    var lowerBound = 0;
    var upperBound = App.Profile.CurrentLayoutElement.WindowHeight;
    int parsedValue;
    if (this.TryParseClippingParameters(this.clippingHeight, lowerBound, upperBound, out parsedValue)) {
      // Profileに書き込み
      App.Profile.CurrentLayoutElement.ClippingHeightWithoutFit = parsedValue;
    }
  }
}
}   // namespace SCFF.GUI.Controls
