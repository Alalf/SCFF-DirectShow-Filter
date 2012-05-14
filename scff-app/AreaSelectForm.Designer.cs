namespace scff_app
{
    partial class AreaSelectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.kDoubleClick = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // kDoubleClick
      // 
      this.kDoubleClick.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.kDoubleClick.Enabled = false;
      this.kDoubleClick.Location = new System.Drawing.Point(12, 9);
      this.kDoubleClick.Name = "kDoubleClick";
      this.kDoubleClick.Size = new System.Drawing.Size(135, 68);
      this.kDoubleClick.TabIndex = 0;
      this.kDoubleClick.Text = "Double-click to Apply";
      this.kDoubleClick.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // AreaSelectForm
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
      this.ClientSize = new System.Drawing.Size(157, 86);
      this.ControlBox = false;
      this.Controls.Add(this.kDoubleClick);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "AreaSelectForm";
      this.Opacity = 0.5D;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "AreaSelectForm";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AreaSelectForm_FormClosed);
      this.Shown += new System.EventHandler(this.AreaSelectForm_Shown);
      this.DoubleClick += new System.EventHandler(this.AreaSelectForm_DoubleClick);
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label kDoubleClick;

    }
}