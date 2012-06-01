namespace scff_app.view
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
      this.cancel = new System.Windows.Forms.Button();
      this.kCaption = new System.Windows.Forms.Label();
      this.accept = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // cancel
      // 
      this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.cancel.Image = global::scff_app.Properties.Resources.Remove;
      this.cancel.Location = new System.Drawing.Point(276, 0);
      this.cancel.Name = "cancel";
      this.cancel.Size = new System.Drawing.Size(24, 24);
      this.cancel.TabIndex = 2;
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // kCaption
      // 
      this.kCaption.AutoSize = true;
      this.kCaption.Enabled = false;
      this.kCaption.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
      this.kCaption.Location = new System.Drawing.Point(12, 9);
      this.kCaption.Name = "kCaption";
      this.kCaption.Size = new System.Drawing.Size(138, 12);
      this.kCaption.TabIndex = 3;
      this.kCaption.Text = "Double-click to Apply";
      // 
      // accept
      // 
      this.accept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.accept.Image = global::scff_app.Properties.Resources.Go;
      this.accept.Location = new System.Drawing.Point(138, 63);
      this.accept.Name = "accept";
      this.accept.Size = new System.Drawing.Size(24, 24);
      this.accept.TabIndex = 4;
      this.accept.UseVisualStyleBackColor = true;
      this.accept.Visible = false;
      this.accept.Click += new System.EventHandler(this.accept_Click);
      // 
      // AreaSelectForm
      // 
      this.AcceptButton = this.accept;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
      this.CancelButton = this.cancel;
      this.ClientSize = new System.Drawing.Size(300, 150);
      this.ControlBox = false;
      this.Controls.Add(this.accept);
      this.Controls.Add(this.kCaption);
      this.Controls.Add(this.cancel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Name = "AreaSelectForm";
      this.Opacity = 0.75D;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "AreaSelectForm";
      this.Load += new System.EventHandler(this.AreaSelectForm_Load);
      this.DoubleClick += new System.EventHandler(this.accept_Click);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label kCaption;
        private System.Windows.Forms.Button accept;


    }
}