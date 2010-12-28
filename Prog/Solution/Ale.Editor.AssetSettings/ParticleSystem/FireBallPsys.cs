using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Ale.Content;
using Microsoft.Xna.Framework;

namespace Ale.Editor.AssetSettings
{
    public class FireBallPsys : AssetSettingsDefinitionBase<ParticleSystemSettings>
    { 
        public override ParticleSystemSettings GetSettings(ContentGroup content)
        {
            ParticleSystemSettings particleSystemSettings = new ParticleSystemSettings();
            particleSystemSettings.Name = "FireBallPsys";

            PointParticleEmitterSettings particleEmitterSettings = new PointParticleEmitterSettings();
            particleSystemSettings.Emitters.Add(particleEmitterSettings);
             
            particleEmitterSettings.MaxParticleCnt = 200;
            particleEmitterSettings.ParticleLifeDuration = 1f;
            particleEmitterSettings.ParticleLifeDurationVariation = 0.5f;
            particleEmitterSettings.ParticleDirectionVariation = MathHelper.ToRadians(360.0f);
            particleEmitterSettings.ParticleSettings.ParticleColorRFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.2f, 0.7f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
            particleEmitterSettings.ParticleSettings.ParticleColorGFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.9f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
            particleEmitterSettings.ParticleSettings.ParticleColorBFunction = new TimeFunction(new Vector2[] { new Vector2(0, 0.3f), new Vector2(0.2f, 0.1f), new Vector2(0.6f, 0.0f), new Vector2(1.0f, 0.0f) });
            particleEmitterSettings.ParticleSettings.ParticleColorAFunction = new TimeFunction(new Vector2[] { new Vector2(0, 1), new Vector2(0.5f, 1), new Vector2(1.0f, 0) });
            particleEmitterSettings.ParticleSettings.ParticleSpeed = new TimeFunction(0.5f, 0.1f);
            particleEmitterSettings.ParticleSettings.ParticleSize = new TimeFunction(0.3f, 0.9f);
            particleEmitterSettings.ParticleSettings.ParticleRotation = new TimeFunction(0, 1);
            particleEmitterSettings.EmissionRateFunction = new TimeFunction(40);
            particleEmitterSettings.CycleTime = 0.5f;
              


            particleEmitterSettings.Material = content.ParentContentManager.OrmManager.FindObject(typeof(MaterialSettings), "Name='ParticleMaterial'");
            particleEmitterSettings.Name = "Emitter1";

         


            return particleSystemSettings;

            //return content.ParentContentManager.OrmManager.LoadObject<ParticleSystemSettings>(string.Format("Name='{0}'", "FireBallPs"));
        }
    }
}
