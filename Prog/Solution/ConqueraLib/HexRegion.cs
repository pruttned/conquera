using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Ale.Graphics;
using Ale.Scene;

namespace Conquera
{
    public class HexRegion : IDisposable
    {
        class HexCellWrapper
        {
            public HexCell HexCell;

            public HexCellWrapper(HexCell hexCell)
            {
                HexCell = hexCell;
            }
        }

        //promoted colections
        private static Dictionary<Point, HexCellWrapper> FirstCells = new Dictionary<Point, HexCellWrapper>(); //corner index(top corner) - cell
        private static Stack<HexCell> CellsToCheck = new Stack<HexCell>(10);
        private static List<HexCell> Siblings = new List<HexCell>(6);
        private static List<HexCell> RegionCells = new List<HexCell>();

        private bool mIsActive = false;
        private List<BorderRenderable> mBorderRenderables = new List<BorderRenderable>();

        public GamePlayer OwningPlayer { get; private set; }
        public bool mIsDisposed = false;
        public Octree mOctree;

        public bool IsActive
        {
            get { return mIsActive; }
        }

        internal HexRegion(GamePlayer owningPlayer, Octree octree)
        {
            if (null == owningPlayer) throw new ArgumentNullException("owningPlayer");

            OwningPlayer = owningPlayer;
            mOctree = octree;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstCell"></param>
        /// <param name="cells"></param>
        /// <param name="cellsOwnerships">nullable - if null, then information in HexCell is used</param>
        /// <param name="graphicsDevice"></param>
        internal void PropagateRegion(HexCell firstCell, HexCell[,] cells, GamePlayer[,] cellsOwnerships, GraphicsDevice graphicsDevice)
        {
            CellsToCheck.Clear();
            CellsToCheck.Push(firstCell);
            GamePlayer owningPlayer = OwningPlayer;

            bool useCellsOwnerships = (null != cellsOwnerships);

            Siblings.Clear();
            FirstCells.Clear();
            RegionCells.Clear();

            while (0 != CellsToCheck.Count)
            {
                HexCell hexCell = CellsToCheck.Pop();
                if (hexCell.NewRegion != this) //not yet checked
                {
                    hexCell.NewRegion = this;
                    RegionCells.Add(hexCell);
                    OnAddCell(hexCell);

                    //for border
                    if (null != hexCell.HexTerrainTile)
                    {
                        HexCell upRightSibling = hexCell.GetSibling(HexDirection.UperRight);
                        if (null == upRightSibling || (upRightSibling.NewRegion != this && 
                            ((useCellsOwnerships && owningPlayer != cellsOwnerships[upRightSibling.Index.X, upRightSibling.Index.Y])
                            || (!useCellsOwnerships && upRightSibling.OwningPlayer != owningPlayer))))
                        {
                            Point cornerIndex;
                            hexCell.GetCornerIndex(HexTileCorner.Top, out cornerIndex);
                            FirstCells.Add(cornerIndex, new HexCellWrapper(hexCell));
                        }
                    }

                    hexCell.GetSiblings(Siblings);
                    foreach (HexCell sibling in Siblings)
                    {
                        bool siblingHasSameOwner;
                        if (useCellsOwnerships)
                        {
                            Point cellIndex = sibling.Index;
                            siblingHasSameOwner = cellsOwnerships[cellIndex.X, cellIndex.Y] == owningPlayer;
                        }
                        else
                        {
                            siblingHasSameOwner = (sibling.OwningPlayer == owningPlayer);
                        }

                        if (siblingHasSameOwner && sibling.NewRegion != this)
                        {
                            CellsToCheck.Push(sibling);
                        }
                    }
                }
            }

            //Only here is clear whether is regoin active or inactive
            foreach (HexCell cell in RegionCells)
            {
                cell.UpdateRegion();
            }

            GenerateBorder(FirstCells, cells, graphicsDevice);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    foreach (var renderable in mBorderRenderables)
                    {
                        mOctree.DestroyObject(renderable);
                    }
                }
                mIsDisposed = true;

                GC.SuppressFinalize(this);
            }
        }

        private void OnAddCell(HexCell hexCell)
        {
            if(hexCell.IsGap)
            {
                throw new ArgumentException("Region can't include a gap");
            }

            if (hexCell.HexTerrainTile is CastleTileDesc)
            {
                mIsActive = true;
            }
        }

        private void AddBorder(IList<Vector2> points, GraphicsDevice graphicsDevice)
        {
            var borderRenderable = new BorderRenderable(graphicsDevice, points, IsActive ? OwningPlayer.ActiveBorderMaterial : OwningPlayer.InactiveBorderMaterial);
            mOctree.AddObject(borderRenderable);
            mBorderRenderables.Add(borderRenderable);
        }

        private void GenerateBorder(Dictionary<Point, HexCellWrapper> firstCells, HexCell[,] cells, GraphicsDevice graphicsDevice)
        {
            GamePlayer owningPlayer = OwningPlayer;
            List<Vector2> points = new List<Vector2>();
            foreach (var pair in firstCells)
            {
                HexCell firstCell = pair.Value.HexCell;

                if (null != firstCell)
                {
                    Point lastCornerIndex = pair.Key;

                    HexTileCorner firstCorner = HexTileCorner.Top;
                    HexCell curCell = firstCell;
                    HexTileCorner curCorner = firstCorner;

                    Point curCornerIndex;
                    do
                    {
                        //check sibling
                        HexCell sibling = curCell.GetSibling((HexDirection)curCorner);
                        if (null != sibling && null != sibling.HexTerrainTile && cells[sibling.Index.X, sibling.Index.Y].OwningPlayer == owningPlayer)//is same
                        {
                            curCorner = (curCorner == HexTileCorner.Top ? HexTileCorner.UperLeft : curCorner - 1);
                            curCell = sibling;
                        }
                        else
                        {
                            curCorner = (curCorner == HexTileCorner.UperLeft ? HexTileCorner.Top : curCorner + 1);
                        }

                        curCell.GetCornerIndex(curCorner, out curCornerIndex);
                        points.Add(curCell.GetCorner2DPos(curCorner));

                        HexCellWrapper wrapper;
                        if (firstCells.TryGetValue(curCornerIndex, out wrapper))
                        {
                            //can't remove because is in foreach
                            wrapper.HexCell = null;
                        }

                    } while (lastCornerIndex != curCornerIndex);

                    AddBorder(points, graphicsDevice);
                    points.Clear();
                }
            }
        }
    }
}
