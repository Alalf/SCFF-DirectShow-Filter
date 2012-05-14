namespace scff_app.gui {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutForm));
      this.main_strip = new System.Windows.Forms.ToolStrip();
      this.add_item = new System.Windows.Forms.ToolStripButton();
      this.remove_item = new System.Windows.Forms.ToolStripButton();
      this.apply_item = new System.Windows.Forms.ToolStripButton();
      this.layout_panel = new System.Windows.Forms.Panel();
      this.cancel_item = new System.Windows.Forms.ToolStripButton();
      this.main_strip.SuspendLayout();
      this.SuspendLayout();
      // 
      // main_strip
      // 
      this.main_strip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.main_strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.add_item,
            this.remove_item,
            this.cancel_item,
            this.apply_item});
      this.main_strip.Location = new System.Drawing.Point(0, 0);
      this.main_strip.Name = "main_strip";
      this.main_strip.Size = new System.Drawing.Size(31, 25);
      this.main_strip.TabIndex = 0;
      this.main_strip.Text = "toolStrip1";
      // 
      // add_item
      // 
      this.add_item.Enabled = false;
      this.add_item.Image = ((System.Drawing.Image)(resources.GetObject("add_item.Image")));
      this.add_item.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.add_item.Name = "add_item";
      this.add_item.Size = new System.Drawing.Size(45, 20);
      this.add_item.Text = "Add";
      this.add_item.Click += new System.EventHandler(this.add_item_Click);
      // 
      // remove_item
      // 
      this.remove_item.Enabled = false;
      this.remove_item.Image = ((System.Drawing.Image)(resources.GetObject("remove_item.Image")));
      this.remove_item.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.remove_item.Name = "remove_item";
      this.remove_item.Size = new System.Drawing.Size(66, 20);
      this.remove_item.Text = "Remove";
      this.remove_item.Click += new System.EventHandler(this.remove_item_Click);
      // 
      // apply_item
      // 
      this.apply_item.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.apply_item.Image = ((System.Drawing.Image)(resources.GetObject("apply_item.Image")));
      this.apply_item.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.apply_item.Name = "apply_item";
      this.apply_item.Size = new System.Drawing.Size(54, 20);
      this.apply_item.Text = "Apply";
      this.apply_item.Click += new System.EventHandler(this.apply_item_Click);
      // 
      // layout_panel
      // 
      this.layout_panel.BackColor = System.Drawing.Color.Black;
      this.layout_panel.Location = new System.Drawing.Point(0, 25);
      this.layout_panel.Margin = new System.Windows.Forms.Padding(0);
      this.layout_panel.Name = "layout_panel";
      this.layout_panel.Size = new System.Drawing.Size(38, 39);
      this.layout_panel.TabIndex = 1;
      // 
      // cancel_item
      // 
      this.cancel_item.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.cancel_item.Image = ((System.Drawing.Image)(resources.GetObject("cancel_item.Image")));
      this.cancel_item.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.cancel_item.Name = "cancel_item";
      this.cancel_item.Size = new System.Drawing.Size(60, 20);
      this.cancel_item.Text = "Cancel";
      this.cancel_item.Click += new System.EventHandler(this.cancel_item_Click);
      // 
      // LayoutForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.ClientSize = new System.Drawing.Size(31, 57);
      this.Controls.Add(this.layout_panel);
      this.Controls.Add(this.main_strip);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LayoutForm";
      this.Text = "LayoutForm";
      this.main_strip.ResumeLayout(false);
      this.main_strip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip main_strip;
    private System.Windows.Forms.Panel layout_panel;
    private System.Windows.Forms.ToolStripButton add_item;
    private System.Windows.Forms.ToolStripButton remove_item;
    private System.Windows.Forms.ToolStripButton apply_item;
    private System.Windows.Forms.ToolStripButton cancel_item;



  }
}