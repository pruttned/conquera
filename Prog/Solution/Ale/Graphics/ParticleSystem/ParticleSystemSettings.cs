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
using SimpleOrmFramework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class ParticleSystemSettings : BaseDataObject
    {
        [DataProperty(CaseSensitive = false, Unique = true, NotNull = true)]
        public string Name { get; set; }

        [DataListProperty(NotNull = true)]
        public List<ParticleEmitterSettings> Emitters { get; private set; }

        public ParticleSystemSettings()
        {
            Emitters = new List<ParticleEmitterSettings>();
        }
    }
}
