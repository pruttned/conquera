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
    { }

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
        public override SpellAnimContent Import(string filename, ContentImporterContext context)
        {
            XmlDocument modelDocument = new XmlDocument();
            modelDocument.Load(filename);

            return new SpellAnimContent();
        }
    }












}
