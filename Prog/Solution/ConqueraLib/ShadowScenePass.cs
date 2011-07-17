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

        public ShadowScenePass(ICamera mainCamera, BaseScene scene, Vector3 lightDir, Plane groundPlane)
            : base(Name, scene, new ShadowOrthoCamera(mainCamera, lightDir, groundPlane), false)
        {
        }

        protected override AleRenderTarget CreateRenderTarget(IRenderTargetManager renderTargetManager)
        {
            PresentationParameters pp = renderTargetManager.GraphicsDeviceManager.GraphicsDevice.PresentationParameters;
            var rt = renderTargetManager.CreateRenderTarget("ShadowMap", pp.BackBufferWidth, pp.BackBufferHeight, 1, pp.BackBufferFormat, DepthFormat.Depth16);
            rt.Color = Color.White;
            return rt;
        }
    }
}
