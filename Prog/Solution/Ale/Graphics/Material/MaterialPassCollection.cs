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
    /// Collection of material passes
    /// </summary>
    public class MaterialPassCollection : ReadOnlyCollection<MaterialPass>
    {
        #region Properties

        /// <summary>
        /// Finds the material pass by its name.
        /// Don't use this method too often because it has O(n) complexity.
        /// </summary>
        /// <param name="name">- Passes's name</param>
        /// <returns>MaterialPass with a given name or null if pass with a given name doesn't exists</returns>
        public MaterialPass this[string name] 
        {
            get
            {
                for (int i = 0; i < Count; ++i)
                {
                    MaterialPass pass = this[i];
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
        /// <param name="materialPasses">- List of material passes that will be used as a internal storage</param>
        internal MaterialPassCollection(IList<MaterialPass> materialPasses)
            : base(materialPasses)
        {
        }

        #endregion Methods
    }
}
