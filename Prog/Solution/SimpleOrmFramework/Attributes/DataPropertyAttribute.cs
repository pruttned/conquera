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
