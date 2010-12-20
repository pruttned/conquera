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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Input;
using System.Collections.Generic;
using System;

namespace Ale.Gui
{
    /// <summary>
    /// Represents a base class for all controls.
    /// </summary>
    public class Control : IGraphicElementRepositoryOwner
    {
        #region Events

        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event EventHandler Click;
        public event EventHandler MouseLeave;
        public event EventHandler MouseEnter;

        /// <summary>
        /// Occurs when a drag`n`drop operation finishes dropping the dragging content on this control.
        /// </summary>
        public event DragDropEventHandler DragDrop;

        /// <summary>
        /// Occurs when a drag`n`drop operation, which this control represents the source control, finishes.
        /// </summary>
        public event DragDropEventHandler DragFinished;

        /// <summary>
        /// Mouse enter while dragging.
        /// </summary>
        public event DragDropEventHandler DragEnter;

        /// <summary>
        /// Mouse leave while dragging.
        /// </summary>
        public event DragDropEventHandler DragLeave;

        #endregion Events

        #region Fields

        /// <summary>
        /// GuiManager for gui this control belongs to. Whole hierarchy tree shares the same GuiManager.
        /// </summary>
        private GuiManager mGuiManager;

        /// <summary>
        /// Location in the parent control.
        /// </summary>
        private Point mLocation = Point.Zero;

        /// <summary>
        /// Location of this control on screen. Calculated automaticaly.
        /// </summary>
        private Point mScreenLocation = Point.Zero;

        /// <summary>
        /// Parent control of this control.
        /// </summary>
        private CompositeControl mParentControl = null;

        /// <summary>
        /// Use it to specify this control`s screen location anchor.
        /// See 'mAnchorRelativeLocation'.
        /// </summary>
        private AnchorStyles mAnchor = AnchorStyles.None;

        /// <summary>
        /// When 'mAnchor' equals to 'AnchorStyles.None', this field is ignored. Use it to specify this control`s relative screen location 
        /// on the specified screen edge. Use values between 0 and 1.
        /// See 'mAnchor'.
        /// </summary>
        private float mAnchorRelativeLocation = 0;

