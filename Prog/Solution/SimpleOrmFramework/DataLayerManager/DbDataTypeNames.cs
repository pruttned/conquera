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

namespace SimpleOrmFramework
{
    /// <summary>
    /// Helpers for converting between system and db types
    /// </summary>
    static class DbDataTypeNames
    {
        private static readonly Dictionary<Type, string> mDbTypes = new Dictionary<Type, string>();
        private static readonly Type mIDataObjectType = typeof(IDataObject);
        private static readonly Type mDataObjectAttributeType = typeof(DataObjectAttribute);
        private static readonly Type mEnumType = typeof(Enum);
        private static readonly string mIdTypeName;
        private static readonly string mEnumTypeName;

        /// <summary>
        /// 
        /// </summary>
        static DbDataTypeNames()
        {
            mDbTypes[typeof(string)] = "STRING";
            mDbTypes[typeof(long)] = mIdTypeName = "LONG";
            mDbTypes[typeof(long?)] = "LONG";
            mDbTypes[typeof(int)] = mEnumTypeName = "INT";
            mDbTypes[typeof(int?)] = "INT";
            mDbTypes[typeof(short)] = "SMALLINT";
            mDbTypes[typeof(short?)] = "SMALLINT";
            mDbTypes[typeof(byte)] = "TINYINT";
            mDbTypes[typeof(byte?)] = "TINYINT";
            mDbTypes[typeof(bool)] = "BIT";
            mDbTypes[typeof(bool?)] = "BIT";
            mDbTypes[typeof(DateTime)] = "DATETIME";
            mDbTypes[typeof(DateTime?)] = "DATETIME";
            mDbTypes[typeof(Guid)] = "GUID";
            mDbTypes[typeof(Guid?)] = "GUID";
            mDbTypes[typeof(double)] = "DOUBLE";
            mDbTypes[typeof(double?)] = "DOUBLE";
            mDbTypes[typeof(float)] = "REAL";
            mDbTypes[typeof(float?)] = "REAL";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDbType(Type type)
        {
            string name;
            if (!mDbTypes.TryGetValue(type, out name))
            {
                //check whether it is DataObject
                if (mIDataObjectType.IsAssignableFrom(type))
                {
                    //Check DataObjectAttribute
                    object[] dataObjectAttributes = type.GetCustomAttributes(mDataObjectAttributeType, false);
                    if (null != dataObjectAttributes && 0 != dataObjectAttributes.Length)
                    {
                        return mIdTypeName;
                    }
                }
                else
                {
                    if (mEnumType.IsAssignableFrom(type))
                    {
                        return mEnumTypeName;
                    }
                }

                throw new UnsupportedTypeException(string.Format("Type '{0}' is not supported. Type must be one the supported basic types or it must implement '{1}' interface and has attribute '{2}'", type.FullName, typeof(IDataObject).FullName, typeof(DataObjectAttribute).FullName));
            }

            return name;
        }
    }
}
