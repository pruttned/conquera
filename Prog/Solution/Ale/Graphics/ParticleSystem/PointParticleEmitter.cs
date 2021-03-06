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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SimpleOrmFramework;
using Ale.Content;

namespace Ale.Graphics
{
    public class PointParticleSystemDesc : ParticleEmitterDesc
    {
        public PointParticleSystemDesc(GraphicsDevice graphicsDevice, PointParticleEmitterSettings settings, ContentGroup content)
            : base(graphicsDevice, settings, content)
        {
        }

        protected override void GenerateParticlePosition(ref Vector3 emitterWorldPosition, out Vector3 particlePosition)
        {
            particlePosition =  emitterWorldPosition;
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    public class PointParticleEmitterSettings : ParticleEmitterSettings
    {
        public override ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            return new PointParticleSystemDesc(graphicsDevice, this, content);
        }
    }
}
