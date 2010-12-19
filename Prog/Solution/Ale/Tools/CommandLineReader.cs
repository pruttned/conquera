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

using System.Collections.Generic;

namespace Ale.Tools
{
    public static class CommandLineReader
    {
        private const string mParameterKeyStartChar = "-";

        public static Dictionary<string, string> ReadParameters(string[] parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (null != parameters)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    string key = parameters[i];

                    if (key.StartsWith(mParameterKeyStartChar))
                    {
                        string value = null;
                        if (i < parameters.Length - 1 && !parameters[i + 1].StartsWith(mParameterKeyStartChar))
                        {
                            value = parameters[i + 1];
                            i++;
                        }
                        dictionary.Add(key.Substring(1), value);
                    }
                }
            }
            return dictionary;
        }
    }
}
