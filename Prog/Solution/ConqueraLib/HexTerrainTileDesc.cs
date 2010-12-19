﻿using System;
using System.Collections.Generic;
using System.Text;
using Ale.Content;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Conquera
{
    [NonContentPipelineAsset(typeof(HexTerrainTileDescLoader))]
    public abstract class HexTerrainTileDesc : IDisposable //todo abstract
    {
        /// <summary>
        /// Corners around 0,0
        /// </summary>
        private static Vector3[] Corners;

        private bool mIsDisposed = false;

        public GraphicModel GroundGraphicModel { get; private set; }
        public GraphicModel WallGraphicModel { get; private set; }
        /// <summary>
        /// Graphic models inserted to the static geometry
        /// </summary>
        public GraphicModel[] StaticGraphicModels { get; private set; } 
        /// <summary>
        /// Graphic models that can't be inserted to the static geometry - transparenta nd models with a skinned animation
        /// </summary>
        public GraphicModelDesc[] GraphicModels { get; private set; }

        /// <summary>
        /// This graphic model will be never placed into the static geometry
        /// </summary>
        public GraphicModelDesc ActiveGraphicModel { get; private set; }

        /// <summary>
        /// This graphic model will be never placed into the static geometry
        /// </summary>
        public GraphicModelDesc InactiveGraphicModel { get; private set; }


        public StaticGmConnectionPointAssigment[] StaticGmConnectionPointAssigments { get; private set; }

        //public TileType TileType
        //{
        //    get { return mSettings.TileType; }
        //}

        public bool IsPassable
        {
            get { return Settings.IsPassable; }
        }

        /// <summary>
        /// E.g. Lava, Temple of Life, Gold Mine
        /// </summary>
        public string NameType
        {
            get { return Settings.TypeName; }
        }
        public long Id
        {
            get { return Settings.Id; }
        }

        public Vector3 UnitPosition 
        {
            get { return Settings.UnitPosition; }
        }

        protected HexTerrainTileSettings Settings { get; private set; }

        public abstract string InfoViewType { get; }

        static HexTerrainTileDesc()
        {
            Corners = new Vector3[6];
            Vector3 baseVec = new Vector3(0, 1, 0);
            for (int i = 0; i < 6; ++i)
            {
                Quaternion rotQuat = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, -MathHelper.ToRadians(i * 60));
                Vector3.Transform(ref baseVec, ref rotQuat, out Corners[i]);
            }
        }

        public HexTerrainTileDesc(HexTerrainTileSettings settings, ContentGroup content)
        {
            Settings = settings;
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            GroundGraphicModel = CreateGroundGraphicModel(settings, content, graphicsDevice);
            WallGraphicModel = new GraphicModel(content.Load<GraphicModelDesc>(settings.WallGraphicModel), content);

            if (0 < settings.ActiveGraphicModel)
            {
                ActiveGraphicModel = content.Load<GraphicModelDesc>(settings.ActiveGraphicModel);
            }
            if (0 <  settings.InactiveGraphicModel)
            {
                InactiveGraphicModel = content.Load<GraphicModelDesc>(settings.InactiveGraphicModel);
            }

            var graphicModelsFromSettings = settings.GraphicModels;
            if (null != graphicModelsFromSettings && 0 < graphicModelsFromSettings.Count)
            {
                List<GraphicModel> staticGraphicModels = new List<GraphicModel>();
                List<GraphicModelDesc> graphicModels = new List<GraphicModelDesc>();
                List<StaticGmConnectionPointAssigment> staticGmConnectionPointAssigments = new List<StaticGmConnectionPointAssigment>();
                
                for (int i = 0; i < graphicModelsFromSettings.Count; ++i)
                {
                    GraphicModelDesc modelDesc = content.Load<GraphicModelDesc>(graphicModelsFromSettings[i]);

                    if (null == modelDesc.Mesh.SkeletalAnimations && IsOpaque(modelDesc)) //no animations and opaque
                    {
                        var gm = new GraphicModel(modelDesc, content);
                        staticGraphicModels.Add(gm);

                        //connection points
                        if(null != modelDesc.ConnectionPointAssigments)
                        {
                            foreach (var cpAssigment in modelDesc.ConnectionPointAssigments)
                            {
                                staticGmConnectionPointAssigments.Add(new StaticGmConnectionPointAssigment(cpAssigment, modelDesc));
                            }
                        }
                    }
                    else
                    {
                        graphicModels.Add(modelDesc);
                    }
                }
             
                if (0 < staticGraphicModels.Count)
                {
                    StaticGraphicModels = staticGraphicModels.ToArray();
                }
                if (0 < graphicModels.Count)
                {
                    GraphicModels = graphicModels.ToArray();
                }
                if(0< staticGmConnectionPointAssigments.Count)
                {
                    StaticGmConnectionPointAssigments = staticGmConnectionPointAssigments.ToArray();
                }
            }
        }

        /// <summary>
        /// Get a corners around 0,0
        /// </summary>
        public static void GetCornerPos(HexTileCorner corner, out Vector3 pos)
        {
            pos = Corners[(int)corner];
        }

        /// <summary>
        /// Get a corners around 0,0
        /// </summary>
        public static Vector3 GetCornerPos(HexTileCorner corner)
        {
            return Corners[(int)corner];
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected internal abstract void OnBeginTurn(HexCell cell);

        protected internal abstract void OnActivated(HexCell cell);

        protected internal abstract void OnDeactivated(HexCell cell);

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    GroundGraphicModel.Dispose();
                }
                mIsDisposed = true;
            }
        }

        protected GraphicModel CreateGroundGraphicModel(HexTerrainTileSettings settings, ContentGroup content, GraphicsDevice graphicsDevice)
        {
            HexTerrainTileAtlas atlas = content.Load<HexTerrainTileAtlas>(settings.HexTerrainTileAtlas);

            float baseCellSize = 1.0f / (float)atlas.Size;
            float textureCellSpacing = atlas.TextureCellSpacing * baseCellSize;
            float textureCellSize = baseCellSize - textureCellSpacing;
            Point textureCellIndex = settings.TileIndex;

            MeshBuilder meshBuilder = new MeshBuilder(graphicsDevice);
            meshBuilder.SetCurrentSubMesh("mat1");

            SimpleVertex vert = new SimpleVertex(new Vector3(0, 0, 0), Vector3.UnitZ, Vector2.Zero);

            vert.Uv = new Vector2(
                   textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
                   (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
                   );

            int vcI = meshBuilder.AddVertex(ref vert);
            vert.Position = Corners[0];

            vert.Uv = new Vector2(
                   textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
                   (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
                   );

            int vOldI = meshBuilder.AddVertex(ref vert);
            int vFirstI = vOldI;
            for (int i = 1; i < 6; ++i)
            {
                vert.Position = Corners[i];

                vert.Uv = new Vector2(
                    textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseCellSize * (float)(textureCellIndex.X),
                    (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseCellSize * (float)(textureCellIndex.Y))
                );

                int vI = meshBuilder.AddVertex(ref vert);

                meshBuilder.AddFace(vOldI, vI, vcI);

                vOldI = vI;
            }

            meshBuilder.AddFace(vOldI, vFirstI, vcI);

            Mesh tileMesh = meshBuilder.BuildMesh();

            tileMesh.AddConnectionPoint("center", null, new Vector3(0.5f, 0.5f, 0), Quaternion.Identity);

            return new GraphicModel(tileMesh, atlas.Material);
        }

        private bool IsOpaque(GraphicModelDesc graphicModelDesc)
        {
            foreach (var material in graphicModelDesc.PartMaterials.Values)
            {
                foreach (var technique in material.Techniques.Values)
                {
                    foreach(var pass in technique.MaterialEffectTechnique.Passes)
                    {
                        if (pass.IsTransparent)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
    }

    public class StaticGmConnectionPointAssigment
    {
        public ConnectionPointAssigmentDesc ConnectionPoint { get; private set; }
        public GraphicModelDesc GraphicModelDesc { get; private set; }

        public StaticGmConnectionPointAssigment(ConnectionPointAssigmentDesc connectionPoint, GraphicModelDesc graphicModelDesc)
        {
            ConnectionPoint = connectionPoint;
            GraphicModelDesc = graphicModelDesc;
        }
    }

    public enum HexTileCorner
    {
        Top = 0,
        UperRight = 1,
        LowerRight = 2,
        Down = 3,
        LowerLeft = 4,
        UperLeft = 5
    }







    public class DimensionGateTileDesc : HexTerrainTileDesc
    {
        private GameCard[] mGameCards;

        public override string InfoViewType
        {
            get { return "DimensionGate"; }
        }

        public DimensionGateTileDesc(DimensionGateTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
        }

        protected internal override void OnActivated(HexCell cell)
        {
            foreach (GameCard card in mGameCards)
            {
                cell.OwningPlayer.AddCard(card);
            }
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
            foreach (GameCard card in mGameCards)
            {
                cell.OwningPlayer.RemoveCard(card);
            }
        }
    }


    public class CastleTileDesc : HexTerrainTileDesc
    {
        public override string InfoViewType
        {
            get { return "Castle"; }
        }

        public CastleTileDesc(CastleTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
        }
    }

    public class GoldMineTileDesc : HexTerrainTileDesc
    {
        public int GoldIncrement
        {
            get { return ((GoldMineTileSettings)Settings).GoldIncrement; }
        }
        public override string InfoViewType
        {
            get { return "GoldMine"; }
        }
        private string NotificationString {get; set;}

        public GoldMineTileDesc(GoldMineTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
            NotificationString = string.Format("+{0}", GoldIncrement.ToString());
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            if (cell.IsActive)
            {
                cell.OwningPlayer.Gold += GoldIncrement;
                if (cell.OwningPlayer.IsHuman)
                {
                    cell.Scene.FireCellNotificationLabel(NotificationString, CellNotificationIcons.Coin, Color.Yellow, cell.Index);
                }
            }
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
        }
    }

    public class LandTileDesc : HexTerrainTileDesc
    {
        public override string InfoViewType
        {
            get { return "Land"; }
        }

        public LandTileDesc(LandTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
        }
    }

    public class VillageTileDesc : HexTerrainTileDesc
    {
        public override string InfoViewType
        {
            get { return "Village"; }
        }

        public VillageTileDesc(VillageTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
        }
    }

    public class LandTempleTileDesc : HexTerrainTileDesc
    {
        public override string InfoViewType
        {
            get { return "LandTemple"; }
        }

        public LandTempleTileDesc(LandTempleTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivated(HexCell cell)
        {
        }
    }

}
