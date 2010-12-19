using System;
using System.Collections.Generic;
using System.Text;
using SimpleOrmFramework;
using Microsoft.Xna.Framework;
using Ale.Content;

namespace Conquera
{
    [DataObject(MaxCachedCnt = 0)]
    public class HexTerrain : BaseDataObject, IDisposable
    {
        private HexTerrainTile[,] mTiles;
        private GameScene mScene;
        private bool mIsDisposed = false;

        public static float TileR = 1.0f;
        public static float TileW = 1.73205f; // mHalfW * 2
        public static float TileH = 1.5f;//1.5*r
        public static float GroundHeight = 0;
        public static float HalfTileR = 0.5f;
        public static float HalfTileW = 0.866025f; // cos30*r

        private string mDefaultTile;

        [DataProperty(NotNull = true)]
        public int Width { get; private set; }
        [DataProperty(NotNull = true)]
        public int Height { get; private set; }

        public Vector3 LowerLeftTileCenter { get; private set; }
        public Vector3 UpperRightTileCenter { get; private set; }

        private ContentGroup Content
        {
            get { return mScene.Content; }
        }

        internal HexTerrain(int width, int height, string defaultTile, GameScene scene)
        {
            if (string.IsNullOrEmpty(defaultTile)) throw new ArgumentNullException("defaultTile");

            Width = width;
            Height = height;
            mDefaultTile = defaultTile;
            
            CommonInit(scene);
            InitFromDefaultTile(scene);
        }

        internal void InitAfterLoad(GameScene scene, OrmManager ormManager)
        {
            CommonInit(scene);

            byte[] data = ormManager.GetBlobData(GetBlobDataName()); //cell data
            int tileCnt = data.Length / 8;
            if (tileCnt != Width * Height)
            {
                throw new ArgumentOutOfRangeException("Blob data has a wrong size");
            }
            long[,] tiles = new long[Width, Height];
            int k = 0;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    tiles[i, j] = BitConverter.ToInt64(data, k);
                    k += 8;
                }
            }

