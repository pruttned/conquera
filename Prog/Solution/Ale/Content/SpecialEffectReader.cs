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

using Microsoft.Xna.Framework.Content;
using Ale.Graphics;
using Ale.SpecialEffects;
using System.Collections.Generic;
using Ale.Tools;

namespace Ale.Content
{
    public class SpecialEffectReader : ContentTypeReader<SpecialEffectDesc>
    {
        protected override SpecialEffectDesc Read(ContentReader input, SpecialEffectDesc existingInstance)
        {
            return new SpecialEffectDesc(input);
        }
    }


    public abstract class SpecialEffectObjectReader
    {
        public virtual SpecialEffectObjectDesc Read(ContentReader input)
        {
            var obj = CreateSpecialEffectObject();

            obj.Name = input.ReadString();
            obj.Position = input.ReadVector3();
            obj.Orientation = input.ReadQuaternion();
            obj.Scale = input.ReadSingle();

            //animations
            obj.Anim = ReadAnimation(input);
            return obj;
        }

        protected abstract SpecialEffectObjectDesc CreateSpecialEffectObject();

        protected virtual SpecialEffectObjectAnim ReadAnimation(ContentReader input)
        {
            int keyframeCnt = input.ReadInt32();
            if (0 == keyframeCnt)
            {
                return null;
            }
            float duration = input.ReadSingle();
            SpecialEffectAnimKeyframe[] keyframes = new SpecialEffectAnimKeyframe[keyframeCnt];
            for (int i = 0; i < keyframeCnt; ++i)
            {
                float time = input.ReadSingle();
                var translation = input.ReadVector3();
                var orientation = input.ReadQuaternion();
                var scale = input.ReadSingle();
                keyframes[i] = new SpecialEffectAnimKeyframe(time, translation, orientation, scale);
            }
            return new SpecialEffectObjectAnim(duration, keyframes);
        }
    }
    public class MeshSpecialEffectObjectReader : SpecialEffectObjectReader
    {
        public override SpecialEffectObjectDesc Read(ContentReader input)
        {
            var obj = (MeshSpecialEffectObjectDesc)base.Read(input);

            obj.Mesh = new Mesh(input);
            Dictionary<NameId, Material> materials = new Dictionary<NameId, Material>();
            //materials
            foreach (var meshPart in obj.Mesh.MeshParts)
            {
                string materialGroup = meshPart.MaterialGroup.ToString();
                materials[materialGroup] = input.ContentManager.Load<Material>(materialGroup);
            }

            obj.Materials = new ReadOnlyDictionary<NameId, Material>(materials);
            return obj;
        }
        protected override SpecialEffectObjectDesc CreateSpecialEffectObject()
        {
            return new MeshSpecialEffectObjectDesc();
        }
    }
    public class ParticleSystemSpecialEffectObjectReader : SpecialEffectObjectReader
    {
        public override SpecialEffectObjectDesc Read(ContentReader input)
        {
            var obj = (ParticleSystemSpecialEffectObjectDesc)base.Read(input);

            obj.Psys = input.ContentManager.Load<ParticleSystemDesc>(input.ReadString());

            return obj;
        }
        protected override SpecialEffectObjectDesc CreateSpecialEffectObject()
        {
            return new ParticleSystemSpecialEffectObjectDesc();
        }
    }
    public class DummySpecialEffectObjectReader : SpecialEffectObjectReader
    {
        protected override SpecialEffectObjectDesc CreateSpecialEffectObject()
        {
            return new DummySpecialEffectObjectDesc();
        }
    }
}
