using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Ale.Content;
using Microsoft.Xna.Framework;

namespace Ale.Editor.AssetSettings
{
    public class SlayerPsys : AssetSettingsDefinitionBase<ParticleSystemSettings>
    { 
        public override ParticleSystemSettings GetSettings(ContentGroup content)
        {//ds
            ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
            particleSystemSettings.Name = "SlayerPsys";

            ZCylinderParticleEmitterSettings particleEmitterSettings = new ZCylinderParticleEmitterSettings();
            particleSystemSettings.Emitters.Add(particleEmitterSettings);

            particleEmitterSettings.Radius = 0.5f;
            particleEmitterSettings.Height = 0.0f;
  //
            particleEmitterSettings.FastForwardTimeOnLoad = 0;
            particleEmitterSettings.MaxParticleCnt = 20 ;
            particleEmitterSettings.ParticleLifeDuration = 2f;
            particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
            particleEmitterSettings.RelativePosition = new Vector3(0, 0, 0);
//            particleEmitterSettings.ParticleDirectionAngles = new Vector2(MathHelper.ToRadians(180), MathHelper.ToRadians(0));
            particleEmitterSettings.ParticleDirectionVariation = MathHelper.ToRadians(5.0f);
            particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction( new Vector2(0, 0.8f), new Vector2(1, 0.8f) );
            particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction( new Vector2(0, 0.8f),new Vector2(1, 0.1f) );
            particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction( new Vector2(0, 0.8f),new Vector2(1, 1.0f)) ;
            particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2(0, 0), new Vector2(0.5f, 0.8f), new Vector2(0.6f, 0.8f), new Vector2(1.0f, 0));
            particleEmitterSettings.ColorVariation = new Vector4(0.5f, 0.0f, 0.5f, 0.0f);
            particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0);
            //particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.1f, 0.1f);
            particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(5f, 5f);
            //particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 0);
            particleEmitterSettings.EmissionRateFunction = new TimeFunction(10);
            particleEmitterSettings.CycleTime = 1f;
              
            particleEmitterSettings.Material = content.ParentContentManager.OrmManager.FindObject(typeof(MaterialSettings), "Name='BeamParticleMat'");
            particleEmitterSettings.Name = "Emitter1";





            return particleSystemSettings;

            //return content.ParentContentManager.OrmManager.LoadObject<ParticleSystemSettings>(string.Format("Name='{0}'", "FireBallPs"));
        }
    }
}
