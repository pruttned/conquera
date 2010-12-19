//using System.IO;
//using System.Xml;
//using System.Xml.Schema;
//using Microsoft.Xna.Framework.Content.Pipeline;

//namespace Ale.Content
//{
//    /// <summary>
//    /// Importer for AleMaterialContent
//    /// </summary>
//    [ContentImporter(".mat", DisplayName = "Material - Ale", DefaultProcessor = "MaterialProcessor")]
//    public class AleMaterialImporter : ContentImporter<AleMaterialContent>
//    {
//        /// <summary>
//        /// See ContentImporter
//        /// </summary>
//        /// <param name="filename"></param>
//        /// <param name="context"></param>
//        /// <returns></returns>
//        public override AleMaterialContent Import(string filename, ContentImporterContext context)
//        {
//           //Xsd schema for cfgFile 
//            string appConfigXsdFile = string.Format("{0}.Material.xsd", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name);
//            XmlSchema materialSchema;
//            using (Stream appConfigXsdStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(appConfigXsdFile))
//            {
//                materialSchema = XmlSchema.Read(appConfigXsdStream, null);
//            }

//            XmlDocument materialXmlContent = new XmlDocument();
//            materialXmlContent.Schemas.Add(materialSchema);
//            materialXmlContent.Load(filename);
//            materialXmlContent.Validate(null);

//            return new AleMaterialContent(materialXmlContent);
//        }
//    }
//}
