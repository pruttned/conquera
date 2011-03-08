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
using Ale.Gui;

namespace Conquera.Gui
{
    public class MainMenuGuiScene : GuiScene
    {
        private MainMenuScene mMainMenuScene;
        private ListBox mMapListBox;
        private ConqueraTextButton mLoadMapButton;

        public MainMenuGuiScene(MainMenuScene mainMenuScene)
        {
            mMainMenuScene = mainMenuScene;

            mMapListBox = new ListBox(HotseatGameScene.QueryMapFiles(mMainMenuScene.Content));
            mMapListBox.Location = new Microsoft.Xna.Framework.Point(200, 200);
            mMapListBox.SelectedItemChanged += new EventHandler<ListBox.SelectedItemChangedEventArgs>(mMapListBox_SelectedItemChanged);
            RootControls.Add(mMapListBox);

            mLoadMapButton = new ConqueraTextButton("Load");
            mLoadMapButton.IsHitTestEnabled = false;
            mLoadMapButton.Location = new Microsoft.Xna.Framework.Point(280, 505);
            mLoadMapButton.Click += new EventHandler<ControlEventArgs>(mLoadMapButton_Click);
            RootControls.Add(mLoadMapButton);
        }

        private void mMapListBox_SelectedItemChanged(object sender, ListBox.SelectedItemChangedEventArgs e)
        {
            mLoadMapButton.IsHitTestEnabled = true;
        }

        private void mLoadMapButton_Click(object sender, ControlEventArgs e)
        {
            HotseatGameScene scene = HotseatGameScene.Load(mMapListBox.SelectedItem, mMainMenuScene.SceneManager, mMainMenuScene.Content);
            mMainMenuScene.SceneManager.ActivateScene(scene);
        }
    }
}