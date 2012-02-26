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

        public List<Point> FindPath(Point start, Point end, HexTerrain hexTerrain)
        {
            if (end == start)
            {
                return null;
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
                        List<Point> path = new List<Point>();
                        //reconstruct
                        while (null != node && null != node.Parent) //null != node.Parent -> start position is not stored in the path
                        {
                            path.Insert(0, node.Position);
                            node = node.Parent;
                        }
                        return path;
                    }
                    else
                    {
                        hexTerrain.ForEachSibling(node.Position, sibling =>
                        {
                            if (IsTileValid(sibling))
                            {
                                mOpenList.Push(new PathNode(sibling.Index, node, end));
                            }
                        });

                        mClosedList.Add(node.Position);
                    }
                }

            }

            return null;
        }

        protected abstract bool IsTileValid(HexTerrainTile tile);
    }

    class SimplePathFinder : PathFinder
    {
        protected override bool IsTileValid(HexTerrainTile tile)
        {
            //ignores flying ability!!!
            //return tile.IsPassableAndEmpty;
            return tile.IsPassable;
        }
    }
}
