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
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace Conquera.BattlePrototype
{
    //public class AiBattlePlayer : BattlePlayer
    //{
    //    Dictionary<HexTerrainTile, AttackMapCell> mAttackMap = new Dictionary<HexTerrainTile, AttackMapCell>();
    //    Dictionary<HexTerrainTile, DefenseMapCell> mDefenseMap = new Dictionary<HexTerrainTile, DefenseMapCell>();
    //    HashSet<Microsoft.Xna.Framework.Point> mPossibleMoves = new HashSet<Microsoft.Xna.Framework.Point>();
    //    List<AttackMapCell> mUnitPossibleAtackPositions = new List<AttackMapCell>();
    //    List<BattleUnit> mAttackingUnits = new List<BattleUnit>();
    //    List<BattleUnit> mUnitsToMove = new List<BattleUnit>();
    //    List<DefenseMapCell> mPossibleDefensePositions = new List<DefenseMapCell>();

    //    Window1 mWindow;

    //    SimplePathFinder mPathFinder = new SimplePathFinder();

    //    public AiBattlePlayer(Color color, int index, Window1 window)
    //        : base(color, index)
    //    {
    //        mWindow = window;
    //    }
    //    protected override void OnTurnStartImpl(int turnNum, bool isActive)
    //    {
    //        if (isActive)
    //        {
    //            Stopwatch stp = new Stopwatch();
    //            stp.Start();

    //            Microsoft.Xna.Framework.Point center = new Microsoft.Xna.Framework.Point(9, 6);
    //            UpdateAttackMap();

    //            mAttackingUnits.Clear();
    //            mUnitsToMove.Clear();

    //            //attacks
    //            foreach (var unit in Units)
    //            {
    //                if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //                {
    //                    mPossibleMoves.Clear();
    //                    unit.GetPossibleMoves(mPossibleMoves);

    //                    if (0 < mPossibleMoves.Count)
    //                    {
    //                        mUnitPossibleAtackPositions.Clear();

    //                        //unit's possible attack positions
    //                        foreach (var pos in mPossibleMoves)
    //                        {
    //                            var tile = mWindow.Terrain[pos];
    //                            AttackMapCell attackMapCell;
    //                            if (mAttackMap.TryGetValue(tile, out attackMapCell))
    //                            {
    //                                if (attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.5 < unit.BaseDefense+unit.Hp)
    //                                {
    //                                    mUnitPossibleAtackPositions.Add(attackMapCell);
    //                                }
    //                            }
    //                        }
    //                        if (0 < mUnitPossibleAtackPositions.Count)
    //                        {
    //                            int i = MathExt.Random.Next(mUnitPossibleAtackPositions.Count);
    //                            unit.Move(mUnitPossibleAtackPositions[i].Tile.Index);
    //                            mAttackingUnits.Add(unit);
    //                        }
    //                        else
    //                        {
    //                            mUnitsToMove.Add(unit);
    //                        }
    //                    }
    //                }
    //            }

    //            if (0 < mUnitsToMove.Count)
    //            {
    //                if (0 < mAttackingUnits.Count)
    //                {
    //                    foreach (var attUnit in mAttackingUnits)
    //                    {
    //                        //max damage for unit
    //                        //abilities (first strike/berseker are not considered here)
    //                        int damage = 0;
    //                        int enemiesCnt = 0;
    //                        mWindow.Terrain.ForEachSibling(attUnit.TileIndex,
    //                            sibling =>
    //                            {
    //                                if (null != sibling.Unit && sibling.Unit.Player != this)
    //                                {
    //                                    damage += sibling.Unit.Attack.Y;
    //                                    enemiesCnt++;
    //                                }
    //                            });
    //                        int maxDamage = damage - attUnit.Defense;

    //                        mWindow.Terrain.ForEachSibling(attUnit.TileIndex,
    //                            sibling =>
    //                            {
    //                                if (sibling.IsPassableAndEmpty)
    //                                {
    //                                    DefenseMapCell defenseMapCell;
    //                                    if (!mDefenseMap.TryGetValue(sibling, out defenseMapCell))
    //                                    {
    //                                        defenseMapCell = new DefenseMapCell(sibling);
    //                                        mDefenseMap[sibling] = defenseMapCell;
    //                                    }

    //                                    defenseMapCell.DefenseReqIndex += maxDamage + enemiesCnt;
    //                                }
    //                            });
    //                    }
    //                }
    //                foreach (var unit in mUnitsToMove)
    //                {
    //                    if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //                    {
    //                        mPossibleMoves.Clear();
    //                        unit.GetPossibleMoves(mPossibleMoves);

    //                        if (0 != mDefenseMap.Count)
    //                        {
    //                            mPossibleDefensePositions.Clear();

    //                            //unit's possible defense positions
    //                            foreach (var pos in mPossibleMoves)
    //                            {
    //                                var tile = mWindow.Terrain[pos];
    //                                DefenseMapCell defenseMapCell;
    //                                if (mDefenseMap.TryGetValue(tile, out defenseMapCell))
    //                                {
    //                                    AttackMapCell attackMapCell;
    //                                    if (!mAttackMap.TryGetValue(tile, out attackMapCell) || attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.5 < unit.BaseDefense + unit.Hp)
    //                                    {
    //                                        mPossibleDefensePositions.Add(defenseMapCell);
    //                                    }
    //                                }
    //                            }
    //                            if (0 < mPossibleDefensePositions.Count)
    //                            {
    //                                mPossibleDefensePositions.OrderBy(d => d.DefenseReqIndex);
    //                                unit.Move(mPossibleDefensePositions[0].Tile.Index);
    //                                mDefenseMap.Remove(mPossibleDefensePositions[0].Tile);
    //                            }
    //                        }

    //                        if (!unit.HasMovedThisTurn)
    //                        {
    //                            var pathToCenter = mPathFinder.FindPath(unit.TileIndex, center, mWindow.Terrain);
    //                            if (null != pathToCenter)
    //                            {
    //                                Point pathPoint;
    //                                if (pathToCenter.Count > unit.MovementDistance)
    //                                {
    //                                    pathPoint = pathToCenter[unit.MovementDistance];
    //                                }
    //                                else
    //                                {
    //                                    pathPoint = pathToCenter[pathToCenter.Count - 1]; ;
    //                                }
    //                                Point movePoint = new Point();
    //                                int minDist = -1;
    //                                foreach (var possibleMove in mPossibleMoves)
    //                                {
    //                                    int dist = HexHelper.GetDistance(possibleMove, pathPoint);
    //                                    if (-1 == minDist || minDist > dist)
    //                                    {
    //                                        minDist = dist;
    //                                        movePoint = possibleMove;
    //                                    }
    //                                }
    //                                unit.Move(movePoint);
    //                            }
    //                        }
    //                    }
    //                }
    //            }



    //            //{
    //            //    List<BattleUnit> attackingUnits = new List<BattleUnit>();
    //            //    //  var enemiesInRange = new List<BattleUnit>();
    //            //    foreach (var unit in Units)
    //            //    {
    //            //        if (unit.HasEnabledMovement && !unit.HasMovedThisTurn)
    //            //        {
    //            //            //enemiesInRange.Clear();
    //            //            //unit.ForEachEnemyInRange(
    //            //            //    enemy => 
    //            //            //    {
    //            //            //        enemiesInRange.Add(enemy);
    //            //            //    });
    //            //            //if (0 < enemiesInRange.Count)
    //            //            //{

    //            //            //}
    //            //            //else
    //            //            {
    //            //                mPossibleMoves.Clear();
    //            //                unit.GetPossibleMoves(mPossibleMoves);

    //            //                if (0 < mPossibleMoves.Count)
    //            //                {
    //            //                    mUnitPossibleAtackPositions.Clear();

    //            //                    //unit's possible attack positions
    //            //                    foreach (var pos in mPossibleMoves)
    //            //                    {
    //            //                        var tile = mWindow.Terrain[pos];
    //            //                        AttackMapCell attackMapCell;
    //            //                        if (mAttackMap.TryGetValue(tile, out attackMapCell))
    //            //                        {
    //            //                            if (attackMapCell.MinDmg + (attackMapCell.MaxDmg - attackMapCell.MinDmg) * 0.7 < unit.BaseDefense)
    //            //                            {
    //            //                                mUnitPossibleAtackPositions.Add(attackMapCell);
    //            //                            }

    //            //                        }
    //            //                    }
    //            //                    if (0 < mUnitPossibleAtackPositions.Count)
    //            //                    {
    //            //                        int i = MathExt.Random.Next(mUnitPossibleAtackPositions.Count);
    //            //                        unit.Move(mUnitPossibleAtackPositions[i].Tile.Index);
    //            //                        attackingUnits.Add(unit);
    //            //                    }
    //            //                    else
    //            //                    {
    //            //                        //defense support
    //            //                        foreach (var attackingUnit in attackingUnits)
    //            //                        {

    //            //                        }



    //            //                        var pathToCenter = mPathFinder.FindPath(unit.TileIndex, center, mWindow.Terrain);
    //            //                        if (null != pathToCenter)
    //            //                        {
    //            //                            Point pathPoint;
    //            //                            if (pathToCenter.Count > unit.MovementDistance)
    //            //                            {
    //            //                                pathPoint = pathToCenter[unit.MovementDistance];
    //            //                            }
    //            //                            else
    //            //                            {
    //            //                                pathPoint = pathToCenter[pathToCenter.Count - 1]; ;
    //            //                            }
    //            //                            Point movePoint = new Point();
    //            //                            int minDist = -1;
    //            //                            foreach (var possibleMove in mPossibleMoves)
    //            //                            {
    //            //                                int dist = HexHelper.GetDistance(possibleMove, pathPoint);
    //            //                                if (-1 == minDist || minDist > dist)
    //            //                                {
    //            //                                    minDist = dist;
    //            //                                    movePoint = possibleMove;
    //            //                                }
    //            //                            }
    //            //                            unit.Move(movePoint);
    //            //                        }


    //            //                        //mPossibleMoves.Sort((a, b) => Comparer<int>.Default.Compare(Math.Abs(a.X - center.X) + Math.Abs(a.Y - center.Y), Math.Abs(b.X - center.X) + Math.Abs(b.Y - center.Y)));
    //            //                        //int i = 0;
    //            //                        //for (; i < mPossibleMoves.Count && MathExt.Random.NextDouble() < 0.5; ++i) { }
    //            //                        //if (mPossibleMoves.Count <= i)
    //            //                        //{
    //            //                        //    i = MathExt.Random.Next(mPossibleMoves.Count);
    //            //                        //}
    //            //                        //unit.Move(mPossibleMoves[i]);
    //            //                    }
    //            //                }
    //            //            }
    //            //        }
    //            //    }
    //            //}

    //            stp.Stop();
    //            Logger.Log(stp.ElapsedMilliseconds);

    //            mWindow.EndTurn();
    //        }
    //    }

    //    void UpdateAttackMap()
    //    {
    //        mAttackMap.Clear(); //todo AttackMapCell pool?
    //        foreach (var player in mWindow.Players)
    //        {
    //            if (player != this)
    //            {
    //                foreach (var unit in player.Units)
    //                {
    //                    mWindow.Terrain.ForEachSibling(unit.TileIndex,
    //                        sibling =>
    //                        {
    //                            if (sibling.IsPassableAndEmpty)
    //                            {
    //                                AttackMapCell attackMapCell;
    //                                if (!mAttackMap.TryGetValue(sibling, out attackMapCell))
    //                                {
    //                                    attackMapCell = new AttackMapCell(sibling);
    //                                    mAttackMap[sibling] = attackMapCell;
    //                                }
    //                                attackMapCell.MinDmg += unit.Attack.X;
    //                                attackMapCell.MaxDmg += unit.Attack.Y;
    //                                attackMapCell.Targets.Add(unit);
    //                            }
    //                        });
    //                }
    //            }
    //        }
    //    }


    //}

    //class AttackMapCell
    //{
    //    public HexTerrainTile Tile;
    //    public int MinDmg;
    //    public int MaxDmg;
    //    public List<BattleUnit> Targets;

    //    public AttackMapCell(HexTerrainTile tile)
    //    {
    //        Tile = tile;
    //        Targets = new List<BattleUnit>();
    //    }
    //}

    //class DefenseMapCell
    //{
    //    public HexTerrainTile Tile;
    //    public int DefenseReqIndex;

    //    public DefenseMapCell(HexTerrainTile tile)
    //    {
    //        Tile = tile;
    //    }
    //}

    //class SimplePathFinder : PathFinder
    //{
    //    protected override bool IsTileValid(HexTerrainTile tile)
    //    {
    //        //ignores flying ability!!!
    //        //return tile.IsPassableAndEmpty;
    //        return tile.IsPassable;
    //    }
    //}

    //abstract class PathFinder
    //{
    //    #region Types
    //    class PathNode : IComparable<PathNode>
    //    {
    //        Point mPosition;
    //        PathNode mParent;
    //        float mF;
    //        float mG;

    //        public Point Position
    //        {
    //            get { return mPosition; }
    //        }

    //        public PathNode Parent
    //        {
    //            get { return mParent; }
    //        }

    //        public PathNode(Point position, PathNode parent, Point target)
    //        {
    //            mPosition = position;
    //            mParent = parent;
    //            if (null != parent)
    //            {
    //                mG = parent.mG + 1; //cellCosts[position.X, position.Y];
    //            }
    //            else
    //            {
    //                mG = 0;
    //            }
    //            int h = HexHelper.GetDistance(position, target);
    //            mF = mG + h;
    //        }

    //        public int CompareTo(PathNode obj)
    //        {
    //            if (mF > obj.mF)
    //            {
    //                return -1;
    //            }
    //            if (mF < obj.mF)
    //            {
    //                return 1;
    //            }
    //            return 0;
    //        }
    //    }
    //    #endregion Types

    //    HashSet<Point> mClosedList = new HashSet<Point>();
    //    PriorityQueue<PathNode> mOpenList = new PriorityQueue<PathNode>(30);

    //    public List<Point> FindPath(Point start, Point end, HexTerrain hexTerrain)
    //    {
    //        if (end == start)
    //        {
    //            return null;
    //        }

    //        mClosedList.Clear();
    //        mOpenList.Clear();

    //        mOpenList.Push(new PathNode(start, null, end));

    //        while (0 != mOpenList.Count)
    //        {
    //            var node = mOpenList.Pop();
    //            if (!mClosedList.Contains(node.Position)) //not in closed list
    //            {
    //                if (node.Position == end) //found path
    //                {
    //                    List<Point> path = new List<Point>();
    //                    //reconstruct
    //                    while (null != node && null != node.Parent) //null != node.Parent -> start position is not stored in the path
    //                    {
    //                        path.Insert(0, node.Position);
    //                        node = node.Parent;
    //                    }
    //                    return path;
    //                }
    //                else
    //                {
    //                    hexTerrain.ForEachSibling(node.Position, sibling =>
    //                    {
    //                        if (IsTileValid(sibling))
    //                        {
    //                            mOpenList.Push(new PathNode(sibling.Index, node, end));
    //                        }
    //                    });

    //                    mClosedList.Add(node.Position);
    //                }
    //            }

    //        }

    //        return null;
    //    }

    //    protected abstract bool IsTileValid(HexTerrainTile tile);

    //}



    //class PriorityQueue<T> : ICollection<T> where T : IComparable<T>
    //{
    //    List<T> mInnerList;

    //    public int Count
    //    {
    //        get { return mInnerList.Count; }
    //    }
    //    public bool IsReadOnly
    //    {
    //        get { return false; ; }
    //    }

    //    public PriorityQueue()
    //    {
    //        mInnerList = new List<T>();
    //    }

    //    public PriorityQueue(int capacity)
    //    {
    //        mInnerList = new List<T>(capacity);
    //    }

    //    public void Push(T item)
    //    {
    //        if (null == item) throw new ArgumentNullException("item");

    //        mInnerList.Insert(FindPosFor(item), item);
    //    }

    //    public T Pop()
    //    {
    //        int lastIndex = mInnerList.Count - 1;
    //        T item = mInnerList[lastIndex];
    //        mInnerList.RemoveAt(lastIndex);
    //        return item;
    //    }

    //    public void Clear()
    //    {
    //        mInnerList.Clear();
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        return mInnerList.GetEnumerator();
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return mInnerList.GetEnumerator();
    //    }

    //    public void Add(T item)
    //    {
    //        Push(item);
    //    }

    //    public bool Contains(T item)
    //    {
    //        return mInnerList.Contains(item);
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        mInnerList.CopyTo(array, arrayIndex);
    //    }

    //    public bool Remove(T item)
    //    {
    //        return mInnerList.Remove(item);
    //    }

    //    private int FindPosFor(T item)
    //    {
    //        if (0 == mInnerList.Count)
    //        {
    //            return 0;
    //        }
    //        int cnt = mInnerList.Count;
    //        if (10 > cnt)
    //        {
    //            for (int i = 0; i < cnt; ++i)
    //            {
    //                int cmpResult = item.CompareTo(mInnerList[i]);
    //                if (0 == cmpResult)
    //                {
    //                    return i;
    //                }
    //                else
    //                {
    //                    if (-1 == cmpResult)
    //                    {
    //                        return i;
    //                    }
    //                }
    //            }
    //            return mInnerList.Count;
    //        }
    //        else
    //        {
    //            int start = 0;
    //            //binary search
    //            int end = cnt;
    //            while (start <= end)
    //            {
    //                int middle = start + ((end - start) >> 1);

    //                if (cnt <= middle)
    //                {
    //                    end = middle - 1;
    //                }
    //                else
    //                {
    //                    int cmpResult = item.CompareTo(mInnerList[middle]);
    //                    if (0 == cmpResult)
    //                    {
    //                        return middle;
    //                    }
    //                    if (1 == cmpResult)
    //                    {
    //                        start = middle + 1;
    //                    }
    //                    else
    //                    {
    //                        end = middle - 1;
    //                    }
    //                }
    //            }
    //            return start;
    //        }
    //    }
    //}

    //class AiPlayerPlan
    //{

    //}
}
