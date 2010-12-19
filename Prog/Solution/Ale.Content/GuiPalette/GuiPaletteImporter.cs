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

