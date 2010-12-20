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
