using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Ale.Input
{
    /// <summary>
    /// Keyboard manager
    /// </summary>
    public class KeyboardManager : IFrameListener
    {
        #region Delegates
        
        /// <summary>
        /// Handler for keyboard events
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyboardManager"></param>
        public delegate void KeyEventHandler(Keys key, KeyboardManager keyboardManager);
        
        #endregion Delegates

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
