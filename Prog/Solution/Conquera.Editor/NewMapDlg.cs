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
    public partial class NewMapDlg : Form
    {
        public NewMapInfo NewMapInfo
        {
            get
            {
                NewMapInfo inf = new NewMapInfo();
                inf.Name = mMapNameTextBox.Text;
                inf.Width = (int)mWidthNumericUpDown.Value;
                inf.Height = (int)mHeightNumericUpDown.Value;

                return inf;
            }
        }
        public NewMapDlg()
        {
            InitializeComponent();
        }
    }
}
