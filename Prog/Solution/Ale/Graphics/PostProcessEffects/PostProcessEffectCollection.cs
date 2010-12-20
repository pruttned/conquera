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

namespace Ale.Graphics
{
    /// <summary>
    /// Collection of post-process effects
    /// </summary>
    public class PostProcessEffectCollection : IList<PostProcessEffect>
    {
        #region Fields
        
        /// <summary>
        /// Items
        /// </summary>
        private List<PostProcessEffect> mItems = new List<PostProcessEffect>();

        /// <summary>
        /// Number of currently enabled effects
        /// </summary>
        private int mEnabledEffectCnt = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the number of currently enabled effects
        /// </summary>
        public int EnabledEffectCnt
        {
            get { return mEnabledEffectCnt; }
        }

        #region IList

        /// <summary>
        /// Gets/sets the PostProcessEffect at a specified position
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public PostProcessEffect this[int index]
        {
            get { return mItems[index]; }
            set
            {
                PostProcessEffect oldItem = mItems[index];
                if (value != oldItem)
                {
                    OnItemDeleted(oldItem);
                    OnItemInserted(value);
                    mItems[index] = value;
                }
            }
        }

        #endregion IList
        
        #region ICollection

        /// <summary>
        /// Gets the number of items
        /// </summary>
        public int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Returns false
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion ICollection

        #endregion Properties

        #region Methods

        #region IList
        
        /// <summary>
        /// Gets the index of a specified item or -1
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(PostProcessEffect item)
        {
            return mItems.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item to a specified position
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, PostProcessEffect item)
        {
            mItems.Insert(index, item);
            OnItemInserted(item);
        }

        /// <summary>
        /// Removes an item at a specified position 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            OnItemDeleted(mItems[index]);
            mItems.RemoveAt(index);
        }

        #endregion IList

        #region ICollection

        /// <summary>
        /// Adds a new item to the end of the collection
        /// </summary>
        /// <param name="item"></param>
        public void Add(PostProcessEffect item)
        {
            mItems.Add(item);
            OnItemInserted(item);
        }

        /// <summary>
        /// Removes all items
        /// </summary>
        public void Clear()
        {
            foreach (PostProcessEffect item in mItems)
            {
                OnItemDeleted(item);
            }
            mItems.Clear();
        }

        /// <summary>
        /// Gets whether collection contains a specified item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(PostProcessEffect item)
        {
            return mItems.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(PostProcessEffect[] array, int arrayIndex)
        {
            mItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the specified item from the collection
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(PostProcessEffect item)
        {
            bool removed = mItems.Remove(item);
            if (removed)
            {
                OnItemDeleted(item);
            }
            return removed;
        }

        #endregion ICollection

        #region IEnumerable

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<PostProcessEffect> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        #endregion IEnumerable

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnItemInserted(PostProcessEffect item)
        {
            if (item.Enabled)
            {
                mEnabledEffectCnt++;
            }
            item.EnableChanged += new PostProcessEffect.EnableChangedHandler(item_EnableChanged);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void OnItemDeleted(PostProcessEffect item)
        {
            if (item.Enabled)
            {
                mEnabledEffectCnt--;
            }
            item.EnableChanged -= item_EnableChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="effect"></param>
        void item_EnableChanged(bool enabled, PostProcessEffect effect)
        {
            mEnabledEffectCnt += (enabled ? 1 : -1);
        }

        #endregion Methods
    }
}
