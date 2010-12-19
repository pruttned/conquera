using Ale.Scene;

namespace Ale.Editor
{
    public class AleApplication : BaseApplication
    {
        protected override string GuiPaletteName
        {
            get { return "PaletteDef"; }
        }

        protected override Ale.Gui.CursorInfo DefaultCursor
        {
            get { return Ale.Gui.AlCursors.Default; }
        }

        public AleApplication(AleRenderControl renderControl)
            : base(renderControl, "AL.mod")
        {
        }

        public AleApplication()
            : base("AL.mod")
        {
            base.GraphicsDeviceManager.IsFullScreen = false;
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            return new DefaultScene(sceneManager, Content.DefaultContentGroup);
        }
    }
}