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
    internal class MovementAreaRenderable : GraphicModel
    {
        /// <summary>
        /// Corners around 0,0
        /// </summary>
        private static Vector3[] Corners;
        private static List<HexCell> Siblings = new List<HexCell>();

        const float ZPos = 0.01f;
        private bool mIsDisposed = false;

        static MovementAreaRenderable()
        {
            Corners = HexTerrainTileDesc.GetCorners();
            for (int i = 0; i < Corners.Length; ++i)
            {
                Vector3 vec = Corners[i];
                vec.Z = ZPos;
                Corners[i] = vec;
            }
        }

        internal static MovementAreaRenderable TryCreate(GraphicsDevice graphicsDevice, ContentGroup content, GameUnit unit)
        {
            Mesh mesh = BuildMesh(graphicsDevice, unit);
            if (null == mesh)
            {
                return null;
            }
            return new MovementAreaRenderable(mesh, content);
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

        private static Mesh BuildMesh(GraphicsDevice graphicsDevice, GameUnit unit)
        {
            //!!! vertex Normal is color

            bool isEmpty = true;

            Vector3 moveColor = Color.Green.ToVector3();
            Vector3 attackColor = Color.Red.ToVector3();

            MeshBuilder meshBuilder = new MeshBuilder(graphicsDevice);
            meshBuilder.SetCurrentSubMesh("m");

            Vector2 pos;
            if (!unit.HasMovedThisTurn)
            {
                int width = unit.GameScene.Terrain.Width;
                int height = unit.GameScene.Terrain.Height;
                Point index = new Point();
                for (; index.X < width; ++index.X)
                {
                    for (index.Y = 0; index.Y < height; ++index.Y)
                    {
                        if (unit.CanMoveTo(index))
                        {
                            HexTerrain.Get2DPosFromIndex(index, out pos);
                            AddHexCell(meshBuilder, moveColor, pos);
                            isEmpty = false;
                        }
                        else
                        {
                            if (!unit.HasAttackedThisTurn && unit.CanAttackTo(index))
                            {
                                HexTerrain.Get2DPosFromIndex(index, out pos);
                                AddHexCell(meshBuilder, attackColor, pos);
                                isEmpty = false;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!unit.HasAttackedThisTurn)
                {
                    Siblings.Clear();
                    unit.Cell.GetSiblings(Siblings);
                    foreach (var sibling in Siblings)
                    {
                        if (unit.CanAttackTo(sibling.Index))
                        {
                            HexTerrain.Get2DPosFromIndex(sibling.Index, out pos);
                            AddHexCell(meshBuilder, attackColor, pos);
                            isEmpty = false;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }

            if (isEmpty)
            {
                return null;
            }
            return meshBuilder.BuildMesh(true);
        }

        private static Material GetMaterial(ContentGroup content)
        {
            return content.Load<Material>("MovementAreaMat");
        }

        private static void AddHexCell(MeshBuilder meshBuilder, Vector3 color, Vector2 pos)
        {
            Vector3 cPos;
            SimpleVertex vert = new SimpleVertex(new Vector3(pos.X, pos.Y, ZPos), color, Vector2.Zero);

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

        private MovementAreaRenderable(Mesh mesh, ContentGroup content)
            : base(mesh, GetMaterial(content))
        {
        }
    }

}
