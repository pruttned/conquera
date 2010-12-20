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
