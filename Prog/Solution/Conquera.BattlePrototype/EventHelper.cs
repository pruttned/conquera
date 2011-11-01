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

namespace Conquera.BattlePrototype
{
    class EventHelper
    {
        public static void RaiseValueChange<T>(EventHandler<ValueChangeEventArgs<T>> handler, object sender, T oldValue, T newValue)
        {
            if (handler != null)
            {
                handler(sender, new ValueChangeEventArgs<T>(oldValue, newValue));
            }
        }

        public static void RaiseEvent<T>(EventHandler<T> handler, object sender, T eventArgs) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, eventArgs);
            }
        }
    }

    public class ValueChangeEventArgs<T> : EventArgs
    {
        public T OldValue { get; private set; }
        public T NewValue { get; private set; }

        public ValueChangeEventArgs(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
