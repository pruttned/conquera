//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Ale.Graphics;
//using Microsoft.Xna.Framework.Graphics;
//using Ale.Content;

//namespace Conquera
//{
//    class WaterReflectionPass : ScenePass
//    {
//        public static readonly string Name = "WaterReflectionPass";
//        ICamera mMainCamera;

//        public WaterReflectionPass(ICamera mainCamera, BaseScene scene, RenderTargetManager renderTargetManager, ContentGroup content)
//            : base(Name, scene, CreateReflectionCamera(mainCamera),
//                CreateRenderTarget(renderTargetManager))
//        {
//            mMainCamera = mainCamera;
//        }

//        private static ICamera CreateReflectionCamera(ICamera mainCamera)
//        {
//            return new WaterReflectionCamera((Camera)mainCamera);
//        }

//        static private AleRenderTarget CreateRenderTarget(RenderTargetManager renderTargetManager)
//        {
//            PresentationParameters pp = renderTargetManager.GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
//            return renderTargetManager.CreateRenderTarget("ReflectionMap", 1024, 1024, 1, pp.BackBufferFormat, DepthFormat.Depth16);
//        }

//        protected override void Clear(GraphicsDevice graphicsDevice)
//        {
//            graphicsDevice.Clear(Color.White);
//        }
//    }
//}
