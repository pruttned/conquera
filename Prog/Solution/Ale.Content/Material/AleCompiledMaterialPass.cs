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

//using System;
//using System.Collections.Generic;
//using System.Xml;
//using Ale.Content.Tools;

//namespace Ale.Content
//{
//    /// <summary>
//    /// Compiled material pass
//    /// </summary>
//    public class AleCompiledMaterialPass
//    {
//        #region Fields

//        /// <summary>
//        /// Name of the pass
//        /// </summary>
//        private string mName;
        
//        /// <summary>
//        /// Passe's parameters
//        /// </summary>
//        private HashSet<AleCompiledMaterialParam> mParameters = new HashSet<AleCompiledMaterialParam>();

//        #endregion Fields

//        #region Properties

//        /// <summary>
//        /// Gets tha passe's name
//        /// </summary>
//        public string Name
//        {
//            get { return mName; }
//        }

//        /// <summary>
//        /// Gets the passe's parameters
//        /// </summary>
//        public HashSet<AleCompiledMaterialParam> Parameters
//        {
//            get { return mParameters; }
//        }

//        #endregion Properties

//        #region Methods

//        /// <summary>
//        /// Ctor
//        /// </summary>
//        /// <param name="passNode">- Xml pass node that describes this pass</param>
//        /// <param name="techniqueParameters">- Material parameters described in the parent technique</param>
//        public AleCompiledMaterialPass(XmlNode passNode, HashSet<AleCompiledMaterialParam> techniqueParameters)
//        {
//            mName = passNode.Attributes["name"].Value;

//            try
//            {
//                //parameters
//                foreach (XmlNode paramNode in passNode.SelectNodes(@"./params/param"))
//                {
//                    mParameters.Add(new AleCompiledMaterialParam(paramNode));
//                }
//                //union with technique's parameters
//                mParameters.UnionWith(techniqueParameters);
//            }
//            catch (Exception ex)
//            {
//                throw new AleInvalidContentException(string.Format("Error occurred while parsing paramters of the '{0}' material pass", Name), ex);
//            }
//        }

//        #endregion Methods

//    }
//}
