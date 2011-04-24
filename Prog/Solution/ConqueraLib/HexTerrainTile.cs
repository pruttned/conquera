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
using SimpleOrmFramework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Tools;
using Ale.Content;
using Microsoft.Xna.Framework;
using Ale.Graphics;
using Ale.Scene;

namespace Conquera
{
    internal class HexTerrainTile : SceneObject
    {
        private List<BathcedModelIdentifier> mStaticGeometryIds = new List<BathcedModelIdentifier>();
        private List<GraphicModel> mGraphicModels;
        private List<Renderable> mStaticCpRenderables;
        private GraphicModel mActiveGraphicModel;
        private GraphicModel mInactiveGraphicModel;
        private Vector3 mCenterPos;
        private HexCellCapturedMark mHexCellCapturedMark;

        public HexTerrainTileDesc Desc { get; private set; }

        public Vector3 CenterPos
        {
            get { return mCenterPos; }
        }

        internal bool HasWall { get; private set; }

        internal HexTerrainTile(HexTerrainTileDesc desc, bool hasWall, Vector3 centerPos)
        {
            Desc = desc;
            HasWall = hasWall;
            mCenterPos = centerPos;
        }

        internal void OnCaptured(HexCell cell)
        {
            if (Desc is CapturableHexTerrainTileDesc)
            {
                if (null != mActiveGraphicModel)
                {
                    mActiveGraphicModel.IsVisible = true;
                }
                if (null != mInactiveGraphicModel)
                {
                    mInactiveGraphicModel.IsVisible = false;
                }

                if (null == mHexCellCapturedMark)
                {
                    InitHexCellCapturedMark(cell);
                }
                mHexCellCapturedMark.Color = cell.OwningPlayer.Color;

                ((CapturableHexTerrainTileDesc)Desc).OnCaptured(cell);
            }
        }
       
        internal void OnSetOwningPlayerDuringLoad(HexCell cell)
        {
            if (null == cell.OwningPlayer) throw new ArgumentNullException("cell.OwningPlayer");

            if (Desc is CapturableHexTerrainTileDesc)
            {
                if (null != mActiveGraphicModel)
                {
                    mActiveGraphicModel.IsVisible = true;
                }
                InitHexCellCapturedMark(cell);
                mHexCellCapturedMark.Color = cell.OwningPlayer.Color;

                ((CapturableHexTerrainTileDesc)Desc).OnSetOwningPlayerDuringLoad(cell);
            }
        }
        

        internal void OnLost(HexCell cell, GamePlayer newPlayer)
        {
            if (Desc is CapturableHexTerrainTileDesc)
            {
                if (null != mActiveGraphicModel)
                {
                    mActiveGraphicModel.IsVisible = false;
                }
                if (null != mInactiveGraphicModel)
                {
                    mInactiveGraphicModel.IsVisible = true;
                }
                ((CapturableHexTerrainTileDesc)Desc).OnLost(cell);

                if (null == newPlayer && null != mHexCellCapturedMark)
                {
                    ((GameScene)Scene).Octree.DestroyObject(mHexCellCapturedMark);
                    mHexCellCapturedMark = null;
                }

            }
        }

        internal void OnBeginTurn(HexCell cell)
        {
            if (Desc is CapturableHexTerrainTileDesc)
            {
                ((CapturableHexTerrainTileDesc)Desc).OnBeginTurn(cell);
            }
        }