            mTiles = new HexTerrainTile[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    long tileDescId = tiles[i, j];
                    if (0 < tileDescId)
                    {
                        HexTerrainTileDesc tileDesc = scene.Content.Load<HexTerrainTileDesc>(tileDescId);
                        HexTerrainTile tile = new HexTerrainTile(tileDesc, TileNeedWall(i, j, tiles), GetPosFromIndex(new Point(i, j)));
                        mTiles[i, j] = tile;
                        tile.OnAddToScene(scene);
                    }
                }
            }
        }


        /// <summary>
        /// Tile or null for a gap
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        internal HexTerrainTile GetTile(Point index)
        {
            return mTiles[index.X, index.Y];
        }

        public bool IsTilePassable(Point index)
        {
            var tile = GetTile(index);
            if (null == tile)
            {
                return false;
            }
            return tile.Desc.IsPassable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tile">Null = gap</param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        internal HexTerrainTile SetTile(Point index, string tile)
        {
            HexTerrainTile oldHexTerrainTile = mTiles[index.X, index.Y];
            if (string.IsNullOrEmpty(tile))
            {
                if (null != oldHexTerrainTile)
                {
                    HexTerrainTileDesc oldDesc = oldHexTerrainTile.Desc;
                    oldHexTerrainTile.Dispose();

                    mTiles[index.X, index.Y] = null;

                    //update siblings
                    foreach (Point pos in GetSiblingsPos(index, false))
                    {
                        SetTileWall(pos.X, pos.Y, true);
                    }
                }
            }
            else
            {
                HexTerrainTileDesc tileDesc = Content.Load<HexTerrainTileDesc>(tile);
                if (null == oldHexTerrainTile || oldHexTerrainTile.Desc != tileDesc)
                {
                    bool wall = TileNeedWall(index.X, index.Y);
                    HexTerrainTile hexTerrainTile = new HexTerrainTile(tileDesc, wall, GetPosFromIndex(index));
                    mTiles[index.X, index.Y] = hexTerrainTile;

                    HexTerrainTileDesc oldDesc = null;

                    if (null != oldHexTerrainTile)
                    {
                        oldDesc = oldHexTerrainTile.Desc;
                        oldHexTerrainTile.Dispose();
                    }
                    else
                    {
                        //update siblings
                        foreach (Point pos in GetSiblingsPos(index, false))
                        {
                            SetTileWall(pos.X, pos.Y, TileNeedWall(pos.X, pos.Y));
                        }
                    }

                    hexTerrainTile.OnAddToScene(mScene);
                }
            }

            return mTiles[index.X, index.Y];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="includeGaps"></param>
        /// <param name="direction"></param>
        /// <param name="sibling"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public bool GetSiblingPos(Point index, bool includeGaps, HexDirection direction, out Point sibling)
        {
            sibling = GetSiblingPos(index, direction);
            if (IsInTerrain(sibling))
            {
                return (includeGaps || null != mTiles[index.X, index.Y]);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="includeGaps"></param>
        /// <param name="siblings"></param>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public void GetSiblingsPos(Point index, bool includeGaps, IList<Point> siblings)
        {
            int i = index.X;
            int j = index.Y;

            TryAddSiblingPos(i - 1, j, includeGaps, siblings);
            TryAddSiblingPos(i + 1, j, includeGaps, siblings);
            if (0 != (j & 1))
            {
                TryAddSiblingPos(i, j - 1, includeGaps, siblings);
                TryAddSiblingPos(i + 1, j - 1, includeGaps, siblings);
                TryAddSiblingPos(i, j + 1, includeGaps, siblings);
                TryAddSiblingPos(i + 1, j + 1, includeGaps, siblings);
            }
            else
            {
                TryAddSiblingPos(i - 1, j - 1, includeGaps, siblings);
                TryAddSiblingPos(i, j - 1, includeGaps, siblings);
                TryAddSiblingPos(i - 1, j + 1, includeGaps, siblings);
                TryAddSiblingPos(i, j + 1, includeGaps, siblings);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="includeGaps"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">HexTerrain has not yet been initialized</exception>
        public IList<Point> GetSiblingsPos(Point index, bool includeGaps)
        {
            List<Point> siblings = new List<Point>();
            GetSiblingsPos(index, includeGaps, siblings);
            return siblings;
        }

        public static Vector3 GetPosFromIndex(Point index)
        {
            Vector3 pos;
            GetPosFromIndex(index, out pos);
            return pos;
        }

        public static void GetPosFromIndex(Point index, out Vector3 pos)
        {
            //http://www.gamedev.net/reference/articles/article747.asp
            //PlotX=MapX*Width+(MapY AND 1)*(Width/2)
            //HeightOverLapping=)*0.75
            pos = new Vector3(
                index.X * TileW + (index.Y & 1) * HalfTileW,
                index.Y * TileH, GroundHeight);
        }

        public Vector2 Get2DPosFromIndex(Point index)
        {
            Vector2 pos;
            Get2DPosFromIndex(index, out pos);
            return pos;
        }

        public void Get2DPosFromIndex(Point index, out Vector2 pos)
        {
            //http://www.gamedev.net/reference/articles/article747.asp
            //PlotX=MapX*Width+(MapY AND 1)*(Width/2)
            //HeightOverLapping=)*0.75
            pos = new Vector2(
                index.X * TileW + (index.Y & 1) * HalfTileW,
                index.Y * TileH);
        }

        public bool GetIndexFromPos(float x, float y, out Point index)
        {
            index = new Point();

            x += HalfTileW;
            y += HalfTileR;

            if (x < 0 || y < -HalfTileR)
            {
                return false;
            }

            //must be floor to handle (-1,0)
            int j = (int)Math.Floor(y / TileH);
            int i = (int)Math.Floor((x - (j & 1) * HalfTileW) / TileW);

            float inRecX = x - (i * TileW + (j & 1) * HalfTileW);
            float inRecY = y - (j * TileH);


            if (inRecY < TileR)
            {
                index.X = i;
                index.Y = j;
            }
            else
            {
                if (inRecX < HalfTileW)
                {
                    float my = TileR + (TileH - TileR) * (inRecX / HalfTileW);
                    if (inRecY < my)
                    {
                        index.X = i;
                        index.Y = j;
                    }
                    else
                    {
                        if (0 == (j & 1))
                        {
                            index.X = i - 1;
                        }
                        else
                        {
                            index.X = i;
                        }
                        index.Y = j + 1;
                    }
                }
                else
                {
                    float my = TileH + (TileR - TileH) * ((inRecX / HalfTileW) - 1);
                    if (inRecY < my)
                    {
                        index.X = i;
                        index.Y = j;
                    }
                    else
                    {
                        if (0 == (j & 1))
                        {
                            index.X = i;
                        }
                        else
                        {
                            index.X = i + 1;
                        }
                        index.Y = j + 1;
                    }
                }
            }

            return (index.X >= 0 && index.X < Width && index.Y >= 0 && index.Y < Height);
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
                                if (null != mTiles[i, j])
                                {
                                    mTiles[i, j].Dispose();
                                }
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
                    if (null != mTiles[i, j])
                    {
                        long descId = mTiles[i, j].Desc.Id;
                        if (descId <= 0)
                        {
                            throw new ArgumentException(string.Format("Tile [{0},{1}] has a wrong desc id {2}", i, j, descId));
                        }
                        byte[] descIdBytes = BitConverter.GetBytes(descId);
                        Array.Copy(descIdBytes, 0, data, k, descIdBytes.Length);
                    }
                    k += 8;
                }
            }

            ormManager.SetBlobData(GetBlobDataName(), data);
            base.AfterSaveImpl(ormManager, isNew);
        }

        private HexTerrain()
        { }

        private void CommonInit(GameScene scene)
        {
            mScene = scene;
            LowerLeftTileCenter = GetPosFromIndex(new Point(0, 0));
            UpperRightTileCenter = GetPosFromIndex(new Point(Width, Height));
        }

        private Point GetSiblingPos(Point index, HexDirection direction)
        {
            int i = index.X;
            int j = index.Y;

            switch (direction)
            {
                case HexDirection.UperRight:
                    return (0 != (j & 1)) ? new Point(i + 1, j + 1) : new Point(i, j + 1);
                case HexDirection.Right:
                    return new Point(i + 1, j);
                case HexDirection.LowerRight:
                    return (0 != (j & 1)) ? new Point(i + 1, j - 1) : new Point(i, j - 1);
                case HexDirection.LowerLeft:
                    return (0 != (j & 1)) ? new Point(i, j - 1) : new Point(i - 1, j - 1);
                case HexDirection.Left:
                    return new Point(i - 1, j);
                case HexDirection.UperLeft:
                    return (0 != (j & 1)) ? new Point(i, j + 1) : new Point(i - 1, j + 1);
                default:
                    throw new ArgumentException("Invalid direction");
            }
        }


        private void TryAddSiblingPos(int i, int j, bool includeGaps, IList<Point> siblings)
        {
            if (i >= 0 && i < Width && j >= 0 && j < Height)
            {
                if (includeGaps || null != mTiles[i, j])
                {
                    siblings.Add(new Point(i, j));
                }
            }
        }

        private void SetTileWall(int i, int j, bool wall)
        {
            HexTerrainTile oldHexTerrainTile = mTiles[i, j];
            if (null != oldHexTerrainTile && oldHexTerrainTile.HasWall != wall)
            {
                HexTerrainTile hexTerrainTile = new HexTerrainTile(oldHexTerrainTile.Desc, wall, GetPosFromIndex(new Point(i, j)));
                oldHexTerrainTile.Dispose();
                mTiles[i, j] = hexTerrainTile;
                hexTerrainTile.OnAddToScene(mScene);
            }
        }

        private bool IsGap(int i, int j)
        {
            return (i < 0 || i >= Width || j < 0 || j >= Height || null == mTiles[i, j]);
        }

        private bool TileNeedWall(int i, int j)
        {
            return (IsGap(i - 1, j) ||
                    IsGap(i + 1, j) ||
                    ((0 != (j & 1)) && (
                        IsGap(i, j - 1) ||
                        IsGap(i + 1, j - 1) ||
                        IsGap(i, j + 1) ||
                        IsGap(i + 1, j + 1))) ||
                    ((0 == (j & 1)) && (
                        IsGap(i - 1, j - 1) ||
                        IsGap(i, j - 1) ||
                        IsGap(i - 1, j + 1) ||
                        IsGap(i, j + 1)
                    )));
        }

        private bool IsGap(int i, int j, long[,] tiles)
        {
            return (i < 0 || i >= Width || j < 0 || j >= Height || 0 >= tiles[i, j]);
        }

        private bool TileNeedWall(int i, int j, long[,] tiles)
        {
            return (IsGap(i - 1, j, tiles) ||
                    IsGap(i + 1, j, tiles) ||
                    ((0 != (j & 1)) && (
                        IsGap(i, j - 1, tiles) ||
                        IsGap(i + 1, j - 1, tiles) ||
                        IsGap(i, j + 1, tiles) ||
                        IsGap(i + 1, j + 1, tiles))) ||
                    ((0 == (j & 1)) && (
                        IsGap(i - 1, j - 1, tiles) ||
                        IsGap(i, j - 1, tiles) ||
                        IsGap(i - 1, j + 1, tiles) ||
                        IsGap(i, j + 1, tiles)
                    )));
        }

        private void InitFromDefaultTile(GameScene scene)
        {
            HexTerrainTileDesc tileDesc = scene.Content.Load<HexTerrainTileDesc>(mDefaultTile);
            mTiles = new HexTerrainTile[Width, Height];
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Height; ++j)
                {
                    bool wall = (i == 0 || j == 0 || i == Width - 1 || j == Height - 1);
                    HexTerrainTile tile = new HexTerrainTile(tileDesc, wall, GetPosFromIndex(new Point(i, j)));
                    mTiles[i, j] = tile;
                    tile.OnAddToScene(scene);
                }
            }
        }
        private string GetBlobDataName()
        {
            return string.Format("HexTerrain_{0}", Id);
        }
    }

    public enum HexDirection
    {
        UperRight = 0,
        Right = 1,
        LowerRight = 2,
        LowerLeft = 3,
        Left = 4,
        UperLeft = 5
    }
}
