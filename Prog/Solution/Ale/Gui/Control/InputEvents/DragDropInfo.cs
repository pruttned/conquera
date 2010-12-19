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