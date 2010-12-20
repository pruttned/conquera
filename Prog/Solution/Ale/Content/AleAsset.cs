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

namespace Ale.Content
{
    /// <summary>
    /// asset
    /// </summary>
    internal class AleAsset
    {
        private object mRealAsset = null;
        private string mName = null;
        private long mId = -1;

        /// <summary>
        /// Number of ContenGroups that references this asset
        /// </summary>
        private int mRefCnt = 1;

        private ReadOnlyCollection<AleAsset> mReadonlyChilds = null;
        private List<AleAsset> mChilds = null;

        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public object RealAsset
        {
            get { return mRealAsset; }
        }

        public ReadOnlyCollection<AleAsset> Childs
        {
            get { return mReadonlyChilds; }
        }

        public long Id
        {
            get { return mId; }
        }

        public AleAsset()
        {
        }

        public void Init(string name, object realAsset, long id)
        {
            if (mName != null)
            {
                throw new InvalidOperationException("Asset has been alredy initialized");
            }
            mName = name;
            mRealAsset = realAsset;
            mId = id;
        }

        public void AddChildAsset(AleAsset asset)
        {
            if (null == mChilds)
            {
                mChilds = new List<AleAsset>();
                mReadonlyChilds = new ReadOnlyCollection<AleAsset>(mChilds);
            }

            mChilds.Add(asset);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int IncRefCnt()
        {
            //if (null != mChilds)
            //{
            //    foreach (AleAsset child in mChilds)
            //    {
            //        child.IncRefCnt();
            //    }
            //}
            return ++mRefCnt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int DecRefCnt()
        {
            //if (null != mChilds)
            //{
            //    foreach (Asset child in mChilds)
            //    {
            //        child.DecRefCnt();
            //    }
            //}

            if (0 != mRefCnt)
            {
                --mRefCnt;
            }
            return mRefCnt;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
