using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.Common;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class CustomBasicTypePropertyMetaInfo : PropertyMetaInfo
    {
        private ICustomBasicTypeProvider mCustomBasicTypeProvider;
        private SubProperty[] mSubProperties;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="dataObjectPropertyAttribute"></param>
        /// <param name="customBasicTypeProvider"></param>
        public CustomBasicTypePropertyMetaInfo(PropertyInfo propertyInfo, DataPropertyAttribute dataObjectPropertyAttribute, ICustomBasicTypeProvider customBasicTypeProvider)
            :base(propertyInfo, dataObjectPropertyAttribute)
        {
            if (!propertyInfo.PropertyType.IsValueType)
            {
                throw new ArgumentException(string.Format("Type '{0}' must be value type", propertyInfo.PropertyType.FullName));
            }

            mCustomBasicTypeProvider = customBasicTypeProvider;
 
            MemberInfo[] dataProperties = customBasicTypeProvider.GetDataProperties();
            if(0 == dataProperties.Length)
            {
                throw new ArgumentException(string.Format("Zero dataProperties provided from '{0}'", customBasicTypeProvider.GetType().ToString()));
            }

            mSubProperties = new SubProperty[dataProperties.Length];
            for (int i = 0; i < dataProperties.Length; ++i)
            {
                mSubProperties[i] = new SubProperty(dataProperties[i], string.Format("{0}_{1}", ColumnName, dataProperties[i].Name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string[] GetMainTableCreateCommandColumns()
        {
            string[] columnFormulas = new string[mSubProperties.Length];
            for(int i = 0; i < mSubProperties.Length; ++i)
            {
                SubProperty property = mSubProperties[i];
                string dbDataTypeName = DbDataTypeNames.GetDbType(property.PropertyType);
                columnFormulas[i] = string.Format("[{0}] {1}", property.Column, dbDataTypeName);
            }

            return columnFormulas;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        public override SofDbCommandParameter[] SaveProperty(IDataObject obj, OrmManager ormManager)
        {
            object propertyValue = GetPropertyValue(obj);

            SofDbCommandParameter[] dbParams = new SofDbCommandParameter[mSubProperties.Length];
            for (int i = 0; i < mSubProperties.Length; ++i)
            {
                SubProperty subProperty = mSubProperties[i];
                object subPropertyValue = subProperty.GetValue(propertyValue);
                dbParams[i] = new SofDbCommandParameter("@" + subProperty.Column, subPropertyValue, subProperty.Column);
            }
            return dbParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="origObj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        public override SofDbCommandParameter[] UpdateProperty(IDataObject newObj, IDataObject origObj, OrmManager ormManager)
        {
            return SaveProperty(newObj, ormManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="dbReader"></param>
        /// <param name="ormManager"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        public override void InitializeProperty(IDataObject newObject, DbDataReader dbReader, OrmManager ormManager, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects)
        {
            object value = mCustomBasicTypeProvider.CreateInstance();

            foreach (SubProperty property in mSubProperties)
            {
                try
                {
                    object propertyValue = dbReader[property.Column];
                    propertyValue = (propertyValue is DBNull ? null : propertyValue);
                    property.SetValue(value, propertyValue);
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new SofException(string.Format("Column '{0}' for property '{1}' in type '{2}' couldn't be found. This may be caused by db and code inconsistency", property.Column, property.Name, value.GetType().Name), ex);
                }
            }

            SetPropertyValue(newObject, value);
        }
    }
}
