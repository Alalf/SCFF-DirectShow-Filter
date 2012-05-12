namespace ScffApp.Views.Layouts
{
    partial class LayoutForm
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addingToolStripMenuItem,
            this.removingToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(125, 48);
            // 
            // addingToolStripMenuItem
            // 
            this.addingToolStripMenuItem.Name = "addingToolStripMenuItem";
            this.addingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.addingToolStripMenuItem.Text = "Add";
            this.addingToolStripMenuItem.Click += new System.EventHandler(this.addingToolStripMenuItem_Click);
            // 
            // removingToolStripMenuItem
            // 
            this.removingToolStripMenuItem.Name = "removingToolStripMenuItem";
            this.removingToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.removingToolStripMenuItem.Text = "Remove";
            // 
            // LayoutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Name = "LayoutForm";
            this.Text = "LayoutForm";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removingToolStripMenuItem;

    }
}