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
