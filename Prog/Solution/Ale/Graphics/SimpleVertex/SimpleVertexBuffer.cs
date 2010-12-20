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
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    internal class SimpleVertexBuffer : VertexBuffer, IPoolableBuffer, IComparable<SimpleVertexBuffer>
    {
        private SimpleVertexBufferManager mManager;

        public int ElmCnt
        {
            get { return SizeInBytes / SimpleVertex.SizeInBytes; }
        }

        public SimpleVertexBuffer(SimpleVertexBufferManager manager, GraphicsDevice graphicsDevice, int elmCnt)
            : base(graphicsDevice, elmCnt * SimpleVertex.SizeInBytes, BufferUsage.None)
        {
            mManager = manager;
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                mManager.ReturnToPool(this);
            }
            else
            {
                base.Dispose(false);
            }
        }

        public void RealDispose()
        {
            base.Dispose(true);
        }

        public int CompareTo(SimpleVertexBuffer other)
        {
            return SizeInBytes.CompareTo(other.SizeInBytes);
        }

        public int CompareTo(object other)
        {
            return SizeInBytes.CompareTo(((SimpleVertexBuffer)other).SizeInBytes);
        }
    }
}
