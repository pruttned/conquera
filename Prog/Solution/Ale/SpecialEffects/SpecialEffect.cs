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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;

namespace Ale.SpecialEffects
{
    public class SpecialEffect
    {
        public static Dictionary<string, SpecialEffectObjectReader> SpecialEffectObjectReaders { get; private set; }
        private SpecialEffectObjectDesc[] mSpecialEffectObjects;

        static SpecialEffect()
        {
            SpecialEffectObjectReaders = new Dictionary<string, SpecialEffectObjectReader>(StringComparer.InvariantCultureIgnoreCase);
            SpecialEffectObjectReaders["Mesh"] = new MeshSpecialEffectObjectReader();
            SpecialEffectObjectReaders["Psys"] = new ParticleSystemSpecialEffectObjectReader();
            SpecialEffectObjectReaders["Dummy"] = new DummySpecialEffectObjectReader();
        }

        public SpecialEffect(ContentReader input)
        {
            int objCnt = input.ReadInt32();
            mSpecialEffectObjects = new SpecialEffectObjectDesc[objCnt];
            for (int i = 0; i < objCnt; ++i)
            {
                string objType = input.ReadString();
                var objReader = GetObjectReader(objType);
                var obj = objReader.Read(input);

                mSpecialEffectObjects[i] = obj;
            }
        }

        protected static SpecialEffectObjectReader GetObjectReader(string name)
        {
            SpecialEffectObjectReader reader;
            if (!SpecialEffectObjectReaders.TryGetValue(name, out reader))
            {
                throw new KeyNotFoundException(string.Format("Special effect object reader '{0}' doesn't exists", name));
            }
            return reader;
        }
    }
}
