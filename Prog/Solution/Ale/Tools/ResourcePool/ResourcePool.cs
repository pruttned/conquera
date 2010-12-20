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

namespace Ale.Tools
{
    /// <summary>
    /// Resource pool
    /// </summary>
    public abstract class ResourcePool : IDisposable
    {
        #region Fields
        private Stack<PoolableResource> mUnusedResources = new Stack<PoolableResource>();
        private bool mIsDisposed = false;
        #endregion Fields

        #region Methods

        /// <summary>
        /// Returns a new or reused resource
        /// </summary>
        /// <returns></returns>
        public PoolableResource Alloc()
        {
            PoolableResource resoruce;
            if (0 == mUnusedResources.Count)
            {
                resoruce = CreateNewResource();
                resoruce.Init(false);
            }
            else
            {
                resoruce = mUnusedResources.Pop();
                resoruce.Init(true);
            }

            return resoruce;
        }

        /// <summary>
        /// Called whenever is an used resource disposed
        /// </summary>
        /// <param name="resource"></param>
        internal void ReturnToPool(PoolableResource resource)
        {
            if (OnResourceReturningToPool(resource))
            {
                mUnusedResources.Push(resource);
            }
            else
            {
                resource.RealDispose();
            }
        }

        /// <summary>
        /// Creates a new resource
        /// </summary>
        /// <returns></returns>
        protected abstract PoolableResource CreateNewResource();

        /// <summary>
        /// Called whenever is an used resource going to be returned to the pool
        /// </summary>
        /// <param name="poolableResource"></param>
        /// <returns>- Whether should be resource returned to the pool or it is in invalid state and can't be reused</returns>
        protected abstract bool OnResourceReturningToPool(PoolableResource poolableResource);

        #endregion Methods

        #region IDisposable Members

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    //dispose all resources
                    while (0 != mUnusedResources.Count)
                    {
                        mUnusedResources.Pop().RealDispose();
                    }
                }
                mIsDisposed = true;
            }
        }

        #endregion
    }
}
