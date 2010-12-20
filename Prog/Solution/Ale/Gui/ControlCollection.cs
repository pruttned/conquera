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
using System.Linq;
using System.Text;
using System.Collections;

namespace Ale.Gui
{
    public class ControlCollection : ListBase<Control>
    {
        public Control Owner { get; private set; }

        public ControlCollection(Control owner)
        {
            Owner = owner;
        }

        public override void Add(Control item)
        {
            CheckControlInNoCollection(item);
            base.Add(item);
            InitializeControl(item);
        }

        public override void Insert(int index, Control item)
        {
            CheckControlInNoCollection(item);
            base.Insert(index, item);
            InitializeControl(item);
        }

        public override bool Remove(Control item)
        {
            CheckControlInThisCollection(item);
            base.Remove(item);
            ClearControl(item);
            return true;
        }

        public override void RemoveAt(int index)
        {
            Control item = this[index];
            base.RemoveAt(index);
            ClearControl(item);
        }

        public override void Clear()
        {
            IEnumerable items = this;
            base.Clear();

            foreach (Control item in items)
            {
                ClearControl(item);
            }
        }

        private void CheckControlInNoCollection(Control control)
        {
            if (control.SiblingColleciton != null)
            {
                throw new ArgumentException("The control is already in a sibling collection.");
            }
        }

        private void CheckControlInThisCollection(Control control)
        {
            if (control.SiblingColleciton != this)
            {
                throw new ArgumentException("The control does not belong to this sibling collection.");
            }
        }

        private void InitializeControl(Control control)
        {
            control.SiblingColleciton = this;
        }

        private void ClearControl(Control control)
        {
            control.SiblingColleciton = null;
        }
    }
}
