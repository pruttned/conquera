using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// Marks the property that should be serialized in the db.
    /// It can be placed above simple types (including string, Guid, DateTime and Enum) and also above other data objects
    /// </summary>    
    [AttributeUsage(AttributeTargets.Property)]
    public class DataPropertyAttribute : Attribute, IDataPropertyAttribute
    {
        private string mColumn = null;
        private bool mUnique = false;
        private bool mNotNull = false;
        private bool mWeakReference = false;
        private bool mCaseSensitive = false;

        /// <summary>
        /// Gets/sets the column name for this property.
        /// Default = property name
        /// </summary>
        public string Column
        {
            get { return mColumn; }
            set { mColumn = value; }
        }

        /// <summary>
        /// Whether this property must be unique (UNIQUE constraint in db).
        /// Ignored if type is custom basic type.
        /// Defaul = false.
        /// </summary>
        public bool Unique
        {
            get { return mUnique; }
            set { mUnique = value; }
        }

        /// <summary>
        /// Whether this property can't be null (NOT NULL constraint in db).
        /// Ignored if type is custom basic type.
        /// Defaul = false.
        /// </summary>
        public bool NotNull
        {
            get { return mNotNull; }
            set { mNotNull = value; }
        }

        /// <summary>
        /// Valid only if property type is data object.
        /// If property is weak reference, then the underlying data object is not updated in db in case of save and remove. 
        /// Data object stored in weak referenced property must have valid id (it must be loaded from db)
        /// Default = false.
        /// </summary>
        public bool WeakReference
        {
            get { return mWeakReference; }
            set { mWeakReference = value; }
        }

        /// <summary>
        /// Valid only if property type is string
        /// Whether should be this column case sensitive
        /// Default = false.
        /// </summary>
        public bool CaseSensitive
        {
            get { return mCaseSensitive; }
            set { mCaseSensitive = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IPropertyMetaInfo IDataPropertyAttribute.CreatePropertyMetaInfo(PropertyInfo propertyInfo, Dictionary<Type, ICustomBasicTypeProvider> customBasicTypeProviders)
        {
            //is custom basic type
            ICustomBasicTypeProvider customBasicTypeProvider;
            if (customBasicTypeProviders.TryGetValue(propertyInfo.PropertyType, out customBasicTypeProvider))
            {
                return new CustomBasicTypePropertyMetaInfo(propertyInfo, this, customBasicTypeProvider);
            }
            else
            {
                return new PropertyMetaInfo(propertyInfo, this);
            }
        }
    }
}
