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

namespace Ale.Content
{
    public class SpellAnimContent
    {
        public List<SpellAnimObjectContent> Objects { get; private set; }

        public SpellAnimContent()
        {
            Objects = new List<SpellAnimObjectContent>();
        }
    }

    public class CompiledSpellAnim
    {
    }
    [ContentProcessor(DisplayName = "Spell animation")]
    public class SpellAnimProcessor : ContentProcessor<SpellAnimContent, CompiledSpellAnim>
    {
        public override CompiledSpellAnim Process(SpellAnimContent input, ContentProcessorContext context)
        {

            return new CompiledSpellAnim();
        }
    }

    /// <summary>
    /// Ale mesh importer
    /// </summary>
    [ContentImporter(".sanim", DisplayName = "Spell animation", CacheImportedData = true, DefaultProcessor = "SpellAnimProcessor")]
    public class SpellAnimImporter : ContentImporter<SpellAnimContent>
    {
        Dictionary<string, SpellAnimObjectImporter> mSpellAnimObjectImporters = new Dictionary<string, SpellAnimObjectImporter>();
        
        public SpellAnimImporter()
        {
            RegisterSpellAnimObjectImporters();
        }

        public override SpellAnimContent Import(string filename, ContentImporterContext context)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            var content = new SpellAnimContent();

            foreach (XmlNode objectNode in xmlDoc.SelectNodes("/spellAnim/objects/*"))
            {
                content.Objects.Add(mSpellAnimObjectImporters[objectNode.Name].Import(objectNode));
            }

            return content;
        }

        protected void RegisterSpellAnimObjectImporter(SpellAnimObjectImporter importer)
        {
            mSpellAnimObjectImporters.Add(importer.ElementName, importer);
        }
        protected virtual void RegisterSpellAnimObjectImporters()
        {
            RegisterSpellAnimObjectImporter(new MeshSpellAnimObjectImporter());
            RegisterSpellAnimObjectImporter(new ParticleSystemSpellAnimObjectImporter());
            RegisterSpellAnimObjectImporter(new ConnectionPointSpellAnimObjectImporter());
        }

    }


    public class MeshSpellAnimObjectImporter : SpellAnimObjectImporter
    {
        public override string ElementName
        {
            get { return "meshObject"; }
        }

        protected override SpellAnimObjectContent CreateContent()
        {
            return new MeshAnimObjectContent();
        }

        public override SpellAnimObjectContent Import(XmlNode objectNode)
        {
            MeshAnimObjectContent content = (MeshAnimObjectContent)base.Import(objectNode);

            MeshImporter meshImporter = new MeshImporter();
            content.MeshContent = meshImporter.ImportFromXmlNode(objectNode, true);

            return content;
        }
    }
    public class ParticleSystemSpellAnimObjectImporter : SpellAnimObjectImporter
    {
        public override string ElementName
        {
            get { return "particleSystem"; }
        }

        protected override SpellAnimObjectContent CreateContent()
        {
            return new SpellAnimObjectContent();
        }
    }
    public class ConnectionPointSpellAnimObjectImporter : SpellAnimObjectImporter
    {
        public override string ElementName
        {
            get { return "connectionPoint"; }
        }

        protected override SpellAnimObjectContent CreateContent()
        {
            return new SpellAnimObjectContent();
        }
    }

    public abstract class SpellAnimObjectImporter
    {
        public abstract string ElementName { get; }

        public virtual SpellAnimObjectContent Import(XmlNode objectNode)
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


            return content;
        }

        protected abstract SpellAnimObjectContent CreateContent();
    }



    public class SpellAnimObjectContent
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Orientation { get; set; }
        public float Scale { get; set; }
    }

    public class MeshAnimObjectContent : SpellAnimObjectContent
    {
        public AleMeshContent MeshContent { get; set; }
    }







}







