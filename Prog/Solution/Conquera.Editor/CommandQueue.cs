////////////////////////////////////////////////////////////////////////
////  Copyright (C) 2010 by Conquera Team
////  Part of the Conquera Project
////
////  This program is free software: you can redistribute it and/or modify
////  it under the terms of the GNU General Public License as published by
////  the Free Software Foundation, either version 2 of the License, or
////  (at your option) any later version.
////
////  This program is distributed in the hope that it will be useful,
////  but WITHOUT ANY WARRANTY; without even the implied warranty of
////  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
////  GNU General Public License for more details.
////
////  You should have received a copy of the GNU General Public License
////  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//////////////////////////////////////////////////////////////////////////

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Conquera.Editor
//{
//    public class CommandQueue
//    {
//        private List<ICommand> mQueue = new List<ICommand>();

//        public void Enqueue(ICommand command)
//        {
//            if (null == command) throw new ArgumentNullException("command");

//            lock (mQueue)
//            {
//                if (command.RemoveEnquedDuplicates && 0 < mQueue.Count)
//                {
//                    for (int i = mQueue.Count - 1; i >= 0; --i)
//                    {
//                        if (mQueue[i].Name == command.Name)
//                        {
//                            mQueue.RemoveAt(i);
//                        }
//                    }
//                }
//                mQueue.Add(command);
//            }
//        }

//        public ICommand Dequeue()
//        {
//            lock (mQueue)
//            {
//                if (0 == mQueue.Count)
//                {
//                    return null;
//                }
//                var item = mQueue[0];
//                mQueue.RemoveAt(0);
//                return item;
//            }
//        }
//    }
//}
