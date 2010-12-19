using System;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    /// <summary>
    /// Dynamic geometry represented by multiple quads that must be initialized each frame by calling DynamicGeometryManager.AllocGeometry.
    /// 
    /// By using transient geometry instead allocating vb and ib for each geometry it is possible to reuse fewer vbs and ibs among
    /// multiple geometries, because usually not all allocated geometries are rendered in each frame (some of them are outside of the camera's frustum).
    ///
    /// Transient geometry is faster then DrawUser*Primitives.
    /// </summary>
    /// <typeparam name="T">Vertex type</typeparam>
    public class TransientQuadGeometry<T> where T : struct
    {
        #region Fields

        /// <summary>
        /// Index of the first vertex
        /// </summary>
        private int mBaseVertex = -1;

        /// <summary>
        /// Number of verticies
        /// </summary>
        private int mNumVertices = -1;

        /// <summary>
        /// Quad buffer that has initialized the the geometry
        /// </summary>
        private QuadBuffer<T> mQuadBuffer = null;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initializes the geometery. This method should be called only by QuadBuffer class
        /// </summary>
        /// <param name="baseVertex">- Index of the first vertex</param>
        /// <param name="numVertices">- Number of verticies</param>
        /// <param name="quadBuffer">- Quad buffer that has called this method</param>
        internal void Init(int baseVertex, int numVertices, QuadBuffer<T> quadBuffer)
        {
            mBaseVertex = baseVertex;
            mNumVertices = numVertices;
            mQuadBuffer = quadBuffer;
        }

        /// <summary>
        /// Draws the geometry. Dont't forget to initialize the geometry each frame by calling DynamicGeometryManager.AllocGeometry
        /// </summary>
        /// <exception cref="InvalidOperationException">- TransientQuadGeometry was not initialized by calling DynamicGeometryManager.AllocGeometry method</exception>
        public void Draw()
        {
            if (mBaseVertex < 0)
            {
                throw new InvalidOperationException("You must initialize the TransientQuadGeometry by calling DynamicGeometryManager.AllocGeometry method before you can render it");
            }

            mQuadBuffer.PrepareForRender();
            mQuadBuffer.GraphicsDeviceManager.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mBaseVertex, 0, mNumVertices, 0, mNumVertices / 2);
        }

        #endregion Methods
    }
}
