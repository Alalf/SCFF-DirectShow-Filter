namespace scff_app.gui
{
    partial class PreviewControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
      this.inner_panel = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // inner_panel
      // 
      this.inner_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.inner_panel.BackColor = System.Drawing.Color.Black;
      this.inner_panel.Location = new System.Drawing.Point(2, 2);
      this.inner_panel.Name = "inner_panel";
      this.inner_panel.Size = new System.Drawing.Size(146, 146);
      this.inner_panel.TabIndex = 0;
      this.inner_panel.Paint += new System.Windows.Forms.PaintEventHandler(this.inner_panel_Paint);
      this.inner_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseDown);
      this.inner_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseMove);
      this.inner_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseUp);
      // 
      // PreviewControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.DarkOrange;
      this.Controls.Add(this.inner_panel);
      this.Name = "PreviewControl";
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel inner_panel;


    }
}
