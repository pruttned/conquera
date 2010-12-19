using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Common;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Meta info for property of the data object
    /// </summary>
    class PropertyMetaInfo : IPropertyMetaInfo
    {
        private static readonly Type mIDataObjectType = typeof(IDataObject);
        private static readonly Type mDataObjectAttributeType = typeof(DataObjectAttribute);
        
        private string mColumnName;
        private PropertyInfo mPropertyInfo;
        private bool mUnique;
        private bool mNotNull;
        private bool mIsDataObject;
        private bool mIsWeakReference;
        private bool mCaseSensitive;

        /// <summary>
        /// 
        /// </summary>
        public string PropertyName
        {
            get { return mPropertyInfo.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get { return mColumnName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type PropertyType
        {
            get { return mPropertyInfo.PropertyType; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="dataObjectPropertyAttribute"></param>
        internal PropertyMetaInfo(PropertyInfo propertyInfo, DataPropertyAttribute dataObjectPropertyAttribute)
        {
            mColumnName = dataObjectPropertyAttribute.Column ?? propertyInfo.Name;
            mPropertyInfo = propertyInfo;
            mUnique = dataObjectPropertyAttribute.Unique;
            mNotNull = dataObjectPropertyAttribute.NotNull;
            mIsWeakReference = dataObjectPropertyAttribute.WeakReference;
            mCaseSensitive = dataObjectPropertyAttribute.CaseSensitive;

            mIsDataObject = false;
            Type type = mPropertyInfo.PropertyType;
            //check whether it is DataObject
            if (mIDataObjectType.IsAssignableFrom(type))
            {
                //Check DataObjectAttribute
                object[] dataObjectAttributes = type.GetCustomAttributes(mDataObjectAttributeType, false);
                if (null != dataObjectAttributes || 0 != dataObjectAttributes.Length)
                {
                    mIsDataObject = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetMainTableCreateCommandColumns()
        {
            string dbDataTypeName = DbDataTypeNames.GetDbType(PropertyType);

            StringBuilder formulaBuilder = new StringBuilder(ColumnName.Length + dbDataTypeName.Length + 5);
            formulaBuilder.Append('[').Append(ColumnName).Append("] ").Append(dbDataTypeName);
            if (mUnique)
            {
                formulaBuilder.Append(" UNIQUE");
            }
            if (mNotNull)
            {
                formulaBuilder.Append(" NOT NULL");
            }
            if (!mCaseSensitive && (typeof(string) == PropertyType))
            {
                formulaBuilder.Append(" COLLATE NOCASE");
            }

            return new string[] { formulaBuilder.ToString() };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        public virtual void ExecuteAdditionalCreateTableCommands(DataLayerManager dataLayerManager)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return mPropertyInfo.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        public virtual SofDbCommandParameter[] SaveProperty(IDataObject obj, OrmManager ormManager)
        {
            object propertyValue = GetPropertyValue(obj);

            if (null != propertyValue)
            {
                if (mIsDataObject)
                {
                    IDataObject iDataObjectPropertyValue = (IDataObject)propertyValue;
                    if (mIsWeakReference)
                    {
                        if (0 >= iDataObjectPropertyValue.Id)
                        {
                            throw new WeakReferenceNotInitializedException(string.Format("Property '{0}' is weak reference that has not been yet initialized in db (Id<=0). All weak references must be inistialzed before saving object", this));
                        }

                        propertyValue = iDataObjectPropertyValue.Id;
                    }
                    else
                    {
                        propertyValue = ormManager.SaveObject(iDataObjectPropertyValue);
                    }
                }
            }
            return new SofDbCommandParameter[] { new SofDbCommandParameter("@" + ColumnName, propertyValue, ColumnName) };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="origObj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        public virtual SofDbCommandParameter[] UpdateProperty(IDataObject newObj, IDataObject origObj, OrmManager ormManager)
        {
            SofDbCommandParameter[] dbParams = SaveProperty(newObj, ormManager);

            //delete original data object stored in property if it is no longer valid
            if (mIsDataObject && !mIsWeakReference)
            {
                IDataObject originalDataObjectPropertyValue = (IDataObject)GetPropertyValue(origObj);
                if (null != originalDataObjectPropertyValue)
                {
                    if (null == dbParams[0].Value || (long)dbParams[0].Value != originalDataObjectPropertyValue.Id)
                    {
                        ormManager.DeleteObject(originalDataObjectPropertyValue.Id);
                    }
                }
            }

            return dbParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="dbReader"></param>
        /// <param name="ormManager"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        public virtual void InitializeProperty(IDataObject newObject, DbDataReader dbReader, OrmManager ormManager, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects)
        {
            object value;
            try
            {
                value = dbReader[ColumnName];
                value = (value is DBNull ? null : value);
            }
            catch (IndexOutOfRangeException ex)
            {
                throw new SofException(string.Format("Column '{0}' for property '{1}' in type '{2}' couldn't be found. This may be caused by db and code inconsistency", ColumnName, PropertyName, mPropertyInfo.DeclaringType.FullName), ex);
            }

            if (null != value && mIsDataObject)
            {//load object
                value = ormManager.LoadObjectInter((long)value, loadCache, initializeReferencedObjects, initializeReferencedObjects);
            }

            SetPropertyValue(newObject, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newObject"></param>
        /// <param name="srcObject"></param>
        /// <param name="ormManager"></param>
        /// <param name="loadCache"></param>
        /// <param name="initializeReferencedObjects"></param>
        public virtual void InitializeProperty(IDataObject newObject, IDataObject srcObject, OrmManager ormManager, Dictionary<long, IDataObject> loadCache, bool initializeReferencedObjects)
        {
            object value = GetPropertyValue(srcObject);

            if (null != value && mIsDataObject)
            { //load object again because object in value may be already invalid (cahce may be inconsistent (for instance: object was deleted but references was not upadted)
                value = ormManager.LoadObjectInter(((IDataObject)value).Id, loadCache, initializeReferencedObjects, initializeReferencedObjects);
            }

            SetPropertyValue(newObject, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        public virtual void DeleteProperty(IDataObject obj, OrmManager ormManager)
        {
            if (mIsDataObject && !mIsWeakReference)
            {
                IDataObject value = (IDataObject)GetPropertyValue(obj);
                if (null != value)
                {
                    ormManager.DeleteObject(value.Id);
                }
            }
        }

        /// <summary>
        /// Remove strong referenced data objects stored in property from cache
        /// </summary>
        /// <param name="ormManager"></param>
        public virtual void RemoveFromCache(OrmManager ormManager, IDataObject obj)
        {
            if (mIsDataObject && !mIsWeakReference)
            {
                IDataObject value = (IDataObject)GetPropertyValue(obj);
                if (null != value)
                {
                    ormManager.RemoveObjectFromCahce(value.Id);
                }
            }
        }

        public virtual void RepairId(OrmManager ormManager, IDataObject obj)
        {
            if (mIsDataObject && !mIsWeakReference)
            {
                IDataObject value = (IDataObject)GetPropertyValue(obj);
                if (null != value)
                {
                    ormManager.RepairObjectId(value);
                }
            }
        }

        public virtual void ClearId(OrmManager ormManager, IDataObject obj)
        {
            if (mIsDataObject && !mIsWeakReference)
            {
                IDataObject value = (IDataObject)GetPropertyValue(obj);
                if (null != value)
                {
                    ormManager.ClearObjectId(value);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected object GetPropertyValue(object obj)
        {
            return mPropertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        protected void SetPropertyValue(object obj, object value)
        {
            mPropertyInfo.SetValue(obj, value, null);
        }
    }
}
