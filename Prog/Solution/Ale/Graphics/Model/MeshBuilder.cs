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
    public class MeshBuilder
    {
        #region Types

        class SubMesh
        {
            public List<SimpleVertex> Vertices = new List<SimpleVertex>(20);
            public List<ushort> Indices = new List<ushort>(20);
        }

        #endregion Types

        private Dictionary<string, SubMesh> mSubMeshes = new Dictionary<string, SubMesh>();

        private string mCurrentSubMeshMaterialGroup = null;
        private SubMesh mCurrentSubMesh = null;
        private GraphicsDevice mGraphicsDevice;

        public MeshBuilder(GraphicsDevice graphicsDevice)
        {
            mGraphicsDevice = graphicsDevice;
        }

        public void SetCurrentSubMesh(string materialGroup)
        {
            if (string.IsNullOrEmpty(materialGroup)) throw new ArgumentNullException("materialGroup");

            mCurrentSubMeshMaterialGroup = materialGroup;

            mCurrentSubMesh = GetSubMesh(materialGroup);
        }

        public int AddVertex(SimpleVertex vertex)
        {
            return AddVertex(ref vertex);
        }

        public int AddVertex(ref SimpleVertex vertex)
        {
            ChcekCurrentSubMeshSet();

            if (mCurrentSubMesh.Vertices.Count == ushort.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Max number of vertices reached");
            }

            mCurrentSubMesh.Vertices.Add(vertex);

            return mCurrentSubMesh.Vertices.Count - 1;
        }

        public int FindVertex(ref SimpleVertex vertex, float epsilon)
        {
            ChcekCurrentSubMeshSet();

            for (int i = 0; i < mCurrentSubMesh.Vertices.Count; ++i)
            {
                if (vertex.Equals(mCurrentSubMesh.Vertices[i], epsilon))
                {
                    return i;
                }
            }
            return -1;
        }

        public int FindVertex(ref SimpleVertex vertex)
        {
            ChcekCurrentSubMeshSet();

            for (int i = 0; i < mCurrentSubMesh.Vertices.Count; ++i)
            {
                if (mCurrentSubMesh.Vertices[i].Equals(ref vertex))
                {
                    return i;
                }
            }
            return -1;
        }

        public int EnsureVertex(ref SimpleVertex vertex, float epsilon)
        {
            ChcekCurrentSubMeshSet();

            int index = FindVertex(ref vertex, epsilon);
            if (0 > index)
            {
                return AddVertex(ref vertex);
            }
            return index;
        }
        
        public int EnsureVertex(SimpleVertex vertex, float epsilon)
        {
            return EnsureVertex(ref vertex, epsilon);
        }

        public void AddFace(int v1, int v2, int v3)
        {
            int vCnt = mCurrentSubMesh.Vertices.Count;

            if (v1 >= vCnt || v2 >= vCnt || v3 >= vCnt)
            {
                throw new ArgumentOutOfRangeException("Vertex index");
            }

            mCurrentSubMesh.Indices.Add((ushort)v1);
            mCurrentSubMesh.Indices.Add((ushort)v2);
            mCurrentSubMesh.Indices.Add((ushort)v3);
        }

        public Mesh BuildMesh()
        {
            return BuildMesh(false);
        }

        public Mesh BuildMesh(bool usePool)
        {
            SimpleVertexList vertexList = new SimpleVertexList(100);
            ShortIndexList indexList = new ShortIndexList(100);

            List<MeshPart> meshParts = new List<MeshPart>();

            foreach (KeyValuePair<string, SubMesh> submesh in mSubMeshes)
            {
                string materialGroup = submesh.Key;
                SubMesh subMesh = submesh.Value;

                int baseVertex = vertexList.Count;
                int startIndex = indexList.Count;
                int numVertices = subMesh.Vertices.Count;
                int primitiveCnt = subMesh.Indices.Count / 3;

                vertexList.AddRange(subMesh.Vertices, 0, subMesh.Vertices.Count);
                indexList.AddRange(subMesh.Indices, 0, subMesh.Indices.Count, 0);

                MeshPart meshPart = new MeshPart(baseVertex, numVertices, primitiveCnt, startIndex, materialGroup);
                meshParts.Add(meshPart);
            }

            VertexDeclaration vertexDeclaration = new VertexDeclaration(mGraphicsDevice, SimpleVertex.VertexElements);
            BoundingSphere bounds = vertexList.ComputeBounds();
            // bounds.Radius *= 2; - This is setable in graphic model

            mSubMeshes.Clear();
            mCurrentSubMesh = null;

            return new Mesh(vertexDeclaration, vertexList.CreateVertexBuffer(mGraphicsDevice, usePool), indexList.CreateIndexBuffer(mGraphicsDevice, usePool),
                meshParts, bounds);
        }

        public void RecalculateNormals()
        {
            //based on http://www.riemers.net/eng/Tutorials/XNA/Csharp/ShortTuts/Normal_generation.php
            foreach (KeyValuePair<string, SubMesh> submesh in mSubMeshes)
            {
                SubMesh subMesh = submesh.Value;
                List<ushort> indices = subMesh.Indices;
                List<SimpleVertex> vertices = subMesh.Vertices;

                for (int i = 0; i < vertices.Count; ++i)
                {
                    Vector3 normal = Vector3.Zero;
                    SetNormal(vertices, i, ref normal);
                }

                for (int i = 0; i < indices.Count; i += 3)
                {
                    Vector3 vector4 = vertices[indices[i]].Position;
                    Vector3 vector = vertices[indices[i + 1]].Position;
                    Vector3 vector3 = vertices[indices[i + 2]].Position;
                    Vector3 normal = Vector3.Cross(vector3 - vector, vector - vector4);
                    normal.Normalize();

                    for (int j = 0; j < 3; ++j)
                    {
                        SetNormal(vertices, indices[i + j], ref normal);
                    }
                }

                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector3 normal = vertices[i].Normal;
                    normal.Normalize();
                    SetNormal(vertices, i, ref normal);
                }
            }
        }

        private void SetNormal(IList<SimpleVertex> vertices, int index, ref Vector3 normal)
        {
            // vertices[i].Normal doesn't work because SimpleVertex is struct

            SimpleVertex v = vertices[index];
            v.Normal = normal;
            vertices[index] = v;
        }

        private static Vector3 SafeNormalize(Vector3 value)
        {
            float num = value.Length();
            if (num == 0f)
            {
                return Vector3.Zero;
            }
            return (Vector3)(value / num);
        }


        private void ChcekCurrentSubMeshSet()
        {
            if (null == mCurrentSubMesh)
            {
                throw new InvalidOperationException("You must set SubMesh before adding vertices");
            }
        }

        private SubMesh GetSubMesh(string materialGroup)
        {
            SubMesh subMesh;
            if (!mSubMeshes.TryGetValue(materialGroup, out subMesh))
            {
                subMesh = new SubMesh();
                mSubMeshes.Add(materialGroup, subMesh);
            }

            return subMesh;
        }

    }
}
