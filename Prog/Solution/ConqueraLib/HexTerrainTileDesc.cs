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
using System.Collections.ObjectModel;

namespace Conquera
{
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

    [NonContentPipelineAsset(typeof(HexTerrainTileDescLoader))]
    public abstract class HexTerrainTileDesc : IDisposable
    {
        private bool mIsDisposed = false;

        List<GraphicModel> mStaticGraphicModels;
        List<GraphicModelDesc> mNonStaticGraphicModels;
        List<StaticGmConnectionPointAssigment> mStaticGraphicModelsConnectionPointAssigments;

        public string Name
        {
            get { return Settings.Name; }
        }

        public string DisplayName
        {
            get { return Settings.DisplayName; }
        }

        /// <summary>
        /// Graphic models inserted to the static geometry
        /// </summary>
        public ReadOnlyCollection<GraphicModel> StaticGraphicModels { get; private set; }
        /// <summary>
        /// Graphic models that can't be inserted to the static geometry - transparent and models with a skinned animation
        /// </summary>
        public ReadOnlyCollection<GraphicModelDesc> NonStaticGraphicModels { get; private set; }

        public ReadOnlyCollection<StaticGmConnectionPointAssigment> StaticGraphicModelsConnectionPointAssigments { get; private set; }

        public abstract bool IsPassable { get; }

        public abstract Vector3 UnitPosition { get; }

        public long Id
        {
            get { return Settings.Id; }
        }

        protected HexTerrainTileSettings Settings { get; private set; }

