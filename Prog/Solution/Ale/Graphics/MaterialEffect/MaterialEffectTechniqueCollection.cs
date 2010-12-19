using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of material effect techniques
    /// </summary>
    public class MaterialEffectTechniqueCollection : ReadOnlyDictionary<NameId, MaterialEffectTechnique>
    {
        #region Properties

        /// <summary>
        /// Finds the material effect technique by its name.
        /// Don't use this method too often because it has O(n) complexity.
        /// </summary>
        /// <param name="name">- Technique's name</param>
        /// <returns>MaterialEffectTechnique</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public MaterialEffectTechnique this[string name] 
        {
            get
            {
                return this[NameId.FromName(name)];
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="materialEffectTechniques">- List of material effect techniques that will be used as a internal storage</param>
        internal MaterialEffectTechniqueCollection(Dictionary<NameId, MaterialEffectTechnique> materialEffectTechniques)
            : base(materialEffectTechniques)
        {
        }

        #endregion Methods
    }
}
