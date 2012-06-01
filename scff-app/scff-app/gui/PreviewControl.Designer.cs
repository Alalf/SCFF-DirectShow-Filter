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
      this.components = new System.ComponentModel.Container();
      this.captureTimer = new System.Windows.Forms.Timer(this.components);
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.fit = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // captureTimer
      // 
      this.captureTimer.Interval = 3000;
      this.captureTimer.Tick += new System.EventHandler(this.captureTimer_Tick);
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fit});
      this.contextMenu.Name = "fit_menu";
      this.contextMenu.Size = new System.Drawing.Size(153, 48);
      // 
      // fit
      // 
      this.fit.Name = "fit";
      this.fit.Size = new System.Drawing.Size(152, 22);
      this.fit.Text = "Fit";
      this.fit.Click += new System.EventHandler(this.fit_Click);
      // 
      // PreviewControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.ContextMenuStrip = this.contextMenu;
      this.Name = "PreviewControl";
      this.Load += new System.EventHandler(this.PreviewControl_Load);
      this.SizeChanged += new System.EventHandler(this.PreviewControl_SizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewControl_Paint);
      this.contextMenu.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer captureTimer;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem fit;



    }
}
