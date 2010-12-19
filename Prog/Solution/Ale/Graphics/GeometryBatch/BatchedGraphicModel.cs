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
using Ale.Tools;

namespace Ale.Graphics
{
    /// <summary>
    /// Model that is batched in the GeomtryBatch
    /// </summary>
    internal class BatchedGraphicModel
    {
        #region Fields

        /// <summary>
        /// Position in the world space
        /// </summary>
        private Vector3 mWorldPosition;

        /// <summary>
        /// Scale in the world space
        /// </summary>
        private float mWorldScale;

        /// <summary>
        /// Orientation in the world space
        /// </summary>
        private Quaternion mWorldOrientation;

        /// <summary>
        /// Whether mWorldTransformation consist only of translation
        /// </summary>
        private bool mOnlyTranslation;

        /// <summary>
        /// Model parts
        /// </summary>
        private ReadOnlyArray<BatchedGraphicModelPart> mParts;

        /// <summary>
        /// Mesh
        /// </summary>
        private Mesh mMesh;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the world translation
        /// </summary>
        public Vector3 WorldTranslation
        {
            get { return mWorldPosition; }
        }

        /// <summary>
        /// Gets whether mWorldTransformation consist only of translation
        /// </summary>
        public bool OnlyTranslation
        {
            get { return mOnlyTranslation; }
        }

        /// <summary>
        /// Gets the model parts
        /// </summary>
        public ReadOnlyArray<BatchedGraphicModelPart> Parts
        {
            get { return mParts; }
        }
  
        /// <summary>
        /// Mesh
        /// </summary>
        public Mesh Mesh
        {
            get { return mMesh; }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Only local transformation is used
        /// </summary>
        /// <param name="model"></param>
        /// <param name="geometryBatch"></param>
        public BatchedGraphicModel(GraphicModel model)
        {
            mWorldPosition = model.Position;
            mWorldScale = model.Scale;
            mWorldOrientation = model.Orientation;

            mOnlyTranslation = (1.0f == mWorldScale && Quaternion.Identity == mWorldOrientation);
            mMesh = model.Mesh;

            int partCnt = model.GraphicModelParts.Count;
            BatchedGraphicModelPart[] parts = new BatchedGraphicModelPart[partCnt];
            for (int i = 0; i < partCnt; ++i)
            {
                parts[i] = new BatchedGraphicModelPart(model.GraphicModelParts[i], this);
            }

            mParts = new ReadOnlyArray<BatchedGraphicModelPart>(parts, false);
        }


        /// <summary>
        /// Gets the world transformation matrix (not cached - because of memory saving)
        /// </summary>
        /// <param name="worldTransformation"></param>
        public void ComputeWorldTransformation(out Matrix worldTransformation)
        {
            if (1.0f != mWorldScale)
            {
                Matrix.CreateScale(mWorldScale, out worldTransformation);
            }
            else
            {
                worldTransformation = Matrix.Identity;
            }

            if (mWorldOrientation != Quaternion.Identity)
            {
                Matrix.Transform(ref worldTransformation, ref mWorldOrientation, out worldTransformation);
            }
            worldTransformation.Translation = mWorldPosition;
        }

        /// <summary>
        /// Gets the bounds in the world space
        /// </summary>
        public void ComputeWorldBounds(out BoundingSphere worldBounds)
        {
            worldBounds = Mesh.Bounds;

            AleMathUtils.TransformBoundingSphere(ref mWorldPosition, mWorldScale, ref worldBounds, out worldBounds);


            //if (OnlyTranslation)
            //{
            //    worldBounds = Mesh.Bounds;
            //    worldBounds.Center += mWorldPosition;
            //}
            //else
            //{
            //    Matrix worldTransformation;
            //    ComputeWorldTransformation(out worldTransformation);
            //    Mesh.Bounds.Transform(ref worldTransformation, out worldBounds);
            //}
        }

        #endregion Methods
    }
}
