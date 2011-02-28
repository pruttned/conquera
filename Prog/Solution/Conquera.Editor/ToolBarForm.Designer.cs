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
            this.mNewMapButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mNewMapButton
            // 
            this.mNewMapButton.Location = new System.Drawing.Point(12, 12);
            this.mNewMapButton.Name = "mNewMapButton";
            this.mNewMapButton.Size = new System.Drawing.Size(68, 36);
            this.mNewMapButton.TabIndex = 0;
            this.mNewMapButton.Text = "NewMap";
            this.mNewMapButton.UseVisualStyleBackColor = true;
            this.mNewMapButton.Click += new System.EventHandler(this.mNewMapButton_Click);
            // 
            // ToolBarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 539);
            this.ControlBox = false;
            this.Controls.Add(this.mNewMapButton);
            this.Name = "ToolBarForm";
            this.Text = "ToolBarForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button mNewMapButton;
    }
}