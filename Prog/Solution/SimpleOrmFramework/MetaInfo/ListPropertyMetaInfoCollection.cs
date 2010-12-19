using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class ListPropertyMetaInfoCollection : ReadOnlyCollection<ListPropertyMetaInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ListPropertyMetaInfoCollection(IList<ListPropertyMetaInfo> list)
            :base(list)
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listTableName"></param>
        /// <returns></returns>
        public ListPropertyMetaInfo GetByListTableName(string listTableName)
        {
            foreach (ListPropertyMetaInfo listProperty in this)
            {
                if (listProperty.ListTable == listTableName)
                {
                    return listProperty;
                }
            }

            throw new ArgumentException(String.Format("List property with binding table '{0}' doesn't exists", listTableName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ListPropertyMetaInfo GetByPropertyName(string propertyName)
        {
            foreach (ListPropertyMetaInfo listProperty in this)
            {
                if (listProperty.PropertyInfo.Name == propertyName)
                {
                    return listProperty;
                }
            }

            throw new ArgumentException(String.Format("List property with name '{0}' doesn't exists", propertyName));
        }
    }
}
