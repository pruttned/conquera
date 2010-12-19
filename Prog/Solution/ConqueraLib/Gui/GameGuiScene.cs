using System;
using Ale.Scene;
using Microsoft.Xna.Framework;
using Ale.Gui;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public class GameGuiScene : GuiScene
    {
        private HeroInfoPanel mHeroPanel = new HeroInfoPanel();
        private TileInfoPanel mTilePanel = new TileInfoPanel();
        private MegaDebugLabel mDebugLabel = new MegaDebugLabel();
        private PlayerGoldView mPlayerGoldView = new PlayerGoldView();

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

        public GameGuiScene()
        {
            UpdateLocations();

            //Debug label.
            mDebugLabel.Visible = false;
            RootControls.Add(mDebugLabel);

            //Side panels.
            RootControls.Add(mHeroPanel);
            RootControls.Add(mTilePanel);

            //Header staff.
            RootControls.Add(mPlayerGoldView);

            //Other.
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

            //Player gold view.
            mPlayerGoldView.Location = Point.Zero;
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
}