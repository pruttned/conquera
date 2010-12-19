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

namespace Ale.Settings
{
    public class IniFileNodeColection<T> : IEnumerable<T> where T : IniFileNode
    {
        Dictionary<string, T> mChildNodesByName = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        private List<T> mChildNodes = new List<T>();

        public T this[int index]
        {
            get { return mChildNodes[index]; }
        }

        public T this[string name]
        {
            get { return mChildNodesByName[name]; }
        }

        public int Count
        {
            get { return mChildNodes.Count; }
        }

        public void Add(T item)
        {
            if (null == item) throw new ArgumentNullException("item");

            mChildNodesByName.Add(item.Name, item);
            mChildNodes.Add(item);
        }

        public void Remove(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            if (mChildNodesByName.Remove(name))
            {
                int index = FindItemIndex(name);
                if (-1 < index)
                {
                    mChildNodes.RemoveAt(index);
                }
            }
        }

        public bool TryGetNode(string name, out T node)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return mChildNodesByName.TryGetValue(name, out node);
        }

        public void Clear()
        {
            mChildNodes.Clear();
            mChildNodesByName.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return mChildNodes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mChildNodes.GetEnumerator();
        }

        private int FindItemIndex(string name)
        {
            for (int i = 0; i < mChildNodes.Count; ++i)
            {
                if (string.Equals(name, mChildNodes[i].Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

    }

}
