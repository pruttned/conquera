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
using Microsoft.Xna.Framework.Graphics;

namespace Conquera.Gui
{
    public class GameUnitInfoPanel : Control
    {
        private GraphicElement mBackground;
        private TextElement mStatText;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public GameUnitInfoPanel()
        {
            mBackground = ConqueraPalette.UnitInfoPanelBackground;
            mStatText = new TextElement((int)Size.Width, (int)Size.Height, ConqueraFonts.SpriteFontSmall, true, Color.Black);
        }

        public void Update(GameUnit gameUnit)
        {
            //Statistics.
            mStatText.Color = new Color(gameUnit.OwningPlayer.Color);
            mStatText.Text = string.Format("{0}/{1}", gameUnit.Hp, gameUnit.GameUnitDesc.MaxHp);
            
            if (gameUnit.OwningPlayer == gameUnit.GameScene.CurrentPlayer)
            {
                mStatText.AppendLine(string.Format("Ready: {0}", !gameUnit.HasMovedThisTurn));
            }

            mStatText.AppendLine(string.Format("Atck :   {0}", gameUnit.GameUnitDesc.Attack));
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        protected override void OnDrawForeground()
        {
            mStatText.Draw(new Microsoft.Xna.Framework.Point(ScreenLocation.X + 15, ScreenLocation.Y + 15));
        }
    }
}
