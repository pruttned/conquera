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
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class GraphicElementCreatorAttribute : Attribute
    {
        /// <summary>
        /// Id of graphic element type (image, animation...) in .xnb file.
        /// </summary>
        private int mGraphicElementTypeId;

        /// <summary>
        /// Gets the id of graphic element type (image, animation...) in .xnb file.
        /// </summary>
        public int GraphicElementTypeId
        {
            get { return mGraphicElementTypeId; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphicElementTypeId">Id of graphic element type (image, animation...) in .xnb file.</param>
        public GraphicElementCreatorAttribute(int graphicElementTypeId)
        {
            mGraphicElementTypeId = graphicElementTypeId;
        }
    }
}
