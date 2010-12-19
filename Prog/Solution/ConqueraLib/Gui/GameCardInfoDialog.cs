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

using Ale.Scene;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Gui;

namespace Conquera.Gui
{
    public class GameCardInfoDialog : Dialog
    {
        private GraphicElementContainer mNameContainer;
        private GraphicElementContainer mPictureContainer;
        private GraphicElementContainer mDescriptionContainer;
        private Rectangle mNameRectangle;
        private GraphicElement mBackground;

        public override System.Drawing.SizeF Size
        {
            get { return mBackground.Size; }
        }

        public GameCardInfoDialog()
        {
            mBackground = GuiManager.Instance.Palette.CreateGraphicElement("GameCardInfoDialogBackground");

            TextElement nameLabel = new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/GameCardDialogCardName"), Color.White);
            mNameContainer = new GraphicElementContainer(nameLabel, Point.Zero);
            mNameRectangle = GuiManager.Instance.Palette.CreateRectangle("GameCardInfoDialogName");

            Rectangle pictureRectangle = GuiManager.Instance.Palette.CreateRectangle("GameCardInfoDialogPicture");
            mPictureContainer = new GraphicElementContainer(null, pictureRectangle.Location);

            Rectangle descriptionRectangle = GuiManager.Instance.Palette.CreateRectangle("GameCardInfoDialogDescription");
            TextElement descriptionLabel = new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/GameCardDialogDescription"), Color.Yellow);
            descriptionLabel.Warp = true;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Width = descriptionRectangle.Width;
            descriptionLabel.Height = descriptionRectangle.Height;
            mDescriptionContainer = new GraphicElementContainer(descriptionLabel, descriptionRectangle.Location);
        }

        public void SetGameCard(GameCard card)
        {
            TextElement nameLabel = (TextElement)mNameContainer.GraphicElement;
            nameLabel.Text = card.Name;
            mNameContainer.Location = new Point(mNameRectangle.Left + mNameRectangle.Width / 2 - nameLabel.Width / 2,
                                                mNameRectangle.Top + mNameRectangle.Height / 2 - nameLabel.Height / 2);

            mPictureContainer.GraphicElement = card.Picture;
            ((TextElement)mDescriptionContainer.GraphicElement).Text = card.Description;
        }

        protected override void OnDrawBackground()
        {
            mBackground.Draw(ScreenLocation);
        }

        protected override void OnDrawForeground()
        {
            mNameContainer.Draw(this);
            mPictureContainer.Draw(this);
            mDescriptionContainer.Draw(this);
        }
    }
}
