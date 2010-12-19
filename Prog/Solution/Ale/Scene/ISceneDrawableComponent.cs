using System;
using System.Collections.Generic;
using System.Text;
using Ale.Graphics;

namespace Ale.Scene
{
    /// <summary>
    /// Enques renderables during scene pass rendering
    /// </summary>
    public interface ISceneDrawableComponent
    {
        void EnqueRenderableUnits(AleGameTime gameTime, Renderer renderer, ScenePass scenePass);
    }
}
