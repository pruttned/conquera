using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    internal class MaterialParamByNameEqualityComparer : IEqualityComparer<MaterialParam>
    {
        static MaterialParamByNameEqualityComparer mInstance = new MaterialParamByNameEqualityComparer();

        public bool Equals(MaterialParam x, MaterialParam y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(MaterialParam obj)
        {
            return obj.Name.GetHashCode();
        }

        private MaterialParamByNameEqualityComparer()
        {
        }

        public static MaterialParamByNameEqualityComparer Instance
        {
            get { return mInstance; }
        }
    }
}
