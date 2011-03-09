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
using Microsoft.Xna.Framework.Input;

namespace Ale.Gui
{
    public class GuiManager
    {
        public event EventHandler ScreenSizeChanged;
        
        private System.Drawing.SizeF mScreenSize;        
        private Control mControlUnderMouseOnLastUpdate = null;        
        private Control mMouseDownControl = null;
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private Control mControlUnderMouse = null;
        private GuiScene mActiveScene;

        public static GuiManager Instance { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public AleGameTime GameTime { get; private set; }        
        public CursorInfo Cursor { get; set; }
        public AleContentManager Content { get; private set; }
        internal DragDropInfo DragDropInfo { get; private set; }

        public GuiScene ActiveScene
        {
            get { return mActiveScene; }
            set
            {
                if (mActiveScene != value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("Cannot be null. You can set DefaultGuiScene.Instance instead.");
                    }
                    mActiveScene = value;
                }
            }
        }

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

        public MouseManager MouseManager { get; private set; }

        public bool HandlesMouse
        {
            get
            {
                return ActiveScene.ModalControl != null || 
                       DragDropInfo.Dragging || 
                       mMouseDownControl != null ||
                       (mControlUnderMouse != null && !IsAnyMouseButtonPressed());
            }
        }

        private GuiManager(GraphicsDeviceManager graphicsDeviceManager, AleContentManager content, MouseManager mouseManager)
        {
            GraphicsDevice device = graphicsDeviceManager.GraphicsDevice;
            mGraphicsDeviceManager = graphicsDeviceManager;
            Content = content;
            ActiveScene = DefaultGuiScene.Instance;
            SpriteBatch = new SpriteBatch(device);
            DragDropInfo = new DragDropInfo();
            ScreenSize = new System.Drawing.SizeF(device.Viewport.Width, device.Viewport.Height);

            MouseManager = mouseManager;

            AppSettingsManager.Default.AppSettingsCommitted += new AppSettingsManager.CommittedHandler(Default_AppSettingsCommitted);
        }

        public static void Initialize(GraphicsDeviceManager graphicsDeviceManager, AleContentManager content, MouseManager mouseManager)
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Already initialized.");
            }

            Instance = new GuiManager(graphicsDeviceManager, content, mouseManager);
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
                Point mouseLocation = new Point((int)MouseManager.CursorPosition.X, (int)MouseManager.CursorPosition.Y);
                Point cursorLocation = new Point(mouseLocation.X - Cursor.HotSpot.X, mouseLocation.Y - Cursor.HotSpot.Y);
                Cursor.GraphicElement.Draw(cursorLocation);
            }

            SpriteBatch.End();
        }

        public void Update(AleGameTime gameTime)
        {
            //if (0.0f != mMouseManager.CursorPositionDelta.X || 0.0f != mMouseManager.CursorPositionDelta.Y) //mouse location changed
            //{
                UpdateControlUnderMouse();

                if (mControlUnderMouseOnLastUpdate != mControlUnderMouse) //control under mouse changed
                {
                    if (DragDropInfo.Dragging) //in drag mode
                    {
                        if (null != mControlUnderMouseOnLastUpdate) //old - drag leave
                        {
                            mControlUnderMouseOnLastUpdate.OnDragLeave();
                        }
                        if (null != mControlUnderMouse) //new - drag enter
                        {
                            mControlUnderMouse.OnDragEnter();
                        }
                    }
                    else if (!IsAnyMouseButtonPressed())
                    {
                        if (null != mControlUnderMouseOnLastUpdate) //old - leave
                        {
                            mControlUnderMouseOnLastUpdate.OnMouseLeave();
                        }
                        if (null != mControlUnderMouse) //new - enter
                        {
                            mControlUnderMouse.OnMouseEnter();
                        }
                    }

                    mControlUnderMouseOnLastUpdate = mControlUnderMouse;
                }
            //}
        }

        public void HandleMouseDown(MouseButton button)        
        {
            UpdateControlUnderMouse();
            mMouseDownControl = mControlUnderMouse;

            if (null != mMouseDownControl)
            {
                mMouseDownControl.OnMouseDown(button, MouseManager);
            }
        }

        public bool HandleMouseUp(MouseButton button) //returns Handled
        {
            UpdateControlUnderMouse();
            
            if (DragDropInfo.Dragging)
            {
                DragDropInfo.EndDrag();

                if (null != DragDropInfo.EventArgs.SourceControl)
                {
                    DragDropInfo.EventArgs.SourceControl.OnDragFinished();
                }

                if (null != mControlUnderMouse)
                {
                    if (DragDropInfo.EventArgs.AllowDrop)
                    {
                        mControlUnderMouse.OnDragDrop();
                    }

                    mControlUnderMouse.OnMouseEnter();
                }
                mMouseDownControl = null;
                return true;
            }
            
            if (null != mMouseDownControl)
            {
                mMouseDownControl.OnMouseUp(button, MouseManager);

                if (mMouseDownControl == mControlUnderMouse)
                {
                    if (button == MouseButton.Left)
                    {
                        mMouseDownControl.OnClick();
                    }
                }
                else
                {
                    mMouseDownControl.OnMouseLeave();

                    if (null != mControlUnderMouse)
                    {
                        mControlUnderMouse.OnMouseEnter();
                    }
                }
                mMouseDownControl = null;
                return true;
            }
            mMouseDownControl = null;
            return false;
        }

        public void HandleKeyDown(Keys key)
        {
            if (ActiveScene != null)
            {
                ActiveScene.OnKeyDown(key);
            }
        }

        public void HandleKeyUp(Keys key)
        {
            if (ActiveScene != null)
            {
                ActiveScene.OnKeyUp(key);
            }
        }

        private void UpdateControlUnderMouse()
        {
            mControlUnderMouse = GetControlUnderMouse();
            if (!CheckModal(mControlUnderMouse))
            {
                mControlUnderMouse = null;
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
            Point mouseLocation = new Point((int)MouseManager.CursorPosition.X, (int)MouseManager.CursorPosition.Y);

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
            return MouseManager.IsButtonDown(MouseButton.Left) || MouseManager.IsButtonDown(MouseButton.Right) ||
                   MouseManager.IsButtonDown(MouseButton.Middle);
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
