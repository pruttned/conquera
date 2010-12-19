using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.Common;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Meta info for list property of the data object
    /// </summary>
    class ListPropertyMetaInfo : IPropertyMetaInfo
    {
        public static string ParentColumnName = "ParentDataObject";
        private static readonly Type mIDataObjectType = typeof(IDataObject);
        private static readonly Type mDataObjectAttributeType = typeof(DataObjectAttribute);

        private string mListTable;
        private bool mIsWeakReference;
        private PropertyInfo mPropertyInfo;
        private ConstructorInfo mListDefaultConstructor;
        private bool mIsDataObject;
        private Type mItemType;
        private bool mNotNull;

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
        protected string ListTable
        {
            get { return mListTable; }
            set { mListTable = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsWeakReference
        {
            get { return mIsWeakReference; }
            set { mIsWeakReference = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected PropertyInfo PropertyInfo
        {
            get { return mPropertyInfo; }
            set { mPropertyInfo = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected ConstructorInfo ListDefaultConstructor
        {
            get { return mListDefaultConstructor; }
            set { mListDefaultConstructor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsDataObject
        {
            get { return mIsDataObject; }
            set { mIsDataObject = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected Type ItemType
        {
            get { return mItemType; }
            set { mItemType = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool NotNull
        {
            get { return mNotNull; }
            set { mNotNull = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="itemType"></param>
        /// <param name="dataObjectListPropertyAttribute"></param>
        internal ListPropertyMetaInfo(PropertyInfo propertyInfo, Type itemType, DataListPropertyAttribute dataObjectListPropertyAttribute)
        {
            //get item type
            Type propertyType = propertyInfo.PropertyType;
            mItemType = itemType;

            mListTable = string.Format("{0}.{1}", propertyInfo.DeclaringType.FullName, propertyInfo.Name);

            mIsWeakReference = dataObjectListPropertyAttribute.WeakReference;
            mNotNull = dataObjectListPropertyAttribute.NotNull;

            mPropertyInfo = propertyInfo;

            mListDefaultConstructor = propertyType.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (null == mListDefaultConstructor)
            {
                throw new SofException(string.Format("Type '{0}' doesn't have a parameterless constructor. In order to use the type with SimpleOrmFramewor, it is necessary that it has parameterless onstructor which can be even private", propertyType.FullName));
            }

            //check whether itemType is DataObject
            mIsDataObject = false;
            if (mIDataObjectType.IsAssignableFrom(mItemType))
            {
                //Check DataObjectAttribute
                object[] dataObjectAttributes = mItemType.GetCustomAttributes(mDataObjectAttributeType, false);
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
        public override string ToString()
        {
            return mPropertyInfo.Name;
        }

        public virtual SofDbCommandParameter[] SaveProperty(IDataObject obj, OrmManager ormManager)
        {
            IList list = (IList)GetPropertyValue(obj);

            if (null != list)
            {
                string command = string.Format("INSERT INTO [{0}] VALUES ({1}, @Child)", mListTable, obj.Id);
                SofDbCommandParameter[] dbParams = new SofDbCommandParameter[] { new SofDbCommandParameter("@Child", null, "Child") };

                //save new values
                if (mIsDataObject)
                {
                    //save items
                    foreach (IDataObject item in list)
                    {
                        if (!mIsWeakReference)
                        {
                            item.Id = ormManager.SaveObject(item);
                        }
                        else
                        {
                            if (0 >= item.Id)
                            {
                                throw new WeakReferenceNotInitializedException(string.Format("Item in weak referenced list property '{0}' has not been yet initialized in db (Id<=0). All weak references must be inistialzed before saving object", this));
                            }
                        }
                        dbParams[0].Value = item.Id;
                        ormManager.DataLayerManager.ExecuteNonQueryCommand(command, dbParams);
                    }
                }
                else
                {
                    foreach (object item in list)
                    {
                        dbParams[0].Value = item;
                        ormManager.DataLayerManager.ExecuteNonQueryCommand(command, dbParams);
                    }
                }
            }

            return new SofDbCommandParameter[0];
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
            CleareList(ormManager.DataLayerManager, newObj.Id);

            if (!IsWeakReference && IsDataObject)
            { //delete objects that were removed from list
                IList newList = (IList)PropertyInfo.GetValue(newObj, null);
                IList origList = (IList)PropertyInfo.GetValue(origObj, null);

                HashSet<long> newItemIds = new HashSet<long>();
                foreach (IDataObject newDataObject in newList)
                {
                    newItemIds.Add(newDataObject.Id);
                }

                //find which objects were removed from the list
                foreach (IDataObject origItem in origList)
                {
                    if (!newItemIds.Contains(origItem.Id))
                    {
                        ormManager.DeleteObject(origItem.Id);
                    }
                }
            }

            return SaveProperty(newObj, ormManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetMainTableCreateCommandColumns()
        {
            return new string[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        public virtual void ExecuteAdditionalCreateTableCommands(DataLayerManager dataLayerManager)
        {
            string dataTypeName;

            dataTypeName = DbDataTypeNames.GetDbType(mItemType);

            dataLayerManager.ExecuteNonQueryCommand(string.Format("CREATE TABLE [{0}] ([{3}] LONG NOT NULL, [Child] {1} {2}); CREATE INDEX [Index_{0}] ON [{0}] ([{3}])", mListTable, dataTypeName, mNotNull ? "NOT NULL" : "", ParentColumnName));
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
            IList list = (IList)ListDefaultConstructor.Invoke(null);

            using (DbDataReader dbReader2 = ormManager.DataLayerManager.ExecuteReaderCommand("SELECT [Child] FROM [{0}] WHERE [{2}]={1}", ListTable, newObject.Id.ToString(), ParentColumnName))
            {
                if(IsDataObject)
                {
                    while(dbReader2.Read())
                    {
                        list.Add(ormManager.LoadObjectInter((long)dbReader2[0], loadCache, initializeReferencedObjects, initializeReferencedObjects));
                    }
                }
                else
                {
                    while(dbReader2.Read())
                    {
                        list.Add(dbReader2[0]);
                    }
                }
            }

            SetPropertyValue(newObject, list);
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
            IList srcList = (IList)GetPropertyValue(srcObject);
            IList list = (IList)ListDefaultConstructor.Invoke(null);
            if (null != srcList)
            {
                if (IsDataObject)
                {
                    foreach (object item in srcList)
                    {
                        list.Add(ormManager.LoadObjectInter(((IDataObject)item).Id, loadCache, initializeReferencedObjects, initializeReferencedObjects));
                    }
                }
                else
                {
                    foreach (object item in srcList)
                    {
                        list.Add(item);
                    }
                }
            }
            SetPropertyValue(newObject, list);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        public virtual void DeleteProperty(IDataObject obj, OrmManager ormManager)
        {
            CleareList(ormManager.DataLayerManager, obj.Id);

            if (IsDataObject && !IsWeakReference)
            {
                IList list = (IList)GetPropertyValue(obj);
                if (null != list)
                {
                    foreach (IDataObject item in list)
                    {
                        ormManager.DeleteObject(item.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Remove strong referenced data objects stored in property from cache
        /// </summary>
        /// <param name="ormManager"></param>
        public virtual void RemoveFromCache(OrmManager ormManager, IDataObject obj)
        {
            if (IsDataObject && !IsWeakReference)
            {
                IList list = (IList)GetPropertyValue(obj);
                if (null != list)
                {
                    foreach (IDataObject item in list)
                    {
                        ormManager.RemoveObjectFromCahce(item.Id);
                    }
                }
            }
        }

        public virtual void RepairId(OrmManager ormManager, IDataObject obj)
        {
            if (IsDataObject && !IsWeakReference)
            {
                IList list = (IList)GetPropertyValue(obj);
                if (null != list)
                {
                    foreach (IDataObject item in list)
                    {
                        ormManager.RepairObjectId(item);
                    }
                }
            }
        }

        public virtual void ClearId(OrmManager ormManager, IDataObject obj)
        {
            if (IsDataObject && !IsWeakReference)
            {
                IList list = (IList)GetPropertyValue(obj);
                if (null != list)
                {
                    foreach (IDataObject item in list)
                    {
                        ormManager.ClearObjectId(item);
                    }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObjectTypeListPropertyMetaInfo"></param>
        /// <param name="objectId"></param>
        protected void CleareList(DataLayerManager dataLayerManager, long objectId)
        {
            dataLayerManager.ExecuteScalarCommand(string.Format("DELETE FROM '{0}' WHERE [{2}]={1}", ListTable, objectId, ListPropertyMetaInfo.ParentColumnName));
        }
    }
}
