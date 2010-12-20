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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;

namespace Ale.Graphics
{
    public class MeshPart
    {
        #region Fields
        
        private Mesh mParentMesh;
        private int mBaseVertex;
        private int mNumVertices;
        private int mPrimitiveCnt;
        private int mStartIndex;
        private NameId mMaterialGroup;

        #endregion Fields

        #region Properties

        public Mesh ParentMesh
        {
            get { return mParentMesh; }
            internal set { mParentMesh = value; }
        }

        public int BaseVertex
        {
            get {return mBaseVertex;}
        }

        public int NumVertices
        {
            get { return mNumVertices; }
        }
        
        public int PrimitiveCnt
        {
            get { return mPrimitiveCnt; }
        }

        public int StartIndex
        {
            get { return mStartIndex; }
        }

        public NameId MaterialGroup
        {
            get { return mMaterialGroup; }
        }

        #endregion Properties

        #region Methods

        internal MeshPart(Mesh parentMesh, ContentReader input)
        {
            mParentMesh = parentMesh;

            mBaseVertex = input.ReadInt32();
            mNumVertices = input.ReadInt32();
            mPrimitiveCnt = input.ReadInt32();
            mStartIndex = input.ReadInt32();
            mMaterialGroup = input.ReadString();
        }

        internal MeshPart(Mesh parentMesh, int baseVertex, int numVertices, int primitiveCnt, int startIndex, string materialGroup)
        {
            if(string.IsNullOrEmpty(materialGroup)) throw new ArgumentNullException("materialGroup");
                
            mParentMesh = parentMesh;
            mBaseVertex = baseVertex;
            mNumVertices = numVertices;
            mPrimitiveCnt = primitiveCnt;
            mStartIndex = startIndex;
            mMaterialGroup = materialGroup;  
        }

        internal MeshPart(int baseVertex, int numVertices, int primitiveCnt, int startIndex, string materialGroup)
            :this(null, baseVertex, numVertices, primitiveCnt, startIndex, materialGroup)
        {
        }

        public void Render()
        {
            mParentMesh.PrepareForRender();
            mParentMesh.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mBaseVertex, 0, mNumVertices, mStartIndex, mPrimitiveCnt);
        }

        #endregion Methods
    }
}
