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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    /// <summary>
    /// Represents an animation with frames as 2D images.
    /// </summary>
    public class Animation : GraphicElement
    {
        /// <summary>
        /// Frames of this Animation.
        /// </summary>
        private AnimationFrameList mFrames;

        /// <summary>
        /// Index of frame, which will be drawn on next Draw() call. Value 2.4 means frame with index 2.
        /// </summary>
        private float mCurrentFrameIndex;

        /// <summary>
        /// Duration (in miliseconds) of each frame.
        /// </summary>
        private int mFrameDuration;

        /// <summary>
        /// True, when the last frame is reached, the animation starts from the biginning. False, when the last frame is reached, the animation stops.
        /// </summary>
        private bool mLoop;

        /// <summary>
        /// True, this Animation is currently running.
        /// </summary>
        private bool mRunning = true;

        private float mGameTimeOnLastUpdate;

        /// <summary>
        /// Gets the frames of this Animation.
        /// </summary>
        public AnimationFrameList Frames
        {
            get { return mFrames; }
        }

        /// <summary>
        /// Gets or sets the duration (in miliseconds) of each frame.
        /// </summary>
        public int FrameDuration
        {
            get { return mFrameDuration; }
            set { mFrameDuration = value; }
        }

        /// <summary>
        /// Gets or sets, whether the animation starts from the beginning or stops when the last frame is reached.
        /// True, when the last frame is reached, the animation starts from the biginning. False, when the last frame is reached, the animation stops.
        /// </summary>
        public bool Loop
        {
            get { return mLoop; }
            set { mLoop = value; }
        }

        /// <summary>
        /// Gets, whether this Animation is currently running.
        /// </summary>
        public bool Running
        {
            get { return mRunning; }
        }

        /// <summary>
        /// Constructs a new Animation instance.
        /// </summary>
        /// <param name="frameDuration">Duration (in miliseconds) of each frame.</param>
        /// <param name="loop">True, when the last frame is reached, the animation starts from the biginning. 
        /// False, when the last frame is reached, the animation stops.</param>
        public Animation(int frameDuration, bool loop)
        {
            mFrames = new AnimationFrameList(this);
            FrameDuration = frameDuration;
            Loop = loop;
        }

        /// <summary>
        /// Starts the animation from the first frame (frame with zero index).
        /// </summary>
        public void Start()
        {
            if (0 == Frames.Count)
            {
                throw new InvalidOperationException("Cannot start an Animation with no frames.");
            }

            mGameTimeOnLastUpdate = -1;
            SetCurrentFrameIndex(0);
            mRunning = true;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            mRunning = false;
        }

        private void SetCurrentFrameIndex(float newCurrentFrameIndex)
        {
            System.Drawing.SizeF oldSize = Size;
            mCurrentFrameIndex = newCurrentFrameIndex;

            if (oldSize != Size)
            {
                RaiseSizeChanged();
            }
        }

        #region GraphicElement

        /// <summary>
        /// Gets the size of this graphic element.
        /// </summary>
        public override System.Drawing.SizeF Size
        {
            get { return Frames[(int)mCurrentFrameIndex].Size; }
        }

        /// <summary>
        /// Draws this graphic element.
        /// </summary>
        /// <param name="spriteBatch">SpriteBatch used to draw.</param>
        /// <param name="screenLocation">Destination location, where to draw.</param>
        /// <param name="gameTime">Current game time.</param>
        public override void Draw(SpriteBatch spriteBatch, Point screenLocation, AleGameTime gameTime)
        {
            Update(gameTime);
            Frames[(int)mCurrentFrameIndex].Draw(spriteBatch, screenLocation, gameTime);
        }

        /// <summary>
        /// Activates this graphic element - calls 'Start' method.
        /// </summary>
        public override void Activate()
        {
            Start();
        }

        /// <summary>
        /// Deactivates this graphic element - calls 'Stop' method.
        /// </summary>
        public override void Deactivate()
        {
            Stop();
        }

        /// <summary>
        /// Updates the state of this graphic element.
        /// </summary>
        /// <param name="gameTime">Current game time.</param>
        private void Update(AleGameTime gameTime)
        {
            if (mRunning)
            {
                //Calculating new current frame index.
                float newCurrentFrameIndex = 0;
                if (-1 != mGameTimeOnLastUpdate)
                {
                    newCurrentFrameIndex = mCurrentFrameIndex + (gameTime.TotalTime - mGameTimeOnLastUpdate) / (FrameDuration / 1000.0f);
                }

                //Updating.
                if (newCurrentFrameIndex >= Frames.Count)
                {
                    if (Loop)
                    {
                        SetCurrentFrameIndex(newCurrentFrameIndex % Frames.Count);
                    }
                    else
                    {
                        Stop();
                    }
                }
                else
                {
                    SetCurrentFrameIndex(newCurrentFrameIndex);
                }
                
                mGameTimeOnLastUpdate = gameTime.TotalTime;
            }
        }

        #endregion GraphicElement
    }
}
