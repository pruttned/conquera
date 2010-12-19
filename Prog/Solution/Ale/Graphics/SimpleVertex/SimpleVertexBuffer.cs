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
