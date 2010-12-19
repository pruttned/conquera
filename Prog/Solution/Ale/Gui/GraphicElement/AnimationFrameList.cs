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
