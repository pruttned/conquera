using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class Dialog : Control
    {
        public void Show(bool modal)
        {
            System.Drawing.SizeF screenSize = GuiManager.Instance.ScreenSize;
            Location = new Point((int)(screenSize.Width / 2 - Size.Width / 2), (int)(screenSize.Height / 2 - Size.Height / 2));

            GuiManager.Instance.ActiveScene.RootControls.Add(this);
            
            if (modal)
            {
                GuiManager.Instance.ActiveScene.ModalControl = this;
            }
        }

        public void Hide()
        {
            SiblingColleciton.Remove(this);
        }
    }
}
