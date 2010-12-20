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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Ale.Content
{
    /// <summary>
    /// Base interface for loader that loads an asset that is not based on xna content pipeline
    /// </summary>
    public interface IAssetLoader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="contentGroup"></param>
        /// <param name="Id">if -1 then the asset has no Id</param>
        /// <returns></returns>
        object LoadAsset(string name, ContentGroup contentGroup, out long Id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contentGroup"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        object LoadAsset(long id, ContentGroup contentGroup, out string name);
    }
}
