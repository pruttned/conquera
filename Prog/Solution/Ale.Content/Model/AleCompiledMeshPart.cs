using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace Ale.Content
{
    public class AleCompiledMeshPart
    {
        #region Fields

        private int mBaseVertex;
        private int mNumVertices;
        private int mPrimitiveCount;
        private int mStartIndex;
        private string mMaterialGroup;

        #endregion Fields

        #region Properties

        public int BaseVertex
        {
            get { return mBaseVertex; }
        }

        public int NumVertices
        {
            get { return mNumVertices; }
        }

        public int PrimitiveCount
        {
            get { return mPrimitiveCount; }
        }

        public int StartIndex
        {
            get { return mStartIndex; }
        }
        
        public string MaterialGroup
        {
            get { return mMaterialGroup; }
        }

        #endregion Properties

        #region Methods

        public AleCompiledMeshPart(int baseVertex, int numVertices, int primitiveCount, int startIndex, string MaterialGroup)
        {
            mBaseVertex = baseVertex;
            mNumVertices = numVertices;
            mPrimitiveCount = primitiveCount;
            mStartIndex = startIndex;
            mMaterialGroup = MaterialGroup;
        }

        public void Write(ContentWriter output)
        {
            output.Write(BaseVertex);
            output.Write(NumVertices);
            output.Write(PrimitiveCount);
            output.Write(StartIndex);
            output.Write(mMaterialGroup);
        }

        #endregion Methods
    }
}
