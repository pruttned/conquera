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
using System.Linq;
using System.Text;
using Ale.Gui;
using SimpleOrmFramework;

namespace Conquera
{
    //[DataObject(MaxCachedCnt = 0)]
    public class GameCard //: BaseDataObject
    {
        public enum GameCardColors { Black, Green, Purple }

        public GraphicElement Picture { get; set; }
        public GraphicElement Icon { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttackPurple { get; set; }
        public int AttackGreen { get; set; }
        public int AttackBlack { get; set; }
        public int DefensePurple { get; set; }
        public int DefenseGreen { get; set; }
        public int DefenseBlack { get; set; }
    }

}
