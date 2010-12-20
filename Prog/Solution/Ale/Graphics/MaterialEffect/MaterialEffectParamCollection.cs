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

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of material effect parameters
    /// </summary>
    /// <remarks>
    /// Parameters are stored in a list becasue dictionary is inefficient here. This collection is iterated often and 
    /// searching for its items by name should be not used too often. This approach is also better for saving a memory.
    /// You should cache MaterialEffectParam if you need to update it manually.
    /// </remarks>
    public class MaterialEffectParamCollection : ReadOnlyCollection<MaterialEffectParam>
    {
        #region Properties

        /// <summary>
        /// Finds the material effect parameter by its name.
        /// Don't use this method too often. You should cache MaterialEffectParam if you need to update it manually.
        /// </summary>
        /// <param name="name">- Parameter's name</param>
        /// <returns>MaterialEffectParam with a given name or null if parameter with a given name doesn't exists</returns>
        public MaterialEffectParam this[string name] 
        {
            get
            {
                for (int i = 0; i < Count; ++i)
                {
                    MaterialEffectParam param = this[i];
                    if (name == param.Name)
                    {
                        return param;
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
        /// <param name="materialEffectParams">- List of material effect parameters that will be used as a internal storage</param>
        internal MaterialEffectParamCollection(IList<MaterialEffectParam> materialEffectParams)
            : base(materialEffectParams)
        {
        }

        /// <summary>
        /// Gets the first parameter that matches a given semantic.
        /// (Case is ignored)
        /// </summary>
        /// <param name="semantic"></param>
        /// <returns>Parameter or null</returns>
        public MaterialEffectParam GetParamBySemantic(string semantic)
        {
            for (int i = 0; i < Count; ++i)
            {
                MaterialEffectParam param = this[i];
                if (string.Equals(semantic, param.Semantic, System.StringComparison.OrdinalIgnoreCase))
                {
                    return param;
                }
            }
            return null;
        }

        #endregion Methods
    }
}
