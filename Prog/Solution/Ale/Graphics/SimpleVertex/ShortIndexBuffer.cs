using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    internal class ShortIndexBuffer : IndexBuffer, IPoolableBuffer, IComparable<ShortIndexBuffer>
    {
        private ShortIndexBufferManager mManager;

        public int ElmCnt
        {
            get { return this.SizeInBytes / 2; } //16bit
        }

        public ShortIndexBuffer(ShortIndexBufferManager manager, GraphicsDevice graphicsDevice, int elmCnt)
            : base(graphicsDevice, elmCnt * 2, BufferUsage.None, IndexElementSize.SixteenBits)
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

        public int CompareTo(ShortIndexBuffer other)
        {
            return SizeInBytes.CompareTo(other.SizeInBytes);
        }

        public int CompareTo(object other)
        {
            return SizeInBytes.CompareTo(((ShortIndexBuffer)other).SizeInBytes);
        }
    }

}
