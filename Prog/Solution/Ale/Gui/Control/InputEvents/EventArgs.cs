using Ale.Input;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    /// <summary>
    /// Base class for all event args.
    /// </summary>
    public class EventArgs
    {
        /// <summary>
        /// An empty event agrs object - has no data.
        /// </summary>
        public static EventArgs Empty = new EventArgs();
    }

    /// <summary>
    /// Event args with mouse information.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Mouse location relative to the sender control.
        /// </summary>
        private Point mLocation;

        /// <summary>
        /// Pressed mouse button.
        /// </summary>
        private MouseButton mButton;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the mouse location relative to the sender control.
        /// </summary>
        public Point Location
        {
            get { return mLocation; }
        }

        /// <summary>
        /// Gets the X coordinate of the mouse location relative to the sender control.
        /// </summary>
        public int X
        {
            get { return Location.X; }
        }

        /// <summary>
        /// Gets the Y coordinate of the mouse location relative to the sender control.
        /// </summary>
        public int Y
        {
            get { return Location.Y; }
        }

        /// <summary>
        /// Gets the pressed mouse button.
        /// </summary>
        public MouseButton Button
        {
            get { return mButton; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new MouseEventArgs instance.
        /// </summary>
        /// <param name="location">Mouse location relative to the sender control.</param>
        /// <param name="button">Pressed mouse button.</param>
        public MouseEventArgs(Point location, MouseButton button)
        {
            mLocation = location;
            mButton = button;
        }

        #endregion Constructors
    }

    /// <summary>
    /// Event args with drag`n`drop information.
    /// </summary>
    public class DragDropEventArgs : EventArgs
    {
        #region Fields

        /// <summary>
        /// Drag data.
        /// </summary>
        private object mData = null;

        /// <summary>
        /// Source control of dragging.
        /// </summary>
        private Control mSourceControl = null;

        /// <summary>
        /// Determines, whether drop is allowed.
        /// </summary>
        private bool mAllowDrop = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the drag data.
        /// </summary>
        public object Data
        {
            get { return mData; }
            set { mData = value; }
        }

        /// <summary>
        /// Gets the source control of dragging.
        /// </summary>
        public Control SourceControl
        {
            get { return mSourceControl; }
            internal set { mSourceControl = value; }
        }

        /// <summary>
        /// Gets or sets the information, whether drop is allowed.
        /// </summary>
        public bool AllowDrop
        {
            get { return mAllowDrop; }
            set { mAllowDrop = value; }
        }

        #endregion Properties
    }
}