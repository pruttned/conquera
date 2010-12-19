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
