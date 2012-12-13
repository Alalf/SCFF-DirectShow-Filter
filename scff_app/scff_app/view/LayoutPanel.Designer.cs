namespace scff_app.view {
partial class LayoutPanel {
  /// <summary> 
  /// 必要なデザイナー変数です。
  /// </summary>
  private System.ComponentModel.IContainer components = null;

  /// <summary> 
  /// 使用中のリソースをすべてクリーンアップします。
  /// </summary>
  /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
  protected override void Dispose(bool disposing) {
    if (disposing && (components != null)) {
      components.Dispose();
    }
    base.Dispose(disposing);
  }

  #region コンポーネント デザイナーで生成されたコード

  /// <summary> 
  /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
  /// コード エディターで変更しないでください。
  /// </summary>
  private void InitializeComponent() {
      this.SuspendLayout();
      // 
      // LayoutPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Black;
      this.Name = "LayoutPanel";
      this.Size = new System.Drawing.Size(640, 360);
      this.Load += new System.EventHandler(this.LayoutPanel_Load);
      this.ResumeLayout(false);

  }

  #endregion
}
}
