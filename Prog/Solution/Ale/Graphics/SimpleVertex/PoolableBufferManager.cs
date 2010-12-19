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
