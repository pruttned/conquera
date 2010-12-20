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
using System.Reflection;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class SubProperty
    {
        private MemberInfo mFieldProperty;
        private string mColumn;
        private Type mPropertyType;

        /// <summary>
        /// 
        /// </summary>
        public string Column
        {
            get { return mColumn; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return mFieldProperty.Name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type PropertyType
        {
            get { return mPropertyType; }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="column"></param>
        public SubProperty(MemberInfo fieldProperty, string column)
        {
            if (!(fieldProperty is PropertyInfo) && !(fieldProperty is FieldInfo))
            {
                throw new ArgumentException("fieldProperty must be PropertyInfo or FieldInfo");
            }

            mFieldProperty = fieldProperty;
            mColumn = column;

            if (mFieldProperty is PropertyInfo)
            {
                mPropertyType = ((PropertyInfo)mFieldProperty).PropertyType;
            }
            else
            {
                mPropertyType = ((FieldInfo)mFieldProperty).FieldType;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public object GetValue(object obj)
        {
            if (mFieldProperty is PropertyInfo)
            {
                return ((PropertyInfo)mFieldProperty).GetValue(obj, null);
            }
            return ((FieldInfo)mFieldProperty).GetValue(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public void SetValue(object obj, object value)
        {
            if (mFieldProperty is PropertyInfo)
            {
                ((PropertyInfo)mFieldProperty).SetValue(obj, value, null);
            }
            else
            {
                ((FieldInfo)mFieldProperty).SetValue(obj, value);
            }
        }
    }
}
