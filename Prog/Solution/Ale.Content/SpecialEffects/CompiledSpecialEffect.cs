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
    public class CompiledSpecialEffect
    {
        public List<CompiledSpecialEffectObject> Objects { get; private set; }

        public CompiledSpecialEffect()
        {
            Objects = new List<CompiledSpecialEffectObject>();
        }
    }


    public abstract class CompiledSpecialEffectObject
    {
        public abstract string ObjType { get; }

        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }
        public float Scale { get; set; }

        public SpecialEffectObjectAnim Anim { get; set; }

        public virtual void Write(ContentWriter output)
        {
            output.Write(ObjType);
            output.Write(Name);
            output.Write(Position);
            output.Write(Orientation);
            output.Write(Scale);

            //anim
            WriteAnim(output);
        }

        protected virtual void WriteAnim(ContentWriter output)
        {
            if (null == Anim || 0 == Anim.Keyframes.Count)
            {
                output.Write(0);
            }
            else
            {
                output.Write(Anim.Keyframes.Count);
                output.Write(Anim.Duration);
                foreach (var keyframe in Anim.Keyframes)
                {
                    output.Write(keyframe.Time);
                    output.Write(keyframe.Translation);
                    output.Write(keyframe.Orientation);
                    output.Write(keyframe.Scale);
                }
            }
        }
    }

    public class MeshCompiledSpecialEffectObject : CompiledSpecialEffectObject
    {
        public override string ObjType
        {
            get { return "Mesh"; }
        }

        public AleCompiledMesh Mesh { get; set; }

        public override void Write(ContentWriter output)
        {
            base.Write(output);
            Mesh.Write(output);
        }
    }
    public class ParticleSystemCompiledSpecialEffectObject : CompiledSpecialEffectObject
    {
        public override string ObjType
        {
            get { return "Psys"; }
        }

        public string PsysName { get; set; }

        public override void Write(ContentWriter output)
        {
            base.Write(output);
            output.Write(PsysName);
        }
    }
    public class DummyCompiledSpecialEffectObject : CompiledSpecialEffectObject
    {
        public override string ObjType
        {
            get { return "Dummy"; }
        }
    }

}
