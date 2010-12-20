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
        public Vector3 mCenterPos;

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

        internal void OnActivated(HexCell cell)
        {
            if (null != mActiveGraphicModel)
            {
                mActiveGraphicModel.IsVisible = true;
            }
            if (null != mInactiveGraphicModel)
            {
                mInactiveGraphicModel.IsVisible = false;
            }
            Desc.OnActivated(cell);
        }

        internal void OnDeactivated(HexCell cell)
        {
            if (null != mActiveGraphicModel)
            {
                mActiveGraphicModel.IsVisible = false;
            }
            if (null != mInactiveGraphicModel)
            {
                mInactiveGraphicModel.IsVisible = true;
            }
            Desc.OnDeactivated(cell);
        }

        internal void OnBeginTurn(HexCell cell)
        {
            Desc.OnBeginTurn(cell);
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
                mActiveGraphicModel = new GraphicModel(Desc.ActiveGraphicModel, scene.Content);
                mActiveGraphicModel.IsVisible = false;
                mActiveGraphicModel.Position = CenterPos;
                hexScene.Octree.AddObject(mActiveGraphicModel);
            }
            if (null != Desc.InactiveGraphicModel)
            {
                mInactiveGraphicModel = new GraphicModel(Desc.InactiveGraphicModel, scene.Content);
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
    }

    public enum TileType
    {
        Land = 0,
        Castle = 1,
        Gold = 2,
        Food = 3,
        Shrine = 4,
        Temple = 5,
        Special = 6
    }
}
