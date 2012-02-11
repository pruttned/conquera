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

using Ale.Input;

namespace Ale.Gui
{
    public delegate void PropertyChangedHandler<TOwner, TValue>(TOwner propertyOwner, TValue oldValue);
    public delegate void MouseEventHandler(GuiNode guiNode, IMouseManager mouseManager);

    public static class EventHelper
    {
        public static void RaiseEvent<TOwner, TValue>(PropertyChangedHandler<TOwner, TValue> handler, TOwner propertyOwner, TValue oldValue)
        {
            if (handler != null)
            {
                handler(propertyOwner, oldValue);
            }
        }

        public static void RaiseEvent(MouseEventHandler handler, GuiNode guiNode, IMouseManager mouseManager)
        {
            if (handler != null)
            {
                handler(guiNode, mouseManager);
            }
        }
    }
}
