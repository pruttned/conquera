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
