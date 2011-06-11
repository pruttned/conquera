namespace Conquera.Editor
{
    partial class ToolBarForm
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
            this.mTileBrushListBox = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.mUnitModeRadioButton = new System.Windows.Forms.RadioButton();
            this.mRegionModeRadioButton = new System.Windows.Forms.RadioButton();
            this.mTileModeRadioButton = new System.Windows.Forms.RadioButton();
            this.mPlayerListBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.mUnitListBox = new System.Windows.Forms.ListBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mTileBrushListBox
            // 
            this.mTileBrushListBox.FormattingEnabled = true;
            this.mTileBrushListBox.Location = new System.Drawing.Point(11, 369);
            this.mTileBrushListBox.Name = "mTileBrushListBox";
            this.mTileBrushListBox.Size = new System.Drawing.Size(195, 290);
            this.mTileBrushListBox.TabIndex = 1;
            this.mTileBrushListBox.SelectedIndexChanged += new System.EventHandler(this.mTileBrushListBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.mUnitModeRadioButton);
            this.groupBox1.Controls.Add(this.mRegionModeRadioButton);
            this.groupBox1.Controls.Add(this.mTileModeRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(0, 42);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(221, 56);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "EditMode";
            // 
            // mUnitModeRadioButton
            // 
            this.mUnitModeRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.mUnitModeRadioButton.AutoSize = true;
            this.mUnitModeRadioButton.Location = new System.Drawing.Point(142, 19);
            this.mUnitModeRadioButton.Name = "mUnitModeRadioButton";
            this.mUnitModeRadioButton.Size = new System.Drawing.Size(36, 23);
            this.mUnitModeRadioButton.TabIndex = 6;
            this.mUnitModeRadioButton.Text = "Unit";
            this.mUnitModeRadioButton.UseVisualStyleBackColor = true;
            this.mUnitModeRadioButton.CheckedChanged += new System.EventHandler(this.mUnitModeRadioButton_CheckedChanged);
            // 
            // mRegionModeRadioButton
            // 
            this.mRegionModeRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.mRegionModeRadioButton.AutoSize = true;
            this.mRegionModeRadioButton.Location = new System.Drawing.Point(75, 19);
            this.mRegionModeRadioButton.Name = "mRegionModeRadioButton";
            this.mRegionModeRadioButton.Size = new System.Drawing.Size(51, 23);
            this.mRegionModeRadioButton.TabIndex = 5;
            this.mRegionModeRadioButton.Text = "Region";
            this.mRegionModeRadioButton.UseVisualStyleBackColor = true;
            this.mRegionModeRadioButton.CheckedChanged += new System.EventHandler(this.mRegionModeRadioButton_CheckedChanged);
            // 
            // mTileModeRadioButton
            // 
            this.mTileModeRadioButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.mTileModeRadioButton.AutoSize = true;
            this.mTileModeRadioButton.Checked = true;
            this.mTileModeRadioButton.Location = new System.Drawing.Point(17, 19);
            this.mTileModeRadioButton.Name = "mTileModeRadioButton";
            this.mTileModeRadioButton.Size = new System.Drawing.Size(34, 23);
            this.mTileModeRadioButton.TabIndex = 4;
            this.mTileModeRadioButton.TabStop = true;
            this.mTileModeRadioButton.Text = "Tile";
            this.mTileModeRadioButton.UseVisualStyleBackColor = true;
            this.mTileModeRadioButton.CheckedChanged += new System.EventHandler(this.mTileModeRadioButton_CheckedChanged);
            // 
            // mPlayerListBox
            // 
            this.mPlayerListBox.FormattingEnabled = true;
            this.mPlayerListBox.Location = new System.Drawing.Point(44, 126);
            this.mPlayerListBox.Name = "mPlayerListBox";
            this.mPlayerListBox.Size = new System.Drawing.Size(120, 43);
            this.mPlayerListBox.TabIndex = 4;
            this.mPlayerListBox.SelectedIndexChanged += new System.EventHandler(this.mPlayerListBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 109);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Player";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 353);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Tile Brush";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 190);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Unit";
            // 
            // mUnitListBox
            // 
            this.mUnitListBox.FormattingEnabled = true;
            this.mUnitListBox.Location = new System.Drawing.Point(11, 204);
            this.mUnitListBox.Name = "mUnitListBox";
            this.mUnitListBox.Size = new System.Drawing.Size(195, 134);
            this.mUnitListBox.TabIndex = 9;
            this.mUnitListBox.SelectedIndexChanged += new System.EventHandler(this.mUnitListBox_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(221, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.loadToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // ToolBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 671);
            this.ControlBox = false;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.mUnitListBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mPlayerListBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.mTileBrushListBox);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ToolBarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ToolBarForm";
            this.Load += new System.EventHandler(this.ToolBarForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox mTileBrushListBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton mTileModeRadioButton;
        private System.Windows.Forms.RadioButton mUnitModeRadioButton;
        private System.Windows.Forms.RadioButton mRegionModeRadioButton;
        private System.Windows.Forms.ListBox mPlayerListBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox mUnitListBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
    }
}