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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data.SQLite;
using System.Transactions;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Meta info of the data object
    /// </summary>
    class DataObjectTypeMetaInfo
    {
        private static PropertyInfo mIdPropertyInfo;
        private static string mIdPropertyInfoExplicitName;

        private string mTableName;
        private Type mType;
        private int mTypeId;
        private PropertyMetaInfoCollection mProperties;
        private ConstructorInfo mDefaultCtor;
        private DataObjectTypeMetaInfo mBaseType;
        private int mMaxCachedCnt;

        /// <summary>
        /// 
        /// </summary>
        public string TableName
        {
            get { return mTableName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyMetaInfoCollection Properties
        {
            get { return mProperties; }
        }


        /// <summary>
        /// 
        /// </summary>
        public Type DescribedType
        {
            get { return mType; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DescribedTypeId
        {
            get { return mTypeId; }
            internal set { mTypeId = value; } //don't foreg to call thus in DataObjectTypeMetaInfoManager
        }

        /// <summary>
        /// 
        /// </summary>
        public ConstructorInfo DefaultCtor
        {
            get { return mDefaultCtor; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataObjectTypeMetaInfo BaseType
        {
            get { return mBaseType; }
        }

        /// <summary>
        /// Gets/sets the maximum number of objects of this type stored in cache. -1 = unlimited.
        /// </summary>
        public int MaxCachedCnt
        {
            get { return mMaxCachedCnt; }
        }

        /// <summary>
        /// 
        /// </summary>
        static DataObjectTypeMetaInfo()
        {
            mIdPropertyInfo = typeof(IDataObject).GetProperty("Id");
            if (null == mIdPropertyInfo)
            {
                //this should not happen
                throw new SofException("Id property could't be found in IDataObject interface");
            }
            mIdPropertyInfoExplicitName = string.Format("{0}.{1}", mIdPropertyInfo.DeclaringType.FullName, mIdPropertyInfo.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObjectTypeMetaInfoManager"></param>
        /// <param name="type"></param>
        /// <param name="changedDb"></param>
        /// <returns></returns>
        public static DataObjectTypeMetaInfo TryCreateFromType(DataObjectTypeMetaInfoManager dataObjectTypeMetaInfoManager, Type type, out bool changedDb)
        {
            DataObjectAttribute dataObjectAttribute = AttributeHelper.FindAttribute<DataObjectAttribute>(type);
            if (null == dataObjectAttribute)
            {
                changedDb = false;
                return null;
            }
            return new DataObjectTypeMetaInfo(dataObjectTypeMetaInfoManager, type, dataObjectAttribute, out changedDb);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return DescribedType.FullName;
        }

        /// <summary>
        /// Finds a specified property. Includes properties in base types
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IPropertyMetaInfo FindProperty(string propertyName)
        {
            IPropertyMetaInfo property = Properties.GetByPropertyName(propertyName);
            if (null == property)
            {
                if (null != mBaseType) //check base type
                {
                    property = mBaseType.FindProperty(propertyName);
                }
            }

            return property;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObjectTypeMetaInfoManager"></param>
        /// <param name="type"></param>
        /// <param name="dataObjectAttribute"></param>
        /// <param name="changedDb"></param>
        private DataObjectTypeMetaInfo(DataObjectTypeMetaInfoManager dataObjectTypeMetaInfoManager, Type type, DataObjectAttribute dataObjectAttribute, out bool changedDb)
        {
            mType = type;

            List<IPropertyMetaInfo> properties = new List<IPropertyMetaInfo>();
            mProperties = new PropertyMetaInfoCollection(properties);

            mTableName = dataObjectAttribute.Table ?? type.FullName;
            mMaxCachedCnt = dataObjectAttribute.MaxCachedCnt;

            mDefaultCtor = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (null == mDefaultCtor)
            {
                throw new SofException(string.Format("Type '{0}' doesn't have a parameterless constructor. In order to use the type with SimpleOrmFramewor, it is necessary that it has parameterless onstructor which can be even private", type.FullName));
            }

            //get custom basic type providers
            Dictionary<Type, ICustomBasicTypeProvider> customBasicTypeProviders = new Dictionary<Type, ICustomBasicTypeProvider>();
            foreach (CustomBasicTypeProviderAttribute customBasicTypeProviderAttribute in type.GetCustomAttributes(typeof(CustomBasicTypeProviderAttribute), false))
            {
                customBasicTypeProviders.Add(customBasicTypeProviderAttribute.BasicType, (ICustomBasicTypeProvider)Activator.CreateInstance(customBasicTypeProviderAttribute.ProviderType, false));
            }

            foreach (PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                IDataPropertyAttribute dataPropertyAttribute = AttributeHelper.FindAttribute<IDataPropertyAttribute>(propertyInfo);
                if (null != dataPropertyAttribute)
                {
                    if (mIdPropertyInfo.Name == propertyInfo.Name ||
                        mIdPropertyInfoExplicitName == propertyInfo.Name)
                    {
                        throw new SofException("It is not allowed to mark Id property as a data property");
                    }

                    properties.Add(dataPropertyAttribute.CreatePropertyMetaInfo(propertyInfo, customBasicTypeProviders));
                }
            }

            mTypeId = EnsureDataObjectTypeInDb(dataObjectTypeMetaInfoManager.DataLayerManager, out changedDb);

            //base type
            bool baseTypeChangedDb;
            mBaseType = GetBaseDataObjectType(dataObjectTypeMetaInfoManager, type, dataObjectAttribute, out baseTypeChangedDb);

            changedDb = changedDb | baseTypeChangedDb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataObjectTypeMetaInfoManager"></param>
        /// <param name="type"></param>
        /// <param name="dataObjectAttribute"></param>
        /// <param name="changedDb"></param>
        /// <returns></returns>
        DataObjectTypeMetaInfo GetBaseDataObjectType(DataObjectTypeMetaInfoManager dataObjectTypeMetaInfoManager, Type type, DataObjectAttribute dataObjectAttribute, out bool changedDb)
        {
            //try to find base DataObject
            Type baseType = type.BaseType;
            Type objectType=  typeof(object);
            while (null != baseType && objectType != type)
            {
                DataObjectTypeMetaInfo baseMetaInfo = TryCreateFromType(dataObjectTypeMetaInfoManager, baseType, out changedDb);
                if (null != baseMetaInfo)
                {
                    return baseMetaInfo;
                }
                baseType = baseType.BaseType;
            }

            changedDb = false;
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        /// <param name="changedDb"></param>
        /// <returns></returns>
        int EnsureDataObjectTypeInDb(DataLayerManager dataLayerManager, out bool changedDb)
        {
            string typeName = GetTypeName(DescribedType);

            object typeIdObject = dataLayerManager.ExecuteScalarCommand("SELECT [Id] FROM [Sys.DataObjectType] WHERE [TypeName]='{0}'", typeName);

            if (null == typeIdObject) //doesn't exists in db now
            {
                using (SofTransaction t = dataLayerManager.BeginTransaction())
                {
                    typeIdObject = dataLayerManager.ExecuteScalarCommand("INSERT INTO [Sys.DataObjectType] ([TypeName]) VALUES ('{0}'); SELECT last_insert_rowid()", typeName);
                    CreateTypeTables(dataLayerManager);

                    t.Commit();
                }
                changedDb = true;
            }
            else
            {
                changedDb = false;
            }
            return (int)(long)typeIdObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        void CreateTypeTables(DataLayerManager dataLayerManager)
        {
            List<string> columns = new List<string>();
            columns.Add("[Id] INTEGER PRIMARY KEY");  //must be same as a id in Sys.DataObjects
            foreach (IPropertyMetaInfo property in Properties)
            {
                columns.AddRange(property.GetMainTableCreateCommandColumns());
            }
            dataLayerManager.CrateTable(TableName, columns);
            foreach (IPropertyMetaInfo property in Properties)
            {
                property.ExecuteAdditionalCreateTableCommands(dataLayerManager);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string GetTypeName(Type type)
        {
            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }
    }
}
