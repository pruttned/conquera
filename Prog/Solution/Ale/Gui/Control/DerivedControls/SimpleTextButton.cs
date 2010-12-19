using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class SimpleTextButton : SimpleControl
    {
        private TextElement mTextElement;

        public string Text
        {
            get { return mTextElement.Text; }
            set { mTextElement.Text = value; }
        }

        protected TextElement TextElement
        {
            get { return mTextElement; }
        }

        public SimpleTextButton(GuiFont font)
            :this(typeof(SimpleTextButton).Name, font)
        {
        }

        public SimpleTextButton(string text, GuiFont font)
            :this(text, font, Color.Black)
        {
        }

        public SimpleTextButton(string text, GuiFont font, Color color)
        {
            mTextElement = new TextElement(font, color);
            Text = text;
            MainGraphicElementRepository.Repositories.Add(new GraphicElementRepository(mTextElement));
        }
    }
}
