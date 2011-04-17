//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

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
            : base(renderControl)
        {
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            return HotseatGameScene.Load("NewMap", sceneManager, Content.DefaultContentGroup);
         //   return new MainMenuScene(sceneManager, Content.DefaultContentGroup);
            //return new HotseatGameScene("TestMap", sceneManager, 15, 15, "Grass1Tile", Content.DefaultContentGroup);
        }

        protected override void OnInit()
        {
            base.OnInit();
        }
    }
}
