//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
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
using System.Collections.Generic;
using System.Text;
using Ale.Tools;

namespace Ale.Graphics
{
    public class ConnectionPointAssigmentDesc
    {
        public NameId ConnectionPoint { get; private set; }
        public long Renderable { get; private set; }
        public int RenderableFactory { get; private set; }

        public ConnectionPointAssigmentDesc(ConnectionPointAssigmentSettings settings)
        {
            ConnectionPoint = settings.ConnectionPoint;
            Renderable = settings.Renderable;
            RenderableFactory = settings.RenderableFactory;
        }
    }
}
