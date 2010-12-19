using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Ale.Gui
{
    public class DummyCompositeControl : CompositeControl
    {
        GraphicElementRepositoryGroup mGroup = new GraphicElementRepositoryGroup();

        public DummyCompositeControl()
        {
            ClientAreaFillsWholeControl = false;
            ClientArea = new Rectangle(4, 11, 143, 141);
        }

        protected override void OnGuiManagerChanged()
        {
            Background = GuiManager.Palette.CreateGraphicElement("composite");

            //mGroup.Owner = this;
            //mGroup.Autosize = false;
            //mGroup.Size = Size;
            //GraphicElementRepository rep = new GraphicElementRepository(GuiManager.Palette.CreateGraphicElement("animation"));
            //GraphicElementRepository rep2 = new GraphicElementRepository(GuiManager.Palette.CreateGraphicElement("animation"));
            //mGroup.Repositories.Add(rep);
            //mGroup.Repositories.Add(rep2);

            //GraphicElementRepository rep3 = new GraphicElementRepository(GuiManager.Palette.CreateGraphicElement("dummyControl"));
            //mGroup.Repositories.Insert(0, rep3);

            base.OnGuiManagerChanged();
        }
    }
}
