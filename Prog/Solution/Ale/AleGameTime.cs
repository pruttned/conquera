using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale
{
    /// <summary>
    /// Provides informations about an actual game time
    /// </summary>
    public class AleGameTime
    {
        #region Fields

        /// <summary>
        /// Current frame num
        /// </summary>
        private long mFrameNum = 0;

        /// <summary>
        /// Elapsed time since last frame in seconds
        /// </summary>
        private float mTimeSinceLastFrame = 0;

        /// <summary>
        /// Total elapsed time sinc game start in seconds
        /// </summary>
        private float mTotalTime = 0;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the current frame num
        /// </summary>
        public long FrameNum
        {
            get { return mFrameNum; }
        }

        /// <summary>
        /// Gets the elapsed time since last frame in seconds
        /// </summary>
        public float TimeSinceLastFrame
        {
            get { return mTimeSinceLastFrame; }
        }

        /// <summary>
        /// Gets the total elapsed time sinc game start in seconds
        /// </summary>
        public float TotalTime
        {
            get { return mTotalTime; }
        }

        #endregion Properties

        /// <summary>
        /// Updates the game time. You must call this method (only) once on a frame start.
        /// </summary>
        /// <param name="xnaGameTime">GameTime from xna</param>
        internal void UpdateOnFrameStart(GameTime xnaGameTime)
        {
            mTimeSinceLastFrame = (float)xnaGameTime.ElapsedGameTime.TotalSeconds;
            mTotalTime += (float)mTimeSinceLastFrame;

            mFrameNum++;
        }
    }
}
