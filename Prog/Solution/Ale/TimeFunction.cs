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
using Microsoft.Xna.Framework;
using System.Collections;

namespace Ale
{
    public class TimeFunction :  IList<Vector2>, IList
    {
        #region Types
        class Vector2Comparer : IComparer<Vector2>
        {
            Comparer<float> mFloatComparer;

            public Vector2Comparer()
            {
                mFloatComparer = Comparer<float>.Default;
            }

            public int Compare(Vector2 x, Vector2 y)
            {
                return mFloatComparer.Compare(x.X, y.Y);
            }
        }
        #endregion Types


        static Vector2Comparer mVector2Comparer = new Vector2Comparer();
        List<Vector2> mPoints;

        public float this[float time]
        {
            get { return GetValueInTime(time); }
        }

        /// <summary>
        /// Creates function from points. 0-time and 1-time points are added if neceseary.
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">-X values are not sorted from min to max</exception>
        /// <exception cref="ArgumentException">-points list is empty</exception>
        /// <exception cref="ArgumentException">-Some X value is lower then 0 or greater then 1</exception>
        public TimeFunction(IList<Vector2> points)
        {
            mPoints = new List<Vector2>(points.Count);
            RecreateFrom(points);
        }

        /// <summary>
        /// Creates function from points. 0-time and 1-time points are added if neceseary.
        /// </summary>
        /// <param name="points"></param>
        /// <exception cref="ArgumentException">-X values are not sorted from min to max</exception>
        /// <exception cref="ArgumentException">-points list is empty</exception>
        /// <exception cref="ArgumentException">-Some X value is lower then 0 or greater then 1</exception>
        public TimeFunction(params Vector2[] points)
        {
            mPoints = new List<Vector2>(points.Length);
            RecreateFrom(points);
        }

        /// <summary>
        /// Creates function from points [0, y1]-[1,y2]
        /// </summary>
        /// <param name="y1">-Y for X=0</param>
        /// <param name="y2">-Y for X=1</param>
        public TimeFunction(float y1, float y2)
            : this(new Vector2[]{new Vector2(0, y1), new Vector2(1, y2)})
        {
        }

        /// <summary>
        /// Creates function from points [0, y]-[1,y]
        /// </summary>
        /// <param name="y"></param>
        public TimeFunction(float y)
            : this(new Vector2[] { new Vector2(0, y), new Vector2(1, y) })
        {
        }

        public void RecreateFrom(IList<Vector2> points)
        {
            mPoints.Clear();
            mPoints.Capacity = Math.Max(points.Count, mPoints.Capacity);

            if (0 == points.Count)
            {
                throw new ArgumentException("No points where provided");
            }
            else
            {
                //add 0-time point
                Vector2 firstPoint = points[0];
                if (firstPoint.X != 0)
                {
                    mPoints.Add(new Vector2(0, firstPoint.Y));
                }

                float lastTime = 0;
                foreach (Vector2 point in points)
                {
                    if (point.X < lastTime || point.X > 1.0)
                    {
                        throw new ArgumentException("X values are no sorted correctly or in 0-1 range");
                    }

                    lastTime = point.X;

                    mPoints.Add(point);
                }

                //add 1-time point
                Vector2 lastPoint = points[points.Count - 1];
                if (lastPoint.X != 1)
                {
                    mPoints.Add(new Vector2(1, lastPoint.Y));
                }
            }
        }

        public float GetValueInTime(float time)
        {
            if(0 >= time)
            {
                return mPoints[0].Y;
            }

            int lastPointIndex = mPoints.Count -1;

            if(1 <= time)
            {
                return mPoints[lastPointIndex].Y;
            }

            int timeIndex = FindIndexForTime(time);
            if (0 == timeIndex)
            {
                return mPoints[0].Y;
            }

            Vector2 point1 = mPoints[timeIndex-1];
            Vector2 point2 = mPoints[timeIndex];

            float xSegmentSize = point2.X - point1.X;
            if(0 == xSegmentSize)
            {
                return point1.Y;
            }
            return MathHelper.Lerp(point1.Y, point2.Y, (time - point1.X) / xSegmentSize);
        }

        public List<Vector2> ToVector2List()
        {
            return new List<Vector2>(mPoints);
        }

