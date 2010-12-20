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

namespace Ale
{
    /// <summary>
    /// Frame listener is called on the start of each frame
    /// </summary>
    public interface IFrameListener
    {
        /// <summary>
        /// Called before updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void BeforeUpdate(AleGameTime gameTime);

        /// <summary>
        /// Called after updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void AfterUpdate(AleGameTime gameTime);

        /// <summary>
        /// Called before rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void BeforeRender(AleGameTime gameTime);

        /// <summary>
        /// Called after rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void AfterRender(AleGameTime gameTime);
    }
}
