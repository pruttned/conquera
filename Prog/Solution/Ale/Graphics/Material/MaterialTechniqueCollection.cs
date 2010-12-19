using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of material techniques
    /// </summary>
    public class MaterialTechniqueCollection : ReadOnlyDictionary<NameId, MaterialTechnique>
    {
        #region Properties

        /// <summary>
        /// Finds the material technique by its name.
        /// </summary>
        /// <param name="name">- Technique's name</param>
        /// <returns>MaterialTechnique with a given name</returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException"></exception>
        public MaterialTechnique this[string name] 
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
        /// <param name="materialTechniques">- dict of material techniques that will be used as a internal storage</param>
        internal MaterialTechniqueCollection(Dictionary<NameId, MaterialTechnique> materialTechniques)
            : base(materialTechniques)
        {
        }

        #endregion Methods
    }
}
