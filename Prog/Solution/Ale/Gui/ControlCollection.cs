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
