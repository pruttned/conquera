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
