namespace scff_app.views.layouts
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
            this.label1 = new System.Windows.Forms.Label();
            this.inner_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // innerPanel
            // 
            this.inner_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inner_panel.BackColor = System.Drawing.Color.ForestGreen;
            this.inner_panel.Controls.Add(this.label1);
            this.inner_panel.Location = new System.Drawing.Point(2, 2);
            this.inner_panel.Name = "innerPanel";
            this.inner_panel.Size = new System.Drawing.Size(146, 146);
            this.inner_panel.TabIndex = 0;
            this.inner_panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseDown);
            this.inner_panel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseMove);
            this.inner_panel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.innerPanel_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // PreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.GreenYellow;
            this.Controls.Add(this.inner_panel);
            this.Name = "PreviewControl";
            this.inner_panel.ResumeLayout(false);
            this.inner_panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel inner_panel;
        private System.Windows.Forms.Label label1;
    }
}
