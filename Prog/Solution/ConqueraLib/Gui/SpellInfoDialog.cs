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

using Ale.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Gui;

namespace Conquera.Gui
{
    public class SpellInfoDialog : Dialog
    {
        private GraphicElementContainer mDisplayNameContainer;
        private GraphicElementContainer mIconContainer;
        private GraphicElementContainer mPictureContainer;                
        private GraphicElementContainer mDescriptionContainer;
        
        private GraphicElement mBackground;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public SpellInfoDialog()
        {
            mBackground = ConqueraPalette.SpellInfoDialogBackground;

            TextElement displayNameLabel = new TextElement(ConqueraFonts.SpriteFont1, Color.White);
            mDisplayNameContainer = new GraphicElementContainer(displayNameLabel, Point.Zero);            

            mIconContainer = new GraphicElementContainer(null, new Point(0, 40));
            mPictureContainer = new GraphicElementContainer(null, new Point(0, 110));

            TextElement descriptionLabel = new TextElement(ConqueraFonts.SpriteFont1, Color.Yellow);
            descriptionLabel.Warp = true;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Width = 200;
            descriptionLabel.Height = 200;
            mDescriptionContainer = new GraphicElementContainer(descriptionLabel, new Point(0, 300));
        }

        public void SetSpell(Spell spell)
        {
            ((TextElement)mDisplayNameContainer.GraphicElement).Text = spell.DisplayName;
            mIconContainer.GraphicElement = spell.Icon;
            mPictureContainer.GraphicElement = spell.Picture;
            ((TextElement)mDescriptionContainer.GraphicElement).Text = spell.Description;
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        protected override void OnDrawForeground()
        {
            mDisplayNameContainer.Draw(this);
            mIconContainer.Draw(this);
            mPictureContainer.Draw(this);
            mDescriptionContainer.Draw(this);
        }
    }
}
