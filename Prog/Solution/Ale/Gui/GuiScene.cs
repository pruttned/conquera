using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ale.Gui
{
    public class GuiScene
    {
        public ControlCollection RootControls { get; private set; }
        public Control ModalControl { get; set; }

        public GuiScene()
        {
            RootControls = new ControlCollection(null);
        }
    }

    public class DefaultGuiScene : GuiScene
    {
    }
}
