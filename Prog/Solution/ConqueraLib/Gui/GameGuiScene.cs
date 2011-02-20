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
        private GameUnitInfoPanel mGameUnitInfoPanel = new GameUnitInfoPanel();
        private TileInfoPanel mTilePanel = new TileInfoPanel();
        private MegaDebugLabel mDebugLabel = new MegaDebugLabel();
        private PlayerGoldView mPlayerGoldView = new PlayerGoldView();
        private PlayerUnitCountView mPlayerUnitCountView = new PlayerUnitCountView();
        private ConqueraTextButton mMainMenuButton;
        private SpellPanel mSpellPanel;
        private ConqueraTextButton mEndTurnButton;
        private CurrentPlayerDisplay mCurrentPlayerDisplay;

        public bool SidePanelsVisible
        {
            get { return mGameUnitInfoPanel.Visible || mTilePanel.Visible; }
            set
            {
                mGameUnitInfoPanel.Visible = value;
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
            RootControls.Add(mGameUnitInfoPanel);
            RootControls.Add(mTilePanel);

            //Player stats.
            mPlayerGoldView.Update(mGameScene.CurrentPlayer.Gold);
            mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
            RootControls.Add(mPlayerGoldView);
            RootControls.Add(mPlayerUnitCountView);

            //Main menu button.
            mMainMenuButton = new ConqueraTextButton("Menu");
            mMainMenuButton.Click += new EventHandler<ControlEventArgs>(mMainMenuButton_Click);
            RootControls.Add(mMainMenuButton);

            //Spell panel.
            mSpellPanel = new SpellPanel(gameScene);
            RootControls.Add(mSpellPanel);
            UpdateSpellPanel();

            //End turn button.
            mEndTurnButton = new ConqueraTextButton("EndTurn");
            mEndTurnButton.Click += new EventHandler<ControlEventArgs>(mEndTurnButton_Click);            
            RootControls.Add(mEndTurnButton);

            //Current player display.
            mCurrentPlayerDisplay = new CurrentPlayerDisplay();
            mCurrentPlayerDisplay.SetPlayer(mGameScene.CurrentPlayer);
            RootControls.Add(mCurrentPlayerDisplay);

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
                //Unit info.
                if (cell.GameUnit == null)
                {
                    mGameUnitInfoPanel.Visible = false;
                }
                else
                {
                    mGameUnitInfoPanel.Update(cell.GameUnit);
                    mGameUnitInfoPanel.Visible = true;
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

            UpdateSpellPanel();

            mCurrentPlayerDisplay.SetPlayer(mGameScene.CurrentPlayer);
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
            mGameUnitInfoPanel.Location = new Point(screenWidth - (int)mGameUnitInfoPanel.Size.Width, 0);
            mTilePanel.Location = new Point(screenWidth - (int)mTilePanel.Size.Width, screenHeight - (int)mTilePanel.Size.Height);

            //Player stat views.
            mPlayerGoldView.Location = Point.Zero;
            mPlayerUnitCountView.Location = new Point(100, 0);

            //MainMenuButton.
            mMainMenuButton.Location = new Point((int)(screenWidth - mMainMenuButton.Size.Width), 0);

            //Spell panel.
            mSpellPanel.Location = new Point(0, screenHeight - (int)mSpellPanel.Size.Height);

            //EndTurnButton.
            mEndTurnButton.Location = new Point(screenWidth - (int)mEndTurnButton.Size.Width, 
                                                mMainMenuButton.Location.Y + (int)mMainMenuButton.Size.Height + 2);

            //Current player display.
            mCurrentPlayerDisplay.Location = new Point(screenWidth / 2 - (int)mCurrentPlayerDisplay.Size.Width / 2, 0);
        }

        private void mMainMenuButton_Click(object sender, ControlEventArgs e)
        {
            MainMenuDialog dialog = new MainMenuDialog(mGameScene);
            dialog.Show(true);
        }

        private void UpdateSpellPanel()
        {
            mSpellPanel.SpellSlotCollection = mGameScene.CurrentPlayer.Spells;
        }

        private void mEndTurnButton_Click(object sender, ControlEventArgs e)
        {
            mGameScene.EndTurn();
        }
    }








    public class PlayerGoldView : Control
    {
        TextElement mGoldTextElement = new TextElement(ConqueraFonts.SpriteFontSmall, Color.Gold);

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
        TextElement mCountTextElement = new TextElement(ConqueraFonts.SpriteFontSmall, Color.Black);

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
        private ConqueraTextButton mQuitButton;
        private ConqueraTextButton mContinueButton;
        private GameScene mGameScene;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public MainMenuDialog(GameScene gameScene)
        {
            mGameScene = gameScene;
            mBackground = ConqueraPalette.MainMenuBackground;

            //Quit button.
            mQuitButton = new ConqueraTextButton("ToMainMenu");
            mQuitButton.Location = new Point(200, 100);
            mQuitButton.Click += new EventHandler<ControlEventArgs>(mQuitButton_Click);
            ChildControls.Add(mQuitButton);

            //Continue button.
            mContinueButton = new ConqueraTextButton("Continue");
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
            mGameScene.ExitToMainMenu();
        }

        private void mContinueButton_Click(object sender, ControlEventArgs e)
        {
            Hide();
        }
    }    

    public class CurrentPlayerDisplay : Control
    {
        private TextElement mPlayerColorLabel;

        public override System.Drawing.SizeF Size
        {
            get { return mPlayerColorLabel.Size; }
        }

        public CurrentPlayerDisplay()
        {
            mPlayerColorLabel = new TextElement(ConqueraFonts.SpriteFont1, Color.Black);
        }

        public void SetPlayer(GamePlayer player)
        {            
            mPlayerColorLabel.Color = new Color(player.Color);
            mPlayerColorLabel.Text = mPlayerColorLabel.Color.ToString();
        }

        protected override void OnDrawForeground()
        {
            mPlayerColorLabel.Draw(ScreenLocation);
        }
    }
}