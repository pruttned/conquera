using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    internal struct ParticlePosEnumerable : IEnumerable<Vector3>
    {
        struct PosEnumerator : IEnumerator<Vector3>
        {
            Particle[] mParticles;
            int mIndex;
            int mParticleCntToIterate;
            int mActiveParticleCnt;

            #region IEnumerator<Vector3> Members

            public Vector3 Current
            {
                get { return mParticles[mIndex].Position; }
            }

            #endregion

            internal PosEnumerator(Particle[] particles, int activeParticleCnt)
            {
                mParticles = particles;
                mIndex = -1;
                mParticleCntToIterate = mActiveParticleCnt = activeParticleCnt;
            }

            #region IDisposable

            public void Dispose()
            {
            }

            #endregion IDisposable

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return mParticles[mIndex].Position; }
            }

            public bool MoveNext()
            {
                if (0 == mParticleCntToIterate)
                {
                    return false;
                }

                do
                {
                    mIndex++;
                    if (mParticles.Length <= mIndex)
                    {
                        return false;
                    }
                } while (null == mParticles[mIndex] || !mParticles[mIndex].IsAlive);

                mParticleCntToIterate--;

                return true;
            }

            public void Reset()
            {
                mIndex = -1;
                mParticleCntToIterate = mActiveParticleCnt;
            }

            #endregion
        }

        Particle[] mParticles;
        int mActiveParticleCnt;

        public ParticlePosEnumerable(Particle[] particles, int activeParticleCnt)
        {
            mParticles = particles;
            mActiveParticleCnt = activeParticleCnt;
        }

        #region IEnumerable<Vector3> Members

        public IEnumerator<Vector3> GetEnumerator()
        {
            return new PosEnumerator(mParticles, mActiveParticleCnt);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Vector3>)this).GetEnumerator();
        }

        #endregion
    }
}
