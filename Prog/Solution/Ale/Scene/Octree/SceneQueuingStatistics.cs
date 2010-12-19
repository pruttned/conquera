using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Scene
{
    /// <summary>
    /// Statistics of the filtering process
    /// </summary>
    public struct OctreeFilterStatistics
    {
        #region Fields

        /// <summary>
        /// Number of included objects
        /// </summary>
        private int mIncludedObjectCnt;

        /// <summary>
        /// Number of checked objects
        /// </summary>
        private int mCheckedObjectCnt;

        /// <summary>
        /// Number of checked nodes
        /// </summary>
        private int mCheckedNodeCnt;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of included objects
        /// </summary>
        public int IncludedObjectCnt
        {
            get {return mIncludedObjectCnt;}
        }

        /// <summary>
        /// Gets the number of checked objects
        /// </summary>
        public int CheckedObjectCnt
        {
            get {return mCheckedObjectCnt;}
        }

        /// <summary>
        /// Gets the number of checked nodes
        /// </summary>
        public int CheckedNodeCnt
        {
            get {return mCheckedNodeCnt;}
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        internal void IncIncludedObjectCnt()
        {
            mIncludedObjectCnt++;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void IncCheckedObjectCnt()
        {
            mCheckedObjectCnt++;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void IncCheckedNodeCnt()
        {
            mCheckedNodeCnt++;
        }

        #endregion Methods
    }
}
