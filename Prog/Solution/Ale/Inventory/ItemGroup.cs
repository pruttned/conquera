using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ale.Inventory
{
    public class ItemGroup
    {
        #region Fields

        private Item mItem;
        private uint mCount;

        #endregion Fields
        
        #region Properties
        
        public Item Item
        {
            get { return mItem; }
        }

        public uint Count
        {
            get { return mCount; }
            set { mCount = value; }
        }

        #endregion Properties

        #region Constructors

        public ItemGroup(Item item, uint count)
        {
            mItem = item;
            mItem.
            Count = count;
        }

        #endregion Constructors
    }
}