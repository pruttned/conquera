using System;
using Ale.Content;

namespace Ale.Graphics
{
    public class GraphicModelRenderableFactory : IRenderableFactory
    {
        private RenderableProvider mRenderableProvider;

        public int Id
        {
            get { return 2; }
        }

        public string Name
        {
            get { return "GraphicModel"; }
        }
        
        public GraphicModelRenderableFactory(RenderableProvider renderableProvider)
        {
            mRenderableProvider = renderableProvider;
        }

        public Renderable CreateRenderable(string name, ContentGroup content)
        {
            GraphicModelDesc desc = content.Load<GraphicModelDesc>(name);
            return new GraphicModel(desc,mRenderableProvider, content);
        }

        public Renderable CreateRenderable(long id, ContentGroup content)
        {
            GraphicModelDesc desc = content.Load<GraphicModelDesc>(id);
            return new GraphicModel(desc, mRenderableProvider, content);
        }
    }
}
