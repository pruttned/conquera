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