        public HexTerrainTileDesc(HexTerrainTileSettings settings, ContentGroup content)
        {
            Settings = settings;
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            if (null != settings.GraphicModels && 0 < settings.GraphicModels.Count)
            {
                foreach (var graphicModel in settings.GraphicModels)
                {
                    AddGraphicModel(graphicModel, content);
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual HexTerrainTile CreateHexTerrainTile(BattleScene scene, Point index)
        {
            return new HexTerrainTile(scene, index, this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    if (null != mStaticGraphicModels)
                    {
                        foreach (var graphicModel in mStaticGraphicModels)
                        {
                            graphicModel.Dispose();
                        }
                    }
                }
                mIsDisposed = true;
            }
        }

        protected void AddGraphicModel(long graphicModelDesc, ContentGroup content)
        {
            GraphicModelDesc modelDesc = content.Load<GraphicModelDesc>(graphicModelDesc);

            if (null == modelDesc.Mesh.SkeletalAnimations && IsGraphicModelFullyOpaque(modelDesc)) //no animations and opaque
            {
                AddStaticGraphicModel(modelDesc, content);
            }
            else
            {
                AddNonStaticGraphicModel(modelDesc);
            }
        }

        protected void AddNonStaticGraphicModel(GraphicModelDesc graphicModelDesc)
        {
            if (null == mNonStaticGraphicModels)
            {
                mNonStaticGraphicModels = new List<GraphicModelDesc>();
                NonStaticGraphicModels = new ReadOnlyCollection<GraphicModelDesc>(mNonStaticGraphicModels);
            }
            mNonStaticGraphicModels.Add(graphicModelDesc);
        }

        protected void AddStaticGraphicModel(GraphicModelDesc graphicModelDesc, ContentGroup content)
        {
            var gm = new GraphicModel(graphicModelDesc, content);
            if (null == mStaticGraphicModels)
            {
                mStaticGraphicModels = new List<GraphicModel>();
                StaticGraphicModels = new ReadOnlyCollection<GraphicModel>(mStaticGraphicModels);
            }
            mStaticGraphicModels.Add(gm);

            //connection points
            if (null != graphicModelDesc.ConnectionPointAssigments)
            {
                if (null == mStaticGraphicModelsConnectionPointAssigments)
                {
                    mStaticGraphicModelsConnectionPointAssigments = new List<StaticGmConnectionPointAssigment>();
                    StaticGraphicModelsConnectionPointAssigments = new ReadOnlyCollection<StaticGmConnectionPointAssigment>(mStaticGraphicModelsConnectionPointAssigments);
                }

                foreach (var cpAssigment in graphicModelDesc.ConnectionPointAssigments)
                {
                    mStaticGraphicModelsConnectionPointAssigments.Add(new StaticGmConnectionPointAssigment(cpAssigment, graphicModelDesc));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// GraphicModel will be disposed automatically. Contected renderables are ignored
        /// </remarks>
        /// <param name="graphicModel"></param>
        protected void AddStaticGraphicModel(GraphicModel graphicModel)
        {
            if (null == mStaticGraphicModels)
            {
                mStaticGraphicModels = new List<GraphicModel>();
                StaticGraphicModels = new ReadOnlyCollection<GraphicModel>(mStaticGraphicModels);
            }
            mStaticGraphicModels.Add(graphicModel);
        }

        protected bool IsGraphicModelFullyOpaque(GraphicModelDesc graphicModelDesc)
        {
            foreach (var material in graphicModelDesc.PartMaterials.Values)
            {
                foreach (var technique in material.Techniques.Values)
                {
                    foreach (var pass in technique.MaterialEffectTechnique.Passes)
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

    public abstract class GroundHexTerrainTileDesc : HexTerrainTileDesc
    {
        public override bool IsPassable
        {
            get { return ((GroundHexTerrainTileSettings)Settings).IsPassable; }
        }

        public long Id
        {
            get { return Settings.Id; }
        }

        public override Vector3 UnitPosition
        {
            get { return ((GroundHexTerrainTileSettings)Settings).UnitPosition; }
        }

        protected HexTerrainTileSettings Settings { get; private set; }

        public GroundHexTerrainTileDesc(GroundHexTerrainTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
            GraphicsDevice graphicsDevice = ((IGraphicsDeviceService)content.ServiceProvider.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            AddStaticGraphicModel(CreateGroundGraphicModel(settings, content, graphicsDevice));
        }

        protected GraphicModel CreateGroundGraphicModel(GroundHexTerrainTileSettings settings, ContentGroup content, GraphicsDevice graphicsDevice)
        {
            HexTerrainTileAtlas atlas = content.Load<HexTerrainTileAtlas>(settings.HexTerrainTileAtlas);

            Settings = settings;

            float baseUvCellSize = 1.0f / (float)atlas.Size;
            float textureCellSpacing = atlas.TextureCellSpacing * baseUvCellSize;
            float textureCellSize = baseUvCellSize - textureCellSpacing;
            Point textureCellIndex = settings.TileIndex;

            MeshBuilder meshBuilder = new MeshBuilder(graphicsDevice);
            meshBuilder.SetCurrentSubMesh("mat1");

            SimpleVertex vert = new SimpleVertex(new Vector3(0, 0, 0), Vector3.UnitZ, Vector2.Zero);

            vert.Uv = new Vector2(
                   textureCellSize * (vert.Position.X / 2.0f + 0.5f) + baseUvCellSize * (float)(textureCellIndex.X),
                   (textureCellSize * (1 - (vert.Position.Y / 2.0f + 0.5f)) + baseUvCellSize * (float)(textureCellIndex.Y))
                   );

            int vcI = meshBuilder.AddVertex(ref vert);
            vert.Position = HexHelper.GetHexTileCornerPos3D(HexTileCorner.Top); //0
            Vector3 unitCornerPos =  HexHelper.GetUnitHexTileCornerPos3D(HexTileCorner.Top); //0

            vert.Uv = new Vector2(
                   textureCellSize * (unitCornerPos.X / 2.0f + 0.5f) + baseUvCellSize * (float)(textureCellIndex.X),
                   (textureCellSize * (1 - (unitCornerPos.Y / 2.0f + 0.5f)) + baseUvCellSize * (float)(textureCellIndex.Y))
                   );

            int vOldI = meshBuilder.AddVertex(ref vert);
            int vFirstI = vOldI;
            for (int i = 1; i < 6; ++i)
            {
                vert.Position = HexHelper.GetHexTileCornerPos3D((HexTileCorner)i);
                unitCornerPos = HexHelper.GetUnitHexTileCornerPos3D((HexTileCorner)i); 

                vert.Uv = new Vector2(
                    textureCellSize * (unitCornerPos.X / 2.0f + 0.5f) + baseUvCellSize * (float)(textureCellIndex.X),
                    (textureCellSize * (1 - (unitCornerPos.Y / 2.0f + 0.5f)) + baseUvCellSize * (float)(textureCellIndex.Y))
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
    }

    public abstract class CapturableHexTerrainTileDesc : GroundHexTerrainTileDesc
    {
        public CapturableHexTerrainTileDesc(GroundHexTerrainTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal abstract void OnOwningPlayerBeginTurn(HexTerrainTile tile);

        protected internal abstract void OnCaptured(HexTerrainTile tile);

        protected internal abstract void OnLost(HexTerrainTile tile);
    }

    public class HexTerrainGapTileDesc : HexTerrainTileDesc
    {
        public HexTerrainGapTileDesc(HexTerrainTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        public override bool IsPassable
        {
            get { return false; }
        }

        public override Vector3 UnitPosition
        {
            get { return Vector3.Zero; }
        }
    }

    public class CastleTileDesc : CapturableHexTerrainTileDesc
    {
        public CastleTileDesc(CastleTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }

        protected internal override void OnOwningPlayerBeginTurn(HexTerrainTile tile)
        {
        }

        protected internal override void OnCaptured(HexTerrainTile tile)
        {
            //todo
            //tile.OwningPlayer.CastleCnt++;
        }

        protected internal override void OnLost(HexTerrainTile tile)
        {
            //todo
            //tile.OwningPlayer.CastleCnt--;
        }
    }

    public class ManaMineTileDesc : CapturableHexTerrainTileDesc
    {
        public int ManaIncrement
        {
            get { return ((ManaMineTileSettings)Settings).ManaIncrement; }
        }
        private string NotificationString { get; set; }

        public ManaMineTileDesc(ManaMineTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
            NotificationString = string.Format("+{0}", ManaIncrement.ToString());
        }

        protected internal override void OnOwningPlayerBeginTurn(HexTerrainTile tile)
        {
            //todo
            //cell.OwningPlayer.Mana += ManaIncrement;
            //if (cell.OwningPlayer.IsHuman)
            //{
            //    cell.Scene.FireCellNotificationLabel(NotificationString, CellNotificationIcons.Coin, Color.Purple, cell.Index);
            //}
        }

        protected internal override void OnCaptured(HexTerrainTile tile)
        {
        }

        protected internal override void OnLost(HexTerrainTile tile)
        {
        }
    }

    public class LandTileDesc : GroundHexTerrainTileDesc
    {
        public LandTileDesc(LandTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
        }
    }

    public class GapWallSampleDesc
    {
        public GraphicModel GraphicModel {get; private set;}
        public int Priority {get; private set;}

        public GapWallSampleDesc(int priority, GraphicModel graphicModel)
        {
            GraphicModel = graphicModel;
            Priority = priority;
        }
    }

    public class GapHexTerrainTileDesc : HexTerrainTileDesc
    {
        private bool mIsDisposed = false;

        public int WallSamplePrioritySum { get; private set; }

        public ReadOnlyCollection<GapWallSampleDesc> WallSamples { get; private set; }


        public override bool IsPassable
        {
            get { return false; }
        }

        public override Vector3 UnitPosition
        {
            get { return Vector3.Zero; }
        }

        public GapHexTerrainTileDesc(GapHexTerrainTileSettings settings, ContentGroup content)
            : base(settings, content)
        {
            List<GapWallSampleDesc> wallSamples = new List<GapWallSampleDesc>();
            WallSamples = new ReadOnlyCollection<GapWallSampleDesc>(wallSamples);
            WallSamplePrioritySum = 0;
            foreach (var wallSampleSettings in settings.GapWallGraphicModelSamples)
            {
                WallSamplePrioritySum += wallSampleSettings.Priority;
                wallSamples.Add(new GapWallSampleDesc(wallSampleSettings.Priority, new GraphicModel(content.Load<GraphicModelDesc>(wallSampleSettings.GraphicModel), content)));
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    foreach(var wallSample in WallSamples)
                    {
                        wallSample.GraphicModel.Dispose();
                    }
                }
                mIsDisposed = true;
            } 
            base.Dispose(isDisposing);
        }

        public override HexTerrainTile CreateHexTerrainTile(BattleScene scene, Point index)
        {
            return new GapHexTerrainTile(scene, index, this);
        }
    }
}
