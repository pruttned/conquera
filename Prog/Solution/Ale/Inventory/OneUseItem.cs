using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ale.Inventory
{
    public class OneUseItem : Item
    {
        #region Fields

        private ItemGroup mItemGroup;

        #endregion Fields
        
        #region Properties

        public ItemGroup ItemGroup
        {
            get { return mItemGroup; }
            internal set { mItemGroup = value; }
        }

        #endregion Properties

        #region Constructors

        public OneUseItem(string name, ItemTypes itemType)
            : base(name, itemType)
        {
        }

        #endregion Constructors

        #region Methods

        public override void Use()
        {
            base.Use();

        }

        #endregion Methods
    }
}
