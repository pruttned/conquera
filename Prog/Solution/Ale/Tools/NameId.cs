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

namespace Ale.Tools
{
    /// <summary>
    /// Provides global name-id mapping.
    /// All names are case insensitive
    /// </summary>
    public class NameId : IEquatable<NameId>, IEquatable<string>, IEquatable<int>
    {
        private static Dictionary<string, NameId> mNameIdsByName = new Dictionary<string, NameId>(StringComparer.InvariantCultureIgnoreCase);
        private static Dictionary<int, NameId> mNameIdsById = new Dictionary<int, NameId>();
        private static int mNextId = 1;

        private string mName;
        private int mId;
        private int mHashCode;

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Id
        /// </summary>
        public int Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Gets an existing NameId or creates a new one
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NameId FromName(string name)
        {
            NameId nameId;
            if (!mNameIdsByName.TryGetValue(name, out nameId))
            {
                nameId = new NameId(name, mNextId++);
                mNameIdsByName.Add(name, nameId);
                mNameIdsById.Add(nameId.Id, nameId);
            }
            return nameId;
        }

        /// <summary>
        /// Gets an existing NameId by its name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">- Specified NameId doesn't exists</exception>
        public static NameId GetByName(string name)
        {
            NameId nameId;
            if (!mNameIdsByName.TryGetValue(name, out nameId))
            {
                throw new ArgumentException(string.Format("NameId with name '{0}' doesn't exists", name));
            }
            return nameId;
        }

        /// <summary>
        /// Gets an existing NameId by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">- Specified NameId doesn't exists</exception>
        public static NameId GetById(int id)
        {
            NameId nameId;
            if (!mNameIdsById.TryGetValue(id, out nameId))
            {
                throw new ArgumentException(string.Format("NameId with id '{0}' doesn't exists", id));
            }
            return nameId;
        }

        #region IEquatable<NameId>

        public bool Equals(NameId other)
        {
            if (null == other)
            {
                return false;
            }
            return mId == other.mId;
        }

        public bool Equals(string otherName)
        {
            return string.Equals(mName, otherName, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Equals(int otherId)
        {
            return mId == otherId;
        }

        #endregion IEquatable<NameId>

        #region Equality operators

        public static bool operator == (NameId a, NameId b)
        {
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (null == a || null == b)
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(NameId a, NameId b)
        {
            return !(a == b);
        }

        #endregion Equality operators

        public override int GetHashCode()
        {
            return mHashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return mName;
        }

        //public static implicit operator string(NameId n)
        //{
        //    return n.Name;
        //}

        public static implicit operator NameId(string n)
        {
            return NameId.FromName(n);
        }

        private NameId(string name, int id)
        {
            mHashCode = name.GetHashCode(); //string dosn't cache its hash code internally
            mName = name;
            mId = id;
        }
    }
}
