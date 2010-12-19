using System;
using System.Collections.Generic;
using System.Text;

namespace Ale
{
    public class CompositeFrameListener : IFrameListener
    {
        private List<IFrameListener> mFrameListeners;

        /// <summary>
        /// Adds a new frame listener
        /// </summary>
        /// <param name="frameListener"></param>
        protected void RegisterFrameListener(IFrameListener frameListener)
        {
            if (null == frameListener) { throw new ArgumentNullException("frameListener"); }

            if (null == mFrameListeners)
            {
                mFrameListeners = new List<IFrameListener>();
            }
            mFrameListeners.Add(frameListener);
        }

        /// <summary>
        /// Adds a new frame listener
        /// </summary>
        /// <param name="frameListener"></param>
        /// <returns>Whether was listener removed</returns>
        protected bool RemoveFrameListener(IFrameListener frameListener)
        {
            if (null != mFrameListeners)
            {
                return mFrameListeners.Remove(frameListener);
            }
            return false;
        }
        
        #region IFrameListener

        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            if (null != mFrameListeners)
            {
                foreach (IFrameListener listener in mFrameListeners)
                {
                    listener.BeforeUpdate(gameTime);
                }
            }
        }

        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
            if (null != mFrameListeners)
            {
                foreach (IFrameListener listener in mFrameListeners)
                {
                    listener.AfterUpdate(gameTime);
                }
            }
        }

        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
            if (null != mFrameListeners)
            {
                foreach (IFrameListener listener in mFrameListeners)
                {
                    listener.BeforeRender(gameTime);
                }
            }
        }

        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
            if (null != mFrameListeners)
            {
                foreach (IFrameListener listener in mFrameListeners)
                {
                    listener.AfterRender(gameTime);
                }
            }
        }

        #endregion IFrameListener
    }
}
