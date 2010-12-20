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

using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a control containing an empty GraphicElementRepositoryGroup, which always covers the whole client area of the control.
    /// </summary>
    public class SimpleControl : Control
    {
        private GraphicElementRepositoryGroup mMainGraphicElementRepository;

        protected GraphicElementRepositoryGroup MainGraphicElementRepository
        {
            get { return mMainGraphicElementRepository; }
        }

        public SimpleControl()
        {
            mMainGraphicElementRepository = new GraphicElementRepositoryGroup(this);
            MainGraphicElementRepository.Autosize = false;
        }

        internal override void SetClientArea(Rectangle value)
        {
            base.SetClientArea(value);

            MainGraphicElementRepository.Location = ClientArea.Location;
            MainGraphicElementRepository.Size = new System.Drawing.Size(ClientArea.Width, ClientArea.Height);
        }
    }
}
