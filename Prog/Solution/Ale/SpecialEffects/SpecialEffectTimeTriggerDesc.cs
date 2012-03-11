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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Tools;
using Ale.Content;
using System.Collections.ObjectModel;

namespace Ale.SpecialEffects
{
    public class SpecialEffectTimeTriggerDesc
    {
        public NameId Action { get; private set; }
        public float Time { get; private set; }
        public ReadOnlyDictionary<NameId, string> Params { get; private set; }

        public SpecialEffectTimeTriggerDesc(ContentReader input)
        {
            Action = input.ReadString();
            Time = input.ReadSingle();
            int paramCnt = input.ReadInt32();
            Dictionary<NameId, string> parameters = new Dictionary<NameId, string>();
            Params = new ReadOnlyDictionary<NameId, string>(parameters);
            for (int j = 0; j < paramCnt; j++)
            {
                string paramName = input.ReadString();
                string paramValue = input.ReadString();
                parameters.Add(paramName, paramValue);
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", Time.ToString(), Action);
        }
    }
}
