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
using Ale.Tools;
using SimpleOrmFramework;
using Ale.Content;

namespace Ale.Graphics
{
    public class SphereParticleEmitterDesc : ParticleEmitterDesc
    {
        private float mRadius;

        public SphereParticleEmitterDesc(GraphicsDevice graphicsDevice, SphereParticleEmitterSettings settings, ContentGroup content)
            : base(graphicsDevice, settings, content)
        {
            mRadius = settings.Radius;
        }

        protected override void GenerateParticlePosition(ref Vector3 emitterWorldPosition, out Vector3 particlePosition)
        {
            float angle1 = AleMathUtils.GetRandomFloat(MathHelper.TwoPi);
            float angle2 = AleMathUtils.GetRandomFloat(MathHelper.TwoPi * 2);
            float radius = (float)AleMathUtils.Random.NextDouble() * mRadius;

            Vector2 xRotPos = new Vector2((float)System.Math.Cos(angle1) * radius, 
                (float)System.Math.Sin(angle1) * radius);
            particlePosition = new Vector3((float)System.Math.Sin(angle2) * xRotPos.X + emitterWorldPosition.X,
               -(float)System.Math.Cos(angle2) * xRotPos.X + emitterWorldPosition.Y,
               -xRotPos.Y + emitterWorldPosition.Z);
        }
    }


    [DataObject(MaxCachedCnt = 5)]
    public class SphereParticleEmitterSettings : ParticleEmitterSettings
    {
        [DataProperty]
        public float Radius { get; set; }

        public override ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            return new SphereParticleEmitterDesc(graphicsDevice, this, content);
        }
    }
}
