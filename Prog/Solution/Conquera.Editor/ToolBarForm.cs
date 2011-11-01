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
    public partial class ToolBarForm : Form
    {
        //public EditorApplication EditorApplication { get; set; }
        //public Control RenderWindow { get; set; }

        public ToolBarForm()
        {
            InitializeComponent();
        }

        public void InitPlayers()
        {
            //mPlayerListBox.Items.Clear();
            //foreach (var player in EditorApplication.Players)
            //{
            //    mPlayerListBox.Items.Add(player);
            //}
            //mPlayerListBox.SelectedIndex = 0;
        }

        private void ToolBarForm_Load(object sender, EventArgs e)
        {
            //mTileBrushListBox.Items.Add("Gap");
            //foreach (var brush in EditorApplication.TileBrushes)
            //{
            //    mTileBrushListBox.Items.Add(brush);
            //}
            //mTileBrushListBox.SelectedIndex = 0;

            //InitPlayers();

            //foreach (var unit in EditorApplication.UnitTypes)
            //{
            //    mUnitListBox.Items.Add(unit);
            //}
            //mUnitListBox.SelectedIndex = 0;

            //mTileModeRadioButton.Checked = true;
        }

        private void mTileBrushListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //string brushName = (0 == mTileBrushListBox.SelectedIndex) ? null : (string)mTileBrushListBox.SelectedItem;
            //mTileModeRadioButton.Checked = true;
            //EditorApplication.CommandQueue.Enqueue(new SetTileBrushCommand(brushName));
        }

        private void mTileModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            //if (mTileModeRadioButton.Checked)
            //{
            //    EditorApplication.CommandQueue.Enqueue(new SetEditModeCommand(EditMode.TileEdit));
            //}
        }

        private void mRegionModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            //if (mRegionModeRadioButton.Checked)
            //{
            //    EditorApplication.CommandQueue.Enqueue(new SetEditModeCommand(EditMode.RegionEdit));
            //}
        }

        private void mUnitModeRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            //if (mUnitModeRadioButton.Checked)
            //{
            //    EditorApplication.CommandQueue.Enqueue(new SetEditModeCommand(EditMode.UnitEdit));
            //}
        }

        private void mPlayerListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var p = (BattlePlayer)mPlayerListBox.SelectedItem;
            //EditorApplication.CommandQueue.Enqueue(new SetPlayerCommand(p));
        }

        private void mUnitListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //mUnitModeRadioButton.Checked = true;
            //EditorApplication.CommandQueue.Enqueue(new SetUnitTypeCommand((string)mUnitListBox.SelectedItem));
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using (NewMapDlg newMapDlg = new NewMapDlg())
            //{
            //    if (DialogResult.OK == newMapDlg.ShowDialog())
            //    {
            //        //InitPlayers();
            //        EditorApplication.CommandQueue.Enqueue(new NewMapCommand(newMapDlg.NewMapInfo, this));
            //        RenderWindow.Focus();
            //    }
            //}
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //EditorApplication.CommandQueue.Enqueue(new SaveMapCommand(this));
            //RenderWindow.Focus();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //using (LoadMapDlg loadMapDlg = new LoadMapDlg())
            //{
            //    if (DialogResult.OK == loadMapDlg.ShowDialog(this))
            //    {
            //        EditorApplication.CommandQueue.Enqueue(new LoadMapCommand(loadMapDlg.MapFile, this));
            //        RenderWindow.Focus();
            //    }
            //}
        }
    }
}
