using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Ale.Scene;

namespace Conquera
{
    public class GameDefaultScenePass : ScenePass
    {
        public GameDefaultScenePass(GameScene scene, ICamera mainCamera)
            : base("Default", scene, mainCamera, null)
        {
        }

        protected override void Clear(GraphicsDevice graphicsDevice)
        {
          //  graphicsDevice.Clear(Color.White);
        }

        //public override void Draw(GraphicsDevice graphicsDevice, Renderer renderer, Ale.AleGameTime gameTime, RenderTargetManager renderTargetManager)
        //{
        //    base.Draw(graphicsDevice, renderer, gameTime, renderTargetManager);
        //}


        //protected override void EnqueRenderableUnits(GraphicsDevice graphicsDevice, AleGameTime gameTime, Renderer renderer)
        //{
        //    base.EnqueRenderableUnits(graphicsDevice, gameTime, renderer);

        //    ((OctreeScene)Scene).Octree.EnqueDebugRenderables(Camera, renderer, gameTime);
        //}
    }
}
