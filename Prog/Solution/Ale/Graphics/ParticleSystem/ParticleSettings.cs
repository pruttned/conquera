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
