namespace Ale.Editor
{
    partial class RendererWindow
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
            this.mRenderControl = new Ale.AleRenderControl();
            this.SuspendLayout();
            // 
            // mRenderControl
            // 
            this.mRenderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mRenderControl.Location = new System.Drawing.Point(0, 0);
            this.mRenderControl.Name = "mRenderControl";
            this.mRenderControl.Size = new System.Drawing.Size(625, 506);
            this.mRenderControl.TabIndex = 0;
            // 
            // RendererWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 506);
            this.Controls.Add(this.mRenderControl);
            this.Name = "RendererWindow";
            this.Text = "RendererWindow";
            this.Load += new System.EventHandler(this.RendererWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private AleRenderControl mRenderControl;
    }
}