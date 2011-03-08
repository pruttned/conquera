using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquera.Editor
{
    public class CommandQueue
    {
        private Queue<ICommand> mQueue = new Queue<ICommand>();

        public void Enqueue(ICommand command)
        {
            if (null == command) throw new ArgumentNullException("command");

            lock (mQueue)
            {
                mQueue.Enqueue(command);
            }
        }

        public ICommand Dequeue()
        {
            lock (mQueue)
            {
                if (0 == mQueue.Count)
                {
                    return null;
                }
                return mQueue.Dequeue();
            }
        }
    }
}
