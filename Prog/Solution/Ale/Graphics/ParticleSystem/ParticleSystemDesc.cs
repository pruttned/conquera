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
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Content;

namespace Ale.Graphics
{
    [NonContentPipelineAsset(typeof(ParticleSystemLoader))]
    public class ParticleSystemDesc
    {
        private ReadOnlyCollection<ParticleEmitterDesc> mEmitters;

        public ReadOnlyCollection<ParticleEmitterDesc> Emitters
        {
            get { return mEmitters; }
        }

        public ParticleSystemDesc(GraphicsDevice graphicsDevice, ParticleSystemSettings settings, ContentGroup content)
        {
            ParticleEmitterDesc[] emitters = new ParticleEmitterDesc[settings.Emitters.Count];

            for(int i = 0; i < emitters.Length; ++i) 
            {
                emitters[i] = settings.Emitters[i].CreateParticleEmitterDesc(graphicsDevice, content);
            }

            mEmitters = new ReadOnlyCollection<ParticleEmitterDesc>(emitters);
        }
    }
}
