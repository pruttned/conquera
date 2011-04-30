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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;


namespace Conquera
{
    public class HexAreaRenderableCell
    {
        public Point Index { get; set; }
        public Vector4 Color { get; set; }

        public HexAreaRenderableCell()
        {
        }

        public HexAreaRenderableCell(Point index, Vector4 color)
        {
            Color = color;
            Index = index;
        }
    }


    public class HexAreaRenderable : GraphicModel
    {//!!! vertex Normal is color
        //!!! vertex Uv.X is alpha

        /// <summary>
        /// Corners around 0,0
        /// </summary>
        private static Vector3[] Corners;

        const float ZPos = 0.01f;
        private bool mIsDisposed = false;

        static HexAreaRenderable()
        {
            Corners = HexHelper.GetHexCellCorners();
            for (int i = 0; i < Corners.Length; ++i)
            {
                Vector3 vec = Corners[i];
                vec.Z = ZPos;
                Corners[i] = vec;
            }
        }

        public HexAreaRenderable(GraphicsDevice graphicsDevice, IList<HexAreaRenderableCell> cells, ContentGroup content)
            : base(BuildMesh(graphicsDevice, cells), GetDefaultMaterial(content))
        {
        }

        public HexAreaRenderable(GraphicsDevice graphicsDevice, IList<HexAreaRenderableCell> cells, Material material)
            : base(BuildMesh(graphicsDevice, cells), material)
        {
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    Mesh.Dispose();
                }
                mIsDisposed = true;
            }

            base.Dispose(isDisposing);
        }

        private static Material GetDefaultMaterial(ContentGroup content)
        {
            return content.Load<Material>("HexAreaMat");
        }

        private static Mesh BuildMesh(GraphicsDevice graphicsDevice, IList<HexAreaRenderableCell> cells)
        {
            MeshBuilder meshBuilder = new MeshBuilder(graphicsDevice);
            meshBuilder.SetCurrentSubMesh("m");

            foreach (var cell in cells)
            {
                AddHexCell(meshBuilder, cell);
            }

            return meshBuilder.BuildMesh(true);
        }

        private static void AddHexCell(MeshBuilder meshBuilder, HexAreaRenderableCell cell)
        {
            Vector2 pos = HexHelper.Get2DPosFromIndex(cell.Index);
            Vector3 cPos;
            Vector4 color = cell.Color;
            SimpleVertex vert = new SimpleVertex(new Vector3(pos.X, pos.Y, ZPos), new Vector3(color.X, color.Y, color.Z), new Vector2(color.W, 0));

            int vcI = meshBuilder.AddVertex(ref vert);
            cPos = Corners[0];
            cPos.X += pos.X;
            cPos.Y += pos.Y;
            vert.Position = cPos;

            int vOldI = meshBuilder.AddVertex(ref vert);
            int vFirstI = vOldI;
            for (int i = 1; i < 6; ++i)
            {
                cPos = Corners[i];
                cPos.X += pos.X;
                cPos.Y += pos.Y;
                vert.Position = cPos;

                int vI = meshBuilder.AddVertex(ref vert);

                meshBuilder.AddFace(vOldI, vI, vcI);

                vOldI = vI;
            }

            meshBuilder.AddFace(vOldI, vFirstI, vcI);
        }
    }


}
