using Ale.Content;
using Ale.Scene;
using Ale.Graphics;

namespace Ale.Editor
{
    [RenderInfo("Material")]
    public class MaterialRenderInfo : IRenderInfo
    {
        private BaseScene mScene = null;

        public BaseScene Scene
        {
            get { return mScene; }
        }

        public void Update(object assetSettings, SceneManager sceneManager, ContentGroup content)
        {
            if (Scene == null)
            {
                mScene = new MaterialScene(sceneManager, content);
            }
            ((MaterialScene)Scene).SetMaterialSettings((MaterialSettings)assetSettings);
        }
    }
}
