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
using Ale.Content;
using Ale.Graphics;
using Ale.Tools;

namespace Conquera
{
    [NonContentPipelineAsset(typeof(HexTerrainTileAtlasLoader))]
    public class HexTerrainTileAtlas
    {
        public NameId Name { get; private set; }

        public Material Material { get; private set; }

        /// <summary>
        /// Size in tiles
        /// </summary>
        public int Size { get; private set; }

        public float TextureCellSpacing { get; private set; }

        public HexTerrainTileAtlas(HexTerrainTileAtlasSettings settings, ContentGroup contentGroup)
        {
            Name = settings.Name;
            Material = contentGroup.Load<Material>(settings.Material);
            Size = settings.Size;
            TextureCellSpacing = settings.TextureCellSpacing;
        }
    }
}
