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
using System.Linq;
using System.Text;
using Ale.Input;
using Microsoft.Xna.Framework;
using Ale.Gui;

namespace Ale.Gui
{
    public class ControlEventArgs : EventArgs
    {
        public Control Control { get; private set; }

        public ControlEventArgs(Control control)
        {
            Control = control;
        }
    }

    public class MouseEventArgs : ControlEventArgs
    {
        public Point Location { get; private set; }
        public MouseButton Button { get; private set; }

        public MouseEventArgs (Control control, Point location, MouseButton button)
            :base(control)
	    {
            Location = location;
            Button = button;
	    }
    }

    public class DragDropEventArgs : ControlEventArgs
    {
        public DragDropEventArgs(Control control)
            :base(control)
        {
        }

        public object Data {get; set;}
        public Control SourceControl { get; internal set; }
        public bool AllowDrop { get; set; }
    }

    public class DragDropInfo
    {
        public bool Dragging { get; private set; }
        public DragDropEventArgs EventArgs { get; private set; }

        public DragDropInfo ()
	    {
            EventArgs = new DragDropEventArgs(null);
    	}

        internal void BeginDrag(object data, Control sourceControl)
        {
            EventArgs.Data = data;
            EventArgs.SourceControl = sourceControl;
            EventArgs.AllowDrop = false;
            Dragging = true;
        }

        internal void EndDrag()
        {
            Dragging = false;
        }

        internal void UpdateEventArgs(Control control)
        {
            DragDropEventArgs newEventArgs = new DragDropEventArgs(control);
            newEventArgs.Data = EventArgs.Data;
            newEventArgs.SourceControl = EventArgs.SourceControl;
            newEventArgs.AllowDrop = EventArgs.AllowDrop;
            EventArgs = newEventArgs;
        }
    }
}
