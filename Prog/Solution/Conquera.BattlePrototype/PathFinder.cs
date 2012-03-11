using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Conquera.BattlePrototype
{
    abstract class PathFinder
    {
        #region Types
        class PathNode : IComparable<PathNode>
        {
            Point mPosition;
            PathNode mParent;
            float mF;
            float mG;

            public float DistanceFromStart { get { return mG; } }


            public Point Position
            {
                get { return mPosition; }
            }

            public PathNode Parent
            {
                get { return mParent; }
            }

            public PathNode(Point position, PathNode parent, Point target)
            {
                mPosition = position;
                mParent = parent;
                if (null != parent)
                {
                    mG = parent.mG + 1; //cellCosts[position.X, position.Y];
                }
                else
                {
                    mG = 0;
                }
                int h = HexHelper.GetDistance(position, target);
                mF = mG + h;
            }

            public int CompareTo(PathNode obj)
            {
                if (mF > obj.mF)
                {
                    return -1;
                }
                if (mF < obj.mF)
                {
                    return 1;
                }
                return 0;
            }
        }
        #endregion Types

        HashSet<Point> mClosedList = new HashSet<Point>();
        PriorityQueue<PathNode> mOpenList = new PriorityQueue<PathNode>(30);

        public List<Point> FindPath(Point start, Point end, int maxDistance, HexTerrain hexTerrain)
        {

            List<Point> path = null;
            EecuteForPath(start, end, maxDistance, hexTerrain, node =>
                {
                    path = new List<Point>();
                    //reconstruct
                    while (null != node && null != node.Parent) //null != node.Parent -> start position is not stored in the path
                    {
                        path.Insert(0, node.Position);
                        node = node.Parent;
                    } 
                });
            return path;
        }

        /// <summary>
        /// Gets whether exists a path from start to end, but the actual path is not constructed.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="hexTerrain"></param>
        /// <param name="maxDistance">-1 = no distance constraint</param>
        /// <returns></returns>
        public bool CheckPathExistance(Point start, Point end, int maxDistance, HexTerrain hexTerrain)
        {
            bool found = false;
            EecuteForPath(start, end, maxDistance, hexTerrain, node => found = true);
            return found;
        }

        private void EecuteForPath(Point start, Point end, int maxDistance, HexTerrain hexTerrain, Action<PathNode> targetNode)
        {
            if (end == start)
            {
                return;
            }

            mClosedList.Clear();
            mOpenList.Clear();

            mOpenList.Push(new PathNode(start, null, end));

            while (0 != mOpenList.Count)
            {
                var node = mOpenList.Pop();
                if (!mClosedList.Contains(node.Position)) //not in closed list
                {
                    if (node.Position == end) //found path
                    {
                        targetNode(node);
                        return;
                    }
                    else
                    {
                        if (-1 == maxDistance || node.DistanceFromStart < maxDistance)
                        {
                            hexTerrain.ForEachSibling(node.Position, sibling =>
                            {
                                if (IsTileValid(sibling))
                                {
                                    mOpenList.Push(new PathNode(sibling.Index, node, end));
                                }
                            });
                        }
                        mClosedList.Add(node.Position);
                    }
                }

            }

            return;
        }

        protected abstract bool IsTileValid(HexTerrainTile tile);
    }

    class SimplePathFinder : PathFinder
    {
        private static SimplePathFinder mDefault = null;

        public static SimplePathFinder Default
        {//Not thread safe - todo if necessary
            get
            {
                if (mDefault == null)
                {
                    mDefault = new SimplePathFinder();
                }
                return mDefault;
            }
        }
        protected override bool IsTileValid(HexTerrainTile tile)
        {
            return tile.IsPassableAndEmpty;
        }
    }

    class OccupationIgnoringPathFinder : PathFinder
    {
        private static OccupationIgnoringPathFinder mDefault = null;

        public static OccupationIgnoringPathFinder Default
        {//Not thread safe - todo if necessary
            get
            {
                if (mDefault == null)
                {
                    mDefault = new OccupationIgnoringPathFinder();
                }
                return mDefault;
            }
        }
        protected override bool IsTileValid(HexTerrainTile tile)
        {
            return tile.IsPassable;
        }
    }
}
