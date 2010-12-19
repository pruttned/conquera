using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// Unloads objects when they moves a specified distance away from the camera's frustum 
    /// </summary>
    public class DynamicallyLoadableObjectsDistanceUnloader : DynamicallyLoadableObjectsUnloader
    {
        #region Fields

        private ICamera mMainCamera = null;
        private float mBoundsMultiplicator;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Main scene camera for checking object distance
        /// </summary>
        public ICamera MainCamera
        {
            get { return mMainCamera; }
            set { mMainCamera = value; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="boundsMultiplicator">- Multiplicator that is applied to the object's bounding radius before checking its visibility for unloading purposes</param>
        public DynamicallyLoadableObjectsDistanceUnloader(float boundsMultiplicator)
            :base(2)
        {
            mBoundsMultiplicator = boundsMultiplicator;
        }

        /// <summary>
        /// Whether should be an object unloaded
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected override bool ObjectCanBeUnloaded(IDynamicallyLoadableObject obj)
        {
            if (null == mMainCamera)
            {
                throw new InvalidOperationException("MainCamera is not set");
            }

            BoundingSphere bounds = obj.WorldBounds;
            bounds.Radius *= mBoundsMultiplicator;
            return !mMainCamera.IsInSight(ref bounds);
        }

        #endregion Methods
    }

}
