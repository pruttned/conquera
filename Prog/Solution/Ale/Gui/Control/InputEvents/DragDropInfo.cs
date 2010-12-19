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

namespace Ale.Gui
{
    /// <summary>
    /// Drag`n`drop event informations.
    /// </summary>
    public class DragDropInfo
    {
        #region Fields

        /// <summary>
        /// True, the end user is currently dragging.
        /// </summary>
        private bool mDragging = false;

        /// <summary>
        /// Event args for all drag`n`drop events. This object is passed to each drag`n`drop event.
        /// </summary>
        private DragDropEventArgs mEventArgs = new DragDropEventArgs();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets, whether the end user is currently dragging.
        /// </summary>
        public bool Dragging
        {
            get { return mDragging; }
        }

        /// <summary>
        /// Gets the event args for all drag`n`drop events. This object is passed to each drag`n`drop event.
        /// </summary>
        public DragDropEventArgs EventArgs
        {
            get { return mEventArgs; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets this DragDropInfo up for a new drag operation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sourceControl"></param>
        internal void BeginDrag(object data, Control sourceControl)
        {
            EventArgs.Data = data;
            EventArgs.SourceControl = sourceControl;
            EventArgs.AllowDrop = false;
            mDragging = true;
        }

        /// <summary>
        /// Sets 'Dragging' property to false.
        /// </summary>
        internal void EndDrag()
        {
            mDragging = false;
        }

        #endregion Methods
    }
}
