//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
