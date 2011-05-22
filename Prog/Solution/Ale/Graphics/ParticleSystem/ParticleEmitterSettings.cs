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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Tools;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Content;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Vector2), typeof(FieldCustomBasicTypeProvider<Vector2>))]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    [CustomBasicTypeProvider(typeof(Vector4), typeof(FieldCustomBasicTypeProvider<Vector4>))]
    public abstract class ParticleEmitterSettings : BaseDataObject
    {
        [DataProperty(CaseSensitive = false, NotNull = true)]
        public string Name { get; set; }

        [DataProperty]
        public float CycleTime { get; set; }
        [DataProperty]
        public int MaxParticleCnt { get; set; }
        [DataListProperty(NotNull=true)]
        public TimeFunction EmissionRateFunction { get; set; }
        [DataProperty]
        public ParticleSettings ParticleSettings { get; set; }
        [DataProperty]
        public long Material { get; set; }
        [DataProperty]
        public Vector2 ParticleDirectionAngles { get; set; }
        [DataProperty]
        public float ParticleDirectionVariation { get; set; }
        [DataProperty]
        public float ParticleLifeDuration { get; set; }
        [DataProperty]
        public float ParticleLifeDurationVariation { get; set; }
        [DataProperty]
        public float FastForwardTimeOnLoad { get; set; }
        [DataProperty]
        public float MaxFastForwardTime { get; set; }
        [DataProperty]
        public float SampleRateOnFastForward { get; set; }
        [DataProperty]
        public bool Sort{ get; set; }
        /// <summary>
        /// Emitter position relative to particle system world position
        /// </summary>
        [DataProperty]
        public Vector3 RelativePosition { get; set; }
        [DataListProperty(NotNull = true)]
        public List<ParticleAffectorSettings> ParticleAffectors { get; private set; }
        [DataProperty]
        public Vector4 ColorVariation { get; set; }
        [DataProperty]
        public float RotationVariation { get; set; }
        [DataProperty]
        public float SizeVariation { get; set; }

        public ParticleEmitterSettings()
        {
            CycleTime = 5.0f;
            MaxParticleCnt = 100;
            EmissionRateFunction = new TimeFunction(20, 20);
            ParticleSettings = new ParticleSettings();
            Material = -1; //todo: sem dat dafaultny material
            ParticleDirectionAngles = Vector2.Zero;
            ParticleDirectionVariation = MathHelper.ToRadians(30.0f);
            ParticleLifeDuration = 5;
            ParticleLifeDurationVariation = 2;
            FastForwardTimeOnLoad = 1.0f;
            MaxFastForwardTime = 1.0f;
            SampleRateOnFastForward = 0.1f;
            Sort = false;
            RelativePosition = Vector3.Zero;
            ColorVariation = Vector4.Zero;
            RotationVariation = 0.0f;
            SizeVariation = 0.0f;

            ParticleAffectors = new List<ParticleAffectorSettings>();
        }

        public abstract ParticleEmitterDesc CreateParticleEmitterDesc(GraphicsDevice graphicsDevice, ContentGroup content);
    }
}
