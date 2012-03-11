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
using Ale.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public class GuiRoot : GuiNode, IDisposable
    {
        private GraphicsDeviceManager mGraphicsDeviceManager;
        private IMouseManager mMouseManager;
        private GuiNode mMouseDownNode;
        private GuiNode mMouseOverNode;
        private SpriteBatch mSpriteBatch;
        private bool mIsDisposed;

        public override int Width
        {
            get { return mGraphicsDeviceManager.GraphicsDevice.Viewport.Width; }
        }

        public override int Height
        {
            get { return mGraphicsDeviceManager.GraphicsDevice.Viewport.Height; }
        }

        public bool IsActive { get; private set; }

        public GuiRoot(GraphicsDeviceManager graphicsDeviceManager, IMouseManager mouseManager)
        {
            mGraphicsDeviceManager = graphicsDeviceManager;
            mMouseManager = mouseManager;
            mSpriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
        }

        /// <summary>
        /// Turns event handling on.
        /// </summary>
        public void Activate()
        {
            if (!IsActive)
            {
                mMouseManager.MouseButtonDown += new MouseButtonEventHandler(mMouseManager_MouseButtonDown);
                mMouseManager.MouseButtonUp += new MouseButtonEventHandler(mMouseManager_MouseButtonUp);
                IsActive = true;
            }
        }

        /// <summary>
        /// Turns event handling off.
        /// </summary>
        public void Deactivate()
        {
            if (IsActive)
            {
                mMouseManager.MouseButtonDown -= mMouseManager_MouseButtonDown;
                mMouseManager.MouseButtonUp -= mMouseManager_MouseButtonUp;
                IsActive = false;
            }
        }

        public void Draw(AleGameTime gameTime)
        {
            mSpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
            Draw(mSpriteBatch, gameTime);
            mSpriteBatch.End();
        }

        protected override void DrawImpl(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            if (IsActive)
            {
                GuiNode nodeUnderMouse = PerformHitTest(GuiHelper.ConvertVectorToPoint(mMouseManager.CursorPosition));
                if (nodeUnderMouse != mMouseOverNode)
                {
                    if (mMouseOverNode != null)
                    {
                        mMouseOverNode.AcceptMouseLeave(mMouseManager);
                    }

                    mMouseOverNode = nodeUnderMouse;

                    if (mMouseOverNode != null)
                    {
                        mMouseOverNode.AcceptMouseEnter(mMouseManager);
                    }
                }
            }
            base.DrawImpl(spriteBatch, gameTime);
        }

        private void mMouseManager_MouseButtonDown(MouseButton button, IMouseManager mouseManager)
        {
            mMouseDownNode = PerformHitTest(GuiHelper.ConvertVectorToPoint(mouseManager.CursorPosition));
            if (mMouseDownNode != null)
            {
                mMouseDownNode.AcceptMouseDown(mouseManager);
            }
        }

        private void mMouseManager_MouseButtonUp(MouseButton button, IMouseManager mouseManager)
        {
            if (mMouseDownNode != null)
            {
                mMouseDownNode.AcceptMouseReleased(mouseManager);
            }

            GuiNode nodeUnderMouse = PerformHitTest(GuiHelper.ConvertVectorToPoint(mouseManager.CursorPosition));
            if (nodeUnderMouse != null)
            {
                nodeUnderMouse.AcceptMouseUp(mouseManager);

                if (mMouseDownNode == nodeUnderMouse)
                {
                    mMouseDownNode.AcceptClick(mouseManager);
                }
            }
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    mSpriteBatch.Dispose();
                    Deactivate();
                }
                mIsDisposed = true;
            }
        }

        #endregion IDisposable
    }
}
