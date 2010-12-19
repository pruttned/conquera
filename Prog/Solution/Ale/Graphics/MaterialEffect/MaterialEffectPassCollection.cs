using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of material effect passes
    /// </summary>
    public class MaterialEffectPassCollection : ReadOnlyCollection<MaterialEffectPass>
    {
        #region Properties

        /// <summary>
        /// Finds the material effect pass by its name.
        /// Don't use this method too often because it has O(n) complexity.
        /// </summary>
        /// <param name="name">- Passe's name</param>
        /// <returns>MaterialEffectPass with a given name or null if pass with a given name doesn't exists</returns>
        public MaterialEffectPass this[string name] 
        {
            get
            {
                for (int i = 0; i < Count; ++i)
                {
                    MaterialEffectPass pass = this[i];
                    if (name == pass.Name)
                    {
                        return pass;
                    }
                }
                return null;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="materialEffectPasses">- List of material effect passes that will be used as a internal storage</param>
        internal MaterialEffectPassCollection(IList<MaterialEffectPass> materialEffectPasses)
            : base(materialEffectPasses)
        {
        }

        #endregion Methods
    }
}
