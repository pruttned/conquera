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
    /// Defines provider for a basic type (complex type that isn't data object - for instance Point, Rect, Vector3, ...).
    /// Custom basic type must be value type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class CustomBasicTypeProviderAttribute : Attribute
    {
        private Type mBasicType;
        private Type mProviderType;

        /// <summary>
        /// Gets the basic type
        /// </summary>
        public Type BasicType
        {
            get { return mBasicType; }
        }

        /// <summary>
        /// Gets the provider for basic type
        /// </summary>
        public Type ProviderType
        {
            get{return mProviderType;}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="basicType"></param>
        /// <param name="providerType">- Must implement ICustomBasicTypeProvider</param>
        public CustomBasicTypeProviderAttribute(Type basicType, Type providerType)
        {
            if (!typeof(ICustomBasicTypeProvider).IsAssignableFrom(providerType))
            {
                throw new ArgumentException(string.Format("Type '{0}' doesn't implement '{1}'", basicType.FullName, typeof(ICustomBasicTypeProvider).FullName));
            }

            mBasicType = basicType;
            mProviderType = providerType;
        }
    }
}
