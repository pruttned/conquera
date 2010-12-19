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
    /// Base class for resource that can be reused trough pooling. 
    /// Resource is returned to the pool whenever is called its Dispose method.
    /// </summary>
    public abstract class PoolableResource : IDisposable
    {
        #region Fields
        private ResourcePool mResourcePool;
        private bool mIsInPool = false;
        #endregion Fields

        #region Properties

        /// <summary>
        /// Pool that has created this resource
        /// </summary>
        protected ResourcePool ResourcePool
        {
            get { return mResourcePool; }
        }

        /// <summary>
        /// Whether is resource in the pool (has been disposed)
        /// </summary>
        protected bool IsInPool
        {
            get { return mIsInPool; }
        }

        #endregion Properties

        #region Methods

        #region IDisposable

        /// <summary>
        /// Returns resource to the pool
        /// </summary>
        public void Dispose()
        {
            if (IsInPool)
            {
                throw new ObjectDisposedException("Resource has been already disposed");
            }
            mResourcePool.ReturnToPool(this);
        }

        #endregion IDisposable

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="resourcePool">- Pool that has created this resource</param>
        protected PoolableResource(ResourcePool resourcePool)
        {
            mResourcePool = resourcePool;
        }

        /// <summary>
        /// Inits the resource after it has been allocated from the pool
        /// </summary>
        /// <param name="reusing"></param>
        internal void Init(bool reusing)
        {
            mIsInPool = false;
            OnInit(reusing);
        }

        /// <summary>
        /// Inits the resource after it has been allocated from the pool
        /// </summary>
        /// <param name="reusing">- Whether was resource reused from the pool or it is a new resource</param>
        internal protected abstract void OnInit(bool reusing);

        /// <summary>
        /// Dispose the resource.
        /// IDisposable.Dispose doesn't really dispose the resource but it just returns it to the pool 
        /// </summary>
        internal protected abstract void RealDispose();

        #endregion Methods
    }
}
