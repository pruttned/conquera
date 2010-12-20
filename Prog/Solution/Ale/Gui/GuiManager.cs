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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Ale.Content;
using Ale.Input;
using Microsoft.Xna.Framework;
using Ale.Settings;

namespace Ale.Gui
{
    public class GuiManager
    {
        public event EventHandler ScreenSizeChanged;

        private Dictionary<string, GuiFont> mGuiFontCache = new Dictionary<string, GuiFont>();
        private ContentGroup mContent;
        private System.Drawing.SizeF mScreenSize;
        private MouseManager mMouseManager;
        private Control mControlUnderMouseOnLastUpdate = null;        
        private Control mMouseDownControl = null;
        private GraphicsDeviceManager mGraphicsDeviceManager;

        public static GuiManager Instance { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public AleGameTime GameTime { get; private set; }
        public Palette Palette { get; private set; }        
        public GuiScene ActiveScene { get; set; }
        public CursorInfo Cursor { get; set; }
        internal DragDropInfo DragDropInfo { get; private set; }

        public System.Drawing.SizeF ScreenSize
        {
            get { return mScreenSize; }
            set
            {
                if (mScreenSize != value)
                {
                    mScreenSize = value;

                    if (ScreenSizeChanged != null)
                    {
                        ScreenSizeChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        private GuiManager(GraphicsDeviceManager graphicsDeviceManager, Palette palette, ContentGroup content, MouseManager mouseManager)
        {
            GraphicsDevice device = graphicsDeviceManager.GraphicsDevice;
            mGraphicsDeviceManager = graphicsDeviceManager;
            ActiveScene = new DefaultGuiScene();
            SpriteBatch = new SpriteBatch(device);
            Palette = palette;
            mContent= content;
            DragDropInfo = new DragDropInfo();
            ScreenSize = new System.Drawing.SizeF(device.Viewport.Width, device.Viewport.Height);

            mMouseManager = mouseManager;
            mMouseManager.MouseButtonDown += new MouseManager.MouseButtonEventHandler(mMouseManager_MouseButtonDown);
            mMouseManager.MouseButtonUp += new MouseManager.MouseButtonEventHandler(mMouseManager_MouseButtonUp);

            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);
        }



        public static void Initialize(GraphicsDeviceManager graphicsDeviceManager, Palette palette, ContentGroup content, MouseManager mouseManager)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Already initialized.");
            }

            Instance = new GuiManager(graphicsDeviceManager, palette, content, mouseManager);
        }

        public void Draw(AleGameTime gameTime)
        {
            GameTime = gameTime;
            SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            //Controls.
            foreach (Control rootControl in ActiveScene.RootControls)
            {
                rootControl.Draw();
            }

            //Cursor.
            if (Cursor != null)
            {
                Point mouseLocation = new Point((int)mMouseManager.CursorPosition.X, (int)mMouseManager.CursorPosition.Y);
                Point cursorLocation = new Point(mouseLocation.X - Cursor.HotSpot.X, mouseLocation.Y - Cursor.HotSpot.Y);
                Cursor.GraphicElement.Draw(cursorLocation);
            }

            SpriteBatch.End();
        }

        public GuiFont GetGuiFont(string name)
        {
            GuiFont guiFont;
            if (!mGuiFontCache.TryGetValue(name, out guiFont))
            {
                guiFont = new GuiFont(mContent.Load<SpriteFont>(name));
                mGuiFontCache.Add(name, guiFont);
            }
            return guiFont;
        }

        public void Update(AleGameTime gameTime)
        {
            if (0.0f != mMouseManager.CursorPositionDelta.X || 0.0f != mMouseManager.CursorPositionDelta.Y) //mouse location changed
            {
                Control controlUnderMouse = GetControlUnderMouse();
                if (!CheckModal(controlUnderMouse))
                {
                    controlUnderMouse = null;
                }

                if (mControlUnderMouseOnLastUpdate != controlUnderMouse) //control under mouse changed
                {
                    if (DragDropInfo.Dragging) //in drag mode
                    {
                        if (null != mControlUnderMouseOnLastUpdate) //old - drag leave
                        {
                            mControlUnderMouseOnLastUpdate.OnDragLeave();
                        }
                        if (null != controlUnderMouse) //new - drag enter
                        {
                            controlUnderMouse.OnDragEnter();
                        }
                    }
                    else if (!IsAnyMouseButtonPressed())
                    {
                        if (null != mControlUnderMouseOnLastUpdate) //old - leave
                        {
                            mControlUnderMouseOnLastUpdate.OnMouseLeave();
                        }
                        if (null != controlUnderMouse) //new - enter
                        {
                            controlUnderMouse.OnMouseEnter();
                        }
                    }

                    mControlUnderMouseOnLastUpdate = controlUnderMouse;
                }
            }
        }

        private void mMouseManager_MouseButtonDown(MouseButton button, MouseManager mouseManager)
        {
            Control controlUnderMouse = GetControlUnderMouse();
            mMouseDownControl = CheckModal(controlUnderMouse) ? controlUnderMouse : null;

            if (null != mMouseDownControl)
            {
                mMouseDownControl.OnMouseDown(button, mouseManager);
            }
        }

        private void mMouseManager_MouseButtonUp(MouseButton button, MouseManager mouseManager)
        {
            Control controlUnderMouse = GetControlUnderMouse();
            if (!CheckModal(controlUnderMouse))
            {
                controlUnderMouse = null;
            }

            if (DragDropInfo.Dragging)
            {
                DragDropInfo.EndDrag();

                if (null != DragDropInfo.EventArgs.SourceControl)
                {
                    DragDropInfo.EventArgs.SourceControl.OnDragFinished();
                }

                if (null != controlUnderMouse)
                {
                    if (DragDropInfo.EventArgs.AllowDrop)
                    {
                        controlUnderMouse.OnDragDrop();
                    }

                    controlUnderMouse.OnMouseEnter();
                }
            }
            else if (null != mMouseDownControl)
            {
                mMouseDownControl.OnMouseUp(button, mouseManager);

                if (mMouseDownControl == controlUnderMouse)
                {
                    mMouseDownControl.OnClick(button, mouseManager);
                }
                else
                {
                    mMouseDownControl.OnMouseLeave();

                    if (null != controlUnderMouse)
                    {
                        controlUnderMouse.OnMouseEnter();
                    }
                }
            }
        }

        public void BeginDragDrop(object data, Control dragSourceControl)
        {
            DragDropInfo.BeginDrag(data, dragSourceControl);

            if (null != mControlUnderMouseOnLastUpdate)
            {
                mControlUnderMouseOnLastUpdate.OnMouseLeave();
            }
        }

        public Control GetControlUnderMouse()
        {
            Point mouseLocation = new Point((int)mMouseManager.CursorPosition.X, (int)mMouseManager.CursorPosition.Y);

            for (int i = ActiveScene.RootControls.Count - 1; i >= 0; i--)            
            {
                Control control = ActiveScene.RootControls[i].GetControl(mouseLocation);
                if (control != null)
                {
                    return control;
                }
            }
            return null;
        }

        private bool IsAnyMouseButtonPressed()
        {
            return mMouseManager.IsButtonDown(MouseButton.Left) || mMouseManager.IsButtonDown(MouseButton.Right) ||
                mMouseManager.IsButtonDown(MouseButton.Middle);
        }

        private bool CheckModal(Control control)
        {
            return ActiveScene.ModalControl == null || ActiveScene.ModalControl.ContainsRecursive(control);
        }

        private void Default_AppSettingsCommitted(IAppSettings settings)
        {
            if (settings is VideoSettings)
            {
                Ale.Gui.GuiManager.Instance.ScreenSize = new System.Drawing.SizeF(mGraphicsDeviceManager.GraphicsDevice.Viewport.Width, mGraphicsDeviceManager.GraphicsDevice.Viewport.Height);
            }
        }
    }
}
