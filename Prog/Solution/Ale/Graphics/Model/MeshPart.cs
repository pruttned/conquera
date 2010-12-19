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
