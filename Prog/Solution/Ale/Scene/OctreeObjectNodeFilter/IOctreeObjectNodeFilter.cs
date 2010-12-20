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
using System.Collections.Generic;
using System.Text;

namespace Ale.Scene
{
    /// <summary>
    /// Gets nodes and objects that passes a given check
    /// </summary>
    public interface IOctreeObjectNodeFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeNode"></param>
        /// <returns></returns>
        NodeFilterResult CheckNode(OctreeSceneNode octreeNode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="octreeObject"></param>
        /// <returns></returns>
        bool CheckObject(IOctreeObject octreeObject);
    }

    public enum NodeFilterResult
    {
        /// <summary>
        /// Dont include node - no child is checked
        /// </summary>
        DontInclude,

        /// <summary>
        /// Include whole node - each chil is included without check
        /// </summary>
        Include,

        /// <summary>
        /// Include node but check childs
        /// </summary>
        IncludePartially
    }
}
