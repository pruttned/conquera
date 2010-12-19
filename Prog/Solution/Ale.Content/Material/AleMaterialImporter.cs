//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
