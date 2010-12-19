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
