namespace Ale
{
    partial class AleRenderControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mTimer
            // 
            this.mTimer.Interval = 16;
            this.mTimer.Tick += new System.EventHandler(this.mTimer_Tick);
            // 
            // AleRenderControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AleRenderControl";
            this.Size = new System.Drawing.Size(384, 376);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer mTimer;
    }
}
