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

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class PropertyMetaInfoCollection : ReadOnlyCollection<IPropertyMetaInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public PropertyMetaInfoCollection(IList<IPropertyMetaInfo> list)
            :base(list)
        {}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="columnName"></param>
        ///// <returns></returns>
        //public IPropertyMetaInfo GetByColumnName(string columnName)
        //{
        //    foreach (IPropertyMetaInfo property in this)
        //    {
        //        if (property.ColumnName == columnName)
        //        {
        //            return property;
        //        }
        //    }

        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public IPropertyMetaInfo GetByPropertyName(string propertyName)
        {
            foreach (IPropertyMetaInfo property in this)
            {
                if (property.PropertyName == propertyName)
                {
                    return property;
                }
            }
            return null;
        }
    }
}
