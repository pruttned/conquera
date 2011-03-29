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
    public class TileInfoView : Control
    {
        private System.Drawing.SizeF mSize;
        private GraphicElementContainer mIconContainer;
        private GraphicElementContainer mNameContainer;
        private GraphicElementContainer mDescriptionContainer;        

        public override System.Drawing.SizeF Size
        {
            get { return mSize; }
        }

        public TileInfoView()
        {
            mIconContainer = new GraphicElementContainer(null, ConqueraPalette.TileInfoViewIconLocation);
            mNameContainer = new GraphicElementContainer(new TextElement(ConqueraFonts.SpriteFontSmall, Color.Black), Point.Zero);
            
            TextElement descriptionLabel = new TextElement(ConqueraFonts.SpriteFontSmall, Color.Black);
            descriptionLabel.Warp = true;
            descriptionLabel.AutoSize = false;
            descriptionLabel.Width = ConqueraPalette.TileInfoViewDescriptionRectangle.Width;
            descriptionLabel.Height = ConqueraPalette.TileInfoViewDescriptionRectangle.Height;
            mDescriptionContainer = new GraphicElementContainer(descriptionLabel, ConqueraPalette.TileInfoViewDescriptionRectangle.Location);
        }

        public void SetSize(System.Drawing.SizeF size)
        {
            mSize = size;
        }

        public virtual void Update(HexCell cell)
        {
            Setup(cell.HexTerrainTile.DisplayName, cell.HexTerrainTile.Icon, cell.HexTerrainTile.Description);
        }        

        protected override void OnDrawForeground()
        {
            mIconContainer.Draw(this);
            mNameContainer.Draw(this);
            mDescriptionContainer.Draw(this);
        }

        private void Setup(string name, GraphicElement icon, string description)
        {
            mIconContainer.GraphicElement = icon;
            ((TextElement)mDescriptionContainer.GraphicElement).Text = description;

            TextElement nameLabel = (TextElement)mNameContainer.GraphicElement;
            nameLabel.Text = name;
            Rectangle nameRectangle = ConqueraPalette.TileInfoViewNameRectangle;
            mNameContainer.Location = new Point(nameRectangle.Left + nameRectangle.Width / 2 - nameLabel.Width / 2,
                                                nameRectangle.Top + nameRectangle.Height / 2 - nameLabel.Height / 2);
        }
    }
}
