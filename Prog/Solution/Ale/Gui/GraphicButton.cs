using System;

namespace Ale.Gui
{
    public class GraphicButton : Control
    {
        private GraphicElement mDefaultGraphicElement;
        private GraphicElement mMouseOverGraphicElement;
        private GraphicElement mActiveGraphicElement;

        public override System.Drawing.SizeF Size
        {
            get { return mActiveGraphicElement.Size; }
        }

        public GraphicButton(GraphicElement defaultGraphicElement, GraphicElement mouseOverGraphicElement)
        {
            mDefaultGraphicElement = defaultGraphicElement;
            mMouseOverGraphicElement = mouseOverGraphicElement;
            mActiveGraphicElement = mDefaultGraphicElement;

            MouseEnter += new EventHandler<ControlEventArgs>(GraphicButton_MouseEnter);
            MouseLeave += new EventHandler<ControlEventArgs>(GraphicButton_MouseLeave);
        }

        protected override void OnDrawBackground()
        {
            mActiveGraphicElement.Draw(ScreenLocation);
        }

        private void GraphicButton_MouseEnter(object sender, ControlEventArgs e)
        {
            mActiveGraphicElement = mMouseOverGraphicElement;
        }

        private void GraphicButton_MouseLeave(object sender, ControlEventArgs e)
        {
            mActiveGraphicElement = mDefaultGraphicElement;
        }
    }
}
