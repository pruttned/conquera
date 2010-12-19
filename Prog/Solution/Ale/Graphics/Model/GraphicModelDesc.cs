using Ale.Content;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Ale.Tools;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
namespace Ale.Graphics
{
    [NonContentPipelineAsset(typeof(GraphicModelDescLoader))]
    public class GraphicModelDesc
    {
        public Mesh Mesh { get; private set; }
        public ReadOnlyDictionary<NameId, Material> PartMaterials { get; private set; }
        public ReadOnlyCollection<ConnectionPointAssigmentDesc> ConnectionPointAssigments { get; private set; }
        public float BoundsMultiplicator { get; private set; }
        public float Scale { get; private set; }
        public Quaternion Orientation { get; private set; }
        public Vector3 Position { get; private set; }


        public GraphicModelDesc(GraphicsDevice graphicsDevice, GraphicModelSettings settings, ContentGroup content)
        {
            Mesh = content.Load<Mesh>(settings.Mesh);

            Dictionary<NameId, Material> partMaterials = new Dictionary<NameId, Material>();
            PartMaterials = new ReadOnlyDictionary<NameId, Material>(partMaterials);
            foreach (MaterialAssignmentsettings materialAssignment in settings.MaterialAssignments)
            {
                Material material = content.Load<Material>(materialAssignment.Material);
                partMaterials.Add(materialAssignment.MaterialGroup, material);
            }

            List<ConnectionPointAssigmentDesc> connectionPointAssigments = new List<ConnectionPointAssigmentDesc>();
            ConnectionPointAssigments = new ReadOnlyCollection<ConnectionPointAssigmentDesc>(connectionPointAssigments);
            foreach (ConnectionPointAssigmentSettings cpAssigment in settings.ConnectionPointAssigments)
            {
                connectionPointAssigments.Add(new ConnectionPointAssigmentDesc(cpAssigment));
            }

            BoundsMultiplicator = settings.BoundsMultiplicator;
            Scale = settings.Scale;
            Orientation = settings.Orientation;
            Position = settings.Position;
        }
    }
}