        private HashSet<EmptyGraphicElementRepository> mGraphicElementRepositories = new HashSet<EmptyGraphicElementRepository>();
        private IGraphicElement mBackground = null;
        private System.Drawing.Size mSize = System.Drawing.Size.Empty;
        private Rectangle mClientArea = Rectangle.Empty;
        private bool mSizeEqualsToBackgroundSize = true;
        private bool mClientAreaFillsWholeControl = true;
        private bool mFirstGuiManagerChange = true;
        private bool mVisible = true;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets (set is protected) the size of this control.
        /// </summary>
        public System.Drawing.Size Size 
        {
            get { return mSize; }
            protected set 
            {
                if (value != Size)
                {
                    BeforeSizeChanged(value);
                    SetSize(value);
                    OnSizeChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets (set is protected) the area in this control, which to crop when needed. For composite controls, child controls can be placed only 
        /// in the client area. Location of the client area is relative to the control`s upper left corner.
        /// </summary>
        public Rectangle ClientArea
        {
            get { return mClientArea; }
            protected set 
            {
                if (value != ClientArea)
                {
                    BeforeClientAreaChanged(value);
                    SetClientArea(value);
                    OnClientAreaChanged();
                }
            }
        }

        /// <summary>
        /// Gets the GuiManager for gui this control belongs to. All controls with the same root shares the same GuiManager.
        /// </summary>
        public GuiManager GuiManager
        {
            get { return mGuiManager; }
            internal set
            {
                if (value != GuiManager)
                {
                    BeforeGuiManagerChanged(value);
                    SetGuiManager(value);
                    OnGuiManagerChanged();

                    if (mFirstGuiManagerChange)
                    {
                        OnGuiManagerFirstChanged();
                        mFirstGuiManagerChange = false;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the location in the parent control.
        /// </summary>
        public Point Location
        {
            get { return mLocation; }
            set 
            {
                if (value != Location)
                {
                    BeforeLocationChanged(value);
                    SetLocation(value);
                    OnLocationChanged();
                }
            }
        }

        /// <summary>
        /// Gets the location of this control on screen. Calculated automaticaly.
        /// </summary>
        public Point ScreenLocation
        {
            get { return mScreenLocation; }
            private set
            {
                if (value != ScreenLocation)
                {
                    BeforeScreenLocationChanged(value);
                    mScreenLocation = value;
                    OnScreenLocationChanged();
                }
            }
        }

        /// <summary>
        /// Gets the parent control of this control. For setting the parent control, add this control to the desired CompositeControl`s 'Controls' collection.
        /// </summary>
        public CompositeControl ParentControl
        {
            get { return mParentControl; }
            internal set 
            {
                if (value != ParentControl)
                {
                    BeforeParentControlChanged(value);
                    SetParentControl(value);
                    OnParentControlChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets this control`s screen location anchor.
        /// See 'AnchorRelativeLocation' property.
        /// </summary>
        public AnchorStyles Anchor
        {
            get { return mAnchor; }
            set
            {
                if (value != Anchor)
                {
                    BeforeAnchorChanged(value);
                    mAnchor = value;
                    RecalculateScreenLocation();
                    OnAnchorChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets this control`s relative location on a screen edge specified by 'Anchor' property. When 'Anchor' property equals to 'AnchorStyles.None', 
        /// 'AnchorRelativeLocation' property is ignored.
        /// See 'Anchor' property.
        /// </summary>
        public float AnchorRelativeLocation
        {
            get { return mAnchorRelativeLocation; }
            set 
            {
                value = MathHelper.Min(1, MathHelper.Max(0, value));

                if (value != AnchorRelativeLocation)
                {
                    BeforeAnchorRelativeLocationChanged(value);
                    mAnchorRelativeLocation = value;
                    RecalculateScreenLocation();
                    OnAnchorRelativeLocationChanged();
                }
            }
        }

        /// <summary>
        /// Gets the X-coordinate of the location in the parent control.
        /// </summary>
        public int X
        {
            get { return Location.X; }
        }

        /// <summary>
        /// Gets the Y-coordinate of the location in the parent control.
        /// </summary>
        public int Y
        {
            get { return Location.Y; }
        }

        /// <summary>
        /// Gets the width of the control.
        /// </summary>
        public int Width
        {
            get { return Size.Width; }
        }

        /// <summary>
        /// Gets the height of the control.
        /// </summary>
        public int Height
        {
            get { return Size.Height; }
        }

        public IGraphicElement Background
        {
            get { return mBackground; }
            set 
            {
                if (value != Background)
                {
                    if (null != Background) //old
                    {
                        Background.SizeChanged -= BackgroundGraphicElement_SizeChanged;
                    }

                    mBackground = value;
                    RefreshSizeCorespondingToBackground();

                    if (null != Background) //new
                    {
                        Background.SizeChanged += new GraphicElementEventHandler(BackgroundGraphicElement_SizeChanged);
                    }
                }
            }
        }

        public bool Visible
        {
            get { return mVisible; }
            set { mVisible = value; }
        }

        /// <summary>
        /// Gets or sets, whether Size is automaticaly set to BackgroundGraphicElement.Size when BackgroundGraphicElement or its size changes.
        /// </summary>
        protected bool SizeEqualsToBackgroundSize
        {
            get { return mSizeEqualsToBackgroundSize; }
            set 
            {
                if (value != SizeEqualsToBackgroundSize)
                {
                    mSizeEqualsToBackgroundSize = value;
                    RefreshSizeCorespondingToBackground();
                }
            }
        }

        /// <summary>
        /// Gets or sets, whether the ClientArea is automaticaly set to fill the whole control, when the control`s size changes.
        /// </summary>
        protected bool ClientAreaFillsWholeControl
        {
            get { return mClientAreaFillsWholeControl; }
            set 
            {
                if (value != ClientAreaFillsWholeControl)
                {
                    mClientAreaFillsWholeControl = value;
                    RefreshClientAreaToFillWholeControl();
                }
            }
        }

        #endregion Properties    

        #region Methods

        /// <summary>
        /// Determines, whether this control occupies the specified screen location.
        /// </summary>
        /// <param name="screenLocation">Screen location.</param>
        /// <returns>True, if this control occupies the specified screen location.</returns>
        public bool OccupiesScreenLocation(Point screenLocation)
        {
            return screenLocation.X >= ScreenLocation.X && screenLocation.Y >= ScreenLocation.Y && screenLocation.X <= ScreenLocation.X + Width &&
                screenLocation.Y <= ScreenLocation.Y + Height;
        }

        /// <summary>
        /// If this control occupies the specified screen location, this control is returned. Otherwise null is returned.
        /// </summary>
        /// <param name="screenLocation">Screen location.</param>
        /// <returns>If this control occupies the specified screen location, this control is returned. Otherwise null is returned.</returns>
        public virtual Control GetControl(Point screenLocation)
        {
            if (OccupiesScreenLocation(screenLocation))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Transforms a screen location to a location relative to the client area of this control.
        /// </summary>
        /// <param name="screenLocation">Screen location.</param>
        /// <returns>Location relative to the client area of this control.</returns>
        public Point PointToClient(Point screenLocation)
        {
            return new Point(screenLocation.X - (ScreenLocation.X + ClientArea.X), screenLocation.Y - (ScreenLocation.Y + ClientArea.Y));
        }

        /// <summary>
        /// Transforms a screen location to a location relative to the client area of this control.
        /// </summary>
        /// <param name="screenLocation">Screen location.</param>
        /// <returns>Location relative to the client area of this control.</returns>
        public Point PointToClient(Vector2 screenLocation)
        {
            return new Point((int)screenLocation.X - (ScreenLocation.X + ClientArea.X), (int)screenLocation.Y - (ScreenLocation.Y + ClientArea.Y));
        }

        /// <summary>
        /// Transforms a location relative to the client area of this control to screen location.
        /// </summary>
        /// <param name="clientLocation">Location relative to the client area of this control.</param>
        /// <returns>Screen location.</returns>
        public Point PointToScreen(Point clientLocation)
        {
            return new Point(clientLocation.X + ClientArea.X + ScreenLocation.X, clientLocation.Y + ClientArea.Y + ScreenLocation.Y);
        }

        /// <summary>
        /// Transforms a location relative to the client area of this control to screen location.
        /// </summary>
        /// <param name="clientLocation">Location relative to the client area of this control.</param>
        /// <returns>Screen location.</returns>
        public Point PointToScreen(Vector2 clientLocation)
        {
            return new Point((int)clientLocation.X + ClientArea.X + ScreenLocation.X, (int)clientLocation.Y + ClientArea.Y + ScreenLocation.Y);
        }

        public void AddGraphicElementRepository(EmptyGraphicElementRepository repository)
        {
            if (this != repository.Owner)
            {
                repository.Owner = this;
            }
        }

        public void RemoveGraphicElementRepository(EmptyGraphicElementRepository repository)
        {
            if (this != repository.Owner)
            {
                throw new ArgumentException("This control does not contain the specified repository.");
            }
            repository.Owner = null;
        }

        /// <summary>
        /// Draws background of this control.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public virtual void DrawBackground(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            if (null != Background)
            {
                Background.Draw(spriteBatch, ScreenLocation, gameTime);
            }
        }

        /// <summary>
        /// Draws foreground of this control.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="gameTime"></param>
        public virtual void DrawForeground(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            foreach (EmptyGraphicElementRepository repository in mGraphicElementRepositories)
            {
                repository.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary>
        /// Draws this control - background and foreground.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch to use.</param>
        /// <param name="gameTime"></param>
        public virtual void Draw(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            DrawBackground(spriteBatch, gameTime);
            DrawForeground(spriteBatch, gameTime);
        }

        /// <summary>
        /// Determines, whether the specified control is this control or is one of the child controls (recursive) of this control.
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public virtual bool ContainsRecursive(Control control)
        {
            return this == control;
        }

        internal virtual void SetSize(System.Drawing.Size value)
        {
            mSize = value;
            RefreshClientAreaToFillWholeControl();
        }

        internal virtual void SetClientArea(Rectangle value)
        {
            mClientArea = value;
        }

        /// <summary>
        /// Sets the value of 'GuiManager' property. When overriding, call also the base implementation.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal virtual void SetGuiManager(GuiManager value)
        {
            mGuiManager = value;
        }

        /// <summary>
        /// Sets the value of 'Location' property. When overriding, call also the base implementation.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal virtual void SetLocation(Point value)
        {
            mLocation = value;
            RecalculateScreenLocation();
        }

        /// <summary>
        /// Sets the value of 'ParentControl' property. When overriding, call also the base implementation.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal virtual void SetParentControl(CompositeControl value)
        {
            CompositeControl oldParent = ParentControl;
            mParentControl = value;

            if (null != value && null != oldParent) //Value is null when 'parentControl.Remove()/RemoveAt()' is called - calling 'Remove()' again would cause 
            {                                           //a resursive endless loop. Value is also null, when 'parentControl.Clear()' is called - no need to call
                oldParent.Controls.Remove(this);        //'Remove()', becouse the collection is cleared using 'List.Clear()'.
            }

            GuiManager = (null != value) ? value.GuiManager : null;
            RecalculateScreenLocation();
        }

        /// <summary>
        /// Recalculates 'ScreenLocation' property. When overriding, call also the base implementation.
        /// </summary>
        internal virtual void RecalculateScreenLocation()
        {
            switch (Anchor)
            {
                case AnchorStyles.Left:
                    ScreenLocation = new Point(0, (int)((mGuiManager.ScreenSize.Height - Height) * AnchorRelativeLocation));
                    break;

                case AnchorStyles.Right:
                    ScreenLocation = new Point(mGuiManager.ScreenSize.Width - Width, (int)((mGuiManager.ScreenSize.Height - Height) * AnchorRelativeLocation));
                    break;

                case AnchorStyles.Top:
                    ScreenLocation = new Point((int)((mGuiManager.ScreenSize.Width - Width) * AnchorRelativeLocation), 0);
                    break;

                case AnchorStyles.Bottom:
                    ScreenLocation = new Point((int)((mGuiManager.ScreenSize.Width - Width) * AnchorRelativeLocation), mGuiManager.ScreenSize.Height - Height);
                    break;

                default: //none
                    RecalculateScreenLocationNoAnchor();
                    break;
            }
        }

        /// <summary>
        /// Handles mouse down on this control.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        internal void HandleMouseDown(MouseButton button, MouseManager mouseManager)
        {
            OnMouseDown(button, mouseManager);
        }

        /// <summary>
        /// Handles mouse up on this control.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        internal void HandleMouseUp(MouseButton button, MouseManager mouseManager)
        {
            OnMouseUp(button, mouseManager);
        }

        /// <summary>
        /// Handles click on this control.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        internal void HandleClick(MouseButton button, MouseManager mouseManager)
        {
            OnClick(button, mouseManager);
        }

        /// <summary>
        /// Handles mouse leave on this control.
        /// </summary>
        internal void HandleMouseLeave()
        {
            OnMouseLeave();
        }

        /// <summary>
        /// Handles mouse enter on this control.
        /// </summary>
        internal void HandleMouseEnter()
        {
            OnMouseEnter();
        }

        /// <summary>
        /// Handles drag drop on this control.
        /// </summary>
        internal void HandleDragDrop()
        {
            OnDragDrop();
        }

        /// <summary>
        /// Handles drag finished on this control.
        /// </summary>
        internal void HandleDragFinished()
        {
            OnDragFinished();
        }

        /// <summary>
        /// Handles drag enter on this control.
        /// </summary>
        internal void HandleDragEnter()
        {
            OnDragEnter();
        }

        /// <summary>
        /// Handles drag leave on this control.
        /// </summary>
        internal void HandleDragLeave()
        {
            OnDragLeave();
        }

        /// <summary>
        /// Occurs on mouse down on this control. Raises the MouseDown event.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        protected virtual void OnMouseDown(MouseButton button, MouseManager mouseManager)
        {
            if (null != MouseDown)
            {
                MouseDown(this, new MouseEventArgs(PointToClient(mouseManager.CursorPosition), button));
            }

            OnMouseDownMouseStateMonitor();
        }

        /// <summary>
        /// Occurs on mouse up on this control. Raises the MouseUp event.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        protected virtual void OnMouseUp(MouseButton button, MouseManager mouseManager)
        {
            if (null != MouseUp)
            {
                MouseUp(this, new MouseEventArgs(PointToClient(mouseManager.CursorPosition), button));
            }

            OnMouseUpMouseStateMonitor();
        }

        /// <summary>
        /// Occurs on click on this control. Raises the Click event.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        protected virtual void OnClick(MouseButton button, MouseManager mouseManager)
        {
            if (null != Click)
            {
                Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Occurs on mouse leave on this control. Raises the MouseLeave event.
        /// </summary>
        protected virtual void OnMouseLeave()
        {
            if (null != MouseLeave)
            {
                MouseLeave(this, EventArgs.Empty);
            }

            OnMouseLeaveMouseStateMonitor();
        }

        /// <summary>
        /// Occurs on mouse enter on this control. Raises the MouseEnter event.
        /// </summary>
        protected virtual void OnMouseEnter()
        {
            if (null != MouseEnter)
            {
                MouseEnter(this, EventArgs.Empty);
            }

            OnMouseEnterMouseStateMonitor();
        }

        /// <summary>
        /// Occurs on drag drop on this control. Raises the DragDrop event.
        /// </summary>
        protected virtual void OnDragDrop()
        {
            if (null != DragDrop)
            {
                DragDrop(this, GuiManager.DragDropInfo.EventArgs);
            }
        }

        /// <summary>
        /// Occurs on drag finished on this control. Raises the DragFinished event.
        /// </summary>
        protected virtual void OnDragFinished()
        {
            if (null != DragFinished)
            {
                DragFinished(this, GuiManager.DragDropInfo.EventArgs);
            }
        }

        /// <summary>
        /// Occurs on drag enter on this control. Raises the DragEnter event.
        /// </summary>
        protected virtual void OnDragEnter()
        {
            if (null != DragEnter)
            {
                DragEnter(this, GuiManager.DragDropInfo.EventArgs);
            }
        }

        /// <summary>
        /// Occurs on drag leave on this control. Raises the DragLeave event.
        /// </summary>
        protected virtual void OnDragLeave()
        {
            if (null != DragLeave)
            {
                GuiManager.DragDropInfo.EventArgs.AllowDrop = false;
                DragLeave(this, GuiManager.DragDropInfo.EventArgs);
            }
        }

        /// <summary>
        /// Recalculates 'ScreenLocation' property when 'Anchor' property equals to 'AnchorStyles.None'.
        /// </summary>
        private void RecalculateScreenLocationNoAnchor()
        {
            if (null == ParentControl)
            {
                ScreenLocation = Location;
            }
            else
            {
                ScreenLocation = new Point(ParentControl.ScreenLocation.X + ParentControl.ClientArea.Location.X + Location.X,
                                           ParentControl.ScreenLocation.Y + ParentControl.ClientArea.Location.Y + Location.Y);
            }
        }

        private void BackgroundGraphicElement_SizeChanged(IGraphicElement graphicElement)
        {
            RefreshSizeCorespondingToBackground();
        }

        private void RefreshSizeCorespondingToBackground()
        {
            if(SizeEqualsToBackgroundSize)
            {
                Size = null != Background ? Background.Size : System.Drawing.Size.Empty;
            }
        }

        private void RefreshClientAreaToFillWholeControl()
        {
            if (ClientAreaFillsWholeControl)
            {
                ClientArea = new Rectangle(0, 0, Size.Width, Size.Height);
            }
        }

        #region Before/On virtual methods for properties

        protected virtual void BeforeParentControlChanged(CompositeControl newParentControl) {}
        protected virtual void OnParentControlChanged() { }
        protected virtual void BeforeGuiManagerChanged(GuiManager newGuiManager) { }
        protected virtual void OnGuiManagerChanged() { }
        protected virtual void OnGuiManagerFirstChanged() { }
        protected virtual void BeforeLocationChanged(Point newLocation) { }
        protected virtual void OnLocationChanged() { }
        protected virtual void BeforeScreenLocationChanged(Point newScreenLocation) { }
        protected virtual void OnScreenLocationChanged() { }
        protected virtual void BeforeAnchorChanged(AnchorStyles newAnchor) { }
        protected virtual void OnAnchorChanged() { }
        protected virtual void BeforeAnchorRelativeLocationChanged(float newAnchorRelativeLocation) { }
        protected virtual void OnAnchorRelativeLocationChanged() { }
        protected virtual void BeforeSizeChanged(System.Drawing.Size newSize) { }
        protected virtual void OnSizeChanged() { }
        protected virtual void BeforeClientAreaChanged(Rectangle newClientArea) { }
        protected virtual void OnClientAreaChanged() { }

        #endregion Before/On virtual methods for properties        

        #endregion Methods

        #region IGraphicElementRepositoryOwner

        /// <summary>
        /// Explicit implementation! Do not place 'public' here!
        /// </summary>
        /// <param name="repository"></param>
        void IGraphicElementRepositoryOwner.RemoveGraphicElementRepositoryCore(EmptyGraphicElementRepository repository)
        {
            mGraphicElementRepositories.Remove(repository);
        }

        /// <summary>
        /// Explicit implementation! Do not place 'public' here!
        /// </summary>
        /// <param name="repository"></param>
        void IGraphicElementRepositoryOwner.AddGraphicElementRepositoryCore(EmptyGraphicElementRepository repository)
        {
            mGraphicElementRepositories.Add(repository);
        }

        #endregion IGraphicElementRepositoryOwner

        #region Mouse State Monitor

        public enum MouseStates { Default, MouseOver, MouseDown }

        public class MouseStateEventArgs : EventArgs
        {
            private MouseStates mMouseState;

            public MouseStates MouseState
            {
                get { return mMouseState; }
            }

            public MouseStateEventArgs(MouseStates mouseState)
            {
                mMouseState = mouseState;
            }
        }

        public delegate void MouseStateEventHandler(Control control, MouseStateEventArgs e);

        public event MouseStateEventHandler MouseStateChanged;

        private MouseStates mMouseState;

        public MouseStates MouseState
        {
            get { return mMouseState; }
            private set //call only if changed
            {
                mMouseState = value;
                OnMouseStateChanged();
            }
        }

        protected virtual void OnMouseStateChanged()
        {
            if (null != MouseStateChanged) //MouseState property is set only when it is changed, so there is no need to check it here 
            {
                MouseStateChanged(this, new MouseStateEventArgs(MouseState));
            }
        }

        private void OnMouseEnterMouseStateMonitor()
        {
            if (MouseStates.Default == MouseState)
            {
                MouseState = MouseStates.MouseOver;
            }
        }

        private void OnMouseLeaveMouseStateMonitor()
        {
            if (MouseStates.MouseOver == MouseState)
            {
                MouseState = MouseStates.Default;
            }
        }

        private void OnMouseDownMouseStateMonitor()
        {
            if (MouseStates.MouseOver == MouseState)
            {
                MouseState = MouseStates.MouseDown;
            }
        }

        private void OnMouseUpMouseStateMonitor()
        {
            if (MouseStates.MouseDown == MouseState)
            {
                MouseState = this == GuiManager.GetControlUnderMouse() ? MouseStates.MouseOver : MouseStates.Default;
            }
        }

        #endregion Mouse State Monitor
    }
}
