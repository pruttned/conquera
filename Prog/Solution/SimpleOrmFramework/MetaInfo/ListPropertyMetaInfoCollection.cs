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
using System.Collections.ObjectModel;

namespace SimpleOrmFramework
{
    /// <summary>
    /// 
    /// </summary>
    class ListPropertyMetaInfoCollection : ReadOnlyCollection<ListPropertyMetaInfo>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        public ListPropertyMetaInfoCollection(IList<ListPropertyMetaInfo> list)
            :base(list)
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="listTableName"></param>
        /// <returns></returns>
        public ListPropertyMetaInfo GetByListTableName(string listTableName)
        {
            foreach (ListPropertyMetaInfo listProperty in this)
            {
                if (listProperty.ListTable == listTableName)
                {
                    return listProperty;
                }
            }

            throw new ArgumentException(String.Format("List property with binding table '{0}' doesn't exists", listTableName));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public ListPropertyMetaInfo GetByPropertyName(string propertyName)
        {
            foreach (ListPropertyMetaInfo listProperty in this)
            {
                if (listProperty.PropertyInfo.Name == propertyName)
                {
                    return listProperty;
                }
            }

            throw new ArgumentException(String.Format("List property with name '{0}' doesn't exists", propertyName));
        }
    }
}
