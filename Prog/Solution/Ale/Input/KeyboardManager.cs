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

namespace Ale.Input
{
    /// <summary>
    /// Handler for keyboard events
    /// </summary>
    /// <param name="key"></param>
    /// <param name="keyboardManager"></param>
    public delegate void KeyEventHandler(Keys key, IKeyboardManager keyboardManager);

    public interface IKeyboardManager : IFrameListener
    {
        bool IsKeyDown(Keys key);
        event KeyEventHandler KeyDown;
        event KeyEventHandler KeyUp;
    }

    /// <summary>
    /// Keyboard manager
    /// </summary>
    public class KeyboardManager : IKeyboardManager
    {
        #region Events

        /// <summary>
        /// Raised whenever is keyboard key pressed
        /// </summary>
        public event KeyEventHandler KeyDown;

        /// <summary>
        /// Raised whenever is keyboard key released
        /// </summary>
        public event KeyEventHandler KeyUp;

        #endregion Events

        #region Fields

        /// <summary>
        /// Current keyboard state
        /// </summary>
        private KeyboardState mCurrentKeyboardState;

        /// <summary>
        /// Previously pressed keys
        /// </summary>
        private Keys[] mPreviouslyPressedKeys = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        public KeyboardManager()
        {
            mCurrentKeyboardState = new KeyboardState();
        }

        /// <summary>
        /// Gets whether is specified keyboard key currently pressed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsKeyDown(Keys key)
        {
            return (KeyState.Down == mCurrentKeyboardState[key]);
        }

        #region IFrameListener

        /// <summary>
        /// Updates keyboard manager
        /// </summary>
        /// <param name="gameTime"></param>
        void IFrameListener.BeforeUpdate(AleGameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();
            if (newState != mCurrentKeyboardState)
            {
                //current state must be updated before handlings of events so it can be used there
                KeyboardState previousKeyboardState = mCurrentKeyboardState;
                mCurrentKeyboardState = newState;

                Keys[] newPressedKeys = newState.GetPressedKeys();

                //key down
                if (null != KeyDown)
                {
                    foreach (Keys key in newPressedKeys)
                    {
                        if (KeyState.Up == previousKeyboardState[key]) //not pressed until now
                        {
                            KeyDown.Invoke(key, this);
                        }
                    }
                }

                //key up
                if (null != KeyUp && null != mPreviouslyPressedKeys)
                {
                    foreach (Keys key in mPreviouslyPressedKeys)
                    {
                        if (KeyState.Up == newState[key]) //released
                        {
                            KeyUp.Invoke(key, this);
                        }
                    }
                }

                mPreviouslyPressedKeys = newPressedKeys;
            }
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

        #endregion Methods
    }
}
