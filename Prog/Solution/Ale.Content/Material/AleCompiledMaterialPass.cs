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
