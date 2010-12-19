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

using Microsoft.Xna.Framework.Content;

namespace Ale.Gui
{
    /// <summary>
    /// Interface for all factories, which Palette uses to create graphic elements.
    /// </summary>
    public interface IGraphicElementCreator
    {
        /// <summary>
        /// Creates a graphic element.
        /// </summary>
        /// <returns></returns>
        GraphicElement CreateGraphicElement();

        /// <summary>
        /// Called before any 'CreateGraphicElement' call.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="palette"></param>
        void Initialize(ContentReader input, Palette palette);
    }
}
