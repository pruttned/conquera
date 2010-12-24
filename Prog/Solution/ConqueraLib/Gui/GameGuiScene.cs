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
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public class GameGuiScene : GuiScene
    {
        private GameScene mGameScene;
        private HeroInfoPanel mHeroPanel = new HeroInfoPanel();
        private TileInfoPanel mTilePanel = new TileInfoPanel();
        private MegaDebugLabel mDebugLabel = new MegaDebugLabel();
        private PlayerGoldView mPlayerGoldView = new PlayerGoldView();
        private PlayerUnitCountView mPlayerUnitCountView = new PlayerUnitCountView();
        private TextButton mMainMenuButton;

        public bool SidePanelsVisible
        {
            get { return mHeroPanel.Visible || mTilePanel.Visible; }
            set
            {
                mHeroPanel.Visible = value;
                mTilePanel.Visible = value;
            }
        }

        public string DebugText
        {
            get { return mDebugLabel.Text; }
            set { mDebugLabel.Text = value; }
        }

        public bool DebugTextVisible
        {
            get { return mDebugLabel.Visible; }
            set { mDebugLabel.Visible = value; }
        }

        public GameGuiScene(GameScene gameScene)
        {
            mGameScene = gameScene;
            BindToCurrentPlayer();            

            //Debug label.
            mDebugLabel.Visible = false;
            RootControls.Add(mDebugLabel);

            //Side panels.
            RootControls.Add(mHeroPanel);
            RootControls.Add(mTilePanel);

            //Player stats.
            mPlayerGoldView.Update(mGameScene.CurrentPlayer.Gold);
            mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
            RootControls.Add(mPlayerGoldView);
            RootControls.Add(mPlayerUnitCountView);

            //Main menu button.
            mMainMenuButton = new TextButton(GuiManager.Instance.Palette.CreateGraphicElement("ShowMainMenuButtonDefault"),
                GuiManager.Instance.Palette.CreateGraphicElement("ShowMainMenuButtonMouseOver"), GuiManager.Instance.GetGuiFont("SpriteFont1"),
                Color.White, "Menu");
            mMainMenuButton.Click += new EventHandler<ControlEventArgs>(mMainMenuButton_Click);
            RootControls.Add(mMainMenuButton);

            //Other.
            UpdateLocations();
            GuiManager.Instance.ScreenSizeChanged += new EventHandler(ScreenSizeChanged);
        }

        public void UpdateHexCell(HexCell cell)
        {
            if (cell == null || cell.IsGap)
            {
                SidePanelsVisible = false;
            }
            else
            {
                //Hero info.
                if (cell.GameUnit == null)
                {
                    mHeroPanel.Visible = false;
                }
                else
                {
                    mHeroPanel.Update(cell.GameUnit);
                    mHeroPanel.Visible = true;
                }

                //Tile info.
                if (cell.HexTerrainTile == null)
                {
                    mTilePanel.Visible = false;
                }
                else
                {
                    mTilePanel.Update(cell);
                    mTilePanel.Visible = true;
                }
            }
        }

        internal void HandleEndTurn(GamePlayer oldPlayer)
        {
            UnBindFromPlayer(oldPlayer);
            BindToCurrentPlayer();

            mPlayerGoldView.Update(mGameScene.CurrentPlayer.Gold);
            mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
        }

        private void BindToCurrentPlayer()
        {
            mGameScene.CurrentPlayer.GoldChanged += new EventHandler(Player_GoldChanged);
            mGameScene.CurrentPlayer.MaxUnitCntChanged += new EventHandler(Player_MaxUnitCntChanged);
            mGameScene.CurrentPlayer.UnitsChanged += new EventHandler(Player_UnitsChanged);
        }

        private void UnBindFromPlayer(GamePlayer player)
        {
            player.GoldChanged -= Player_GoldChanged;
            player.MaxUnitCntChanged -= Player_MaxUnitCntChanged;
            player.UnitsChanged -= Player_UnitsChanged;
        }

        private void Player_GoldChanged(object sender, EventArgs e)
        {
            mPlayerGoldView.Update(mGameScene.CurrentPlayer.Gold);
        }

        private void Player_MaxUnitCntChanged(object sender, EventArgs e)
        {
            mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
        }

        private void Player_UnitsChanged(object sender, EventArgs e)
        {
            mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
        }

        private void ScreenSizeChanged(object sender, EventArgs e)
        {
            UpdateLocations();
        }

        private void UpdateLocations()
        {
            int screenWidth = (int)GuiManager.Instance.ScreenSize.Width;
            int screenHeight = (int)GuiManager.Instance.ScreenSize.Height;

            //Side panels.
            mHeroPanel.Location = new Point(screenWidth - (int)mHeroPanel.Size.Width, 0);
            mTilePanel.Location = new Point(screenWidth - (int)mTilePanel.Size.Width, screenHeight - (int)mTilePanel.Size.Height);

            //Player stat views.
            mPlayerGoldView.Location = Point.Zero;
            mPlayerUnitCountView.Location = new Point(100, 0);

            //MainMenuButton.
            mMainMenuButton.Location = new Point((int)(screenWidth - mMainMenuButton.Size.Width), 0);
        }

        private void mMainMenuButton_Click(object sender, ControlEventArgs e)
        {
            MainMenuDialog dialog = new MainMenuDialog(mGameScene.SceneManager);
            dialog.Show(true);
        }
    }








    public class PlayerGoldView : Control
    {
        TextElement mGoldTextElement = new TextElement(GuiManager.Instance.GetGuiFont("SpriteFontSmall"), Color.Gold);

        public override System.Drawing.SizeF Size
        {
            get { return mGoldTextElement.Size; }
        }

        public void Update(int gold)
        {
            mGoldTextElement.Text = string.Format("Gold: {0}", gold);
        }

        protected override void OnDrawForeground()
        {
            mGoldTextElement.Draw(ScreenLocation);
        }
    }

    public class PlayerUnitCountView : Control
    {
        TextElement mCountTextElement = new TextElement(GuiManager.Instance.GetGuiFont("SpriteFontSmall"), Color.Black);

        public override System.Drawing.SizeF Size
        {
            get { return mCountTextElement.Size; }
        }

        public void Update(int count, int maxCount)
        {
            mCountTextElement.Text = string.Format("Units: {0}/{1}", count, maxCount);
        }

        protected override void OnDrawForeground()
        {
            mCountTextElement.Draw(ScreenLocation);
        }
    }

    public class MainMenuDialog : Dialog
    {
        private SceneManager mSceneManager;
        private GraphicElement mBackground;
        private GraphicElement mButtonBackgroundDefault = GuiManager.Instance.Palette.CreateGraphicElement("MainMenuButtonDefault");
        private GraphicElement mButtonBackgroundMouseOver = GuiManager.Instance.Palette.CreateGraphicElement("MainMenuButtonMouseOver");
        private GuiFont mButtonFont = GuiManager.Instance.GetGuiFont("SpriteFont1");
        private TextButton mQuitButton;
        private TextButton mContinueButton;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public MainMenuDialog(SceneManager sceneManager)
        {
            mSceneManager = sceneManager;
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("MainMenuDialogBackground");

            //Quit button.
            mQuitButton = new TextButton(mButtonBackgroundDefault, mButtonBackgroundMouseOver, mButtonFont, Color.White, "Quit");
            mQuitButton.Location = new Point(200, 100);
            mQuitButton.Click += new EventHandler<ControlEventArgs>(mQuitButton_Click);
            ChildControls.Add(mQuitButton);

            //Continue button.
            mContinueButton = new TextButton(mButtonBackgroundDefault, mButtonBackgroundMouseOver, mButtonFont, Color.White, "Continue");
            mContinueButton.Location = new Point(200, 200);
            mContinueButton.Click += new EventHandler<ControlEventArgs>(mContinueButton_Click);
            ChildControls.Add(mContinueButton);
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        private void mQuitButton_Click(object sender, ControlEventArgs e)
        {
            mSceneManager.ExitApplication();
        }

        private void mContinueButton_Click(object sender, ControlEventArgs e)
        {
            Hide(); //todo if not in game, load last save
        }
    }

    public class TextButton : GraphicButton
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        public TextButton(GraphicElement defaultGraphicElement, GraphicElement mouseOverGraphicElement, GuiFont font, Color textColor, string text)
            :base(defaultGraphicElement, mouseOverGraphicElement)
        {
            mTextElement = new TextElement(font, textColor);
            mTextElement.Text = text;
        }

        protected override void OnDrawForeground()
        {
            Point location = new Point((int)(ScreenLocation.X + Size.Width / 2 - mTextElement.Width / 2),
                                       (int)(ScreenLocation.Y + Size.Height / 2 - mTextElement.Height / 2));
            mTextElement.Draw(location);
        }
    }
}