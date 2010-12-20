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
using SimpleOrmFramework;
using System.Collections.Generic;
using Ale.Tools;
using Microsoft.Xna.Framework;

namespace Ale.Graphics
{
    [DataObject(MaxCachedCnt = 5)]
    [CustomBasicTypeProvider(typeof(Quaternion), typeof(FieldCustomBasicTypeProvider<Quaternion>))]
    [CustomBasicTypeProvider(typeof(Vector3), typeof(FieldCustomBasicTypeProvider<Vector3>))]
    public class GraphicModelSettings : BaseDataObject
    {
        [DataProperty(CaseSensitive = false, NotNull = true, Unique=true)]
        public string Name {get; set;}

        [DataProperty(NotNull = true, CaseSensitive = false)]
        public string Mesh { get; set; }

        [DataListProperty(NotNull = true)]
        public List<MaterialAssignmentsettings> MaterialAssignments { get; private set; }

        [DataListProperty(NotNull = true)]
        public List<ConnectionPointAssigmentSettings> ConnectionPointAssigments { get; private set; }

        [DataProperty]
        public float BoundsMultiplicator { get; set; }

        [DataProperty]
        public float Scale { get; set; }

        [DataProperty]
        public Quaternion Orientation { get; set; }

        [DataProperty]
        public Vector3 Position { get; set; }

        public GraphicModelSettings()
        {
            Name = null;
            Mesh = null;
            BoundsMultiplicator = 1.0f;
            Scale = 1.0f;
            Orientation = Quaternion.Identity;
            Position = Vector3.Zero;
            MaterialAssignments = new List<MaterialAssignmentsettings>();
            ConnectionPointAssigments = new List<ConnectionPointAssigmentSettings>();
        }

        public GraphicModelSettings(string name, string mesh)
            :this()
        {
            Name = name;
            Mesh = mesh;
        }
    }
}
