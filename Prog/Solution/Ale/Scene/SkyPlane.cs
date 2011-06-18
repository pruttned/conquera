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
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Scene
{
    public sealed class SkyPlane : ISceneDrawableComponent, IDisposable
    {
        GraphicModel mGraphicsModel;
        public bool mIsDisposed = false;

        public SkyPlane(GraphicsDevice graphicsDevice, Material material, float height)
        {
            MeshBuilder meshBuilder = new MeshBuilder(graphicsDevice);
            meshBuilder.SetCurrentSubMesh("m1");

            int v1 = meshBuilder.AddVertex(new SimpleVertex(new Vector3(-1f, -1f, height), Vector3.UnitZ, new Vector2(0, 0)));
            int v2 = meshBuilder.AddVertex(new SimpleVertex(new Vector3(1f, -1f, height), Vector3.UnitZ, new Vector2(1, 0)));
            int v3 = meshBuilder.AddVertex(new SimpleVertex(new Vector3(-1f, 1f, height), Vector3.UnitZ, new Vector2(0, 1)));
            int v4 = meshBuilder.AddVertex(new SimpleVertex(new Vector3(1f, 1f, height), Vector3.UnitZ, new Vector2(1, 1)));


            if (0 > height)
            {
                meshBuilder.AddFace(v3, v2, v1);
                meshBuilder.AddFace(v3, v4, v2);
            }
            else
            {
                meshBuilder.AddFace(v1, v2, v3);
                meshBuilder.AddFace(v2, v4, v3);
            }

            meshBuilder.RecalculateNormals();

            mGraphicsModel = new GraphicModel(meshBuilder.BuildMesh(), material);
        }

        #region ISceneDrawableComponent Members

        public void EnqueRenderableUnits(AleGameTime gameTime, IRenderer renderer, ScenePass scenePass)
        {
            mGraphicsModel.EnqueRenderableUnits(renderer, gameTime);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (!mIsDisposed)
            {
                GC.SuppressFinalize(this);

                mGraphicsModel.Dispose();

                mIsDisposed = true;
            }
        }
    }
}
