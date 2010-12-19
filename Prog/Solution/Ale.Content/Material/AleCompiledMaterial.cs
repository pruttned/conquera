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
