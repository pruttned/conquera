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
using Microsoft.Xna.Framework;
using Ale.Content;
using Ale.Scene;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    public class HexTerrain : BaseDataObject, IDisposable
    {
        //promoted collections
        private static List<HexTerrainTile> Siblings = new List<HexTerrainTile>(6);

        public static float GroundHeight = 0;

        private HexTerrainTile[,] mTiles;
        private BattleScene mScene;
        private bool mIsDisposed = false;

        private string mDefaultTile;

        [DataProperty(NotNull = true)]
        public int Width { get; private set; }
        [DataProperty(NotNull = true)]
        public int Height { get; private set; }

        public Vector3 LowerLeftTileCenter { get; private set; }
        public Vector3 UpperRightTileCenter { get; private set; }

        public HexTerrainTile this[int i, int j]
        {
            get
            {
                return mTiles[i, j];
            }
        }
        public HexTerrainTile this[Point index]
        {
            get
            {
                return mTiles[index.X, index.Y];
            }
        }
        private ContentGroup Content
        {
            get { return mScene.Content; }
        }

        public HexTerrain(int width, int height, string defaultTile, BattleScene scene)
        {
            if (string.IsNullOrEmpty(defaultTile)) throw new ArgumentNullException("defaultTile");

            Width = width;
            Height = height;
            mDefaultTile = defaultTile;
            
            CommonInit(scene);
            InitFromDefaultTile(scene);
        }

        internal void InitAfterLoad(BattleScene scene, OrmManager ormManager)
        {
            CommonInit(scene);

            byte[] data = ormManager.GetBlobData(GetBlobDataName()); //cell data
            int tileCnt = data.Length / 8;
            if (tileCnt != Width * Height)
            {
                throw new ArgumentOutOfRangeException("Blob data has a wrong size");
            }

            mTiles = new HexTerrainTile[Width, Height];
            int k = 0;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    long tileDescId = BitConverter.ToInt64(data, k);
                    HexTerrainTileDesc tileDesc = scene.Content.Load<HexTerrainTileDesc>(tileDescId);
                    mTiles[i, j] = tileDesc.CreateHexTerrainTile(mScene, new Point(i, j));

                    k += 8;
                }
            }

            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    mTiles[i, j].Init();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tile"></param>
        public HexTerrainTile SetTile(Point index, string tile)
        {
            //todo
        //    throw new NotImplementedException("Treba preniest aj jednotku, ownera a tak");
            if (string.IsNullOrEmpty(tile)) throw new ArgumentNullException("tile");

            HexTerrainTile oldHexTerrainTile = mTiles[index.X, index.Y];

            HexTerrainTileDesc tileDesc = Content.Load<HexTerrainTileDesc>(tile);
            if (oldHexTerrainTile.Desc != tileDesc)
            {
                oldHexTerrainTile.Dispose();
                
                mTiles[index.X, index.Y] = tileDesc.CreateHexTerrainTile(mScene, index);
                mTiles[index.X, index.Y].Init();

                //Notify siblings
                Siblings.Clear();
                GetSiblings(index, Siblings);
                foreach (var sibling in Siblings)
                {
                    sibling.OnSiblingChanged(index);
                }
            }

            return mTiles[index.X, index.Y];
        }

        /// <summary>
        /// Gets a tile sibling in a given direction (or null in case of terrain end)
        /// </summary>
        /// <param name="index"></param>
        /// <param name="direction"></param>
        /// <param name="sibling"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public HexTerrainTile GetSibling(Point index, HexDirection direction)
        {
            Point siblingIndex = HexHelper.GetSibling(index, direction);
            if (IsInTerrain(siblingIndex))
            {
                return mTiles[siblingIndex.X, siblingIndex.Y];
            }
            return null;
        }

        /// <summary>
        /// Gets all siblings of a given tile
        /// </summary>
        /// <param name="index"></param>
        /// <param name="siblings"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public void GetSiblings(Point index, IList<HexTerrainTile> siblings)
        {
            int i = index.X;
            int j = index.Y;

            TryAddSibling(i - 1, j, siblings);
            TryAddSibling(i + 1, j, siblings);
            if (0 != (j & 1))
            {
                TryAddSibling(i, j - 1, siblings);
                TryAddSibling(i + 1, j - 1, siblings);
                TryAddSibling(i, j + 1, siblings);
                TryAddSibling(i + 1, j + 1, siblings);
            }
            else
            {
                TryAddSibling(i - 1, j - 1, siblings);
                TryAddSibling(i, j - 1, siblings);
                TryAddSibling(i - 1, j + 1, siblings);
                TryAddSibling(i, j + 1, siblings);
            }
        }

        /// <summary>
        /// Gets all siblings of a given tile
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public IList<HexTerrainTile> GetSiblings(Point index)
        {
            List<HexTerrainTile> siblings = new List<HexTerrainTile>();
            GetSiblings(index, siblings);
            return siblings;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsInTerrain(Point index)
        {
            return (index.X >= 0 && index.X < Width && index.Y >= 0 & index.Y < Height);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    if (null != mTiles)
                    {
                        for (int i = 0; i < Width; ++i)
                        {
                            for (int j = 0; j < Height; ++j)
                            {
                                mTiles[i, j].Dispose();
                            }
                        }
                    }
                }
                mIsDisposed = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ormManager"></param>
        /// <param name="isNew"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        protected override void AfterSaveImpl(OrmManager ormManager, bool isNew)
        {
            byte[] data = new byte[Width * Height * 8];
            int k = 0;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    long descId = mTiles[i, j].Desc.Id;
                    if (descId <= 0)
                    {
                        throw new ArgumentException(string.Format("Tile [{0},{1}] has a wrong desc id {2}", i, j, descId));
                    }
                    byte[] descIdBytes = BitConverter.GetBytes(descId);
                    Array.Copy(descIdBytes, 0, data, k, descIdBytes.Length);
                    k += 8;
                }
            }

            ormManager.SetBlobData(GetBlobDataName(), data);
            base.AfterSaveImpl(ormManager, isNew);
        }

        private HexTerrain()
        { }

        private void CommonInit(BattleScene scene)
        {
            mScene = scene;
            LowerLeftTileCenter = HexHelper.Get3DPosFromIndex(new Point(0, 0), GroundHeight);
            UpperRightTileCenter = HexHelper.Get3DPosFromIndex(new Point(Width, Height), GroundHeight);
        }

        private void TryAddSibling(int i, int j, IList<HexTerrainTile> siblings)
        {
            if (i >= 0 && i < Width && j >= 0 && j < Height)
            {
                siblings.Add(this[i, j]);
            }
        }

        private void InitFromDefaultTile(BattleScene scene)
        {
            HexTerrainTileDesc tileDesc = scene.Content.Load<HexTerrainTileDesc>(mDefaultTile);
            mTiles = new HexTerrainTile[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    mTiles[i, j] = tileDesc.CreateHexTerrainTile(mScene, new Point(i, j));
                }
            }
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    mTiles[i, j].Init();
                }
            }
        }

        private string GetBlobDataName()
        {
            return string.Format("HexTerrain_{0}", Id);
        }
    }
}
