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

namespace SimpleOrmFramework
{
    /// <summary>
    /// Marks class that can be serialized to the db
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataObjectAttribute : Attribute
    {
        private string mTable;
        private int mMaxCachedCnt = -1;

        /// <summary>
        /// Gets/sets the name of the table that holds instances of this class.
        /// Default value is full name of the type
        /// </summary>
        public string Table
        {
            get { return mTable; }
            set { mTable = value; }
        }

        /// <summary>
        /// Gets/sets the maximum number of objects of this type stored in cache. -1 = unlimited.
        /// Default = -1.
        /// </summary>
        public int MaxCachedCnt
        {
            get { return mMaxCachedCnt; }
            set { mMaxCachedCnt = value; }
        }
    }
}
