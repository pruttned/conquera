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
using Ale.Tools;
using SimpleOrmFramework;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    public class DirectionRandomizerParticleAffectorSettings : ParticleAffectorSettings
    {
        [DataProperty]
        public float DirectionVariation { get; set; }
        [DataProperty]
        public float BounceDistanceFromPsys { get; set; }
        [DataProperty]
        public float MaxDistanceFromPsys { get; set; }
        [DataProperty]
        public float DirectionCahngeProbability { get; set; }

        public DirectionRandomizerParticleAffectorSettings()
        {
            DirectionVariation = 1;
            BounceDistanceFromPsys = 10;
            MaxDistanceFromPsys = 12;
            DirectionCahngeProbability = 0.8f;
        }

        public override IParticleAffector CreateParticleAffector()
        {
            return new DirectionRandomizerParticleAffector(this);
        }
    }
}
