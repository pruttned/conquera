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
