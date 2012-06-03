namespace scff_app.view {
  partial class LayoutForm {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.mainToolBar = new System.Windows.Forms.ToolStrip();
      this.add = new System.Windows.Forms.ToolStripButton();
      this.remove = new System.Windows.Forms.ToolStripButton();
      this.cancel = new System.Windows.Forms.ToolStripButton();
      this.apply = new System.Windows.Forms.ToolStripButton();
      this.layoutPanel = new scff_app.view.LayoutPanel();
      this.mainToolBar.SuspendLayout();
      this.SuspendLayout();
      // 
      // mainToolBar
      // 
      this.mainToolBar.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.mainToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.add,
            this.remove,
            this.cancel,
            this.apply});
      this.mainToolBar.Location = new System.Drawing.Point(0, 0);
      this.mainToolBar.Name = "mainToolBar";
      this.mainToolBar.Size = new System.Drawing.Size(159, 25);
      this.mainToolBar.TabIndex = 0;
      this.mainToolBar.Text = "toolStrip1";
      // 
      // add
      // 
      this.add.Enabled = false;
      this.add.Image = global::scff_app.Properties.Resources.Add;
      this.add.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.add.Name = "add";
      this.add.Size = new System.Drawing.Size(45, 22);
      this.add.Text = "Add";
      this.add.Visible = false;
      this.add.Click += new System.EventHandler(this.add_Click);
      // 
      // remove
      // 
      this.remove.Enabled = false;
      this.remove.Image = global::scff_app.Properties.Resources.Remove;
      this.remove.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.remove.Name = "remove";
      this.remove.Size = new System.Drawing.Size(66, 22);
      this.remove.Text = "Remove";
      this.remove.Visible = false;
      this.remove.Click += new System.EventHandler(this.remove_Click);
      // 
      // cancel
      // 
      this.cancel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.cancel.Image = global::scff_app.Properties.Resources.Remove;
      this.cancel.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.cancel.Name = "cancel";
      this.cancel.Size = new System.Drawing.Size(60, 22);
      this.cancel.Text = "Cancel";
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // apply
      // 
      this.apply.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.apply.Image = global::scff_app.Properties.Resources.Go;
      this.apply.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.apply.Name = "apply";
      this.apply.Size = new System.Drawing.Size(52, 22);
      this.apply.Text = "apply";
      this.apply.Click += new System.EventHandler(this.apply_Click);
      // 
      // layoutPanel
      // 
      this.layoutPanel.BackColor = System.Drawing.Color.Black;
      this.layoutPanel.DataMember = null;
      this.layoutPanel.DataSource = null;
      this.layoutPanel.Location = new System.Drawing.Point(0, 25);
      this.layoutPanel.Name = "layoutPanel";
      this.layoutPanel.Size = new System.Drawing.Size(159, 116);
      this.layoutPanel.TabIndex = 1;
      // 
      // LayoutForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.ClientSize = new System.Drawing.Size(159, 141);
      this.Controls.Add(this.layoutPanel);
      this.Controls.Add(this.mainToolBar);
      this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::scff_app.Properties.Settings.Default, "LayoutFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Location = global::scff_app.Properties.Settings.Default.LayoutFormLocation;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LayoutForm";
      this.Text = "LayoutForm";
      this.Load += new System.EventHandler(this.LayoutForm_Load);
      this.mainToolBar.ResumeLayout(false);
      this.mainToolBar.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip mainToolBar;
    private System.Windows.Forms.ToolStripButton add;
    private System.Windows.Forms.ToolStripButton remove;
    private System.Windows.Forms.ToolStripButton apply;
    private System.Windows.Forms.ToolStripButton cancel;
    private LayoutPanel layoutPanel;



  }
}