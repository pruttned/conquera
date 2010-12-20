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
using Microsoft.Xna.Framework;
using Ale.Graphics;

namespace Ale.Scene
{
    public class OctreeSceneDrawableComponent : Octree, ISceneDrawableComponent
    {
        #region ISceneDrawableComponent Members
        private OctreeFilterStatistics mSceneQueuingStatistics;

        /// <summary>
        /// Gets the statistic of a last executed octree query in EnqueRenderableUnits method
        /// </summary>
        public OctreeFilterStatistics SceneQueuingStatistics
        {
            get { return mSceneQueuingStatistics; }
        }

        public OctreeSceneDrawableComponent(BoundingBox bounds)
            : base(bounds)
        {
        }

        public void EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ScenePass scenePass)
        {
            mSceneQueuingStatistics = ForEachObject(delegate(IOctreeObject obj)
            {
                Renderable renderable = obj as Renderable;
                if (null != renderable)
                {
                    renderable.EnqueRenderableUnits(renderer, gameTime);
                }
            }, scenePass.Camera);
        }

        #endregion
    }
}
