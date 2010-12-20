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
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using System.Reflection;

namespace Ale.Tools
{
    /// <summary>
    /// Takes all public fields of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FieldCustomBasicTypeProvider<T> : ICustomBasicTypeProvider where T :  new()
    {
        #region ICustomBasicTypeProvider Members

        public MemberInfo[] GetDataProperties()
        {
            return Array.ConvertAll<FieldInfo, MemberInfo>(typeof(T).GetFields(), 
                delegate(FieldInfo field)
                {
                    return (MemberInfo)field;
                });
        }

        public object CreateInstance()
        {
            return new T();
        }

        #endregion
    }

    /// <summary>
    /// Takes all public properties of the specified type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyCustomBasicTypeProvider<T> : ICustomBasicTypeProvider where T : new()
    {
        #region ICustomBasicTypeProvider Members

        public MemberInfo[] GetDataProperties()
        {
            return Array.ConvertAll<PropertyInfo, MemberInfo>(typeof(T).GetProperties(),
                delegate(PropertyInfo field)
                {
                    return (MemberInfo)field;
                });
        }

        public object CreateInstance()
        {
            return new T();
        }

        #endregion
    }

}
