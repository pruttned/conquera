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