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
    static class AttributeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T FindAttribute<T>(MemberInfo type, bool inherit)
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), inherit);
            if (null == attributes || 0 == attributes.Length)
            {
                return default(T);
            }

            return (T)attributes[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T FindAttribute<T>(MemberInfo type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(T), false);
            if (null == attributes || 0 == attributes.Length)
            {
                return default(T);
            }

            return (T)attributes[0];
        }
    }

}
