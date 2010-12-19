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
    public abstract class TileInfoView : Control
    {
        private System.Drawing.SizeF mSize;
        private GraphicElementContainer mIconContainer;
        private GraphicElementContainer mNameContainer;
        private GraphicElementContainer mDescriptionContainer;
        private Rectangle mNameRectangle;

        public override System.Drawing.SizeF Size
        {
            get { return mSize; }
        }

        public TileInfoView()
        {
            Rectangle iconRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewIcon");
            mIconContainer = new GraphicElementContainer(null, new Point(iconRectangle.Left, iconRectangle.Top));

            mNameRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewName");
            mNameContainer = new GraphicElementContainer(new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/TileTypeName"), Color.Black), Point.Zero);

            Rectangle descriptionRectangle = GuiManager.Instance.Palette.CreateRectangle("TileInfoViewDescription");
            TextElement descriptionLabel = new TextElement(GuiManager.Instance.GetGuiFont("TileInfo/TileDescription"), Color.Black);
            descriptionLabel.Warp = true;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Width = descriptionRectangle.Width;
            descriptionLabel.Height = descriptionRectangle.Height;
            mDescriptionContainer = new GraphicElementContainer(descriptionLabel, new Point(descriptionRectangle.Left, descriptionRectangle.Top));
        }

        public void SetSize(System.Drawing.SizeF size)
        {
            mSize = size;
        }

        public abstract void Update(HexCell cell);

        public void Setup(string name, GraphicElement icon, string description)
        {
            mIconContainer.GraphicElement = icon;
            ((TextElement)mDescriptionContainer.GraphicElement).Text = description;

            TextElement nameLabel = (TextElement)mNameContainer.GraphicElement;
            nameLabel.Text = name;
            mNameContainer.Location = new Point(mNameRectangle.Left + mNameRectangle.Width / 2 - nameLabel.Width / 2,
                                                mNameRectangle.Top + mNameRectangle.Height / 2 - nameLabel.Height / 2);
        }

        protected override void OnDrawForeground()
        {
            mIconContainer.Draw(this);
            mNameContainer.Draw(this);
            mDescriptionContainer.Draw(this);
        }
    }
}
