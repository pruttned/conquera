using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Dynamically unloads objects
    /// </summary>
    public abstract class DynamicallyLoadableObjectsUnloader : IFrameListener
    {
        #region Fields
        private List<IDynamicallyLoadableObject> mLoadedObjects = new List<IDynamicallyLoadableObject>();
        private float mCheckInterval = 5;
        private float mNextCheckTime = -1;
        #endregion Fields

        #region Methods

        /// <summary>
        /// Vtor
        /// </summary>
        /// <param name="checkInterval">- How often to check objects (interval in sec)</param>
        public DynamicallyLoadableObjectsUnloader(float checkInterval)
        {
            mCheckInterval = checkInterval;
        }

        #region IFrameListener

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// Called after rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
            float totalTime = gameTime.TotalTime;
            long frameNum = gameTime.FrameNum;

            if (totalTime > mNextCheckTime)
            {
                mNextCheckTime = totalTime + mCheckInterval;

                //find which objects can be unloaded
                for (int i = mLoadedObjects.Count - 1; i >= 0; i-- )
                {
                    if (!mLoadedObjects[i].IsLoaded) //already unloaded
                    {
                        mLoadedObjects.RemoveAt(i);
                    }
                    else
                    {
                        if (mLoadedObjects[i].LastRenderFrameNum != frameNum) //not rendered this frame
                        {
                            if (ObjectCanBeUnloaded(mLoadedObjects[i]))
                            {
                                mLoadedObjects[i].Unload();
                                mLoadedObjects.RemoveAt(i);
                            }
                        }
                    }
                }
            }
        }

        #endregion IFrameListener

        /// <summary>
        /// Whether should be an object unloaded
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected abstract bool ObjectCanBeUnloaded(IDynamicallyLoadableObject obj);

        /// <summary>
        /// Register an object
        /// </summary>
        /// <param name="obj"></param>
        public void RegisterObject(IDynamicallyLoadableObject obj)
        {
            obj.AfterLoad += new DynamicallyLoadableObjectAfterLoadHandler(DynamicallyLoadableObject_AfterLoad);
            obj.Disposing += new EventHandler(DynamicallyLoadableObject_Disposing);
        }

        /// <summary>
        /// Unregister an object
        /// </summary>
        /// <param name="obj"></param>
        public void UnregisterObject(IDynamicallyLoadableObject obj)
        {
            //if(mLoadedObjects.IsLoaded) - not sufficient
            mLoadedObjects.Remove(obj);

            obj.AfterLoad -= DynamicallyLoadableObject_AfterLoad;
            obj.Disposing -= DynamicallyLoadableObject_Disposing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dynamicallyLoadableObject"></param>
        private void DynamicallyLoadableObject_AfterLoad(IDynamicallyLoadableObject dynamicallyLoadableObject)
        {
            if (!mLoadedObjects.Contains(dynamicallyLoadableObject))
            {
                mLoadedObjects.Add(dynamicallyLoadableObject);
            }
        }

        private void DynamicallyLoadableObject_Disposing(object sender, EventArgs e)
        {
            UnregisterObject((IDynamicallyLoadableObject)sender);
        }

        #endregion Methods
    }
}
