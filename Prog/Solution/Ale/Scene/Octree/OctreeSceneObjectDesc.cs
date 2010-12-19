using System;
using Ale.Content;
using Ale.Graphics;

namespace Ale.Scene
{
    [NonContentPipelineAsset(typeof(OctreeSceneObjectDescLoader))]
    public class OctreeSceneObjectDesc
    {
        public GraphicModelDesc GraphicModel { get; private set; }

        public OctreeSceneObjectDesc(OctreeSceneObjectSettings settings, ContentGroup content)
        {
            GraphicModel = content.Load<GraphicModelDesc>(settings.GraphicModel);
        }
    }
}
