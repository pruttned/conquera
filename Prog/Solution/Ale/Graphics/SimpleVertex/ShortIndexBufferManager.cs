using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    internal class ShortIndexBufferManager : PoolableBufferManager<ShortIndexBuffer>
    {
        private static ShortIndexBufferManager mInstance = null;

        public static ShortIndexBufferManager Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new ShortIndexBufferManager();
                }
                return mInstance;
            }
        }

        private ShortIndexBufferManager()
        {
        }

        protected override ShortIndexBuffer CreateBuffer(GraphicsDevice graphicsDevice, int elmCnt)
        {
            return new ShortIndexBuffer(this, graphicsDevice, elmCnt);
        }
    }
}
