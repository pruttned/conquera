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
