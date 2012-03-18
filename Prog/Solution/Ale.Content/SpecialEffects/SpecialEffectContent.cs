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
using Microsoft.Xna.Framework;
using Ale.SpecialEffects;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Ale.Content
{
    public class SpecialEffectContent
    {
        public List<SpecialEffectObjectContent> Objects { get; private set; }
        public List<SpecialEffectTimeTriggerContent> TimeTriggers { get; private set; }

        public SpecialEffectContent()
        {
            Objects = new List<SpecialEffectObjectContent>();
            TimeTriggers = new List<SpecialEffectTimeTriggerContent>();
        }
    }

    public class SpecialEffectTimeTriggerContent
    {
        public float Time { get; set; }
        public string Action { get; set; }
        public Dictionary<string, string> Params { get; private set; }

        public SpecialEffectTimeTriggerContent()
        {
            Params = new Dictionary<string, string>();
        }
    }


    public abstract class SpecialEffectObjectContent
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }
        public float Scale { get; set; }

        public SpecialEffectObjectAnim Anim { get; set; }

        public virtual CompiledSpecialEffectObject Process(ContentProcessorContext context)
        {
            CompiledSpecialEffectObject c = CreateCompiledSpecialEffectObject();
            c.Name = Name;
            c.Position = Position;
            c.Orientation = Orientation;
            c.Scale = Scale;
            c.Anim = Anim;

            return c;
        }

        protected abstract CompiledSpecialEffectObject CreateCompiledSpecialEffectObject();
    }

    public class MeshSpecialEffectObjectContent : SpecialEffectObjectContent
    {
        public AleMeshContent Mesh { get; set; }

        public override CompiledSpecialEffectObject Process(ContentProcessorContext context)
        {
            var c = (MeshCompiledSpecialEffectObject)base.Process(context);
            c.Mesh = new MeshProcessor().Process(Mesh, context);
            return c;
        }

        protected override CompiledSpecialEffectObject CreateCompiledSpecialEffectObject()
        {
            return new MeshCompiledSpecialEffectObject();
        }
    }
    public class ParticleSystemSpecialEffectObjectContent : SpecialEffectObjectContent
    {
        public string PsysName { get; set; }

        public override CompiledSpecialEffectObject Process(ContentProcessorContext context)
        {
            var c = (ParticleSystemCompiledSpecialEffectObject)base.Process(context);
            c.PsysName = PsysName;

            return c;
        }

        protected override CompiledSpecialEffectObject CreateCompiledSpecialEffectObject()
        {
            return new ParticleSystemCompiledSpecialEffectObject();
        }
    }
    public class DummySpecialEffectObjectContent : SpecialEffectObjectContent
    {
        protected override CompiledSpecialEffectObject CreateCompiledSpecialEffectObject()
        {
            return new DummyCompiledSpecialEffectObject();
        }
    }


}
