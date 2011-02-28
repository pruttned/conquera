namespace Conquera.Editor
{
    partial class NewMapDlg
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
            this.mMapNameTextBox = new System.Windows.Forms.TextBox();
            this.mWidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mHeightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.mCreateButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mWidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mHeightNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // mMapNameTextBox
            // 
            this.mMapNameTextBox.Location = new System.Drawing.Point(9, 11);
            this.mMapNameTextBox.Name = "mMapNameTextBox";
            this.mMapNameTextBox.Size = new System.Drawing.Size(246, 20);
            this.mMapNameTextBox.TabIndex = 0;
            this.mMapNameTextBox.Text = "NewMap";
            // 
            // mWidthNumericUpDown
            // 
            this.mWidthNumericUpDown.Location = new System.Drawing.Point(9, 37);
            this.mWidthNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mWidthNumericUpDown.Name = "mWidthNumericUpDown";
            this.mWidthNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.mWidthNumericUpDown.TabIndex = 1;
            this.mWidthNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // mHeightNumericUpDown
            // 
            this.mHeightNumericUpDown.Location = new System.Drawing.Point(135, 37);
            this.mHeightNumericUpDown.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.mHeightNumericUpDown.Name = "mHeightNumericUpDown";
            this.mHeightNumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.mHeightNumericUpDown.TabIndex = 2;
            this.mHeightNumericUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // mCreateButton
            // 
            this.mCreateButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.mCreateButton.Location = new System.Drawing.Point(12, 78);
            this.mCreateButton.Name = "mCreateButton";
            this.mCreateButton.Size = new System.Drawing.Size(99, 25);
            this.mCreateButton.TabIndex = 3;
            this.mCreateButton.Text = "Create";
            this.mCreateButton.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(151, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 25);
            this.button1.TabIndex = 4;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // NewMapDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 115);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.mCreateButton);
            this.Controls.Add(this.mHeightNumericUpDown);
            this.Controls.Add(this.mWidthNumericUpDown);
            this.Controls.Add(this.mMapNameTextBox);
            this.Name = "NewMapDlg";
            this.Text = "NewMapDlg";
            ((System.ComponentModel.ISupportInitialize)(this.mWidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mHeightNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox mMapNameTextBox;
        private System.Windows.Forms.NumericUpDown mWidthNumericUpDown;
        private System.Windows.Forms.NumericUpDown mHeightNumericUpDown;
        private System.Windows.Forms.Button mCreateButton;
        private System.Windows.Forms.Button button1;
    }
}