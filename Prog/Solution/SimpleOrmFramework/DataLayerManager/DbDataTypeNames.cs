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
