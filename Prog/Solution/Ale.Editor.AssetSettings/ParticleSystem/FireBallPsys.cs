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
