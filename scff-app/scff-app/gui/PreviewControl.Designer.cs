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
      this.SuspendLayout();
      // 
      // capture_timer
      // 
      this.capture_timer.Interval = 3000;
      this.capture_timer.Tick += new System.EventHandler(this.capture_timer_Tick);
      // 
      // PreviewControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.Transparent;
      this.Name = "PreviewControl";
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.PreviewControl_Paint);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer capture_timer;



    }
}
