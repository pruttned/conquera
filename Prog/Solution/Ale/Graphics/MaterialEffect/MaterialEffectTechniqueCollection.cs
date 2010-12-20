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
