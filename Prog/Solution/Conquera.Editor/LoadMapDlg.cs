//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ale.Content;
using System.IO;

namespace Conquera.Editor
{
    public partial class LoadMapDlg : Form
    {
        private class ListBoxItem
        {
            public string Name { get; private set; }
            public string FileName { get; private set; }

            public ListBoxItem(string fileName)
            {
                Name = Path.GetFileNameWithoutExtension(fileName);
                FileName = fileName;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public string MapFile { get; private set; }

        public LoadMapDlg()
        {
            InitializeComponent();
        }

        private void LoadMapDlg_Load(object sender, EventArgs e)
        {
            foreach (string file in HotseatGameScene.QueryMapFiles())
            {
                listBox1.Items.Add(new ListBoxItem(file));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapFile = ((ListBoxItem)listBox1.SelectedItem).FileName;
            mLoadButton.Enabled = true;
        }
    }
}
