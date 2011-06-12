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
    [ContentImporter(".alsfx", DisplayName = "Ale Special Effect", CacheImportedData = true, DefaultProcessor = "SpecialEffectProcessor")]
    public class SpecialEffectImporter : ContentImporter<SpecialEffectContent>
    {
        Dictionary<string, SpecialEffectObjectImporter> mSpecialEffectObjectImporters = new Dictionary<string, SpecialEffectObjectImporter>();

        public SpecialEffectImporter()
        {
            RegisterSpecialEffectObjectImporters();
        }

        public override SpecialEffectContent Import(string filename, ContentImporterContext context)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            var content = new SpecialEffectContent();

            foreach (XmlNode objectNode in xmlDoc.SelectNodes("/specialEffect/objects/*"))
            {
                content.Objects.Add(mSpecialEffectObjectImporters[objectNode.Name].Import(objectNode));
            }

            return content;
        }

        protected void RegisterSpecialEffectObjectImporter(SpecialEffectObjectImporter importer)
        {
            mSpecialEffectObjectImporters.Add(importer.ElementName, importer);
        }
        protected virtual void RegisterSpecialEffectObjectImporters()
        {
            RegisterSpecialEffectObjectImporter(new MeshSpecialEffectObjectImporter());
            RegisterSpecialEffectObjectImporter(new ParticleSystemSpecialEffectObjectImporter());
            RegisterSpecialEffectObjectImporter(new DummySpecialEffectObjectImporter());
        }

    }

    public abstract class SpecialEffectObjectImporter
    {
        public abstract string ElementName { get; }

        public virtual SpecialEffectObjectContent Import(XmlNode objectNode)
        {
            var content = CreateContent();
            content.Name = objectNode.Attributes["name"].Value;

            Vector3 position;
            Quaternion orientation;
            float scale;
            XmlCommonParser.LoadTransformation(objectNode, out position, out orientation, out scale);
            content.Position = position;
            content.Orientation = orientation;
            content.Scale = scale;

            content.Anim = ImportAnim(objectNode);

            return content;
        }

        protected abstract SpecialEffectObjectContent CreateContent();

        protected SpecialEffectObjectAnim ImportAnim(XmlNode objectNode)
        {
            var animNode = objectNode.SelectSingleNode("./anim");
            if (null == animNode)
            {
                return null;
            }

            float duration = float.Parse(animNode.Attributes["duration"].Value, CultureInfo.InvariantCulture);
            List<SpecialEffectAnimKeyframe> keyframes = new List<SpecialEffectAnimKeyframe>();
            foreach (XmlNode keyframeNode in animNode.SelectNodes("./keyframe"))
            {
                float time = float.Parse(keyframeNode.Attributes["time"].Value, CultureInfo.InvariantCulture);
                Vector3 position;
                Quaternion orientation;
                float scale;
                XmlCommonParser.LoadTransformation(objectNode, out position, out orientation, out scale);
                keyframes.Add(new SpecialEffectAnimKeyframe(time, position, orientation, scale));
            }

            if (0 == keyframes.Count)
            {
                return null;
            }

            return new SpecialEffectObjectAnim(duration, keyframes);
        }
    }
    public class MeshSpecialEffectObjectImporter : SpecialEffectObjectImporter
    {
        public override string ElementName
        {
            get { return "meshObject"; }
        }

        public override SpecialEffectObjectContent Import(XmlNode objectNode)
        {
            MeshSpecialEffectObjectContent content = (MeshSpecialEffectObjectContent)base.Import(objectNode);

            MeshImporter meshImporter = new MeshImporter();
            content.Mesh = meshImporter.ImportFromXmlNode(objectNode, true);

            return content;
        }

        protected override SpecialEffectObjectContent CreateContent()
        {
            return new MeshSpecialEffectObjectContent();
        }
    }
    public class ParticleSystemSpecialEffectObjectImporter : SpecialEffectObjectImporter
    {
        public override string ElementName
        {
            get { return "particleSystem"; }
        }

        public override SpecialEffectObjectContent Import(XmlNode objectNode)
        {
            ParticleSystemSpecialEffectObjectContent content = (ParticleSystemSpecialEffectObjectContent)base.Import(objectNode);

            content.PsysName = objectNode.Attributes["psys"].Value;

            return content;
        }

        protected override SpecialEffectObjectContent CreateContent()
        {
            return new ParticleSystemSpecialEffectObjectContent();
        }
    }
    public class DummySpecialEffectObjectImporter : SpecialEffectObjectImporter
    {
        public override string ElementName
        {
            get { return "dummy"; }
        }

        protected override SpecialEffectObjectContent CreateContent()
        {
            return new DummySpecialEffectObjectContent();
        }
    }
}
