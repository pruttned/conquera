using System;

namespace Ale.Graphics
{
	public interface IRenderableListener
	{
        void OnEnqueRenderableUnits(Renderable renderable, Renderer renderer, AleGameTime gameTime);
        void OnWorldBoundsChanged(Renderable renderable);
        void OnVisibleChanged(Renderable renderable);
	}
}
