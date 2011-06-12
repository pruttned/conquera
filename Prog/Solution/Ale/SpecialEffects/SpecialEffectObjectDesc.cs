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
using Ale.Tools;
using Microsoft.Xna.Framework;
using Ale.Graphics;

namespace Ale.SpecialEffects
{
    public class SpecialEffectObjectDesc
    {
        public NameId Name { get; internal set; }
        public Vector3 Position { get; internal set; }
        public Quaternion Orientation { get; internal set; }
        public float Scale { get; internal set; }

        public SpecialEffectObjectAnim Anim { get; internal set; }
    }

    public class MeshSpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
        public Mesh Mesh { get; internal set; }
        public ReadOnlyDictionary<string, Material> Materials { get; internal set; }
    }
    public class ParticleSystemSpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
        public ParticleSystemDesc Psys { get; internal set; }
    }
    public class DummySpecialEffectObjectDesc : SpecialEffectObjectDesc
    {
    }
}
