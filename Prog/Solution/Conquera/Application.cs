using System;
using System.Collections.Generic;
using System.Text;
using Ale;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using Ale.Graphics;
using Ale.Input;
using System.Diagnostics;
using Ale.Tools;
using Ale.Scene;
using Microsoft.Xna.Framework.Content;
using Ale.Content;
using Ale.Gui;
using Conquera.Gui;

namespace Conquera
{
    class Application : BaseApplication
    {
        protected override string GuiPaletteName
        {
            get { return "PaletteDef"; }
        }

        protected override CursorInfo DefaultCursor
        {
            get { return AlCursors.Default; }
        }

        public Application()
            : this(null)
        {
        }

        public Application(AleRenderControl renderControl)
            : base(renderControl, "Conquera.mod")
        {
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            return GameScene.Load("TestMap", sceneManager, Content.DefaultContentGroup);
            //return new GameScene("TestMap", sceneManager, 20,20, "Grass1Tile", Content.DefaultContentGroup);
        }
    }
}
