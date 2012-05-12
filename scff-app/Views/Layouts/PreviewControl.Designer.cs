namespace ScffApp.Views.Layouts
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
            this.innerPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.innerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // innerPanel
            // 
            this.innerPanel.BackColor = System.Drawing.Color.ForestGreen;
            this.innerPanel.Controls.Add(this.label1);
            this.innerPanel.Location = new System.Drawing.Point(2, 2);
            this.innerPanel.Name = "innerPanel";
            this.innerPanel.Size = new System.Drawing.Size(146, 146);
            this.innerPanel.TabIndex = 0;
            this.innerPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseDown);
            this.innerPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseMove);
            this.innerPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseUp);
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
            this.Controls.Add(this.innerPanel);
            this.Name = "PreviewControl";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PreviewControl_MouseUp);
            this.innerPanel.ResumeLayout(false);
            this.innerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel innerPanel;
        private System.Windows.Forms.Label label1;
    }
}
