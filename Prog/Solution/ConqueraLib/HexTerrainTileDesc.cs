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
using Ale.Content;
using Ale.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Ale.Gui;

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

        public string Name
        {
            get { return Settings.Name; }
        }
        
        public string DisplayName
        {
            get { return Settings.DisplayName; }
        }

        public string Description
        {
            get { return Settings.Description.Text; }
        }

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

        public long Id
        {
            get { return Settings.Id; }
        }

        public Vector3 UnitPosition 
        {
            get { return Settings.UnitPosition; }
        }

        public GraphicElement Icon { get; private set; }

        protected HexTerrainTileSettings Settings { get; private set; }

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

            Icon = Conquera.Gui.ConqueraPalette.GetTileIcon(settings.Icon);
        }

        /// <summary>
        /// Gets corners around 0,0
        /// </summary>
        /// <returns></returns>
        public static Vector3[] GetCorners()
        {
            Vector3[] corners = new Vector3[6];
            Corners.CopyTo(corners, 0);
            return corners;
        }

        /// <summary>
        /// Get a corner around 0,0
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

        protected internal virtual void OnBeginTurn(HexCell cell)
        {
            var unit = cell.GameUnit;
            if (null != unit)
            {
                unit.Heal(Settings.HpIncrement);
            }
        }

        protected internal abstract void OnActivated(HexCell cell);

        protected internal abstract void OnDeactivating(HexCell cell);

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






  public class CastleTileDesc : HexTerrainTileDesc
    {
        public CastleTileDesc(CastleTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            base.OnBeginTurn(cell);
        }

        protected internal override void OnActivated(HexCell cell)
        {
            cell.OwningPlayer.CastleCnt++;
        }

        protected internal override void OnDeactivating(HexCell cell)
        {
            cell.OwningPlayer.CastleCnt--;
        }
    }

    public class GoldMineTileDesc : HexTerrainTileDesc
    {
        public int GoldIncrement
        {
            get { return ((GoldMineTileSettings)Settings).GoldIncrement; }
        }
        private string NotificationString {get; set;}

        public GoldMineTileDesc(GoldMineTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
            NotificationString = string.Format("+{0}", GoldIncrement.ToString());
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            base.OnBeginTurn(cell);
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

        protected internal override void OnDeactivating(HexCell cell)
        {
        }
    }

    public class LandTileDesc : HexTerrainTileDesc
    {
        public LandTileDesc(LandTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            base.OnBeginTurn(cell);
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivating(HexCell cell)
        {
        }
    }

    public class VillageTileDesc : HexTerrainTileDesc
    {
        public VillageTileDesc(VillageTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            base.OnBeginTurn(cell);
        }

        protected internal override void OnActivated(HexCell cell)
        {
            cell.OwningPlayer.MaxUnitCnt += ((VillageTileSettings)Settings).MaxUnitCntIncrement;
        }

        protected internal override void OnDeactivating(HexCell cell)
        {
            cell.OwningPlayer.MaxUnitCnt -= ((VillageTileSettings)Settings).MaxUnitCntIncrement;
        }
    }

    public class LandTempleTileDesc : HexTerrainTileDesc
    {
        public LandTempleTileDesc(LandTempleTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnBeginTurn(HexCell cell)
        {
            base.OnBeginTurn(cell);
        }

        protected internal override void OnActivated(HexCell cell)
        {
        }

        protected internal override void OnDeactivating(HexCell cell)
        {
        }
    }

}
