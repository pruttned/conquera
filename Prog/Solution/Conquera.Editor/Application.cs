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
using System.IO;
using Ale.Settings;

namespace Conquera.Editor
{
    public class EditorApplication : BaseApplication
    {
        private ToolBarForm mToolBarForm = new ToolBarForm();
        private EditorScene mEditorScene;

        public CommandQueue CommandQueue { get; private set; }
        
        public GameScene GameScene
        {
            get
            {
                return mEditorScene.GameScene;
            }
            set
            {
                mEditorScene.GameScene = value;
            }
        }

        protected override string GuiPaletteName
        {
            get { return "PaletteDef"; }
        }

        protected override CursorInfo DefaultCursor
        {
            get { return null; }
        }

        public EditorApplication()
            : base(null, "Conquera.mod")
        {
            CommandQueue = new CommandQueue();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                mToolBarForm.Dispose();
            }

            base.Dispose(isDisposing);
        }

        protected override BaseScene CreateDefaultScene(SceneManager sceneManager)
        {
            mEditorScene = new EditorScene(new HotseatGameScene("TestMap", sceneManager, 20,20, "Grass1Tile", Content.DefaultContentGroup));
            return mEditorScene;
        }

        protected override void OnInit()
        {
            mToolBarForm = new ToolBarForm();
            mToolBarForm.EditorApplication = this;
            mToolBarForm.RenderWindow = RenderWindow;
            mToolBarForm.Show();
            ShowSysCursor = true;

            base.OnInit();

            GuiManager.Instance.Cursor = null;
        }

        protected override void Update(AleGameTime gameTime)
        {
            ICommand command = null;
            while(null != (command = CommandQueue.Dequeue()))
            {
                command.Execute(this);
            }

            base.Update(gameTime);
        }
    }
}
