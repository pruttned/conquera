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