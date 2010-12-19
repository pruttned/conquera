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
