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
