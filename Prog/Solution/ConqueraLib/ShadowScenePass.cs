using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Ale.Graphics;
using Microsoft.Xna.Framework;
using Ale.Content;
using Ale.Scene;

namespace Conquera
{
    class ShadowScenePass : ScenePass
    {
        public static readonly string Name = "ShadowPass";

        public ShadowScenePass(ICamera mainCamera, BaseScene scene, Vector3 lightDir, Plane groundPlane, RenderTargetManager renderTargetManager, ContentGroup content)
            : base(Name, scene, new ShadowOrthoCamera(mainCamera, lightDir, groundPlane),
                CreateRenderTarget(renderTargetManager))
        {
        }

        static private AleRenderTarget CreateRenderTarget(RenderTargetManager renderTargetManager)
        {
            PresentationParameters pp = renderTargetManager.GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            return renderTargetManager.CreateRenderTarget("ShadowMap", 1024, 1024, 1, pp.BackBufferFormat, DepthFormat.Depth16);
        }

        protected override void Clear(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.Clear(Color.White);
        }
    }

}
