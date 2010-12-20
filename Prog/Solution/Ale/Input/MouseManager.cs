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
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Ale.Input
{
    /// <summary>
    /// Handles mouse input.
    /// Only one instance of mouse manager can exist at a same time
    /// </summary>
    public sealed class MouseManager : IDisposable, IFrameListener
    {
        #region Delegates

        /// <summary>
        /// Handler for mouse button events
        /// </summary>
        /// <param name="button"></param>
        /// <param name="mouseManager"></param>
        public delegate void MouseButtonEventHandler(MouseButton button, MouseManager mouseManager);
        
        #endregion Delegates

        #region Events

        /// <summary>
        /// Raised whenever is mouse button pressed
        /// </summary>
        public event MouseButtonEventHandler MouseButtonDown;

        /// <summary>
        /// Raised whenever is mouse button released
        /// </summary>
        public event MouseButtonEventHandler MouseButtonUp;

        #endregion Events

        #region Fields

        /// <summary>
        /// Check whether instance already exists
        /// </summary>
        private static bool mInstanceExists = false;

        /// <summary>
        /// Current mouse state
        /// </summary>
        private MouseState mCurrentMouseState;
        
        /// <summary>
        /// Control(Form) that is used for rendering
        /// </summary>
        private System.Windows.Forms.Control mRenderControl;

        /// <summary>
        /// Whether should be real cursor clipped into the client area of the rendering control
        /// </summary>
        private bool mClipRealCursor = false;

        /// <summary>
        /// Position of the cursor
        /// </summary>
        private Vector2 mCursorPosition;

        /// <summary>
        /// Mouse movement (z-wheel)
        /// </summary>
        private Vector3 mCursorPositionDelta;

        /// <summary>
        /// Previous position of the real mouse cursor
        /// </summary>
        private Vector2 mRealCursorPreviousPosition;

        /// <summary>
        /// Render control client bounds in screen space
        /// </summary>
        private Win32Api.RECT mRenderControlClientBounds;

        /// <summary>
        /// Center of the client area in the window space
        /// </summary>
        private Point mRenderControlClientAreaCenter;

        /// <summary>
        /// Whether is object already disposed
        /// </summary>
        private bool mIsDisposed = false;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the position of the cursor in the render control client area
        /// </summary>
        public Vector2 CursorPosition
        {
            get { return mCursorPosition; }
        }

        /// <summary>
        /// Gets the movement of the cursor (z-wheel)
        /// </summary>
        public Vector3 CursorPositionDelta
        {
            get { return mCursorPositionDelta; }
        }

        /// <summary>
        /// Gets whether should be real cursor clipped into the client area of the rendering control
        /// </summary>
        public bool ClipRealCursor
        {
            get { return mClipRealCursor; }
            set
            {
                if (value != mClipRealCursor)
                {
                    mClipRealCursor = value;
                    if (mClipRealCursor)
                    {
                        ClipRealCursorToClientArea();

                        Mouse.SetPosition(mRenderControlClientAreaCenter.X, mRenderControlClientAreaCenter.Y);
                    }
                    else
                    {
                        UnclipRealCursorFromClientArea();
                        Mouse.SetPosition((int)mCursorPosition.X, (int)mCursorPosition.Y);
                        mRealCursorPreviousPosition = mCursorPosition;
                    }
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderControl">- Control that is used for rendering</param>
        /// <exception cref="InvalidOperationException">- Only one instance of mouse manager can exist at a same time</exception>
        public MouseManager(System.Windows.Forms.Control renderControl)
        {
            if (mInstanceExists) //not thread safe
            {
                throw new InvalidOperationException("Only one instance of mouse manager can exist at a same time");
            }
            else
            {
                mInstanceExists = true;
            }

            Mouse.WindowHandle = renderControl.Handle;

            mRenderControl = renderControl;

            mRenderControl.Move += new EventHandler(mRenderControl_Move); ;
            mRenderControl.Resize += new EventHandler(mRenderControl_Resize); ;

            OnRenderControlClientAreaChange();

            mRealCursorPreviousPosition = mCursorPosition = new Vector2(mRenderControlClientAreaCenter.X, mRenderControlClientAreaCenter.Y);
        }

        /// <summary>
        /// Gets whether is specified mouse button currently pressed
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsButtonDown(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return (ButtonState.Pressed == mCurrentMouseState.LeftButton);
                case MouseButton.Right:
                    return (ButtonState.Pressed == mCurrentMouseState.RightButton);
                default:
                    return (ButtonState.Pressed == mCurrentMouseState.MiddleButton);
            }
        }

        #region IDisposable

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mRenderControl.Move -= mRenderControl_Move;
                mRenderControl.Resize -= mRenderControl_Resize;
                mRenderControl = null;

                mInstanceExists = false;

                mIsDisposed = true;
                GC.SuppressFinalize(this);
            }
        }

        #endregion IDisposable

        #region IFrameListener

        /// <summary>
        /// Updates mouse manager
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            MouseState newMouseState = Mouse.GetState();

            //current state must be updated before handlings of events so it can be used there
            MouseState previousMouseState = mCurrentMouseState;
            mCurrentMouseState = newMouseState;

            if (ClipRealCursor)
            {
                mCursorPositionDelta.X = newMouseState.X - mRenderControlClientAreaCenter.X;
                mCursorPositionDelta.Y = newMouseState.Y - mRenderControlClientAreaCenter.Y;

                Mouse.SetPosition(mRenderControlClientAreaCenter.X, mRenderControlClientAreaCenter.Y);

                mCursorPosition.X += mCursorPositionDelta.X;
                mCursorPosition.Y += mCursorPositionDelta.Y;

            }
            else
            {
                mCursorPositionDelta.X = newMouseState.X - mRealCursorPreviousPosition.X;
                mCursorPositionDelta.Y = newMouseState.Y - mRealCursorPreviousPosition.Y;

                mCursorPosition.X = newMouseState.X;
                mCursorPosition.Y = newMouseState.Y;
            }

            mCursorPositionDelta.Z = newMouseState.ScrollWheelValue - previousMouseState.ScrollWheelValue;

            mRealCursorPreviousPosition.X = newMouseState.X;
            mRealCursorPreviousPosition.Y = newMouseState.Y;

            mCursorPosition.X = MathHelper.Clamp(mCursorPosition.X, 0, mRenderControlClientBounds.Width);
            mCursorPosition.Y = MathHelper.Clamp(mCursorPosition.Y, 0, mRenderControlClientBounds.Height);


            //buton events
            HandleMouseButtonStateChange(MouseButton.Left, previousMouseState.LeftButton, newMouseState.LeftButton);
            HandleMouseButtonStateChange(MouseButton.Right, previousMouseState.RightButton, newMouseState.RightButton);
            HandleMouseButtonStateChange(MouseButton.Middle, previousMouseState.MiddleButton, newMouseState.MiddleButton);
        }

        /// <summary>
        /// Called after updating a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterUpdate(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// Called before rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeRender(AleGameTime gameTime)
        {
        }

        /// <summary>
        /// Called after rendering a frame
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.AfterRender(AleGameTime gameTime)
        {
        }

        #endregion IFrameListener

        /// <summary>
        /// Checks and handles mouse button state change
        /// </summary>
        /// <param name="button"></param>
        /// <param name="previousButtonState"></param>
        /// <param name="newButtonState"></param>
        private void HandleMouseButtonStateChange(MouseButton button, ButtonState previousButtonState, ButtonState newButtonState)
        {
            if (previousButtonState != newButtonState)
            {
                if (newButtonState == ButtonState.Pressed)
                {
                    if (null != MouseButtonDown)
                    {
                        MouseButtonDown.Invoke(button, this);
                    }
                }
                else
                {
                    if (null != MouseButtonUp)
                    {
                        MouseButtonUp.Invoke(button, this);
                    }
                }
            }
        }

        /// <summary>
        /// Clips the real cursor to render control's client area
        /// </summary>
        private void ClipRealCursorToClientArea()
        {
            //Mouse.SetPosition is not enough to keep the cursor in the client area because of possibility of low fps
            Win32Api.ClipCursor(ref mRenderControlClientBounds);
        }

        /// <summary>
        /// Unclips the real cursor from the render control's client area
        /// </summary>
        private void UnclipRealCursorFromClientArea()
        {
            Win32Api.ClipCursor(IntPtr.Zero);
        }

        /// <summary>
        /// Handles render control's client area changes
        /// </summary>
        private void OnRenderControlClientAreaChange()
        {
            System.Drawing.Point point = mRenderControl.PointToScreen(System.Drawing.Point.Empty);
            mRenderControlClientBounds = new Win32Api.RECT(point.X, point.Y, point.X + mRenderControl.ClientSize.Width, point.Y + mRenderControl.ClientSize.Height);
            mRenderControlClientAreaCenter = new Point(mRenderControl.ClientSize.Width / 2, mRenderControl.ClientSize.Height / 2);

            if (ClipRealCursor)
            {
                ClipRealCursorToClientArea();
            }
        }

        /// <summary>
        /// Handles client control resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderControl_Resize(object sender, EventArgs e)
        {
            OnRenderControlClientAreaChange();
        }

        /// <summary>
        /// Handles client control movement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mRenderControl_Move(object sender, EventArgs e)
        {
            OnRenderControlClientAreaChange();
        }

        #endregion Methods
    }
}
