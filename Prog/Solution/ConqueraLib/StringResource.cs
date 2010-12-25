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
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;

namespace Conquera
{
    [DataObject]
    public class StringResource : BaseDataObject
    {
        [DataProperty(NotNull=true, Unique=true)]
        public string Name { get; private set; }
        [DataProperty(NotNull = true)]
        public string Text { get; private set; }

        public StringResource(string name, string text)
        {
            Name = name;
            Text = text;
        }

        private StringResource()
        {
        }
    }
}