        public float GetMax()
        {
            float max = mPoints[0].Y;
            foreach (Vector2 point in mPoints)
            {
                if (point.Y > max)
                {
                    max = point.Y;
                }
            }

            return max;
        }

        public float GetMin()
        {
            float min = mPoints[0].Y;
            foreach (Vector2 point in mPoints)
            {
                if (point.Y < min)
                {
                    min = point.Y;
                }
            }

            return min;
        }


        /// <summary>
        /// For orm
        /// </summary>
        private TimeFunction()
        {
            mPoints = new List<Vector2>();
        }

        private int FindIndexForTime(float time)
        {
            int start = 0;
            int cnt = mPoints.Count;

            if (10 > cnt)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    float value = mPoints[i].X;
                    if (value == time)
                    {
                        return i;
                    }
                    else
                    {
                        if (value > time)
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
                        float middleValue = mPoints[middle].X;
                        if (middleValue == time)
                        {
                            return middle;
                        }
                        if (time > middleValue)
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

        #region IEnumerable

        public IEnumerator<Vector2> GetEnumerator()
        {
            return mPoints.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)mPoints).GetEnumerator();
        }

        #endregion IEnumerable


        #region ICollection<Vector2> Members

        public void Add(Vector2 item)
        {
            int index = FindIndexForTime(item.X);
            mPoints.Insert(index, item);
        }

        public void Clear()
        {
            mPoints = new List<Vector2>(2);
            mPoints.Add(new Vector2(0, 1));
            mPoints.Add(new Vector2(1, 1));
        }

        public bool Contains(Vector2 item)
        {
            return mPoints.Contains(item);
        }

        public void CopyTo(Vector2[] array, int arrayIndex)
        {
            mPoints.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return mPoints.Count; }
        }

        bool ICollection<Vector2>.IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Vector2 item)
        {
            int index = mPoints.IndexOf(item);
            if (-1 < index)
            {
               ((IList<Vector2>)this).RemoveAt(index);

               return true;
            }
            return false;
        }

        #endregion

        #region IList<Vector2> Members

        public int IndexOf(Vector2 item)
        {
            return mPoints.IndexOf((Vector2)item);
        }

        public void Insert(int index, Vector2 item)
        {
            if (0 == index && 0 != item.X)
            {
                throw new ArgumentException("Time (X-value) of the first point must be 0");
            }
            if (mPoints.Count - 1 == index && 1 != item.X)
            {
                throw new ArgumentException("Time (X-value) of the last point must be 1");
            }

            if ((0 < index && mPoints[index - 1].X > item.X) || (mPoints[index].X < item.X))
            {
                throw new ArgumentException("Cannot insert point because it will result in time incosistency");
            }
        }

        void IList<Vector2>.RemoveAt(int index)
        {
            if (0 == index || mPoints.Count - 1 == index)
            {
                throw new ArgumentException("First and last point can't be removed");
            }

            mPoints.RemoveAt(index);
        }

        public Vector2 this[int index]
        {
            get
            {
                return mPoints[index];
            }
            set
            {
                if (0 == index && 0 != value.X)
                {
                    throw new ArgumentException("Time (X-value) of the first point must be 0");
                }
                if (mPoints.Count -1 == index && 1 != value.X)
                {
                    throw new ArgumentException("Time (X-value) of the last point must be 1");
                }

                if ((0 < index && mPoints[index - 1].X > value.X) || (mPoints.Count - 1 > index && mPoints[index + 1].X < value.X))
                {
                    throw new ArgumentException("Cannot insert point because it will result in time incosistency");
                }
            }
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            Add((Vector2)value);
            return mPoints.Count - 1;
        }

        void IList.Clear()
        {
            ((ICollection<Vector2>)this).Clear();
        }

        bool IList.Contains(object value)
        {
            return ((ICollection<Vector2>)this).Contains((Vector2)value);
        }

        int IList.IndexOf(object value)
        {
            return mPoints.IndexOf((Vector2)value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList<Vector2>)this).Insert(index, (Vector2)value);
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        void IList.Remove(object value)
        {
            ((ICollection<Vector2>)this).Remove((Vector2)value);
        }

        void IList.RemoveAt(int index)
        {
            ((IList<Vector2>)this).RemoveAt(index);
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList<Vector2>)this)[index];
            }
            set
            {
                ((IList<Vector2>)this)[index] = (Vector2)value;
            }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)mPoints).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return mPoints.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

 


    }
}
