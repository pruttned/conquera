//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
    public class HeroInfoPanel : Control
    {
        private GraphicElement mBackground;
        private TextElement mStatText;
        private GameCardSlot mCardSlot1 = new GameCardSlot();
        private GameCardSlot mCardSlot2 = new GameCardSlot();
        private GameCardSlot mCardSlot3 = new GameCardSlot();

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public HeroInfoPanel()
        {
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("HeroInfoPanelBackground");

            mStatText = new TextElement((int)Size.Width, (int)Size.Height, GuiManager.Instance.GetGuiFont("SpriteFontSmall"), true, Color.Black);

            mCardSlot1.Location = new Microsoft.Xna.Framework.Point(15, 110);
            mCardSlot2.Location = new Microsoft.Xna.Framework.Point(80, 110);
            mCardSlot3.Location = new Microsoft.Xna.Framework.Point(150, 110);
            ChildControls.Add(mCardSlot1);
            ChildControls.Add(mCardSlot2);
            ChildControls.Add(mCardSlot3);
        }

        public void Update(GameUnit hero)
        {
            //Statistics.
            mStatText.Color = new Color(hero.OwningPlayer.Color);
            mStatText.Text = string.Format("{0}/{1}", hero.Hp, hero.GameUnitDesc.MaxHp);
            
            if (hero.OwningPlayer == hero.GameScene.CurrentPlayer)
            {
                mStatText.AppendLine(string.Format("Ready: {0}", !hero.HasMovedThisTurn));
            }

            mStatText.AppendLine(string.Format("Atck Red:   {0} | Dfs Red:   {1}", hero.AttackPurple, hero.DefensePurple));
            mStatText.AppendLine(string.Format("Atck Green: {0} | Dfs Green: {1}", hero.AttackGreen, hero.DefenseGreen));
            mStatText.AppendLine(string.Format("Atck Black: {0} | Dfs Black: {1}", hero.AttackBlack, hero.DefenseBlack));

            //Cards.
            mCardSlot1.Card = hero.Cards.Count >= 1 ? hero.Cards[0] : null;
            mCardSlot2.Card = hero.Cards.Count >= 2 ? hero.Cards[1] : null;
            mCardSlot3.Card = hero.Cards.Count >= 3 ? hero.Cards[2] : null;
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
