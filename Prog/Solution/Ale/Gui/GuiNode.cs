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
using System.Collections.ObjectModel;
using Ale.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Gui
{
    public abstract class GuiNode
    {
        /// <summary>
        /// Mouse pressed over this node.
        /// </summary>
        public event MouseEventHandler MouseDown;

        /// <summary>
        /// This node was under the mouse on last mouse down and the mouse was now released over this, other or no node.
        /// </summary>
        public event MouseEventHandler MouseReleased;

        /// <summary>
        /// Mouse released over this node.
        /// </summary>
        public event MouseEventHandler MouseUp;

        /// <summary>
        /// Mouse click on this node.
        /// </summary>
        public event MouseEventHandler Click;

        /// <summary>
        /// Mouse left this node. Mouse buttons do not affect this event.
        /// </summary>
        public event MouseEventHandler MouseLeave;

        /// <summary>
        /// Mouse entered this node. Mouse buttons do not affect this event.
        /// </summary>
        public event MouseEventHandler MouseEnter;

        private List<GuiNode> mChildrenPrivate = new List<GuiNode>();
        private Point mPosition = Point.Zero;

        public GuiNode Parent { get; private set; }
        public ReadOnlyCollection<GuiNode> Children { get; private set; }
        public abstract int Width { get; }
        public abstract int Height { get; }
        public Point PositionOnScreen { get; private set; }
        public bool IsVisible { get; set; }
        public bool IsHitTestEnabled { get; set; }

        public Point Position
        {
            get { return mPosition; }
            set
            {
                if (value != mPosition)
                {
                    mPosition = value;
                    UpdatePositionOnScreen();
                }
            }
        }

        public int LeftOnScreen
        {
            get { return PositionOnScreen.X; }
        }

        public int RightOnScreen
        {
            get { return PositionOnScreen.X + Width - 1; }
        }

        public int TopOnScreen
        {
            get { return PositionOnScreen.Y; }
        }

        public int BottomOnScreen
        {
            get { return PositionOnScreen.Y + Height - 1; }
        }

        public GuiNode()
        {
            Children = new ReadOnlyCollection<GuiNode>(mChildrenPrivate);
            IsVisible = true;
            IsHitTestEnabled = true;
        }

        public void AddChild(GuiNode node)
        {
            if(node == null) throw new ArgumentNullException("node");
            if (node.Parent != null) throw new ArgumentException("The specified node already has a parent.");

            node.Parent = this;
            mChildrenPrivate.Add(node);
            node.UpdatePositionOnScreen();
        }

        public bool RemoveChild(GuiNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            if (node.Parent != this)
            {
                return false;
            }

            node.Parent = null;
            mChildrenPrivate.Remove(node);
            node.UpdatePositionOnScreen();
            return true;
        }

        public void ClearChildren()
        {
            foreach (GuiNode child in Children)
            {
                child.Parent = null;
                child.UpdatePositionOnScreen();
            }
            mChildrenPrivate.Clear();
        }

        public GuiNode PerformHitTest(Point positionOnScreen)
        {
            if (IsVisible && IsHitTestEnabled && OccupiesPositionOnScreen(positionOnScreen))
            {
                for(int i = Children.Count - 1; i >= 0; i--)
                {
                    GuiNode hitChild = Children[i].PerformHitTest(positionOnScreen);
                    if (hitChild != null)
                    {
                        return hitChild;
                    }
                }
                return this;
            }
            return null;
        }

        public bool OccupiesPositionOnScreen(Point positionOnScreen)
        {
            return positionOnScreen.X >= LeftOnScreen && positionOnScreen.X <= RightOnScreen &&
                   positionOnScreen.Y >= TopOnScreen && positionOnScreen.Y <= BottomOnScreen;
        }

        public void Draw(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            if (IsVisible)
            {
                DrawImpl(spriteBatch, gameTime);
            }
        }
        
        internal void AcceptMouseDown(IMouseManager mouseManager)
        {
            OnMouseDown(mouseManager);
            EventHelper.RaiseEvent(MouseDown, this, mouseManager);
        }

        internal void AcceptMouseReleased(IMouseManager mouseManager)
        {
            OnMouseReleased(mouseManager);
            EventHelper.RaiseEvent(MouseReleased, this, mouseManager);
        }

        internal void AcceptMouseUp(IMouseManager mouseManager)
        {
            OnMouseUp(mouseManager);
            EventHelper.RaiseEvent(MouseUp, this, mouseManager);
        }

        internal void AcceptClick(IMouseManager mouseManager)
        {
            OnClick(mouseManager);
            EventHelper.RaiseEvent(Click, this, mouseManager);
        }

        internal void AcceptMouseEnter(IMouseManager mouseManager)
        {
            OnMouseEnter(mouseManager);
            EventHelper.RaiseEvent(MouseEnter, this, mouseManager);
        }

        internal void AcceptMouseLeave(IMouseManager mouseManager)
        {
            OnMouseLeave(mouseManager);
            EventHelper.RaiseEvent(MouseLeave, this, mouseManager);
        }

        protected virtual void DrawImpl(SpriteBatch spriteBatch, AleGameTime gameTime)
        {
            foreach (GuiNode child in Children)
            {
                child.Draw(spriteBatch, gameTime);
            }
        }

        protected virtual void OnMouseDown(IMouseManager mouseManager) { }
        protected virtual void OnMouseReleased(IMouseManager mouseManager) { }
        protected virtual void OnMouseUp(IMouseManager mouseManager) { }
        protected virtual void OnClick(IMouseManager mouseManager) { }
        protected virtual void OnMouseEnter(IMouseManager mouseManager) { }
        protected virtual void OnMouseLeave(IMouseManager mouseManager) { }
        
        private void UpdatePositionOnScreen()
        {
            int parentPositionOnScreenX = Parent != null ? Parent.PositionOnScreen.X : 0;
            int parentPositionOnScreenY = Parent != null ? Parent.PositionOnScreen.Y : 0;

            PositionOnScreen = new Point(parentPositionOnScreenX + Position.X, parentPositionOnScreenY + Position.Y);

            foreach (GuiNode child in Children)
            {
                child.UpdatePositionOnScreen();
            }
        }        
    }
}