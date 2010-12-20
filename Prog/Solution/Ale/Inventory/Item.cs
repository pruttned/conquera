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

namespace Ale.Inventory
{
    public class Item
    {
        #region Fields

        private string mName;
        private ItemTypes mItemType;
        private string mBaseDescription = string.Empty;
        private int mPrice = 0;
        private bool mIsQuestItem = false;

        #endregion Fields

        #region Properties

        public string Name
        {
            get { return mName; }
        }

        public ItemTypes ItemType
        {
            get { return mItemType; }
        }

        public string BaseDescription
        {
            get { return mBaseDescription; }
        }

        public int Price
        {
            get { return mPrice; }
        }

        public bool IsQuestItem
        {
            get { return mIsQuestItem; }
        }

        #endregion Properties

        #region Constructors

        public Item(string name, ItemTypes itemType)
        {
            mName = name;
            mItemType = itemType;
        }

        #endregion Constructors

        #region Methods

        public virtual string GetDescription()
        {
            return BaseDescription;
        }

        public virtual void Use()
        {
            OnUsed();
        }

        protected virtual void OnUsed()
        {
        }

        #endregion Methods
    }
}
