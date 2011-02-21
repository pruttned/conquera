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