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
      this.capture_timer = new System.Windows.Forms.Timer(this.components);
      this.fit_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.fit_item = new System.Windows.Forms.ToolStripMenuItem();
      this.fit_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // capture_timer
      // 
      this.capture_timer.Interval = 3000;
      this.capture_timer.Tick += new System.EventHandler(this.capture_timer_Tick);
      // 
      // fit_menu
      // 
      this.fit_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fit_item});
      this.fit_menu.Name = "fit_menu";
      this.fit_menu.Size = new System.Drawing.Size(153, 48);
      // 
      // fit_item
      // 
      this.fit_item.Name = "fit_item";
      this.fit_item.Size = new System.Drawing.Size(152, 22);
      this.fit_item.Text = "Fit";
      this.fit_item.Click += new System.EventHandler(this.fit_item_Click);
      // 
      // PreviewControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.ContextMenuStrip = this.fit_menu;
      this.Name = "PreviewControl";
      this.Load += new System.EventHandler(this.PreviewControl_Load);
      this.SizeChanged += new System.EventHandler(this.PreviewControl_SizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewControl_Paint);
      this.fit_menu.ResumeLayout(false);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer capture_timer;
        private System.Windows.Forms.ContextMenuStrip fit_menu;
        private System.Windows.Forms.ToolStripMenuItem fit_item;



    }
}
