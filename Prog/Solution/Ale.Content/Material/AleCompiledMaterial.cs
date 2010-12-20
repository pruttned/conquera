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
//    /// <summary>
//    /// Compiled material
//    /// </summary>
//    public class AleCompiledMaterial
//    {
//        #region Fields

//        /// <summary>
//        /// Name of the effect on which is this material based
//        /// </summary>
//        private string mEffect;

//        /// <summary>
//        /// Material's techniques
//        /// </summary>
//        List<AleCompiledMaterialTechnique> mTechniques = new List<AleCompiledMaterialTechnique>();
        
//        #endregion Fields

//        #region Properties

//        /// <summary>
//        /// Gets the name of the effect on which is this material based
//        /// </summary>
//        public string Effect
//        {
//            get { return mEffect; }
//        }

//        /// <summary>
//        /// Gets all material's techniques
//        /// </summary>
//        public List<AleCompiledMaterialTechnique> Techniques
//        {
//            get { return mTechniques; }
//        }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="materialNode">- Xml material node that describes this material</param>
//        public AleCompiledMaterial(XmlNode materialNode)
//        {
//            mEffect = materialNode.Attributes["effect"].Value;

//            //parameters
//            HashSet<AleCompiledMaterialParam> materialParameters = new HashSet<AleCompiledMaterialParam>();
//            foreach (XmlNode paramNode in materialNode.SelectNodes(@"./params/param"))
//            {
//                materialParameters.Add(new AleCompiledMaterialParam(paramNode));
//            }

//            //techniques
//            foreach (XmlNode techniqueNode in materialNode.SelectNodes(@"./techniques/technique"))
//            {
//                mTechniques.Add(new AleCompiledMaterialTechnique(techniqueNode, materialParameters));
//            }
//        }

//        #endregion Methods
//    }
//}
