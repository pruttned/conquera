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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Input;

namespace Ale.Gui
{
    public class Control
    {
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<ControlEventArgs> Click;
        public event EventHandler<ControlEventArgs> MouseLeave;
        public event EventHandler<ControlEventArgs> MouseEnter;
        public event EventHandler<DragDropEventArgs> DragDrop;
        public event EventHandler<DragDropEventArgs> DragFinished;
        public event EventHandler<DragDropEventArgs> DragEnter;
        public event EventHandler<DragDropEventArgs> DragLeave;

        public Control ParentControl
        {
            get { return SiblingColleciton != null ? SiblingColleciton.Owner : null; }
        }

        public ControlCollection ChildControls { get; private set; }
        public ControlCollection SiblingColleciton { get; internal set; }
        public Point Location { get; set; }
        public bool Visible { get; set; }
        public bool IsHitTestEnabled { get; set; }

        public Point ScreenLocation
        {
            get
            {
                if (ParentControl == null)
                {
                    return Location;
                }
                return new Point(ParentControl.ScreenLocation.X + Location.X, ParentControl.ScreenLocation.Y + Location.Y);
            }
        }

        public virtual System.Drawing.SizeF Size
        {
            get { return System.Drawing.SizeF.Empty; }
        }

        public Control()
        {
            ChildControls = new ControlCollection(this);
            Location = Point.Zero;
            Visible = true;
            IsHitTestEnabled = true;
        }

        public void Draw()
        {
            if (Visible)
            {
                OnDrawBackground();
                foreach (Control childControl in ChildControls)
                {
                    childControl.Draw();
                }
                OnDrawForeground();
            }
        }

        public Control GetControl(Point screenLocation)
        {
            if (Visible && IsHitTestEnabled && OccupiesScreenLocation(screenLocation))
            {
                for (int i = ChildControls.Count - 1; i >= 0; i--)
                {
                    Control control = ChildControls[i].GetControl(screenLocation);
                    if (null != control)
                    {
                        return control;
                    }
                }
                return this;
            }
            return null;
        }

        public bool OccupiesScreenLocation(Point screenLocation)
        {
            return screenLocation.X >= ScreenLocation.X && screenLocation.Y >= ScreenLocation.Y && screenLocation.X <= ScreenLocation.X + Size.Width &&
                screenLocation.Y <= ScreenLocation.Y + Size.Height;
        }

        public bool ContainsRecursive(Control control)
        {
            if (control == this)
            {
                return true;
            }

            foreach (Control childControl in ChildControls)
            {
                if (childControl.ContainsRecursive(control))
                {
                    return true;
                }
            }
            return false;
        }

        public Point PointToClient(Vector2 screenLocation)
        {
            return new Point((int)screenLocation.X - ScreenLocation.X, (int)screenLocation.Y - ScreenLocation.Y);
        }

        protected internal virtual void OnMouseDown(MouseButton button, MouseManager mouseManager)
        {
            if (null != MouseDown)
            {
                MouseDown(this, new MouseEventArgs(this, PointToClient(mouseManager.CursorPosition), button));
            }
        }

        protected internal virtual void OnMouseUp(MouseButton button, MouseManager mouseManager)
        {
            if (null != MouseUp)
            {
                MouseUp(this, new MouseEventArgs(this, PointToClient(mouseManager.CursorPosition), button));
            }
        }

        protected internal virtual void OnClick()
        {
            if (null != Click)
            {
                Click(this, new ControlEventArgs(this));
            }
        }

        protected internal virtual void OnMouseLeave()
        {
            if (null != MouseLeave)
            {
                MouseLeave(this, new ControlEventArgs(this));
            }
        }

        protected internal virtual void OnMouseEnter()
        {
            if (null != MouseEnter)
            {
                MouseEnter(this, new ControlEventArgs(this));
            }
        }

        protected internal virtual void OnDragDrop()
        {
            if (null != DragDrop)
            {
                GuiManager.Instance.DragDropInfo.UpdateEventArgs(this);
                DragDrop(this, GuiManager.Instance.DragDropInfo.EventArgs);
            }
        }

        protected internal virtual void OnDragFinished()
        {
            if (null != DragFinished)
            {
                GuiManager.Instance.DragDropInfo.UpdateEventArgs(this);
                DragFinished(this, GuiManager.Instance.DragDropInfo.EventArgs);
            }
        }

        protected internal virtual void OnDragEnter()
        {
            if (null != DragEnter)
            {
                GuiManager.Instance.DragDropInfo.UpdateEventArgs(this);
                DragEnter(this, GuiManager.Instance.DragDropInfo.EventArgs);
            }
        }

        protected internal virtual void OnDragLeave()
        {
            if (null != DragLeave)
            {
                GuiManager.Instance.DragDropInfo.UpdateEventArgs(this);
                GuiManager.Instance.DragDropInfo.EventArgs.AllowDrop = false;
                DragLeave(this, GuiManager.Instance.DragDropInfo.EventArgs);
            }
        }

        protected virtual void OnDrawBackground() { }
        protected virtual void OnDrawForeground() { }
    }
}
