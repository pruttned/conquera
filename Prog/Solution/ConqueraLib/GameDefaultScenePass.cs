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
