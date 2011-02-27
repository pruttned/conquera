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
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    internal abstract class PoolableBufferManager<T> : IDisposable where T : IPoolableBuffer
    {
        private List<T> mBuffers = new List<T>();
        private bool mIsDisposed = false;
        private bool mRemoveFirst = false;

        public T GetBuffer(int elmCnt, GraphicsDevice graphicsDevice)
        {
            int index = FindBuffer(elmCnt);
            if (index == mBuffers.Count || mBuffers[index].ElmCnt > elmCnt * 3)
            {
                if (elmCnt > 1000)
                {
                    return CreateBuffer(graphicsDevice, elmCnt);
                }
                else
                {
                    return CreateBuffer(graphicsDevice, elmCnt * 2);
                }
            }
            T buffer = mBuffers[index];
            mBuffers.RemoveAt(index);
            return buffer;
        }

        internal void ReturnToPool(T buffer)
        {
            if (!mIsDisposed)
            {
                while (mBuffers.Count > 150) //max number of pooled buffers
                {
                    if (mRemoveFirst)
                    {
                        mBuffers[0].RealDispose();
                        mBuffers.RemoveAt(0);
                    }
                    else
                    {
                        mBuffers[mBuffers.Count - 1].RealDispose();
                        mBuffers.RemoveAt(mBuffers.Count - 1);
                    }

                    mRemoveFirst = !mRemoveFirst;
                }

                mBuffers.Add(buffer);
                mBuffers.Sort();

            }
            else
            {
                buffer.RealDispose();
            }
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                foreach (var buffer in mBuffers)
                {
                    buffer.RealDispose();
                }

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }

        protected abstract T CreateBuffer(GraphicsDevice graphicsDevice, int elmCnt);

        private int FindBuffer(int elmCnt)
        {
            int start = 0;
            int cnt = mBuffers.Count;

            if (10 > cnt)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    int size = mBuffers[i].ElmCnt;
                    if (size == elmCnt)
                    {
                        return i;
                    }
                    else
                    {
                        if (size > elmCnt)
                        {
                            return i;
                        }
                    }
                }
                return cnt;
            }
            else
            {
                //binary search
                int end = cnt;
                while (start <= end)
                {
                    int middle = start + ((end - start) >> 1);

                    if (cnt <= middle)
                    {
                        end = middle - 1;
                    }
                    else
                    {
                        int middleValue = mBuffers[middle].ElmCnt;
                        if (middleValue == elmCnt)
                        {
                            return middle;
                        }
                        if (elmCnt > middleValue)
                        {
                            start = middle + 1;
                        }
                        else
                        {
                            end = middle - 1;
                        }
                    }
                }
            }
            return start;
        }
    }
}
