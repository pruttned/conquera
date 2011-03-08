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
    /// Collection of material parameters
    /// </summary>
    /// <remarks>
    /// Parameters are stored in a list becasue dictionary is inefficient here. This collection is iterated often and 
    /// searching for its items by name should be not used too often. This approach is also better for saving a memory.
    /// You should cache MaterialParam if you need to update it manually.
    /// </remarks>
    class MaterialParamCollection : List<MaterialParam>
    {
        #region Properties

        /// <summary>
        /// Finds the material parameter by its name.
        /// Don't use this method too often. You should cache MaterialParam if you need to update it manually.
        /// </summary>
        /// <param name="name">- Parameter's name</param>
        /// <returns>MaterialParam with a given name or null if parameter with a given name doesn't exists</returns>
        public MaterialParam this[string name] 
        {
            get
            {
                int index = FindParam(name);
                if (-1 != index)
                {
                    return this[index];
                }
                return null;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        internal MaterialParamCollection()
            : base()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal MaterialParamCollection(int capacity)
            : base(capacity)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="materialParams">- List of material parameters that will be used as a internal storage</param>
        internal MaterialParamCollection(IList<MaterialParam> materialParams)
            : base(materialParams)
        {
        }

        /// <summary>
        /// Applies all material parameters. This means that the each material 
        /// parameter updates its value in a coresponding effect parameter.
        /// </summary>
        internal void Apply()
        {
            for (int i = 0; i < Count; ++i)
            {
                this[i].Apply();
            }
        }

        /// <summary>
        /// Add or replaces a parameter
        /// </summary>
        /// <param name="param"></param>
        internal void SetParam(MaterialParam param)
        {
            int index = FindParam(param.Name);
            if (-1 != index)
            {
                this[index] = param;
            }
            else
            {
                this.Add(param);
            }
        }

        private int FindParam(string name)
        {
            for (int i = 0; i < Count; ++i)
            {
                MaterialParam param = this[i];
                if (name == param.Name)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion Methods
    }
}
