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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Vector2), typeof(FieldCustomBasicTypeProvider<Vector2>))]
    public class ParticleSettings : BaseDataObject
    {
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleColorRFunction { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleColorGFunction { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleColorBFunction { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleColorAFunction { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleSpeed { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleRotation { get; set; }
        [DataListProperty(NotNull = true)]
        public TimeFunction ParticleSize { get; set; }

        public ParticleSettings()
        {
            ParticleColorRFunction = new TimeFunction(0.5f);
            ParticleColorGFunction = new TimeFunction(0.5f);
            ParticleColorBFunction = new TimeFunction(0.5f);
            ParticleColorAFunction = new TimeFunction(1.0f);
            ParticleSpeed = new TimeFunction(3.0f);
            ParticleRotation = new TimeFunction(0);
            ParticleSize = new TimeFunction(1);
        }
    }
}
