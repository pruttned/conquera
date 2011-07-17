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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;
using Ale.Scene;
using Ale.Sound;

namespace Ale.SpecialEffects
{
    public interface ISpecialEffectTimeTriggerAction
    {
        string Name { get; }
        void Execute(float timeInAnim, SpecialEffect specialEffect, IDictionary<NameId, string> parameters);
    }

    public class PsysExplosionTimeTriggerAction : ISpecialEffectTimeTriggerAction
    {
        private static NameId ObjParamName = "obj";
        private static NameId PsysParamName = "psys";

        private IParticleSystemManager mParticleSystemManager;
        private ContentGroup mContent;

        public string Name 
        { 
            get {return "PsysExplosion"; }
        }

        public PsysExplosionTimeTriggerAction(IParticleSystemManager particleSystemManager, ContentGroup content)
        {
            if (null == particleSystemManager) throw new ArgumentNullException("particleSystemManager");
            if (null == content) throw new ArgumentNullException("content");

            mParticleSystemManager = particleSystemManager;
            mContent = content;
        }
        public void Execute(float timeInAnim, SpecialEffect specialEffect, IDictionary<NameId, string> parameters)
        {
            if (null == specialEffect) throw new ArgumentNullException("specialEffect");
            if (null == parameters) throw new ArgumentNullException("parameters");

            var obj = specialEffect.GetObject(parameters[ObjParamName]);
            mParticleSystemManager.CreateFireAndForgetParticleSystem(mContent.Load<ParticleSystemDesc>(parameters[PsysParamName]), obj.Position);
        }
    }
    public class Sound3dTimeTriggerAction : ISpecialEffectTimeTriggerAction
    {
        private static NameId ObjParamName = "obj";
        private static NameId SoundNameParamName = "soundName";

        private ContentGroup mContent;
        private SoundManager mSoundManager;
        
        public string Name 
        { 
            get {return "Sound3d"; }
        }

        public Sound3dTimeTriggerAction(SoundManager soundManager, ContentGroup content)
        {
            if (null == content) throw new ArgumentNullException("content");
            if (null == soundManager) throw new ArgumentNullException("soundManager");

            mSoundManager = soundManager;
            mContent = content;
        }
        public void Execute(float timeInAnim, SpecialEffect specialEffect, IDictionary<NameId, string> parameters)
        {
            if (null == specialEffect) throw new ArgumentNullException("specialEffect");
            if (null == parameters) throw new ArgumentNullException("parameters");

            var obj = specialEffect.GetObject(parameters[ObjParamName]);
            mContent.Load<Sound3d>(parameters[SoundNameParamName]).Play(obj.Position);
        }
    }
    public class DestroyObjTimeTriggerAction : ISpecialEffectTimeTriggerAction
    {
        private static NameId ObjParamName = "obj";
        
        public string Name
        {
            get { return "DestroyObj"; }
        }

        public void Execute(float timeInAnim, SpecialEffect specialEffect, IDictionary<NameId, string> parameters)
        {
            if (null == specialEffect) throw new ArgumentNullException("specialEffect");
            if (null == parameters) throw new ArgumentNullException("parameters");

            var obj = specialEffect.GetObject(parameters[ObjParamName]);
            obj.Destroy();
        }
    }
}
