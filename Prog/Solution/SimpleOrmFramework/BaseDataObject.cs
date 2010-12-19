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
    /// Base implementation of the IDataObject
    /// </summary>
    public class BaseDataObject : IDataObject
    {
        long mId = -1;

        /// <summary>
        /// Gets the Id
        /// </summary>
        public long Id
        {
            get { return mId; }
        }

        /// <summary>
        /// 
        /// </summary>
        long IDataObject.Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ormManager"></param>
        void SimpleOrmFramework.IDataObject.AfterLoad(OrmManager ormManager)
        {
            AfterLoadImpl(ormManager);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ormManager"></param>
        void SimpleOrmFramework.IDataObject.BeforeSave(OrmManager ormManager)
        {
            BeforeSaveImpl(ormManager);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ormManager"></param>
        void SimpleOrmFramework.IDataObject.BeforeDelete(OrmManager ormManager)
        {
            BeforeDeleteImpl(ormManager);
        }

        void IDataObject.AfterSave(OrmManager ormManager, bool isNew)
        {
            AfterSaveImpl(ormManager, isNew);
        }     

        /// <summary>
        /// Executed whenever is data object loaded from the db
        /// </summary>
        /// <param name="ormManager"></param>
        protected virtual void AfterLoadImpl(OrmManager ormManager)
        {
        }

        /// <summary>
        /// Executed before saving the data object to the db
        /// </summary>
        /// <param name="ormManager"></param>
        protected virtual void BeforeSaveImpl(OrmManager ormManager)
        {
        }
        
        /// <summary>
        /// Executed before deleting the data object from the db
        /// </summary>
        /// <param name="ormManager"></param>
        protected virtual void BeforeDeleteImpl(OrmManager ormManager)
        {
        }

        /// <summary>
        /// Executed after saving the data object to the db.
        /// Id is valid at this time.
        /// </summary>
        /// <param name="ormManager"></param>
        /// <param name="isNew">- Whether has been object created or updated</param>
        protected virtual void AfterSaveImpl(OrmManager ormManager, bool isNew)
        {
        }

        #region IDataObject Members

        #endregion
    }
}
