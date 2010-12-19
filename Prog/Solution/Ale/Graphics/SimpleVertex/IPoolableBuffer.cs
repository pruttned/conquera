using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Graphics
{
    internal interface IPoolableBuffer : IDisposable, IComparable
    {
        int ElmCnt { get; }
        void RealDispose();
    }
}
