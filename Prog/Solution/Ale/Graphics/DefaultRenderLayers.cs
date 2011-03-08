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
using System.Text;

namespace Ale.Graphics
{
    /// <summary>
    /// Default render layers
    /// </summary>
    public static class DefaultRenderLayers
    {
        /// <summary>
        /// Water
        /// </summary>
        public const int Water = 1000;

        /// <summary>
        /// Ground
        /// </summary>
        public const int Ground = 2000;

        /// <summary>
        /// Ground wall
        /// </summary>
        public const int GroundWall = 1500;

        /// <summary>
        /// Objects that are lying on the ground (shadow, road, ...)
        /// </summary>
        public const int GroundLyingObjects = 3000;

        public const int MovementArea = 3100;
        public const int Region = 3200;

        /// <summary>
        /// Cursor3dCellSel
        /// </summary>
        public const int Cursor3dCellSel = 3300;

        /// <summary>
        /// Cursor3D
        /// </summary>
        public const int Cursor3D = 3500;


        /// <summary>
        /// Objects that are standing on the ground
        /// </summary>
        public const int GroundStandingObjects = 4000;

        /// <summary>
        /// MovementArrow
        /// </summary>
        public const int MovementArrow = 7000;
    }
}
