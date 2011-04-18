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

namespace Ale.Gui
{
    /// <summary>
    /// List of animation frames owned by an Animation.
    /// </summary>
    public class AnimationFrameList : ListBase<Image>
    {
        #region Fields

        /// <summary>
        /// Animation, which these frames belongs to.
        /// </summary>
        private Animation mOwnerAnimation;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the Animation, which these frames belongs to.
        /// </summary>
        public Animation OwnerAnimation
        {
            get { return mOwnerAnimation; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructs a new AnimationFrameList instance.
        /// </summary>
        /// <param name="ownerAnimation">Animation, which these frames belongs to.</param>
        public AnimationFrameList(Animation ownerAnimation)
        {
            mOwnerAnimation = ownerAnimation;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Overriden. When the owner animation is not running, removes the specified frame from this list. 
        /// Otherwise throws an InvalidOperationException.
        /// </summary>
        /// <param name="item">Frame to remove.</param>
        /// <returns>True if the frame is successfully removed, otherwise false. This method also returns false 
        /// if the frame was not found in this list.</returns>
        public override bool Remove(Image item)
        {
            CheckRunning();
            return base.Remove(item);
        }

        /// <summary>
        /// Overriden. When the owner animation is not running, removes a frame with the specified index from this list.
        /// Otherwise throws an InvalidOperationException.
        /// </summary>
        /// <param name="index">Index of the frame to remove.</param>
        public override void RemoveAt(int index)
        {
            CheckRunning();
            base.RemoveAt(index);
        }

        /// <summary>
        /// Overriden. When the owner animation is not running, removes all frames from this list.
        /// Otherwise throws an InvalidOperationException.
        /// </summary>
        public override void Clear()
        {
            CheckRunning();
            base.Clear();
        }

        /// <summary>
        /// If the owner Animation is running, throws an InvalidOperationException.
        /// </summary>
        private void CheckRunning()
        {
            if (OwnerAnimation.Running)
            {
                throw new InvalidOperationException("Cannot remove a frame from a running animation.");
            }
        }

        #endregion Methods
    }
}
