using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Ale.Graphics
{
    internal class SimpleVertexBufferManager : PoolableBufferManager<SimpleVertexBuffer>
    {
        private static SimpleVertexBufferManager mInstance = null;

        public static SimpleVertexBufferManager Instance
        {
            get
            {
                if (null == mInstance)
                {
                    mInstance = new SimpleVertexBufferManager();
                }
                return mInstance;
            }
        }

        private SimpleVertexBufferManager()
        {
        }

        protected override SimpleVertexBuffer CreateBuffer(GraphicsDevice graphicsDevice, int elmCnt)
        {
            return new SimpleVertexBuffer(this, graphicsDevice, elmCnt);
        }
    }
}
