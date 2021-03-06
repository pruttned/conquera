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
    public class HexTerrainTile : IDisposable
    {
        private List<BatchedModelIdentifier> mStaticGeometryIds = new List<BatchedModelIdentifier>();
        private List<GraphicModel> mGraphicModels;
        private List<Renderable> mStaticCpRenderables;

        private Vector3 mCenterPos;
        private HexTileCapturedMark mHexCellCapturedMark;
        private bool mIsDisposed = false;

        public HexTerrainTileDesc Desc { get; private set; }
        public Vector3 CenterPos
        {
            get { return mCenterPos; }
        }
        public Point Index { get; private set; }
        public BattleScene Scene { get; private set; }

        public bool IsPassable
        {
            //todo - unit .. or no?? - (Unit property will be used ??)
            get { return Desc.IsPassable; }
        }

        public HexTerrainTile(BattleScene scene, Point index, HexTerrainTileDesc desc)
        {
            Scene = scene;
            Index = index;
            Desc = desc;
            mCenterPos = HexHelper.Get3DPosFromIndex(index, HexTerrain.GroundHeight);
        }

        public bool IsSiblingTo(HexTerrainTile tile)
        {
            if (null == tile) throw new ArgumentNullException("tile");

            int i = Index.X;
            int j = Index.Y;

            int i2 = tile.Index.X;
            int j2 = tile.Index.Y;

            return (i2 == i - 1 && j2 == j) ||
                    (i2 == i + 1 && j2 == j) ||
                    ((0 != (j & 1)) && ((i2 == i && j2 == j - 1) || (i2 == i + 1 && j2 == j - 1) || (i2 == i && j2 == j + 1) || (i2 == i + 1 && j2 == j + 1))) ||
                    ((0 == (j & 1)) && ((i2 == i - 1 && j2 == j - 1) || (i2 == i && j2 == j - 1) || (i2 == i - 1 && j2 == j + 1) || (i2 == i && j2 == j + 1)));
        }

        internal void OnCaptured()
        {
            throw new NotImplementedException();
            //todo
            //if (Desc is CapturableHexTerrainTileDesc)
            //{
            //    if (null == mHexCellCapturedMark)
            //    {
            //        InitHexCellCapturedMark(cell);
            //    }
            //    mHexCellCapturedMark.Color = cell.OwningPlayer.Color;

            //    ((CapturableHexTerrainTileDesc)Desc).OnCaptured(cell);
            //}
        }

        internal void OnLost(BattlePlayer newPlayer)
        {
            throw new NotImplementedException();
            //todo    
            //if (Desc is CapturableHexTerrainTileDesc)
            //{
            //    ((CapturableHexTerrainTileDesc)Desc).OnLost(cell);

            //    if (null == newPlayer && null != mHexCellCapturedMark)
            //    {
            //        ((BattleScene)Scene).Octree.DestroyObject(mHexCellCapturedMark);
            //        mHexCellCapturedMark = null;
            //    }
            //}
        }

        internal void OnBeginTurn()
        {
            //todo
            //if (Desc is CapturableHexTerrainTileDesc)
            //{
            //    ((CapturableHexTerrainTileDesc)Desc).OnOwningPlayerBeginTurn(cell);
            //}
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds renderables to scene
        /// Called by HexTerrain after creating all hex terrain tiles. Or after changing a specified tile.
        /// </summary>
        internal void Init()
        {
            if (null != Desc.StaticGraphicModels)
            {
                foreach (GraphicModel gm in Desc.StaticGraphicModels)
                {
                    Vector3 oldPos = gm.Position;
                    gm.Position += CenterPos;
                    mStaticGeometryIds.Add(Scene.StaticGeomtery.AddGraphicModel(gm));
                    gm.Position = oldPos;
                }
            }

            if (null != Desc.NonStaticGraphicModels)
            {
                mGraphicModels = new List<GraphicModel>();

                foreach (GraphicModelDesc gmDesc in Desc.NonStaticGraphicModels)
                {
                    GraphicModel gm = new GraphicModel(gmDesc, Scene.RenderableProvider, Scene.Content);
                    //   gm.Position = pos + gmDesc.Position;
                    gm.Position += mCenterPos;
                    Scene.Octree.AddObject(gm);

                    if (null != gmDesc.Mesh.SkeletalAnimations)
                    {
                        gm.AnimationPlayer.Play(true);
                    }

                    mGraphicModels.Add(gm);
                }
            }

            if (null != Desc.StaticGraphicModelsConnectionPointAssigments)
            {
                mStaticCpRenderables = new List<Renderable>();
                foreach (var cpa in Desc.StaticGraphicModelsConnectionPointAssigments)
                {
                    Renderable renderable = Scene.RenderableProvider.CreateRenderable(cpa.ConnectionPoint.RenderableFactory, cpa.ConnectionPoint.Renderable,
                        Scene.Content);
                    var cp = cpa.GraphicModelDesc.Mesh.ConnectionPoints[cpa.ConnectionPoint.ConnectionPoint];
                    renderable.SetTransformation(mCenterPos + cp.LocPosition, cp.LocOrientation);
                    mStaticCpRenderables.Add(renderable);
                    Scene.Octree.AddObject(renderable);
                }
            }

            OnInit();
        }

        /// <summary>
        /// Called by HexTerrrain to notify that a tile's sibling has been changed
        /// </summary>
        /// <param name="siblingIndex"></param>
        internal void OnSiblingChanged(Point siblingIndex)
        {
            OnSiblingChangedImpl(siblingIndex);
        }
        
        /// <summary>
        /// Called by HexTerrrain to notify that a tile's sibling has been changed
        /// </summary>
        /// <param name="siblingIndex"></param>
        protected virtual void OnSiblingChangedImpl(Point siblingIndex)
        {
        }

        protected virtual void OnInit()
        {
        }
        protected virtual void OnDispose()
        {
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    RemoveFromScene();
                }
                mIsDisposed = true;
            }
        }

        private void InitHexCellCapturedMark()
        {
            mHexCellCapturedMark = new HexTileCapturedMark(Scene.Content);
            mHexCellCapturedMark.Position = CenterPos;
            ((BattleScene)Scene).Octree.AddObject(mHexCellCapturedMark);
        }

        private void RemoveFromScene()
        {
            OnDispose();

            foreach (var id in mStaticGeometryIds)
            {
                Scene.StaticGeomtery.RemoveGraphicModel(id);
            }

            if (null != mGraphicModels)
            {
                foreach (var gm in mGraphicModels)
                {
                    Scene.Octree.DestroyObject(gm);
                }
            }

            if (null != mStaticCpRenderables)
            {
                foreach (var r in mStaticCpRenderables)
                {
                    Scene.Octree.DestroyObject(r);
                }
            }
        }
     }


    public class GapHexTerrainTile : HexTerrainTile
    {
        List<BatchedModelIdentifier> mWalls;
        
        public GapHexTerrainTile(BattleScene scene, Point index, HexTerrainTileDesc desc)
            :base(scene, index, desc)
        {
        }

        protected override void OnInit()
        {
            AddToScene();

            base.OnInit();
        }

        protected override void OnSiblingChangedImpl(Point siblingIndex)
        {
            //slightly ineffective but should be used only in editor
            RemoveFromScene();
            AddToScene();

            base.OnSiblingChangedImpl(siblingIndex);
        }

        private void AddToScene()
        {
            GapHexTerrainTileDesc gapDesc = (GapHexTerrainTileDesc)Desc;

            HexTerrain terrain = Scene.Terrain;

            for (int i = 0; i < 6; ++i)
            {
                var sibling = terrain.GetSibling(Index, (HexDirection)i);
                if (null != sibling && !(sibling is GapHexTerrainTile))
                {
                    if (null == mWalls)
                    {
                        mWalls = new List<BatchedModelIdentifier>();
                    }
                    GraphicModel wallGm = GetRandomWallGraphicModelSample(i);
                    Vector3 oldPos = wallGm.Position;
                    Quaternion oldRot = wallGm.Orientation;
                    wallGm.Position = oldPos + CenterPos;

                    wallGm.Orientation = oldRot * Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(i * (-60) + 60));
                    mWalls.Add(Scene.StaticGeomtery.AddGraphicModel(wallGm));
                    wallGm.SetTransformation(oldPos, oldRot);
                }
            }
        }

        protected override void OnDispose()
        {
            RemoveFromScene();
            base.OnDispose();
        }

        private void RemoveFromScene()
        {
            if (null != mWalls)
            {
                foreach (var wall in mWalls)
                {
                    Scene.StaticGeomtery.RemoveGraphicModel(wall);
                }
            }
            mWalls = null;
        }

        Random rand = new Random();


        private GraphicModel GetRandomWallGraphicModelSample(int direction)
        {
            GapHexTerrainTileDesc gapDesc = (GapHexTerrainTileDesc)Desc;

            //Console.WriteLine());
            int rnd =(int)Math.Pow((float)(Index.X + Index.Y + direction)*12.2364f, 2) % gapDesc.WallSamplePrioritySum;
            //int rnd = rand.Next(gapDesc.WallSamplePrioritySum);
            int index = 0;
            for (index = 0; rnd >= 0; rnd -= gapDesc.WallSamples[index++].Priority);
            return gapDesc.WallSamples[index-1].GraphicModel;
        }

    }



    class HexTileCapturedMark : GraphicModel, IMaterialEffectParametersUpdater
    {
        private Vector3MaterialEffectParam mColorParam;

        public Vector3 Color { get; set; }

        public HexTileCapturedMark(ContentGroup content)
            : base(CreateMesh(content), CreateMaterial(content))
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
