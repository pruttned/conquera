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
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Conquera.Gui
{
    public class GameGuiScene : GuiScene
    {
        private GameScene mGameScene;
        private MegaDebugLabel mDebugLabel = new MegaDebugLabel();
        private GraphicButton mIngameMenuButton;
        private ToolTipBar mToolTipBar;

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

            //Debug label
            mDebugLabel.Visible = false;
            mDebugLabel.IsHitTestEnabled = false;
            RootControls.Add(mDebugLabel);

            //Tool tip bar
            mToolTipBar = new ToolTipBar();
            RootControls.Add(mToolTipBar);

            //Ingame menu button
            mIngameMenuButton = new GraphicButton(ConqueraPalette.IngameMenuButtonDefault, ConqueraPalette.IngameMenuButtonOver);
            mIngameMenuButton.Click += new EventHandler<ControlEventArgs>(mIngameMenuButton_Click);
            mToolTipBar.SetToolTipTextToControl(mIngameMenuButton, "Ingame menu");
            RootControls.Add(mIngameMenuButton);

            //Other
            UpdateLocations();
            GuiManager.Instance.ScreenSizeChanged += new EventHandler(ScreenSizeChanged);
        }

        internal void HandleEndTurn(GamePlayer oldPlayer)
        {
        }

        private void mIngameMenuButton_Click(object sender, ControlEventArgs e)
        {
            MainMenuDialog dialog = new MainMenuDialog(mGameScene);
            dialog.Show(true);
        }

        private void ScreenSizeChanged(object sender, EventArgs e)
        {
            UpdateLocations();
        }

        private void UpdateLocations()
        {
            int screenWidth = (int)GuiManager.Instance.ScreenSize.Width;
            int screenHeight = (int)GuiManager.Instance.ScreenSize.Height;
            int screenMiddleX = screenWidth / 2;

            //Ingame menu button
            mIngameMenuButton.Location = new Point((int)(screenWidth - mIngameMenuButton.Size.Width), 0);

            //Tool tip bar
            mToolTipBar.Location = new Point((int)(screenMiddleX - mToolTipBar.Size.Width / 2), (int)(screenHeight - mToolTipBar.Size.Height));
        }
    }

    public class MainMenuDialog : Dialog
    {
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

    public class ToolTipBar : Control
    {
        private TextElement mTextElement = new TextElement(ConqueraFonts.SpriteFontSmall, Color.Black);
        private Dictionary<Control, string> mToolTipTextsByControl = new Dictionary<Control, string>();

        public override System.Drawing.SizeF Size
        {
            get { return ConqueraPalette.ToolTipBackground.Size; }
        }

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        public void SetToolTipTextToControl(Control control, string text)
        {
            if (mToolTipTextsByControl.Remove(control)) //old
            {
                control.MouseEnter -= control_MouseEnter;
                control.MouseLeave -= control_MouseLeave;
            }

            if (!string.IsNullOrEmpty(text)) //new
            {
                mToolTipTextsByControl.Add(control, text);
                control.MouseEnter += new EventHandler<ControlEventArgs>(control_MouseEnter);
                control.MouseLeave += new EventHandler<ControlEventArgs>(control_MouseLeave);
            }
        }

        protected override void OnDrawBackground()
        {
            ConqueraPalette.ToolTipBackground.Draw(ScreenLocation);
            mTextElement.Draw(new Point(ScreenLocation.X + (int)(Size.Width / 2 - mTextElement.Width / 2), ScreenLocation.Y + (int)(Size.Height / 2 - mTextElement.Height / 2)));
        }

        private void control_MouseEnter(object sender, ControlEventArgs e)
        {
            Text = mToolTipTextsByControl[e.Control];
        }

        private void control_MouseLeave(object sender, ControlEventArgs e)
        {
            Text = string.Empty;
        }
    }








    //public class GameGuiScene : GuiScene
    //{
    //    private GameScene mGameScene;
    //    private GameUnitInfoPanel mGameUnitInfoPanel = new GameUnitInfoPanel();
    //    private TileInfoPanel mTilePanel = new TileInfoPanel();
    //    private MegaDebugLabel mDebugLabel = new MegaDebugLabel();
    //    private PlayerManaView mPlayerManaView = new PlayerManaView();
    //    private PlayerUnitCountView mPlayerUnitCountView = new PlayerUnitCountView();
    //    private ConqueraTextButton mMainMenuButton;
    //    private SpellPanel mSpellPanel;
    //    private ConqueraTextButton mEndTurnButton;
    //    private CurrentPlayerDisplay mCurrentPlayerDisplay;

    //    public bool SidePanelsVisible
    //    {
    //        get { return mGameUnitInfoPanel.Visible || mTilePanel.Visible; }
    //        set
    //        {
    //            mGameUnitInfoPanel.Visible = value;
    //            mTilePanel.Visible = value;
    //        }
    //    }

    //    public string DebugText
    //    {
    //        get { return mDebugLabel.Text; }
    //        set { mDebugLabel.Text = value; }
    //    }

    //    public bool DebugTextVisible
    //    {
    //        get { return mDebugLabel.Visible; }
    //        set { mDebugLabel.Visible = value; }
    //    }

    //    public GameGuiScene(GameScene gameScene)
    //    {
    //        mGameScene = gameScene;
    //        BindToCurrentPlayer();            

    //        //Debug label.
    //        mDebugLabel.Visible = false;
    //        RootControls.Add(mDebugLabel);

    //        //Side panels.
    //        RootControls.Add(mGameUnitInfoPanel);
    //        RootControls.Add(mTilePanel);

    //        //Player stats.
    //        mPlayerManaView.Update(mGameScene.CurrentPlayer.Mana);
    //        mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
    //        RootControls.Add(mPlayerManaView);
    //        RootControls.Add(mPlayerUnitCountView);

    //        //Main menu button.
    //        mMainMenuButton = new ConqueraTextButton("Menu");
    //        mMainMenuButton.Click += new EventHandler<ControlEventArgs>(mMainMenuButton_Click);
    //        RootControls.Add(mMainMenuButton);

    //        //Spell panel.
    //        mSpellPanel = new SpellPanel(gameScene);
    //        RootControls.Add(mSpellPanel);
    //        UpdateSpellPanel();

    //        //End turn button.
    //        mEndTurnButton = new ConqueraTextButton("EndTurn");
    //        mEndTurnButton.Click += new EventHandler<ControlEventArgs>(mEndTurnButton_Click);            
    //        RootControls.Add(mEndTurnButton);

    //        //Current player display.
    //        mCurrentPlayerDisplay = new CurrentPlayerDisplay();
    //        mCurrentPlayerDisplay.SetPlayer(mGameScene.CurrentPlayer);
    //        RootControls.Add(mCurrentPlayerDisplay);

    //        UpdateLocations();
    //        GuiManager.Instance.ScreenSizeChanged += new EventHandler(ScreenSizeChanged);
    //    }        

    //    public void UpdateHexCell(HexCell cell)
    //    {
    //        if (cell == null || cell.IsGap)
    //        {
    //            SidePanelsVisible = false;
    //        }
    //        else
    //        {
    //            //Unit info.
    //            if (cell.GameUnit == null)
    //            {
    //                mGameUnitInfoPanel.Visible = false;
    //            }
    //            else
    //            {
    //                mGameUnitInfoPanel.Update(cell.GameUnit);
    //                mGameUnitInfoPanel.Visible = true;
    //            }

    //            //Tile info.
    //            if (cell.HexTerrainTile == null)
    //            {
    //                mTilePanel.Visible = false;
    //            }
    //            else
    //            {
    //                mTilePanel.Update(cell);
    //                mTilePanel.Visible = true;
    //            }
    //        }
    //    }

    //    internal void HandleEndTurn(GamePlayer oldPlayer)
    //    {
    //        UnBindFromPlayer(oldPlayer);
    //        BindToCurrentPlayer();

    //        mPlayerManaView.Update(mGameScene.CurrentPlayer.Mana);
    //        mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);            

    //        UpdateSpellPanel();

    //        mCurrentPlayerDisplay.SetPlayer(mGameScene.CurrentPlayer);
    //    }

    //    protected override void OnKeyDown(Keys key)
    //    {
    //        if (key >= Keys.D1 && key <= Keys.D9)
    //        {
    //            mSpellPanel.ToggleSpellSlotButton((int)key - (int)Keys.D1);
    //        }
    //    }

    //    private void BindToCurrentPlayer()
    //    {
    //        mGameScene.CurrentPlayer.ManaChanged += new EventHandler(Player_ManaChanged);
    //        mGameScene.CurrentPlayer.MaxUnitCntChanged += new EventHandler(Player_MaxUnitCntChanged);
    //        mGameScene.CurrentPlayer.UnitsChanged += new EventHandler(Player_UnitsChanged);
    //    }

    //    private void UnBindFromPlayer(GamePlayer player)
    //    {
    //        player.ManaChanged -= Player_ManaChanged;
    //        player.MaxUnitCntChanged -= Player_MaxUnitCntChanged;
    //        player.UnitsChanged -= Player_UnitsChanged;
    //    }

    //    private void Player_ManaChanged(object sender, EventArgs e)
    //    {
    //        mPlayerManaView.Update(mGameScene.CurrentPlayer.Mana);
    //    }

    //    private void Player_MaxUnitCntChanged(object sender, EventArgs e)
    //    {
    //        mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
    //    }

    //    private void Player_UnitsChanged(object sender, EventArgs e)
    //    {
    //        mPlayerUnitCountView.Update(mGameScene.CurrentPlayer.Units.Count, mGameScene.CurrentPlayer.MaxUnitCnt);
    //    }

    //    private void ScreenSizeChanged(object sender, EventArgs e)
    //    {
    //        UpdateLocations();
    //    }

    //    private void UpdateLocations()
    //    {
    //        int screenWidth = (int)GuiManager.Instance.ScreenSize.Width;
    //        int screenHeight = (int)GuiManager.Instance.ScreenSize.Height;

    //        //Side panels.
    //        mGameUnitInfoPanel.Location = new Point(screenWidth - (int)mGameUnitInfoPanel.Size.Width, 0);
    //        mTilePanel.Location = new Point(screenWidth - (int)mTilePanel.Size.Width, screenHeight - (int)mTilePanel.Size.Height);

    //        //Player stat views.
    //        mPlayerManaView.Location = Point.Zero;
    //        mPlayerUnitCountView.Location = new Point(100, 0);

    //        //MainMenuButton.
    //        mMainMenuButton.Location = new Point((int)(screenWidth - mMainMenuButton.Size.Width), 0);

    //        //Spell panel.
    //        mSpellPanel.Location = new Point(0, screenHeight - (int)mSpellPanel.Size.Height);

    //        //EndTurnButton.
    //        mEndTurnButton.Location = new Point(screenWidth - (int)mEndTurnButton.Size.Width, 
    //                                            mMainMenuButton.Location.Y + (int)mMainMenuButton.Size.Height + 2);

    //        //Current player display.
    //        mCurrentPlayerDisplay.Location = new Point(screenWidth / 2 - (int)mCurrentPlayerDisplay.Size.Width / 2, 0);
    //    }

    //    private void mMainMenuButton_Click(object sender, ControlEventArgs e)
    //    {
    //        MainMenuDialog dialog = new MainMenuDialog(mGameScene);
    //        dialog.Show(true);
    //    }

    //    private void UpdateSpellPanel()
    //    {
    //        mSpellPanel.SpellSlotCollection = mGameScene.Spells;
    //    }

    //    private void mEndTurnButton_Click(object sender, ControlEventArgs e)
    //    {
    //        mGameScene.EndTurn();
    //    }
    //}

    //public class PlayerManaView : Control
    //{
    //    TextElement mManaTextElement = new TextElement(ConqueraFonts.SpriteFont1, Color.White);

    //    public override System.Drawing.SizeF Size
    //    {
    //        get { return mManaTextElement.Size; }
    //    }

    //    public void Update(int mana)
    //    {
    //        mManaTextElement.Text = string.Format("Mana: {0}", mana);
    //    }

    //    protected override void OnDrawForeground()
    //    {
    //        mManaTextElement.Draw(ScreenLocation);
    //    }
    //}

    //public class PlayerUnitCountView : Control
    //{
    //    TextElement mCountTextElement = new TextElement(ConqueraFonts.SpriteFont1, Color.White);

    //    public override System.Drawing.SizeF Size
    //    {
    //        get { return mCountTextElement.Size; }
    //    }

    //    public void Update(int count, int maxCount)
    //    {
    //        mCountTextElement.Text = string.Format("Units: {0}/{1}", count, maxCount);
    //    }

    //    protected override void OnDrawForeground()
    //    {
    //        mCountTextElement.Draw(ScreenLocation);
    //    }
    //}

    //public class MainMenuDialog : Dialog
    //{        
    //    private GraphicElement mBackground;
    //    private ConqueraTextButton mQuitButton;
    //    private ConqueraTextButton mContinueButton;
    //    private GameScene mGameScene;

    //    public override System.Drawing.SizeF Size
    //    {
    //        get { return mBackground.Size; }
    //    }

    //    public MainMenuDialog(GameScene gameScene)
    //    {
    //        mGameScene = gameScene;
    //        mBackground = ConqueraPalette.MainMenuBackground;

    //        //Quit button.
    //        mQuitButton = new ConqueraTextButton("ToMainMenu");
    //        mQuitButton.Location = new Point(200, 100);
    //        mQuitButton.Click += new EventHandler<ControlEventArgs>(mQuitButton_Click);
    //        ChildControls.Add(mQuitButton);

    //        //Continue button.
    //        mContinueButton = new ConqueraTextButton("Continue");
    //        mContinueButton.Location = new Point(200, 200);
    //        mContinueButton.Click += new EventHandler<ControlEventArgs>(mContinueButton_Click);
    //        ChildControls.Add(mContinueButton);
    //    }

    //    protected override void OnDrawBackground()
    //    {
    //        mBackground.Draw(ScreenLocation);
    //    }

    //    private void mQuitButton_Click(object sender, ControlEventArgs e)
    //    {
    //        mGameScene.ExitToMainMenu();
    //    }

    //    private void mContinueButton_Click(object sender, ControlEventArgs e)
    //    {
    //        Hide();
    //    }
    //}    

    //public class CurrentPlayerDisplay : Control
    //{
    //    private TextElement mPlayerColorLabel;

    //    public override System.Drawing.SizeF Size
    //    {
    //        get { return mPlayerColorLabel.Size; }
    //    }

    //    public CurrentPlayerDisplay()
    //    {
    //        mPlayerColorLabel = new TextElement(ConqueraFonts.SpriteFont1, Color.Black);
    //    }

    //    public void SetPlayer(GamePlayer player)
    //    {            
    //        mPlayerColorLabel.Color = new Color(player.Color);
    //        mPlayerColorLabel.Text = player.Name;
    //    }

    //    protected override void OnDrawForeground()
    //    {
    //        mPlayerColorLabel.Draw(ScreenLocation);
    //    }
    //}
}