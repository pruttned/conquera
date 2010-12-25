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
        private SpellSlot mCardSlot1 = new SpellSlot();
        private SpellSlot mCardSlot2 = new SpellSlot();
        private SpellSlot mCardSlot3 = new SpellSlot();

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public GameUnitInfoPanel()
        {
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("UnitInfoPanelBackground");

            mStatText = new TextElement((int)Size.Width, (int)Size.Height, GuiManager.Instance.GetGuiFont("SpriteFontSmall"), true, Color.Black);

            mCardSlot1.Location = new Microsoft.Xna.Framework.Point(15, 110);
            mCardSlot2.Location = new Microsoft.Xna.Framework.Point(80, 110);
            mCardSlot3.Location = new Microsoft.Xna.Framework.Point(150, 110);
            ChildControls.Add(mCardSlot1);
            ChildControls.Add(mCardSlot2);
            ChildControls.Add(mCardSlot3);
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

            mStatText.AppendLine(string.Format("Atck :   {0} | Dfs :   {1}", gameUnit.GameUnitDesc.Attack, gameUnit.GameUnitDesc.Defense));

            //Cards.
            //todo: spell
            //mCardSlot1.Spell = gameUnit.Cards.Count >= 1 ? gameUnit.Cards[0] : null;
            //mCardSlot2.Spell = gameUnit.Cards.Count >= 2 ? gameUnit.Cards[1] : null;
            //mCardSlot3.Spell = gameUnit.Cards.Count >= 3 ? gameUnit.Cards[2] : null;
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
