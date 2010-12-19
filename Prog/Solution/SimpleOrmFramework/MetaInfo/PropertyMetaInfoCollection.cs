using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class PropertyMetaInfoCollection : ReadOnlyCollection<IPropertyMetaInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public PropertyMetaInfoCollection(IList<IPropertyMetaInfo> list)
            :base(list)
        {}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="columnName"></param>
        ///// <returns></returns>
        //public IPropertyMetaInfo GetByColumnName(string columnName)
        //{
        //    foreach (IPropertyMetaInfo property in this)
        //    {
        //        if (property.ColumnName == columnName)
        //        {
        //            return property;
        //        }
        //    }

        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IPropertyMetaInfo GetByPropertyName(string propertyName)
        {
            foreach (IPropertyMetaInfo property in this)
            {
                if (property.PropertyName == propertyName)
                {
                    return property;
                }
            }
            return null;
        }
    }
}
