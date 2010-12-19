using System;
using System.Collections.Generic;
using System.Text;

namespace Ale.Tools
{
    internal sealed class EmptyListEnumerator<T> : IEnumerator<T>
    {
        private static EmptyListEnumerator<T> mInstance = new EmptyListEnumerator<T>();

        public static EmptyListEnumerator<T> Instance
        {
            get { return mInstance; }
        }

        public T Current
        {
            get { return default(T); }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return default(T); }
        }

        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        private EmptyListEnumerator()
        {
        }
    }
}
