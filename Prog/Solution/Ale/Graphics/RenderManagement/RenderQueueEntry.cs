using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    /// <summary>
    /// One entry in the render queue that describes order of a given renderable unit in the queue
    /// </summary>
    struct RenderQueueEntry : IComparable<RenderQueueEntry>
    {
        #region Fields

        /// <summary>
        /// Renderable's order in the queue
        /// </summary>
        private ulong mRenderingOrder;

        /// <summary>
        /// Renderable unit
        /// </summary>
        private IRenderableUnit mRenderableUnit;

        /// <summary>
        /// Pass that should be used for rendering
        /// </summary>
        private MaterialPass mMaterialPass;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the renderable unit's order in the queue
        /// </summary>
        public ulong RenderingOrder
        {
            get { return mRenderingOrder; }
        }

        /// <summary>
        /// Gets the renderable unit
        /// </summary>
        public IRenderableUnit RenderableUnit
        {
            get { return mRenderableUnit; }
        }

        /// <summary>
        /// Gets the pass that should be used for rendering
        /// </summary>
        public MaterialPass MaterialPass
        {
            get { return mMaterialPass; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="renderableUnit">- Renderable unit</param>
        /// <param name="camera">- Currently active camera</param>
        /// <param name="preferDistanceOrder">- Whether should be distance order prefered over a by material grouping (distance is usually prefered when transparency is used)</param>
        /// <param name="materialPass">- Pass that should be used for rendering</param>
        public RenderQueueEntry(IRenderableUnit renderableUnit, MaterialPass materialPass, ICamera camera, bool preferDistanceOrder)
        {
            mRenderableUnit = renderableUnit;
            mMaterialPass = materialPass;
            uint renderableSqrDistanceToCameraBits = GetFloatBits(Vector3.DistanceSquared(renderableUnit.ParentRenderable.WorldBoundsCenter, camera.WorldPosition));
            if (preferDistanceOrder) //prefer distance order (for transparent objects)
            {
                mRenderingOrder = ((ulong)renderableSqrDistanceToCameraBits << 32) | (ulong)materialPass.RenderBatchNumber;
            }
            else //prefer material order
            {
                mRenderingOrder = ((ulong)materialPass.RenderBatchNumber << 32) | (ulong)renderableSqrDistanceToCameraBits;
            }
        }

        /// <summary>
        /// Gets the float in a raw bit representation
        /// </summary>
        /// <param name="num">- Float num</param>
        /// <returns>Raw bit representation</returns>
        private static uint GetFloatBits(float num)
        {
            unsafe
            {
                return *((uint*)&num);
            }
        }
        #endregion Methods

        #region IComparable

        /// <summary>
        /// Compares rendering order of this and other RenderQueueEntry
        /// </summary>
        /// <param name="other">- RenderQueueEntry that should be compared with this entry</param>
        /// <returns>-1 - this &lt; other | 1 - this &gt; other | 0 - this == other</returns>
        public int CompareTo(RenderQueueEntry other)
        {
            return Comparer<ulong>.Default.Compare(RenderingOrder, other.RenderingOrder);
        }

        #endregion IComparable
    }
}
