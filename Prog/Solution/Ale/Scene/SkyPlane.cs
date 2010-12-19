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

        public void EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ScenePass scenePass)
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
