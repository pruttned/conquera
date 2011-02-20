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

using Ale.Gui;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public static class ConqueraPalette
    {
        private static readonly Texture2D mPaletteTexture = GuiManager.Instance.Content.Load<Texture2D>("Palette");
        private static readonly Texture2D mCellNotificationTexture = GuiManager.Instance.Content.Load<Texture2D>("CellNotificationText");

        public static readonly Image MessageBoxBackground = new Image(new Rectangle(257, 1, 510, 475), mPaletteTexture);
        public static readonly Image MainMenuBackground = new Image(new Rectangle(257, 1, 510, 475), mPaletteTexture);
        public static readonly Image SpellInfoDialogBackground = new Image(new Rectangle(257, 1, 510, 475), mPaletteTexture);
        public static readonly Image TextButtonDefault = new Image(new Rectangle(0, 0, 42, 42), mPaletteTexture);
        public static readonly Image TextButtonOver = new Image(new Rectangle(43, 0, 42, 42), mPaletteTexture);
        public static readonly Image TextButtonDisabled = new Image(new Rectangle(86, 0, 42, 42), mPaletteTexture);
        public static readonly Image UnitInfoPanelBackground = new Image(new Rectangle(0, 259, 243, 199), mPaletteTexture);
        public static readonly Point CastleBuyUnit1ButtonLocation = new Point(80, 100);
        public static readonly Image TileInfoPanelBackground = new Image(new Rectangle(0, 259, 243, 199), mPaletteTexture);
        public static readonly Point TileInfoViewLocation = new Point(10, 10);
        public static readonly System.Drawing.SizeF TileInfoViewSize = new System.Drawing.SizeF(225, 180);
        public static readonly Point TileInfoViewIconLocation = new Point(0, 0);
        public static readonly Rectangle TileInfoViewNameRectangle = new Rectangle(64, 0, 161, 64);
        public static readonly Rectangle TileInfoViewDescriptionRectangle = new Rectangle(0, 64, 225, 116);

        public static readonly Image CursorDefault = new Image(new Rectangle(0, 43, 24, 24), mPaletteTexture);
        public static readonly Image CursorAttack = new Image(new Rectangle(50, 43, 24, 24), mPaletteTexture);
        public static readonly Image CursorMove = new Image(new Rectangle(25, 43, 24, 24), mPaletteTexture);
        public static readonly Image CursorMoveDisabled = new Image(new Rectangle(75, 43, 24, 24), mPaletteTexture);
        public static readonly Point CursorDefaultHotSpot = new Point(3, 4);
        public static readonly Point CursorAttackHotSpot = new Point(3, 4);
        public static readonly Point CursorMoveHotSpot = new Point(3, 4);
        public static readonly Point CursorMoveDisabledHotSpot = new Point(3, 4);

        public static readonly Image SpellIconSlayer = new Image(new Rectangle(0, 64, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconSpikes = new Image(new Rectangle(128, 0, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconFireStorm = new Image(new Rectangle(64, 0, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconVampiricTouch = new Image(new Rectangle(192, 0, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconPackReinforcement = new Image(new Rectangle(64, 64, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconMindControl = new Image(new Rectangle(128, 64, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconPlague = new Image(new Rectangle(192, 64, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconBloodMadness = new Image(new Rectangle(0, 128, 64, 64), mCellNotificationTexture);
        public static readonly Image SpellIconLastSacrifice = new Image(new Rectangle(128, 128, 64, 64), mCellNotificationTexture);

        public static readonly Image ListBoxBackground = new Image(new Rectangle(257, 1, 150, 200), mPaletteTexture);
        public static readonly Rectangle ListBoxItemsRectangle = new Rectangle(10, 50, 130, 130);
        public static readonly Image ListBoxNextPageButtonDefault = new Image(new Rectangle(0, 0, 42, 42), mPaletteTexture);
        public static readonly Image ListBoxNextPageButtonOver = new Image(new Rectangle(43, 0, 42, 42), mPaletteTexture);
        public static readonly Image ListBoxNextPageButtonDisabled = new Image(new Rectangle(86, 0, 42, 42), mPaletteTexture);
        public static readonly Image ListBoxPreviousPageButtonDefault = new Image(new Rectangle(0, 0, 42, 42), mPaletteTexture);
        public static readonly Image ListBoxPreviousPageButtonOver = new Image(new Rectangle(43, 0, 42, 42), mPaletteTexture);
        public static readonly Image ListBoxPreviousPageButtonDisabled = new Image(new Rectangle(86, 0, 42, 42), mPaletteTexture);
        public static readonly Point ListBoxNextPageButtonLocation = new Point(80, 5);
        public static readonly Point ListBoxPreviousPageButtonLocation = new Point(5, 5);
        public static readonly Image ListBoxOverItem = new Image(new Rectangle(1, 525, 130, 24), mPaletteTexture);
        public static readonly Image ListBoxSelectedItem = new Image(new Rectangle(1, 550, 130, 24), mPaletteTexture);

        public static Image GetTileIcon(string name)
        {
            switch (name)
            {
                case "TileIconCastle":
                    return new Image(new Rectangle(1, 460, 64, 64), mPaletteTexture);
                case "TileIconSpellTower":
                    return new Image(new Rectangle(67, 460, 64, 64), mPaletteTexture);
                default:
                    return new Image(new Rectangle(133, 460, 64, 64), mPaletteTexture);
            }
        }
    }
}
