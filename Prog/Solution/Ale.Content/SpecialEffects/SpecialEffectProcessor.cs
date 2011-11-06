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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Ale.Content.Tools;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Ale.SpecialEffects;

namespace Ale.Content
{

    [ContentProcessor(DisplayName = "Ale Special Effect")]
    public class SpecialEffectProcessor : ContentProcessor<SpecialEffectContent, CompiledSpecialEffect>
    {
        public override CompiledSpecialEffect Process(SpecialEffectContent input, ContentProcessorContext context)
        {
            float duration = 0;
            var compiledSpecialEffect = new CompiledSpecialEffect();
            //objects
            foreach (var objC in input.Objects)
            {
                var obj = objC.Process(context);
                compiledSpecialEffect.Objects.Add(obj);

                if (null != obj.Anim && duration < obj.Anim.Duration)
                {
                    duration = obj.Anim.Duration;
                }
            }
            //time triggers
            foreach (var trigger in input.TimeTriggers)
            {
                if (duration < trigger.Time)
                {
                    duration = trigger.Time;
                }
            }
            compiledSpecialEffect.TimeTriggers = new List<SpecialEffectTimeTriggerContent>(input.TimeTriggers);

            compiledSpecialEffect.Duration = duration;
            
            return compiledSpecialEffect;
        }
    }
}







