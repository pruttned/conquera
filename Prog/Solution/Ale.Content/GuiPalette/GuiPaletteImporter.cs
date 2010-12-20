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

using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Ale.Content
{
    [ContentImporter(".plt", DisplayName = "Gui palette - Ale", DefaultProcessor = "GuiPaletteProcessor")]
    public class GuiPaletteImporter : ContentImporter<GuiPaletteContent>
    {
        public override GuiPaletteContent Import(string filename, ContentImporterContext context)
        {
            //Xsd schema.
            string schemaName = string.Format("{0}.GuiPalette.GuiPalette.xsd", typeof(GuiPaletteImporter).Namespace);
            XmlSchema schema;
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(schemaName))
            {
                schema = XmlSchema.Read(stream, null);
            }

            //Validating, creating, returning.
            XmlDocument document = new XmlDocument();
            document.Schemas.Add(schema);
            document.Load(filename);
            document.Validate(null);
            return new GuiPaletteContent(document.DocumentElement);
        }
    }
}

