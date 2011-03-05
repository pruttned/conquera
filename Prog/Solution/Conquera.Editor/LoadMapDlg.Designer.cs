namespace Conquera.Editor
{
    partial class LoadMapDlg
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.mLoadButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(260, 238);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(173, 256);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 25);
            this.button1.TabIndex = 6;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // mLoadButton
            // 
            this.mLoadButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mLoadButton.Enabled = false;
            this.mLoadButton.Location = new System.Drawing.Point(12, 256);
            this.mLoadButton.Name = "mLoadButton";
            this.mLoadButton.Size = new System.Drawing.Size(99, 25);
            this.mLoadButton.TabIndex = 5;
            this.mLoadButton.Text = "Load";
            this.mLoadButton.UseVisualStyleBackColor = true;
            // 
            // LoadMapDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 289);
            this.ControlBox = false;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mLoadButton);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "LoadMapDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "LoadMap";
            this.Load += new System.EventHandler(this.LoadMapDlg_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button mLoadButton;
    }
}