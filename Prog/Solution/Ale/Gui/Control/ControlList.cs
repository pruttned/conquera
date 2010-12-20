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

using System.Collections;
using System.Collections.Generic;

namespace Ale.Gui
{
    /// <summary>
    /// List of controls. Used by CompositeControls to store their child controls.
    /// </summary>
    public class ControlList : ListBase<Control>
    {
        #region Fields

        /// <summary>
        /// CompositeControl that owns this list. It is a parent control of controls stored in this list.
        /// </summary>
        private CompositeControl mOwnerControl;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new ControlList instance.
        /// </summary>
        /// <param name="ownerControl">CompositeControl that owns this list. It is a parent control of controls stored in this list.</param>
        public ControlList(CompositeControl ownerControl)
        {
            mOwnerControl = ownerControl;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Adds the specified control to this list.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public override void Add(Control control)
        {
            control.ParentControl = mOwnerControl;
            base.Add(control);
        }

        /// <summary>
        /// Inserts the specified control to this list at the specified position.
        /// </summary>
        /// <param name="index">Position it this list, where to insert the control.</param>
        /// <param name="control">Control to insert.</param>
        public override void Insert(int index, Control control)
        {         
            control.ParentControl = mOwnerControl;
            base.Insert(index, control);
        }

        /// <summary>
        /// Removes the specified control from this list.
        /// </summary>
        /// <param name="control">Control to remove.</param>
        /// <returns>True if the control is successfully removed, otherwise false. 
        /// This method also returns false if the control was not found in this list.</returns>
        public override bool Remove(Control control)
        {
            bool removed = base.Remove(control);

            if (removed && mOwnerControl == control.ParentControl)
            {
                control.ParentControl = null;
            }

            return removed;
        }

        /// <summary>
        /// Removes a control with the specified index from this list.
        /// </summary>
        /// <param name="index">Index of control to remove.</param>
        public override void RemoveAt(int index)
        {
            Control control = this[index];
            base.RemoveAt(index);

            if (mOwnerControl == control.ParentControl)
            {
                control.ParentControl = null;
            }
        }

        /// <summary>
        /// Removes all controls from this list.
        /// </summary>
        public override void Clear()
        {
            foreach (Control control in this)
            {
                control.ParentControl = null;
            }

            base.Clear();
        }

        #endregion Methods 
    }
}
