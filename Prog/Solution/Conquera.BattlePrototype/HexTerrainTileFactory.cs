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
using System.Reflection;
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HexTerrainTileAttribute : Attribute
    {
        public string Template { get; private set; }
        public HexTerrainTileAttribute(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            Template = name;
        }
    }

    public static class HexTerrainTileFactory
    {
        private static Dictionary<string, ConstructorInfo> mTileCtors = new Dictionary<string, ConstructorInfo>(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<Type, string> mTemplateNames = new Dictionary<Type, string>();

        private static Type[] mCtorArgTypes = new Type[] { typeof(Point) , typeof(int)};

        public static ICollection<string> TemplateNames
        {
            get { return mTemplateNames.Values; }
        }

        static HexTerrainTileFactory()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(HexTerrainTile).IsAssignableFrom(type))
                {
                    HexTerrainTileAttribute att = (HexTerrainTileAttribute)type.GetCustomAttributes(typeof(HexTerrainTileAttribute), true).FirstOrDefault();
                    if (null != att)
                    {
                        var ctor = type.GetConstructor(mCtorArgTypes);
                        if(null  == ctor)
                        {
                            throw new Exception(string.Format("Type '{0}' is missing public ctor with arguments '{1}'", type.Name, 
                                string.Join(",", (from t in mCtorArgTypes select t.Name).ToArray()))); 
                        }
                        mTileCtors.Add(att.Template, ctor);
                        mTemplateNames.Add(type, att.Template);
                    }
                }
            }
        }

        public static HexTerrainTile CreateTile(string templateName, Point index, int terrainHeight)
        {
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException("templateName");
            return (HexTerrainTile)mTileCtors[templateName].Invoke(new object[] { index, terrainHeight });
        }

        public static string GetTemplateName(Type type)
        {
            if (null == type) throw new ArgumentNullException("type");

            return mTemplateNames[type];
        }

    }
}
