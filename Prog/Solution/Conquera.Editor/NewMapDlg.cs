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