        protected override void OnAddToSceneImpl(BaseScene scene)
        {
            GameScene hexScene = (GameScene)scene;

            Desc.GroundGraphicModel.Position = CenterPos;
            mStaticGeometryIds.Add(hexScene.StaticGeomtery.AddGraphicModel(Desc.GroundGraphicModel));

            if (HasWall)
            {
                Desc.WallGraphicModel.Position = CenterPos;
                mStaticGeometryIds.Add(hexScene.StaticGeomtery.AddGraphicModel(Desc.WallGraphicModel));
            }

            if (null != Desc.StaticGraphicModels)
            {
                foreach (GraphicModel gm in Desc.StaticGraphicModels)
                {
                    gm.Position = CenterPos;
                    mStaticGeometryIds.Add(hexScene.StaticGeomtery.AddGraphicModel(gm));
                }
            }

            if (null != Desc.GraphicModels)
            {
                mGraphicModels = new List<GraphicModel>();

                foreach (GraphicModelDesc gmDesc in Desc.GraphicModels)
                {
                    GraphicModel gm = new GraphicModel(gmDesc, scene.RenderableProvider, scene.Content);
                    //   gm.Position = pos + gmDesc.Position;
                    gm.Position = mCenterPos;
                    hexScene.Octree.AddObject(gm);

                    if (null != gmDesc.Mesh.SkeletalAnimations)
                    {
                        gm.AnimationPlayer.Play(true);
                    }

                    mGraphicModels.Add(gm);
                }
            }

            if (null != Desc.StaticGmConnectionPointAssigments)
            {
                mStaticCpRenderables = new List<Renderable>();
                foreach (var cpa in Desc.StaticGmConnectionPointAssigments)
                {
                    Renderable renderable = hexScene.RenderableProvider.CreateRenderable(cpa.ConnectionPoint.RenderableFactory, cpa.ConnectionPoint.Renderable,
                        scene.Content);
                    var cp = cpa.GraphicModelDesc.Mesh.ConnectionPoints[cpa.ConnectionPoint.ConnectionPoint];
                    renderable.SetTransformation(mCenterPos + cp.LocPosition, cp.LocOrientation);
                    mStaticCpRenderables.Add(renderable);
                    hexScene.Octree.AddObject(renderable);
                }
            }

            if (null != Desc.ActiveGraphicModel)
            {
                mActiveGraphicModel = new GraphicModel(Desc.ActiveGraphicModel, hexScene.RenderableProvider, scene.Content);
                mActiveGraphicModel.IsVisible = false;
                mActiveGraphicModel.Position = CenterPos;
                hexScene.Octree.AddObject(mActiveGraphicModel);
            }
            if (null != Desc.InactiveGraphicModel)
            {
                mInactiveGraphicModel = new GraphicModel(Desc.InactiveGraphicModel, hexScene.RenderableProvider, scene.Content);
                mInactiveGraphicModel.Position = CenterPos;
                hexScene.Octree.AddObject(mInactiveGraphicModel);
            }
        } 

        protected override void OnDispose()
        {
            GameScene hexScene = (GameScene)Scene;
            foreach (var id in mStaticGeometryIds)
            {
                hexScene.StaticGeomtery.RemoveGraphicModel(id);
            }

            if (null != mGraphicModels)
            {
                foreach (var gm in mGraphicModels)
                {
                    hexScene.Octree.DestroyObject(gm);
                }
            }

            if (null != mStaticCpRenderables)
            {
                foreach (var r in mStaticCpRenderables)
                {
                    hexScene.Octree.DestroyObject(r);
                }
            }

            if (null != mActiveGraphicModel)
            {
                hexScene.Octree.DestroyObject(mActiveGraphicModel);
            }
            if (null != mInactiveGraphicModel)
            {
                hexScene.Octree.DestroyObject(mInactiveGraphicModel);
            }
        }

        protected override bool IsSceneValid(BaseScene scene)
        {
            return scene is GameScene;
        }

        private void InitHexCellCapturedMark(HexCell cell)
        {
            mHexCellCapturedMark = new HexCellCapturedMark(Scene.Content);
            mHexCellCapturedMark.Position = cell.CenterPos;
            ((GameScene)Scene).Octree.AddObject(mHexCellCapturedMark);
        }
    }

    class HexCellCapturedMark : GraphicModel, IMaterialEffectParametersUpdater
    {
        private Vector3MaterialEffectParam mColorParam;

        public Vector3 Color { get; set; }

        public HexCellCapturedMark(ContentGroup content)
            :base(CreateMesh(content), CreateMaterial(content))
        {
            if (1 != GraphicModelParts.Count)
            {
                throw new ArgumentException("HexCellCapturedMarkMesh must have only one material part");
            }
            mColorParam = (Vector3MaterialEffectParam)GraphicModelParts[0].Material.MaterialEffect.ManualParameters["gColor"];
            if (null != mColorParam)
            {
                GraphicModelParts[0].CustomMaterialEffectParametersUpdater = this;
            }
        }

        private static Material CreateMaterial(ContentGroup content)
        {
            return content.Load<Material>("HexCellCapturedMarkMat");
        }

        private static Mesh CreateMesh(ContentGroup content)
        {
            return content.Load<Mesh>("HexCellCapturedMarkMesh");
        }

        void IMaterialEffectParametersUpdater.UpdateMaterialEffectParameters()
        {
            mColorParam.Value = Color;
        }
    }
}
