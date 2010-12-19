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
