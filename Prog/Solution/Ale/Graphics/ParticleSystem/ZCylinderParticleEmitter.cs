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
    public class ZCylinderParticleEmitterDesc : ParticleEmitterDesc
    {
        private float mRadius;
        private float mHeight;

        public ZCylinderParticleEmitterDesc(GraphicsDevice graphicsDevice, ZCylinderParticleEmitterSettings settings, ContentGroup content)
            : base(graphicsDevice, settings, content)
        {
            mRadius = settings.Radius;
            mHeight = settings.Height;
        }

        protected override void GenerateParticlePosition(ref Vector3 emitterWorldPosition, out Vector3 particlePosition)
        {
            float angle1 = AleMathUtils.GetRandomFloat(MathHelper.TwoPi);
            float radius = (float)AleMathUtils.Random.NextDouble() * mRadius;

            Vector2 rotPos = new Vector2((float)System.Math.Cos(angle1) * radius,
                (float)System.Math.Sin(angle1) * radius);

            float height = (float)AleMathUtils.Random.NextDouble() * mHeight;

            particlePosition = new Vector3(emitterWorldPosition.X + rotPos.X, emitterWorldPosition.Y + rotPos.Y, emitterWorldPosition.Z + height);

            //particlePosition = new Vector3((float)System.Math.Sin(angle2) * xRotPos.X + emitterWorldPosition.X,
            //   -(float)System.Math.Cos(angle2) * xRotPos.X + emitterWorldPosition.Y,
            //   -xRotPos.Y + emitterWorldPosition.Z);
        }
    }

    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class ZCylinderParticleEmitterSettings : ParticleEmitterSettings
    {
        [DataProperty]
        public float Radius { get; set; }
        [DataProperty]
        public float Height { get; set; }

        public override ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content)
        {
            return new ZCylinderParticleEmitterDesc(graphicsDevice, this, content);
        }
    }

}
