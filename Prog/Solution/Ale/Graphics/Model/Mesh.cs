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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.ObjectModel;
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Graphic mesh that contains of multiple MeshParts according to material groups.
    /// Mesh or MaterialPart doesn't contatin any material by its self. It is supposed that the appropriate material and transformations are
    /// activated before rendering of the MeshPart.
    /// </summary>
    public sealed class Mesh : IDisposable
    {
        #region Fields

        private VertexDeclaration mVertexDeclaration;
        private VertexBuffer mVertexBuffer;
        private IndexBuffer mIndexBuffer;
        private int mVertexStride;
        private BoundingSphere mBounds;
        private bool mIsDisposed = false;

        private MeshPartCollection mMeshParts;

        /// <summary>
        /// Indices of the geometry (Index buffer)
        /// </summary>
        private ShortIndexReadOnlyArray mIndices = null;

        /// <summary>
        /// Untransformed vertices of the geometry
        /// </summary>
        private SimpleVertexReadOnlyArray mVertices = null;

        private Skeleton mSkeleton;

        private SkeletalAnimationCollection mSkeletalAnimations;

        private MeshConnectionPointCollection mConnectionPoints;

        #endregion Fields

        #region Properties

        public BoundingSphere Bounds
        {
            get { return mBounds; }
        }

        public MeshPartCollection MeshParts
        {
            get { return mMeshParts; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return mVertexDeclaration.GraphicsDevice; }
        }

        /// <summary>
        /// Gets the indices of the geometry (Index buffer)
        /// </summary>
        internal ShortIndexReadOnlyArray Indices
        {
            get
            {
                if (null == mIndices)
                {//load indices
                    mIndices = ShortIndexReadOnlyArray.FromIndexBuffer(mIndexBuffer);
                }
                return mIndices;
            }
        }

        /// <summary>
        /// Gets the untransformed vertices of the geometry
        /// </summary>
        internal SimpleVertexReadOnlyArray Vertices
        {
            get
            {
                if (null == mVertices)
                {//load vertices
                    mVertices = SimpleVertexReadOnlyArray.FromVertexBuffer(mVertexBuffer, mVertexDeclaration);
                }
                return mVertices;
            }
        }

        public Skeleton Skeleton
        {
            get { return mSkeleton; }
        }

        public SkeletalAnimationCollection SkeletalAnimations
        {
            get { return mSkeletalAnimations; }
        }

        public MeshConnectionPointCollection ConnectionPoints
        {
            get { return mConnectionPoints; }
        }

        #endregion Properties

        #region Methods

        internal Mesh(ContentReader input)
        {
            mVertexDeclaration = input.ReadObject<VertexDeclaration>();
            mVertexBuffer = input.ReadObject<VertexBuffer>();
            mIndexBuffer = input.ReadObject<IndexBuffer>();
            mVertexStride = mVertexDeclaration.GetVertexStrideSize(0);
            mBounds = new BoundingSphere(input.ReadVector3(), input.ReadSingle());

            //Mesh parts
            int meshPartCnt = input.ReadInt32();
            MeshPart[] meshParts = new MeshPart[meshPartCnt];
            for (int i = 0; i < meshPartCnt; ++i)
            {
                meshParts[i] = new MeshPart(this, input);
            }
            mMeshParts = new MeshPartCollection(meshParts);

            //skeleton
            mSkeleton = Skeleton.Read(input);

            //skeletal animation
            mSkeletalAnimations = SkeletalAnimationCollection.Read(input);

            //Connection point
            mConnectionPoints = MeshConnectionPointCollection.Read(input, this);
        }

        internal Mesh(VertexDeclaration vertexDeclaration, VertexBuffer vb, IndexBuffer ib, List<MeshPart> meshParts, BoundingSphere bounds)
        {
            mVertexDeclaration = vertexDeclaration;
            mVertexBuffer = vb;
            mIndexBuffer = ib;
            mVertexStride = mVertexDeclaration.GetVertexStrideSize(0);
            mBounds = bounds;

            mMeshParts = new MeshPartCollection(meshParts.ToArray());
            foreach (MeshPart meshPart in mMeshParts)
            {
                meshPart.ParentMesh = this;
            }

            //skeletal anim is not suported for manual meshes now
            mSkeleton = null;
            mSkeletalAnimations = null;
            mConnectionPoints = null;
        }

        public void AddConnectionPoint(NameId name, NameId parentBoneName, Vector3 position, Quaternion orientation)
        {
            if (null == mConnectionPoints)
            {
                mConnectionPoints = new MeshConnectionPointCollection();
            }
            mConnectionPoints.Add(new MeshConnectionPoint(name, parentBoneName, position, orientation, this));
        }

        public void PrepareForRender()
        {
            GraphicsDevice.VertexDeclaration = mVertexDeclaration;
            GraphicsDevice.Vertices[0].SetSource(mVertexBuffer, 0, mVertexStride);
            GraphicsDevice.Indices = mIndexBuffer;
        }

        /// <summary>
        /// Gets the index of a first part with a given material group or -1
        /// </summary>
        /// <param name="material"></param>
        public int FindPartByMaterialGroup(NameId material)
        {
            for (int i = 0; i < mMeshParts.Count; ++i)
            {
                if (mMeshParts[i].MaterialGroup == material)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Dispose()
        {
            if (!mIsDisposed)
            {
                mVertexDeclaration.Dispose();
                mVertexBuffer.Dispose();
                mIndexBuffer.Dispose();

                GC.SuppressFinalize(this);
                mIsDisposed = true;
            }
        }


        #endregion Methods
    }
}
