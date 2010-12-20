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

namespace SimpleOrmFramework
{
    /// <summary>
    /// Each data object must implement this interface
    /// </summary>
    public interface IDataObject
    {
        /// <summary>
        /// Id. 
        /// Don't set this property manually. I must be lower or equal to 0 for object that were not loaded from db.
        /// </summary>
        long Id
        {
            get;
            set;
        }

        /// <summary>
        /// Executed whenever is data object loaded from the db
        /// </summary>
        /// <param name="ormManager"></param>
        void AfterLoad(OrmManager ormManager);

        /// <summary>
        /// Executed before saving the data object to the db
        /// </summary>
        /// <param name="ormManager"></param>
        void BeforeSave(OrmManager ormManager);

        /// <summary>
        /// Executed after saving the data object to the db.
        /// Id is valid at this time.
        /// </summary>
        /// <param name="ormManager"></param>
        /// <param name="isNew">- Whether has been object created or updated</param>
        void AfterSave(OrmManager ormManager, bool isNew);

        /// <summary>
        /// Executed before deleting the data object from the db
        /// </summary>
        /// <param name="ormManager"></param>
        void BeforeDelete(OrmManager ormManager);
    }
}
