using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class DummyControl : Control
    {
        protected override void OnGuiManagerChanged()
        {
            if (null != GuiManager)
            {
                Background = GuiManager.Palette.CreateGraphicElement("dummyControl");            
            }

            base.OnGuiManagerChanged();
        }
    }
}
