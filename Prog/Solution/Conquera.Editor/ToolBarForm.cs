using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Conquera.Editor
{
    public partial class ToolBarForm : Form
    {
        public EditorApplication EditorApplication { get; set; }
        public Control RenderWindow { get; set; }

        public ToolBarForm()
        {
            InitializeComponent();
        }

        private void mNewMapButton_Click(object sender, EventArgs e)
        {
            using (NewMapDlg newMapDlg = new NewMapDlg())
            {
                if(DialogResult.OK == newMapDlg.ShowDialog())
                {
                    var newMapInfo = newMapDlg.NewMapInfo;
                    EditorApplication.CommandQueue.Enqueue(new NewMapCommand(newMapInfo));
                    RenderWindow.Focus();
                }
            }
            
        }
    }
}
