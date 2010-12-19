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
