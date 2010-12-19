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

//using System.Collections.Generic;
//using System.Xml;

//namespace Ale.Content
//{
//    public class AleCompiledMaterialTechnique
//    {
//        #region Fields

//        /// <summary>
//        /// Name of the technique
//        /// </summary>
//        private string mName;

//        /// <summary>
//        /// Technique's passes
//        /// </summary>
//        List<AleCompiledMaterialPass> mPasses = new List<AleCompiledMaterialPass>();

//        #endregion Fields

//        #region Properties

//        /// <summary>
//        /// Gets the technique's name
//        /// </summary>
//        public string Name
//        {
//            get { return mName; }
//        }

//        /// <summary>
//        /// Gets all passes of this technique
//        /// </summary>
//        public List<AleCompiledMaterialPass> Passes
//        {
//            get { return mPasses; }
//        }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="techniqueNode">- Xml technique node that describes this technique</param>
//        /// <param name="materialParameters">- Material parameters described in the parent material</param>
//        public AleCompiledMaterialTechnique(XmlNode techniqueNode, HashSet<AleCompiledMaterialParam> materialParameters)
//        {
//            mName = techniqueNode.Attributes["name"].Value;

//            //parameters
//            HashSet<AleCompiledMaterialParam> techniqueParameters = new HashSet<AleCompiledMaterialParam>();
//            foreach (XmlNode paramNode in techniqueNode.SelectNodes(@"./params/param"))
//            {
//                techniqueParameters.Add(new AleCompiledMaterialParam(paramNode));
//            }
//            //union with material's parameters
//            techniqueParameters.UnionWith(materialParameters);

//            //pases
//            foreach (XmlNode passNode in techniqueNode.SelectNodes(@"./passes/pass"))
//            {
//                mPasses.Add(new AleCompiledMaterialPass(passNode, techniqueParameters));
//            }
//        }
//        #endregion Methods
//    }
//}
