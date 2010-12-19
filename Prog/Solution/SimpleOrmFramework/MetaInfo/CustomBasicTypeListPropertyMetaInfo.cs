//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Data.Common;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class CustomBasicTypeListPropertyMetaInfo : ListPropertyMetaInfo
    {
        private ICustomBasicTypeProvider mCustomBasicTypeProvider;
        private SubProperty[] mSubProperties;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="itemType"></param>
        /// <param name="dataObjectListPropertyAttribute"></param>
        /// <param name="customBasicTypeProvider"></param>
        public CustomBasicTypeListPropertyMetaInfo(PropertyInfo propertyInfo, Type itemType, DataListPropertyAttribute dataObjectListPropertyAttribute, ICustomBasicTypeProvider customBasicTypeProvider)
            :base(propertyInfo, itemType, dataObjectListPropertyAttribute)
        {
            if (!ItemType.IsValueType)
            {
                throw new ArgumentException(string.Format("Type '{0}' must be value type", propertyInfo.PropertyType.FullName));
            }

            mCustomBasicTypeProvider = customBasicTypeProvider;

            MemberInfo[] dataProperties = customBasicTypeProvider.GetDataProperties();
            if (0 == dataProperties.Length)
            {
                throw new ArgumentException(string.Format("Zero dataProperties provided from '{0}'", customBasicTypeProvider.GetType().ToString()));
            }

            mSubProperties = new SubProperty[dataProperties.Length];
            for (int i = 0; i < dataProperties.Length; ++i)
            {
                mSubProperties[i] = new SubProperty(dataProperties[i], dataProperties[i].Name);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataLayerManager"></param>
        public override void ExecuteAdditionalCreateTableCommands(DataLayerManager dataLayerManager)
        {
            StringBuilder commandBuilder = new StringBuilder("CREATE TABLE [");
            commandBuilder.Append(ListTable).Append("] ([").Append(ParentColumnName).Append("] LONG NOT NULL");

            foreach (SubProperty subProperty in mSubProperties)
            {
                string dataTypeName = DbDataTypeNames.GetDbType(subProperty.PropertyType);
                commandBuilder.Append(", [").Append(subProperty.Column).Append("] ").Append(dataTypeName);
            }
            commandBuilder.Append("); CREATE INDEX [Index_").Append(ListTable).Append("] ON [").Append(ListTable).Append("] ([").Append(ParentColumnName).Append("])");

            dataLayerManager.ExecuteNonQueryCommand(commandBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ormManager"></param>
        /// <returns></returns>
        public override SofDbCommandParameter[] SaveProperty(IDataObject obj, OrmManager ormManager)
        {
            IList list = (IList)GetPropertyValue(obj);

            if (null != list)
            {
                List<SofDbCommandParameter> dbParams = new List<SofDbCommandParameter>(mSubProperties.Length);

                //create command
                StringBuilder commandBuilder = new StringBuilder(20);
                commandBuilder.Append("INSERT INTO [").Append(ListTable).Append("] ([").Append(ParentColumnName).Append(']');
                foreach (SubProperty subProperty in mSubProperties)
                {
                    dbParams.Add(new SofDbCommandParameter("@" + subProperty.Column, null, subProperty.Column));
                    commandBuilder.Append(",[").Append(subProperty.Column).Append(']');
                }
                commandBuilder.Append(") VALUES (").Append(obj.Id);
                foreach (SubProperty subProperty in mSubProperties)
                {
                    commandBuilder.Append(",@").Append(subProperty.Column);
                }
                commandBuilder.Append(')');
                string command = commandBuilder.ToString();

                //save items
                foreach (object item in list)
                {
                    for (int i = 0; i < mSubProperties.Length; ++i)
                    {
                        dbParams[i].Value = mSubProperties[i].GetValue(item);
                    }

                    ormManager.DataLayerManager.ExecuteNonQueryCommand(command, dbParams);
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
        public override SofDbCommandParameter[] UpdateProperty(IDataObject newObj, IDataObject origObj, OrmManager ormManager)
        {
            CleareList(ormManager.DataLayerManager, newObj.Id);
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
            IList list = (IList)ListDefaultConstructor.Invoke(null);

            using (DbDataReader dbReader2 = ormManager.DataLayerManager.ExecuteReaderCommand("SELECT * FROM [{0}] WHERE [{2}]={1}", ListTable, newObject.Id.ToString(), ParentColumnName))
            {
                while (dbReader2.Read())
                {
                    object item = mCustomBasicTypeProvider.CreateInstance();

                    foreach (SubProperty subProperty in mSubProperties)
                    {
                        object subPropertyValue = dbReader2[subProperty.Column];
                        subPropertyValue = (subPropertyValue is DBNull ? null : subPropertyValue);
                        subProperty.SetValue(item, subPropertyValue);
                    }

                    list.Add(item);
                }
            }

            SetPropertyValue(newObject, list);
        }

        //no need to override - items are value types
        //public override void InitializeProperty(IDataObject newObject, IDataObject srcObject, OrmManager ormManager, Dictionary<long, IDataObject> loadCache)
        //{
        //}
    }
}
