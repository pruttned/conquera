using Microsoft.Xna.Framework.Graphics;
using Ale.Gui;

namespace Conquera.Gui
{
    public class MegaDebugLabel : Control
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        public override System.Drawing.SizeF Size
        {
            get { return new System.Drawing.SizeF(800.0f, 600.0f); }
        }

        public MegaDebugLabel()
        {
            mTextElement = new TextElement(800, 600, GuiManager.Instance.GetGuiFont("SpriteFont1"), true, Color.White);
            Text = "Praise the Emperor! I am visible!";
        }

        protected override void OnDrawForeground()
        {
            mTextElement.Draw(ScreenLocation);
        }
    }
}
