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

using System;

namespace Ale.Gui
{
    public class GraphicButton : Control
    {
        private GraphicElement mDefaultGraphicElement;
        private GraphicElement mMouseOverGraphicElement;
        private GraphicElement mActiveGraphicElement;

        public override System.Drawing.SizeF Size
        {
            get { return mActiveGraphicElement.Size; }
        }

        public GraphicButton(GraphicElement defaultGraphicElement, GraphicElement mouseOverGraphicElement)
        {
            mDefaultGraphicElement = defaultGraphicElement;
            mMouseOverGraphicElement = mouseOverGraphicElement;
            mActiveGraphicElement = mDefaultGraphicElement;

            MouseEnter += new EventHandler<ControlEventArgs>(GraphicButton_MouseEnter);
            MouseLeave += new EventHandler<ControlEventArgs>(GraphicButton_MouseLeave);
        }

        protected override void OnDrawBackground()
        {
            mActiveGraphicElement.Draw(ScreenLocation);
        }

        private void GraphicButton_MouseEnter(object sender, ControlEventArgs e)
        {
            mActiveGraphicElement = mMouseOverGraphicElement;
        }

        private void GraphicButton_MouseLeave(object sender, ControlEventArgs e)
        {
            mActiveGraphicElement = mDefaultGraphicElement;
        }
    }
}
